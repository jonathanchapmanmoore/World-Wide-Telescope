using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Regions;
using System.Windows;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Renci.Wwt.DataManager.Common.Framework;
using Renci.Wwt.DataManager.Common;
using Renci.Wwt.DataManager.ViewModels;
using Renci.Wwt.DataManager.Views;
using Renci.Wwt.DataManager.Adaptors;
using Renci.Wwt.DataManager.Services;
using Renci.Wwt.DataManager.Common.Models;
using Renci.Wwt.DataManager.Common.Services;
using Microsoft.Windows.Controls.Ribbon;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Events;

namespace Renci.Wwt.DataManager
{
    public class Bootstrapper : UnityBootstrapper
    {
        #region Method Overrides

        /// <summary>
        /// Populates the Module Catalog.
        /// </summary>
        /// <returns>A new Module Catalog.</returns>
        /// <remarks>
        /// This method uses the Module Discovery method of populating the Module Catalog. It requires
        /// a post-build event in each module to place the module assembly in the module catalog
        /// directory.
        /// </remarks>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            var moduleCatalog = new DirectoryModuleCatalog();
            moduleCatalog.ModulePath = @".\Modules";
            //moduleCatalog.ModulePath = @".";
            return moduleCatalog;
        }

        /// <summary>
        /// Configures the default region adapter mappings to use in the application, in order 
        /// to adapt UI controls defined in XAML to use a region and register it automatically.
        /// </summary>
        /// <returns>The RegionAdapterMappings instance containing all the mappings.</returns>
        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            // Call base method
            var mappings = base.ConfigureRegionAdapterMappings();
            if (mappings == null)
                return null;

            // Add custom mappings
            var ribbonRegionAdapter = ServiceLocator.Current.GetInstance<RibbonRegionAdapter>();
            mappings.RegisterMapping(typeof(Ribbon), ribbonRegionAdapter);

            var tabRegionAdapter = ServiceLocator.Current.GetInstance<TabRegionAdapter>();
            mappings.RegisterMapping(typeof(TabControl), tabRegionAdapter);

            // Set return value
            return mappings;
        }

        /// <summary>
        /// Instantiates the Shell window.
        /// </summary>
        /// <returns>A new ShellWindow window.</returns>
        protected override DependencyObject CreateShell()
        {
            ///* This method sets the UnityBootstrapper.Shell property to the ShellWindow
            // * we declared elsewhere in this project. Note that the UnityBootstrapper base 
            // * class will attach an instance of the RegionManager to the new Shell window. */

            this.Container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<IApplicationService, ApplicationService>(new ContainerControlledLifetimeManager());

            // Use the container to create an instance of the shell.
            ShellWindow view = this.Container.TryResolve<ShellWindow>();

            return view;

        }

        /// <summary>
        /// Displays the Shell window to the user.
        /// </summary>
        protected override void InitializeShell()
        {
            base.InitializeShell();

            var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

            regionManager.RegisterViewWithRegion(RegionNames.DataSourceInfoListViewRegion,
                                           () => this.Container.Resolve<DataSourceInfoListView>());

            regionManager.RegisterViewWithRegion(RegionNames.DataSourceFilterListViewRegion,
                                           () => this.Container.Resolve<FilterListView>());

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();

        }

        protected override ILoggerFacade CreateLogger()
        {
            return new Logger();
        }

        #endregion
    }
}
