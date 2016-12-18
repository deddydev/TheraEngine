using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class Frustum : IEnumerable<Plane>
    {
        public static bool UseBoundingSphere = true;

        //For quickly testing if objects in large scenes should even be tested against the frustum at all
        private Sphere _boundingSphere;

        private Plane[] _planes = new Plane[6];
        
        private Vec3 _farBottomLeft, _farBottomRight, _farTopLeft, _farTopRight,
            _nearBottomLeft, _nearBottomRight, _nearTopLeft, _nearTopRight;

        public Vec3 FarBottomLeft { get { return _farBottomLeft; } }
        public Vec3 FarBottomRight { get { return _farBottomRight; } }

        public Vec3 FarTopLeft { get { return _farTopLeft; } }
        public Vec3 FarTopRight { get { return _farTopRight; } }

        public Vec3 NearBottomLeft { get { return _nearBottomLeft; } }
        public Vec3 NearBottomRight { get { return _nearBottomRight; } }

        public Vec3 NearTopLeft { get { return _nearTopLeft; } }
        public Vec3 NearTopRight { get { return _nearTopRight; } }

        public Plane Near { get { return _planes[0]; } }
        public Plane Far { get { return _planes[1]; } }

        public Plane Left { get { return _planes[2]; } }
        public Plane Right { get { return _planes[3]; } }

        public Plane Top { get { return _planes[4]; } }
        public Plane Bottom { get { return _planes[5]; } }

        public Frustum(
            Vec3 farBottomLeft, Vec3 farBottomRight, Vec3 farTopLeft, Vec3 farTopRight,
            Vec3 nearBottomLeft, Vec3 nearBottomRight, Vec3 nearTopLeft, Vec3 nearTopRight)
        {
            _farBottomLeft = farBottomLeft;
            _farBottomRight = farBottomRight;
            _farTopLeft = farTopLeft;
            _farTopRight = farTopRight;
            _nearBottomLeft = nearBottomLeft;
            _nearBottomRight = nearBottomRight;
            _nearTopLeft = nearTopLeft;
            _nearTopRight = nearTopRight;

            //near, far
            _planes[0] = new Plane(nearBottomRight, nearBottomLeft, nearTopRight);
            _planes[1] = new Plane(farBottomLeft, farBottomRight, farTopLeft);

            //left, right
            _planes[2] = new Plane(nearBottomLeft, farBottomLeft, nearTopLeft);
            _planes[3] = new Plane(farBottomRight, nearBottomRight, farTopRight);

            //top, bottom
            _planes[4] = new Plane(farTopLeft, farTopRight, nearTopLeft);
            _planes[5] = new Plane(nearBottomLeft, nearBottomRight, farBottomLeft);


        }
        public IEnumerator<Plane> GetEnumerator() { return ((IEnumerable<Plane>)_planes).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Plane>)_planes).GetEnumerator(); }

        public Frustum TransformedBy(Matrix4 transform)
        {
            return new Frustum(
                transform.TransformPosition(FarBottomLeft),
                transform.TransformPosition(FarBottomRight),
                transform.TransformPosition(FarTopLeft),
                transform.TransformPosition(FarTopRight),
                transform.TransformPosition(NearBottomLeft),
                transform.TransformPosition(NearBottomRight),
                transform.TransformPosition(NearTopLeft),
                transform.TransformPosition(NearTopRight));
        }
        public EContainment Contains(Shape shape)
        {
            if (shape is Box)
                return Contains((Box)shape);
            else if (shape is Sphere)
                return Contains((Sphere)shape);
            else if (shape is Capsule)
                return Contains((Capsule)shape);
            return EContainment.Disjoint;
        }
        public EContainment Contains(Box box) { return Collision.FrustumContainsBox1(this, box); }
        public EContainment Contains(Sphere sphere) { return Collision.FrustumContainsSphere(this, sphere); }
        public EContainment Contains(Capsule capsule)
        {
            Sphere top = capsule.GetTopSphere();
            Sphere bottom = capsule.GetBottomSphere();
            EContainment ct = Contains(top);
            EContainment cb = Contains(bottom);

            if (ct == EContainment.Contains && cb == EContainment.Contains)
                return EContainment.Contains;

            if (ct == EContainment.Intersects || cb == EContainment.Intersects)
                return EContainment.Intersects;

            if ((ct == EContainment.Disjoint && cb == EContainment.Contains) ||
                (ct == EContainment.Contains && cb == EContainment.Disjoint))
                return EContainment.Intersects;

            //TODO: test for when both spheres lie outside, but the area between interects the frustum
            return EContainment.Disjoint;
        }
        public Sphere GetBoundingSphere()
        {
            Vec3 center =
                (_nearBottomLeft + _nearBottomRight + _nearTopLeft + _nearTopRight +
                _farBottomLeft + _farBottomRight + _farTopLeft + _farTopRight) / 8.0f;
            return new Sphere(center.DistanceToFast(_farBottomLeft), center);
        }
    }
}
