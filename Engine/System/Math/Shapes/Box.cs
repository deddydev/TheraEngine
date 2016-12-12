using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using CustomEngine.Files;
using System.Globalization;
using BulletSharp;

namespace System
{
    public class Box : Shape
    {
        public Vec3 _min, _max;
        
        public Vec3 Minimum { get { return _min; } set { _min = value; CheckValid(); } }
        public Vec3 Maximum { get { return _max; } set { _max = value; CheckValid(); } }

        public Box(float halfZ, float halfY, float halfX)
        {
            _max = new Vec3(halfX, halfY, halfZ);
            _min = -_max;
            CheckValid();
        }
        public Box(Vec3 min, Vec3 max)
        {
            _min = min;
            _max = max;
            CheckValid();
        }
        public Box(Vec3 bounds)
        {
            _min = -bounds / 2.0f;
            _max = bounds / 2.0f;
            CheckValid();
        }
        public Box(float uniformBounds)
        {
            Vec3 bounds = new Vec3(uniformBounds);
            _min = -bounds / 2.0f;
            _max = bounds / 2.0f;
            CheckValid();
        }
        private void CheckValid()
        {
            if (_min.X > _max.X)
                CustomMath.Swap(ref _min.X, ref _max.X);
            if (_min.Y > _max.Y)
                CustomMath.Swap(ref _min.Y, ref _max.Y);
            if (_min.Z > _max.Z)
                CustomMath.Swap(ref _min.Z, ref _max.Z);
        }

        public Vec3 CenterPoint
        {
            get { return Vec3.TransformPosition((_min + _max) / 2.0f, WorldMatrix); }
            set
            {
                Vec3 currentOrigin = (_min + _max) / 2.0f;
                Vec3 newOrigin = Vec3.TransformPosition(value, InverseWorldMatrix);
                Vec3 diff = newOrigin - currentOrigin;
                _min += diff;
                _max += diff;
            }
        }
        public override CollisionShape GetCollisionShape()
        {
            Vec3 halfExtents = CenterPoint - _min;
            return new BoxShape(halfExtents.X, halfExtents.Y, halfExtents.Z);
        }
        /// <summary>
        /// T = top, B = bottom
        /// B = back, F = front
        /// L = left,  R = right
        /// </summary>
        public void GetUntransformedCorners(
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            GetCorners(Matrix4.Identity, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
        }
        public void GetTransformedCorners(
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            GetCorners(WorldMatrix, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
        }
        /// <summary>
        /// T = top, B = bottom
        /// B = back, F = front
        /// L = left,  R = right
        /// </summary>
        public void GetCorners(Matrix4 transform,
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            float Top = _max.Y;
            float Bottom = _min.Y;
            float Front = _max.Z;
            float Back = _min.Z;
            float Right = _max.X;
            float Left = _min.X;

            TBL = transform * new Vec3(Left, Top, Back);
            TBR = transform * new Vec3(Right, Top, Back);

            TFL = transform * new Vec3(Left, Top, Front);
            TFR = transform * new Vec3(Right, Top, Front);

            BBL = transform * new Vec3(Left, Bottom, Back);
            BBR = transform * new Vec3(Right, Bottom, Back);

            BFL = transform * new Vec3(Left, Bottom, Front);
            BFR = transform * new Vec3(Right, Bottom, Front);
        }
        public Vec3[] GetTransformedCorners() { return GetCorners(WorldMatrix); }
        public Vec3[] GetUntransformedCorners() { return GetCorners(Matrix4.Identity); }
        public Vec3[] GetCorners(Matrix4 transform)
        {
            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;
            GetCorners(transform, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public void ExpandBounds(Vec3 point)
        {
            _min.SetLequalTo(point);
            _max.SetGequalTo(point);
        }
        public override void Render() { Render(false); }
        public override void Render(bool solid)
        {
            //if (solid)
            //    Engine.Renderer.DrawBoxSolid(this);
            //else
            //    Engine.Renderer.DrawBoxWireframe(this);
        }
        public override PrimitiveData GetPrimitiveData()
        {
            VertexQuad left, right, top, bottom, front, back;
            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;

            GetUntransformedCorners(out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);

            Vec3 rightNormal = Vec3.Right;
            Vec3 frontNormal = Vec3.Forward;
            Vec3 topNormal = Vec3.Up;
            Vec3 leftNormal = -rightNormal;
            Vec3 backNormal = -frontNormal;
            Vec3 bottomNormal = -topNormal;

            left = VertexQuad.MakeQuad(BBL, BFL, TFL, TBL, leftNormal);
            right = VertexQuad.MakeQuad(BFR, BBR, TBR, TFR, rightNormal);
            top = VertexQuad.MakeQuad(TFL, TFR, TBR, TBL, topNormal);
            bottom = VertexQuad.MakeQuad(BBL, BBR, BFR, BFL, bottomNormal);
            front = VertexQuad.MakeQuad(BFL, BFR, TFR, TFL, frontNormal);
            back = VertexQuad.MakeQuad(BBR, BBL, TBL, TBR, backNormal);

            return PrimitiveData.FromQuads(Culling.Back, new PrimitiveBufferInfo(), left, right, top, bottom, front, back);
        }
        public Frustum AsFrustum(bool transformed = true)
        {
            Matrix4 m = transformed ? WorldMatrix : Matrix4.Identity;

            Vec3 ftl, ftr, ntl, ntr, fbl, fbr, nbl, nbr;
            GetCorners(m, out ftl, out ftr, out ntl, out ntr, out fbl, out fbr, out nbl, out nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }
        public bool Intersects(Ray ray) { float distance; return Collision.RayIntersectsBoxDistance(ray, this, out distance); }
        public bool Intersects(Ray ray, out float distance) { return Collision.RayIntersectsBoxDistance(ray, this, out distance); }
        public bool Intersects(Ray ray, out Vec3 point) { return Collision.RayIntersectsBox(ray, this, out point); }
        public EPlaneIntersection Intersects(Plane plane) { return Collision.PlaneIntersectsBox(plane, this); }
        public bool Intersects(Box box) { return Collision.BoxIntersectsBox(this, box); }
        public bool Intersects(Sphere sphere) { return Collision.BoxIntersectsSphere(this, sphere); }
        public override bool Contains(Vec3 point) { return Collision.BoxContainsPoint(this, point); }
        public override EContainment Contains(Box box) { return Collision.BoxContainsBox(this, box); }
        public override EContainment Contains(Sphere sphere) { return Collision.BoxContainsSphere(this, sphere); }
        public override EContainment Contains(Capsule capsule)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Cone cone)
        {
            throw new NotImplementedException();
        }
        public static Box FromSphere(Sphere sphere)
        {
            return new Box(
                new Vec3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius),
                new Vec3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius));
        }
        public static Box Merge(Box box1, Box box2)
        {
            return new Box(Vec3.ComponentMin(box1.Minimum, box2.Maximum), Vec3.ComponentMax(box1.Maximum, box2.Maximum));
        }
        public static bool operator ==(Box left, Box right) { return left.Equals(ref right); }
        public static bool operator !=(Box left, Box right) { return !left.Equals(ref right); }
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Minimum:{0} Maximum:{1}", Minimum.ToString(), Maximum.ToString());
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (Minimum.GetHashCode() * 397) ^ Maximum.GetHashCode();
            }
        }
        public bool Equals(ref Box value) { return Minimum == value.Minimum && Maximum == value.Maximum; }
        public override bool Equals(object value)
        {
            if (!(value is Box))
                return false;

            var strongValue = (Box)value;
            return Equals(ref strongValue);
        }
    }
}