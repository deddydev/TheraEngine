using BulletSharp;
using CustomEngine;
using System;
using System.Collections.Generic;
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
            _upAxis = upAxis;
            _upAxis.NormalizeFast();
            _center = center;
        }

        protected Vec3 _upAxis, _center;
        protected float _radius, _halfHeight;
        
        public Vec3 Center
        {
            get => _center;
            set => _center = value;
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
        public Vec3 UpAxis => _upAxis;

        public float GetTotalHalfHeight() => _halfHeight + _radius;
        public float GetTotalHeight() => GetTotalHalfHeight() * 2.0f;
        public override void Render()
        {
            Engine.Renderer.RenderCylinder(
                _center + _upAxis * _halfHeight,
                _center - _upAxis * _halfHeight,
                _radius, _radius, _renderSolid);
        }
    }
}
