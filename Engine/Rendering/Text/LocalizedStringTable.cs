using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Rendering.Text
{
    public class LocalizedStringTable
    {
        [TSerialize]
        private Dictionary<string, string> _table = new Dictionary<string, string>();

        public string this[string id, string nameSpace]
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
