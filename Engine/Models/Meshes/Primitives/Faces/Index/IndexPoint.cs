﻿using TheraEngine.ComponentModel;
using TheraEngine.Core.Memory;

namespace TheraEngine.Rendering.Models
{
    public class IndexPoint : TObject, ISerializableString, ISerializablePointer
    {
        public IndexPoint() { }
        public IndexPoint(int vertexIndex)
            => _vertexIndex = vertexIndex;

        public int VertexIndex
            => _vertexIndex;
        //public ReadOnlyCollection<IndexLine> ConnectedEdges
        //    => _connectedEdges.AsReadOnly();

        public override string ToString()
            => _vertexIndex.ToString();

        [TSerialize("Index", NodeType = ENodeType.Attribute)]
        private int _vertexIndex;
        //private List<IndexLine> _connectedEdges = new List<IndexLine>();

        //internal void AddLine(IndexLine edge)
        //{
        //    if (!_connectedEdges.Contains(edge))
        //        _connectedEdges.Add(edge);
        //}
        //internal void RemoveLine(IndexLine edge)
        //{
        //    if (_connectedEdges.Contains(edge))
        //        _connectedEdges.Remove(edge);
        //}

        public string WriteToString() => _vertexIndex.ToString();
        public void ReadFromString(string str)
        {
            if (!int.TryParse(str, out _vertexIndex))
                _vertexIndex = 0;
        }

        public int GetSize() => sizeof(int);
        public void WriteToPointer(VoidPtr address) => address.WriteInt(_vertexIndex, false);
        public void ReadFromPointer(VoidPtr address, int size) => _vertexIndex = address.ReadInt(false);

        //public IndexLine LinkTo(IndexPoint otherPoint)
        //{
        //    foreach (IndexLine edge in _connectedEdges)
        //        if (edge.Point0 == otherPoint || 
        //            edge.Point1 == otherPoint)
        //            return edge;

        //    //Creating a new line automatically links the points.
        //    return new IndexLine(this, otherPoint);
        //}
        //public void UnlinkFrom(IndexPoint otherPoint)
        //{
        //    for (int i = 0; i < _connectedEdges.Count; ++i)
        //        if (_connectedEdges[i].Point0 == otherPoint ||
        //            _connectedEdges[i].Point1 == otherPoint)
        //        {
        //            _connectedEdges[i].Unlink();
        //            return;
        //        }
        //}

        public static implicit operator IndexPoint(int i) => new IndexPoint(i);
        public static implicit operator int(IndexPoint i) => i.VertexIndex;
    }
}
