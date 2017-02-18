using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class FacePoint : ObjectBase
    {
        public int Index { get { return _index; } }
        public List<int> Indices { get { return _bufferIndices; } }

        private int _index;
        private List<int> _bufferIndices = new List<int>();
        internal PrimitiveData _data;
        internal Influence _influence;

        public FacePoint(int index, PrimitiveData data)
        {
            _index = index;
            _data = data;
        }

        //public int this[string name]
        //{
        //    get { return _bufferIndices.ContainsKey(name) ? _bufferIndices[name] : -1; }
        //    set
        //    {
        //        if (_bufferIndices.ContainsKey(name))
        //            _bufferIndices[name] = value;
        //        else
        //            _bufferIndices.Add(name, value);
        //    }
        //}

        public override int GetHashCode() { return ToString().GetHashCode(); }
        public override string ToString()
        {
            string vtx = "VTX" + _index;
            if (_bufferIndices.Count > 0)
                vtx += ": ";
            for (int i = 0; i < _bufferIndices.Count; ++i)
                vtx += "(" + i + ": " + _bufferIndices[i] + ")";
            return vtx;
        }
    }
}
