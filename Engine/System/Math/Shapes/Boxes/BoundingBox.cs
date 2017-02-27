using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using CustomEngine.Files;
using System.Globalization;
using BulletSharp;
using CustomEngine.Worlds.Actors.Components;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

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
        public BoundingBox()
        {

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
        public static BoundingBox FromMinMax(Vec3 min, Vec3 max)
        {
            return new BoundingBox()
            {
                _translation = (max + min) / 2.0f,
                _halfExtents = (max - min) / 2.0f,
            };
        }
        public static BoundingBox FromHalfExtentsTranslation(Vec3 halfExtents, Vec3 translation)
        {
            return new BoundingBox()
            {
                _translation = translation,
                _halfExtents = halfExtents,
            };
        }
        public override CollisionShape GetCollisionShape()
        {
            return new BoxShape(HalfExtents);
        }
        /// <summary>
        /// [T = top, B = bottom] 
        /// [B = back, F = front] 
        /// [L = left,  R = right]
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
        /// <summary>
        /// [T = top, B = bottom] 
        /// [B = back, F = front] 
        /// [L = left,  R = right]
        /// </summary>
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
        /// <summary>
        /// [T = top, B = bottom] 
        /// [B = back, F = front] 
        /// [L = left,  R = right]
        /// </summary>
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
        /// <summary>
        /// [T = top, B = bottom] 
        /// [B = back, F = front] 
        /// [L = left,  R = right]
        /// </summary>
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
            GetCorners(out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public Vec3[] GetCorners(Matrix4 transform)
        {
            GetCorners(transform, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public static Vec3[] GetCorners(Vec3 halfExtents, Matrix4 transform)
        {
            GetCorners(halfExtents, transform, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public static Vec3[] GetCorners(Vec3 boxMin, Vec3 boxMax)
        {
            GetCorners(boxMin, boxMax, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
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
        public static PrimitiveData WireframeMesh(Vec3 min, Vec3 max)
        {
            VertexLine 
                topFront, topRight, topBack, topLeft, 
                frontLeft, frontRight, backLeft, backRight,
                bottomFront, bottomRight, bottomBack, bottomLeft;

            GetCorners(min, max, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);

            topFront = new VertexLine(new Vertex(TFL), new Vertex(TFR));
            topBack = new VertexLine(new Vertex(TBL), new Vertex(TBR));
            topLeft = new VertexLine(new Vertex(TFL), new Vertex(TBL));
            topRight = new VertexLine(new Vertex(TFR), new Vertex(TBR));

            bottomFront = new VertexLine(new Vertex(BFL), new Vertex(BFR));
            bottomBack = new VertexLine(new Vertex(BBL), new Vertex(BBR));
            bottomLeft = new VertexLine(new Vertex(BFL), new Vertex(BBL));
            bottomRight = new VertexLine(new Vertex(BFR), new Vertex(BBR));

            frontLeft = new VertexLine(new Vertex(TFL), new Vertex(BFL));
            frontRight = new VertexLine(new Vertex(TFR), new Vertex(BFR));
            backLeft = new VertexLine(new Vertex(TBL), new Vertex(BBL));
            backRight = new VertexLine(new Vertex(TBR), new Vertex(BBR));

            return PrimitiveData.FromLines(new PrimitiveBufferInfo(), 
                topFront, topRight, topBack, topLeft,
                frontLeft, frontRight, backLeft, backRight,
                bottomFront, bottomRight, bottomBack, bottomLeft);
        }
        public static PrimitiveData SolidMesh(Vec3 min, Vec3 max)
        {
            VertexQuad left, right, top, bottom, front, back;

            GetCorners(min, max, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);

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
                return SolidMesh(Minimum, Maximum);
            else
                return SolidMesh(-_halfExtents, _halfExtents);
        }
        public static Frustum GetFrustum(Vec3 min, Vec3 max)
        {
            GetCorners(min, max, out Vec3 ftl, out Vec3 ftr, out Vec3 ntl, out Vec3 ntr, out Vec3 fbl, out Vec3 fbr, out Vec3 nbl, out Vec3 nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }
        public static Frustum GetFrustum(Vec3 halfExtents, Matrix4 transform)
        {
            GetCorners(halfExtents, transform, out Vec3 ftl, out Vec3 ftr, out Vec3 ntl, out Vec3 ntr, out Vec3 fbl, out Vec3 fbr, out Vec3 nbl, out Vec3 nbr);
            return new Frustum(fbl, fbr, ftl, ftr, nbl, nbr, ntl, ntr);
        }
        public Frustum AsFrustum() { return GetFrustum(Minimum, Maximum); }
        public Frustum AsFrustum(Matrix4 transform) { return GetFrustum(_halfExtents, transform); }
        public bool Intersects(Ray ray)
        {
            return Collision.RayIntersectsAABBDistance(ray.StartPoint, ray.Direction, Minimum, Maximum, out float distance);
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
            return FromMinMax(
                new Vec3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius),
                new Vec3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius));
        }
        public static BoundingBox Merge(BoundingBox box1, BoundingBox box2)
        {
            return FromMinMax(Vec3.ComponentMin(box1.Minimum, box2.Maximum), Vec3.ComponentMax(box1.Maximum, box2.Maximum));
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
            return FromHalfExtentsTranslation(_halfExtents, _translation);
        }
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            BoundingBox newBox = FromHalfExtentsTranslation(_halfExtents, _translation);
            newBox.SetTransform(worldMatrix);
            return newBox;
        }

        public unsafe override void Write(VoidPtr address, StringTable table)
        {
            *(Header*)address = this;
        }
        public unsafe override void Read(VoidPtr address, VoidPtr strings)
        {
            Header h = *(Header*)address;
            _halfExtents = h._halfExtents;
            _translation = h._translation;
        }
        public override void Write(XmlWriter writer)
        {
            writer.WriteStartElement("aabb");
            if (_halfExtents != Vec3.Zero)
                writer.WriteElementString("halfExtents", _halfExtents.ToString(false, false));
            if (_translation != Vec3.Zero)
                writer.WriteElementString("translation", _translation.ToString(false, false));
            writer.WriteEndElement();
        }
        public override void Read(XMLReader reader)
        {
            if (!reader.Name.Equals("aabb", true))
                throw new Exception();
            while (reader.BeginElement())
            {
                if (reader.Name.Equals("translation", true))
                    _translation = Vec3.Parse(reader.ReadElementString());
                else if (reader.Name.Equals("halfExtents", true))
                    _halfExtents = Vec3.Parse(reader.ReadElementString());
                reader.EndElement();
            }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public BVec3 _halfExtents;
            public BVec3 _translation;

            public static implicit operator Header(BoundingBox b)
            {
                return new Header()
                {
                    _halfExtents = b._halfExtents,
                    _translation = b._translation,
                };
            }
            public static implicit operator BoundingBox(Header h)
            {
                return FromHalfExtentsTranslation(h._halfExtents, h._translation);
            }
        }
    }
}