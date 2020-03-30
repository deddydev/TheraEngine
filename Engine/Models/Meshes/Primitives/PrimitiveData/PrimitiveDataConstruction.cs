using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    [TFileExt("primdata")]
    [TFileDef("Mesh Primitive Data")]
    public partial class Mesh : TFileObject, IDisposable
    {
        private static Dictionary<Type, EPrimitiveType> PrimTypeDic { get; }
            = new Dictionary<Type, EPrimitiveType>()
            {
                { typeof(VertexQuad), EPrimitiveType.Quads },
                { typeof(VertexTriangle), EPrimitiveType.Triangles },
                { typeof(VertexTriangleFan), EPrimitiveType.TriangleFan },
                { typeof(VertexTriangleStrip), EPrimitiveType.TriangleStrip },
                { typeof(VertexLine), EPrimitiveType.Lines },
                { typeof(VertexLineStrip), EPrimitiveType.LineStrip },
                { typeof(Vertex), EPrimitiveType.Points },
            };

        private static Dictionary<EPrimitiveType, Func<IEnumerable<VertexPrimitive>, IEnumerable<Vertex>>> PrimConvDic { get; }
            = new Dictionary<EPrimitiveType, Func<IEnumerable<VertexPrimitive>, IEnumerable<Vertex>>>()
            {
                { EPrimitiveType.Quads, p => p.SelectMany(x => ((VertexQuad)x).ToTriangles()).SelectMany(x => x.Vertices) },
                { EPrimitiveType.Triangles, p => p.SelectMany(x => x.Vertices) },
                { EPrimitiveType.TriangleFan, p => p.SelectMany(x => ((VertexTriangleFan)x).ToTriangles()).SelectMany(x => x.Vertices) },
                { EPrimitiveType.TriangleStrip, p => p.SelectMany(x => ((VertexTriangleStrip)x).ToTriangles()).SelectMany(x => x.Vertices) },
                { EPrimitiveType.Lines, p => p.SelectMany(x => x.Vertices) },
                { EPrimitiveType.LineStrip, p => p.SelectMany(x => ((VertexLineStrip)x).ToLines()).SelectMany(x => x.Vertices) },
                { EPrimitiveType.Points, p => p.Select(x => (Vertex)x) },
            };

        public static Mesh Create<T>(VertexShaderDesc info, params T[] polygons) where T : VertexPrimitive
        {
            if (polygons is null || !GetPrimType<T>(out EPrimitiveType type))
                return null;

            return new Mesh(info, PrimConvDic[type](polygons), type);
        }
        public static Mesh Create<T>(VertexShaderDesc info, IEnumerable<T> polygons) where T : VertexPrimitive
        {
            if (polygons is null || !GetPrimType<T>(out EPrimitiveType type))
                return null;

            return new Mesh(info, PrimConvDic[type](polygons), type);
        }
        public static Mesh Create(params Vec3[] points)
        {
            if (points is null)
                return null;

            return new Mesh(VertexShaderDesc.JustPositions(), points.Select(x => new Vertex(x)), EPrimitiveType.Points);
        }
        public static Mesh Create(IEnumerable<Vec3> points)
        {
            if (points is null)
                return null;

            return new Mesh(VertexShaderDesc.JustPositions(), points.Select(x => new Vertex(x)), EPrimitiveType.Points);
        }

        private static bool GetPrimType<T>(out EPrimitiveType type) where T : VertexPrimitive
            => PrimTypeDic.TryGetValue(typeof(T), out type);

        //public static PrimitiveData FromQuads(VertexShaderDesc info, params VertexQuad[] quads)
        //    => FromQuadList(info, quads);
        //public static PrimitiveData FromQuadList(VertexShaderDesc info, IEnumerable<VertexQuad> quads)
        //    => FromTriangleList(info, quads.SelectMany(x => x.ToTriangles()));
        //public static PrimitiveData FromTriangleStrips(VertexShaderDesc info, params VertexTriangleStrip[] strips)
        //    => FromTriangleStripList(info, strips);
        //public static PrimitiveData FromTriangleStripList(VertexShaderDesc info, IEnumerable<VertexTriangleStrip> strips)
        //    => FromTriangleList(info, strips.SelectMany(x => x.ToTriangles()));
        //public static PrimitiveData FromTriangleFans(VertexShaderDesc info, params VertexTriangleFan[] fans)
        //    => FromTriangleFanList(info, fans);
        //public static PrimitiveData FromTriangleFanList(VertexShaderDesc info, IEnumerable<VertexTriangleFan> fans)
        //    => FromTriangleList(info, fans.SelectMany(x => x.ToTriangles()));
        //public static PrimitiveData FromTriangles(VertexShaderDesc info, params VertexTriangle[] triangles)
        //    => FromTriangleList(info, triangles);
        //public static PrimitiveData FromTriangleList(VertexShaderDesc info, IEnumerable<VertexTriangle> triangles)
        //{
        //    //TODO: convert triangles to tristrips and use primitive restart to render them all in one call
        //    return new PrimitiveData(info, triangles.SelectMany(x => x.Vertices), EPrimitiveType.Triangles);
        //}
        //public static PrimitiveData FromLineStrips(VertexShaderDesc info, params VertexLineStrip[] lines)
        //    => FromLineStripList(info, lines);
        //public static PrimitiveData FromLineStripList(VertexShaderDesc info, IEnumerable<VertexLineStrip> lines)
        //    => FromLineList(info, lines.SelectMany(x => x.ToLines()));
        //public static PrimitiveData FromLines(VertexShaderDesc info, params VertexLine[] lines)
        //    => FromLineList(info, lines);
        //public static PrimitiveData FromLineList(VertexShaderDesc info, IEnumerable<VertexLine> lines)
        //    => new PrimitiveData(info, lines.SelectMany(x => x.Vertices), EPrimitiveType.Lines);
        //public static PrimitiveData FromPoints(params Vec3[] points)
        //    => FromPointList(points);
        //public static PrimitiveData FromPointList(IEnumerable<Vec3> points)
        //    => new PrimitiveData(VertexShaderDesc.JustPositions(), points.Select(x => new Vertex(x)), EPrimitiveType.Points);
        
        public Mesh() { }
        public Mesh(VertexShaderDesc info, IEnumerable<Vertex> points, EPrimitiveType type)
        {
            _bufferInfo = info;
            _type = type;
            _influences = null;

            List<Vertex> vertices = points.ToList();
            Remapper remapper = null;
            remapper = _type switch
            {
                EPrimitiveType.Triangles => SetTriangleIndices(vertices),
                EPrimitiveType.Lines => SetLineIndices(vertices),
                _ => SetPointIndices(vertices),//case EPrimitiveType.Points:
            };
            int[] firstAppearanceArray = null;
            if (remapper is null)
            {
                firstAppearanceArray = new int[vertices.Count];
                for (int i = 0; i < vertices.Count; ++i)
                    firstAppearanceArray[i] = i;
            }
            else
                firstAppearanceArray = remapper.ImplementationTable;

            CreateFacePoints(firstAppearanceArray.Length);

            if (info.HasSkinning)
                SetInfluences(firstAppearanceArray.Select(x => vertices[x].Influence).ToArray());

            for (int i = 0; i < info.MorphCount + 1; ++i)
            {
                var data = firstAppearanceArray.Select(x => vertices[x].Position).ToList();
                AddBuffer(data, new VertexAttribInfo(EBufferType.Position, i));
            }
            if (info.HasNormals)
                for (int i = 0; i < info.MorphCount + 1; ++i)
                {
                    var data = firstAppearanceArray.Select(x => vertices[x].Normal).ToList();
                    AddBuffer(data, new VertexAttribInfo(EBufferType.Normal, i));
                }
            if (info.HasBinormals)
                for (int i = 0; i < info.MorphCount + 1; ++i)
                {
                    var data = firstAppearanceArray.Select(x => vertices[x].Binormal).ToList();
                    AddBuffer(data, new VertexAttribInfo(EBufferType.Binormal, i));
                }
            if (info.HasTangents)
                for (int i = 0; i < info.MorphCount + 1; ++i)
                {
                    var data = firstAppearanceArray.Select(x => vertices[x].Tangent).ToList();
                    AddBuffer(data, new VertexAttribInfo(EBufferType.Tangent, i));
                }
            for (int i = 0; i < info.ColorCount; ++i)
            {
                var data = firstAppearanceArray.Select(x => vertices[x].Color[i]).ToList();
                AddBuffer(data, new VertexAttribInfo(EBufferType.Color, i));
            }
            for (int i = 0; i < info.TexcoordCount; ++i)
            {
                var data = firstAppearanceArray.Select(x => vertices[x].TexCoord[i]).ToList();
                AddBuffer(data, new VertexAttribInfo(EBufferType.TexCoord, i));
            }
        }
    }
    public enum ECulling
    {
        /// <summary>
        /// No faces will be invisible.
        /// </summary>
        None,
        /// <summary>
        /// Back faces will be invisible.
        /// </summary>
        Back,
        /// <summary>
        /// Front faces will be invisible.
        /// </summary>
        Front,
        /// <summary>
        /// All faces will be invisible.
        /// </summary>
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
