using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.Models;
using System.Xml.Linq;
using System.ComponentModel;
using Renci.Wwt.DataManager.Common.Editors;
using Microsoft.Windows.Controls.PropertyGrid;
using Renci.Wwt.Core;
using System.IO;
using Renci.Wwt.Core.Frames.Layers.Spreadsheet;
using System.Windows.Media;

namespace Renci.Wwt.DataManager.NetCDF.Models
{
    public class ShapeDataSourceInfo : DataSourceInfo
    {
        private List<DataItem> _dataItems;

        [CategoryAttribute("Shape File"),
        DisplayName("File Path"),
        DescriptionAttribute("Shape file path")]
        [PropertyEditor(typeof(PathSelectionEditor))]
        public string Path
        {
            get { return this.Get(() => this.Path); }
            set { this.Set(() => this.Path, value); }
        }

        [CategoryAttribute("Shape File"),
        DisplayName("Color"),
        DescriptionAttribute("Elements variable name, must be double array of integers.")]
        public Color Color
        {
            get { return this.Get(() => this.Color); }
            set { this.Set(() => this.Color, value); }
        }


        public ShapeDataSourceInfo()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNetCDFDataSourceInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public ShapeDataSourceInfo(Guid id, string name)
            : base(id, name)
        {
        }


        public override int GetDataCount()
        {
            if (!File.Exists(this.Path))
                return 0;

            this._dataItems = this.readShapeFile(this.Path);

            return this._dataItems.Count;
        }

        public override IEnumerable<Core.DataItem> GetData()
        {
            return this._dataItems;
        }

        protected override System.Xml.Linq.XElement SaveAttributes()
        {
            return new XElement("shape",
                new XAttribute("path", this.Path),
                new XAttribute("color", this.Color)
                );
        }

        protected override void LoadAttributes(System.Xml.Linq.XElement element)
        {
            if (!element.Name.LocalName.Equals("shape", StringComparison.InvariantCulture))
                throw new InvalidOperationException("'shape' element expected.");

            if (element.Attribute("path") != null)
                this.Path = element.Attribute("path").Value;

            if (element.Attribute("color") != null)
                this.Color = (Color)ColorConverter.ConvertFromString(element.Attribute("color").Value);
        }

        public List<DataItem> readShapeFile(string filename)
        {
            List<DataItem> dataItems = new List<DataItem>();

            FileStream fs = new FileStream(filename, FileMode.Open);
            long fileLength = fs.Length;
            Byte[] data = new Byte[fileLength];
            fs.Read(data, 0, (int)fileLength);
            fs.Close();
            var filecode = readIntBig(data, 0);
            var filelength = readIntBig(data, 24);
            var version = readIntLittle(data, 28);
            var shapetype = readIntLittle(data, 32);
            var xMin = ReadSignedDouble(data, 36);
            var yMin = ReadSignedDouble(data, 44);
            yMin = 0 - yMin;
            var xMax = ReadSignedDouble(data, 52);
            var yMax = ReadSignedDouble(data, 60);
            yMax = 0 - yMax;
            var zMin = ReadSignedDouble(data, 68);
            var zMax = ReadSignedDouble(data, 76);
            var mMin = ReadSignedDouble(data, 84);
            var mMax = ReadSignedDouble(data, 92);
            int currentPosition = 100;

            while (currentPosition < fileLength)
            {
                DataItem dataItem = null;
                int recordStart = currentPosition;
                int recordNumber = readIntBig(data, recordStart);
                int contentLength = readIntBig(data, recordStart + 4);
                int recordContentStart = recordStart + 8;
                if (shapetype == 1)
                {
                    int recordShapeType = readIntLittle(data, recordContentStart);
                    var lon = (float)ReadSignedDouble(data, recordContentStart + 4);
                    var lat = (float)ReadSignedDouble(data, recordContentStart + 12);

                    dataItem = new SpreadsheetDataItem(string.Empty, DateTime.Now.AddHours(-1), DateTime.Now.AddHours(-1 + 1),
                        lat,
                        lon,
                        0,
                        1,
                    System.Drawing.Color.FromArgb(this.Color.A, this.Color.R, this.Color.G, this.Color.B),
                    string.Empty,
                    string.Empty
                    );
                }
                else if (shapetype == 3)
                {
                    var geometry = new StringBuilder();
                    geometry.Append("LINESTRING ((");

                    //Line line = new Line();
                    int recordShapeType = readIntLittle(data, recordContentStart);
                    //line.box = new Double[4];
                    //line.box[0] = readDoubleLittle(data, recordContentStart + 4);
                    //line.box[1] = readDoubleLittle(data, recordContentStart + 12);
                    //line.box[2] = readDoubleLittle(data, recordContentStart + 20);
                    //line.box[3] = readDoubleLittle(data, recordContentStart + 28);
                    ReadSignedDouble(data, recordContentStart + 4);
                    ReadSignedDouble(data, recordContentStart + 12);
                    ReadSignedDouble(data, recordContentStart + 20);
                    ReadSignedDouble(data, recordContentStart + 28);
                    //line.numParts = readIntLittle(data, recordContentStart + 36);
                    var numberOfParts = readIntLittle(data, recordContentStart + 36);
                    //line.parts = new int[line.numParts];
                    //line.numPoints = readIntLittle(data, recordContentStart + 40);
                    var numberOfPoints = readIntLittle(data, recordContentStart + 40);
                    //line.points = new PointF[line.numPoints];
                    int partStart = recordContentStart + 44;
                    for (int i = 0; i < numberOfParts; i++)
                    {
                        //line.parts[i] = readIntLittle(data, partStart + i * 4);
                        readIntLittle(data, partStart + i * 4);
                    }

                    int pointStart = recordContentStart + 44 + 4 * numberOfParts;
                    //for (int i = 0; i < line.numPoints; i++)
                    //{
                    //    line.points[i].X = (float)readDoubleLittle(data, pointStart + (i * 16));
                    //    line.points[i].Y = (float)readDoubleLittle(data, pointStart + (i * 16) + 8);
                    //    line.points[i].Y = 0 - line.points[i].Y;
                    //}
                    //lines.Add(line);

                    for (int i = 0; i < numberOfPoints; i++)
                    {
                        var lon = (float)ReadSignedDouble(data, pointStart + (i * 16));
                        var lat = (float)ReadSignedDouble(data, pointStart + (i * 16) + 8);
                        var alt = 0f;
                        geometry.AppendFormat("{0} {1} {2},", lon, lat, alt);
                    }

                    geometry.Append(") 0)");

                    dataItem = new SpreadsheetDataItem(string.Empty, DateTime.Now.AddHours(-1), DateTime.Now.AddHours(-1 + 1),
                    geometry.ToString(),
                    System.Drawing.Color.FromArgb(this.Color.A, this.Color.R, this.Color.G, this.Color.B),
                    string.Empty,
                    string.Empty
                    );

                }
                //else if (shapetype == 5)
                //{
                //    Polygon polygon = new Polygon();
                //    int recordShapeType = readIntLittle(data, recordContentStart);
                //    polygon.box = new Double[4];
                //    polygon.box[0] = readDoubleLittle(data, recordContentStart + 4);
                //    polygon.box[1] = readDoubleLittle(data, recordContentStart + 12);
                //    polygon.box[2] = readDoubleLittle(data, recordContentStart + 20);
                //    polygon.box[3] = readDoubleLittle(data, recordContentStart + 28);
                //    polygon.numParts = readIntLittle(data, recordContentStart + 36);
                //    polygon.parts = new int[polygon.numParts];
                //    polygon.numPoints = readIntLittle(data, recordContentStart + 40);
                //    polygon.points = new PointF[polygon.numPoints];
                //    int partStart = recordContentStart + 44;
                //    for (int i = 0; i < polygon.numParts; i++)
                //    {
                //        polygon.parts[i] = readIntLittle(data, partStart + i * 4);
                //    }
                //    int pointStart = recordContentStart + 44 + 4 * polygon.numParts;
                //    for (int i = 0; i < polygon.numPoints; i++)
                //    {
                //        polygon.points[i].X = (float)readDoubleLittle(data, pointStart + (i * 16));
                //        polygon.points[i].Y = (float)readDoubleLittle(data, pointStart + (i * 16) + 8);
                //        polygon.points[i].Y = 0 - polygon.points[i].Y;
                //    }
                //    polygons.Add(polygon);
                //}
                currentPosition = recordStart + (4 + contentLength) * 2;
                if (dataItem != null)
                    dataItems.Add(dataItem);
            }

            return dataItems;
        }

        public int readIntBig(byte[] data, int pos)
        {
            byte[] bytes = new byte[4];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public int readIntLittle(byte[] data, int pos)
        {
            byte[] bytes = new byte[4];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            return BitConverter.ToInt32(bytes, 0);
        }

        public double ReadSignedDouble(byte[] data, int pos)
        {
            byte[] bytes = new byte[8];
            bytes[0] = data[pos];
            bytes[1] = data[pos + 1];
            bytes[2] = data[pos + 2];
            bytes[3] = data[pos + 3];
            bytes[4] = data[pos + 4];
            bytes[5] = data[pos + 5];
            bytes[6] = data[pos + 6];
            bytes[7] = data[pos + 7];
            return BitConverter.ToDouble(bytes, 0);
        }

    }
}
