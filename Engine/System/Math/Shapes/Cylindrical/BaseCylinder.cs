using BulletSharp;
using TheraEngine;
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
        public BaseCylinder(Vec3 center, Rotator rotation, Vec3 scale, Vec3 upAxis, float radius, float halfHeight)
        {
            _radius = Math.Abs(radius);
            _halfHeight = Math.Abs(halfHeight);
            _localUpAxis = upAxis;
            _localUpAxis.NormalizeFast();
            _state.Translation = center;
            _state.Rotation = rotation;
            _state.Scale = scale;
        }

        public FrameState State
        {
            get => _state;
            set => _state = value;
        }

        protected FrameState _state = FrameState.GetIdentity(TransformOrder.TRS, Rotator.Order.YPR);
        
        protected Vec3 _localUpAxis = Vec3.Up;
        protected float _radius = 0.5f, _halfHeight = 1.0f;
        
        public Vec3 GetTopCenterPoint()
            => Vec3.TransformPosition(_localUpAxis * _halfHeight, _state.Matrix);
        public Vec3 GetBottomCenterPoint()
            => Vec3.TransformPosition(_localUpAxis * -_halfHeight, _state.Matrix);
        public Circle3D GetBottomCircle(bool normalFacingIn = false)
            => new Circle3D(_radius, GetBottomCenterPoint(), normalFacingIn ? WorldUpAxis : -WorldUpAxis);
        public Circle3D GetTopCircle(bool normalFacingIn = false)
            => new Circle3D(_radius, GetTopCenterPoint(), normalFacingIn ? -WorldUpAxis : WorldUpAxis);
        
        public Vec3 Center
        {
            get => _state.Translation;
            set => _state.Translation = value;
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
        public Vec3 WorldUpAxis => Vec3.TransformNormalInverse(_localUpAxis, _state.InverseMatrix);

        public override void SetTransform(Matrix4 worldMatrix)
        {
            _state.Matrix = worldMatrix;
            base.SetTransform(worldMatrix);
        }
        public float GetTotalHalfHeight() => _halfHeight + _radius;
        public float GetTotalHeight() => GetTotalHalfHeight() * 2.0f;
        public override void Render()
        {
            Engine.Renderer.RenderCylinder(_state.Matrix, _localUpAxis, _radius, _halfHeight, _renderSolid, Color.Black);
        }
    }
}
