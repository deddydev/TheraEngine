using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomEngine.Rendering.Models
{
    public class IndexLine : ObjectBase
    {
        public IndexLine() { }
        public IndexLine(IndexPoint point1, IndexPoint point2)
        {
            _point0 = point1;
            _point1 = point2;

            _point0.AddLine(this);
            _point1.AddLine(this);
        }

        [Serialize("Point0")]
        private IndexPoint _point0;
        [Serialize("Point1")]
        private IndexPoint _point1;

        public List<IndexPrimitive> _connectedFaces = new List<IndexPrimitive>();

        public IndexPoint Point0 => _point0;
        public IndexPoint Point1 => _point1;

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
