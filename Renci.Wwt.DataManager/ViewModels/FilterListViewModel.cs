using System;
using System.Linq;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Microsoft.Practices.Prism.Events;
using Renci.Wwt.DataManager.Common.Services;
using System.Windows.Data;
using Renci.Wwt.DataManager.Common;
using Renci.Wwt.DataManager.Common.Models;
using System.Windows.Input;
using Renci.Wwt.DataManager.Common.Events;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Renci.Wwt.DataManager.ViewModels
{
    public class FilterListViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IApplicationService _applicationService;

        public ListCollectionView FiltersList { get; private set; }

        public ICommand SelectFilterCommand
        {
            get { return new RelayCommand<FilterInfo>((dsi) => this.SelectDataSourceInfo(dsi), (dsi) => this.CanSelectDataSourceInfo(dsi)); }
        }

        public ICommand DeleteFilterCommand
        {
            get { return new RelayCommand<FilterInfo>((dsi) => this.DeleteDataSourceInfo(dsi), (dsi) => this.CanDeleteDataSourceInfo(dsi)); }
        }

        public ICommand MoveUpFilterCommand
        {
            get { return new RelayCommand<FilterInfo>((dsi) => this.MoveUpDataSourceInfo(dsi), (dsi) => this.CanMoveUpDataSourceInfo(dsi)); }
        }

        public ICommand MoveDownFilterCommand
        {
            get { return new RelayCommand<FilterInfo>((dsi) => this.MoveDownDataSourceInfo(dsi), (dsi) => this.CanMoveDownDataSourceInfo(dsi)); }
        }

        public FilterListViewModel(IApplicationService applicationService, IEventAggregator eventAggregator)
        {
            if (applicationService == null)
                throw new ArgumentNullException("dataService");
            if (eventAggregator == null)
                throw new ArgumentNullException("eventAggregator");

            this._eventAggregator = eventAggregator;
            this._applicationService = applicationService;

            this._eventAggregator.GetEvent<WorkDocumentChangedEvent>().Subscribe((workDocument) =>
            {
                //  Refresh when new document selected
                if (workDocument == null)
                {
                    this.FiltersList = null;
                }
                else
                {
                    workDocument.DataSources.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
                    {
                        this.NotifyOfPropertyChange(() => this.FiltersList);
                    };

                    this.FiltersList = new ListCollectionView(workDocument.DataFilters);
                    this.FiltersList.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));

                }

                this.NotifyOfPropertyChange(() => this.FiltersList);
            });
        }

        private void SelectDataSourceInfo(FilterInfo dataSourceInfo)
        {
            this._eventAggregator.GetEvent<FilterInfoSelectedEvent>().Publish(dataSourceInfo);
        }

        private bool CanSelectDataSourceInfo(FilterInfo dataSourceInfo)
        {
            return true;
        }

        private void DeleteDataSourceInfo(FilterInfo dataSourceFilter)
        {
            this._applicationService.CurrentWorkDocument.RemoveDataSourceFilter(dataSourceFilter);
        }

        private bool CanDeleteDataSourceInfo(FilterInfo dataSourceFilter)
        {
            return dataSourceFilter != null;
        }

        private bool CanMoveUpDataSourceInfo(FilterInfo dataSourceFilter)
        {
            if (this.FiltersList == null)
                return false;

            return this.FiltersList.CurrentPosition > 0;
        }

        private void MoveUpDataSourceInfo(FilterInfo dataSourceFilter)
        {
            var prevItem = this.FiltersList.CurrentItem as FilterInfo;
            this.FiltersList.MoveCurrentToPrevious();
            var nextItem = this.FiltersList.CurrentItem as FilterInfo;

            var prevIndex = prevItem.Index;
            var nextIndex = nextItem.Index;

            this.FiltersList.EditItem(prevItem);
            prevItem.Index = nextIndex;
            this.FiltersList.CommitEdit();
            this.FiltersList.EditItem(nextItem);
            nextItem.Index = prevIndex;
            this.FiltersList.CommitEdit();
            this.FiltersList.MoveCurrentTo(prevItem);

        }

        private bool CanMoveDownDataSourceInfo(FilterInfo dataSourceFilter)
        {
            if (this.FiltersList == null)
                return false;

            return this.FiltersList.CurrentPosition < this.FiltersList.Count - 1;
        }

        private void MoveDownDataSourceInfo(FilterInfo dataSourceFilter)
        {
            var prevItem = this.FiltersList.CurrentItem as FilterInfo;
            this.FiltersList.MoveCurrentToNext();
            var nextItem = this.FiltersList.CurrentItem as FilterInfo;

            var prevIndex = prevItem.Index;
            var nextIndex = nextItem.Index;

            this.FiltersList.EditItem(prevItem);
            prevItem.Index = nextIndex;
            this.FiltersList.CommitEdit();
            this.FiltersList.EditItem(nextItem);
            nextItem.Index = prevIndex;
            this.FiltersList.CommitEdit();
            this.FiltersList.MoveCurrentTo(prevItem);
        }

    }
}
