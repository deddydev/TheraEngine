using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using static System.CustomMath;
using static System.Math;

namespace System
{
    [Serializable]
    public unsafe class EventVec2 : IEquatable<EventVec2>, IUniformable2Float, IBufferable
    {
        private int _updating = 0;
        private float _oldX, _oldY;
        private Vec2 _data;

        public float X
        {
            get { return _data.X; }
            set
            {
                BeginUpdate();
                _data.X = value;
                EndUpdate();
            }
        }
        public float Y
        {
            get { return _data.Y; }
            set
            {
                BeginUpdate();
                _data.Y = value;
                EndUpdate();
            }
        }

        public float* Data { get { return _data.Data; } }
        public VoidPtr Address { get { return _data.Address; } }
        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Float; } }
        public int ComponentCount { get { return 2; } }
        bool IBufferable.Normalize { get { return false; } }
        public void Write(VoidPtr address)
        {
            _data.Write(address);
        }
        public void Read(VoidPtr address)
        {
            BeginUpdate();
            _data.Read(address);
            EndUpdate();
        }

        public EventVec2(float value)
        {
            X = value;
            Y = value;
        }
        public EventVec2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public EventVec2(Vec3 v, bool normalizeWithZ)
        {
            if (normalizeWithZ)
                _data = new Vec2(v.X / v.Z, v.Y / v.Z);
            else
                _data = new Vec2(v.X, v.Y);
        }

        public event Action XChanged;
        public event Action YChanged;
        public event ValueChange XValueChanged;
        public event ValueChange YValueChanged;
        public event Action Changed;

        private void BeginUpdate()
        {
            ++_updating;
            _oldX = X;
            _oldY = Y;
        }
        private void EndUpdate()
        {
            --_updating;
            if (_updating > 0)
                return;
            bool anyChanged = false;
            if (X != _oldX)
            {
                XChanged?.Invoke();
                XValueChanged?.Invoke(_oldX, X);
                anyChanged = true;
            }
            if (Y != _oldY)
            {
                YChanged?.Invoke();
                YValueChanged?.Invoke(_oldY, Y);
                anyChanged = true;
            }
            if (anyChanged)
                Changed?.Invoke();
        }

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 1)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 1)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        public float Length { get { return _data.Length; } }
        public float LengthFast { get { return _data.LengthFast; } }
        public float LengthSquared { get { return _data.LengthSquared; } }

        public float DistanceToSquared(EventVec2 otherPoint) { return _data.DistanceToSquared(otherPoint._data); }
        public float DistanceToFast(EventVec2 otherPoint) { return _data.DistanceToFast(otherPoint._data); }
        public float DistanceTo(EventVec2 otherPoint) { return _data.DistanceTo(otherPoint._data); }

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        public Vec2 PerpendicularRight { get { return _data.PerpendicularRight; } }
        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        public Vec2 PerpendicularLeft { get { return _data.PerpendicularLeft; } }
        
        public void Normalize()
        {
            BeginUpdate();
            _data.Normalize();
            EndUpdate();
        }
        public void NormalizeFast()
        {
            BeginUpdate();
            _data.NormalizeFast();
            EndUpdate();
        }
        public float Dot(EventVec2 right)
        {
            return X * right.X + Y * right.Y;
        }
        
        [XmlIgnore]
        public EventVec2 Yx { get { return new EventVec2(Y, X); } set { Y = value.X; X = value.Y; } }

        public static EventVec2 operator -(EventVec2 left, EventVec2 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }
        public static EventVec2 operator -(EventVec2 vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            return vec;
        }
        public static EventVec2 operator *(EventVec2 vec, float scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }
        public static EventVec2 operator *(float scale, EventVec2 vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }
        public static EventVec2 operator *(EventVec2 vec1, EventVec2 EventVec2)
        {
            vec1.X *= EventVec2.X;
            vec1.Y *= EventVec2.Y;
            return vec1;
        }
        public static EventVec2 operator /(EventVec2 vec1, EventVec2 EventVec2)
        {
            vec1.X /= EventVec2.X;
            vec1.Y /= EventVec2.Y;
            return vec1;
        }
        public static EventVec2 operator /(EventVec2 vec, float scale)
        {
            vec.X /= scale;
            vec.Y /= scale;
            return vec;
        }
        public static EventVec2 operator /(float scale, EventVec2 vec)
        {
            vec.X = scale / vec.X;
            vec.Y = scale / vec.Y;
            return vec;
        }
        public static EventVec2 operator +(EventVec2 vec, float amount)
        {
            vec.X = vec.X + amount;
            vec.Y = vec.Y + amount;
            return vec;
        }
        public static EventVec2 operator -(EventVec2 vec, float amount)
        {
            vec.X = vec.X - amount;
            vec.Y = vec.Y - amount;
            return vec;
        }
        public static EventVec2 operator -(float amount, EventVec2 vec)
        {
            vec.X = amount - vec.X;
            vec.Y = amount - vec.Y;
            return vec;
        }
        public static bool operator ==(EventVec2 left, EventVec2 right) { return left.Equals(right); }
        public static bool operator !=(EventVec2 left, EventVec2 right) { return !left.Equals(right); }
        public static explicit operator EventVec2(Vec3 v) { return new EventVec2(v.X, v.Y); }
        public static explicit operator EventVec2(Vec4 v) { return new EventVec2(v.X, v.Y); }
        public static implicit operator EventVec2(PointF v) { return new EventVec2(v.X, v.Y); }
        public static implicit operator PointF(EventVec2 v) { return new PointF(v.X, v.Y); }
        
        private static string listSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString()
        {
            return String.Format("({0}{2} {1})", X, Y, listSeparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is EventVec2))
                return false;

            return Equals((EventVec2)obj);
        }
        public bool Equals(EventVec2 other)
        {
            return
                X == other.X &&
                Y == other.Y;
        }
        public bool Equals(EventVec2 other, float precision)
        {
            return
                Abs(X - other.X) < precision &&
                Abs(Y - other.Y) < precision;
        }
    }
}
