using System;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Renci.Wwt.DataManager.Common.Services;
using System.Windows.Input;
using Renci.Wwt.DataManager.Common;
using Renci.Wwt.DataManager.Filters.Models;

namespace Renci.Wwt.DataManager.Filters.ViewModels
{
    public class AddFilterRibbonMenuItemViewModel : ViewModelBase
    {
        public string Header { get; private set; }

        public string Description { get; private set; }

        public ICommand Command { get; private set; }

        public AddFilterRibbonMenuItemViewModel(string header, string description, Action action)
        {
            this.Header = header;
            this.Description = description;
            this.Command = new RelayCommand(() => action());
        }
    }
}
