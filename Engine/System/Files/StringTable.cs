using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering.Models;

namespace CustomEngine.Files
{
    public unsafe class StringTable
    {
        SortedList<string, int> _table = new SortedList<string, int>(StringComparer.Ordinal);

        public void Add(string s)
        {
            if ((!string.IsNullOrEmpty(s)) && (!_table.ContainsKey(s)))
                _table.Add(s, 0);
        }
        public void AddRange(IEnumerable<string> values)
        {
            foreach (string s in values)
                Add(s);
        }

        public int GetTotalSize()
        {
            int len = 0;
            foreach (string s in _table.Keys)
                len += s.Length + 1;
            return len.Align(4);
        }

        public void Clear() { _table.Clear(); }

        public int this[string s]
        {
            get
            {
                if ((!string.IsNullOrEmpty(s)) && (_table.ContainsKey(s)))
                    return _table[s];
                return _table.Values[0];
            }
        }
        public void WriteTable(FileCommonHeader* address)
        {
            VoidPtr baseAddress = address->Strings;
            VoidPtr currentAddress = baseAddress;
            for (int i = 0; i < _table.Count; i++)
            {
                string s = _table.Keys[i];
                _table[s] = currentAddress - baseAddress;
                s.Write(ref currentAddress);
            }
        }
    }
}
