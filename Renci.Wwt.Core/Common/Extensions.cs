using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Renci.Wwt.Core.Common
{
    public static class Extensions
    {
        public static Color GetWwtColor(this string color)
        {
            var elements = color.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (elements[0].Equals("ARGBColor"))
            {
                var a = int.Parse(elements[1]);
                var r = int.Parse(elements[2]);
                var g = int.Parse(elements[3]);
                var b = int.Parse(elements[4]);
                return Color.FromArgb(a, r, g, b);
            }
            else
            {
                throw new ArgumentException("color");
            }
        }

        public static string ToWwtColor(this Color color)
        {
            return string.Format("{0}", color.ToArgb().ToString("X8"));
        }
    }
}
