using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TheraEngine;
using TheraEngine.Core.Memory;

namespace System.IO
{
    public unsafe class XMLReader
    {
        internal byte* _base, _ptr, _ceil;
        /// <summary>
        /// True if within tge brackets of an element.
        /// </summary>
        internal bool _inTag;

        private string _name, _value;

        public string Name => _name;
        public string Value => _value;

        public XMLReader() { }
        public XMLReader(void* pSource, int length, bool needsOpenDocument) : this()
        {
            SetMemoryAddress(pSource, length, needsOpenDocument);
        }

        internal void SetStringBuffer(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public void SetMemoryAddress(DataSource source, bool needsOpenDocument) => SetMemoryAddress(source.Address, source.Length, needsOpenDocument);
        public void SetMemoryAddress(VoidPtr pSource, int length, bool needsOpenDocument)
        {
            _base = _ptr = (byte*)pSource;
            _ceil = _ptr + length;

            //Find start of XML
            if (needsOpenDocument)
            {
                if (BeginElement() && Name.Equals("?xml"))
                {
                    while ((_ptr < _ceil) && (*_ptr++ != '>')) ;
                    _inTag = false;
                }
                else
                    throw new IOException("File is not a valid XML document.");
            }
            else if (!BeginElement())
                throw new IOException("Data is not a valid XML snippet.");
        }
        
        private bool ReadString()
        {
            bool inStr = false;
            List<byte> bytes = new List<byte>();
            while (_ptr < _ceil)
            {
                if (*_ptr <= 0x20)
                {
                    if (inStr)
                        break;

                    inStr = true;
                    continue;
                }
                else if ((*_ptr == '<') || (*_ptr == '>') || (*_ptr == '/'))
                {
                    if (!inStr)
                        break;
                }
                else
                {
                    if (!inStr)
                        inStr = true;
                }
                bytes.Add(*_ptr++);
            }
            _name = Encoding.UTF8.GetString(bytes.ToArray());
            return bytes.Count > 0;
        }
        //Reads characters into name pointer. Mainly for element/attribute names
        private bool ReadString(out string value)
        {
            int len = 0;
            bool inStr = false;
            List<byte> bytes = new List<byte>();
            byte b;

            SkipWhitespace();
            while (_ptr < _ceil)
            {
                if (inStr)
                {
                    b = *_ptr++;
                    if (b == '"')
                        break;
                    //if (b < 0x20)
                    //    continue;
                }
                else
                {
                    b = *_ptr;

                    if ((b <= 0x20) || (b == '<') || (b == '>') || (b == '/') || (b == '='))
                        break;

                    if (b == '"')
                    {
                        if (len == 0)
                        {
                            _ptr++;
                            inStr = true;
                            continue;
                        }
                        break;
                    }
                    _ptr++;
                }
                bytes.Add(b);
            }
            value = Encoding.UTF8.GetString(bytes.ToArray());
            return bytes.Count > 0;
        }
        
        private void SkipWhitespace()
        {
            while (_ptr < _ceil && *_ptr <= 0x20)
                _ptr++;
        }
        
        //Stops on tag end when inside tag.
        //Ignores comments
        //Exits current tag before searching
        public bool BeginElement()
        {
            bool comment = false;
            bool literal = false;
            byte b;

            Top:
            SkipWhitespace();
            while (_ptr < _ceil)
            {
                if (!_inTag)
                {
                    if (*_ptr++ == '<')
                    {
                        _inTag = true;
                        if (ReadString(out _name)) //Will fail on delimiter
                        {
                            if (string.Equals(_name.Substring(0, 3), "!--", StringComparison.InvariantCulture))
                                comment = true;
                            else
                                return true;
                        }
                    }
                }
                else
                {
                    //Skip string literals when inside tags
                    if (literal)
                    {
                        if (*_ptr++ == '"')
                            literal = false;
                        continue;
                    }

                    //Skip comments
                    if (comment)
                    {
                        if ((*_ptr++ == '>') && (_ptr[-2] == '-') && (_ptr[-3] == '-'))
                        {
                            comment = false;
                            _inTag = false;
                            goto Top;
                        }
                        continue;
                    }

                    if (*_ptr == '/')
                        return false;

                    b = *_ptr++;
                    if (b == '"')
                        literal = true;
                    else if (b == '>')
                        _inTag = false;
                }
            }

            return false;
        }

        //Continues until tag end has been found, then finds end bracket.
        public void EndElement()
        {
            //Guarantees that we are in the end tag, sitting on the delimiter. If not, something is wrong!
            while (BeginElement())
                EndElement();

            if (!_inTag || (_ptr >= _ceil) || (*_ptr != '/'))
                return;

            while ((_ptr < _ceil) && (*_ptr++ != '>'));

            _inTag = false;
        }

        public bool ReadAttribute()
        {
            if (!_inTag)
                return false;

            SkipWhitespace();
            if (ReadString(out _name))
            {
                SkipWhitespace();
                if ((_ptr < _ceil) && (*_ptr == '='))
                {
                    _ptr++;
                    if (ReadString(out _value))
                        return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Returns true if there is content to be read within this element.
        /// Returns false if the element is empty/self-closed.
        /// </summary>
        private unsafe bool LeaveTag()
        {
            if (!_inTag)
                return true;

            while (_ptr < _ceil)
            {
                if (*_ptr == '/')
                    return false;

                if (*_ptr++ == '>')
                {
                    _inTag = false;
                    return true;
                }
            }

            return false;
        }

        public unsafe bool ReadValue(float* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (float.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float f))
                {
                    *pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(sbyte* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (sbyte.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out sbyte f))
                {
                    *pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(byte* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (byte.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out byte f))
                {
                    *pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(short* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (short.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out short f))
                {
                    *pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(ushort* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (ushort.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out ushort f))
                {
                    *pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(int* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (int.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out int f))
                {
                    *pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(uint* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (uint.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out uint f))
                {
                    *pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(long* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (long.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out long f))
                {
                    *pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(ulong* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (ulong.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out ulong f))
                {
                    *pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(double* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out double f))
                {
                    *pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(ref float pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (float.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float f))
                {
                    pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(float* pOut, float scale)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (float.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float f))
                {
                    *pOut = f * scale;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(ref float pOut, float scale)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (float.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float f))
                {
                    pOut = f * scale;
                    return true;
                }
            }
            return false;
        }
        
        public unsafe bool ReadValue(ref int pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
            {
                if (int.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out int f))
                {
                    pOut = f;
                    return true;
                }
            }

            return false;
        }

        public bool ReadStringSingle()
        {
            if (!LeaveTag())
                return false;

            if (ReadString(out _value))
                return true;

            return false;
        }
        public void MoveBackToElementClose()
        {
            if (!LeaveTag())
            {
                _inTag = false;
                while (*(_ptr - 1) != '>' && _ptr > _base) --_ptr;
            }
        }
        public string ReadElementString()
        {
            MoveBackToElementClose();

            byte* start = _ptr;
            while (*_ptr != '<' && _ptr < _ceil) ++_ptr;
            int length = (int)_ptr - (int)start;
            return new string((sbyte*)start, 0, length, Text.Encoding.UTF8);

            //string output = "";
            //int len = 0;
            //while (*_ptr != '<')
            //{
            //    while (len < _valueMax && _ptr < _ceil && *_ptr != '<')
            //        _valPtr[len++] = *_ptr++;
            //    _valPtr[len] = 0;
            //    output += new string((sbyte*)_valPtr);
            //}

            //return output;
        }
    }
}
