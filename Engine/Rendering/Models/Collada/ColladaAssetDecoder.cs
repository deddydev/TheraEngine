using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.WriteLine("Weighted: " + geo._id);

            Bone[] boneList;
            Bone bone = null;
            int boneCount;

            float weight = 0;
            float[] pWeights = null;
            Influence[] infList = new Influence[skin._weightCount];
            Matrix4* pMatrix = null;
            
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
                //else if (inp._semantic == SemanticType.INV_BIND_MATRIX)
                //{
                //    SourceEntry src = skin._sources.FirstOrDefault(x => x._id == inp._source);
                //    if (src != null)
                //        pMatrix = (Matrix4*)((DataSource)src._arrayData).Address;
                //}
            
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
            byte[] pCmd = new byte[skin._weightInputs.Count];
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
                                pWeights = src._arrayData as float[];
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
                int iCount = skin._weights[i].Length / pCmd.Length;
                Influence inf = new Influence();
                int[] iPtr = skin._weights[i];
                for (int x = 0, j = 0; x < iCount; x++)
                {
                    for (int cmd = 0; cmd < pCmd.Length; cmd++, j++)
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
                infList[i] = inf;
            }
            return DecodePrimitives(geo, bindMatrix * skin._bindMatrix, infList);
        }
        static NodeEntry RecursiveTestNode(string jointStrings, NodeEntry node)
        {
            if (jointStrings.IndexOf(node._name) >= 0 || 
                jointStrings.IndexOf(node._sid) >= 0 || 
                jointStrings.IndexOf(node._id) >= 0)
                return node;

            NodeEntry e;
            foreach (NodeEntry n in node._children)
                if ((e = RecursiveTestNode(jointStrings, n)) != null)
                    return e;

            return null;
        }

        static PrimitiveData DecodePrimitivesUnweighted(Matrix4 bindMatrix, GeometryEntry geo, bool isZup)
        {
            Debug.WriteLine("Unweighted: " + geo._id);
            return DecodePrimitives(geo, bindMatrix, null);
        }
        static PrimitiveData DecodePrimitives(GeometryEntry geo, Matrix4 bindMatrix, Influence[] infList)
        {
            SourceEntry src;
            List<VertexPrimitive> linePrimitives = null;
            List<VertexPolygon> facePrimitives = null;
            int boneCount = 0;
            if (infList != null)
            {
                HashSet<string> bones = new HashSet<string>();
                foreach (Influence inf in infList)
                    for (int i = 0; i < inf.WeightCount; ++i)
                        bones.Add(inf.Weights[i].Bone);
                boneCount = bones.Count;
            }
            PrimitiveBufferInfo info = new PrimitiveBufferInfo()
            {
                _morphCount = 0,
                _texcoordCount = 0,
                _hasNormals = false,
                _hasBinormals = false,
                _hasTangents = false,
                _boneCount = boneCount,
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
                    info._morphCount = sources[SemanticType.VERTEX].Count - 1;
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
                    info._morphCount + 1,
                    info._colorCount,
                    info._texcoordCount);
                
                int stride, index, startIndex;
                Vertex vtx;
                Vertex[][] vertices;
                float[] list;
                Matrix4 invBind = bindMatrix;
                if (info.HasNormals || info.HasBinormals || info.HasTangents)
                {
                    invBind.Invert();
                    invBind.Transpose();
                    invBind = invBind.GetRotationMatrix4();
                }
                foreach (PrimitiveFace f in prim._faces)
                {
                    vertices = new Vertex[f._pointCount][];
                    foreach (InputEntry inp in prim._inputs)
                    {
                        src = sources[inp._semantic][inp._set];
                        stride = src._accessorStride;
                        list = src._arrayData as float[];
                        for (int i = 0, x = 0; i < f._pointCount; ++i, x += prim._inputs.Count)
                        {
                            if (vertices[i] == null)
                                vertices[i] = new Vertex[maxSets];
                            
                            startIndex = (index = f._pointIndices[x + inp._offset]) * stride;

                            vtx = vertices[i][inp._set];
                            if (vtx == null)
                                vtx = new Vertex();

                            switch (inp._semantic)
                            {
                                case SemanticType.VERTEX:
                                    Vec3 position = new Vec3(list[startIndex], list[startIndex + 1], list[startIndex + 2]);
                                    position = position * bindMatrix;
                                    vtx._position = position;
                                    if (infList != null)
                                        vtx._influence = infList[index];
                                    break;
                                case SemanticType.NORMAL:
                                    Vec3 normal = new Vec3(list[startIndex], list[startIndex + 1], list[startIndex + 2]);
                                    normal = normal * invBind;
                                    normal.Normalize();
                                    vtx._normal = normal;
                                    break;
                                case SemanticType.TEXBINORMAL:
                                    Vec3 binormal = new Vec3(list[startIndex], list[startIndex + 1], list[startIndex + 2]);
                                    binormal = binormal * invBind;
                                    binormal.Normalize();
                                    vtx._binormal = binormal;
                                    break;
                                case SemanticType.TEXTANGENT:
                                    Vec3 tangent = new Vec3(list[startIndex], list[startIndex + 1], list[startIndex + 2]);
                                    tangent = tangent * invBind;
                                    tangent.Normalize();
                                    vtx._tangent = tangent;
                                    break;
                                case SemanticType.TEXCOORD:
                                    vtx._texCoord = new Vec2(list[startIndex], list[startIndex + 1]);
                                    vtx._texCoord.Y = 1.0f - vtx._texCoord.Y;
                                    //vtx._texCoord.X = vtx._texCoord.X.RemapToRange(0.0f, 1.0f);
                                    //vtx._texCoord.Y = vtx._texCoord.Y.RemapToRange(0.0f, 1.0f);
                                    break;
                                case SemanticType.COLOR:
                                    vtx._color = new ColorF4(list[startIndex], list[startIndex + 1], list[startIndex + 2], list[startIndex + 3]);
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

            return PrimitiveData.FromTriangleList(Culling.None, info, triangles);
        }
    }
}
