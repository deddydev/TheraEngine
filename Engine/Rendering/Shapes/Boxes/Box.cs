using TheraEngine.Rendering.Models;
using System.Globalization;
using System.Drawing;
using System.ComponentModel;
using System;
using TheraEngine.Physics;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Box")]
    public class Box : Shape
    {
        public EventVec3 _halfExtents;
        public Transform _transform;

        [Category("Box")]
        [TSerialize]
        public Transform Transform
        {
            get => _transform;
            set => _transform = value;
        }
        [Category("Box")]
        [TSerialize]
        public EventVec3 HalfExtents
        {
            get => _halfExtents;
            set => _halfExtents = value;
        }
        [Browsable(false)]
        public Matrix4 WorldMatrix
        {
            get => _transform.Matrix;
            set => _transform.Matrix = value;
        }
        [Browsable(false)]
        public Matrix4 InverseWorldMatrix
        {
            get => _transform.InverseMatrix;
            set => _transform.InverseMatrix = value;
        }

        public Vec3 Center => _transform.Matrix.Translation;

        public Box(BoundingBox aabb)
            : this(aabb.HalfExtents, new Transform(aabb.Translation, Quat.Identity, Vec3.One)) { }
        public Box(float halfExtentX, float halfExtentY, float halfExtentZ) 
            : this(halfExtentX, halfExtentY, halfExtentZ, Transform.GetIdentity()) { }
        public Box(float halfExtentX, float halfExtentY, float halfExtentZ, Transform transform)
            : this()
        {
            _halfExtents = new Vec3(halfExtentX, halfExtentY, halfExtentZ);
            _transform = transform;
        }
        public Box(Vec3 halfExtents) 
            : this(halfExtents, Transform.GetIdentity()) { }
        public Box(Vec3 halfExtents, Transform transform)
            : this()
        {
            _halfExtents = halfExtents;
            _transform = transform;
        }
        public Box(float uniformHalfExtents) 
            : this(uniformHalfExtents, Transform.GetIdentity()) { }
        public Box(float uniformHalfExtents, Transform transform) 
            : this()
        {
            _halfExtents = new Vec3(uniformHalfExtents);
            _transform = transform;
        }
        public Box()
        {
            _halfExtents = Vec3.Half;
            _transform = Transform.GetIdentity();
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
            Engine.Renderer.RenderBox(_halfExtents, _transform.Matrix, _renderSolid, Color.Black);
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

            return PrimitiveData.FromQuads(VertexShaderDesc.PosNormTex(), left, right, top, bottom, front, back);
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
        //public static bool operator ==(Box left, Box right) { return left.Equals(ref right); }
        //public static bool operator !=(Box left, Box right) { return !left.Equals(ref right); }
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "HalfExtents:{0} Matrix:{1}", HalfExtents.ToString(), WorldMatrix.ToString());
        }
        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        return (HalfExtents.GetHashCode() * 397) ^ WorldMatrix.GetHashCode();
        //    }
        //}
        //public bool Equals(ref Box value)
        //{
        //    return 
        //        HalfExtents == value.HalfExtents &&
        //        WorldMatrix == value.WorldMatrix;
        //}
        //public override bool Equals(object value)
        //{
        //    if (!(value is Box))
        //        return false;

        //    var strongValue = (Box)value;
        //    return Equals(ref strongValue);
        //}
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
        public override void SetRenderTransform(Matrix4 worldMatrix)
        {
            _transform.Matrix = worldMatrix;
            //_transform = worldMatrix;
            base.SetRenderTransform(worldMatrix);
        }
        public override TCollisionShape GetCollisionShape()
        {
            return TCollisionBox.New(_halfExtents);
        }
        public override Shape HardCopy()
        {
            return new Box(_halfExtents, _transform);
        }
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            return new Box(_halfExtents, _transform);
        }

        public override Matrix4 GetTransformMatrix()
            => _transform.Matrix;

        public override Vec3 ClosestPoint(Vec3 point)
        {
            Vec3 aabbPoint = Vec3.TransformPosition(point, _transform.InverseMatrix);
            Vec3 closest = Collision.ClosestPointAABBPoint(-_halfExtents, _halfExtents, aabbPoint);
            return Vec3.TransformPosition(closest, _transform.Matrix);
        }

        public override BoundingBox GetAABB()
        {
            Vec3[] corners = GetCorners();
            Vec3 min = Vec3.ComponentMin(corners);
            Vec3 max = Vec3.ComponentMax(corners);
            return BoundingBox.FromMinMax(min, max);
        }

        public override EContainment Contains(BaseCone cone)
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(BaseCylinder cylinder)
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(BaseCapsule capsule)
        {
            Vec3 top = capsule.GetTopCenterPoint();
            Vec3 bot = capsule.GetBottomCenterPoint();
            float radius = capsule.Radius;
            Vec3 topLocal = Vec3.TransformPosition(top, _transform.InverseMatrix);
            Vec3 botLocal = Vec3.TransformPosition(bot, _transform.InverseMatrix);
            Vec3 min = Vec3.ComponentMin(topLocal, botLocal) - radius;
            Vec3 max = Vec3.ComponentMax(topLocal, botLocal) + radius;
            return Collision.AABBContainsAABB(-_halfExtents.Raw, _halfExtents.Raw, min, max);
        }
    }
}