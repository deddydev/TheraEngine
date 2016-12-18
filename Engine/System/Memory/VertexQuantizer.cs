using System;
using System.Runtime.InteropServices;

namespace System
{
    public unsafe class VertexQuantizer : IDisposable
    {
        private const float _maxError = 0.0005f;

        private Vec3[] _vertices;
        private Vec3 _min, _max;
        private bool _hasZ;
        private int _bits = 1;
        private bool _signed;
        private float _quantScale;

        private int _srcComponents, _srcCount;
        private int _dstBitStride;
        private int _scale;
        private int _dataLen;

        private GCHandle _handle;
        private float* _pData;

        private VertexQuantizer() { }
        ~VertexQuantizer() { Dispose(); }
        public void Dispose()
        {
            if (_handle.IsAllocated)
                _handle.Free();
            _pData = null;
        }
        
        public VertexQuantizer(Vec3[] vertices, bool allowCompression)
        {
            _srcCount = vertices.Length;
            _srcComponents = 3;
            _handle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            _pData = (float*)_handle.AddrOfPinnedObject();
            Evaluate(allowCompression);
        }
        public VertexQuantizer(Vec2[] vertices, bool allowCompression)
        {
            _srcCount = vertices.Length;
            _srcComponents = 2;
            _handle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            _pData = (float*)_handle.AddrOfPinnedObject();
            Evaluate(allowCompression);
        }
        public VertexQuantizer(Vec3* sPtr, int count, bool allowCompression)
        {
            _srcCount = count;
            _srcComponents = 3;
            _pData = (float*)sPtr;
            Evaluate(allowCompression);
        }
        public VertexQuantizer(Vec2* sPtr, int count, bool allowCompression)
        {
            _srcCount = count;
            _srcComponents = 2;
            _pData = (float*)sPtr;
            Evaluate(allowCompression);
        }

        private int GetMaxValue(int bitCount, bool signed)
        {
            if (signed)
            {
                int mask = 0;
                for (int i = 0; i < bitCount - 1; ++i)
                    mask |= (1 << i);
                return mask;
            }
            else
            {
                int mask = 0;
                for (int i = 0; i < bitCount; ++i)
                    mask |= (1 << i);
                return mask;
            }
        }
        private int GetMinValue(int bitCount, bool signed)
        {
            if (signed)
            {
                int mask = 0;
                for (int i = 0; i < bitCount - 1; ++i)
                    mask |= (1 << i);
                return -mask;
            }
            else
                return 0;
        }
        private void Evaluate(bool allowCompression)
        {
            float* fPtr;
            int bestScale = 0;

            _min = Vec3.Max;
            _max = Vec3.Min;

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

            //Find max distance from 0 using overall max and min
            vDist = Math.Max(Math.Abs(vMin), Math.Abs(vMax));

            //Determine if Z can be ignored (for flat primitives)
            _hasZ = _srcComponents == 3 && !(Math.Abs(_min.Z) > _maxError || Math.Abs(_max.Z) > _maxError);

            if (allowCompression)
            {
                _signed = vMin < 0;

                int divisor = 0;
                float rMin = 0.0f, rMax;
                for (_bits = 1; _bits < 32; ++_bits)
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
            }

            _bits = 32;
            bestScale = 0;

            Next:

            _scale = bestScale;

            //Get bit count, align count to nearest multiple of 8,
            //divide by 8 to get byte count, align result for padding
            _dataLen = ((_srcCount * _bits).Align(8) / 8).Align(4);
            _quantScale = VQuant.QuantTable[_scale];
        }
        
        public static Vec3[] ExtractVertices(
            VoidPtr address,
            int count,
            bool hasZ,
            bool signed,
            int bitCount, 
            float divisor)
        {
            byte* sPtr = (byte*)address;
            int bitOffset = 0;
            Vec3[] verts = new Vec3[count];
            fixed (Vec3* p = verts)
            {
                float* dPtr = (float*)p;
                for (int i = 0; i < count; i++)
                {
                    *dPtr++ = ReadValue(ref sPtr, ref bitOffset, bitCount, signed, divisor);
                    *dPtr++ = ReadValue(ref sPtr, ref bitOffset, bitCount, signed, divisor);
                    *dPtr++ = hasZ ? ReadValue(ref sPtr, ref bitOffset, bitCount, signed, divisor) : 0.0f;
                }
            }
            return verts;
        }
        private static float ReadValue(ref byte* sPtr, ref int bitOffset, int bitCount, bool signed, float divisor)
        {
            int value = 0, currentShift = bitCount, mask = 0;
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
            if (signed && (value & (1 << (bitCount - 1))) != 0)
                value = -((value - 1) ^ mask);
            return value / divisor;
        }
        public void EncodeVertices(VoidPtr address)
        {
            byte* dPtr = (byte*)address;

            int bitOffset = 0;
            foreach (Vec3 v in _vertices)
            {
                WriteValue(v.X, ref dPtr, ref bitOffset);
                WriteValue(v.Y, ref dPtr, ref bitOffset);
                if (_hasZ)
                    WriteValue(v.Z, ref dPtr, ref bitOffset);
            }
        }
        private void WriteValue(float value, ref byte* dPtr, ref int bitOffset)
        {
            float scaledValue = value * _quantScale;
            int result = (int)Math.Round(scaledValue);

            
        }
    }
}
