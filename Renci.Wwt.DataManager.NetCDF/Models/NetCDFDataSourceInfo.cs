using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.Models;
using System.ComponentModel;
using Microsoft.Windows.Controls.PropertyGrid;
using Renci.Wwt.DataManager.Common.Editors;
using Renci.Wwt.DataManager.NetCDF.Editors;
using System.Diagnostics;
using Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI;
using System.IO;
using Renci.Wwt.Core;
using Renci.Wwt.Core.Frames.Layers.Spreadsheet;

namespace Renci.Wwt.DataManager.NetCDF.Models
{
    [Serializable]
    public abstract class NetCDFDataSourceInfo : DataSourceInfo
    {
        private IList<IList<DataPoint>> _analizedDataPoints;

        [CategoryAttribute("NET CDF"),
        DisplayName("File Path"),
        DescriptionAttribute("NET CDF file path")]
        [PropertyEditor(typeof(PathSelectionEditor))]
        public string Path
        {
            get { return this.Get(() => this.Path); }
            set
            {
                if (this.Path != value)
                {
                    this.OnPathChanged(value);
                }
                this.Set(() => this.Path, value);
            }
        }

        [CategoryAttribute("NET CDF"),
        DisplayName("Latitude"),
        DescriptionAttribute("Latitude variable name, must be of array of double.")]
        [PropertyEditor(typeof(NetCDFVariableSelectionEditor))]
        public string Latitude
        {
            get { return this.Get(() => this.Latitude); }
            set { this.Set(() => this.Latitude, value); }
        }

        [CategoryAttribute("NET CDF"),
        DisplayName("Longitude"),
        DescriptionAttribute("Longitude variable name, must be of array of double.")]
        [PropertyEditor(typeof(NetCDFVariableSelectionEditor))]
        public string Longitude
        {
            get { return this.Get(() => this.Longitude); }
            set { this.Set(() => this.Longitude, value); }
        }

        [CategoryAttribute("NET CDF"),
        DisplayName("Depth"),
        DescriptionAttribute("Depth variable name, must be of array of double.")]
        [PropertyEditor(typeof(NetCDFVariableSelectionEditor))]
        public string Depth
        {
            get { return this.Get(() => this.Depth); }
            set { this.Set(() => this.Depth, value); }
        }

        [CategoryAttribute("NET CDF"),
        DisplayName("Frame Type"),
        DescriptionAttribute("Specifies what shape will be used to draw a frame.")]
        public ShapeTypes FrameType
        {
            get { return this.Get(() => this.FrameType); }
            set { this.Set(() => this.FrameType, value); }
        }

        [CategoryAttribute("NET CDF"),
        DisplayName("Color Transparency"),
        DefaultValue(50),
        DescriptionAttribute("Specifies color transparency. Mostly useful when drawing polygon.")]
        public int ColorTransparency
        {
            get
            {
                var value = this.Get(() => this.ColorTransparency);
                if (value < 1)
                {
                    value = 50;
                    this.Set(() => this.ColorTransparency, value);
                }
                return value;
            }
            set
            {
                if (value < 1)
                    value = 1;
                if (value > 100)
                    value = 100;
                this.Set(() => this.ColorTransparency, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetCDFDataSourceInfo"/> class.
        /// </summary>
        public NetCDFDataSourceInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetCDFDataSourceInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public NetCDFDataSourceInfo(Guid id, string name)
            : base(id, name)
        {
        }

        protected NetCDFReader Reader { get; private set; }

        public override int GetDataCount()
        {
            this._analizedDataPoints = this.AnalyzeData();

            return this._analizedDataPoints.Count;
        }

        public override IEnumerable<DataItem> GetData()
        {
            if (this._analizedDataPoints == null)
                this._analizedDataPoints = this.AnalyzeData();

            var dataItems = new List<DataItem>();

            foreach (var dataPoints in this._analizedDataPoints)
            {
                SpreadsheetDataItem dataItem = null;
                var firstDataPoint = dataPoints.FirstOrDefault();
                if (firstDataPoint == null)
                    continue;

                var color = this.GetColor(dataPoints);

                if (dataPoints.Count < 2 || this.FrameType == ShapeTypes.Point)
                {
                    dataItem = new SpreadsheetDataItem(firstDataPoint.Text, DateTime.Now.AddHours(-1), DateTime.Now,
                                        firstDataPoint.Latitude,
                                        firstDataPoint.Longitude,
                                        firstDataPoint.Altitude,
                                        0.5f,
                                        color,
                                        "marker",
                                        "http://www.cnn.com"
                                        );
                }
                else
                {
                    var geometry = new StringBuilder();
                    switch (this.FrameType)
                    {
                        case ShapeTypes.Linestring:
                            geometry.Append("LINESTRING ((");
                            break;
                        case ShapeTypes.Polygon:
                            geometry.Append("POLYGON ((");
                            break;
                        default:
                            break;
                    }

                    foreach (var dataPoint in dataPoints)
                    {
                        geometry.AppendFormat("{0} {1} {2},", dataPoint.Longitude, dataPoint.Latitude, dataPoint.Altitude);
                    }
                    geometry.AppendFormat("{0} {1} {2}) 0)", firstDataPoint.Longitude, firstDataPoint.Latitude, firstDataPoint.Altitude);

                    dataItem = new SpreadsheetDataItem(firstDataPoint.Text, DateTime.Now.AddHours(-1), DateTime.Now,
                                        geometry.ToString(),
                                        color,
                                        "marker",
                                        "http://www.cnn.com"
                                        );

                }

                dataItems.Add(dataItem);
            }

            return dataItems;
        }

        protected virtual void OnPathChanged(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    this.Reader = NetCDFReader.Create(path);
                }
                catch
                {
                    //  Reader cannot be created
                    this.Reader = null;
                }
            }
            else
            {
                this.Reader = null;
            }
        }

        protected abstract IList<IList<DataPoint>> AnalyzeData();

        protected abstract System.Drawing.Color GetColor(IEnumerable<DataPoint> dataPoints);

    }
}
