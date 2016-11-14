using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class Frustrum : IEnumerable<Plane>
    {
        private Plane[] _planes = new Plane[6];

        public Plane Near { get { return _planes[0]; } }
        public Plane Far { get { return _planes[1]; } }
        public Plane Top { get { return _planes[2]; } }
        public Plane Bottom { get { return _planes[3]; } }
        public Plane Left { get { return _planes[4]; } }
        public Plane Right { get { return _planes[5]; } }

        public Frustrum(Plane near, Plane far, Plane top, Plane bottom, Plane left, Plane right)
        {
            _planes[0] = near;
            _planes[1] = far;
            _planes[2] = top;
            _planes[3] = bottom;
            _planes[4] = left;
            _planes[5] = right;
        }

        public IEnumerator<Plane> GetEnumerator()
        {
            return ((IEnumerable<Plane>)_planes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Plane>)_planes).GetEnumerator();
        }
    }
}
