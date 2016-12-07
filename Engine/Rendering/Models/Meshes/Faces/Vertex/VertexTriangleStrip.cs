using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class VertexTriangleStrip
    {
        public List<Vertex> _points;

        public VertexTriangleStrip(params Vertex[] vertices)
        {
            if (vertices.Length < 3)
                throw new Exception("Not enough points for a triangle strip.");
            _points = vertices.ToList();
        }
    }
}
