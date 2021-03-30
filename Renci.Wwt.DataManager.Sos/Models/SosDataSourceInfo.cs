using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.Models;
using System.Windows;
using Renci.Wwt.Core;
using System.ComponentModel;
using System.Xml.Linq;

namespace Renci.Wwt.DataManager.Sos.Models
{
    [Serializable]
    public class SosDataSourceInfo : DataSourceInfo
    {
        [CategoryAttribute("SOS"),
        DisplayName("Url"),
        DescriptionAttribute("SOS web service url.")]
        public string Url { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SosDataSourceInfo"/> class.
        /// </summary>
        public SosDataSourceInfo()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SosDataSourceInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public SosDataSourceInfo(Guid id, string name)
            : base(id, name)
        {

        }

        public override int GetDataCount()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DataItem> GetData()
        {
            throw new NotImplementedException();
        }

        protected override XElement SaveAttributes()
        {
            return new XElement("sos",
                new XAttribute("url", this.Url ?? string.Empty)
                );
        }

        protected override void LoadAttributes(XElement element)
        {
            if (!element.Name.LocalName.Equals("sos", StringComparison.InvariantCulture))
                throw new InvalidOperationException("'sos' element expected.");

            this.Url = element.Attribute("url").Value;
        }
    }
}
