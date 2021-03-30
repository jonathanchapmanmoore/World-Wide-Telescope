namespace VirtualEarthWPFControl
{
    public class VELatLong
    {
        double lat;
        double lon;
        double alt;
        string altMode;

        public VELatLong(double lat, double lon)
        {
            this.lat = lat;
            this.lon = lon;
        }

        public VELatLong(double lat, double lon, double alt)
        {
            this.lat = lat;
            this.lon = lon;
            this.alt = alt;
        }

        public VELatLong(double lat, double lon, double alt, string altMode)
        {
            this.lat = lat;
            this.lon = lon;
            this.alt = alt;
            this.altMode = altMode;
        }

        public double Latitude
        {
            get { return this.lat; }
            set { value = this.lat; }
        }

        public double Longitude
        {
            get { return this.lon; }
            set { value = this.lon; }
        }

        public double Altitude
        {
            get { return this.alt; }
            set { value = this.alt; }
        }

        public string AltMode
        {
            get { return this.altMode; }
            set { value = this.altMode; }
        }

        public override string ToString()
        {
            return this.lat + "," + this.lon;
        }
    }
}
