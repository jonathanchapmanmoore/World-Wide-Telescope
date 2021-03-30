using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Regions;
using Renci.Wwt.DataManager.Common.Framework;
using Microsoft.Practices.Unity;
using Renci.Wwt.DataManager.Common;
using Renci.Wwt.DataManager.NetCDF.Models;
using System.Threading;
using Renci.Wwt.DataManager.Common.Models;
using Renci.Wwt.DataManager.Common.Services;
using Renci.Wwt.DataManager.NetCDF.Views;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using Renci.Wwt.DataManager.Common.Events;
using Microsoft.Practices.Prism.Events;
using System.Diagnostics;
using Renci.Wwt.DataManager.NetCDF.ViewModels;
using System.Windows.Controls;

namespace Renci.Wwt.DataManager.NetCDF
{
    public class NetCDFModuleInit : IModule
    {
        private IRegionManager _regionManager;

        private IUnityContainer _container;

        private IApplicationService _applicationService;

        private IEventAggregator _eventAggregator;

        private UserControl _activeView;

        public NetCDFModuleInit(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, IApplicationService applicationService)
        {
            this._container = container;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._applicationService = applicationService;
        }

        #region IModule Members

        public void Initialize()
        {
            this._regionManager.AddToRegion(RegionNames.NewDataSourceButtonsRegion, new AddNETCDFRibbonMenuItemView(new AddNETCDFRibbonMenuItemViewModel("Generic NET CDF", "Produces single color output.", () => 
            {
                this._applicationService.CurrentWorkDocument.AddDataSourceInfo(new GenericNetCDFDataSourceInfo(Guid.NewGuid(), string.Format("New Generic NET CDF Data Source")));
            })));

            this._regionManager.AddToRegion(RegionNames.NewDataSourceButtonsRegion, new AddNETCDFRibbonMenuItemView(new AddNETCDFRibbonMenuItemViewModel("Variable NET CDF", "Produces multiple color output based on variable.", () =>
            {
                this._applicationService.CurrentWorkDocument.AddDataSourceInfo(new VariableNetCDFDataSourceInfo(Guid.NewGuid(), string.Format("New Variable NET CDF Data Source")));
            })));

            this._regionManager.AddToRegion(RegionNames.NewDataSourceButtonsRegion, new AddNETCDFRibbonMenuItemView(new AddNETCDFRibbonMenuItemViewModel("Geography Text", "Produces output based on text.", () =>
            {
                this._applicationService.CurrentWorkDocument.AddDataSourceInfo(new TextDataSourceInfo(Guid.NewGuid(), string.Format("Text Data Source")));
            })));

            this._regionManager.AddToRegion(RegionNames.NewDataSourceButtonsRegion, new AddNETCDFRibbonMenuItemView(new AddNETCDFRibbonMenuItemViewModel("Shape File", "Produces output based shape file.", () =>
            {
                this._applicationService.CurrentWorkDocument.AddDataSourceInfo(new ShapeDataSourceInfo(Guid.NewGuid(), string.Format("Shape File Data Source")));
            })));

            this._applicationService.RegisterViewModel<GenericNetCDFDataSourceInfo>((item) => { return new NetCDFDataSourceInfoViewModel(item as NetCDFDataSourceInfo); });
            this._applicationService.RegisterViewModel<VariableNetCDFDataSourceInfo>((item) => { return new NetCDFDataSourceInfoViewModel(item as NetCDFDataSourceInfo); });
            this._applicationService.RegisterViewModel<TextDataSourceInfo>((item) => { return new TextDataSourceInfoViewModel(item as TextDataSourceInfo); });
            this._applicationService.RegisterViewModel<ShapeDataSourceInfo>((item) => { return new ShapeDataSourceInfoViewModel(item as ShapeDataSourceInfo); });

            this._eventAggregator.GetEvent<DataSourceInfoSelectedEvent>().Subscribe((dataSourceInfoViewModel) =>
            {
                if (dataSourceInfoViewModel is NetCDFDataSourceInfoViewModel)
                {
                    var viewName = string.Format("NetCDFWorkspaceView-{0}", dataSourceInfoViewModel.DataSourceInfo.ID);

                    this._activeView = this._regionManager.Regions[RegionNames.WorkspaceRegion].GetView(viewName) as UserControl;
                    if (this._activeView == null)
                    {
                        this._activeView = new NetCDFWorkspaceView(dataSourceInfoViewModel);
                        this._regionManager.Regions[RegionNames.WorkspaceRegion].Add(this._activeView, viewName);
                    }
                    this._regionManager.Regions[RegionNames.WorkspaceRegion].Activate(this._activeView);
                }
                else if (dataSourceInfoViewModel is TextDataSourceInfoViewModel)
                {
                    var viewName = string.Format("TextWorkspaceView-{0}", dataSourceInfoViewModel.DataSourceInfo.ID);

                    this._activeView = this._regionManager.Regions[RegionNames.WorkspaceRegion].GetView(viewName) as UserControl;
                    if (this._activeView == null)
                    {
                        this._activeView = new TextWorkspaceView(dataSourceInfoViewModel);
                        this._regionManager.Regions[RegionNames.WorkspaceRegion].Add(this._activeView, viewName);
                    }
                    this._regionManager.Regions[RegionNames.WorkspaceRegion].Activate(this._activeView);
                }
                else if (dataSourceInfoViewModel is ShapeDataSourceInfoViewModel)
                {
                    var viewName = string.Format("ShapeWorkspaceView-{0}", dataSourceInfoViewModel.DataSourceInfo.ID);

                    this._activeView = this._regionManager.Regions[RegionNames.WorkspaceRegion].GetView(viewName) as UserControl;
                    if (this._activeView == null)
                    {
                        this._activeView = new ShapeWorkspaceView(dataSourceInfoViewModel);
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