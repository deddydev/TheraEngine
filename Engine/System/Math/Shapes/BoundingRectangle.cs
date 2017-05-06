using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public struct BoundingRectangle
    {
        public static readonly BoundingRectangle Empty = new BoundingRectangle();

        [DefaultValue("0 0")]
        [Serialize("Translation")]
        private Vec2 _translation;
        [DefaultValue("0 0")]
        [Serialize("Bounds")]
        private Vec2 _bounds;
        [DefaultValue("0 0")]
        [Serialize("LocalOriginPercentage")]
        private Vec2 _localOriginPercentage;

        /// <summary>
        /// The origin of the component's rotation angle, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1.0,1.0 is top right.
        /// </summary>
        public Vec2 LocalOriginPercentage
        {
            get => _localOriginPercentage;
            set
            {
                Vec2 diff = value - _localOriginPercentage;
                _translation.X += diff.X * Width;
                _translation.Y += diff.Y * Height;
                _localOriginPercentage = value;
            }
        }
        public Vec2 LocalOrigin
        {
            get => _localOriginPercentage * _bounds;
            set => _localOriginPercentage = value / _bounds;
        }
        public Vec2 WorldOrigin
        {
            get => _translation + LocalOrigin;
            set => LocalOrigin = value - _translation;
        }

        public BoundingRectangle(float x, float y, float width, float height)
        {
            _localOriginPercentage = Vec2.Zero;
            _translation = new Vec2(x, y);
            _bounds = new Vec2(width, height);
        }
        public BoundingRectangle(Vec2 translation, Vec2 bounds)
        {
            _localOriginPercentage = Vec2.Zero;
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
                float origX = _bounds.X;
                _translation.X = value;
                _bounds.X = origX - _translation.X;
            }
        }
        public float MinY
        {
            get => _translation.Y;
            set
            {
                float origY = _bounds.Y;
                _translation.Y = value;
                _bounds.Y = origY - _translation.Y;
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

        public int IntX
        {
            get => (int)/*Math.Round(*/X/*)*/;
            set => X = value;
        }
        public int IntY
        {
            get => (int)/*Math.Round(*/Y/*)*/;
            set => Y = value;
        }
        public int IntWidth
        {
            get => (int)/*Math.Round(*/Width/*)*/;
            set => Width = value;
        }
        public int IntHeight
        {
            get => (int)/*Math.Round(*/Height/*)*/;
            set => Height = value;
        }

        public void Translate(Vec2 offset)
            => _translation += offset;
        
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
