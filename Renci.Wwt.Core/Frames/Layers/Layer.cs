using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;
using Renci.Wwt.Core.Common;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Renci.Wwt.Core.Frames.Layers
{
    public abstract class Layer : Frame
    {
        private static IDictionary<string, Func<WwtClient, Frame, XElement, Layer>> _layerLoaders = new Dictionary<string, Func<WwtClient, Frame, XElement, Layer>>() 
        { 
            {"SpreadSheetLayer", (client, parent, content)=>{return new SpreadsheetLayer(client, parent, content);}},
            {"Object3dLayer", (client, parent, content)=>{return new Object3DLayer(client, parent, content);}},
            {"ShapeFileRenderer", (client, parent, content)=>{return new ShapefileLayer(client, parent, content);}},
            {"ImageSetLayer", (client, parent, content)=>{return new ImageSetLayer(client, parent, content);}},
            {"GroundOverlayLayer", (client, parent, content)=>{return new GroundOverlayLayer(client, parent, content);}},
        };

        private IDictionary<string, string> _properties = new Dictionary<string, string>();

        protected abstract string LayerClassName { get; }

        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name
        {
            get
            {
                return this.GetValue<string>(() => this.Name);
            }
            set
            {
                this.SetValue(() => this.Name, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Layer"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public override bool Enabled
        {
            get
            {
                return this.GetValue<bool>(() => this.Enabled);
            }
            set
            {
                this.SetValue(() => this.Enabled, value);
            }
        }

        private Guid _id;
        /// <summary>
        /// Gets the ID of the layer.
        /// </summary>
        [XmlAttribute("ID ")]
        public Guid ID
        {
            get
            {
                return this._id;
            }
            protected set
            {
                this._id = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data is astronomical rather than terrestrial. If this is set to true the declination will be by default the Latitude column, and the right ascension the Longitude column.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the data is astronomical rather than terrestrial; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("Astronomical")]
        public bool Astronomical
        {
            get
            {
                return this.GetValue<bool>(() => this.Astronomical);
            }
            set
            {
                this.SetValue(() => this.Astronomical, value);
            }
        }

        /// <summary>
        /// Gets the color value.
        /// </summary>
        /// <remarks>String containing ARGB value of the color, in the format: "ARGBColor:255:255:255:255". </remarks>
        [XmlAttribute("ColorValue")]
        public Color Color
        {
            get
            {
                return this.GetValue<Color>(() => this.Color);
            }
            set
            {
                this.SetValue(() => this.Color, value);
            }
        }

        /// <summary>
        /// Gets the date and time to start the visualizations. This property is visible in the Lifetime dialog for the layer.
        /// </summary>
        [XmlAttribute("StartTime")]
        public DateTime StartTime
        {
            get
            {
                return this.GetValue<DateTime>(() => this.StartTime);
            }
            set
            {
                this.SetValue(() => this.StartTime, value);
            }
        }

        /// <summary>
        /// Gets the date and time to end the visualizations, and return to the start time in the case of an auto loop.  This property is visible in the Lifetime dialog for a layer.
        /// </summary>
        [XmlAttribute("EndTime")]
        public DateTime EndTime
        {
            get
            {
                return this.GetValue<DateTime>(() => this.EndTime);
            }
            set
            {
                this.SetValue(() => this.EndTime, value);
            }
        }

        /// <summary>
        /// Gets the time it takes to fade out the graphic.
        /// </summary>
        /// <remarks>
        /// This property is visible in the Lifetime dialog for the layer and is shown in days in the dialog.
        /// </remarks>
        [XmlAttribute("FadeSpan")]
        public TimeSpan FadeSpan
        {
            get
            {
                return this.GetValue<TimeSpan>(() => this.FadeSpan);
            }
            set
            {
                this.SetValue(() => this.FadeSpan, value);
            }
        }

        /// <summary>
        /// Gets the type of the fade.
        /// </summary>
        /// <value>
        /// The type of the fade.
        /// </value>
        [XmlAttribute("FadeType")]
        public FadeType FadeType
        {
            get
            {
                return this.GetValue<FadeType>(() => this.FadeType);
            }
            set
            {
                this.SetValue(() => this.FadeType, value);
            }
        }

        /// <summary>
        /// Gets the opacity of the layer graphics from 0.0 to 1.0..
        /// </summary>
        [XmlAttribute("Opacity")]
        public float Opacity
        {
            get
            {
                return this.GetValue<float>(() => this.Opacity);
            }
            set
            {
                this.SetValue(() => this.Opacity, value);
            }
        }

        public static new Layer Create(WwtClient client, Frame parent, XElement content)
        {
            var typeAttribute = content.Attribute("Type");
            if (typeAttribute == null)
                throw new InvalidOperationException("Attribute 'Type' is not found.");

            var name = typeAttribute.Value;

            if (_layerLoaders.ContainsKey(name))
            {
                return _layerLoaders[name](client, parent, content);
            }
            else
            {
                throw new NotSupportedException(string.Format("Frame '{0}' is not supported.", name));
            }
        }

        internal Layer(WwtClient client, Frame parent, XElement content)
            : base(client, parent, content)
        {
        }

        public Layer(WwtClient client, Frame parent, string name)
            : base(client, parent, name)
        {
        }

        public virtual void Update()
        {
            this.Client.SetLayerPropertyValues(this.ID, this._properties);
        }

        /// <summary>
        /// Highlights the selected layer in the layer manager.
        /// </summary>
        public virtual void Activate()
        {
            this.Client.ActivateLayer(this.ID);
        }

        /// <summary>
        /// Permanently deletes the layer.
        /// </summary>
        public virtual void Delete()
        {
            this.Client.DeleteLayer(this.ID);
        }

        protected override void LoadContent(XElement content)
        {
            this._properties = (from x in content.Attributes()
                                select new { Key = x.Name.LocalName, Value = x.Value }).ToDictionary((k) => k.Key, (v) => v.Value);
        }

        protected TProperty GetValue<TProperty>(Expression<Func<TProperty>> property)
        {
            //  TODO:   Refactor Get method to minimize reflection
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;

            var xmlAttribute = memberExpression.Member.GetCustomAttributes(typeof(XmlAttributeAttribute), true).SingleOrDefault() as XmlAttributeAttribute;

            var propertyName = memberExpression.Member.Name;

            if (xmlAttribute != null)
            {
                propertyName = xmlAttribute.AttributeName;
            }

            var type = typeof(TProperty);

            TProperty propertyValue = default(TProperty);

            if (this._properties.ContainsKey(propertyName))
            {
                //  Handle special types
                if (type == typeof(Color))
                {
                    propertyValue = (TProperty)Convert.ChangeType(this._properties[propertyName].GetWwtColor(), type);
                }
                else if (type == typeof(Guid))
                {
                    propertyValue = (TProperty)(object)Guid.Parse(this._properties[propertyName]);
                }
                else
                {
                    propertyValue = (TProperty)Convert.ChangeType(this._properties[propertyName], type);
                }
            }

            ////  TODO:   Set flag whether update property immidiatly or batch updates
            //var response = this.Send(string.Format("cmd=getprop &id={0}&propname={2}", this.ID, propertyName));

            return propertyValue;
        }

        protected void SetValue<TProperty>(Expression<Func<TProperty>> property, TProperty value)
        {
            //  TODO:   Refactor Get method to minimize reflection
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;

            var xmlAttribute = memberExpression.Member.GetCustomAttributes(typeof(XmlAttributeAttribute), true).SingleOrDefault() as XmlAttributeAttribute;

            var propertyName = memberExpression.Member.Name;

            if (xmlAttribute != null)
            {
                propertyName = xmlAttribute.AttributeName;
            }

            var propertyValue = value.ToString();

            var type = typeof(TProperty);

            //  Handle special types
            if (type == typeof(Color))
            {
                dynamic color = value;
                propertyValue = string.Format("ARGBColor:{0}:{1}:{2}:{3}", color.A, color.R, color.G, color.B);
            }

            if (this._properties.ContainsKey(propertyName))
            {
                this._properties[propertyName] = propertyValue;
            }
            else
            {
                this._properties.Add(propertyName, propertyValue);
            }

            //  TODO:   Set flag whether update property immidiatly or batch updates
            //var response = this.Send(string.Format("cmd=setprop&id={0}&propname={2}&propvalue={3}", this.ID, propertyName, propertyValue));
            this.Client.SetLayerPropertyValue(this.ID, propertyName, propertyValue);
        }
    }
}
