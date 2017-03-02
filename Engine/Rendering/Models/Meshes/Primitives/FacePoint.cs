using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class FacePoint : ObjectBase
    {
        public int VertexIndex { get { return _vertexIndex; } }
        public List<int> BufferIndices { get { return _bufferIndices; } }

        private int _vertexIndex;
        private List<int> _bufferIndices = new List<int>();
        internal PrimitiveData _data;
        internal int _influenceIndex;

        public FacePoint(int index, PrimitiveData data)
        {
            _vertexIndex = index;
            _data = data;
        }

        public Influence GetInfluence()
        {
            return _data._influences?[_influenceIndex];
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
            string vtx = "VTX" + _vertexIndex;
            if (_bufferIndices.Count > 0)
                vtx += ": ";
            for (int i = 0; i < _bufferIndices.Count; ++i)
                vtx += "(" + i + ": " + _bufferIndices[i] + ")";
            return vtx;
        }
    }
}
