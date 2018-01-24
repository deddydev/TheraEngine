using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models
{
    [FileExt("primdata")]
    [FileDef("Mesh Primitive Data")]
    public partial class PrimitiveData : FileObject, IDisposable
    {
        public static PrimitiveData FromQuads(Culling culling, VertexShaderDesc info, params VertexQuad[] quads)
        {
            return FromQuadList(culling, info, quads);
        }
        public static PrimitiveData FromQuadList(Culling culling, VertexShaderDesc info, IEnumerable<VertexQuad> quads)
        {
            return FromTriangleList(culling, info, quads.SelectMany(x => x.ToTriangles()));
        }
        public static PrimitiveData FromTriangleStrips(Culling culling, VertexShaderDesc info, params VertexTriangleStrip[] strips)
        {
            return FromTriangleStripList(culling, info, strips);
        }
        public static PrimitiveData FromTriangleStripList(Culling culling, VertexShaderDesc info, IEnumerable<VertexTriangleStrip> strips)
        {
            return FromTriangleList(culling, info, strips.SelectMany(x => x.ToTriangles()));
        }
        public static PrimitiveData FromTriangleFans(Culling culling, VertexShaderDesc info, params VertexTriangleFan[] fans)
        {
            return FromTriangleFanList(culling, info, fans);
        }
        public static PrimitiveData FromTriangleFanList(Culling culling, VertexShaderDesc info, IEnumerable<VertexTriangleFan> fans)
        {
            return FromTriangleList(culling, info, fans.SelectMany(x => x.ToTriangles()));
        }
        public static PrimitiveData FromTriangles(Culling culling, VertexShaderDesc info, params VertexTriangle[] triangles)
        {
            return FromTriangleList(culling, info, triangles);
        }
        public static PrimitiveData FromTriangleList(Culling culling, VertexShaderDesc info, IEnumerable<VertexTriangle> triangles)
        {
            //TODO: convert triangles to tristrips and use primitive restart to render them all in one call
            return new PrimitiveData(culling, info, triangles.SelectMany(x => x.Vertices), EPrimitiveType.Triangles);
        }
        public static PrimitiveData FromLineStrips(VertexShaderDesc info, params VertexLineStrip[] lines)
        {
            return FromLineStripList(info, lines);
        }
        public static PrimitiveData FromLineStripList(VertexShaderDesc info, IEnumerable<VertexLineStrip> lines)
        {
            return FromLineList(info, lines.SelectMany(x => x.ToLines()));
        }
        public static PrimitiveData FromLines(VertexShaderDesc info, params VertexLine[] lines)
        {
            return FromLineList(info, lines);
        }
        public static PrimitiveData FromLineList(VertexShaderDesc info, IEnumerable<VertexLine> lines)
        {
            return new PrimitiveData(Culling.None, info, lines.SelectMany(x => x.Vertices), EPrimitiveType.Lines);
        }
        public static PrimitiveData FromPoints(params Vec3[] points)
        {
            return FromPointList(points);
        }
        public static PrimitiveData FromPointList(IEnumerable<Vec3> points)
        {
            return new PrimitiveData(Culling.None, VertexShaderDesc.JustPositions(), points.Select(x => new Vertex(x)), EPrimitiveType.Points);
        }

        public PrimitiveData()
        {

        }
        public PrimitiveData(Culling culling, VertexShaderDesc info, IEnumerable<Vertex> points, EPrimitiveType type)
        {
            _bufferInfo = info;
            _type = type;
            _influences = null;
            _culling = culling;

            List<Vertex> vertices = points.ToList();
            Remapper remapper = null;
            switch (_type)
            {
                case EPrimitiveType.Triangles:
                    remapper = SetTriangleIndices(vertices);
                    break;
                case EPrimitiveType.Lines:
                    remapper = SetLineIndices(vertices);
                    break;
                default:
                //case EPrimitiveType.Points:
                    remapper = SetPointIndices(vertices);
                    break;
            }

            int[] firstAppearanceArray = null;
            if (remapper == null)
            {
                firstAppearanceArray = new int[vertices.Count];
                for (int i = 0; i < vertices.Count; ++i)
                    firstAppearanceArray[i] = i;
            }
            else
                firstAppearanceArray = remapper.ImplementationTable;

            CreateFacePoints(firstAppearanceArray.Length);

            if (info.HasSkinning)
                SetInfluences(firstAppearanceArray.Select(x => vertices[x]._influence).ToArray());

            for (int i = 0; i < info._morphCount + 1; ++i)
            {
                var data = firstAppearanceArray.Select(x => vertices[x]._position).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.Position, i));
            }
            if (info.HasNormals)
                for (int i = 0; i < info._morphCount + 1; ++i)
                {
                    var data = firstAppearanceArray.Select(x => vertices[x]._normal).ToList();
                    AddBuffer(data, new VertexAttribInfo(BufferType.Normal, i));
                }
            if (info.HasBinormals)
                for (int i = 0; i < info._morphCount + 1; ++i)
                {
                    var data = firstAppearanceArray.Select(x => vertices[x]._binormal).ToList();
                    AddBuffer(data, new VertexAttribInfo(BufferType.Binormal, i));
                }
            if (info.HasTangents)
                for (int i = 0; i < info._morphCount + 1; ++i)
                {
                    var data = firstAppearanceArray.Select(x => vertices[x]._tangent).ToList();
                    AddBuffer(data, new VertexAttribInfo(BufferType.Tangent, i));
                }
            for (int i = 0; i < info._colorCount; ++i)
            {
                var data = firstAppearanceArray.Select(x => vertices[x]._color).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.Color, i));
            }
            for (int i = 0; i < info._texcoordCount; ++i)
            {
                var data = firstAppearanceArray.Select(x => vertices[x]._texCoord).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.TexCoord, i));
            }
        }
    }
    public enum Culling
    {
        None,
        Back,
        Front,
        Both
    }
    public enum EPrimitiveType
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
        Quads = 7,
        QuadStrip = 8,
        Polygon = 9,
        LinesAdjacency = 10,
        LineStripAdjacency = 11,
        TrianglesAdjacency = 12,
        TriangleStripAdjacency = 13,
        Patches = 14,
    }
}
