using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.BaseClasses;
using Renci.Wwt.DataManager.Common.Models;

namespace Renci.Wwt.DataManager.Common.ViewModels
{
    public abstract class FilterInfoViewModel : ViewModelBase
    {
        public FilterInfo FilterInfo { get; private set; }

        public FilterInfoViewModel(FilterInfo filterInfo)
        {
            if (filterInfo == null)
            {
                throw new ArgumentNullException("filterInfo");
            }

            this.FilterInfo = filterInfo;
        }
    }
}
