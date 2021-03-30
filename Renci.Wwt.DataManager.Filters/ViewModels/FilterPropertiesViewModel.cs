using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Renci.Wwt.DataManager.Filters.Models;
using Renci.Wwt.DataManager.Common.Models;

namespace Renci.Wwt.DataManager.Filters.ViewModels
{
    public class FilterPropertiesViewModel : ViewModelBase
    {
        public FilterInfo DataSourceFilter { get; private set; }

        public FilterPropertiesViewModel(FilterInfo dataSourceFilter)
        {
            this.DataSourceFilter = dataSourceFilter;
        }
    }
}
