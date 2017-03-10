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
using System.Drawing;

namespace System
{
    public class Box : Shape
    {
        public static List<Box> Active = new List<Box>();

        public Vec3 _halfExtents;
        public FrameState _transform;

        public event Action TransformChanged;

        public Matrix4 WorldMatrix
        {
            get => _transform.Matrix;
            set => _transform.Matrix = value;
        }
        public FrameState Transform
        {
            get => _transform;
            set => _transform = value;
        }
        public Vec3 HalfExtents
        {
            get { return _halfExtents; }
            set { _halfExtents = value; }
        }
        public Vec3 Center { get { return _transform.Matrix.GetPoint(); } }

        public Box(float halfExtentX, float halfExtentY, float halfExtentZ) 
            : this(halfExtentX, halfExtentY, halfExtentZ, FrameState.Identity) { }
        public Box(float halfExtentX, float halfExtentY, float halfExtentZ, FrameState transform)
            : this()
        {
            _halfExtents = new Vec3(halfExtentX, halfExtentY, halfExtentZ);
            _transform = transform;
        }
        public Box(Vec3 halfExtents) 
            : this(halfExtents, FrameState.Identity) { }
        public Box(Vec3 halfExtents, FrameState transform)
            : this()
        {
            _halfExtents = halfExtents;
            _transform = transform;
        }
        public Box(float uniformHalfExtents) 
            : this(uniformHalfExtents, FrameState.Identity) { }
        public Box(float uniformHalfExtents, FrameState transform) 
            : this()
        {
            _halfExtents = new Vec3(uniformHalfExtents);
            _transform = transform;
        }
        public Box()
        {
            ShapeIndex = Active.Count;
            Active.Add(this);
            _halfExtents = Vec3.Half;
            _transform = FrameState.Identity;
        }
        ~Box()
        {
            Active.Remove(this);
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
            BoundingBox.GetCorners(_halfExtents, WorldMatrix, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
        }
        public Vec3[] GetCorners()
        {
            BoundingBox.GetCorners(_halfExtents, WorldMatrix, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public override void Render()
        {
            Engine.Renderer.RenderBox(ShapeName, _halfExtents, _transform.Matrix, _renderSolid, Color.Black);
        }
        public static PrimitiveData Mesh(Vec3 halfExtents, Matrix4 transform)
        {
            VertexQuad left, right, top, bottom, front, back;

            BoundingBox.GetCorners(halfExtents, transform, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);

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
        public PrimitiveData GetMesh() { return Mesh(HalfExtents, WorldMatrix); }
        public Frustum AsFrustum() { return BoundingBox.GetFrustum(HalfExtents, WorldMatrix); }
        public bool Intersects(Ray ray, out float distance)
        {
            return Collision.RayIntersectsBoxDistance(ray.StartPoint, ray.Direction, HalfExtents, WorldMatrix, out distance);
        }
        public bool Intersects(Ray ray, out Vec3 point)
        {
            return Collision.RayIntersectsBox(ray.StartPoint, ray.Direction, HalfExtents, WorldMatrix, out point);
        }
        public static BoundingBox FromSphere(Sphere sphere)
        {
            return BoundingBox.FromMinMax(
                new Vec3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius),
                new Vec3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius));
        }
        public static BoundingBox Merge(BoundingBox box1, BoundingBox box2)
        {
            return BoundingBox.FromMinMax(Vec3.ComponentMin(box1.Minimum, box2.Maximum), Vec3.ComponentMax(box1.Maximum, box2.Maximum));
        }
        public static bool operator ==(Box left, Box right) { return left.Equals(ref right); }
        public static bool operator !=(Box left, Box right) { return !left.Equals(ref right); }
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "HalfExtents:{0} Matrix:{1}", HalfExtents.ToString(), WorldMatrix.ToString());
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (HalfExtents.GetHashCode() * 397) ^ WorldMatrix.GetHashCode();
            }
        }
        public bool Equals(ref Box value)
        {
            return 
                HalfExtents == value.HalfExtents &&
                WorldMatrix == value.WorldMatrix;
        }
        public override bool Equals(object value)
        {
            if (!(value is Box))
                return false;

            var strongValue = (Box)value;
            return Equals(ref strongValue);
        }
        public override bool Contains(Vec3 point)
        {
            return Collision.BoxContainsPoint(HalfExtents, WorldMatrix, point);
        }
        public override EContainment Contains(BoundingBox box)
        {
            return Collision.BoxContainsAABB(HalfExtents, WorldMatrix, box.Minimum, box.Maximum);
        }
        public override EContainment Contains(Box box)
        {
            return Collision.BoxContainsBox(HalfExtents, WorldMatrix, box.HalfExtents, box.WorldMatrix);
        }
        public override EContainment Contains(Sphere sphere)
        {
            return Collision.BoxContainsSphere(HalfExtents, WorldMatrix, sphere.Center, sphere.Radius);
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
            throw new NotImplementedException();
            //_transform = worldMatrix;
        }
        public override CollisionShape GetCollisionShape()
        {
            return new BoxShape(_halfExtents);
        }
        public override Shape HardCopy()
        {
            return new Box(_halfExtents, _transform);
        }
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            return new Box(_halfExtents, _transform);
        }
        public unsafe override void Write(VoidPtr address, StringTable table)
        {
            *(Header*)address = this;
        }
        public unsafe override void Read(VoidPtr address, VoidPtr strings)
        {
            Header h = *(Header*)address;
            _halfExtents = h._halfExtents;
            _transform = h._transform;
        }
        public override void Write(XmlWriter writer)
        {
            writer.WriteStartElement("box");
            if (_halfExtents != Vec3.Zero)
                writer.WriteElementString("halfExtents", _halfExtents.ToString(false, false));
            _transform.Write(writer);
            writer.WriteEndElement();
        }
        public override void Read(XMLReader reader)
        {
            if (!reader.Name.Equals("box", true))
                throw new Exception();
            while (reader.BeginElement())
            {
                if (reader.Name.Equals("transform", true))
                {
                    _transform = new FrameState();
                    _transform.Read(reader);
                }
                else if (reader.Name.Equals("halfExtents", true))
                    _halfExtents = Vec3.Parse(reader.ReadElementString());
                reader.EndElement();
            }
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public BVec3 _halfExtents;
            public FrameState.Header _transform;

            public static implicit operator Header(Box b)
            {
                return new Header()
                {
                    _halfExtents = b._halfExtents,
                    _transform = b._transform,
                };
            }
            public static implicit operator Box(Header h)
            {
                return new Box(h._halfExtents, h._transform);
            }
        }
    }
}