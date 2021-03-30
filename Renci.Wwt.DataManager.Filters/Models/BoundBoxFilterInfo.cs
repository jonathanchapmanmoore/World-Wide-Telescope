using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Renci.Wwt.DataManager.Common.Models;
using System.ComponentModel;

namespace Renci.Wwt.DataManager.Filters.Models
{
    public class BoundBoxFilterInfo : FilterInfo
    {
        [CategoryAttribute("Boundaries"),
        DisplayName("Upper Latitude"),
        DescriptionAttribute("Upper Latitude value")]
        public double UpperLatitude
        {
            get { return this.Get(() => this.UpperLatitude); }
            set { this.Set(() => this.UpperLatitude, value); }
        }

        [CategoryAttribute("Boundaries"),
        DisplayName("Lower Latitude"),
        DescriptionAttribute("Lower Latitude value")]
        public double LowerLatitude
        {
            get { return this.Get(() => this.LowerLatitude); }
            set { this.Set(() => this.LowerLatitude, value); }
        }

        [CategoryAttribute("Boundaries"),
        DisplayName("Left Longitude"),
        DescriptionAttribute("Left Longitude value")]
        public double LeftLongitude
        {
            get { return this.Get(() => this.LeftLongitude); }
            set { this.Set(() => this.LeftLongitude, value); }
        }

        [CategoryAttribute("Boundaries"),
        DisplayName("Right Longitude"),
        DescriptionAttribute("Right Longitude value")]
        public double RightLongitude
        {
            get { return this.Get(() => this.RightLongitude); }
            set { this.Set(() => this.RightLongitude, value); }
        }

        public BoundBoxFilterInfo()
            : base()
        {

        }

        public BoundBoxFilterInfo(Guid id, string name)
            : base(id, name)
        {

        }

        protected override XElement SaveAttributes()
        {
            return new XElement("boundBox",
                new XAttribute("upperLatitude", this.UpperLatitude),
                new XAttribute("lowerLatitude", this.LowerLatitude),
                new XAttribute("leftLongitude", this.LeftLongitude),
                new XAttribute("rightLongitude", this.RightLongitude)
                );

        }

        protected override void LoadAttributes(XElement element)
        {
            if (!element.Name.LocalName.Equals("boundBox", StringComparison.InvariantCulture))
                throw new InvalidOperationException("'boundBox' element expected.");

            if (element.Attribute("upperLatitude") != null)
                this.UpperLatitude = double.Parse(element.Attribute("upperLatitude").Value);
            if (element.Attribute("lowerLatitude") != null)
                this.LowerLatitude = double.Parse(element.Attribute("lowerLatitude").Value);
            if (element.Attribute("leftLongitude") != null)
                this.LeftLongitude = double.Parse(element.Attribute("leftLongitude").Value);
            if (element.Attribute("rightLongitude") != null)
                this.RightLongitude = double.Parse(element.Attribute("rightLongitude").Value);

        }

        public override bool InRange(double lon, double lat)
        {
            if (lon > this.LeftLongitude && lon < this.RightLongitude &&
                lat < this.UpperLatitude && lat > this.LowerLatitude)
                return true;
            return false;
        }
    }
}
