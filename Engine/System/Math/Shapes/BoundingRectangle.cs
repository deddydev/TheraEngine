using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public struct BoundingRectangle
    {
        public static readonly BoundingRectangle Empty = new BoundingRectangle();

        private Vec2 _translation, _bounds;

        public BoundingRectangle(float x, float y, float width, float height)
        {
            _translation = new Vec2(x, y);
            _bounds = new Vec2(width, height);
        }
        public BoundingRectangle(Vec2 translation, Vec2 bounds)
        {
            _translation = translation;
            _bounds = bounds;
        }

        public float X
        {
            get => _translation.X;
            set => _translation.X = value;
        }
        public float Y
        {
            get => _translation.Y;
            set => _translation.Y = value;
        }
        public float Width
        {
            get => _bounds.X;
            set => _bounds.X = value;
        }
        public float Height
        {
            get => _bounds.Y;
            set => _bounds.Y = value;
        }

        public float MaxX
        {
            get => _translation.X + _bounds.X;
            set => _bounds.X = value - _translation.X;
        }
        public float MaxY
        {
            get => _translation.Y + _bounds.Y;
            set => _bounds.Y = value - _translation.Y;
        }
        public float MinX
        {
            get => _translation.X;
            set
            {

            }
        }
        public float MinY
        {
            get => _translation.Y;
            set
            {

            }
        }
        public Vec2 Center
        {
            get => _translation + (_bounds / 2.0f);
            set => _translation = value - (_bounds / 2.0f);
        }
        public Vec2 Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }
        public Vec2 Translation
        {
            get => _translation;
            set => _translation = value;
        }
        public Vec2 BottomLeft
        {
            get => _translation;
            set
            {
                Vec2 upper = TopRight;
                _translation = value;
                _bounds = TopRight - _translation;
            }
        }
        public Vec2 TopRight
        {
            get => _translation + _bounds;
            set => _bounds = value - _translation;
        }
        public Vec2 BottomRight
        {
            get => new Vec2(_translation.X + _bounds.X, _translation.Y);
            set
            {
                float upperY = _translation.Y + _bounds.Y;
                _translation.Y = value.Y;
                _bounds.X = value.X - _translation.X;
                _bounds.Y = upperY - value.Y;
            }
        }
        public Vec2 TopLeft
        {
            get => new Vec2(_translation.X, _translation.Y + _bounds.Y);
            set
            {
                float upperX = _translation.X + _bounds.X;
                _translation.X = value.X;
                _bounds.X = upperX - value.X;
                _bounds.Y = value.Y - _translation.Y;
            }
        }
        public void Translate(Vec2 offset)
        {
            _translation += offset;
        }
        public void AssertProperDimensions()
        {
            if (_bounds.X < 0)
            {
                _translation.X += _bounds.X;
                _bounds.X = -_bounds.X;
            }
            if (_bounds.Y < 0)
            {
                _translation.Y += _bounds.Y;
                _bounds.Y = -_bounds.Y;
            }
        }

        public bool Contains(Vec2 point)
            => _bounds.Contains(point - _translation);
        public EContainment Contains(BoundingRectangle bounds)
        {
            int flag = 0;
            
            float r = MaxX - bounds.MaxX;
            float t = MaxY - bounds.MaxY;
            float l = bounds.MinX - MinX;
            float b = bounds.MinY - MinY;

            //contains right side?
            flag |= r > 0 ? 0b0001 : 0;
            //contains top side?
            flag |= t > 0 ? 0b0010 : 0;
            //contains left side?
            flag |= l > 0 ? 0b0100 : 0;
            //contains bottom side?
            flag |= b > 0 ? 0b1000 : 0;

            if (flag == 0b1111)
                return EContainment.Contains;
            else
                return EContainment.Intersects;
        }
    }
}
