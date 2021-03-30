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
using System.Threading.Tasks;
//using System.Windows.Media;

namespace Renci.Wwt.DataManager.NetCDF.Models
{
    [Serializable]
    public class VariableNetCDFDataSourceInfo : NetCDFDataSourceInfo
    {
        private float _maximumValue;

        private float _minimumValue;

        [CategoryAttribute("NET CDF"),
        DisplayName("Minimum Color"),
        DescriptionAttribute("Color that represents minimum value, must be double array of integers.")]
        public Color MinimumValueColor
        {
            get { return this.Get(() => this.MinimumValueColor); }
            set { this.Set(() => this.MinimumValueColor, value); }
        }

        [CategoryAttribute("NET CDF"),
        DisplayName("Maximum  Color"),
        DescriptionAttribute("Color that represents maximum value, must be double array of integers.")]
        public Color MaximumValueColor
        {
            get { return this.Get(() => this.MaximumValueColor); }
            set { this.Set(() => this.MaximumValueColor, value); }
        }

        [CategoryAttribute("NET CDF"),
        DisplayName("Value Variable"),
        DescriptionAttribute("Value variable name to use for coloring, must be of array of double.")]
        [PropertyEditor(typeof(NetCDFVariableSelectionEditor))]
        public string Value
        {
            get { return this.Get(() => this.Value); }
            set { this.Set(() => this.Value, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericVariableNetCDFDataSourceInfoNetCDFDataSourceInfo"/> class.
        /// </summary>
        public VariableNetCDFDataSourceInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNetCDFDataSourceInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public VariableNetCDFDataSourceInfo(Guid id, string name)
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
                new XAttribute("minimumColor", this.MinimumValueColor),
                new XAttribute("maximumColor", this.MaximumValueColor),
                new XAttribute("value", this.Value ?? string.Empty)
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
            if (element.Attribute("minimumColor") != null)
                this.MinimumValueColor = (Color)ColorConverter.ConvertFromString(element.Attribute("minimumColor").Value);
            if (element.Attribute("maximumColor") != null)
                this.MaximumValueColor = (Color)ColorConverter.ConvertFromString(element.Attribute("maximumColor").Value);
            if (element.Attribute("value") != null)
                this.Value = element.Attribute("value").Value;

        }

        protected override IList<IList<DataPoint>> AnalyzeData()
        {
            var reader = this.Reader;

            double[] lat;
            double[] lon;
            double[] depth;
            float[] values;

            lock (reader)
            {
                lat = reader.ReadDecimalVariable(this.Latitude).Select((v) => decimal.ToDouble(v)).ToArray();
                lon = reader.ReadDecimalVariable(this.Longitude).Select((v) => decimal.ToDouble(v)).ToArray();
                depth = reader.ReadDecimalVariable(this.Depth).Select((v) => decimal.ToDouble(v)).ToArray();
                values = reader.ReadDecimalVariable(this.Value).Select((v) => decimal.ToSingle(v)).ToArray();
            }

            //  TODO:   Only first array will be considered in values 

            //  Find maximum and minimum values
            var missingValue = (from attribute in reader.Variables[this.Value].Attributes
                                where attribute.Key == "missing_value"
                                select ((float[])attribute.Value.Value).FirstOrDefault()).FirstOrDefault();

            this._maximumValue = (from v in values where v != missingValue select v).Max();
            this._minimumValue = (from v in values where v != missingValue select v).Min();

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

                for (int j = 0; j < dimension; j++)
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
                        var value = values[array[i, j] - 1];

                        if (value == missingValue)
                            break;

                        var dataPoint = new DataPoint(string.Format("Node: {0}, Value {1}, Lat: {2}, Lon:{3}", array[i, j], value, lat[array[i, j] - 1], lon[array[i, j] - 1]), lat[array[i, j] - 1], lon[array[i, j] - 1], depth[array[i, j] - 1] * unitConverter, value);
                        dataPoints.Insert(0, dataPoint);
                    }

                    if (dataPoints.Count == dimension)
                    {
                        //  Add only fully completed data points
                        //  any data points that has missing value should be ignored
                        lock (analizedDataPoints)
                        {
                            analizedDataPoints.Add(dataPoints);
                        }
                    }
                }
            });

            return analizedDataPoints;
        }

        protected override System.Drawing.Color GetColor(IEnumerable<DataPoint> dataPoints)
        {
            var colorValue = (from dp in dataPoints select dp.Value).Average();

            var color = this.MinimumValueColor.Lerp(this.MaximumValueColor, (float)(colorValue / (this._maximumValue - this._minimumValue)));

            return System.Drawing.Color.FromArgb((int)(255 * (this.ColorTransparency / 100f)), color.R, color.G, color.B);
        }
    }

    public static class ColorExtension
    {
        public static Color Lerp(this Color colour, Color to, float amount)
        {
            // start colors as lerp-able floats
            float sr = colour.R, sg = colour.G, sb = colour.B;

            // end colors as lerp-able floats
            float er = to.R, eg = to.G, eb = to.B;

            // lerp the colors to get the difference
            byte r = (byte)sr.Lerp(er, amount),
                 g = (byte)sg.Lerp(eg, amount),
                 b = (byte)sb.Lerp(eb, amount);

            // return the new color
            return new Color() { A = 255, R = r, G = g, B = b };
        }

        public static float Lerp(this float start, float end, float amount)
        {
            float difference = end - start;
            float adjusted = difference * amount;
            return start + adjusted;
        }
    }

}

