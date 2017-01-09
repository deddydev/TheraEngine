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
        }

        protected Vec3 _upAxis, _center;
        protected float _radius, _halfHeight;

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        public float HalfHeight
        {
            get { return _halfHeight; }
            set { _halfHeight = value; }
        }
        public Vec3 UpAxis
        {
            get { return _upAxis; }
        }

        public float GetTotalHalfHeight() { return _halfHeight + _radius; }
        public float GetTotalHeight() { return GetTotalHalfHeight() * 2.0f; }
        public override void Render()
        {
            Engine.Renderer.RenderCylinder(
                _center + _upAxis * _halfHeight,
                _center - _upAxis * _halfHeight,
                _radius, _radius, _renderSolid);
        }
    }
}
