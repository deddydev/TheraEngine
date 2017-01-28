using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CustomEngine.Rendering.Models.Collada
{
    public unsafe partial class Collada
    {
        static PrimitiveData DecodePrimitivesWeighted(
            Matrix4 bindMatrix,
            GeometryEntry geo,
            SkinEntry skin,
            SceneEntry scene,
            bool isZup)
        {
            PrimitiveData data = DecodePrimitives(geo, isZup, bindMatrix);
            bindMatrix = bindMatrix * skin._bindMatrix;

            Bone[] boneList;
            Bone bone = null;
            int boneCount;

            float weight = 0;
            float* pWeights = null;
            Vec3* pVert = null;
            List<Vertex> vertList = new List<Vertex>(skin._weightCount);
            Matrix4* pMatrix = null;

            DataSource remap = new DataSource(skin._weightCount * 2);
            ushort* pRemap = (ushort*)remap.Address;

            //Find vertex source
            SourceEntry s = geo._sources.FirstOrDefault(x => x._id == geo._verticesInput._source);
            if (s != null)
                pVert = (Vec3*)((DataSource)s._arrayData).Address;
            
            //Find joint source
            string[] jointStringArray = null;
            string jointString = null;
            foreach (InputEntry inp in skin._jointInputs)
                if (inp._semantic == SemanticType.JOINT)
                {
                    SourceEntry src = skin._sources.FirstOrDefault(x => x._id == inp._source);
                    if (src != null)
                    {
                        jointStringArray = src._arrayData as string[];
                        jointString = src._arrayDataString;
                    }
                }
                else if (inp._semantic == SemanticType.INV_BIND_MATRIX)
                {
                    SourceEntry src = skin._sources.FirstOrDefault(x => x._id == inp._source);
                    if (src != null)
                        pMatrix = (Matrix4*)((DataSource)src._arrayData).Address;
                }
            
            //Populate bone list
            boneCount = jointStringArray.Length;
            boneList = new Bone[boneCount];
            for (int i = 0; i < boneCount; i++)
            {
                NodeEntry entry = scene.FindNode(jointStringArray[i]);
                if (entry != null && entry._node != null)
                    boneList[i] = entry._node as Bone;
                else
                {
                    //Search in reverse!
                    foreach (NodeEntry node in scene._nodes)
                    {
                        if ((entry = RecursiveTestNode(jointString, node)) != null)
                        {
                            if (entry._node != null)
                                boneList[i] = entry._node as Bone;
                            break;
                        }
                    }

                    //Couldn't find the bone
                    if (boneList[i] == null)
                        boneList[i] = new Bone();
                }
            }

            //Build command list
            int cmdCount = skin._weightInputs.Count;
            byte* pCmd = stackalloc byte[4];
            foreach (InputEntry inp in skin._weightInputs)
            {
                switch (inp._semantic)
                {
                    case SemanticType.JOINT:
                        pCmd[inp._offset] = 1;
                        break;

                    case SemanticType.WEIGHT:
                        pCmd[inp._offset] = 2;

                        //Get weight source
                        foreach (SourceEntry src in skin._sources)
                            if (src._id == inp._source)
                            {
                                pWeights = (float*)((DataSource)src._arrayData).Address;
                                break;
                            }

                        break;

                    default:
                        pCmd[inp._offset] = 0;
                        break;
                }
            }
            
            //Build vertex list and remap table
            for (int i = 0; i < skin._weightCount; i++)
            {
                //Create influence
                int iCount = skin._weights[i].Length / cmdCount;
                Influence inf = new Influence();
                int[] iPtr = skin._weights[i];
                int j = 0;
                for (int x = 0; x < iCount; x++)
                {
                    for (int cmd = 0; cmd < cmdCount; cmd++, j++)
                    {
                        int index = iPtr[j];
                        switch (pCmd[cmd])
                        {
                            case 1: //JOINT
                                bone = boneList[index];
                                break;
                            case 2: //WEIGHT
                                weight = pWeights[index];
                                break;
                        }
                    }
                    inf.AddWeight(new BoneWeight(bone.Name, weight));
                }
                vertList.Add(new Vertex(bindMatrix * (Vec3)pVert[i], inf));
            }
            return data;
        }
        static NodeEntry RecursiveTestNode(string jointStrings, NodeEntry node)
        {
            if (jointStrings.IndexOf(node._name) >= 0)
                return node;
            else if (jointStrings.IndexOf(node._sid) >= 0)
                return node;
            else if (jointStrings.IndexOf(node._id) >= 0)
                return node;

            NodeEntry e;
            foreach (NodeEntry n in node._children)
                if ((e = RecursiveTestNode(jointStrings, n)) != null)
                    return e;

            return null;
        }

        static PrimitiveData DecodePrimitivesUnweighted(Matrix4 bindMatrix, GeometryEntry geo, bool isZup)
        {
            PrimitiveData manager = DecodePrimitives(geo, isZup, bindMatrix);

            //Vec3* pV = null;
            //int vCount = 0;
            
            //SourceEntry s = geo._sources.FirstOrDefault(x => x._id == geo._verticesInput._source);
            //if (s != null)
            //{
            //    DataSource b = s._arrayData as DataSource;
            //    pV = (Vec3*)b.Address;
            //    vCount = b.Length / 12;
            //}

            //Vertex[] vertices = new Vertex[vCount];
            //for (int i = 0; i < vCount; i++)
            //    vertices[i] = new Vertex(bindMatrix * pV[i]);

            return manager;
        }
        static PrimitiveData DecodePrimitives(GeometryEntry geo, bool isZup, Matrix4 bindMatrix)
        {
            SourceEntry src;
            List<VertexPrimitive> linePrimitives = null;
            List<VertexPolygon> facePrimitives = null;
            PrimitiveBufferInfo info = new PrimitiveBufferInfo()
            {
                _pnbtCount = 0,
                _texcoordCount = 0,
                _hasNormals = false,
                _hasBinormals = false,
                _hasTangents = false,
            };

            foreach (PrimitiveEntry prim in geo._primitives)
            {
                Dictionary<SemanticType, Dictionary<int, SourceEntry>> sources = new Dictionary<SemanticType, Dictionary<int, SourceEntry>>();
                src = geo._sources.FirstOrDefault(x => x._id == geo._verticesInput._source);
                if (src != null)
                    sources.Add(SemanticType.VERTEX, new Dictionary<int, SourceEntry>() { { 0, src } });

                //Collect sources
                foreach (InputEntry inp in prim._inputs)
                {
                    if (inp._semantic == SemanticType.VERTEX)
                        continue;

                    src = geo._sources.FirstOrDefault(x => x._id == inp._source);
                    if (src != null)
                    {
                        if (!sources.ContainsKey(inp._semantic))
                            sources.Add(inp._semantic, new Dictionary<int, SourceEntry>());

                        if (!sources[inp._semantic].ContainsKey(inp._set))
                            sources[inp._semantic].Add(inp._set, src);
                        else
                            sources[inp._semantic][inp._set] = src;
                    }
                }

                if (sources.ContainsKey(SemanticType.VERTEX))
                    info._pnbtCount = sources[SemanticType.VERTEX].Count;
                if (sources.ContainsKey(SemanticType.NORMAL))
                    info._hasNormals = sources[SemanticType.NORMAL].Count > 0;
                if (sources.ContainsKey(SemanticType.COLOR))
                    info._colorCount = sources[SemanticType.COLOR].Count;
                if (sources.ContainsKey(SemanticType.TEXCOORD))
                    info._texcoordCount = sources[SemanticType.TEXCOORD].Count;
                if (sources.ContainsKey(SemanticType.TEXBINORMAL))
                    info._hasBinormals = sources[SemanticType.TEXBINORMAL].Count > 0;
                if (sources.ContainsKey(SemanticType.TEXTANGENT))
                    info._hasTangents = sources[SemanticType.TEXTANGENT].Count > 0;

                int maxSets = CustomMath.Max(
                    info._pnbtCount,
                    info._colorCount,
                    info._texcoordCount);

                int stride;
                Vertex vtx;
                VoidPtr addr;
                Vertex[][] vertices;
                foreach (PrimitiveFace f in prim._faces)
                {
                    vertices = new Vertex[f._pointCount][];
                    foreach (InputEntry inp in prim._inputs)
                    {
                        src = sources[inp._semantic][inp._set];
                        stride = src._accessorStride * 4;
                        for (int i = 0; i < f._pointCount; ++i)
                        {
                            if (vertices[i] == null)
                                vertices[i] = new Vertex[maxSets];
                            
                            int index = f._pointIndices[i * prim._inputs.Count + inp._offset];
                            addr = ((DataSource)src._arrayData).Address[index, stride];
                            vtx = vertices[i][inp._set];
                            if (vtx == null)
                                vtx = new Vertex();
                            switch (inp._semantic)
                            {
                                case SemanticType.VERTEX:
                                    Vec3 position = *(Vec3*)addr;
                                    position = Vec3.TransformPosition(position, bindMatrix);
                                    if (isZup)
                                        position.ChangeZupToYup();
                                    vtx._position = position;
                                    break;
                                case SemanticType.NORMAL:
                                    Vec3 normal = *(Vec3*)addr;
                                    normal = Vec3.TransformNormal(normal, bindMatrix);
                                    if (isZup)
                                        normal.ChangeZupToYup();
                                    normal.Normalize();
                                    vtx._normal = normal;
                                    break;
                                case SemanticType.TEXBINORMAL:
                                    Vec3 binormal = *(Vec3*)addr;
                                    binormal = Vec3.TransformNormal(binormal, bindMatrix);
                                    if (isZup)
                                        binormal.ChangeZupToYup();
                                    binormal.Normalize();
                                    vtx._binormal = binormal;
                                    break;
                                case SemanticType.TEXTANGENT:
                                    Vec3 tangent = *(Vec3*)addr;
                                    tangent = Vec3.TransformNormal(tangent, bindMatrix);
                                    if (isZup)
                                        tangent.ChangeZupToYup();
                                    tangent.Normalize();
                                    vtx._tangent = tangent;
                                    break;
                                case SemanticType.TEXCOORD:
                                    vtx._texCoord = *(Vec2*)addr;
                                    vtx._texCoord.Y = 1.0f - vtx._texCoord.Y;
                                    break;
                                case SemanticType.COLOR:
                                    vtx._color = *(ColorF4*)addr;
                                    break;
                            }
                            vertices[i][inp._set] = vtx;
                        }
                    }
                    int setIndex = 0;
                    switch (prim._type)
                    {
                        case ColladaPrimitiveType.lines:
                            if (linePrimitives == null)
                                linePrimitives = new List<VertexPrimitive>();
                            VertexLine[] lines = new VertexLine[vertices.Length / 2];

                            for (int i = 0, x = 0; i < vertices.Length; i += 2, ++x)
                                lines[x] = new VertexLine(vertices[i][setIndex], vertices[i + 1][setIndex]);

                            linePrimitives.AddRange(lines);
                            break;
                        case ColladaPrimitiveType.linestrips:
                            if (linePrimitives == null)
                                linePrimitives = new List<VertexPrimitive>();

                            linePrimitives.Add(new VertexLineStrip(false, vertices.Select(x => x[setIndex]).ToArray()));

                            break;
                        case ColladaPrimitiveType.triangles:
                            if (facePrimitives == null)
                                facePrimitives = new List<VertexPolygon>();
                            VertexTriangle[] tris = new VertexTriangle[vertices.Length / 3];

                            for (int i = 0, x = 0; i < vertices.Length; i += 3, ++x)
                                tris[x] = new VertexTriangle(
                                    vertices[i][setIndex],
                                    vertices[i + 1][setIndex],
                                    vertices[i + 2][setIndex]);

                            facePrimitives.AddRange(tris);
                            break;
                        case ColladaPrimitiveType.trifans:
                            if (facePrimitives == null)
                                facePrimitives = new List<VertexPolygon>();

                            facePrimitives.Add(new VertexTriangleFan(vertices.Select(x => x[setIndex]).ToArray()));

                            break;
                        case ColladaPrimitiveType.tristrips:
                            if (facePrimitives == null)
                                facePrimitives = new List<VertexPolygon>();

                            facePrimitives.Add(new VertexTriangleStrip(vertices.Select(x => x[setIndex]).ToArray()));

                            break;
                    }
                }
            }
            
            List<VertexTriangle> triangles = new List<VertexTriangle>();
            foreach (VertexPolygon p in facePrimitives)
                triangles.AddRange(p.ToTriangles());

            return PrimitiveData.FromTriangleList(Culling.Back, info, triangles);
        }
    }
}
