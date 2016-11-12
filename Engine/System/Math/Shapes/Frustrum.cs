using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class Frustrum
    {
        private Plane _near, _far, _top, _bottom, _left, _right;

        public Plane Near { get { return _near; } }
        public Plane Far { get { return _far; } }
        public Plane Top { get { return _top; } }
        public Plane Bottom { get { return _bottom; } }
        public Plane Left { get { return _left; } }
        public Plane Right { get { return _right; } }

        public Frustrum(Plane near, Plane far, Plane top, Plane bottom, Plane left, Plane right)
        {
            _near = near;
            _far = far;
            _top = top;
            _bottom = bottom;
            _left = left;
            _right = right;
        }
    }
}
