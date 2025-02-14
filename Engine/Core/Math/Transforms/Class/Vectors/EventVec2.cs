﻿using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using TheraEngine;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;
using static System.Math;

namespace System
{
    public delegate Vec2 DelGetVec2Value(float delta);

    /// <summary>
    /// A wrapper class for <see cref="Vec2"/> that supports events and synchronization when its values change.
    /// </summary>
    public unsafe class EventVec2 : TObject, IEquatable<EventVec2>, IUniformable2Float, IBufferable, ISerializableString
    {
        public EventVec2() { }

        public EventVec2(Vec2 xy) => _value = xy;
        public EventVec2(float x, float y) => _value = new Vec2(x, y);
        public EventVec2(float xy) : this(xy, xy) { }

        public EventVec2(Vec3 xyz, bool divideByZ) => _value = new Vec2(xyz, divideByZ);

        public static EventVec2 UnitX => new EventVec2(1.0f, 0.0f);
        public static EventVec2 UnitY => new EventVec2(0.0f, 1.0f);
        public static EventVec2 Zero => new EventVec2(0.0f);
        public static EventVec2 Half => new EventVec2(0.5f);
        public static EventVec2 One => new EventVec2(1.0f);
        public static EventVec2 Min => new EventVec2(float.MinValue);
        public static EventVec2 Max => new EventVec2(float.MaxValue);

        public event Action XChanged;
        public event Action YChanged;
        public event DelFloatChange XValueChanged;
        public event DelFloatChange YValueChanged;
        public event Action Changed;

        private float _oldX, _oldY;
        [TSerialize("XY", NodeType = ENodeType.ElementContent)]
        private Vec2 _value;

        public EventVec2 _syncX, _syncY, _syncAll;

        public void Reset()
        {
            _value = Vec2.Zero;

            _oldX = 0.0f;
            _oldY = 0.0f;

            _syncX = null;
            _syncY = null;
            _syncAll = null;

            XChanged = null;
            YChanged = null;

            XValueChanged = null;
            YValueChanged = null;

            Changed = null;
        }

        public void SetValueSilent(Vec2 value)
            => _value = value;

        [Browsable(false)]
        public Vec2 Value
        {
            get => _value;
            set
            {
                BeginUpdate();
                try
                {
                    //OnPropertyChanging(nameof(X));
                    //OnPropertyChanging(nameof(Y));
                    Set(ref _value, value);
                    OnPropertiesChanged(nameof(X), nameof(Y));
                }
                finally
                {
                    EndUpdate();
                }
            }
        }
        public float X
        {
            get => _value.X;
            set
            {
                if (value == _value.X)
                    return;

                //if (OnPropertyChanging())
                //    return;

                BeginUpdate();
                try
                {
                    _value.X = value;
                }
                finally
                {
                    EndUpdate();
                }

                OnPropertyChanged();
            }
        }
        public float Y
        {
            get => _value.Y;
            set
            {
                if (value == _value.Y)
                    return;

                //if (OnPropertyChanging())
                //    return;

                try
                {
                    _value.Y = value;
                }
                finally
                {
                    EndUpdate();
                }

                OnPropertyChanged();
            }
        }

        [Browsable(false)]
        public float* Data => _value.Data;
        [Browsable(false)]
        public VoidPtr Address => _value.Address;
        [Browsable(false)]
        public DataBuffer.EComponentType ComponentType => DataBuffer.EComponentType.Float;
        [Browsable(false)]
        public int ComponentCount => 2;
        [Browsable(false)]
        bool IBufferable.Normalize => false;

        public void Write(VoidPtr address)
            => _value.Write(address);
        public void Read(VoidPtr address)
        {
            BeginUpdate();
            try
            {
                _value.Read(address);
            }
            finally
            {
                EndUpdate();
            }
        }
        public void SyncXFrom(EventVec2 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            if (_syncX != null)
            {
                _syncX.XValueChanged -= Other_XChanged;
                _syncX = null;
            }
            if (other != null)
            {
                other.XValueChanged += Other_XChanged;
                _syncX = other;
                X = other.X;
            }
        }
        public void SyncYFrom(EventVec2 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            if (_syncY != null)
            {
                _syncY.YValueChanged -= Other_YChanged;
                _syncY = null;
            }
            if (other != null)
            {
                other.YValueChanged += Other_YChanged;
                _syncY = other;
                Y = other.Y;
            }
        }
        public void SyncFrom(EventVec2 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            if (_syncX != null)
            {
                _syncX.XValueChanged -= Other_XChanged;
                _syncX = null;
            }
            if (_syncY != null)
            {
                _syncY.YValueChanged -= Other_YChanged;
                _syncY = null;
            }
            if (other != null)
            {
                other.Changed += Other_Changed;
                _syncAll = other;
                Value = other.Value;
            }
        }
        public void StopSynchronization()
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            if (_syncX != null)
            {
                _syncX.XValueChanged -= Other_XChanged;
                _syncX = null;
            }
            if (_syncY != null)
            {
                _syncY.YValueChanged -= Other_YChanged;
                _syncY = null;
            }
        }

        private void Other_Changed() => Value = _syncAll.Value;
        private void Other_XChanged(float newValue, float oldValue) => X = newValue;
        private void Other_YChanged(float newValue, float oldValue) => Y = newValue;

        private void BeginUpdate()
        {
            //Interlocked.Increment(ref _recursiveUpdates);
            _oldX = X;
            _oldY = Y;
        }
        private void EndUpdate()
        {
            //if (Interlocked.Decrement(ref _recursiveUpdates) > 0)
            //    return;

            float x = X, y = Y;
            float ox = _oldX, oy = _oldY;

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
        public float Length => _value.Length;
        [Browsable(false)]
        public float LengthFast => _value.LengthFast;
        [Browsable(false)]
        public float LengthSquared => _value.LengthSquared;

        public float DistanceToSquared(EventVec2 otherPoint) 
            => _value.DistanceToSquared(otherPoint._value);
        public float DistanceToFast(EventVec2 otherPoint)
            => _value.DistanceToFast(otherPoint._value);
        public float DistanceTo(EventVec2 otherPoint) 
            => _value.DistanceTo(otherPoint._value);

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        public Vec2 PerpendicularRight => _value.PerpendicularRight;
        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        public Vec2 PerpendicularLeft => _value.PerpendicularLeft;

        public void Normalize()
        {
            BeginUpdate();
            try
            {
                _value.Normalize();
            }
            finally
            {
                EndUpdate();
            }
        }
        public void NormalizeFast()
        {
            BeginUpdate();
            try
            {
                _value.NormalizeFast();
            }
            finally
            {
                EndUpdate();
            }
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
            vec.X += amount;
            vec.Y += amount;
            return vec;
        }
        public static EventVec2 operator -(EventVec2 vec, float amount)
        {
            vec.X -= amount;
            vec.Y -= amount;
            return vec;
        }
        public static EventVec2 operator -(float amount, EventVec2 vec)
        {
            vec.X = amount - vec.X;
            vec.Y = amount - vec.Y;
            return vec;
        }
        public static bool operator ==(EventVec2 left, EventVec2 right) => left?.Equals(right) ?? right is null;
        public static bool operator !=(EventVec2 left, EventVec2 right) => !(left?.Equals(right) ?? right is null);
        public static explicit operator EventVec2(Vec3 v) => new EventVec2(v.X, v.Y);
        public static explicit operator EventVec2(Vec4 v) => new EventVec2(v.X, v.Y);
        public static implicit operator EventVec2(PointF v) => new EventVec2(v.X, v.Y);
        public static implicit operator PointF(EventVec2 v) => new PointF(v.X, v.Y);
        public static implicit operator EventVec2(Vec2 v) => new EventVec2(v.X, v.Y);
        public static implicit operator Vec2(EventVec2 v) => v.Value;

        private static readonly string ListSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString() => string.Format("({0}{2} {1})", X, Y, ListSeparator);
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
            if (other is null)
                return false;

            return
                X == other.X &&
                Y == other.Y;
        }
        public bool Equals(EventVec2 other, float precision)
            => Abs(X - other.X) < precision &&
               Abs(Y - other.Y) < precision;

        public string WriteToString()
            => _value.WriteToString();
        public void ReadFromString(string str)
            => _value.ReadFromString(str);
    }
}
