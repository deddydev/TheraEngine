using System.Runtime.InteropServices;
using static System.Math;
using static TheraEngine.Core.Maths.TMath;
using TheraEngine;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;
using TheraEngine.Core.Maths;
using TheraEngine.ComponentModel;
using System.ComponentModel;

namespace System
{
    public unsafe class EventMatrix4 : TObject
    {
        public EventMatrix4() { }
        public EventMatrix4(Matrix4 matrix) => _value = matrix;
        public EventMatrix4(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33)
            => _value = new Matrix4(
                m00, m01, m02, m03,
                m10, m11, m12, m13,
                m20, m21, m22, m23,
                m30, m31, m32, m33);

        public Vec3 RightVec { get => Value.RightVec; set => Value = new Matrix4(new Vec4(value.X, value.Y, value.Z, Value.Row0.W), Value.Row1, Value.Row2, Value.Row3); }
        public Vec3 UpVec { get => Value.UpVec; set => Value = new Matrix4(Value.Row0, new Vec4(value.X, value.Y, value.Z, Value.Row1.W), Value.Row2, Value.Row3); }
        public Vec3 ForwardVec { get => Value.ForwardVec; set => Value = new Matrix4(Value.Row0, Value.Row1, new Vec4(value.X, value.Y, value.Z, Value.Row2.W), Value.Row3); }
        public Vec3 Translation { get => Value.Translation; set => Value = new Matrix4(Value.Row0, Value.Row1, Value.Row2, new Vec4(value.X, value.Y, value.Z, Value.Row3.W)); }

        public event Action Changed;

        private Matrix4 _oldMatrix;
        [TSerialize("MTX", NodeType = ENodeType.ElementContent)]
        private Matrix4 _value;

        [Browsable(false)]
        public Matrix4 Value
        {
            get => _value;
            set
            {
                BeginUpdate();
                try
                {
                    Set(ref _value, value);
                }
                finally
                {
                    EndUpdate();
                }
            }
        }

        private void BeginUpdate()
        {
            _oldMatrix = _value;
        }
        private void EndUpdate()
        {
            Matrix4 current = _value;
            Matrix4 old = _oldMatrix;
            for (int i = 0; i < 16; ++i)
            {
                if (current.Data[i] != old.Data[i])
                {
                    Changed?.Invoke();
                    break;
                }
            }
        }
        public static Matrix4 operator *(Matrix4 left, EventMatrix4 right) => left * right.Value;
        public static Matrix4 operator *(EventMatrix4 left, Matrix4 right) => left.Value * right;
        public static Matrix4 operator *(EventMatrix4 left, EventMatrix4 right) => left.Value * right.Value;
        public static implicit operator Matrix4(EventMatrix4 matrix) => matrix.Value;
        public static explicit operator EventMatrix4(Matrix4 matrix) => new EventMatrix4(matrix);
    }
}
