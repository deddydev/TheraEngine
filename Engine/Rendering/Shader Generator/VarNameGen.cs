using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering
{
    public class VarNameGen
    {
        private int _generatedNameCount = 0;
        private string _selection = "abcdefghijklmnopqrstuvwxyz";
        
        public string New()
        {
            int digitCount = (_generatedNameCount % _selection.Length) + 1;
            int digitIndex = _generatedNameCount / _selection.Length;
            string s = "";
            for (int i = 0; i < digitCount; ++i)
                s += _selection[digitIndex];
            ++_generatedNameCount;
            return s;
        }
    }
}
