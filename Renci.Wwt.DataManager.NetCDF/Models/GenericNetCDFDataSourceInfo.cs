using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.Models;
using System.Windows;
using Renci.Wwt.Core;
using System.ComponentModel;
using System.Xml.Linq;
using Renci.Wwt.Core.Frames.Layers.Spreadsheet;
using System.IO;
using Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI;
using Microsoft.Windows.Controls.PropertyGrid;
using Renci.Wwt.DataManager.NetCDF.Editors;
using Renci.Wwt.DataManager.Common.Editors;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Renci.Wwt.DataManager.NetCDF.Events;
using System.Windows.Media;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Renci.Wwt.DataManager.NetCDF.Models
{
    [Serializable]
    public class GenericNetCDFDataSourceInfo : NetCDFDataSourceInfo
    {
        //private IEnumerable<int> _analizedDataItems;

        [CategoryAttribute("NET CDF"),
        DisplayName("Color"),
        DescriptionAttribute("Elements variable name, must be double array of integers.")]
        public Color Color
        {
            get { return this.Get(() => this.Color); }
            set { this.Set(() => this.Color, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNetCDFDataSourceInfo"/> class.
        /// </summary>
        public GenericNetCDFDataSourceInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNetCDFDataSourceInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public GenericNetCDFDataSourceInfo(Guid id, string name)
            : base(id, name)
        {
        }

        protected override XElement SaveAttributes()
        {
            return new XElement("netcdf",
                new XAttribute("path", this.Path ?? string.Empty),
                new XAttribute("latitude", this.Latitude ?? string.Empty),
                new XAttribute("longitude", this.Longitude ?? string.Empty),
                new XAttribute("depth", this.Depth ?? string.Empty),
                new XAttribute("frameType", this.FrameType),
                new XAttribute("colorTransparency", this.ColorTransparency),
                new XAttribute("color", this.Color)
                );
        }

        protected override void LoadAttributes(XElement element)
        {
            if (!element.Name.LocalName.Equals("netcdf", StringComparison.InvariantCulture))
                throw new InvalidOperationException("'netcdf' element expected.");

            this.Path = element.Attribute("path").Value;
            this.Latitude = element.Attribute("latitude").Value;
            this.Longitude = element.Attribute("longitude").Value;
            if (element.Attribute("depth") != null)
                this.Depth = element.Attribute("depth").Value;
            if (element.Attribute("frameType") != null)
                this.FrameType = (ShapeTypes)Enum.Parse(typeof(ShapeTypes), element.Attribute("frameType").Value);
            if (element.Attribute("colorTransparency") != null)
                this.ColorTransparency = int.Parse(element.Attribute("colorTransparency").Value);
            if (element.Attribute("color") != null)
                this.Color = (Color)ColorConverter.ConvertFromString(element.Attribute("color").Value);
        }

        protected override IList<IList<DataPoint>> AnalyzeData()
        {
            var reader = this.Reader;

            double[] lat;
            double[] lon;
            double[] depth;

            lock (reader)
            {
                lat = reader.ReadDecimalVariable(this.Latitude).Select((v)=> decimal.ToDouble(v)).ToArray();
                lon = reader.ReadDecimalVariable(this.Longitude).Select((v) => decimal.ToDouble(v)).ToArray();
                depth = reader.ReadDecimalVariable(this.Depth).Select((v) => decimal.ToDouble(v)).ToArray();
            }

            int width, height;

            int[] elements = null;
            uint dimension = 1;

            if (reader.Variables.ContainsKey("ele"))
            {
                lock (reader)
                {
                    elements = reader.ReadVariable<int>("ele");
                }
                if (reader.Variables["ele"].DimensionIDs.Length > 1)
                    dimension = reader.Dimensions[reader.Variables["ele"].DimensionIDs[1]].Length;


                height = 3;
                width = elements.Length / height;
            }
            else
            {
                var index = 1;
                elements = (from i in lat select index++).ToArray();

                height = 1;
                width = lat.Length;
            }

            var array = new int[width, height];

            Parallel.For(0, elements.Length, (i) => { array[i / height, i % height] = elements[i]; });

            var units = reader.Variables[this.Depth].Attributes["units"].Value as string[];
            float scale = 1.0f;
            if (reader.Variables[this.Depth].Attributes.ContainsKey("wwt_scale"))
            {
                scale = (reader.Variables[this.Depth].Attributes["wwt_scale"].Value as float[]).First();
            }

            var unitName = units.FirstOrDefault();
            double unitConverter = 1;
            switch (unitName)
            {
                case "meters":
                case "m":
                    unitConverter = scale * 39.3700787;
                    break;
                default:
                    break;
            }

            var analizedDataPoints = new List<IList<DataPoint>>();

            Parallel.For(0, width, (i) =>
            {
                var query = from f in this.Filters where f.Enabled select f;

                for (int j = 0; j < height; j++)
                {
                    var localLon = lon[array[i, j] - 1];
                    var localLat = lat[array[i, j] - 1];
                    query = query.Where((f) => f.FilterInfo.InRange(localLon, localLat));
                }

                var matchFilterCount = query.Count();

                if (matchFilterCount > 0)
                {
                    var dataPoints = new List<DataPoint>();
                    for (int j = 0; j < dimension; j++)
                    {
                        var dataPoint = new DataPoint(string.Format("Node: {0}, Value {1}", array[i, j] - 1, 0), lat[array[i, j] - 1], lon[array[i, j] - 1], depth[array[i, j] - 1] * unitConverter, 0);
                        dataPoints.Insert(0, dataPoint);
                    }

                    lock (analizedDataPoints)
                    {
                        analizedDataPoints.Add(dataPoints);
                    }
                }
            });

            return analizedDataPoints;
        }

        protected override System.Drawing.Color GetColor(IEnumerable<DataPoint> dataPoints)
        {
            return System.Drawing.Color.FromArgb((int)(255 * (this.ColorTransparency / 100f)), this.Color.R, this.Color.G, this.Color.B);
        }
    }
}

