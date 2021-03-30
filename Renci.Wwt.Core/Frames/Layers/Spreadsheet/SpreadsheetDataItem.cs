using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;

namespace Renci.Wwt.Core.Frames.Layers.Spreadsheet
{
    public class SpreadsheetDataItem : DataItem
    {
        //NameColumn,
        public string Name { get; private set; }

        //LatColumn,
        public double? Latitude { get; private set; }

        //LngColumn,
        public double? Longitude { get; private set; }

        //AltColumn,
        public double? Altitude { get; private set; }

        //SizeColumn,
        public double Size { get; private set; }

        public string Geometry { get; private set; }

        //StartDateColumn,
        public DateTime? StartDate { get; private set; }

        //EndDateColumn,
        public DateTime? EndDate { get; private set; }

        //ColorMapColumn,
        public Color Color { get; private set; }

        //MarkerColumn,
        public string Marker { get; private set; }

        //HyperlinkColumn,
        public string Hyperlink { get; private set; }

        //XAxisColumn,
        public double? X { get; set; }

        //YAxisColumn,
        public double? Y { get; set; }

        //ZAxisColumn
        public double? Z { get; set; }

        public SpreadsheetDataItem(string name, DateTime? startDateTime, DateTime? endDateTime, double latitude, double longtitude, double altitude, float size, Color color)
            : this(name, startDateTime, endDateTime, color, string.Empty, string.Empty)
        {
            this.Latitude = latitude;
            this.Longitude = longtitude;
            this.Altitude = altitude;
            this.Size = size;
        }

        public SpreadsheetDataItem(string name, DateTime? startDateTime, DateTime? endDateTime, double latitude, double longtitude, double altitude, float size, Color color, string marker, string hyperlink)
            : this(name, startDateTime, endDateTime, color, marker, hyperlink)
        {
            this.Latitude = latitude;
            this.Longitude = longtitude;
            this.Altitude = altitude;
            this.Size = size;
        }

        public SpreadsheetDataItem(string name, DateTime? startDateTime, DateTime? endDateTime, string geometry, Color color, string marker, string hyperlink)
            : this(name, startDateTime, endDateTime, color, marker, hyperlink)
        {
            this.Geometry = geometry;
        }

        private SpreadsheetDataItem(string name, DateTime? startDateTime, DateTime? endDateTime, Color color, string marker, string hyperlink)
            : base()
        {
            this.Name = name;
            this.StartDate = startDateTime;
            this.EndDate = endDateTime;
            this.Color = color;
            this.Marker = marker;
            this.Hyperlink = hyperlink;
        }

        public SpreadsheetDataItem()
            : base()
        {

        }

        public override void WriteHeaderTo(TextWriter sw)
        {
            sw.WriteLine(string.Join(",", "Name", "Lat", "Lon", "Alt", "Size", "Geo", "Start", "End", "Color", "Marker", "hyper", "X", "Y", "Z"));
        }

        public override void WriteDataTo(TextWriter sw)
        {
            sw.WriteLine(string.Join(",", this.Name, this.Latitude, this.Longitude, this.Altitude, this.Size, string.Format("\"{0}\"", this.Geometry), this.StartDate, this.EndDate, this.Color.ToArgb(), this.Marker, this.Hyperlink, this.X, this.Y, this.Z));
        }

        public override void ReadHeaderFrom(TextReader sr)
        {
            //  Read line to ignore header at the moment
            sr.ReadLine();
        }

        public override void ReadDataFrom(TextReader sr)
        {
            var fields = this.GetCsvFields(sr.ReadLine()).ToArray();

            this.Name = fields[0];
            double value;
            DateTime dateTime;
            int intValue;

            if (double.TryParse(fields[1], out value))
            {
                this.Latitude = value;
            }
            if (double.TryParse(fields[2], out value))
            {
                this.Longitude = value;
            }
            if (double.TryParse(fields[3], out value))
            {
                this.Altitude = value;
            }
            if (double.TryParse(fields[4], out value))
            {
                this.Size = value;
            }
            this.Geometry = fields[5];
            if (DateTime.TryParse(fields[6], out dateTime))
            {
                this.StartDate = dateTime;
            }
            if (DateTime.TryParse(fields[7], out dateTime))
            {
                this.EndDate = dateTime;
            }
            if (int.TryParse(fields[8], out intValue))
            {
                this.Color = Color.FromArgb(intValue);
            }
            this.Marker = fields[9];
            this.Hyperlink = fields[10];
            if (double.TryParse(fields[11], out value))
            {
                this.X = value;
            }
            if (double.TryParse(fields[12], out value))
            {
                this.Y = value;
            }
            if (double.TryParse(fields[13], out value))
            {
                this.Z = value;
            }
        }
    }
}
