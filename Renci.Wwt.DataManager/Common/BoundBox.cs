using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renci.Wwt.DataManager.Common
{
    public struct BoundBox
    {
        public static BoundBox Empty
        {
            get
            {
                return new BoundBox
                {
                    Left = -180,
                    Right = 180,
                    Top = 90,
                    Bottom = -90,
                };
            }
        }
        public double Left { get; set; }

        public double Right { get; set; }

        public double Top { get; set; }

        public double Bottom { get; set; }

        /// <summary>
        /// Determines whether coordinates are in in range.
        /// </summary>
        /// <param name="x">The x (longitude).</param>
        /// <param name="y">The y (latitude).</param>
        /// <returns>
        ///   <c>true</c> if specified x,y is ni range; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInRange(double x, double y)
        {
            if ((y > this.Top || y < this.Bottom) || (x < this.Left || x > this.Right))
                return false;
            return true;
        }
    }
}
