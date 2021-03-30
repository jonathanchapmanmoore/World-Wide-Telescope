using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.ViewModels;
using Renci.Wwt.DataManager.NetCDF.Models;

namespace Renci.Wwt.DataManager.NetCDF.ViewModels
{
    public class ShapeDataSourceInfoViewModel : DataSourceInfoViewModel
    {
        private readonly ShapeDataSourceInfo _dataSourceInfo;

        public ShapeDataSourceInfoViewModel(ShapeDataSourceInfo dataSourceInfo)
            : base(dataSourceInfo)
        {
            this._dataSourceInfo = dataSourceInfo;
        }
    }
}
