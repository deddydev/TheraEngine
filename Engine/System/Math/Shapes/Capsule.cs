﻿using static System.Math;
using CustomEngine;
using CustomEngine.Rendering.Models;

namespace System
{
    public class Capsule : IShape
    {
        public float _radius, _halfHeight;
        
        public float Radius { get { return _radius; } set { _radius = value; } }
        public float HalfHeight { get { return _halfHeight; } set { _halfHeight = value; } }

        public Capsule(float radius, float halfHeight)
        {
            _radius = Abs(radius);
            _halfHeight = Abs(halfHeight);
        }
        public bool ContainsPoint(Vec3 point)
        {
            float totalHalfHeight = GetTotalHalfHeight();
            if (point.Z < totalHalfHeight && point.Z > -totalHalfHeight)
            {
                //Adjust Z to origin
                if (point.Z > _halfHeight)
                    point.Z -= _halfHeight;
                else if (point.Z < -_halfHeight)
                    point.Z += _halfHeight;
                return Abs(point.LengthSquared) < _radius * _radius;
            }
            return false;
        }
        public ContainsShape ContainsBox(Box box)
        {
            return ContainsShape.No;
        }
        public float GetTotalHalfHeight()
        {
            return _halfHeight + _radius;
        }
        public float GetTotalHeight()
        {
            return GetTotalHalfHeight() * 2.0f;
        }
        public void Render(float delta) { Render(delta, false); }
        public void Render(float delta, bool solid)
        {
            if (solid)
                Engine.Renderer.DrawCapsuleSolid(this);
            else
                Engine.Renderer.DrawCapsuleWireframe(this);
        }

        public PrimitiveData GetPrimitives()
        {
            throw new NotImplementedException();
        }
    }
}
