using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.ViewModels;
using Renci.Wwt.DataManager.NetCDF.Models;
using System.ComponentModel;

namespace Renci.Wwt.DataManager.NetCDF.ViewModels
{
    public class NetCDFDataSourceInfoViewModel : DataSourceInfoViewModel
    {
        private readonly NetCDFDataSourceInfo _dataSourceInfo;

        public NetCDFDataSourceInfoViewModel(NetCDFDataSourceInfo dataSourceInfo)
            : base(dataSourceInfo)
        {
            this._dataSourceInfo = dataSourceInfo;
        }
    }
}
