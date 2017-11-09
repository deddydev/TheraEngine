using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryControllers.Controller;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryGeometries;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryGeometries.Geometry.Mesh;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryVisualScenes;
using static TheraEngine.Rendering.Models.Collada.Source;

namespace TheraEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        static PrimitiveData DecodePrimitivesWeighted(
            VisualScene scene,
            Matrix4 bindMatrix,
            Geometry geo,
            ControllerChild rig)
        {
            if (rig is Skin skin)
            {
                Bone[] boneList;
                Bone bone = null;
                int boneCount;

                var bindShapeMatrix = skin.BindShapeMatrixElement?.StringContent?.Value ?? Matrix4.Identity;
                var joints = skin.JointsElement;
                var influences = skin.VertexWeightsElement;
                var boneCounts = influences.BoneCountsElement;
                var prims = influences.PrimitiveIndicesElement;

                Influence[] infList = new Influence[influences.Count];
                
                //Find joint source
                string[] jointSIDs = null;
                foreach (InputUnshared inp in joints.GetChildren<InputUnshared>())
                    if (inp.CommonSemanticType == ESemantic.JOINT && inp.Source.GetElement(inp.Root) is Source src)
                    {
                        jointSIDs = src.GetChild<NameArray>().StringContent.Values;
                        break;
                    }

                if (jointSIDs == null)
                    return null;

                //Populate bone list
                boneCount = jointSIDs.Length;
                boneList = new Bone[boneCount];
                for (int i = 0; i < boneCount; i++)
                {
                    string sid = jointSIDs[i];
                    var node = scene.FindNode(sid);
                    
                    if (node != null && node.UserData is Bone b)
                        boneList[i] = b;
                    else
                        Engine.LogError(string.Format("Bone '{0}' not found", sid));
                }

                //Build input command list
                float[] pWeights = null;
                byte[] pCmd = new byte[influences.InputElements.Length];
                foreach (InputShared inp in influences.InputElements)
                {
                    switch (inp.CommonSemanticType)
                    {
                        case ESemantic.JOINT:
                            pCmd[inp.Offset] = 1;
                            break;

                        case ESemantic.WEIGHT:
                            pCmd[inp.Offset] = 2;

                            Source src = inp.Source.GetElement<Source>(inp.Root);
                            pWeights = src.GetArrayElement<FloatArray>().StringContent.Values;

                            break;

                        default:
                            pCmd[inp.Offset] = 0;
                            break;
                    }
                }
                
                float weight = 0;
                int[] boneIndices = boneCounts.StringContent.Values;
                int[] primIndices = prims.StringContent.Values;
                for (int i = 0, primIndex = 0; i < influences.Count; i++)
                {
                    Influence inf = new Influence();
                    for (int boneIndex = 0; boneIndex < boneIndices[i]; boneIndex++)
                    {
                        for (int cmd = 0; cmd < pCmd.Length; cmd++, primIndex++)
                        {
                            int index = primIndices[primIndex];
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
                    inf.Normalize();
                    infList[i] = inf;
                }
                return DecodePrimitives(geo, bindMatrix * bindShapeMatrix, infList);
            }
            else
            {
                return null;
            }
        }
        
        static PrimitiveData DecodePrimitivesUnweighted(Matrix4 bindMatrix, Geometry geo)
        {
            return DecodePrimitives(geo, bindMatrix, null);
        }
        static PrimitiveData DecodePrimitives(Geometry geo, Matrix4 bindMatrix, Influence[] infList)
        {
            Source src;
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
            VertexShaderDesc info = VertexShaderDesc.JustPositions();
            info._boneCount = boneCount;

            var m = geo.MeshElement;
            if (m == null)
                return null;
            
            foreach (var prim in m.PrimitiveElements)
            {
                Dictionary<ESemantic, Dictionary<int, Source>> sources = new Dictionary<ESemantic, Dictionary<int, Source>>();
                foreach (InputShared inp in prim.InputElements)
                {
                    ESemantic semantic = inp.CommonSemanticType;
                    if (inp.CommonSemanticType == ESemantic.VERTEX)
                    {
                        var verts = inp.Source.GetElement<Vertices>(inp.Root);
                        sources.Add(ESemantic.VERTEX, new Dictionary<int, Source>());
                        int i = 0;
                        foreach (var input in verts.InputElements)
                        {
                            src = input.Source.GetElement<Source>(verts.Root);
                            sources[ESemantic.VERTEX].Add(i++, src);
                        }
                        continue;
                    }
                    else
                    {
                        src = inp.Source.GetElement<Source>(inp.Root);
                        if (src != null)
                        {
                            if (!sources.ContainsKey(semantic))
                                sources.Add(semantic, new Dictionary<int, Source>());

                            int set = (int)inp.Set;
                            if (!sources[semantic].ContainsKey(set))
                                sources[semantic].Add(set, src);
                            else
                                sources[semantic][set] = src;
                        }
                    }
                }

                if (sources.ContainsKey(ESemantic.VERTEX))
                    info._morphCount = sources[ESemantic.VERTEX].Count - 1;
                else
                    Engine.LogError("Mesh has no vertices.");

                info._hasNormals =
                    sources.ContainsKey(ESemantic.NORMAL) &&
                    sources[ESemantic.NORMAL].Count > 0;
                info._hasBinormals =
                    sources.ContainsKey(ESemantic.TEXBINORMAL) &&
                    sources[ESemantic.TEXBINORMAL].Count > 0;
                info._hasTangents =
                    sources.ContainsKey(ESemantic.TEXTANGENT) &&
                    sources[ESemantic.TEXTANGENT].Count > 0;

                info._colorCount =
                    sources.ContainsKey(ESemantic.COLOR) ? 
                    sources[ESemantic.COLOR].Count : 0;
                info._texcoordCount =
                    sources.ContainsKey(ESemantic.TEXCOORD) ?
                    sources[ESemantic.TEXCOORD].Count : 0;
                
                int maxSets = CustomMath.Max(
                    info._morphCount + 1,
                    info._colorCount,
                    info._texcoordCount);
                
                int stride, pointIndex, startIndex;
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

                TechniqueCommon.Accessor acc;
                var indices = prim.IndicesElement.StringContent.Values;

                vertices = new Vertex[prim.PointCount][];
                foreach (var inp in prim.InputElements)
                {
                    int set = (int)inp.Set;
                    int offset = (int)inp.Offset;

                    src = sources[inp.CommonSemanticType][set];
                    acc = src.TechniqueCommonElement.AccessorElement;
                    stride = (int)acc.Stride;

                    list = src.GetArrayElement<FloatArray>().StringContent.Values;
                    for (int i = 0, x = 0; i < prim.PointCount; ++i, x += prim.InputElements.Length)
                    {
                        if (vertices[i] == null)
                            vertices[i] = new Vertex[maxSets];

                        startIndex = (pointIndex = indices[x + inp.Offset]) * stride;

                        vtx = vertices[i][set];
                        if (vtx == null)
                            vtx = new Vertex();

                        switch (inp.CommonSemanticType)
                        {
                            case ESemantic.VERTEX:
                                Vec3 position = new Vec3(list[startIndex], list[startIndex + 1], list[startIndex + 2]);
                                position = Vec3.TransformPosition(position, bindMatrix);
                                vtx._position = position;
                                if (infList != null)
                                    vtx._influence = infList[pointIndex];
                                break;
                            case ESemantic.NORMAL:
                                Vec3 normal = new Vec3(list[startIndex], list[startIndex + 1], list[startIndex + 2]);
                                normal = normal * invBind;
                                normal.Normalize();
                                vtx._normal = normal;
                                break;
                            case ESemantic.TEXBINORMAL:
                                Vec3 binormal = new Vec3(list[startIndex], list[startIndex + 1], list[startIndex + 2]);
                                binormal = binormal * invBind;
                                binormal.Normalize();
                                vtx._binormal = binormal;
                                break;
                            case ESemantic.TEXTANGENT:
                                Vec3 tangent = new Vec3(list[startIndex], list[startIndex + 1], list[startIndex + 2]);
                                tangent = tangent * invBind;
                                tangent.Normalize();
                                vtx._tangent = tangent;
                                break;
                            case ESemantic.TEXCOORD:
                                vtx._texCoord = new Vec2(list[startIndex], list[startIndex + 1]);
                                vtx._texCoord.Y = 1.0f - vtx._texCoord.Y;
                                //vtx._texCoord.X = vtx._texCoord.X.RemapToRange(0.0f, 1.0f);
                                //vtx._texCoord.Y = vtx._texCoord.Y.RemapToRange(0.0f, 1.0f);
                                break;
                            case ESemantic.COLOR:
                                vtx._color = new ColorF4(list[startIndex], list[startIndex + 1], list[startIndex + 2], list[startIndex + 3]);
                                break;
                        }
                        vertices[i][set] = vtx;
                    }
                }
                int setIndex = 0;
                switch (prim.Type)
                {
                    case EColladaPrimitiveType.Lines:
                        if (linePrimitives == null)
                            linePrimitives = new List<VertexPrimitive>();
                        VertexLine[] lines = new VertexLine[vertices.Length / 2];

                        for (int i = 0, x = 0; i < vertices.Length; i += 2, ++x)
                            lines[x] = new VertexLine(vertices[i][setIndex], vertices[i + 1][setIndex]);

                        linePrimitives.AddRange(lines);
                        break;
                    case EColladaPrimitiveType.Linestrips:
                        if (linePrimitives == null)
                            linePrimitives = new List<VertexPrimitive>();

                        linePrimitives.Add(new VertexLineStrip(false, vertices.Select(x => x[setIndex]).ToArray()));

                        break;
                    case EColladaPrimitiveType.Triangles:
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
                    case EColladaPrimitiveType.Trifans:
                        if (facePrimitives == null)
                            facePrimitives = new List<VertexPolygon>();

                        facePrimitives.Add(new VertexTriangleFan(vertices.Select(x => x[setIndex]).ToArray()));

                        break;
                    case EColladaPrimitiveType.Tristrips:
                        if (facePrimitives == null)
                            facePrimitives = new List<VertexPolygon>();

                        facePrimitives.Add(new VertexTriangleStrip(vertices.Select(x => x[setIndex]).ToArray()));

                        break;
                }
            }
            
            List<VertexTriangle> triangles = new List<VertexTriangle>();
            foreach (VertexPolygon p in facePrimitives)
                triangles.AddRange(p.ToTriangles());

            return PrimitiveData.FromTriangleList(Culling.None, info, triangles);
        }
    }
}
