using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Renci.Wwt.Core
{
    public class ViewState
    {
        public ViewMode LookAt { get; private set; }

        public double RightAscension { get; private set; }

        public double Declination { get; private set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public float Zoom { get; private set; }

        public double Angle { get; private set; }

        public double Rotation { get; private set; }

        public DateTime Time { get; private set; }

        public int Timerate { get; private set; }

        public string ReferenceFrame { get; private set; }

        public string ViewToken { get; private set; }

        public string ZoomText { get; private set; }

        internal ViewState(XElement element)
        {
            if (element == null)
                throw new WwtException("Not valid response. 'ViewState' node is expected.");

            if (element.Attribute("lookat") != null)
                this.LookAt = (ViewMode)Enum.Parse(typeof(ViewMode), element.Attribute("lookat").Value);

            if (element.Attribute("lat") != null)
                this.Latitude = double.Parse(element.Attribute("lat").Value);

            if (element.Attribute("lng") != null)
                this.Longitude = double.Parse(element.Attribute("lng").Value);

            if (element.Attribute("zoom") != null)
                this.Zoom = float.Parse(element.Attribute("zoom").Value);

            if (element.Attribute("angle") != null)
                this.Angle = double.Parse(element.Attribute("angle").Value);

            if (element.Attribute("rotation") != null)
                this.Rotation = double.Parse(element.Attribute("rotation").Value);

            if (element.Attribute("time") != null)
                this.Time = DateTime.Parse(element.Attribute("time").Value);

            if (element.Attribute("timerate") != null)
                this.Timerate = int.Parse(element.Attribute("timerate").Value);

            if (element.Attribute("ReferenceFrame") != null)
                this.ReferenceFrame = element.Attribute("ReferenceFrame").Value;

            if (element.Attribute("ViewToken") != null)
                this.ViewToken = element.Attribute("ViewToken").Value;

            if (element.Attribute("zoomText") != null)
                this.ZoomText = element.Attribute("zoomText").Value;

            if (element.Attribute("ra") != null)
                this.RightAscension = double.Parse(element.Attribute("ra").Value);

            if (element.Attribute("dec") != null)
                this.Declination = double.Parse(element.Attribute("dec").Value);
        }
    }
}
