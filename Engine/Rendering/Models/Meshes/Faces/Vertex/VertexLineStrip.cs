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
    }
}
