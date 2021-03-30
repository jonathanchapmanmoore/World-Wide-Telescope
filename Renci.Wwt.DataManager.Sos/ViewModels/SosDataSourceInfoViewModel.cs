using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.ViewModels;
using System.ComponentModel;
using Renci.Wwt.DataManager.Sos.Models;

namespace Renci.Wwt.DataManager.Sos.ViewModels
{
    public class SosDataSourceInfoViewModel : DataSourceInfoViewModel
    {
        private readonly SosDataSourceInfo _dataSourceInfo;

        [CategoryAttribute("SOS"),
        DisplayName("Url"),
        DescriptionAttribute("SOS web service url.")]
        public string Url
        {
            get
            {
                return this._dataSourceInfo.Url;
            }
            set
            {
                this._dataSourceInfo.Url = value;
                this.NotifyOfPropertyChange(() => this.Url);
            }
        }

        public SosDataSourceInfoViewModel(SosDataSourceInfo dataSourceInfo)
            : base(dataSourceInfo)
        {
            this._dataSourceInfo = dataSourceInfo;
        }
    }
}
