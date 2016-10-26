using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class FacePoint : ObjectBase
    {
        int _index;
        List<int> _bufferIndices = new List<int>();

        public List<int> Indices { get { return _bufferIndices; } }

        public FacePoint(int index)
        {
            _index = index;
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

        public override string ToString()
        {
            string vtx = "VTX{_index}";
            if (_bufferIndices.Count > 0)
                vtx += ": ";
            for (int i = 0; i < _bufferIndices.Count; ++i)
                vtx += "(" + i + ": " + _bufferIndices[i] + ")";
            return vtx;
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
