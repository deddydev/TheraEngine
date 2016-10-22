using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class FacePoint : ObjectBase
    {
        int _index;
        Dictionary<string, int> _bufferIndices = new Dictionary<string, int>();

        public FacePoint(int index) { _index = index; }

        public int this[string name]
        {
            get { return _bufferIndices.ContainsKey(name) ? _bufferIndices[name] : -1; }
            set
            {
                if (_bufferIndices.ContainsKey(name))
                    _bufferIndices[name] = value;
                else
                    _bufferIndices.Add(name, value);
            }
        }

        public override string ToString()
        {
            string vtx = "VTX{_index}";
            if (_bufferIndices.Count > 0)
                vtx += ": ";
            foreach (var c in _bufferIndices)
                vtx += "(" + c.Key + ": " + c.Value + ")";
            return vtx;
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
