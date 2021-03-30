using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Renci.Wwt.Core.Frames.Layers
{
    public class GroundOverlayLayer : Layer
    {
        protected override string LayerClassName
        {
            get { return "GroundOverlayLayer"; }
        }

        internal GroundOverlayLayer(WwtClient client, Frame parent, XElement content)
            : base(client, parent, content)
        { 
        }
    }
}
