using System;
using System.Linq;
using Renci.Wwt.DataManager.Common.BaseClasses;
using System.ComponentModel;
using System.Xml.Linq;

namespace Renci.Wwt.DataManager.Common.Models
{
    public abstract class FilterInfo : ModelBase
    {
        [CategoryAttribute("Generic"),
        DisplayName("Filter ID"),
        DescriptionAttribute("Unique filter id assigned by the system and cannot changed.")]
        public Guid ID { get; private set; }

        [CategoryAttribute("Generic"),
        DisplayName("Filter Name"),
        DescriptionAttribute("Name of the filter.")]
        public string Name
        {
            get { return this.Get(() => this.Name); }
            set { this.Set(() => this.Name, value); }
        }

        [Browsable(false)]
        public int Index { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterInfo"/> class.
        /// </summary>
        public FilterInfo()
        {
            //  Empty class needed for deserialization
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sortIndex">Index of the sort.</param>
        /// <param name="name">The name.</param>
        public FilterInfo(Guid id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

        public XElement Save()
        {
            return new XElement("dataFilter",
                new XAttribute("id", this.ID),
                new XAttribute("name", this.Name),
                new XAttribute("index", this.Index),
                new XAttribute("type", this.GetType().AssemblyQualifiedName),
                this.SaveAttributes()
                );
        }

        public void Load(XElement element)
        {
            this.ID = Guid.Parse(element.Attribute("id").Value);
            this.Name = element.Attribute("name").Value;
            this.Index = int.Parse(element.Attribute("index").Value);
            this.LoadAttributes(element.Descendants().FirstOrDefault());
        }

        protected abstract XElement SaveAttributes();

        protected abstract void LoadAttributes(XElement element);

        public abstract bool InRange(double lon, double lat);
    }
}
