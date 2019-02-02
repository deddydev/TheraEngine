using System.Drawing;
using System.ComponentModel;
using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileExt("cylinder")]
    public abstract class Cylinder : TShape
    {
        public Cylinder(
            EventVec3 center,
            Vec3 upAxis,
            float radius,
            float halfHeight)
        {
            Center = center;
            _radius = Math.Abs(radius);
            _halfHeight = Math.Abs(halfHeight);
            _upAxis = upAxis;
            _upAxis.Normalize();
        }

        protected EventVec3 _center = Vec3.Zero;
        protected Vec3 _upAxis = Vec3.Up;
        protected float _radius = 0.5f, _halfHeight = 1.0f;
        
        public Vec3 GetTopCenterPoint()
            => _center + _upAxis * _halfHeight;
        public Vec3 GetBottomCenterPoint()
            => _center - _upAxis * _halfHeight;
        public Circle3D GetBottomCircle(bool normalFacingIn = false)
            => new Circle3D(_radius, GetBottomCenterPoint(), normalFacingIn ? UpAxis : -UpAxis);
        public Circle3D GetTopCircle(bool normalFacingIn = false)
            => new Circle3D(_radius, GetTopCenterPoint(), normalFacingIn ? -UpAxis : UpAxis);
        
        [Browsable(false)]
        public EventVec3 Center
        {
            get => _center;
            set => _center = value ?? Vec3.Zero;
        }
        [Category("Cylinder")]
        public float Radius
        {
            get => _radius;
            set => _radius = Math.Abs(value);
        }
        [Category("Cylinder")]
        public float HalfHeight
        {
            get => _halfHeight;
            set => _halfHeight = value;
        }

        public Vec3 UpAxis => _upAxis;

        public override BoundingBox GetAABB()
        {
            throw new NotImplementedException();
        }
        public override bool Contains(Vec3 point)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(BoundingBox box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Cylinder cylinder)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Cone cone)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Capsule capsule)
        {
            throw new NotImplementedException();
        }
        public override Vec3 ClosestPoint(Vec3 point)
        {
            throw new NotImplementedException();
        }
        public float GetTotalHalfHeight() => _halfHeight + _radius;
        public float GetTotalHeight() => GetTotalHalfHeight() * 2.0f;
        public override void Render()
        {
            Engine.Renderer.RenderCylinder(_center.AsTranslationMatrix(), _upAxis, _radius, _halfHeight, RenderSolid, Color.Black);
        }

        public override TCollisionShape GetCollisionShape() => throw new NotImplementedException();
        public override TShape HardCopy() => throw new NotImplementedException();

        public override EContainment Contains(BoundingBoxStruct box)
        {
            throw new NotImplementedException();
        }

        public override void SetTransformMatrix(Matrix4 matrix)
        {
            Center.Raw = matrix.Translation;
        }
        public override Matrix4 GetTransformMatrix()
        {
            return Center.AsTranslationMatrix();
        }
    }
}
