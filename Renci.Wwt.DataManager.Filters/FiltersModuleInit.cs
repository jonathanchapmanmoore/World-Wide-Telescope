using System;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Events;
using Renci.Wwt.DataManager.Common;
using Renci.Wwt.DataManager.Filters.Views;
using Renci.Wwt.DataManager.Common.Events;
using Renci.Wwt.DataManager.Filters.Models;
using Renci.Wwt.DataManager.Filters.ViewModels;
using Renci.Wwt.DataManager.Common.Services;
using Renci.Wwt.DataManager.Common.ViewModels;

namespace Renci.Wwt.DataManager.Filters
{
    public class FiltersModuleInit : IModule
    {
        private IRegionManager _regionManager;

        private IUnityContainer _container;

        private IApplicationService _applicationService;

        private IEventAggregator _eventAggregator;

        private object _activeView;

        public FiltersModuleInit(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, IApplicationService applicationService)
        {
            this._container = container;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._applicationService = applicationService;
        }

        #region IModule Members

        public void Initialize()
        {
            this._regionManager.AddToRegion(RegionNames.NewDataFilterButtonsRegion, new AddFilterRibbonMenuItemView(new AddFilterRibbonMenuItemViewModel("Bounding Box", "Filters results based on specified bounding box.", () =>
            {
                this._applicationService.CurrentWorkDocument.AddDataSourceFilter(new BoundBoxFilterInfo(Guid.NewGuid(), string.Format("New Bounding Box Filter")));
            })));

            this._regionManager.AddToRegion(RegionNames.NewDataFilterButtonsRegion, new AddFilterRibbonMenuItemView(new AddFilterRibbonMenuItemViewModel("Bounding Circle", "Filters results based on specified bounding circle.", () =>
            {
                this._applicationService.CurrentWorkDocument.AddDataSourceFilter(new BoundCircleFilterInfo(Guid.NewGuid(), string.Format("New Bounding Circle Filter")));
            })));

            this._eventAggregator.GetEvent<FilterInfoSelectedEvent>().Subscribe((filterInfo) =>
            {
                if (filterInfo is BoundBoxFilterInfo)
                {
                    var viewName = string.Format("BoundBoxWorkspaceView-{0}", filterInfo.ID);

                    this._activeView = this._regionManager.Regions[RegionNames.WorkspaceRegion].GetView(viewName);
                    if (this._activeView == null)
                    {
                        this._activeView = new BoundBoxFilterWorkspaceView(new FilterViewModel(filterInfo));
                        this._regionManager.Regions[RegionNames.WorkspaceRegion].Add(this._activeView, viewName);
                    }
                    this._regionManager.Regions[RegionNames.WorkspaceRegion].Activate(this._activeView);
                } 
                else if (filterInfo is BoundCircleFilterInfo)
                {
                    var viewName = string.Format("BoundCircleWorkspaceView-{0}", filterInfo.ID);

                    this._activeView = this._regionManager.Regions[RegionNames.WorkspaceRegion].GetView(viewName);

                    if (this._activeView == null)
                    {
                        this._activeView = new BoundCircleFilterWorkspaceView(new FilterViewModel(filterInfo));
                        this._regionManager.Regions[RegionNames.WorkspaceRegion].Add(this._activeView, viewName);
                    }
                    this._regionManager.Regions[RegionNames.WorkspaceRegion].Activate(this._activeView);
                }
                else if (filterInfo == null && this._activeView != null)
                {
                    this._regionManager.Regions[RegionNames.WorkspaceRegion].Deactivate(this._activeView);
                }

            }, true);
        }

        #endregion
    }
}
