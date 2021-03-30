using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.Models;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Renci.Wwt.Core;
using Renci.Wwt.Core.Frames.Layers.Spreadsheet;

namespace Renci.Wwt.DataManager.NetCDF.Models
{
    public class TextDataSourceInfo : DataSourceInfo
    {
        private List<DataItem> _dataItems;

        [CategoryAttribute("Text"),
        DisplayName("Color"),
        DescriptionAttribute("Elements variable name, must be double array of integers.")]
        public Color Color
        {
            get { return this.Get(() => this.Color); }
            set { this.Set(() => this.Color, value); }
        }

        [Browsable(false)]
        public string Text
        {
            get { return this.Get(() => this.Text); }
            set
            {
                this.Set(() => this.Text, value);
            }
        }

        public TextDataSourceInfo()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNetCDFDataSourceInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public TextDataSourceInfo(Guid id, string name)
            : base(id, name)
        {
        }


        public override int GetDataCount()
        {
            var lines = this.Text.Split(new char[] { '\n' });

            var re = new Regex(@"(?<lon>-?\d+([.]\d+)?)[,](?<lat>-?\d+([.]\d+)?)[,](?<alt>-?\d+([.]\d+)?)", RegexOptions.Compiled);

            this._dataItems = new List<DataItem>();

            foreach (var line in lines)
            {
                var match = re.Match(line);
                if (match != null)
                {
                    var inRange = false;
                    var geometry = new StringBuilder();
                    geometry.Append("LINESTRING ((");

                    while (match.Success)
                    {
                        var lat = double.Parse(match.Result("${lat}"));
                        var lon = double.Parse(match.Result("${lon}"));
                        var alt = double.Parse(match.Result("${alt}"));
                        geometry.AppendFormat("{0} {1} {2},", lon, lat, alt);

                        foreach (var filter in this.Filters)
                        {
                            if (inRange)
                                break;

                            inRange = filter.FilterInfo.InRange(lon, lat);
                        }

                        match = match.NextMatch();
                    }

                    geometry.Append(") 0)");

                    if (inRange)
                    {
                        var dataItem = new SpreadsheetDataItem(string.Empty, DateTime.Now.AddHours(-1), DateTime.Now.AddHours(-1 + 1),
                        geometry.ToString(),
                        System.Drawing.Color.FromArgb(this.Color.A, this.Color.R, this.Color.G, this.Color.B),
                        string.Empty,
                        string.Empty
                        );

                        this._dataItems.Add(dataItem);
                    }
                }
            }

            return _dataItems.Count;
        }

        public override IEnumerable<Core.DataItem> GetData()
        {
            return this._dataItems;
        }

        protected override System.Xml.Linq.XElement SaveAttributes()
        {
            return new XElement("text",
                new XElement("text", this.Text ?? string.Empty),
                new XAttribute("color", this.Color)
                );
        }

        protected override void LoadAttributes(System.Xml.Linq.XElement element)
        {
            if (!element.Name.LocalName.Equals("text", StringComparison.InvariantCulture))
                throw new InvalidOperationException("'text' element expected.");

            //this.Text = element.Attribute("text").Value;
            var textElement = element.Descendants("text").FirstOrDefault();
            if (textElement != null)
            {
                this.Text = textElement.Value;
            }

            if (element.Attribute("color") != null)
                this.Color = (Color)ColorConverter.ConvertFromString(element.Attribute("color").Value);
        }
    }
}
