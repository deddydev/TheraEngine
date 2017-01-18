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
    public class BoundingBox : Shape
    {
        protected Vec3 _halfExtents, _translation;

        public Vec3 Minimum
        {
            get { return _translation - _halfExtents; }
            set
            {
                _translation = (Maximum + value) / 2.0f;
                _halfExtents = (Maximum - value) / 2.0f;
            }
        }
        public Vec3 Maximum
        {
            get { return _translation + _halfExtents; }
            set
            {
                _translation = (value + Minimum) / 2.0f;
                _halfExtents = (value - Minimum) / 2.0f;
            }
        }
        public Vec3 HalfExtents
        {
            get { return _halfExtents; }
            set { _halfExtents = value; }
        }
        public Vec3 Translation
        {
            get { return _translation; }
            set { _translation = value; }
        }
        public BoundingBox(float extentX, float extentY, float extentZ)
        {
            _halfExtents = new Vec3(extentX, extentY, extentZ);
        }
        public BoundingBox(Vec3 extents)
        {
            _halfExtents = extents / 2.0f;
        }
        public BoundingBox(float uniformExtents)
        {
            _halfExtents = new Vec3(uniformExtents / 2.0f);
        }
        public BoundingBox(Vec3 min, Vec3 max)
        {
            _translation = (max + min) / 2.0f;
            _halfExtents = (max - min) / 2.0f;
        }
        public override CollisionShape GetCollisionShape()
        {
            return new BoxShape(HalfExtents);
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
            GetCorners(Minimum, Maximum, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
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
            GetCorners(_halfExtents, transform, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
        }
        public static void GetCorners(
            Vec3 halfExtents,
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
            float Top = halfExtents.Y;
            float Bottom = -halfExtents.Y;
            float Front = halfExtents.Z;
            float Back = -halfExtents.Z;
            float Right = halfExtents.X;
            float Left = -halfExtents.X;

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
        public static Vec3[] GetCorners(Vec3 halfExtents, Matrix4 transform)
        {
            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;
            GetCorners(halfExtents, transform, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public static Vec3[] GetCorners(Vec3 boxMin, Vec3 boxMax)
        {
            Vec3 TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR;
            GetCorners(boxMin, boxMax, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public void ExpandBounds(Vec3 point)
        {
            Minimum.SetLequalTo(point);
            Maximum.SetGequalTo(point);
        }
        public override void Render()
        {
            Engine.Renderer.RenderAABB(HalfExtents, Translation, _renderSolid);
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
        public PrimitiveData GetMesh(bool includeTranslation)
        {
            if (includeTranslation)
                return Mesh(Minimum, Maximum);
            else
                return Mesh(-_halfExtents, _halfExtents);
        }
        public static Frustum GetFrustum(Vec3 min, Vec3 max)
        {
            Vec3 ftl, ftr, ntl, ntr, fbl, fbr, nbl, nbr;
            GetCorners(min, max, out ftl, out ftr, out ntl, out ntr, out fbl, out fbr, out nbl, out nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }
        public static Frustum GetFrustum(Vec3 halfExtents, Matrix4 transform)
        {
            Vec3 ftl, ftr, ntl, ntr, fbl, fbr, nbl, nbr;
            GetCorners(halfExtents, transform, out ftl, out ftr, out ntl, out ntr, out fbl, out fbr, out nbl, out nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }
        public Frustum AsFrustum() { return GetFrustum(Minimum, Maximum); }
        public Frustum AsFrustum(Matrix4 transform) { return GetFrustum(_halfExtents, transform); }
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
        public override EContainment Contains(BoundingBox box)
        {
            return Collision.AABBContainsAABB(Minimum, Maximum, box.Minimum, box.Maximum);
        }
        public override EContainment Contains(Box box)
        {
            return Collision.AABBContainsBox(Minimum, Maximum, box.HalfExtents, box.WorldMatrix);
        }
        public override EContainment Contains(Sphere sphere)
        {
            return Collision.AABBContainsSphere(Minimum, Maximum, sphere.Center, sphere.Radius);
        }
        public override EContainment ContainedWithin(BoundingBox box)
        {
            return box.Contains(this);
        }
        public override EContainment ContainedWithin(Box box)
        {
            return box.Contains(this);
        }
        public override EContainment ContainedWithin(Sphere sphere)
        {
            return sphere.Contains(this);
        }
        public override EContainment ContainedWithin(Frustum frustum)
        {
            return frustum.Contains(this);
        }
        public override void SetTransform(Matrix4 worldMatrix)
        {
            _translation = worldMatrix.GetPoint();
        }
        public override Shape HardCopy()
        {
            return new BoundingBox(_halfExtents * 2.0f);
        }
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            BoundingBox newBox = new BoundingBox(_halfExtents, _translation);
            newBox.SetTransform(worldMatrix);
            return newBox;
        }
    }
}