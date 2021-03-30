using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using Renci.Wwt.Core.Common;
using Renci.Wwt.Core.Frames.Layers.Spreadsheet;
using System.IO;
using System.Threading.Tasks;

namespace Renci.Wwt.Core.Frames.Layers
{
    public class SpreadsheetLayer : Layer
    {
        private static SortedDictionary<string, Func<SpreadsheetDataItem, object>> _columnMaps = new SortedDictionary<string, Func<SpreadsheetDataItem, object>>() 
        { 
            { "NameColumn", (d) => d.Name }, 
            { "StartDateColumn", (d) => d.StartDate }, 
            { "EndDateColumn", (d) => d.EndDate }, 
            { "LngColumn", (d) => d.Longitude }, 
            { "LatColumn", (d) => d.Latitude }, 
            { "GeometryColumn", (d) => d.Geometry }, 
            { "AltColumn", (d) => d.Altitude }, 
            { "SizeColumn", (d) => d.Size }, 
            { "ColorMapColumn", (d) => d.Color.ToWwtColor() }, 
            { "MarkerColumn", (d) => d.Marker }, 
            { "HyperlinkColumn", (d) => d.Hyperlink }, 
            { "XAxisColumn", (d) => d.X }, 
            { "YAxisColumn", (d) => d.Y }, 
            { "ZAxisColumn", (d) => d.Z }, 
        };

        protected override string LayerClassName
        {
            get { return "SpreadSheetLayer"; }
        }

        /// <summary>
        /// Gets a value indicating whether the layer should be treated as time series data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the layer should be treated as time series data; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("TimeSeries")]
        public bool TimeSeries
        {
            get
            {
                return this.GetValue<bool>(() => this.TimeSeries);
            }
            set
            {
                this.SetValue(() => this.TimeSeries, value);
            }
        }

        /// <summary>
        /// Gets the date and time of the first data entry in a time series.
        /// </summary>
        [XmlAttribute("BeginRange")]
        public DateTime BeginRange
        {
            get
            {
                return this.GetValue<DateTime>(() => this.BeginRange);
            }
            set
            {
                this.SetValue(() => this.BeginRange, value);
            }
        }

        /// <summary>
        /// Gets the date and time of the last data entry in a time series.
        /// </summary>
        [XmlAttribute("EndRange")]
        public DateTime EndRange
        {
            get
            {
                return this.GetValue<DateTime>(() => this.EndRange);
            }
            set
            {
                this.SetValue(() => this.EndRange, value);
            }
        }

        /// <summary>
        /// Gets the decay rate of the visualization, in days, in the range 0.00025 to 4096.
        /// </summary>
        [XmlAttribute("Decay")]
        public float Decay
        {
            get
            {
                return this.GetValue<float>(() => this.Decay);
            }
            set
            {
                this.SetValue(() => this.Decay, value);
            }
        }

        /// <summary>
        /// Gets the type of the coordinates.
        /// </summary>
        /// <value>
        /// The type of the coordinates.
        /// </value>
        [XmlAttribute("CoordinatesType")]
        public CoordinatesType CoordinatesType
        {
            get
            {
                return this.GetValue<CoordinatesType>(() => this.CoordinatesType);
            }
            set
            {
                this.SetValue(() => this.CoordinatesType, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether direction of the X axis is reversed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if direction of the X axis is reversed; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("XAxisReverse")]
        public bool XAxisReverse
        {
            get
            {
                return this.GetValue<bool>(() => this.XAxisReverse);
            }
            set
            {
                this.SetValue(() => this.XAxisReverse, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether direction of the Y axis is reversed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if direction of the Y axis is reversed; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("YAxisReverse")]
        public bool YAxisReverse
        {
            get
            {
                return this.GetValue<bool>(() => this.YAxisReverse);
            }
            set
            {
                this.SetValue(() => this.YAxisReverse, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether direction of the Z axis is reversed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if direction of the Z axis is reversed; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("ZAxisReverse")]
        public bool ZAxisReverse
        {
            get
            {
                return this.GetValue<bool>(() => this.ZAxisReverse);
            }
            set
            {
                this.SetValue(() => this.ZAxisReverse, value);
            }
        }

        /// <summary>
        /// Gets the type of the alt.
        /// </summary>
        /// <value>
        /// The type of the alt.
        /// </value>
        [XmlAttribute("AltType")]
        public AltitudeType AltType
        {
            get
            {
                return this.GetValue<AltitudeType>(() => this.AltType);
            }
            set
            {
                this.SetValue(() => this.AltType, value);
            }
        }

        /// <summary>
        /// Gets the marker mix.
        /// </summary>
        [XmlAttribute("MarkerMix")]
        public GroupType MarkerMix
        {
            get
            {
                return this.GetValue<GroupType>(() => this.MarkerMix);
            }
            set
            {
                this.SetValue(() => this.MarkerMix, value);
            }
        }

        /// <summary>
        /// Gets the RA unit for astronomical data.
        /// </summary>
        [XmlAttribute("RaUnits")]
        public RAUnits RaUnits
        {
            get
            {
                return this.GetValue<RAUnits>(() => this.RaUnits);
            }
            set
            {
                this.SetValue(() => this.RaUnits, value);
            }
        }

        /// <summary>
        /// Gets the type of the plot.
        /// </summary>
        /// <value>
        /// The type of the plot.
        /// </value>
        [XmlAttribute("PlotType")]
        public PlotType PlotType
        {
            get
            {
                return this.GetValue<PlotType>(() => this.PlotType);
            }
            set
            {
                this.SetValue(() => this.PlotType, value);
            }
        }

        /// <summary>
        /// Gets the index number of marker graphic selected, or -1 if no marker has been selected.
        /// </summary>
        /// <value>
        /// The index of the marker.
        /// </value>
        [XmlAttribute("MarkerIndex")]
        public int MarkerIndex
        {
            get
            {
                return this.GetValue<int>(() => this.MarkerIndex);
            }
            set
            {
                this.SetValue(() => this.MarkerIndex, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data points on the invisible side of the body should be shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the data points on the invisible side of the body should be shown; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("ShowFarSide")]
        public bool ShowFarSide
        {
            get
            {
                return this.GetValue<bool>(() => this.ShowFarSide);
            }
            set
            {
                this.SetValue(() => this.ShowFarSide, value);
            }
        }

        /// <summary>
        /// Gets the marker scale.
        /// </summary>
        [XmlAttribute("MarkerScale")]
        public MarkerScale MarkerScale
        {
            get
            {
                return this.GetValue<MarkerScale>(() => this.MarkerScale);
            }
            set
            {
                this.SetValue(() => this.MarkerScale, value);
            }
        }

        /// <summary>
        /// Gets the altitude unit.
        /// </summary>
        [XmlAttribute("AltUnit")]
        public AltitudeUnits AltUnit
        {
            get
            {
                return this.GetValue<AltitudeUnits>(() => this.AltUnit);
            }
            set
            {
                this.SetValue(() => this.AltUnit, value);
            }
        }

        /// <summary>
        /// Gets the cartesian scale.
        /// </summary>
        [XmlAttribute("CartesianScale")]
        public CartesianScaleType CartesianScale
        {
            get
            {
                return this.GetValue<CartesianScaleType>(() => this.CartesianScale);
            }
            set
            {
                this.SetValue(() => this.CartesianScale, value);
            }
        }

        /// <summary>
        /// Gets the cartesian custom scale.
        /// </summary>
        /// <remarks>
        /// Used to divide the Cartesian co-ordinate values to a custom scale. If this value was set to "0.01" and the scale to Meters, the custom scale would be "centimeters".
        /// </remarks>
        [XmlAttribute("CartesianCustomScale")]
        public double CartesianCustomScale
        {
            get
            {
                return this.GetValue<double>(() => this.CartesianCustomScale);
            }
            set
            {
                this.SetValue(() => this.CartesianCustomScale, value);
            }
        }

        /// <summary>
        /// Gets the hyperlink format.
        /// </summary>
        [XmlAttribute("HyperlinkFormat")]
        public string HyperlinkFormat
        {
            get
            {
                return this.GetValue<string>(() => this.HyperlinkFormat);
            }
            set
            {
                this.SetValue(() => this.HyperlinkFormat, value);
            }
        }

        /// <summary>
        /// Gets the scale factor to apply to the magnitude of the event.
        /// </summary>
        [XmlAttribute("ScaleFactor")]
        public float ScaleFactor
        {
            get
            {
                return this.GetValue<float>(() => this.ScaleFactor);
            }
            set
            {
                this.SetValue(() => this.ScaleFactor, value);
            }
        }

        /// <summary>
        /// Gets the type of the point scale.
        /// </summary>
        /// <value>
        /// The type of the point scale.
        /// </value>
        [XmlAttribute("PointScaleType")]
        public PointScaleType PointScaleType
        {
            get
            {
                return this.GetValue<PointScaleType>(() => this.PointScaleType);
            }
            set
            {
                this.SetValue(() => this.PointScaleType, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetLayer"/> class that references an existing layer.
        /// </summary>
        /// <param name="client">The client.</param>
        internal SpreadsheetLayer(WwtClient client, Frame parent, XElement content)
            : base(client, parent, content)
        {

        }

        public SpreadsheetLayer(WwtClient client, Frame parent, string name)
            : base(client, parent, name)
        {
            //  Create new layer
            this.ID = client.CreateNewLayer(parent.Name, name);

            var counter = 0;

            var columnMap = _columnMaps.Keys.ToDictionary((k) => k, (k) => (counter++).ToString());

            //  Set layer default properties and map columns
            client.SetLayerPropertyValues(this.ID, columnMap);

            var content = this.Client.GetLayerProperties(this.ID);

            this.LoadContent(content);
        }

        public void AddData(DataItem data)
        {
            this.Client.AddLayerData(this.ID, this.FormatData((dynamic)data));
        }

        public void AddData(IEnumerable<DataItem> data)
        {
            //Parallel.ForEach(data,
            //() =>
            //{
            //    return new StringBuilder();
            //},

            //(value, loop, sb) =>
            //{
            //    var textValue = this.FormatData((dynamic)value);

            //    if (sb.Length > 0 && sb.Length + textValue.Length > 1024 * 1024)
            //    {
            //        this.Client.AddLayerData(this.ID, sb.ToString());
            //        sb.Length = 0;
            //    }

            //    if (sb.Length > 0)
            //        sb.Append("\r\n");
            //    sb.Append(textValue);

            //    return sb;
            //},
            //(sb) =>
            //{
            //    this.Client.AddLayerData(this.ID, sb.ToString());
            //});

            var sb = new StringBuilder();
            foreach (var value in data)
            {
                var textValue = this.FormatData((dynamic)value);

                if (sb.Length > 0 && sb.Length + textValue.Length > 1024 * 1024)
                {
                    this.Client.AddLayerData(this.ID, sb.ToString());
                    sb.Length = 0;
                }

                if (sb.Length > 0)
                    sb.Append("\r\n");
                sb.Append(textValue);
            }

            this.Client.AddLayerData(this.ID, sb.ToString());
        }

        public void LoadData(Stream input)
        {
            using (var sr = new StreamReader(input))
            {
                var data = sr.ReadLine();
            }
        }

        public void ClearData()
        {
            this.Client.ClearLayerData(this.ID);
        }

        private string FormatData(DataItem data)
        {
            throw new NotImplementedException();
        }

        private string FormatData(SpreadsheetDataItem data)
        {
            return string.Join("\t", from k in _columnMaps.Values select k(data));
        }
    }
}
