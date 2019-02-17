using TheraEngine;
using TheraEngine.Rendering.Models;
using System.Drawing;
using System.Xml.Serialization;
using static System.Math;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace System
{
    [Serializable]
    public unsafe class EventVec2 : IEquatable<EventVec2>, IUniformable2Float, IBufferable, ISerializableString
    {
        //private int _updating = 0;
        private float _oldX, _oldY;
        [TSerialize("XY", NodeType = ENodeType.ElementContent)]
        private Vec2 _data;

        public void SetRawNoUpdate(Vec2 raw)
            => _data = raw;

        [Browsable(false)]
        public Vec2 Raw
        {
            get => _data;
            set
            {
                BeginUpdate();
                _data = value;
                EndUpdate();
            }
        }
        public float X
        {
            get => _data.X;
            set
            {
                BeginUpdate();
                _data.X = value;
                EndUpdate();
            }
        }
        public float Y
        {
            get => _data.Y;
            set
            {
                BeginUpdate();
                _data.Y = value;
                EndUpdate();
            }
        }

        [Browsable(false)]
        public float* Data => _data.Data;
        [Browsable(false)]
        public VoidPtr Address => _data.Address;
        [Browsable(false)]
        public DataBuffer.EComponentType ComponentType => DataBuffer.EComponentType.Float;
        [Browsable(false)]
        public int ComponentCount => 2;
        [Browsable(false)]
        bool IBufferable.Normalize => false;

        public void Write(VoidPtr address)
            => _data.Write(address);
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
        public event DelFloatChange XValueChanged;
        public event DelFloatChange YValueChanged;
        public event Action Changed;

        private void BeginUpdate()
        {
            //++_updating;
            _oldX = X;
            _oldY = Y;
        }
        private void EndUpdate()
        {
            float x = X, y = Y;
            float ox = _oldX, oy = _oldY;

            //--_updating;
            //if (_updating > 0)
            //    return;
            bool anyChanged = false;
            if (*(int*)&x != *(int*)&ox)
            {
                XChanged?.Invoke();
                XValueChanged?.Invoke(_oldX, X);
                anyChanged = true;
            }
            if (*(int*)&y != *(int*)&oy)
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

        [Browsable(false)]
        public float Length => _data.Length;
        [Browsable(false)]
        public float LengthFast => _data.LengthFast;
        [Browsable(false)]
        public float LengthSquared => _data.LengthSquared;

        public float DistanceToSquared(EventVec2 otherPoint) 
            => _data.DistanceToSquared(otherPoint._data);
        public float DistanceToFast(EventVec2 otherPoint)
            => _data.DistanceToFast(otherPoint._data);
        public float DistanceTo(EventVec2 otherPoint) 
            => _data.DistanceTo(otherPoint._data);

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        public Vec2 PerpendicularRight => _data.PerpendicularRight;
        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        public Vec2 PerpendicularLeft => _data.PerpendicularLeft;
        
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
            => X * right.X + Y * right.Y;

        [Browsable(false)]
        [XmlIgnore]
        public EventVec2 Yx { get => new EventVec2(Y, X); set { Y = value.X; X = value.Y; } }

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
        public static implicit operator EventVec2(Vec2 v) { return new EventVec2(v.X, v.Y); }
        public static implicit operator Vec2(EventVec2 v) { return v.Raw; }

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

        public string WriteToString()
            => _data.WriteToString();
        public void ReadFromString(string str)
            => _data.ReadFromString(str);
    }
}
