using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CustomEngine.Rendering.Models
{
    public class Point : ObjectBase
    {
        public Point() { }
        public Point(int vertexIndex)
        {
            _vertexIndex = vertexIndex;
        }

        public int VertexIndex { get { return _vertexIndex; } }
        public ReadOnlyCollection<Line> ConnectedEdges { get { return _connectedEdges.AsReadOnly(); } }
        
        int _vertexIndex;
        List<Line> _connectedEdges = new List<Line>();

        public void LinkTo(Point otherPoint, bool noCheck = false)
        {
            if (!noCheck)
                foreach (Line edge in _connectedEdges)
                    if (edge.Point1 == otherPoint)
                        return;
            _connectedEdges.Add(new Line(this, otherPoint));
            if (!noCheck)
                otherPoint.LinkTo(this, true);
        }
        public void UnlinkFrom(Point otherPoint, bool noCheck = false)
        {
            if (_connectedEdges.Contains(otherPoint))
            {
                _connectedEdges.Remove(otherPoint);
                otherPoint.UnlinkFrom(this);
            }
        }

        public static implicit operator Point(int i) { return new Point(i); }
        public static implicit operator int(Point i) { return i.VertexIndex; }
    }
}
