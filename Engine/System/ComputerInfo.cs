using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class ComputerInfo
    {
        public int _processorCount;
        public bool _is64BitOS;
        public OperatingSystem _osVersion;

        public static ComputerInfo Analyze()
        {
            ComputerInfo c = new ComputerInfo();
            c._processorCount = Environment.ProcessorCount;
            c._osVersion = Environment.OSVersion;
            c._is64BitOS = Environment.Is64BitOperatingSystem;
            return c;
        }
    }
}
