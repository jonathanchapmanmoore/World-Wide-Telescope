using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Renci.Wwt.Core.Frames.Layers
{
    public class LayerGroup : Frame
    {
        private IList<Layer> _layers = new List<Layer>();

        public IEnumerable<Layer> Layers
        {
            get
            {
                return new List<Layer>(this._layers);
            }
        }

        internal LayerGroup(WwtClient client, Frame parent, XElement content)
            : base(client, parent, content)
        {
        }

        protected override void LoadContent(XElement content)
        {
            base.LoadContent(content);

            //  Load child layers
            foreach (var element in content.Elements())
            {
                var layer = Layer.Create(this.Client, this, element);
                this._layers.Add(layer);
            }
        }
    }
}
