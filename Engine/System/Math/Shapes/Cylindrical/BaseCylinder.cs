using BulletSharp;
using CustomEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public abstract class BaseCylinder : Shape
    {
        public BaseCylinder(Vec3 center, Vec3 upAxis, float radius, float halfHeight)
        {
            _radius = Math.Abs(radius);
            _halfHeight = Math.Abs(halfHeight);
            _localUpAxis = upAxis;
            _localUpAxis.NormalizeFast();
            _localCenter = center;
        }

        protected Vec3 _localUpAxis, _localCenter;
        protected float _radius, _halfHeight;
        protected Matrix4 _transform = Matrix4.Identity;
        
        public Vec3 GetTopCenterPoint()
        {
            return _transform * (_localCenter + _localUpAxis * _halfHeight);
        }
        public Vec3 GetBottomCenterPoint()
        {
            return _transform * (_localCenter - _localUpAxis * _halfHeight);
        }
        public Circle GetBottomCircle(bool normalFacingIn = false)
        {
            return new Circle(_radius, GetBottomCenterPoint(), normalFacingIn ? WorldUpAxis : -WorldUpAxis);
        }
        public Circle GetTopCircle(bool normalFacingIn = false)
        {
            return new Circle(_radius, GetTopCenterPoint(), normalFacingIn ? -WorldUpAxis : WorldUpAxis);
        }
        public Vec3 Center
        {
            get => _localCenter;
            set => _localCenter = value;
        }
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        public float HalfHeight
        {
            get => _halfHeight;
            set => _halfHeight = value;
        }

        public Vec3 LocalUpAxis => _localUpAxis;
        public Vec3 WorldUpAxis => Vec3.TransformNormal(_localUpAxis, _transform);
        public override void SetTransform(Matrix4 worldMatrix)
        {
            _transform = worldMatrix;
        }
        public float GetTotalHalfHeight() => _halfHeight + _radius;
        public float GetTotalHeight() => GetTotalHalfHeight() * 2.0f;
        public override void Render()
        {
            Engine.Renderer.RenderCylinder(
                ShapeName, _transform, _localCenter, _localUpAxis, _radius, _halfHeight, _renderSolid, Color.Black);
        }
        public override EContainment ContainedWithin(Frustum frustum)
        {
            return EContainment.Disjoint;
        }
    }
}
