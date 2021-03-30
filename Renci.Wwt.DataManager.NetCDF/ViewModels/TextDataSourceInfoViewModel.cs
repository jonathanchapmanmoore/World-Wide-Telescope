using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.ViewModels;
using Renci.Wwt.DataManager.NetCDF.Models;

namespace Renci.Wwt.DataManager.NetCDF.ViewModels
{
    public class TextDataSourceInfoViewModel : DataSourceInfoViewModel
    {
        private readonly TextDataSourceInfo _dataSourceInfo;

        public TextDataSourceInfoViewModel(TextDataSourceInfo dataSourceInfo)
            : base(dataSourceInfo)
        {
            this._dataSourceInfo = dataSourceInfo;
        }
    }
}
