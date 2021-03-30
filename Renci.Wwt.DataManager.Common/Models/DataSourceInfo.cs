using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Renci.Wwt.Core;
using System.ComponentModel;
using System.Xml.Linq;
using Renci.Wwt.DataManager.Common.BaseClasses;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Renci.Wwt.DataManager.Common.Models
{
    /// <summary>
    /// Represents base class for different data sources
    /// </summary>
    public abstract class DataSourceInfo : ModelBase
    {
        [CategoryAttribute("Generic"),
        DisplayName("Data Source ID"),
        DescriptionAttribute("Unique data source id assigned by the system and cannot changed.")]
        public Guid ID { get; private set; }

        [CategoryAttribute("Generic"),
        DisplayName("Data Source Name"),
        DescriptionAttribute("Name of data source.")]
        public string Name
        {
            get { return this.Get(() => this.Name); }
            set { this.Set(() => this.Name, value); }
        }

        [CategoryAttribute("Generic"),
        DisplayName("Enabled"),
        DescriptionAttribute("Whether Data Source is enabled for publishing.")]
        public bool Enabled
        {
            get { return this.Get(() => this.Enabled); }
            set { this.Set(() => this.Enabled, value); }
        }

        [Browsable(false)]
        public int Index { get; set; }

        [Browsable(false)]
        public ObservableCollection<DataSourceFilter> Filters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceInfo"/> class.
        /// </summary>
        public DataSourceInfo()
        {
            //  Empty class needed for deserialization
            this.Filters = new ObservableCollection<DataSourceFilter>();
            this.Enabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sortIndex">Index of the sort.</param>
        /// <param name="name">The name.</param>
        public DataSourceInfo(Guid id, string name)
            : this()
        {
            this.ID = id;
            this.Name = name;
            this.Enabled = true;
        }

        public abstract int GetDataCount();

        public abstract IEnumerable<DataItem> GetData();

        public XElement Save()
        {
            return new XElement("dataSource",
                new XAttribute("id", this.ID),
                new XAttribute("name", this.Name),
                new XAttribute("index", this.Index),
                new XAttribute("enabled", this.Enabled),
                new XAttribute("type", this.GetType().AssemblyQualifiedName),
                this.SaveAttributes(),
                new XElement("filters",
                    from filter in this.Filters select new XElement("filter",
                        new XAttribute("id", filter.FilterInfo.ID),
                        new XAttribute("enabled", filter.Enabled)
                        )
                    )
                );
        }

        public void Load(WorkDocument document, XElement element)
        {
            this.ID = Guid.Parse(element.Attribute("id").Value);
            this.Name = element.Attribute("name").Value;
            this.Index = int.Parse(element.Attribute("index").Value);
            if (element.Attribute("enabled") != null)
                this.Enabled = bool.Parse(element.Attribute("enabled").Value);
            this.LoadAttributes(element.Descendants().FirstOrDefault());

            foreach (var filterElement in element.Descendants("filters").Descendants("filter"))
            {
                var id = Guid.Parse(filterElement.Attribute("id").Value);
                var enabled = bool.Parse(filterElement.Attribute("enabled").Value);

                var filter = (from f in document.DataFilters
                              where f.ID == id
                              select f).FirstOrDefault();

                if (filter != null)
                {
                    var dataSourceFilter = new DataSourceFilter(filter);
                    dataSourceFilter.Enabled = enabled;
                    this.Filters.Add(dataSourceFilter);
                }
            }
        }

        protected abstract XElement SaveAttributes();

        protected abstract void LoadAttributes(XElement element);
    }
}
