using System.Drawing;
using System.ComponentModel;
using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileExt("cylinder")]
    public class Cylinder : Shape
    {
        public Cylinder(
            Transform transform,
            Vec3 upAxis,
            float radius,
            float halfHeight)
        {
            _radius = Math.Abs(radius);
            _halfHeight = Math.Abs(halfHeight);
            _localUpAxis = upAxis;
            _localUpAxis.Normalize();
            Transform = transform;
        }
        protected Vec3 _localUpAxis = Vec3.Up;
        protected float _radius = 0.5f, _halfHeight = 1.0f;
        
        public Vec3 GetTopCenterPoint()
            => Vec3.TransformPosition(_localUpAxis * _halfHeight, _transform.Matrix);
        public Vec3 GetBottomCenterPoint()
            => Vec3.TransformPosition(_localUpAxis * -_halfHeight, _transform.Matrix);
        public Circle3D GetBottomCircle(bool normalFacingIn = false)
            => new Circle3D(_radius, GetBottomCenterPoint(), normalFacingIn ? WorldUpAxis : -WorldUpAxis);
        public Circle3D GetTopCircle(bool normalFacingIn = false)
            => new Circle3D(_radius, GetTopCenterPoint(), normalFacingIn ? -WorldUpAxis : WorldUpAxis);
        
        [Browsable(false)]
        public Vec3 Center
        {
            get => _transform.Translation;
            set => _transform.Translation = value;
        }
        [Category("Cylinder")]
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        [Category("Cylinder")]
        public float HalfHeight
        {
            get => _halfHeight;
            set => _halfHeight = value;
        }

        public Vec3 LocalUpAxis => _localUpAxis;
        public Vec3 WorldUpAxis => Vec3.TransformNormalInverse(_localUpAxis, _transform.InverseMatrix);

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
            Engine.Renderer.RenderCylinder(_transform.Matrix, _localUpAxis, _radius, _halfHeight, _renderSolid, Color.Black);
        }

        public override TCollisionShape GetCollisionShape() => throw new NotImplementedException();
        public override Shape HardCopy() => throw new NotImplementedException();
    }
}
