using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Regions;
using Renci.Wwt.DataManager.Common.Framework;
using Renci.Wwt.DataManager.Common.Models;
using Renci.Wwt.DataManager.Sos.Models;
using Renci.Wwt.DataManager.Common;
using System.Threading;
using Renci.Wwt.DataManager.Common.Services;
using Renci.Wwt.DataManager.Sos.Views;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Events;
using Renci.Wwt.DataManager.Common.Events;
using Renci.Wwt.DataManager.Sos.ViewModels;
using System.Windows.Controls;

namespace Renci.Wwt.DataManager.Sos
{
    public class SosModuleInit : IModule
    {
        private IRegionManager _regionManager;

        private IUnityContainer _container;
        
        private IApplicationService _applicationService;

        private IEventAggregator _eventAggregator;

        private UserControl _activeView;

        public SosModuleInit(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, IApplicationService applicationService)
        {
            this._container = container;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._applicationService = applicationService;
        }

        #region IModule Members

        public void Initialize()
        {
            this._regionManager.AddToRegion(RegionNames.NewDataSourceButtonsRegion, this._container.Resolve<AddSosRibbonMenuItemView>());

            this._applicationService.RegisterViewModel<SosDataSourceInfo>((item) => { return new SosDataSourceInfoViewModel(item as SosDataSourceInfo); });

            this._eventAggregator.GetEvent<DataSourceInfoSelectedEvent>().Subscribe((dataSourceInfoViewModel) =>
            {
                if (dataSourceInfoViewModel is SosDataSourceInfoViewModel)
                {
                    var viewName = string.Format("NetCDFWorkspaceView-{0}", dataSourceInfoViewModel.DataSourceInfo.ID);

                    this._activeView = this._regionManager.Regions[RegionNames.WorkspaceRegion].GetView(viewName) as UserControl;
                    if (this._activeView == null)
                    {
                        this._activeView = new SosWorkspaceView(dataSourceInfoViewModel as SosDataSourceInfoViewModel);
                        this._regionManager.Regions[RegionNames.WorkspaceRegion].Add(this._activeView, viewName);
                    }
                    this._regionManager.Regions[RegionNames.WorkspaceRegion].Activate(this._activeView);
                }
                else if (dataSourceInfoViewModel == null && this._activeView != null)
                {
                    this._regionManager.Regions[RegionNames.WorkspaceRegion].Deactivate(this._activeView);
                }

            }, true);
        }

        #endregion
    }
}
