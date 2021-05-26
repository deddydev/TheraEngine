namespace TheraEngine.Rendering.Models
{
    public class VertexLineStrip : TVertexPrimitive
    {
        public override FaceType Type => ClosedLoop ? FaceType.LineLoop : FaceType.LineStrip;

        public bool ClosedLoop { get; set; }

        public VertexLineStrip(bool closedLoop, params TVertex[] vertices)
            : base(vertices) => ClosedLoop = closedLoop;
        
        public TVertexLine[] ToLines()
        {
            int count = _vertices.Count;
            if (!ClosedLoop && count > 0)
                --count;
            TVertexLine[] lines = new TVertexLine[count];
            for (int i = 0; i < count; ++i)
            {
                TVertex next = i + 1 == _vertices.Count ? _vertices[0] : _vertices[i + 1];
                lines[i] = new TVertexLine(_vertices[i], next);
            }
            return lines;
        }
    }
}
