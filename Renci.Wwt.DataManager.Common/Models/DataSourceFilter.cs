using System;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using Renci.Wwt.DataManager.Common.BaseClasses;

namespace Renci.Wwt.DataManager.Common.Models
{
    public class DataSourceFilter : ModelBase
    {
        public FilterInfo FilterInfo { get; private set; }

        public bool Enabled
        {
            get
            {
                return this.Get<bool>(() => this.Enabled);
            }
            set
            {
                this.Set(() => this.Enabled, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceFilter"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sortIndex">Index of the sort.</param>
        /// <param name="name">The name.</param>
        public DataSourceFilter(FilterInfo filterInfo)
        {
            this.FilterInfo = filterInfo;
        }
    }
}
