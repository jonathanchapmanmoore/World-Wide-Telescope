using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Renci.Wwt.DataManager.Common.Models;
using System.ComponentModel;

namespace Renci.Wwt.DataManager.Filters.Models
{
    public class BoundCircleFilterInfo : FilterInfo
    {
        [CategoryAttribute("Boundaries"),
        DisplayName("Latitude"),
        DescriptionAttribute("Upper Latitude value")]
        public double Latitude
        {
            get { return this.Get(() => this.Latitude); }
            set { this.Set(() => this.Latitude, value); }
        }

        [CategoryAttribute("Boundaries"),
        DisplayName("Longitude"),
        DescriptionAttribute("Longitude value")]
        public double Longitude
        {
            get { return this.Get(() => this.Longitude); }
            set { this.Set(() => this.Longitude, value); }
        }

        [CategoryAttribute("Boundaries"),
        DisplayName("Radius"),
        DescriptionAttribute("Boundary radius in miles")]
        public double Radius
        {
            get { return this.Get(() => this.Radius); }
            set { this.Set(() => this.Radius, value); }
        }

        public BoundCircleFilterInfo()
            : base()
        {

        }

        public BoundCircleFilterInfo(Guid id, string name)
            : base(id, name)
        {

        }

        protected override XElement SaveAttributes()
        {
            return new XElement("boundCircle",
                new XAttribute("latitude", this.Latitude),
                new XAttribute("longitude", this.Longitude),
                new XAttribute("radius", this.Radius)
                );

        }

        protected override void LoadAttributes(XElement element)
        {
            if (!element.Name.LocalName.Equals("boundCircle", StringComparison.InvariantCulture))
                throw new InvalidOperationException("'boundCircle' element expected.");

            if (element.Attribute("latitude") != null)
                this.Latitude = double.Parse(element.Attribute("latitude").Value);
            if (element.Attribute("longitude") != null)
                this.Longitude = double.Parse(element.Attribute("longitude").Value);
            if (element.Attribute("radius") != null)
                this.Radius = double.Parse(element.Attribute("radius").Value);
        }

        public override bool InRange(double lon, double lat)
        {
            return Calc(lat, lon, this.Latitude, this.Longitude) < this.Radius;
        }

        public static double Calc(double Lat1, double Long1, double Lat2, double Long2)
        {
            /*
                The Haversine formula according to Dr. Math.
                http://mathforum.org/library/drmath/view/51879.html
                
                dlon = lon2 - lon1
                dlat = lat2 - lat1
                a = (sin(dlat/2))^2 + cos(lat1) * cos(lat2) * (sin(dlon/2))^2
                c = 2 * atan2(sqrt(a), sqrt(1-a)) 
                d = R * c
                
                Where
                    * dlon is the change in longitude
                    * dlat is the change in latitude
                    * c is the great circle distance in Radians.
                    * R is the radius of a spherical Earth.
                    * The locations of the two points in 
                        spherical coordinates (longitude and 
                        latitude) are lon1,lat1 and lon2, lat2.
            */
            double dDistance = Double.MinValue;
            double dLat1InRad = Lat1 * (Math.PI / 180.0);
            double dLong1InRad = Long1 * (Math.PI / 180.0);
            double dLat2InRad = Lat2 * (Math.PI / 180.0);
            double dLong2InRad = Long2 * (Math.PI / 180.0);

            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;

            // Intermediate result a.
            double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                       Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                       Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Intermediate result c (great circle distance in Radians).
            double c = 2.0 * Math.Asin(Math.Sqrt(a));

            // Distance.
            const Double kEarthRadiusMiles = 3956.0;
            //const Double kEarthRadiusKms = 6376.5;
            dDistance = kEarthRadiusMiles * c;

            return dDistance;
        }
    }
}
