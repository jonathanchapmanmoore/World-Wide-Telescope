using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renci.Wwt.DataManager.Common.BaseClasses
{
    public abstract class DataProviderBase
    {
        public abstract string Name { get; }
        
        public abstract string DisplayName { get; }
    }
}
