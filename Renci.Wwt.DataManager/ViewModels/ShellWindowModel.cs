using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Input;
using Renci.Wwt.DataManager.Common;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Renci.Wwt.DataManager.Common.Events;
using Renci.Wwt.DataManager.Common.Models;
using Renci.Wwt.DataManager.Services;
using System.IO;
using Microsoft.Practices.Unity;
using System.Xml.Linq;
using Renci.Wwt.DataManager.Common.Services;
using Renci.Wwt.DataManager.Views;

namespace Renci.Wwt.DataManager.ViewModels
{
    public class ShellWindowModel : ViewModelBase
    {
        private readonly IUnityContainer _container;

        private IApplicationService _applicationService;
        public IApplicationService ApplicationService
        {
            get
            {
                if (this._applicationService == null)
                {
                    this._applicationService = ServiceLocator.Current.GetInstance<IApplicationService>();
                }
                return this._applicationService;
            }
        }

        private IDialogService _dialogService;
        public IDialogService DialogService
        {
            get
            {
                if (this._dialogService == null)
                {
                    this._dialogService = ServiceLocator.Current.GetInstance<IDialogService>();
                }
                return this._dialogService;
            }
        }

        public ICommand AnalyzeCommand
        {
            get { return new DelegateCommand(() => this.Analyze()); }
        }

        public ICommand PublishCommand
        {
            get { return new DelegateCommand(() => this.PublishData()); }
        }
        public ICommand LoadWorkDocumentCommand
        {
            get { return new DelegateCommand(() => this.LoadWorkDocument()); }
        }

        public ICommand SaveWorkDocumentCommand
        {
            get { return new DelegateCommand(() => this.SaveWorkDocument()); }
        }

        public ICommand NewWorkDocumentCommand
        {
            get { return new DelegateCommand(() => this.NewWorkDocument()); }
        }

        public ShellWindowModel(IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("dataService");

            this._container = container;
        }

        private void LoadWorkDocument()
        {
            var result = this.DialogService.ShowOpenFileDialog(this, new OpenFileDialogOption()
            {
                DefaultExt = "xml",
                Filter = "Work Document files (*.xml)|*.xml|All files (*.*)|*.*",
            });
            if (result != null)
            {
                using (var stream = File.OpenRead(result.FileNames.First()))
                {
                    var element = XElement.Load(stream);
                    var document = new WorkDocument(element);
                    this.ApplicationService.CurrentWorkDocument = document;
                }
            }
        }

        private void SaveWorkDocument()
        {
            var result = this.DialogService.ShowSaveFileDialog(this, new SaveFileDialogOption()
            {
                OverwritePrompt = true,
                DefaultExt = "xml",
                Filter = "Work Document files (*.xml)|*.xml|All files (*.*)|*.*",
            });

            if (result != null)
            {

                using (var stream = File.Open(result.FileNames.First(), FileMode.Create))
                {
                    var xml = this.ApplicationService.CurrentWorkDocument.Save();
                    xml.Save(stream);
                }
            }
        }

        private void NewWorkDocument()
        {
            this.ApplicationService.CurrentWorkDocument = new WorkDocument();
        }

        private void Analyze()
        {
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();

            this.DialogService.ShowDialog<AnalyzeDataView>(this, container.Resolve<PublishDataViewModel>());
        }

        private void PublishData()
        {
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();

            this.DialogService.ShowDialog<PublishDataView>(this, container.Resolve<PublishDataViewModel>());
        }
    }
}
