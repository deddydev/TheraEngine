using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    public class FacePoint : TObject
    {
        public int VertexIndex => _vertexIndex;
        public List<int> BufferIndices => _bufferIndices;

        //[Serialize("VertexIndex")]
        private int _vertexIndex;
        [TSerialize("BufferIndices")]
        private List<int> _bufferIndices = new List<int>();
        [TSerialize("InfluenceIndex")]
        internal int _influenceIndex;
        internal PrimitiveData _data;

        public FacePoint(int index, PrimitiveData data)
        {
            _vertexIndex = index;
            _data = data;
        }

        public InfluenceDef GetInfluence()
            => _data._influences?[_influenceIndex];

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

        public override int GetHashCode() => ToString().GetHashCode();
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
