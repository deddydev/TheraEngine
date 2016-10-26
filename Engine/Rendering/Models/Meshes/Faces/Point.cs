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
        public ReadOnlyCollection<Point> ConnectedPoints { get { return _connectedPoints.AsReadOnly(); } }

        int _vertexIndex;
        List<Point> _connectedPoints;

        public void LinkTo(Point otherPoint)
        {
            if (!_connectedPoints.Contains(otherPoint))
            {
                _connectedPoints.Add(otherPoint);
                otherPoint.LinkTo(this);
            }
        }
        public void UnlinkFrom(Point otherPoint)
        {
            if (_connectedPoints.Contains(otherPoint))
            {
                _connectedPoints.Remove(otherPoint);
                otherPoint.UnlinkFrom(this);
            }
        }

        public static implicit operator Point(int i) { return new Point(i); }
        public static implicit operator int(Point i) { return i.VertexIndex; }
    }
}
