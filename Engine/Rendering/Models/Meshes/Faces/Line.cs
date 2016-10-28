using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Line : ObjectBase
    {
        public Line() { }
        public Line(Point point1, Point point2)
        {
            _point0 = point1;
            _point1 = point2;

            _point0.AddLine(this);
            _point1.AddLine(this);
        }

        Point _point0, _point1;
        public List<Polygon> _connectedFaces = new List<Polygon>();

        public Point Point0 { get { return _point0; } }
        public Point Point1 { get { return _point1; } }

        public void Unlink()
        {
            _point0.RemoveLine(this);
            _point1.RemoveLine(this);
        }

        internal void AddFace(Polygon poly)
        {
            if (!_connectedFaces.Contains(poly))
                _connectedFaces.Add(poly);
        }

        internal void RemoveFace(Polygon poly)
        {
            if (_connectedFaces.Contains(poly))
                _connectedFaces.Remove(poly);
        }
    }
}
