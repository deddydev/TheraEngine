using System;
using System.ComponentModel;
using System.Drawing;

namespace TheraEngine.Core.Shapes
{
    /// <summary>
    /// Axis-aligned rectangle struct. Supports position, size, and a local origin. All translations are relative to the bottom left (0, 0), like a graph.
    /// </summary>
    public class EventBoundingRectangleF
    {
        [TSerialize(nameof(Raw))]
        private BoundingRectangleFStruct _raw;

        public BoundingRectangleFStruct Raw
        {
            get => _raw;
            set
            {
                _raw = value;
            }
        }
        
        /// <summary>
        /// The relative translation of the origin from the bottom left, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1,1 is top right.
        /// </summary>
        [Description(@"The relative translation of the origin from the bottom left, as a percentage.
0,0 is bottom left, 0.5,0.5 is center, 1,1 is top right.")]
        public Vec2 LocalOriginPercentage
        {
            get => _raw.LocalOriginPercentage;
            set
            {
                _raw.LocalOriginPercentage = value;
            }
        }
        /// <summary>
        /// The actual translation of the origin of this rectangle, relative to the bottom left.
        /// </summary>
        [Description(@"The actual translation of the origin of this rectangle, relative to the bottom left.")]
        public Vec2 LocalOrigin
        {
            get => _raw.LocalOrigin;
            set
            {
                _raw.LocalOrigin = value;
            }
        }
        /// <summary>
        /// The location of the origin of this rectangle as a world point relative to the bottom left (0, 0).
        /// Bottom left point of this rectangle is Position - LocalOrigin.
        /// </summary>
        [Description(@"The location of the origin of this rectangle as a world point relative to the bottom left (0, 0).
Bottom left point of this rectangle is Position - LocalOrigin.")]
        public Vec2 OriginTranslation
        {
            get => _raw.OriginTranslation;
            set
            {
                _raw.OriginTranslation = value;
            }
        }

        public EventBoundingRectangleF() => _raw = new BoundingRectangleFStruct();
        public EventBoundingRectangleF(BoundingRectangleFStruct value) => _raw = value;
        public EventBoundingRectangleF(float x, float y, float width, float height, float localOriginPercentageX, float localOriginPercentageY)
            : this(new Vec2(x, y), new Vec2(width, height), new Vec2(localOriginPercentageX, localOriginPercentageY)) { }
        public EventBoundingRectangleF(float x, float y, float width, float height)
            : this(new Vec2(x, y), new Vec2(width, height)) { }
        public EventBoundingRectangleF(Vec2 translation, Vec2 bounds)
            : this(translation, bounds, Vec2.Zero) { }
        public EventBoundingRectangleF(Vec2 translation, Vec2 bounds, Vec2 localOriginPercentage)
        {
            _raw = new BoundingRectangleFStruct(translation, bounds, localOriginPercentage);
        }

        public static EventBoundingRectangleF FromMinMaxSides(
            float minX, float maxX,
            float minY, float maxY,
            float localOriginPercentageX, float localOriginPercentageY)
            => new EventBoundingRectangleF(minX, minY, maxX - minX, maxY - minY, localOriginPercentageX, localOriginPercentageY);

        /// <summary>
        /// The horizontal translation of this rectangle's position. 0 is fully left, positive values are right.
        /// </summary>
        public float X
        {
            get => _raw.X;
            set
            {
                _raw.X = value;
            }
        }
        /// <summary>
        /// The vertical translation of this rectangle's position. 0 is fully down, positive values are up.
        /// </summary>
        public float Y
        {
            get => _raw.Y;
            set
            {
                _raw.Y = value;
            }
        }
        /// <summary>
        /// The width of this rectangle.
        /// </summary>
        public float Width
        {
            get => _raw.Width;
            set
            {
                _raw.Width = value;
            }
        }
        /// <summary>
        /// The height of this rectangle.
        /// </summary>
        public float Height
        {
            get => _raw.Height;
            set
            {
                _raw.Height = value;
            }
        }
        /// <summary>
        /// The X value of the right boundary line.
        /// Only moves the right edge by resizing width.
        /// </summary>
        public float MaxX
        {
            get => _raw.MaxX;
            set
            {
                _raw.MaxX = value;
            }
        }
        /// <summary>
        /// The Y value of the top boundary line.
        /// Only moves the top edge by resizing height.
        /// </summary>
        public float MaxY
        {
            get => _raw.MaxY;
            set
            {
                _raw.MaxY = value;
            }
        }

        /// <summary>
        /// The X value of the left boundary line.
        /// Only moves the left edge by resizing width.
        /// </summary>
        public float MinX
        {
            get => _raw.MinX;
            set
            {
                _raw.MinX = value;
            }
        }
        /// <summary>
        /// The Y value of the bottom boundary line.
        /// Only moves the bottom edge by resizing height.
        /// </summary>
        public float MinY
        {
            get => _raw.MinY;
            set
            {
                _raw.MinY = value;
            }
        }
        /// <summary>
        /// The world position of the center point of the rectangle (regardless of the local origin).
        /// </summary>
        public Vec2 Center
        {
            get => _raw.Center;
            set
            {
                _raw.Center = value;
            }
        }
        /// <summary>
        /// The width and height of this rectangle.
        /// </summary>
        public Vec2 Extents
        {
            get => _raw.Extents;
            set
            {
                _raw.Extents = value;
            }
        }

        /// <summary>
        /// The location of this rectangle's bottom left point (top right if both width and height are negative). 0 is fully left/down, positive values are right/up.
        /// </summary>
        public Vec2 Translation
        {
            get => _raw.Translation;
            set
            {
                _raw.Translation = value;
            }
        }
        /// <summary>
        /// Bottom left point in world space regardless of width or height being negative.
        /// </summary>
        public Vec2 BottomLeft
        {
            get => _raw.BottomLeft;
            set
            {
                _raw.BottomLeft = value;
            }
        }
        /// <summary>
        /// Top right point in world space regardless of width or height being negative.
        /// </summary>
        public Vec2 TopRight
        {
            get => _raw.TopRight;
            set
            {
                _raw.TopRight = value;
            }
        }
        /// <summary>
        /// Bottom right point in world space regardless of width or height being negative.
        /// </summary>
        public Vec2 BottomRight
        {
            get => _raw.BottomRight;
            set
            {
                _raw.BottomRight = value;
            }
        }

        public RectangleF AsRectangleF(float containerHeight) => _raw.AsRectangleF(containerHeight);

        /// <summary>
        /// Top left point in world space regardless of width or height being negative.
        /// </summary>
        public Vec2 TopLeft
        {
            get => _raw.TopLeft;
            set
            {
                _raw.TopLeft = value;
            }
        }
        /// <summary>
        /// The horizontal location of this rectangle's origin, as an integer value floored from float value. 0 is fully left, positive values are right.
        /// </summary>
        public int IntX
        {
            get => _raw.IntX;
            set
            {
                _raw.IntX = value;
            }
        }
        /// <summary>
        /// The vertical location of this rectangle's origin, as an integer value floored from float value. 0 is fully down, positive values are up.
        /// </summary>
        public int IntY
        {
            get => _raw.IntY;
            set
            {
                _raw.IntY = value;
            }
        }
        /// <summary>
        /// The width of this rectangle, as an integer value floored from float value.
        /// </summary>
        public int IntWidth
        {
            get => _raw.IntWidth;
            set
            {
                _raw.IntWidth = value;
            }
        }
        /// <summary>
        /// The height of this rectangle, as an integer value floored from float value.
        /// </summary>
        public int IntHeight
        {
            get => _raw.IntHeight;
            set
            {
                _raw.IntHeight = value;
            }
        }

        public Size SizeInt
        {
            get => new Size(IntWidth, IntHeight);
            set
            {
                IntWidth = value.Width;
                IntHeight = value.Height;
            }
        }
        public SizeF Size
        {
            get => new SizeF(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Translates this rectangle relative to the current translation using an offset.
        /// </summary>
        /// <param name="offset">The translation delta to add to the current translation.</param>
        public void Translate(Vec2 offset) => _raw.Translate(offset);
        /// <summary>
        /// Checks that the width and height are positive values. Will move the location of the rectangle to fix this.
        /// </summary>
        public void CheckProperDimensions() => _raw.CheckProperDimensions();
        /// <summary>
        /// Checks if the point is contained within this rectangle.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is contained within this rectangle.</returns>
        public bool Contains(Vec2 point) => _raw.Contains(point);
        /// <summary>
        /// Determines if this rectangle is contained within another.
        /// </summary>
        /// <param name="other">The other rectangle.</param>
        /// <returns>EContainment.Disjoint if not intersecting. EContainment.Intersecting if intersecting, but not fully contained. EContainment.Contains if fully contained.</returns>
        public EContainment ContainmentWithin(BoundingRectangleFStruct other) => _raw.ContainmentWithin(other);
        /// <summary>
        /// Determines if this rectangle contains another.
        /// </summary>
        /// <param name="other">The other rectangle.</param>
        /// <returns>EContainment.Disjoint if not intersecting. EContainment.Intersecting if intersecting, but not fully contained. EContainment.Contains if fully contained.</returns>
        public EContainment ContainmentOf(BoundingRectangleFStruct other) => _raw.ContainmentOf(other);
        public bool DisjointWith(float width, float height) => _raw.DisjointWith(width, height);
        /// <summary>
        /// Returns true if this rectangle and the given rectangle are not touching or contained within another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool DisjointWith(BoundingRectangleFStruct other) => _raw.DisjointWith(other);
        /// <summary>
        /// Returns true if full contains the given rectangle. If intersecting at all (including a same edge) or disjoint, returns false.
        /// </summary>
        public bool Contains(BoundingRectangleFStruct other) => _raw.Contains(other);
        /// <summary>
        /// Returns true if intersecting at all (including a same edge). If no edges are touching, returns false.
        /// </summary>
        public bool Intersects(BoundingRectangleFStruct other) => _raw.Intersects(other);
        public override string ToString() => _raw.ToString();

        public Vec2 ClosestPoint(Vec2 point) => _raw.ClosestPoint(point);

        public bool IsEmpty() => _raw.IsEmpty();

        public static explicit operator BoundingRectangleFStruct(EventBoundingRectangleF value) => value._raw;
        public static implicit operator EventBoundingRectangleF(BoundingRectangleFStruct value) => new EventBoundingRectangleF(value);
    }
}
