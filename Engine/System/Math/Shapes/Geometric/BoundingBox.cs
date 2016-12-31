using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using CustomEngine.Files;
using System.Globalization;
using BulletSharp;
using CustomEngine.Worlds.Actors.Components;

namespace System
{
    /// <summary>
    /// Axis Aligned Bounding Box (AABB)
    /// </summary>
    public class BoundingBox : BoundingShape, IBoundingBox
    {
        private Vec3 _min, _max;
        
        /// <summary>
        /// In world space
        /// </summary>
        public Vec3 Minimum
        {
            get { return _point + _min; }
            set
            {
                _min = value - _point;
                CheckValid();
            }
        }
        /// <summary>
        /// In world space
        /// </summary>
        public Vec3 Maximum
        {
            get { return _point + _max; }
            set
            {
                _max = value - _point;
                CheckValid();
            }
        }
        public Vec3 Extents { get { return _max - _min; } }
        public Vec3 HalfExtents { get { return Extents / 2.0f; } }
        public Vec3 ExtentsCenter
        {
            get { return _point + (_min + _max) / 2.0f; }
            set
            {
                Vec3 currentOrigin = (_min + _max) / 2.0f;
                Vec3 diff = value - _point - currentOrigin;
                _min += diff;
                _max += diff;
            }
        }
        public BoundingBox(Vec3 center, float halfZ, float halfY, float halfX) : base(center)
        {
            _max = new Vec3(halfX, halfY, halfZ);
            _min = -_max;
            CheckValid();
        }
        public BoundingBox(Vec3 center, Vec3 min, Vec3 max) : base(center)
        {
            _min = min;
            _max = max;
            CheckValid();
        }
        public BoundingBox(Vec3 center, Vec3 bounds) : base(center)
        {
            _max = bounds / 2.0f;
            _min = -_max;
            CheckValid();
        }
        public BoundingBox(Vec3 center, float uniformBounds) : base(center)
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
        public override CollisionShape GetCollisionShape()
        {
            return new BoxShape((_max - _min) / 2.0f);
        }
        /// <summary>
        /// T = top, B = bottom
        /// B = back, F = front
        /// L = left,  R = right
        /// </summary>
        public void GetCorners(
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            GetCorners(_min, _max, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
        }
        public static void GetCorners(
            Vec3 min,
            Vec3 max,
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            float Top = max.Y;
            float Bottom = min.Y;
            float Front = max.Z;
            float Back = min.Z;
            float Right = max.X;
            float Left = min.X;

            TBL = new Vec3(Left, Top, Back);
            TBR = new Vec3(Right, Top, Back);

            TFL = new Vec3(Left, Top, Front);
            TFR = new Vec3(Right, Top, Front);

            BBL = new Vec3(Left, Bottom, Back);
            BBR = new Vec3(Right, Bottom, Back);

            BFL = new Vec3(Left, Bottom, Front);
            BFR = new Vec3(Right, Bottom, Front);
        }
        public void GetCorners(
            Matrix4 transform,
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            GetCorners(_min, _max, transform, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
        }
        public static void GetCorners(
            Vec3 min,
            Vec3 max,
            Matrix4 transform,
            out Vec3 TBL,
            out Vec3 TBR,
            out Vec3 TFL,
            out Vec3 TFR,
            out Vec3 BBL,
            out Vec3 BBR,
            out Vec3 BFL,
            out Vec3 BFR)
        {
            float Top = max.Y;
            float Bottom = min.Y;
            float Front = max.Z;
            float Back = min.Z;
            float Right = max.X;
            float Left = min.X;

            TBL = transform * new Vec3(Left, Top, Back);
            TBR = transform * new Vec3(Right, Top, Back);

            TFL = transform * new Vec3(Left, Top, Front);
            TFR = transform * new Vec3(Right, Top, Front);

            BBL = transform * new Vec3(Left, Bottom, Back);
            BBR = transform * new Vec3(Right, Bottom, Back);

            BFL = transform * new Vec3(Left, Bottom, Front);
            BFR = transform * new Vec3(Right, Bottom, Front);
        }
        public Vec3[] GetCorners()
        {
            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;
            GetCorners(out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public Vec3[] GetCorners(Matrix4 transform)
        {
            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;
            GetCorners(transform, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public static Vec3[] GetCorners(Vec3 min, Vec3 max, Matrix4 transform)
        {
            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;
            GetCorners(min, max, transform, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public void ExpandBounds(Vec3 point)
        {
            _min.SetLequalTo(point);
            _max.SetGequalTo(point);
        }
        public override void Render()
        {
            Engine.Renderer.RenderAABB(Minimum, Maximum, _renderSolid);
        }
        public static PrimitiveData Mesh(Vec3 min, Vec3 max)
        {
            VertexQuad left, right, top, bottom, front, back;
            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;

            GetCorners(min, max, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);

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
        public PrimitiveData GetMesh(bool includeCenter)
        {
            if (includeCenter)
                return Mesh(_point + _min, _point + _max);
            else
                return Mesh(_min, _max);
        }
        public static Frustum GetFrustum(Vec3 min, Vec3 max)
        {
            Vec3 ftl, ftr, ntl, ntr, fbl, fbr, nbl, nbr;
            GetCorners(min, max, out ftl, out ftr, out ntl, out ntr, out fbl, out fbr, out nbl, out nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }
        public static Frustum GetFrustum(Vec3 min, Vec3 max, Matrix4 transform)
        {
            Vec3 ftl, ftr, ntl, ntr, fbl, fbr, nbl, nbr;
            GetCorners(min, max, transform, out ftl, out ftr, out ntl, out ntr, out fbl, out fbr, out nbl, out nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }
        public Frustum AsFrustum() { return GetFrustum(Minimum, Maximum); }
        public Frustum AsFrustum(Matrix4 transform) { return GetFrustum(Minimum, Maximum, transform); }
        public bool Intersects(Ray ray)
        {
            float distance; return Collision.RayIntersectsAABBDistance(ray.StartPoint, ray.Direction, Minimum, Maximum, out distance);
        }
        public bool Intersects(Ray ray, out float distance)
        {
            return Collision.RayIntersectsAABBDistance(ray.StartPoint, ray.Direction, Minimum, Maximum, out distance);
        }
        public bool Intersects(Ray ray, out Vec3 point)
        {
            return Collision.RayIntersectsAABB(ray, Minimum, Maximum, out point);
        }
        public static BoundingBox FromSphere(Sphere sphere)
        {
            return new BoundingBox(
                new Vec3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius),
                new Vec3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius));
        }
        public static BoundingBox Merge(BoundingBox box1, BoundingBox box2)
        {
            return new BoundingBox(Vec3.ComponentMin(box1.Minimum, box2.Maximum), Vec3.ComponentMax(box1.Maximum, box2.Maximum));
        }
        public static bool operator ==(BoundingBox left, BoundingBox right) { return left.Equals(ref right); }
        public static bool operator !=(BoundingBox left, BoundingBox right) { return !left.Equals(ref right); }
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
        public bool Equals(ref BoundingBox value) { return Minimum == value.Minimum && Maximum == value.Maximum; }
        public override bool Equals(object value)
        {
            if (!(value is BoundingBox))
                return false;

            var strongValue = (BoundingBox)value;
            return Equals(ref strongValue);
        }

        public override bool Contains(Vec3 point)
        {
            return Collision.AABBContainsPoint(Minimum, Maximum, point);
        }
        public override EContainment Contains(IBoundingBox box)
        {
            return Collision.AABBContainsAABB(Minimum, Maximum, box.Minimum, box.Maximum);
        }
        public override EContainment Contains(IBox box)
        {
            return Collision.AABBContainsBox(Minimum, Maximum, box.Minimum, box.Maximum, box.InverseWorldMatrix);
        }
        public override EContainment Contains(ISphere sphere)
        {
            return Collision.AABBContainsSphere(Minimum, Maximum, sphere.Center, sphere.Radius);
        }
        public override EContainment ContainedWithin(IBoundingBox box)
        {
            return box.Contains(this);
        }
        public override EContainment ContainedWithin(IBox box)
        {
            return box.Contains(this);
        }
        public override EContainment ContainedWithin(ISphere sphere)
        {
            return sphere.Contains(this);
        }
        public override EContainment ContainedWithin(Frustum frustum)
        {
            return frustum.Contains(this);
        }
    }
}