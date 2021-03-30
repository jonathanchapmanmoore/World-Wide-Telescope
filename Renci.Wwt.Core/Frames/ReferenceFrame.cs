using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Renci.Wwt.Core.Frames
{
    public class ReferenceFrame : Frame
    {
        private IList<Frame> _frames = new List<Frame>();

        public IEnumerable<Frame> Elements
        {
            get
            {
                return new List<Frame>(this._frames);
            }
        }

        internal ReferenceFrame(WwtClient client, Frame parent, XElement content)
            : base(client, parent, content)
        {
        }

        protected override void LoadContent(XElement content)
        {
            base.LoadContent(content);

            //  Load child 
            foreach (var element in content.Elements())
            {
                var frame = Frame.Create(this.Client, this, element);
                this._frames.Add(frame);
            }
        }
    }
}
