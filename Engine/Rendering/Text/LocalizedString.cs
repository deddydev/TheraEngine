using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Text
{
    public class LString
    {
        public string _id;
        public string _namespace;
        public bool _global;

        public string GetLocalizedString()
        {
            return Engine.Game.LocalizedStringTable[_id];
        }

        public static implicit operator String(LString str) => str.GetLocalizedString();
    }
}
