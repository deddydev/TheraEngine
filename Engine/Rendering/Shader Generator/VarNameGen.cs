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
        private string _selection = "abcdefghijklmnopqrstuvwxyz0123456789";
        
        public string New()
        {
            int digitCount = ((_generatedNameCount) % _selection.Length) + 1;

            for (int i = 1; i < digitCount; ++i)
            {

            }
            ++_generatedNameCount;

            return "";
        }
    }
}
