using TheraEngine.Rendering.Models;
using System.Globalization;
using System.Drawing;
using System.ComponentModel;
using System;
using TheraEngine.Physics;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Shapes;
using TheraEngine.ComponentModel;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Box")]
    public class Box : TShape
    {
        public event Action<Box, EventVec3> HalfExtentsPreSet;
        public event Action<Box, EventVec3> HalfExtentsPostSet;

        public EventVec3 _halfExtents;

        [Category("Box")]
        [TSerialize]
        public EventVec3 HalfExtents
        {
            get => _halfExtents;
            set
            {
                HalfExtentsPreSet?.Invoke(this, _halfExtents);
                if (_halfExtents != null)
                    _halfExtents.Changed -= OnVolumePropertyChanged;
                _halfExtents = value ?? new EventVec3();
                _halfExtents.Changed += OnVolumePropertyChanged;
                HalfExtentsPostSet?.Invoke(this, _halfExtents);
                OnVolumePropertyChanged();
            }
        }
        
        private TTransform _transform;
        public TTransform Transform
        {
            get => _transform;
            set
            {
                //if (_transform != null)
                //    _transform.MatrixChanged -= _transform_MatrixChanged;
                _transform = value ?? TTransform.GetIdentity();
                //_transform.MatrixChanged += _transform_MatrixChanged;
            }
        }
        //private void _transform_MatrixChanged(Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        //{

        //}
        public override void SetTransformMatrix(Matrix4 matrix) => _transform.Matrix = matrix;
        public override Matrix4 GetTransformMatrix() => _transform.Matrix;

        public Vec3 Center => _transform.Matrix.Translation;

        public Box(BoundingBox aabb)
            : this(aabb.HalfExtents, new TTransform(aabb.Translation, Quat.Identity, Vec3.One)) { }
        public Box(float halfExtentX, float halfExtentY, float halfExtentZ) 
            : this(halfExtentX, halfExtentY, halfExtentZ, TTransform.GetIdentity()) { }
        public Box(float halfExtentX, float halfExtentY, float halfExtentZ, TTransform transform)
            : this()
        {
            HalfExtents = new Vec3(halfExtentX, halfExtentY, halfExtentZ);
            _transform = transform;
        }
        public Box(Vec3 halfExtents) 
            : this(halfExtents, TTransform.GetIdentity()) { }
        public Box(Vec3 halfExtents, TTransform transform)
            : this()
        {
            HalfExtents = halfExtents;
            _transform = transform;
        }
        public Box(float uniformHalfExtents) 
            : this(uniformHalfExtents, TTransform.GetIdentity()) { }
        public Box(float uniformHalfExtents, TTransform transform) 
            : this()
        {
            HalfExtents = new Vec3(uniformHalfExtents);
            _transform = transform;
        }
        public Box()
        {
            HalfExtents = Vec3.Half;
            _transform = TTransform.GetIdentity();
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
            BoundingBox.GetCorners(_halfExtents, Transform.Matrix, out TBL, out TBR, out TFL, out TFR, out BBL, out BBR, out BFL, out BFR);
        }
        public Vec3[] GetCorners()
        {
            BoundingBox.GetCorners(_halfExtents, Transform.Matrix, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);
            return new Vec3[] { TBL, TBR, TFL, TFR, BBL, BBR, BFL, BFR };
        }
        public override void Render(bool shadowPass)
        {
            Engine.Renderer.RenderBox(_halfExtents, _transform.Matrix, RenderSolid, Color.Black);
        }
        public static TMesh Mesh(Vec3 halfExtents, Matrix4 transform)
        {
            TVertexQuad left, right, top, bottom, front, back;

            BoundingBox.GetCorners(halfExtents, transform, out Vec3 TBL, out Vec3 TBR, out Vec3 TFL, out Vec3 TFR, out Vec3 BBL, out Vec3 BBR, out Vec3 BFL, out Vec3 BFR);

            Vec3 rightNormal = Vec3.Right;
            Vec3 frontNormal = Vec3.Forward;
            Vec3 topNormal = Vec3.Up;
            Vec3 leftNormal = -rightNormal;
            Vec3 backNormal = -frontNormal;
            Vec3 bottomNormal = -topNormal;

            left = TVertexQuad.Make(BBL, BFL, TFL, TBL, leftNormal);
            right = TVertexQuad.Make(BFR, BBR, TBR, TFR, rightNormal);
            top = TVertexQuad.Make(TFL, TFR, TBR, TBL, topNormal);
            bottom = TVertexQuad.Make(BBL, BBR, BFR, BFL, bottomNormal);
            front = TVertexQuad.Make(BFL, BFR, TFR, TFL, frontNormal);
            back = TVertexQuad.Make(BBR, BBL, TBL, TBR, backNormal);

            return Rendering.Models.TMesh.Create(VertexShaderDesc.PosNormTex(), left, right, top, bottom, front, back);
        }

        public TMesh GetMesh()
            => Mesh(HalfExtents, Transform.Matrix);
        public IFrustum AsFrustum() 
            => BoundingBox.GetFrustum(HalfExtents, Transform.Matrix);

        public bool Intersects(Ray ray, out float distance)
        {
            return Collision.RayIntersectsBoxDistance(ray.StartPoint, ray.Direction, HalfExtents, Transform.Matrix, out distance);
        }
        public bool Intersects(Ray ray, out Vec3 point)
        {
            return Collision.RayIntersectsBox(ray.StartPoint, ray.Direction, HalfExtents, Transform.Matrix, out point);
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
            return string.Format(CultureInfo.CurrentCulture, "HalfExtents:{0} Matrix:{1}", HalfExtents.ToString(), Transform.Matrix.ToString());
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
            return Collision.BoxContainsPoint(HalfExtents, Transform.Matrix, point);
        }
        public override EContainment Contains(BoundingBoxStruct box)
        {
            return Collision.BoxContainsAABB(HalfExtents, Transform.Matrix, box.Minimum, box.Maximum);
        }
        public override EContainment Contains(BoundingBox box)
        {
            return Collision.BoxContainsAABB(HalfExtents, Transform.Matrix, box.Minimum, box.Maximum);
        }
        public override EContainment Contains(Box box)
        {
            return Collision.BoxContainsBox(HalfExtents, Transform.Matrix, box.HalfExtents, box.Transform.Matrix);
        }
        public override EContainment Contains(Sphere sphere)
        {
            return Collision.BoxContainsSphere(HalfExtents, Transform.Matrix, sphere.Center, sphere.Radius);
        }
        public override TCollisionShape GetCollisionShape()
        {
            return TCollisionBox.New(_halfExtents);
        }
        public override TShape HardCopy()
        {
            return new Box(_halfExtents, _transform);
        }

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
        public override EContainment Contains(Cone cone)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Cylinder cylinder)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Capsule capsule)
        {
            Vec3 top = capsule.GetTopCenterPoint();
            Vec3 bot = capsule.GetBottomCenterPoint();
            float radius = capsule.Radius;
            Vec3 topLocal = Vec3.TransformPosition(top, _transform.InverseMatrix);
            Vec3 botLocal = Vec3.TransformPosition(bot, _transform.InverseMatrix);
            Vec3 min = Vec3.ComponentMin(topLocal, botLocal) - radius;
            Vec3 max = Vec3.ComponentMax(topLocal, botLocal) + radius;
            return Collision.AABBContainsAABB(-_halfExtents.Value, _halfExtents.Value, min, max);
        }
    }
}