using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public class LineStrip : ObjectBase
    {
        public LineStrip() { }
        public LineStrip(params Line[] lines)
        {
            _lines = lines.ToList();
        }

        List<Line> _lines;
    }
}
