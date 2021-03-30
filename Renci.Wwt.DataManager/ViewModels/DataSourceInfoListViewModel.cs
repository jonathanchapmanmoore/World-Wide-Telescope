using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Microsoft.Practices.Prism.Events;
using System.ComponentModel;
using Renci.Wwt.DataManager.Common.Models;
using Renci.Wwt.DataManager.Common.Events;
using System.Windows.Data;
using System.Windows.Input;
using Renci.Wwt.DataManager.Common;
using Microsoft.Practices.Prism.Commands;
using System.Diagnostics;
using Renci.Wwt.DataManager.Common.Services;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Renci.Wwt.DataManager.Common.ViewModels;

namespace Renci.Wwt.DataManager.ViewModels
{
    public class DataSourceInfoListViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IApplicationService _applicationService;
        
        private WorkDocument _workDocument;

        public ObservableCollection<ViewModelBase> DataSourceInfoList { get; private set; }

        public ICommand DeleteDataSourceInfoCommand
        {
            get { return new RelayCommand<ViewModelBase>((dataSourceInfo) => this.DeleteDataSourceInfo(dataSourceInfo), (dataSourceInfo) => this.CanDeleteDataSourceInfo(dataSourceInfo)); }
        }

        public ICommand UpDataSourceInfoCommand
        {
            get { return new RelayCommand<ViewModelBase>((dataSourceInfo) => this.MoveUpDataSourceInfo(dataSourceInfo), (dataSourceInfo) => this.CanMoveUpDataSourceInfo(dataSourceInfo)); }
        }

        public ICommand DownDataSourceInfoCommand
        {
            get { return new RelayCommand<ViewModelBase>((dataSourceInfo) => this.MoveDownDataSourceInfo(dataSourceInfo), (dataSourceInfo) => this.CanMoveDownDataSourceInfo(dataSourceInfo)); }
        }

        public DataSourceInfoListViewModel(IApplicationService applicationService, IEventAggregator eventAggregator)
        {
            if (applicationService == null)
                throw new ArgumentNullException("dataService");
            if (eventAggregator == null)
                throw new ArgumentNullException("eventAggregator");

            this._eventAggregator = eventAggregator;
            this._applicationService = applicationService;

            this._eventAggregator.GetEvent<WorkDocumentChangedEvent>().Subscribe((workDocument) =>
            {
                this._workDocument = workDocument;

                //  Refresh when new document selected
                if (this._workDocument == null)
                {
                    this.DataSourceInfoList = null;

                    this.NotifyOfPropertyChange(() => this.DataSourceInfoList);
                }
                else
                {
                    this._workDocument.DataSources.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
                    {
                        this.RefreshDataSourceInfoList();
                    };

                    this.RefreshDataSourceInfoList();
                }

            });
        }

        private void RefreshDataSourceInfoList()
        {
            if (this._workDocument == null)
            {
                this.DataSourceInfoList = null;
            }
            else 
            {
                this.DataSourceInfoList = new ObservableCollection<ViewModelBase>(from item in this._workDocument.DataSources
                                                                                  orderby item.Index ascending
                                                                                  select this._applicationService.ResolveViewModel(item));
            }

            this.NotifyOfPropertyChange(() => this.DataSourceInfoList);
        }

        private void DeleteDataSourceInfo(ViewModelBase item)
        {
            var dataSourceInfoViewModel = item as DataSourceInfoViewModel;
            if (dataSourceInfoViewModel != null)
            {
                this._applicationService.CurrentWorkDocument.RemoveDataSourceInfo(dataSourceInfoViewModel.DataSourceInfo);
            }

            var dataSourceFilterViewModel = item as DataSourceFilterViewModel;

            if (dataSourceFilterViewModel != null)
            {
                dataSourceFilterViewModel.DataSourceInfo.Filters.Remove(dataSourceFilterViewModel.DataSourceFilter);
            }
        }

        private bool CanDeleteDataSourceInfo(ViewModelBase item)
        {
            if (item is DataSourceInfoViewModel)
                return true;
            if (item is DataSourceFilterViewModel)
                return true;
            return false;
        }

        private bool CanMoveUpDataSourceInfo(ViewModelBase item)
        {
            var dataSourceInfoViewModel = item as DataSourceInfoViewModel;
            if (dataSourceInfoViewModel != null)
            {
                var previouse = (from i in this.DataSourceInfoList.OfType<DataSourceInfoViewModel>()
                                 where i.DataSourceInfo.Index < dataSourceInfoViewModel.DataSourceInfo.Index
                                 orderby i.DataSourceInfo.Index descending
                                 select i.DataSourceInfo).FirstOrDefault();
                return previouse != null;
            }
            return false;
        }

        private void MoveUpDataSourceInfo(ViewModelBase item)
        {
            var dataSourceInfoViewModel = item as DataSourceInfoViewModel;
            if (dataSourceInfoViewModel != null)
            {
                //  Find item with previous index
                var previouse = (from i in this.DataSourceInfoList.OfType<DataSourceInfoViewModel>()
                                 where i.DataSourceInfo.Index < dataSourceInfoViewModel.DataSourceInfo.Index
                                 orderby i.DataSourceInfo.Index descending
                                 select i.DataSourceInfo).FirstOrDefault();

                if (previouse != null)
                {
                    var index = previouse.Index;
                    previouse.Index = dataSourceInfoViewModel.DataSourceInfo.Index;
                    dataSourceInfoViewModel.DataSourceInfo.Index = index;
                    this.RefreshDataSourceInfoList();
                }

            }
        }

        private bool CanMoveDownDataSourceInfo(ViewModelBase item)
        {
            var dataSourceInfoViewModel = item as DataSourceInfoViewModel;
            if (dataSourceInfoViewModel != null)
            {
                var next = (from i in this.DataSourceInfoList.OfType<DataSourceInfoViewModel>()
                                 where i.DataSourceInfo.Index > dataSourceInfoViewModel.DataSourceInfo.Index
                                 orderby i.DataSourceInfo.Index ascending
                                 select i.DataSourceInfo).FirstOrDefault();
                return next != null;
            }
            return false;
        }

        private void MoveDownDataSourceInfo(ViewModelBase item)
        {
            var dataSourceInfoViewModel = item as DataSourceInfoViewModel;
            if (dataSourceInfoViewModel != null)
            {
                //  Find item with previous index
                var next = (from i in this.DataSourceInfoList.OfType<DataSourceInfoViewModel>()
                            where i.DataSourceInfo.Index > dataSourceInfoViewModel.DataSourceInfo.Index
                            orderby i.DataSourceInfo.Index ascending
                            select i.DataSourceInfo).FirstOrDefault();

                if (next != null)
                {
                    var index = next.Index;
                    next.Index = dataSourceInfoViewModel.DataSourceInfo.Index;
                    dataSourceInfoViewModel.DataSourceInfo.Index = index;
                    this.RefreshDataSourceInfoList();
                }
            }
        }
    }
}
