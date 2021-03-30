using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Renci.Wwt.DataManager.NetCDF.Models
{
    public class DataPoint
    {
        public string Text { get; set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public double Altitude { get; private set; }

        public float Value { get; private set; }

        public DataPoint(string text, double latitude, double longitude, double altitude, float value)
        {
            this.Text = text;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Altitude = altitude;
            this.Value = value;
        }
    }
}
