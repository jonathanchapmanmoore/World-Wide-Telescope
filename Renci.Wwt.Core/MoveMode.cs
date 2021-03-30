using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renci.Wwt.Core
{
    public enum MoveMode
    {
        /// <summary>
        /// Zoom in on the current view.
        /// </summary>
        ZoomIn,
        /// <summary>
        /// Zoom out of the current view.
        /// </summary>
        ZoomOut,
        /// <summary>
        /// Move the current view up.
        /// </summary>
        Up,
        /// <summary>
        /// Move the current view down.
        /// </summary>
        Down,
        /// <summary>
        /// Move the current view left.
        /// </summary>
        Left,
        /// <summary>
        /// Move the current view right.
        /// </summary>
        Right,
        /// <summary>
        /// Rotate the view clockwise 0.2 of one radian.
        /// </summary>
        Clockwise,
        /// <summary>
        /// Rotate the view counterclockwise 0.2 of one radian.
        /// </summary>
        CounterClockwise,
        /// <summary>
        /// Angle the view up 0.2 of one radian.
        /// </summary>
        TiltUp,
        /// <summary>
        /// Angle the view down 0.2 of one radian.
        /// </summary>
        TiltDown,
        /// <summary>
        /// Currently unimplemented.
        /// </summary>
        Finder,
    }
}
