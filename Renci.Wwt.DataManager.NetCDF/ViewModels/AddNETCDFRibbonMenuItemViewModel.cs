using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.BaseClasses;
using System.Windows.Input;
using Renci.Wwt.DataManager.Common;

namespace Renci.Wwt.DataManager.NetCDF.ViewModels
{
    public class AddNETCDFRibbonMenuItemViewModel : ViewModelBase
    {
        public string Header { get; private set; }

        public string Description { get; private set; }

        public ICommand Command { get; private set; }

        public AddNETCDFRibbonMenuItemViewModel(string header, string description, Action action)
        {
            this.Header = header;
            this.Description = description;
            this.Command = new RelayCommand(() => action());
        }
    }
}
