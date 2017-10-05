using System.Globalization;

namespace System.IO
{
    public unsafe class XMLReader : IDisposable
    {
        private const int _nameMax = 128;
        private const int _valueMax = 384;

        internal byte* _base, _ptr, _ceil;
        private int _length, _position;
        internal bool _inTag;

        private DataSource _stringBuffer;
        private byte* _namePtr, _valPtr;

        public PString Name => _namePtr;
        public PString Value => _valPtr;

        public XMLReader()
        {
            _stringBuffer = new DataSource(_nameMax + _valueMax + 2);
            _namePtr = (byte*)_stringBuffer.Address;
            _valPtr = _namePtr + _nameMax + 1;
        }
        public XMLReader(void* pSource, int length, bool needsOpenDocument) : this()
        {
            SetMemoryAddress(pSource, length, needsOpenDocument);
        }

        public void SetMemoryAddress(DataSource source, bool needsOpenDocument) => SetMemoryAddress(source.Address, source.Length, needsOpenDocument);
        public void SetMemoryAddress(VoidPtr pSource, int length, bool needsOpenDocument)
        {
            _position = 0;
            _length = length;
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

        ~XMLReader() { Dispose(); }
        public void Dispose()
        {
            if (_stringBuffer != null)
            {
                _stringBuffer.Dispose();
                _stringBuffer = null;
            }
        }

        private bool ReadString()
        {
            int len = 0;
            bool inStr = false;
            byte* pOut = _valPtr;

            while ((len < _valueMax) && (_ptr < _ceil))
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
                pOut[len++] = *_ptr++;
            }
            pOut[len] = 0;

            return len > 0;
        }
        //Reads characters into name pointer. Mainly for element/attribute names
        private bool ReadString(byte* pOut, int length)
        {
            int len = 0;
            bool inStr = false;
            //byte* pOut = _namePtr;
            byte b;

            SkipWhitespace();
            while ((len < length) && (_ptr < _ceil))
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

                pOut[len++] = b;
            }
            pOut[len] = 0;

            return len > 0;
        }
        
        private void SkipWhitespace()
        {
            while ((_ptr < _ceil) && (*_ptr <= 0x20))
                _ptr++;
        }

        //Read next non-whitespace byte. Returns -1 on EOF
        private int ReadByte()
        {
            byte b;
            if (_position < _length)
            {
                b = *(_base + _position++);
                if (b >= 0x20)
                    return b;
            }
            return -1;
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
                        if (ReadString(_namePtr, _nameMax)) //Will fail on delimiter
                        {
                            if ((_namePtr[0] == '!') && (_namePtr[1] == '-') && (_namePtr[2] == '-'))
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
            if (ReadString(_namePtr, _nameMax))
            {
                SkipWhitespace();
                if ((_ptr < _ceil) && (*_ptr == '='))
                {
                    _ptr++;
                    if (ReadString(_valPtr, _valueMax))
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

            if (ReadString(_valPtr, _valueMax))
            {
                if (float.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float f))
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

            if (ReadString(_valPtr, _valueMax))
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

            if (ReadString(_valPtr, _valueMax))
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

            if (ReadString(_valPtr, _valueMax))
            {
                if (float.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float f))
                {
                    pOut = f * scale;
                    return true;
                }
            }
            return false;
        }

        public unsafe bool ReadValue(int* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(_valPtr, _valueMax))
            {
                if (int.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out int f))
                {
                    *pOut = f;
                    return true;
                }
            }

            return false;
        }
        public unsafe bool ReadValue(ref int pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadString(_valPtr, _valueMax))
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

            if (ReadString(_valPtr, _valueMax))
                return true;

            return false;
        }

        public string ReadElementString()
        {
            if (!LeaveTag())
                return null;

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
