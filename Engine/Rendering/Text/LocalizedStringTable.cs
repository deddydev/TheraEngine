using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Text
{
    public class LocalizedStringTable
    {
        [Serialize]
        private Dictionary<string, string> _table = new Dictionary<string, string>();
        public string this[string id]
        {
            get
            {
                if (!_table.ContainsKey(id))
                    throw new Exception("Invalid localized string id: " + id);
                return _table[id];
            }
        }

#if EDITOR
        public Dictionary<string, string> Table => _table;
#endif
    }
}
