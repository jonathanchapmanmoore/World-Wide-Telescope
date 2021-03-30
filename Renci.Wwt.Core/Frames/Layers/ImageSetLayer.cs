using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Renci.Wwt.Core.Frames.Layers
{
    public class ImageSetLayer : Layer
    {
        protected override string LayerClassName
        {
            get { return "ImageSetLayer"; }
        }

        //internal ImageSetLayer(WwtClient client, Guid id)
        //    : base(client, id)
        //{
        //}

        internal ImageSetLayer(WwtClient client, Frame parent, XElement content)
            : base(client, parent, content)
        {

        }
    }
}
