using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CustomEngine.Rendering.Models
{
    public class IndexPoint : ObjectBase
    {
        public IndexPoint() { }
        public IndexPoint(int vertexIndex)
        {
            _vertexIndex = vertexIndex;
        }

        public int VertexIndex { get { return _vertexIndex; } }
        public ReadOnlyCollection<IndexLine> ConnectedEdges { get { return _connectedEdges.AsReadOnly(); } }
        
        int _vertexIndex;
        List<IndexLine> _connectedEdges = new List<IndexLine>();

        internal void AddLine(IndexLine edge)
        {
            if (!_connectedEdges.Contains(edge))
                _connectedEdges.Add(edge);
        }

        internal void RemoveLine(IndexLine edge)
        {
            if (_connectedEdges.Contains(edge))
                _connectedEdges.Remove(edge);
        }

        public IndexLine LinkTo(IndexPoint otherPoint)
        {
            foreach (IndexLine edge in _connectedEdges)
                if (edge.Point0 == otherPoint || 
                    edge.Point1 == otherPoint)
                    return edge;

            //Creating a new line automatically links the points.
            return new IndexLine(this, otherPoint);
        }
        public void UnlinkFrom(IndexPoint otherPoint)
        {
            for (int i = 0; i < _connectedEdges.Count; ++i)
                if (_connectedEdges[i].Point0 == otherPoint ||
                    _connectedEdges[i].Point1 == otherPoint)
                {
                    _connectedEdges[i].Unlink();
                    return;
                }
        }

        public static implicit operator IndexPoint(int i) { return new IndexPoint(i); }
        public static implicit operator int(IndexPoint i) { return i.VertexIndex; }
    }
}
