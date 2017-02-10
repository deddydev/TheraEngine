using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class VertexLineStrip : VertexPrimitive
    {
        public override FaceType Type { get { return _closedLoop ? FaceType.LineLoop : FaceType.LineStrip; } }
        bool _closedLoop;

        public VertexLineStrip(bool closedLoop, params Vertex[] vertices) : base(vertices)
        {
            _closedLoop = closedLoop;
        }

        public VertexLine[] ToLines()
        {
            int count = _vertices.Count;
            if (!_closedLoop)
                --count;
            VertexLine[] lines = new VertexLine[count];
            for (int i = 0; i < count; ++i)
            {
                Vertex next = i + 1 == _vertices.Count ? _vertices[0] : _vertices[i + 1];
                lines[i] = new VertexLine(_vertices[i], next);
            }
            return lines;
        }
    }
}
