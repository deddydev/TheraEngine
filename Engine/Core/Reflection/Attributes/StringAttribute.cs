﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public enum StringType
    {
        SingleLine,
        MultiLine,
        SinglePath,
        MultiPath,
    }
    public class StringAttribute : Attribute
    {
        public bool MultiLine { get; set; }
        public bool Path { get; set; }
        public bool Unicode { get; set; }
        public StringAttribute(bool multiLine, bool path, bool unicode)
        {
            MultiLine = multiLine;
            Path = path;
            Unicode = unicode;
        }
    }
}
