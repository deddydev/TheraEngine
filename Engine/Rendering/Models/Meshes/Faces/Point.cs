using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Point : ObjectBase
    {
        public Point() { }
        public Point(uint vertexIndex)
        {
            _vertexIndex = vertexIndex;
        }

        uint _vertexIndex;
        List<Line> _connectedEdges;
    }
}
