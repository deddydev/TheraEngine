﻿using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Core.Shapes
{
    /// <summary>
    /// Axis-aligned rectangle struct. Supports position, size, and a local origin. All translations are relative to the bottom left (0, 0), like a graph.
    /// </summary>
    [Serializable]
    public struct BoundingRectangle
    {
        /// <summary>
        /// A rectangle with a location at 0,0 (bottom left), a size of 0, and a local origin at the bottom left.
        /// </summary>
        public static readonly BoundingRectangle Empty = new BoundingRectangle();
        
        [TSerialize("Translation")]
        private IVec2 _translation;
        [TSerialize("Bounds")]
        private IVec2 _bounds;
        [TSerialize("LocalOriginPercentage")]
        private Vec2 _localOriginPercentage;

        /// <summary>
        /// The relative translation of the origin from the bottom left, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1,1 is top right.
        /// </summary>
        [Description(
            "The relative translation of the origin from the bottom left, as a percentage." +
            "0,0 is bottom left, 0.5,0.5 is center, 1,1 is top right.")]
        public Vec2 LocalOriginPercentage
        {
            get => _localOriginPercentage;
            set
            {
                Vec2 diff = value - _localOriginPercentage;

                Vec2 trans = _translation;
                trans += diff * _bounds;
                _translation = (IVec2)trans;

                _localOriginPercentage = value;
            }
        }
        /// <summary>
        /// The actual translation of the origin of this rectangle, relative to the bottom left.
        /// </summary>
        [Description(@"The actual translation of the origin of this rectangle, relative to the bottom left.")]
        public IVec2 LocalOrigin
        {
            get => (IVec2)(_localOriginPercentage * _bounds);
            set => _localOriginPercentage = value / _bounds;
        }
        /// <summary>
        /// The location of the origin of this rectangle as a world point relative to the bottom left (0, 0).
        /// Bottom left point of this rectangle is Position - LocalOrigin.
        /// </summary>
        [Description(@"The location of the origin of this rectangle as a world point relative to the bottom left (0, 0).
Bottom left point of this rectangle is Position - LocalOrigin.")]
        public IVec2 OriginTranslation
        {
            get => _translation + LocalOrigin;
            set => _translation = value - LocalOrigin;
        }

        public BoundingRectangle(int x, int y, int width, int height, float localOriginPercentageX, float localOriginPercentageY)
            : this(new IVec2(x, y), new IVec2(width, height), new Vec2(localOriginPercentageX, localOriginPercentageY)) { }
        public BoundingRectangle(int x, int y, int width, int height)
            : this(new IVec2(x, y), new IVec2(width, height)) { }
        public BoundingRectangle(IVec2 translation, IVec2 bounds)
            : this(translation, bounds, Vec2.Zero) { }
        public BoundingRectangle(IVec2 translation, IVec2 bounds, Vec2 localOriginPercentage)
        {
            _localOriginPercentage = localOriginPercentage;
            _bounds = bounds;
            _translation = (IVec2)(translation - (localOriginPercentage * bounds));
        }

        public static BoundingRectangle FromMinMaxSides(
            int minX, int maxX,
            int minY, int maxY,
            float localOriginPercentageX, float localOriginPercentageY)
            => new BoundingRectangle(minX, minY, maxX - minX, maxY - minY, localOriginPercentageX, localOriginPercentageY);

        /// <summary>
        /// The horizontal translation of this rectangle's position. 0 is fully left, positive values are right.
        /// </summary>
        public int X
        {
            get => OriginTranslation.X;
            set => _translation.X = value - LocalOrigin.X;
        }
        /// <summary>
        /// The vertical translation of this rectangle's position. 0 is fully down, positive values are up.
        /// </summary>
        public int Y
        {
            get => OriginTranslation.Y;
            set => _translation.Y = value - LocalOrigin.Y;
        }
        /// <summary>
        /// The width of this rectangle.
        /// </summary>
        public int Width
        {
            get => _bounds.X;
            set
            {
                _bounds.X = value;
                CheckProperDimensions();
            }
        }
        /// <summary>
        /// The height of this rectangle.
        /// </summary>
        public int Height
        {
            get => _bounds.Y;
            set
            {
                _bounds.Y = value;
                CheckProperDimensions();
            }
        }
        /// <summary>
        /// The X value of the right boundary line.
        /// Only moves the right edge by resizing width.
        /// </summary>
        public int MaxX
        {
            get => _translation.X + (Width < 0 ? 0 : Width);
            set
            {
                CheckProperDimensions();
                _bounds.X = value - _translation.X;
            }
        }
        /// <summary>
        /// The Y value of the top boundary line.
        /// Only moves the top edge by resizing height.
        /// </summary>
        public int MaxY
        {
            get => _translation.Y + (Height < 0 ? 0 : Height);
            set
            {
                CheckProperDimensions();
                _bounds.Y = value - _translation.Y;
            }
        }

        /// <summary>
        /// The X value of the left boundary line.
        /// Only moves the left edge by resizing width.
        /// </summary>
        public int MinX
        {
            get => _translation.X + (Width < 0 ? Width : 0);
            set
            {
                CheckProperDimensions();
                int origX = _bounds.X;
                _translation.X = value;
                _bounds.X = origX - _translation.X;
            }
        }
        /// <summary>
        /// The Y value of the bottom boundary line.
        /// Only moves the bottom edge by resizing height.
        /// </summary>
        public int MinY
        {
            get => _translation.Y + (Height < 0 ? Height : 0);
            set
            {
                CheckProperDimensions();
                int origY = Height;
                _translation.Y = value;
                Height = origY - _translation.Y;
            }
        }
        /// <summary>
        /// The world position of the center point of the rectangle (regardless of the local origin).
        /// </summary>
        public IVec2 Center
        {
            get => _translation + (_bounds / 2);
            set => _translation = value - (_bounds / 2);
        }
        /// <summary>
        /// The width and height of this rectangle.
        /// </summary>
        public IVec2 Extents
        {
            get => _bounds;
            set => _bounds = value;
        }
        /// <summary>
        /// The location of this rectangle's bottom left point (top right if both width and height are negative). 0 is fully left/down, positive values are right/up.
        /// </summary>
        public IVec2 Translation
        {
            get => _translation;
            set => _translation = value;
        }
        /// <summary>
        /// Bottom left point in world space regardless of width or height being negative.
        /// </summary>
        public IVec2 BottomLeft
        {
            get => new IVec2(MinX, MinY);
            set
            {
                CheckProperDimensions();
                IVec2 upper = TopRight;
                _translation = value;
                _bounds = TopRight - _translation;
            }
        }
        /// <summary>
        /// Top right point in world space regardless of width or height being negative.
        /// </summary>
        public IVec2 TopRight
        {
            get => new IVec2(MaxX, MaxY);
            set
            {
                CheckProperDimensions();
                _bounds = value - _translation;
            }
        }
        /// <summary>
        /// Bottom right point in world space regardless of width or height being negative.
        /// </summary>
        public IVec2 BottomRight
        {
            get => new IVec2(MaxX, MinY);
            set
            {
                CheckProperDimensions();
                int upperY = _translation.Y + _bounds.Y;
                _translation.Y = value.Y;
                _bounds.X = value.X - _translation.X;
                _bounds.Y = upperY - value.Y;
            }
        }

        public Rectangle AsRectangle(int containerHeight)
        {
            IVec2 pos = TopLeft;
            return new Rectangle(pos.X, containerHeight - pos.Y, Width, Height);
        }

        /// <summary>
        /// Top left point in world space regardless of width or height being negative.
        /// </summary>
        public IVec2 TopLeft
        {
            get => new IVec2(MinX, MaxY);
            set
            {
                CheckProperDimensions();
                int upperX = _translation.X + _bounds.X;
                _translation.X = value.X;
                _bounds.X = upperX - value.X;
                _bounds.Y = value.Y - _translation.Y;
            }
        }
        /// <summary>
        /// Translates this rectangle relative to the current translation using an offset.
        /// </summary>
        /// <param name="offset">The translation delta to add to the current translation.</param>
        public void Translate(IVec2 offset)
            => _translation += offset;
        /// <summary>
        /// Checks that the width and height are positive values. Will move the location of the rectangle to fix this.
        /// </summary>
        public void CheckProperDimensions()
        {
            if (Width < 0)
            {
                _translation.X += Width;
                Width = -Width;
            }
            if (Height < 0)
            {
                _translation.Y += Height;
                Height = -Height;
            }
        }
        /// <summary>
        /// Checks if the point is contained within this rectangle.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is contained within this rectangle.</returns>
        public bool Contains(IVec2 point)
            => _bounds.Contains(point - BottomLeft);
        /// <summary>
        /// Determines if this rectangle is contained within another.
        /// </summary>
        /// <param name="other">The other rectangle.</param>
        /// <returns>EContainment.Disjoint if not intersecting. EContainment.Intersecting if intersecting, but not fully contained. EContainment.Contains if fully contained.</returns>
        public EContainment ContainmentWithin(BoundingRectangle other)
            => other.ContainmentOf(this);
        /// <summary>
        /// Determines if this rectangle contains another.
        /// </summary>
        /// <param name="other">The other rectangle.</param>
        /// <returns>EContainment.Disjoint if not intersecting. EContainment.Intersecting if intersecting, but not fully contained. EContainment.Contains if fully contained.</returns>
        public EContainment ContainmentOf(BoundingRectangle other)
        {
            if (Intersects(other))
                return EContainment.Intersects;
            return Contains(other) ? EContainment.Contains : EContainment.Disjoint;
        }
        public bool DisjointWith(float width, float height)
        {
            //Can't just negate contains operation, because that would also include intersection.
            return
                0 > MaxX || width < MinX ||
                0 > MaxY || height < MinY;
        }
        /// <summary>
        /// Returns true if this rectangle and the given rectangle are not touching or contained within another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool DisjointWith(BoundingRectangle other)
        {
            //Can't just negate contains operation, because that would also include intersection.
            return
                other.MinX > MaxX ||
                other.MaxX < MinX ||
                other.MinY > MaxY ||
                other.MaxY < MinY;
        }
        /// <summary>
        /// Returns true if full contains the given rectangle. If intersecting at all (including a same edge) or disjoint, returns false.
        /// </summary>
        public bool Contains(BoundingRectangle other)
        {
            return
                other.MaxX <= MaxX &&
                other.MinX >= MinX &&
                other.MaxY <= MaxY &&
                other.MinY >= MinY;
        }
        /// <summary>
        /// Returns true if intersecting at all (including a same edge). If no edges are touching, returns false.
        /// </summary>
        public bool Intersects(BoundingRectangle other)
        {
            return !Contains(other) && !DisjointWith(other);
            //MinX <= other.MaxX &&
            //MaxX >= other.MinX &&
            //MinY <= other.MaxY &&
            //MaxY >= other.MinY;
        }
        public override string ToString()
        {
            return string.Format("[X:{0} Y:{1} W:{2} H:{3}]", OriginTranslation.X, OriginTranslation.Y, Width, Height);
        }

        public IVec2 ClosestPoint(IVec2 point) => point.Clamped(BottomLeft, TopRight);

        public bool IsEmpty() => Height == 0 || Width == 0;
    }
}
