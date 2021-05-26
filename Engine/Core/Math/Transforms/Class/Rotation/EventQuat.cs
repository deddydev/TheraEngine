using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Core.Maths.Transforms
{
    public unsafe class EventQuat : TObject, IEquatable<EventQuat>, IUniformable4Float, IBufferable, ISerializableString
    {
        public event Action Changed;
        private void OnChanged() => Changed?.Invoke();

        public static EventQuat Identity => new EventQuat(0.0f, 0.0f, 0.0f, 1.0f);

        [TSerialize("XYZW", NodeType = ENodeType.ElementContent)]
        public Quat _data;

        /// <summary>
        /// Sets the internal <see cref="Vec3"/> value and does not fire any events.
        /// </summary>
        public void SetValueSilent(Quat raw)
            => _data = raw;

        [Browsable(false)]
        public Quat Value
        {
            get => _data;
            set
            {
                if (Set(ref _data, value))
                    OnChanged();
            }
        }
        public EventQuat() => _data = Quat.Identity;
        public EventQuat(Vec4 v) => _data = new Quat(v);
        public EventQuat(Vec3 v, float w) => _data = new Quat(v, w);
        public EventQuat(float x, float y, float z, float w) => _data = new Quat(x, y, z, w);

        public static implicit operator EventQuat(Quat v) => new EventQuat(v);
        public static implicit operator EventQuat(Vec4 v) => new EventQuat(v);
        public static implicit operator Quat(EventQuat v) => v.Value;
        public static implicit operator Vec4(EventQuat v) => v.Value;

        public float* Data => _data.Data;

        public float LengthFast => _data.LengthFast;
        public float Length => _data.Length;
        public float LengthSquared => _data.LengthSquared;

        public float this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public void ToAxisAngleDeg(out Vec3 axis, out float angle)
            => _data.ToAxisAngleDeg(out axis, out angle);

        public void ToAxisAngleRad(out Vec3 axis, out float angle)
            => _data.ToAxisAngleRad(out axis, out angle);

        /// <summary>
        /// Returns a euler rotation in the order of yaw, pitch, roll.
        /// </summary>
        public Rotator ToRotator() 
            => _data.ToRotator();

        public void Normalize(ENormalizeOption normalizeMethod)
        {
            _data.Normalize(normalizeMethod);
            OnChanged();
        }

        public void Normalize(bool safe = true)
        {
            _data.Normalize(safe);
            OnChanged();
        }
           
        public void NormalizeFast(bool safe = true)
        {
            _data.NormalizeFast(safe);
            OnChanged();
        }

        public void Invert()
        {
            _data.Invert();
            OnChanged();
        }

        /// <summary>
        /// Makes this quaternion the opposite version of itself.
        /// There are two rotations about the same axis that equal the same rotation,
        /// but from different directions.
        /// </summary>
        public void Conjugate()
        {
            _data.Conjugate();
            OnChanged();
        }

        /// <summary>
        /// Optimized form for Vec3 (W == 0)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vec3 Transform(Vec3 v) 
            => _data.Transform(v);

        /// <summary>
        /// Slower, traditional form
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vec4 Transform(Vec4 v)
            => _data.Transform(v);

        public static Vec3 operator *(EventQuat quat, Vec3 vec)
            => quat.Transform(vec);
        public static Vec3 operator *(Vec3 vec, EventQuat quat)
            => quat.Transform(vec);
        public static Vec4 operator *(EventQuat quat, Vec4 vec)
            => quat.Transform(vec);
        public static Vec4 operator *(Vec4 vec, EventQuat quat)
            => quat.Transform(vec);

        public static Quat operator +(EventQuat left, EventQuat right)
            => left._data + right._data;
        public static Quat operator -(EventQuat left, EventQuat right)
            => left._data - right._data;

        public static Quat operator +(Quat left, EventQuat right)
            => left + right._data;
        public static Quat operator -(Quat left, EventQuat right)
            => left - right._data;

        public static Quat operator +(EventQuat left, Quat right)
            => left._data + right;
        public static Quat operator -(EventQuat left, Quat right)
            => left._data - right;

        public static Quat operator *(EventQuat left, EventQuat right)
            => left._data * right._data;
        public static Quat operator *(Quat left, EventQuat right)
            => left * right._data;
        public static Quat operator *(EventQuat left, Quat right)
            => left._data * right;

        public static Quat operator *(EventQuat quaternion, float scale)
            => quaternion._data * scale;

        public static Quat operator *(float scale, EventQuat quaternion)
            => scale * quaternion._data;

        public static bool operator ==(EventQuat left, EventQuat right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }
        public static bool operator !=(EventQuat left, EventQuat right)
        {
            if (left is null)
                return !(right is null);

            return !left.Equals(right);
        }

        public override string ToString()
            => _data.ToString();
        public string ToString(bool includeParentheses, bool includeSeparator)
            => _data.ToString(includeParentheses, includeSeparator);
        
        public override bool Equals(object other)
        {
            if (other is EventQuat == false)
                return false;
            return this == (EventQuat)other;
        }

        public override int GetHashCode() 
            => _data.GetHashCode();
        public bool Equals(EventQuat other)
            => _data == other._data;

        public string WriteToString()
            => _data.WriteToString();
        public void ReadFromString(string str)
            => _data.ReadFromString(str);

        public VoidPtr Address => _data.Address;
        public DataBuffer.EComponentType ComponentType => DataBuffer.EComponentType.Float;
        public int ComponentCount => 4;
        bool IBufferable.Normalize => false;
        public void Write(VoidPtr address) 
            => _data.Write(address);
        public void Read(VoidPtr address)
        {
            _data.Read(address);
            OnChanged();
        }
    }
}
