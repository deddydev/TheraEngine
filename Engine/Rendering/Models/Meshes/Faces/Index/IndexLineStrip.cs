using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public class IndexLineStrip : IndexPrimitive
    {
        private bool _closed = false;

        public IndexLineStrip() { }
        public IndexLineStrip(bool closed, params IndexPoint[] points) 
            : base(points) { _closed = closed; }

        public override FaceType Type { get { return _closed ? FaceType.LineLoop : FaceType.LineStrip; } }
    }
}
