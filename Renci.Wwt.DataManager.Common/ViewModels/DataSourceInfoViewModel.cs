using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Renci.Wwt.DataManager.Common.Models;
using System.ComponentModel;
using Renci.Wwt.DataManager.Common.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;
using Renci.Wwt.DataManager.Common.Services;
using System.Windows.Input;
using System.Collections.Specialized;

namespace Renci.Wwt.DataManager.Common.ViewModels
{
    public abstract class DataSourceInfoViewModel : ViewModelBase
    {
        private IApplicationService _applicationService;

        private IEventAggregator _eventAggregator;

        public DataSourceInfo DataSourceInfo { get; private set; }

        public ObservableCollection<FilterInfo> Filters { get; private set; }

        public ObservableCollection<DataSourceFilterViewModel> DataSourceFilters { get; private set; }
        
        public ICommand AddDataSourceFilterCommand
        {
            get { return new RelayCommand<FilterInfo>((filter) => this.AddDataSourceFilter(filter), (filter) => this.CanAddDataSourceFilter(filter)); }
        }

        public ICommand SelectDataSourceInfoCommand
        {
            get { return new RelayCommand<DataSourceInfoViewModel>((dataSourceInfoViewModel) => this.SelectDataSourceInfo(dataSourceInfoViewModel), (dataSourceInfoViewModel) => this.CanSelectDataSourceInfo(dataSourceInfoViewModel)); }
        }
        
        public DataSourceInfoViewModel(DataSourceInfo dataSourceInfo)
        {
            if (dataSourceInfo == null)
            {
                throw new ArgumentNullException("dataSourceInfo");
            }

            this._applicationService = ServiceLocator.Current.GetInstance<IApplicationService>();

            this._eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            
            this.DataSourceInfo = dataSourceInfo;

            this.DataSourceFilters = new ObservableCollection<DataSourceFilterViewModel>(from item in dataSourceInfo.Filters select new DataSourceFilterViewModel(this.DataSourceInfo, item));

            dataSourceInfo.Filters.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
            {
                this.DataSourceFilters = new ObservableCollection<DataSourceFilterViewModel>(from item in dataSourceInfo.Filters select new DataSourceFilterViewModel(this.DataSourceInfo, item));
                this.NotifyOfPropertyChange(() => this.DataSourceFilters);
            };

            this.Filters = this._applicationService.CurrentWorkDocument.DataFilters;
        }

        private bool CanAddDataSourceFilter(FilterInfo filter)
        {
            return filter != null;
        }

        private void AddDataSourceFilter(FilterInfo filter)
        {
            this.DataSourceInfo.Filters.Add(new DataSourceFilter(filter));
        }

        private bool CanSelectDataSourceInfo(DataSourceInfoViewModel dataSourceInfoViewModel)
        {
            return dataSourceInfoViewModel != null;
        }

        private void SelectDataSourceInfo(DataSourceInfoViewModel dataSourceInfoViewModel)
        {
            this._eventAggregator.GetEvent<DataSourceInfoSelectedEvent>().Publish(dataSourceInfoViewModel);
        }
    }
}
