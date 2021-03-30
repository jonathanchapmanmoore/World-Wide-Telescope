using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.Models;
using Renci.Wwt.DataManager.Common.BaseClasses;

namespace Renci.Wwt.DataManager.Common.Services
{
    public interface IApplicationService
    {
        WorkDocument CurrentWorkDocument { get; set; }

        void RegisterViewModel<T>(Func<ModelBase, ViewModelBase> resolver) where T : ModelBase;

        ViewModelBase ResolveViewModel(ModelBase dataItem);
    }
}
