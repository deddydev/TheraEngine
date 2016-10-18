using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class QuadStrip : Polygon
    {
        public QuadStrip() { _quads = new List<Quad>(); }
        public QuadStrip(params Quad[] quads)
        {
            _quads = quads.ToList();
        }

        List<Quad> _quads = new List<Quad>();
    }
}
