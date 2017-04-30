using System.Globalization;

namespace System.IO
{
    public unsafe class XMLReader
    {
        private const char NullTerminator = '\0';
        private const int NameMax = 128;
        private const int ValueMax = 384;
        
        internal bool _inTag;
        private char[] _nameBuffer, _valueBuffer;
        private string _name, _value;
        private StreamReader _reader;

        public String Name => _name;
        public String Value => _value;

        public bool End => _reader.EndOfStream;

        public StreamReader Reader => _reader;

        public int Byte(bool advance = false)
            => advance ? _reader.Read() : _reader.Peek();

        public XMLReader(Stream stream)
        {
            _reader = new StreamReader(stream, Text.Encoding.UTF8, false);
            _nameBuffer = new char[NameMax];
            _valueBuffer = new char[ValueMax];

            //Find start of Xml file
            if (BeginElement() && Name.Equals("?xml"))
            {
                while (!End && (Byte(true) != '>'));

                _inTag = false;
            }
            else
                throw new IOException("File is not a valid XML file.");

        }

        //private bool ReadString()
        //{
        //    int len = 0;
        //    bool inStr = false;

        //    while ((len < _valueMax) && (_ptr < _ceil))
        //    {
        //        if (*_ptr <= 0x20)
        //        {
        //            if (inStr)
        //                break;

        //            inStr = true;
        //            continue;
        //        }
        //        else if ((*_ptr == '<') || (*_ptr == '>') || (*_ptr == '/'))
        //        {
        //            if (!inStr)
        //                break;
        //        }
        //        else
        //        {
        //            if (!inStr)
        //                inStr = true;
        //        }
        //        _valueBuffer[len++] = (char)*_ptr++;
        //    }
        //    _valueBuffer[len] = NullTerminator;

        //    return len > 0;
        //}
        //Reads characters into name pointer. Mainly for element/attribute names
        private bool ReadString(char[] pOut)
        {
            int len = 0;
            bool inStr = false;
            int b;

            SkipWhitespace();
            while ((len < pOut.Length) && !End)
            {
                if (inStr)
                {
                    b = Byte(true);
                    if (b == '"')
                        break;
                    //if (b < 0x20)
                    //    continue;
                }
                else
                {
                    b = Byte();

                    if ((b <= 0x20) || (b == '<') || (b == '>') || (b == '/') || (b == '='))
                        break;

                    if (b == '"')
                    {
                        if (len == 0)
                        {
                            Byte(true);
                            inStr = true;
                            continue;
                        }
                        break;
                    }
                    Byte(true);
                }
                pOut[len++] = (char)b;
            }
            pOut[len] = NullTerminator;
            return len > 0;
        }

        private void SkipWhitespace()
        {
            while (!End && (Byte() <= 0x20))
                Byte(true);
        }
        
        //Stops on tag end when inside tag.
        //Ignores comments
        //Exits current tag before searching
        public bool BeginElement()
        {
            bool comment = false;
            bool literal = false;
            int b;

            Top:
            SkipWhitespace();
            while (!End)
            {
                if (!_inTag)
                {
                    if (Byte(true) == '<')
                    {
                        _inTag = true;
                        if (ReadString(_nameBuffer)) //Will fail on delimiter
                        {
                            _name = new string(_nameBuffer);
                            if (_nameBuffer[0] == '!' &&
                                _nameBuffer[1] == '-' &&
                                _nameBuffer[2] == '-')
                                comment = true;
                            else
                                return true;
                        }
                        else
                            _name = "";
                    }
                }
                else
                {
                    //Skip string literals when inside tags
                    if (literal)
                    {
                        if (Byte(true) == '"')
                            literal = false;
                        continue;
                    }

                    //Skip comments
                    if (comment)
                    {
                        if ((Byte(true) == '-') && (Byte(true) == '-') && (Byte(true) == '>'))
                        {
                            comment = false;
                            _inTag = false;
                            goto Top;
                        }
                        continue;
                    }

                    if (Byte() == '/')
                        return false;

                    b = Byte(true);
                    if (b == '"')
                        literal = true;
                    else if (b == '>')
                        _inTag = false;
                }

                //if ((*_ptr == '/') && _inTag)
                //    return false;

                //b = *_ptr++;

                //if (b == '"')
                //{
                //    if (_inTag)
                //        literal = true;
                //}
                //else if (b == '<')
                //{
                //    _inTag = true;
                //    if (ReadString(_namePtr, _nameMax)) //Will fail on delimiter
                //    {
                //        if ((_namePtr[0] == '!') && (_namePtr[1] == '-') && (_namePtr[2] == '-'))
                //            comment = true;
                //        else
                //            return true;
                //    }
                //}
                //else if (b == '>')
                //{
                //    _inTag = false;
                //    goto Top;
                //}
            }

            return false;
        }

        //Continues until tag end has been found, then finds end bracket.
        public void EndElement()
        {
            //Guarantees that we are in the end tag, sitting on the delimiter. If not, something is wrong!
            while (BeginElement())
                EndElement();

            if (!_inTag || End || (Byte() != '/'))
                return;

            while (!End && (Byte(true) != '>'));

            _inTag = false;
        }

        public bool ReadAttribute()
        {
            if (!_inTag)
                return false;

            SkipWhitespace();
            if (ReadString(_nameBuffer))
            {
                _name = new string(_nameBuffer);
                SkipWhitespace();
                if (!End && (Byte() == '='))
                {
                    Byte(true);
                    if (ReadString(_valueBuffer))
                    {
                        _value = new string(_valueBuffer);
                        return true;
                    }
                    else
                        _value = "";
                }
            }
            else
                _name = "";

            return false;
        }
        private unsafe bool LeaveTag()
        {
            if (!_inTag)
                return true;

            while (!End)
            {
                if (Byte() == '/')
                    return false;

                if (Byte(true) == '>')
                {
                    _inTag = false;
                    return true;
                }
            }

            return false;
        }
        private unsafe bool ReadValue()
        {
            int len = 0;
            int b;

            SkipWhitespace();
            while ((len + 1 < ValueMax) && !End)
            {
                b = Byte();

                if ((b <= 0x20) || (b == '<'))
                    break;

                Byte(true);
                _valueBuffer[len++] = (char)b;
            }
            _valueBuffer[len] = NullTerminator;
            if (len > 0)
            {
                _value = new string(_valueBuffer);
                return true;
            }
            else
            {
                _value = "";
                return false;
            }
        }
        public unsafe bool ReadValue(float* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadValue())
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
            
            if (ReadValue())
            {
                if (float.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float f))
                {
                    pOut = f;
                    return true;
                }
            }
            return false;
        }
        public unsafe bool ReadValue(int* pOut)
        {
            if (!LeaveTag())
                return false;

            if (ReadValue())
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

            if (ReadValue())
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

            if (ReadString(_valueBuffer))
            {
                _value = new string(_valueBuffer);
                return true;
            }
            else
                _value = "";

            return false;
        }

        public string ReadElementString(bool advance = true)
        {
            long pos = _reader.BaseStream.Position;
            bool inTag = _inTag;

            if (!LeaveTag())
                return null;

            int len = 0;
            while ((len < ValueMax) && !End && (Byte() != '<'))
                _valueBuffer[len++] = (char)Byte(true);

            _valueBuffer[len] = NullTerminator;

            if (!advance)
            {
                _reader.BaseStream.Position = pos;
                _inTag = inTag;
            }

            return _value = new string(_valueBuffer);
        }
    }
}
