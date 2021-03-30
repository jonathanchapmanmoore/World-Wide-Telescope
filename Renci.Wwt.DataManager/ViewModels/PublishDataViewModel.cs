using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Renci.Wwt.DataManager.Common.Services;
using System.Windows.Input;
using Renci.Wwt.DataManager.Common;
using System.Threading;
using System.Threading.Tasks;
using Renci.Wwt.Core;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.IO;
using Renci.Wwt.Core.Frames.Layers;
using Renci.Wwt.DataManager.Common.Models;
using System.Windows.Media;

namespace Renci.Wwt.DataManager.ViewModels
{
    public class PublishDataViewModel : ViewModelBase
    {
        private IList<string> _layerNames;

        private readonly IApplicationService _applicationService;

        private readonly IDialogService _dialogService;

        public ICommand SummaryCommand
        {
            get { return new RelayCommand(() => this.Summarize()); }
        }

        public ICommand AnalyzeCommand
        {
            get { return new RelayCommand(() => this.Analyze()); }
        }

        public ICommand SelectCsvFileCommand
        {
            get { return new RelayCommand(() => this.SelectCsvFile()); }
        }

        public ICommand PublishCommand
        {
            get { return new RelayCommand(() => this.Publish()); }
        }

        public ICommand PublishCompletedCommand
        {
            get { return new RelayCommand(() => this.PublishCompleted()); }
        }

        public ICommand FinishWizardCommand
        {
            get { return new RelayCommand(() => this.FinishWizard()); }
        }

        public string ExportPathName
        {
            get
            {
                return this.Get<string>(() => this.ExportPathName);
            }
            set
            {
                this.Set(() => this.ExportPathName, value);
            }
        }

        public bool IsAnalizesFinished
        {
            get
            {
                return this.Get<bool>(() => this.IsAnalizesFinished);
            }
            set
            {
                this.Set(() => this.IsAnalizesFinished, value);
            }
        }

        public bool IsPublishFinished
        {
            get
            {
                return this.Get<bool>(() => this.IsPublishFinished);
            }
            set
            {
                this.Set(() => this.IsPublishFinished, value);
            }
        }

        public bool IsMergedLayers
        {
            get
            {
                return this.Get<bool>(() => this.IsMergedLayers);
            }
            set
            {
                this.Set(() => this.IsMergedLayers, value);
            }
        }

        public string MergedLayerName
        {
            get
            {
                return this.Get<string>(() => this.MergedLayerName);
            }
            set
            {
                this.Set(() => this.MergedLayerName, value);
            }
        }

        public string AlertColor
        {
            get
            {
                return this.Get<string>(() => this.AlertColor);
            }
            set
            {
                this.Set(() => this.AlertColor, value);
            }
        }

        public string AlertText
        {
            get
            {
                return this.Get<string>(() => this.AlertText);
            }
            set
            {
                this.Set(() => this.AlertText, value);
            }
        }

        public ObservableCollectionEx<string> ProgressLog { get; private set; }

        public PublishDataViewModel(IApplicationService applicationService, IDialogService dialogService)
        {
            this._applicationService = applicationService;
            this._dialogService = dialogService;

            this.ProgressLog = new ObservableCollectionEx<string>();

            this.AlertColor = "Blue";
            this.AlertText = "Not analyzed";

            this.MergedLayerName = string.Format("Merged {0:hh}:{0:mm}", DateTime.Now);

            this._layerNames = new List<string>();
        }

        private void FinishWizard()
        {
        }

        private void Summarize()
        {
            if (this._applicationService.CurrentWorkDocument != null)
            {
                this.ProgressLog.Clear();


                foreach (var dataSource in this.GetActiveDataSources())
                {
                    if (dataSource.Filters.Count < 1)
                    {
                        Log(string.Format("{0}", dataSource.Name));
                    }
                    else
                    {
                        foreach (var filter in dataSource.Filters.Where((f) => f.Enabled))
                        {
                            Log(string.Format("{0} ({1})", dataSource.Name, filter.FilterInfo.Name));
                        }
                    }
                }
            }
        }

        private void Analyze()
        {
            this.ProgressLog.Clear();

            var task = Task.Factory.StartNew(() =>
            {
                Log("Started");

                if (this._applicationService.CurrentWorkDocument != null)
                {
                    Log("Calculating total.");

                    var total = 0;

                    Parallel.ForEach(this.GetActiveDataSources(), dataSource =>
                    {
                        Log(string.Format("Processing '{0}' data source.", dataSource.Name));
                        var count = dataSource.GetDataCount();

                        Log(string.Format("Data source '{0}' has {1} items.", dataSource.Name, count));

                        Interlocked.Add(ref total, count);
                    });

                    Log(string.Format("Total {0} items.", total));

                    if (total > 600000)
                    {
                        this.AlertColor = "Red";
                        this.AlertText = "Will be a problem";
                    }
                    else if (total > 300000)
                    {
                        this.AlertColor = "Yellow";
                        this.AlertText = "Could be a problem";
                    }
                    else
                    {
                        this.AlertColor = "Green";
                        this.AlertText = "Will not be a problem";
                    }

                }

                Log("Finished.");

                this.IsAnalizesFinished = true;
            });
        }

        private void Log(string text)
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(() => { this.ProgressLog.Add(text); }));
        }

        private void SelectCsvFile()
        {
            var result = this._dialogService.ShowSaveFileDialog(this, new SaveFileDialogOption()
            {
                OverwritePrompt = true,
                DefaultExt = "csv",
                Filter = "Comma Separated Value (CSV) Files (*.csv)|*.csv|All files (*.*)|*.*",
            });

            if (result != null)
            {
                this.ExportPathName = result.FileNames.First();
            }
        }

        private void Publish()
        {
            this.ProgressLog.Clear();

            var task = Task.Factory.StartNew(() =>
            {
                Log("Started");

                this.IsPublishFinished = false;

                if (this._applicationService.CurrentWorkDocument != null)
                {
                    var wwt = new WwtClient();
                    this._layerNames.Clear();

                    SpreadsheetLayer layer = null;

                    if (this.IsMergedLayers)
                    {
                        layer = new SpreadsheetLayer(wwt, wwt.Earth, this.MergedLayerName);
                        this._layerNames.Add(layer.Name);

                        Log(string.Format("Publishing all data items into '{0}' layer.", layer.Name));

                    }

                    //Parallel.ForEach(activeDataSources, dataSource =>
                    foreach (var dataSource in this.GetActiveDataSources())
                    {
                        Log(string.Format("Processing '{0}' data source.", dataSource.Name));
                        var data = dataSource.GetData();

                        if (layer == null)
                        {
                            layer = new SpreadsheetLayer(wwt, wwt.Earth, dataSource.Name);
                            layer.AltUnit = Core.Frames.Layers.Spreadsheet.AltitudeUnits.Meters;
                            this._layerNames.Add(layer.Name);

                            Log(string.Format("Publishing {0} data items into '{1}' layer.", data.Count(), layer.Name));

                            layer.AddData(data);

                            layer = null;
                        }
                        else
                        {
                            layer.AddData(data);
                        }
                    }
                }

                Log("Finished.");

                this.IsPublishFinished = true;
            });
        }

        private void PublishCompleted()
        {
            this.ProgressLog.Clear();
            foreach (var layerName in this._layerNames)
            {
                this.ProgressLog.Add(layerName);
            }
        }

        private IEnumerable<DataSourceInfo> GetActiveDataSources()
        {
            var activeDataSources = (from ds in this._applicationService.CurrentWorkDocument.DataSources
                                     from f in ds.Filters
                                     where f.Enabled == true && ds.Enabled == true
                                     select ds).Concat(from ds in this._applicationService.CurrentWorkDocument.DataSources
                                                       where ds.Filters.Count < 1 && ds.Enabled == true
                                                       select ds);
            return activeDataSources.Distinct();
        }
    }
}
