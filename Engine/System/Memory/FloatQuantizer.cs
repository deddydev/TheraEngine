using CustomEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace System
{
    public struct FloatQuantizeHeader
    {

    }
    public unsafe class FloatQuantizer : IDisposable
    {
        private const float _maxError = 0.0005f;

        private Vec4[] _values;
        private Vec4 _min, _max;
        internal BoolVec4 _includedComponents;
        internal int _bits = 1;
        internal bool _signed;
        internal float _quantScale;

        public float Divisor { get { return _quantScale; } }
        public bool Signed { get { return _signed; } }
        public int Bits { get { return _bits; } }
        public BoolVec4 IncludedComponents { get { return _includedComponents; } }

        public int DataLength { get { return _dataLen; } }

        private int _srcComponents, _srcCount;
        private int _scale;
        private int _dataLen;

        private GCHandle _handle;
        private float* _pData;

        private FloatQuantizer() { }
        ~FloatQuantizer() { Dispose(); }
        public void Dispose()
        {
            if (_handle.IsAllocated)
                _handle.Free();
            _pData = null;
        }
        public FloatQuantizer(params Vec4[] values)
        {
            _values = values;
            _srcCount = values.Length;
            _srcComponents = 4;
            _handle = GCHandle.Alloc(values, GCHandleType.Pinned);
            _pData = (float*)_handle.AddrOfPinnedObject();
            Evaluate();
        }
        public FloatQuantizer(params Vec3[] values)
        {
            _values = values.Select(x => new Vec4(x.X, x.Y, x.Z, 0.0f)).ToArray();
            _srcCount = values.Length;
            _srcComponents = 3;
            _handle = GCHandle.Alloc(values, GCHandleType.Pinned);
            _pData = (float*)_handle.AddrOfPinnedObject();
            Evaluate();
        }
        public FloatQuantizer(params Vec2[] values)
        {
            _values = values.Select(x => new Vec4(x.X, x.Y, 0.0f, 0.0f)).ToArray();
            _srcCount = values.Length;
            _srcComponents = 2;
            _handle = GCHandle.Alloc(values, GCHandleType.Pinned);
            _pData = (float*)_handle.AddrOfPinnedObject();
            Evaluate();
        }
        public FloatQuantizer(params float[] values)
        {
            _values = values.Select(x => new Vec4(x, 0.0f, 0.0f, 0.0f)).ToArray();
            _srcCount = values.Length;
            _srcComponents = 1;
            _handle = GCHandle.Alloc(values, GCHandleType.Pinned);
            _pData = (float*)_handle.AddrOfPinnedObject();
            Evaluate();
        }

        private int GetMaxValue(int bitCount, bool signed)
        {
            int value = 0;
            if (signed)
            {
                for (int i = 0; i < bitCount - 1; ++i)
                    value |= (1 << i);
                return value;
            }
            else
            {
                for (int i = 0; i < bitCount; ++i)
                    value |= (1 << i);
                return value;
            }
        }
        private int GetMinValue(int bitCount, bool signed)
        {
            if (signed)
                return -GetMaxValue(bitCount, signed) - 1;
            else
                return 0;
        }
        private void Evaluate()
        {
            float* fPtr;
            int bestScale = 0;
            bool negateScale;

            _min = Vec4.Max;
            _max = Vec4.Min;

            //Smallest overall value, largest overall value, farthest value (absolute)
            float vMin = float.MaxValue, vMax = float.MinValue, vDist, val;
            
            //Find max and min for each component and overall
            for (int i = 0, offset = 0; i < _srcCount; i++, offset += _srcComponents)
            {
                fPtr = &_pData[offset];
                for (int x = 0; x < _srcComponents; x++)
                {
                    val = *fPtr++;
                    if (val < _min[x])
                        _min[x] = val;
                    if (val > _max[x])
                        _max[x] = val;
                    if (val < vMin)
                        vMin = val;
                    if (val > vMax)
                        vMax = val;
                }
            }

            //Check if we need to account for negative and positive values
            if (vMin < 0)
            {
                if (vMax > 0)
                    _signed = true;
                else
                    negateScale = true;
            }
            else
                _signed = negateScale = false;

            //Find max distance from 0 using overall max and min
            vDist = Math.Max(Math.Abs(vMin), Math.Abs(vMax));

            int componentCount = 0;
            for (int i = 0; i < 4; ++i)
                if (_srcComponents > i && (_includedComponents.Data[i] = Math.Abs(_min[i]) > _maxError || Math.Abs(_max[i]) > _maxError))
                    ++componentCount;
            if (componentCount == 0)
            {
                _bits = 32;
                bestScale = 0;
                goto Next;
            }

            int divisor = 0;
            float rMin = 0.0f, rMax;
            for (_bits = _signed ? 2 : 1; _bits <= 32; ++_bits)
            {
                float bestError = _maxError;
                float scale, maxVal;

                rMax = GetMaxValue(_bits, _signed);
                rMin = GetMinValue(_bits, _signed);

                maxVal = rMax / vDist;
                while ((divisor < 32) && ((scale = VQuant.QuantTable[divisor]) <= maxVal))
                {
                    float worstError = float.MinValue;
                    for (int y = 0, offset = 0; y < _srcCount; y++, offset += _srcComponents)
                    {
                        fPtr = &_pData[offset];
                        for (int z = 0; z < _srcComponents; z++)
                        {
                            if ((val = *fPtr++) == 0)
                                continue;

                            val *= scale;
                            if (val > rMax)
                                val = rMax;
                            else if (val < rMin)
                                val = rMin;

                            int step = (int)Math.Round(val * scale);
                            float error = Math.Abs((step / scale) - val);

                            if (error > worstError)
                                worstError = error;

                            if (error > bestError)
                                goto Check;
                        }
                    }

                    Check:

                    if (worstError < bestError)
                    {
                        bestScale = divisor;
                        bestError = worstError;
                        if (bestError == 0)
                            goto Next;
                    }

                    ++divisor;
                }

                if (bestError < _maxError)
                    goto Next;

                ++_bits;
            }

            _bits = 32;
            bestScale = 0;

            Next:

            _scale = bestScale;

            //Get bit count, align count to nearest multiple of 8,
            //divide by 8 to get byte count, align result for padding
            _dataLen = ((_srcCount * componentCount * _bits).Align(32) / 8);
            _quantScale = VQuant.QuantTable[_scale];
        }
        
        public static Vec3[] DecodeValues(
            VoidPtr address,
            int count,
            bool hasX,
            bool hasY,
            bool hasZ,
            bool signed,
            int bitCount, 
            float divisor)
        {
            Vec3[] verts = new Vec3[count];
            fixed (Vec3* p = verts)
                DecodeToBuffer(
                    (byte*)address, (float*)p, count,
                    hasX, hasY, hasZ, false, 3,
                    signed, bitCount, divisor);
            return verts;
        }
        private static void DecodeToBuffer(
            byte* sPtr,
            float* dPtr,
            int count,
            bool hasX,
            bool hasY,
            bool hasZ,
            bool hasW,
            int maxComponentCount,
            bool signed,
            int bitCount,
            float divisor)
        {
            BoolVec4 included = new BoolVec4() { X = hasX, Y = hasY, Z = hasZ, W = hasW };
            int bitOffset = 0;
            for (int i = 0; i < count; i++)
                for (int j = 0; j < maxComponentCount; ++j)
                    *dPtr++ = included.Data[j] ? ReadValue(ref sPtr, ref bitOffset, bitCount, signed, divisor) : 0.0f;
        }
        private static float ReadValue(ref byte* sPtr, ref int bitOffset, int bitCount, bool signed, float divisor)
        {
            int value = 0, currentShift = bitCount, mask = 0;
            int signBit = 1 << (bitCount - 1);
            while (currentShift-- > 0)
            {
                int shift = 7 - bitOffset++;
                int bit = (*sPtr >> shift) & 1;
                value |= bit << currentShift;
                if (bitOffset == 8)
                {
                    bitOffset = 0;
                    ++sPtr;
                }
                if (signed)
                    mask |= (1 << currentShift);
            }
            if (signed && (value & signBit) != 0)
                value = ((-value - 1) ^ mask);
            return value / divisor;
        }
        public void EncodeValues(VoidPtr address)
        {
            byte* dPtr = (byte*)address;

            int bitOffset = 0, bitMask = 0;
            for (int i = 0; i < _bits; ++i)
                bitMask |= (1 << i);
            foreach (Vec4 v in _values)
            {
                if (_includedComponents.X)
                    WriteValue(v.X, ref dPtr, ref bitOffset);
                if (_includedComponents.Y)
                    WriteValue(v.Y, ref dPtr, ref bitOffset);
                if (_includedComponents.Z)
                    WriteValue(v.Z, ref dPtr, ref bitOffset);
                if (_includedComponents.W)
                    WriteValue(v.W, ref dPtr, ref bitOffset);
            }
            Console.WriteLine();
        }
        private void WriteValue(float value, ref byte* dPtr, ref int bitOffset/*, int bitMask*/)
        {
            float scaledValue = value * _quantScale;
            int result = (int)Math.Round(scaledValue);

            //if (result < 0)
            //    result = (-result - 1) ^ bitMask;
            
            int valueShift = _bits;
            while (valueShift-- > 0)
            {
                //Clear byte of unwanted data
                if (bitOffset == 0)
                    *dPtr = 0;

                int shift = 7 - bitOffset++;
                int bit = (result >> valueShift) & 1;
                if (bit != 0)
                    *dPtr |= (byte)(bit << shift);
                if (bitOffset >= 8)
                {
                    bitOffset = 0;
                    ++dPtr;
                }
            }
        }
    }
}
