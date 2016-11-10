using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class RenderHandler
    {
        private class HashElement
        {
            RenderKey key;
            Action value;
        }

        private LinkedList<HashElement>[] _table;
        private int _elementCount;
        private double _maxLoadFactor;

        public RenderHandler(int size)
        {
            _table = new LinkedList<HashElement>[size];
        }

        //public bool Add(RenderKey key, Action value)
        //{
        //    if (Contains(key))
        //        return false;


        //}

        //public bool Remove(RenderKey key)
        //{

        //}

        //public bool Contains(RenderKey key)
        //{

        //}

        private ulong IndexOf(RenderKey key)
        {
            return key % (ulong)_table.Length;
        }

        private double LoadFactor() { return (double)_elementCount / _table.Length; }
    }
}
