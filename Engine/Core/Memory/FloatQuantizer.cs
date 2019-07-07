using Extensions;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Core.Memory
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FloatQuantizeHeader
    {
        public const int Size = 0x10;

        private Bin32 _flags;
        private bfloat _divisor;
        private bint _elementCount;
        private bint _dataLength;
        
        public int ElementCount
        {
            get => _elementCount;
            set => _elementCount = value;
        }
        public int DataLength
        {
            get => _dataLength;
            set => _dataLength = value;
        }
        public float Divisor
        {
            get => _divisor;
            set => _divisor = value;
        }
        public int BitCount
        {
            get => (int)_flags[7, 5];
            set
            {
                if (value < 1 || value > 32)
                    throw new InvalidOperationException("Bit count must be between 1 and 32.");
                _flags[7, 5] = (uint)value;
            }
        }
        public bool HasW { get { return _flags[6]; } set { _flags[6] = value; } }
        public bool HasZ { get { return _flags[5]; } set { _flags[5] = value; } }
        public bool HasY { get { return _flags[4]; } set { _flags[4] = value; } }
        public bool HasX { get { return _flags[3]; } set { _flags[3] = value; } }
        public bool Signed { get { return _flags[2]; } set { _flags[2] = value; } }
        public int ComponentCount
        {
            get { return (int)(_flags[0, 2] + 1); }
            set
            {
                if (value < 1 || value > 4)
                    throw new InvalidOperationException("Component count must be 1, 2, 3 or 4.");
                _flags[0, 2] = (byte)(value - 1);
            }
        }
    }
    public unsafe class FloatQuantizer
    {
        private const float _maxError = 0.0005f;
        
        private Vec4 _min, _max;
        private BoolVec4 _includedComponents;
        private int _bitCount = 1;
        private bool _signed;
        private float _quantScale;

        private int _srcComponents, _srcCount;
        private int _scale;
        private int _dataLen;
        
        private readonly float[] _pData;

        private FloatQuantizer() { }
        public FloatQuantizer(params Vec4[] values)
        {
            _srcCount = values.Length;
            _srcComponents = 4;
            _pData = values.SelectMany(x => new float[] { x.X, x.Y, x.Z, x.W }).ToArray();
            Evaluate();
        }
        public FloatQuantizer(params Vec3[] values)
        {
            _srcCount = values.Length;
            _srcComponents = 3;
            _pData = values.SelectMany(x => new float[] { x.X, x.Y, x.Z }).ToArray();
            Evaluate();
        }
        public FloatQuantizer(params Vec2[] values)
        {
            _srcCount = values.Length;
            _srcComponents = 2;
            _pData = values.SelectMany(x => new float[] { x.X, x.Y }).ToArray();
            Evaluate();
        }
        public FloatQuantizer(params float[] values)
        {
            _srcCount = values.Length;
            _srcComponents = 1;
            _pData = values;
            Evaluate();
        }

        public FloatQuantizeHeader GetHeader()
            => new FloatQuantizeHeader()
            {
                Signed = _signed,
                BitCount = _bitCount,
                DataLength = _dataLen,
                Divisor = _quantScale,
                ElementCount = _srcCount,
                HasX = _includedComponents.X,
                HasY = _includedComponents.Y,
                HasZ = _includedComponents.Z,
                HasW = _includedComponents.W,
                ComponentCount = _srcComponents
            };
        
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
            int bestScale = 0;
            bool negateScale;

            _min = Vec4.Max;
            _max = Vec4.Min;

            //Smallest overall value, largest overall value, farthest value (absolute)
            float vMin = float.MaxValue, vMax = float.MinValue, vDist, val;
            
            //Find max and min for each component and overall
            for (int i = 0, offset = 0; i < _srcCount; i++, offset += _srcComponents)
            {
                for (int x = 0; x < _srcComponents; x++)
                {
                    val = _pData[offset + x];
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
                if (vMax < 0)
                    negateScale = true;
                else
                {
                    negateScale = false;
                    _signed = true;
                }
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
                _bitCount = 32;
                bestScale = 0;
                goto Next;
            }

            int divisor = 0;
            float rMin = 0.0f, rMax;
            for (_bitCount = _signed ? 2 : 1; _bitCount <= 32; ++_bitCount)
            {
                float bestError = _maxError;
                float scale, maxVal;

                rMax = GetMaxValue(_bitCount, _signed);
                rMin = GetMinValue(_bitCount, _signed);

                maxVal = rMax / vDist;
                while ((divisor < 32) && ((scale = (negateScale ? -1.0f : 1.0f) * VQuant.QuantTable[divisor]) <= maxVal))
                {
                    float worstError = float.MinValue;
                    for (int y = 0, offset = 0; y < _srcCount; y++, offset += _srcComponents)
                    {
                        for (int z = 0; z < _srcComponents; z++)
                        {
                            if ((val = _pData[offset + z]) == 0)
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

                ++_bitCount;
            }

            _bitCount = 32;
            bestScale = 0;

            Next:

            _scale = bestScale;
            
            //Get bit count, 
            //align count to nearest multiple of 32 to align to 4 bytes,
            //divide by 8 to get byte count
            _dataLen = ((_srcCount * componentCount * _bitCount).Align(32) / 8);
            _quantScale = (negateScale ? -1.0f : 1.0f) * VQuant.QuantTable[_scale];
        }
        public static Vec4[] Decode4(FloatQuantizeHeader header, VoidPtr address)
        {
            Vec4[] values = new Vec4[header.ElementCount];
            fixed (Vec4* p = values)
                DecodeToBuffer(
                    (byte*)address, (float*)p, header.ElementCount,
                    header.HasX, header.HasY, header.HasZ, header.HasW, 4,
                    header.Signed, header.BitCount, header.Divisor);
            return values;
        }
        public static Vec3[] Decode3(FloatQuantizeHeader header, VoidPtr address)
        {
            Vec3[] values = new Vec3[header.ElementCount];
            fixed (Vec3* p = values)
                DecodeToBuffer(
                    (byte*)address, (float*)p, header.ElementCount,
                    header.HasX, header.HasY, header.HasZ, false, 3,
                    header.Signed, header.BitCount, header.Divisor);
            return values;
        }
        public static Vec2[] Decode2(FloatQuantizeHeader header, VoidPtr address)
        {
            Vec2[] values = new Vec2[header.ElementCount];
            fixed (Vec2* p = values)
                DecodeToBuffer(
                    (byte*)address, (float*)p, header.ElementCount,
                    header.HasX, header.HasY, false, false, 2,
                    header.Signed, header.BitCount, header.Divisor);
            return values;
        }
        public static float[] Decode1(FloatQuantizeHeader header, VoidPtr address)
        {
            float[] values = new float[header.ElementCount];
            fixed (float* p = values)
                DecodeToBuffer(
                    (byte*)address, p, header.ElementCount,
                    true, false, false, false, 1,
                    header.Signed, header.BitCount, header.Divisor);
            return values;
        }
        private static void DecodeToBuffer(
            byte* sPtr,
            float* dPtr,
            int count,
            bool hasX,
            bool hasY,
            bool hasZ,
            bool hasW,
            int componentCount,
            bool signed,
            int bitCount,
            float divisor)
        {
            BoolVec4 included = new BoolVec4() { X = hasX, Y = hasY, Z = hasZ, W = hasW };
            int bitOffset = 0, bitMask = 0, signBit = 0;
            if (signed)
            {
                signBit = 1 << (bitCount - 1);
                for (int i = 0; i < bitCount; ++i)
                    bitMask |= (1 << i);
            }
            for (int i = 0; i < count; i++)
                for (int j = 0; j < componentCount; ++j)
                    *dPtr++ = included.Data[j] ? ReadValue(ref sPtr, ref bitOffset, bitCount, signed, divisor, bitMask, signBit) : 0.0f;
        }
        private static float ReadValue(ref byte* sPtr, ref int bitOffset, int bitCount, bool signed, float divisor, int bitMask, int signBit)
        {
            if (bitCount == 32)
            {
                float v = *(bfloat*)sPtr;
                sPtr += 4;
                return v;
            }

            int value = 0, currentShift = bitCount;
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
            }
            if (signed && (value & signBit) != 0)
                value = ((-value - 1) ^ bitMask);
            return value / divisor;
        }
        public void EncodeValues(VoidPtr address)
        {
            byte* dPtr = (byte*)address;

            int bitOffset = 0;
            for (int i = 0; i < _srcCount; ++i)
                for (int j = 0; j < _srcComponents; ++j)
                {
                    int offset = i * _srcComponents;
                    if (_includedComponents[j])
                        WriteValue(_pData[offset + j], ref dPtr, ref bitOffset);
                }
        }
        private void WriteValue(float value, ref byte* dPtr, ref int bitOffset)
        {
            if (_bitCount == 32)
            {
                *(bfloat*)dPtr = value;
                dPtr += 4;
                return;
            }

            float scaledValue = value * _quantScale;
            int result = (int)Math.Round(scaledValue);
            
            int valueShift = _bitCount;
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
