using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Renci.Wwt.DataManager.Common.Models;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Renci.Wwt.DataManager.Common.Events;

namespace Renci.Wwt.DataManager.Common.ViewModels
{
    public class FilterViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;

        public FilterInfo Filter { get; private set; }

        public ICommand DisplayPropertiesCommand
        {
            get { return new RelayCommand<FilterInfo>((filterInfo) => this.DisplayProperties(filterInfo), (filterInfo) => this.CanDisplayProperties(filterInfo)); }
        }

        public FilterViewModel(FilterInfo dataSourceFilter)
        {
            if (dataSourceFilter == null)
            {
                throw new ArgumentNullException("dataSourceFilter");
            }

            this._eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            this.Filter = dataSourceFilter;
        }

        private bool CanDisplayProperties(FilterInfo filterInfo)
        {
            return filterInfo != null;
        }

        private void DisplayProperties(FilterInfo filterInfo)
        {
            this._eventAggregator.GetEvent<FilterInfoSelectedEvent>().Publish(filterInfo);
        }
    }
}
