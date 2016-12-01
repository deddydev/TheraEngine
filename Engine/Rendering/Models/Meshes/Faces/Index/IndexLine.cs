using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class IndexLine : ObjectBase
    {
        public IndexLine() { }
        public IndexLine(Point point1, Point point2)
        {
            _point0 = point1;
            _point1 = point2;

            _point0.AddLine(this);
            _point1.AddLine(this);
        }

        Point _point0, _point1;
        public List<IndexPrimitive> _connectedFaces = new List<IndexPrimitive>();

        public Point Point0 { get { return _point0; } }
        public Point Point1 { get { return _point1; } }

        public void Unlink()
        {
            _point0.RemoveLine(this);
            _point1.RemoveLine(this);
        }

        internal void AddFace(IndexPrimitive poly)
        {
            if (!_connectedFaces.Contains(poly))
                _connectedFaces.Add(poly);
        }

        internal void RemoveFace(IndexPrimitive poly)
        {
            if (_connectedFaces.Contains(poly))
                _connectedFaces.Remove(poly);
        }
    }
}
