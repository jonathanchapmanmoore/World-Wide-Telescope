using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Renci.Wwt.DataManager.Common.Services;
using System.Windows.Input;
using Renci.Wwt.DataManager.Common;
using Renci.Wwt.DataManager.NetCDF.Models;
using Microsoft.Practices.ServiceLocation;
using Renci.Wwt.DataManager.Common.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Events;

namespace Renci.Wwt.DataManager.NetCDF.ViewModels
{
    public class AddGenericNETCDFRibbonMenuItemViewModel : ViewModelBase
    {
        private readonly IApplicationService _applicationService;

        public string Header { get { return "Generic NET CDF"; } }

        public string Description { get { return "Produces single color output."; } }

        public ICommand Command
        {
            get { return new RelayCommand(() => this.AddDataSourceInfo(), () => { return this.CanAddDataSourceInfo(); }); }
        }

        public AddGenericNETCDFRibbonMenuItemViewModel(IApplicationService applicationService)
        {
            this._applicationService = applicationService;
        }

        private bool CanAddDataSourceInfo()
        {
            return true;
        }

        private void AddDataSourceInfo()
        {
            var dataSourceInfo = new GenericNetCDFDataSourceInfo(Guid.NewGuid(), string.Format("New Generic NET CDF Data Source"));
            this._applicationService.CurrentWorkDocument.AddDataSourceInfo(dataSourceInfo);
        }
    }
}
