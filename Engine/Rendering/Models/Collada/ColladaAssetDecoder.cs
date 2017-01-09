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
            SceneEntry scene)
        {
            PrimitiveData data = DecodePrimitives(geo);
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
                vertList.Add(new Vertex(bindMatrix * pVert[i], inf));
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

        static PrimitiveData DecodePrimitivesUnweighted(Matrix4 bindMatrix, GeometryEntry geo)
        {
            PrimitiveData manager = DecodePrimitives(geo);

            Vec3* pV = null;
            int vCount = 0;
            
            SourceEntry s = geo._sources.FirstOrDefault(x => x._id == geo._verticesInput._source);
            if (s != null)
            {
                DataSource b = s._arrayData as DataSource;
                pV = (Vec3*)b.Address;
                vCount = b.Length / 12;
            }

            Vertex[] vertices = new Vertex[vCount];
            for (int i = 0; i < vCount; i++)
                vertices[i] = new Vertex(bindMatrix * pV[i]);

            return manager;
        }
        static PrimitiveData DecodePrimitives(GeometryEntry geo)
        {
            SourceEntry src;
            //int triangleCount = 0, lineCount = 0, pointCount = 0;
 
            List<VertexPrimitive> linePrimitives = null;
            List<VertexPolygon> facePrimitives = null;
            PrimitiveBufferInfo info = new PrimitiveBufferInfo()
            {
                _positionCount = 0,
                _normalCount = 0,
                _texcoordCount = 0
            };

            foreach (PrimitiveEntry prim in geo._primitives)
            {
                ////Get face/line count
                //if (prim._type == ColladaPrimitiveType.lines || 
                //    prim._type == ColladaPrimitiveType.linestrips)
                //    lineCount += prim._faceCount;
                //else
                //    triangleCount += prim._faceCount;

                ////Get point total
                //pointCount += prim._pointCount;

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
                    info._positionCount = sources[SemanticType.VERTEX].Count;
                if (sources.ContainsKey(SemanticType.NORMAL))
                    info._normalCount = sources[SemanticType.NORMAL].Count;
                if (sources.ContainsKey(SemanticType.COLOR))
                    info._colorCount = sources[SemanticType.COLOR].Count;
                if (sources.ContainsKey(SemanticType.TEXCOORD))
                    info._texcoordCount = sources[SemanticType.TEXCOORD].Count;
                if (sources.ContainsKey(SemanticType.TEXBINORMAL))
                    info._binormalCount = sources[SemanticType.TEXBINORMAL].Count;
                if (sources.ContainsKey(SemanticType.TEXTANGENT))
                    info._tangentCount = sources[SemanticType.TEXTANGENT].Count;

                int maxSets = CustomMath.Max(
                    info._positionCount, 
                    info._normalCount, 
                    info._colorCount, 
                    info._texcoordCount, 
                    info._binormalCount,
                    info._tangentCount);

                int stride;
                Vertex vtx;
                VoidPtr addr;
                //int indexStride;
                Vertex[][] vertices;
                foreach (PrimitiveFace f in prim._faces)
                {
                    //indexStride = f._pointCount / f._faceCount;
                    vertices = new Vertex[f._pointCount][];
                    foreach (InputEntry inp in prim._inputs)
                    {
                        src = sources[inp._semantic][inp._set];
                        stride = src._accessorStride * 4;
                        for (int i = 0; i < f._pointCount; ++i)
                        {
                            if (vertices[i] == null)
                                vertices[i] = new Vertex[maxSets];
                            
                            int index = f._pointIndices[i * sources.Count + inp._offset];
                            addr = ((DataSource)src._arrayData).Address[index, stride];
                            vtx = vertices[i][inp._set];
                            if (vtx == null)
                                vtx = vertices[i][inp._set] = new Vertex();
                            switch (inp._semantic)
                            {
                                case SemanticType.VERTEX:
                                    vtx._position = *(Vec3*)addr;
                                    break;
                                case SemanticType.NORMAL:
                                    vtx._normal = *(Vec3*)addr;
                                    break;
                                case SemanticType.TEXCOORD:
                                    vtx._texCoord = *(Vec2*)addr;
                                    break;
                                case SemanticType.COLOR:
                                    vtx._color = *(ColorF4*)addr;
                                    break;
                                case SemanticType.TEXTANGENT:
                                    vtx._tangent = *(Vec3*)addr;
                                    break;
                                case SemanticType.TEXBINORMAL:
                                    vtx._binormal = *(Vec3*)addr;
                                    break;
                            }
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

            //int[] triangleIndices, lineIndices;

            ////Create primitives
            //if (triangleCount > 0)
            //    triangleIndices = new int[triangleCount * 3];
            //if (lineCount > 0)
            //    lineIndices = new int[lineCount * 2];

            //    //Process point indices
            //    switch (prim._type)
            //    {
            //        case ColladaPrimitiveType.triangles:
            //            count = prim._faceCount * 3;
            //            while (count-- > 0)
            //                pTriarr[pTri++] = fIndex++;
            //            break;
            //        case ColladaPrimitiveType.trifans:
            //        case ColladaPrimitiveType.polygons:
            //        case ColladaPrimitiveType.polylist:
            //            foreach (PrimitiveFace f in prim._faces)
            //            {
            //                count = f._pointCount - 2;
            //                temp = fIndex;
            //                fIndex += 2;
            //                while (count-- > 0)
            //                {
            //                    pTriarr[pTri++] = temp;
            //                    pTriarr[pTri++] = fIndex - 1;
            //                    pTriarr[pTri++] = fIndex++;
            //                }
            //            }
            //            break;
            //        case ColladaPrimitiveType.tristrips:
            //            foreach (PrimitiveFace f in prim._faces)
            //            {
            //                count = f._pointCount;
            //                fIndex += 2;
            //                for (int i = 2; i < count; i++)
            //                {
            //                    if ((i & 1) == 0)
            //                    {
            //                        pTriarr[pTri++] = fIndex - 2;
            //                        pTriarr[pTri++] = fIndex - 1;
            //                        pTriarr[pTri++] = fIndex++;
            //                    }
            //                    else
            //                    {
            //                        pTriarr[pTri++] = fIndex - 2;
            //                        pTriarr[pTri++] = fIndex;
            //                        pTriarr[pTri++] = fIndex++ - 1;
            //                    }
            //                }
            //            }
            //            break;

            //        case ColladaPrimitiveType.linestrips:
            //            foreach (PrimitiveFace f in prim._faces)
            //            {
            //                count = f._pointCount - 1;
            //                lIndex++;
            //                while (count-- > 0)
            //                {
            //                    pLinarr[pLin++] = lIndex - 1;
            //                    pLinarr[pLin++] = lIndex++;
            //                }
            //            }
            //            break;

            //        case ColladaPrimitiveType.lines:
            //            foreach (PrimitiveFace f in prim._faces)
            //            {
            //                count = f._pointCount;
            //                while (count-- > 0)
            //                    pLinarr[pLin++] = lIndex++;
            //            }
            //            break;
            //    }
            //}

            List<VertexTriangle> triangles = new List<VertexTriangle>();
            foreach (VertexPolygon p in facePrimitives)
                triangles.AddRange(p.ToTriangles());

            return PrimitiveData.FromTriangleList(Culling.Back, info, triangles);
        }
    }
}
