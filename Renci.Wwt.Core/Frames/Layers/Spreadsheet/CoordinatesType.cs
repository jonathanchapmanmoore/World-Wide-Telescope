using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renci.Wwt.Core.Frames.Layers.Spreadsheet
{
    public enum CoordinatesType
    {
        /// <summary>
        /// Applies when the reference frame is a sphere, and the coordinates will be Lat/Lng or RA/Dec.
        /// </summary>
        Spherical,

        /// <summary>
        /// Applies if there are X,Y and Z coordinates. 0,0,0 in this case is the center of the reference frame.
        /// </summary>
        Rectangular, 

        Orbital,
    }
}
