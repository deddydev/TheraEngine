using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using System.Globalization;
using BulletSharp;
using System.Drawing;
using System.ComponentModel;

namespace System
{
    public class Box : Shape
    {
        public static List<Box> Active = new List<Box>();

        [DefaultValue("0 0 0")]
        [Serialize("HalfExtents")]
        public Vec3 _halfExtents;
        [Serialize("Transform")]
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

        public Box(BoundingBox aabb)
            : this(aabb.HalfExtents, new FrameState(aabb.Translation, Quat.Identity, Vec3.One)) { }
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
            _transform.Matrix = worldMatrix;
            //_transform = worldMatrix;
            base.SetTransform(worldMatrix);
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
    }
}