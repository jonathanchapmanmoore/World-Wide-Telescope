using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using Renci.Wwt.Core.Frames.Layers.Object3D;

namespace Renci.Wwt.Core.Frames.Layers
{
    public class Object3DLayer : Layer
    {
        protected override string LayerClassName
        {
            get { return "Object3dLayer"; }
        }

        /// <summary>
        /// Gets the heading angle of the model in radians.
        /// </summary>
        [XmlAttribute("Heading")]
        public double Heading
        {
            get
            {
                return this.GetValue<double>(() => this.Heading);
            }
            private set
            {
                this.SetValue(() => this.Heading, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the textures for the 3D model should be flipped.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the textures for the 3D model should be flipped; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("FlipV")]
        public bool FlipV
        {
            get
            {
                return this.GetValue<bool>(() => this.FlipV);
            }
            private set
            {
                this.SetValue(() => this.FlipV, value);
            }
        }

        /// <summary>
        /// Gets the pitch angle of the model in radians.
        /// </summary>
        [XmlAttribute("Pitch")]
        public double Pitch
        {
            get
            {
                return this.GetValue<double>(() => this.Pitch);
            }
            private set
            {
                this.SetValue(() => this.Pitch, value);
            }
        }

        /// <summary>
        /// Gets the roll angle of the model in radians.
        /// </summary>
        [XmlAttribute("Roll")]
        public double Roll
        {
            get
            {
                return this.GetValue<double>(() => this.Roll);
            }
            private set
            {
                this.SetValue(() => this.Roll, value);
            }
        }

        /// <summary>
        /// Gets the scale of the model in x, y and z dimensions.
        /// </summary>  
        [XmlAttribute("Scale")]
        public Vector3d Scale
        {
            get
            {
                return this.GetValue<Vector3d>(() => this.Scale);
            }
            private set
            {
                this.SetValue(() => this.Scale, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the 3D Model should have its normals smoothed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the 3D Model should have its normals smoothed; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("Smooth")]
        public bool Smooth
        {
            get
            {
                return this.GetValue<bool>(() => this.Smooth);
            }
            private set
            {
                this.SetValue(() => this.Smooth, value);
            }
        }

        /// <summary>
        /// Gets the translation (movement offset) of the model in x, y and z dimensions, in units of the model size.
        /// </summary>
        [XmlAttribute("Translate")]
        public Vector3d Translate
        {
            get
            {
                return this.GetValue<Vector3d>(() => this.Translate);
            }
            private set
            {
                this.SetValue(() => this.Translate, value);
            }
        }

        internal Object3DLayer(WwtClient client, Frame parent, XElement content)
            : base(client, parent, content)
        { 
        }

    }
}
