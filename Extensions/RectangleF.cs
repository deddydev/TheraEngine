using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Extensions
{
    public static partial class Ext
    {
        public static Rectangle AsIntRect(this RectangleF rect)
            => new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        public static RectangleF AsFloatRect(this Rectangle rect)
            => new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
    }
}
