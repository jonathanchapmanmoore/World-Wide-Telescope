namespace VirtualEarthWPFControl
{
    public class VEPixel
    {

        public double X { get; set; }
        public double Y { get; set; }

        public VEPixel(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public override string ToString()
        {
            return this.X + "," + this.Y;
        }
    }
}
