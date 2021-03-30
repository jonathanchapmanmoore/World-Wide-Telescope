using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Renci.Wwt.DataManager.Common.Services;
using System.Windows.Input;
using Renci.Wwt.DataManager.Common;
using Renci.Wwt.DataManager.Sos.Models;

namespace Renci.Wwt.DataManager.Sos.ViewModels
{
    public class AddSosRibbonMenuItemViewModel : ViewModelBase
    {
        private readonly IApplicationService _applicationService;

        public string Header { get { return "SOS"; } }

        public ICommand Command
        {
            get { return new RelayCommand(() => this.AddDataSourceInfo(), () => { return this.CanAddDataSourceInfo(); }); }
        }

        public AddSosRibbonMenuItemViewModel(IApplicationService applicationService)
        {
            this._applicationService = applicationService;
        }

        private bool CanAddDataSourceInfo()
        {
            return true;
        }

        private void AddDataSourceInfo()
        {
            var dataSourceInfo = new SosDataSourceInfo(Guid.NewGuid(), string.Format("New SOS Data Source"));
            this._applicationService.CurrentWorkDocument.AddDataSourceInfo(dataSourceInfo);
        }
    }
}
