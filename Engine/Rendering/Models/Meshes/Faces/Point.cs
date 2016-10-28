﻿using System;
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

        internal void AddLine(Line edge)
        {
            if (!_connectedEdges.Contains(edge))
                _connectedEdges.Add(edge);
        }

        internal void RemoveLine(Line edge)
        {
            if (_connectedEdges.Contains(edge))
                _connectedEdges.Remove(edge);
        }

        public Line LinkTo(Point otherPoint)
        {
            foreach (Line edge in _connectedEdges)
                if (edge.Point0 == otherPoint || 
                    edge.Point1 == otherPoint)
                    return edge;

            //Creating a new line automatically links the points.
            return new Line(this, otherPoint);
        }
        public void UnlinkFrom(Point otherPoint)
        {
            for (int i = 0; i < _connectedEdges.Count; ++i)
                if (_connectedEdges[i].Point0 == otherPoint ||
                    _connectedEdges[i].Point1 == otherPoint)
                {
                    _connectedEdges[i].Unlink();
                    return;
                }
        }

        public static implicit operator Point(int i) { return new Point(i); }
        public static implicit operator int(Point i) { return i.VertexIndex; }
    }
}
