using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Renci.Wwt.Core.Frames.Layers;

namespace Renci.Wwt.Core.Frames
{
    public abstract class Frame
    {
        private static IDictionary<string, Func<WwtClient, Frame, XElement, Frame>> _frameLoaders = new Dictionary<string, Func<WwtClient, Frame, XElement, Frame>>() { 
            {"ReferenceFrame", (client, parent, content)=>{return new ReferenceFrame(client, parent, content);}},
            {"LayerGroup", (client, parent, content)=>{return new LayerGroup(client, parent, content);}},
            {"Layer", (client, parent, content)=>{return Layer.Create(client, parent, content);}},
        };

        public virtual string Name { get; set; }

        public virtual bool Enabled { get; set; }

        public Frame Parent { get; private set; }

        protected WwtClient Client { get; private set; }

        public static Frame Create(WwtClient client, Frame parent, XElement content)
        {
            var name = content.Name.LocalName;

            if (_frameLoaders.ContainsKey(name))
            {
                return _frameLoaders[name](client, parent, content);
            }
            else 
            {
                throw new NotSupportedException(string.Format("Frame '{0}' is not supported.", name));
            }
        }

        internal Frame(WwtClient client, Frame parent, XElement content)
        {
            this.Client = client;
            this.Parent = parent;

            //  Load current node
            this.LoadContent(content);
        }

        public Frame(WwtClient client, Frame parent, string name)
        {
            this.Client = client;
            this.Parent = parent;

            this.Name = name;
        }

        protected virtual void LoadContent(XElement content)
        {
            //  TODO:   Ensure attributes exists in content
            this.Name = content.Attribute("Name").Value;
            this.Enabled = bool.Parse(content.Attribute("Enabled").Value);
        }
    }
}
