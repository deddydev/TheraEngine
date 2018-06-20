using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    public class FacePoint : TObject
    {
        public int VertexIndex { get; set; }

        [TSerialize]
        public List<int> BufferIndices { get; } = new List<int>();
        [TSerialize]
        internal int InfluenceIndex { get; set; }

        internal PrimitiveData _data;

        public FacePoint(int index, PrimitiveData data)
        {
            VertexIndex = index;
            _data = data;
        }

        public InfluenceDef GetInfluence()
            => _data._influences?[InfluenceIndex];

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
            string vtx = "VTX" + VertexIndex;
            if (BufferIndices.Count > 0)
                vtx += ": ";
            for (int i = 0; i < BufferIndices.Count; ++i)
                vtx += "(" + i + ": " + BufferIndices[i] + ")";
            return vtx;
        }
    }
}
