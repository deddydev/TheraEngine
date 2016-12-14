using System;
using System.Collections.Generic;
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
            Vec3* pVert = null, pNorms = null;
            List<Vertex> vertList = new List<Vertex>(skin._weightCount);
            Matrix4* pMatrix = null;

            DataSource remap = new DataSource(skin._weightCount * 2);
            ushort* pRemap = (ushort*)remap.Address;

            //Find vertex source
            foreach (SourceEntry s in geo._sources)
                if (s._id == geo._verticesInput._source)
                {
                    pVert = (Vec3*)((DataSource)s._arrayData).Address;
                    break;
                }

            //Find joint source
            string[] jointStringArray = null;
            string jointString = null;
            foreach (InputEntry inp in skin._jointInputs)
                if (inp._semantic == SemanticType.JOINT)
                {
                    foreach (SourceEntry src in skin._sources)
                        if (src._id == inp._source)
                        {
                            jointStringArray = src._arrayData as string[];
                            jointString = src._arrayDataString;
                            break;
                        }
                }
                else if (inp._semantic == SemanticType.INV_BIND_MATRIX)
                {
                    foreach (SourceEntry src in skin._sources)
                        if (src._id == inp._source)
                        {
                            pMatrix = (Matrix4*)((DataSource)src._arrayData).Address;
                            break;
                        }
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

            Vec3* pVert = null, pNorms = null;
            int vCount = 0;

            //Find vertex source
            foreach (SourceEntry s in geo._sources)
                if (s._id == geo._verticesInput._source)
                {
                    DataSource b = s._arrayData as DataSource;
                    pVert = (Vec3*)b.Address;
                    vCount = b.Length / 12;
                    break;
                }

            Vertex[] vertices = new Vertex[vCount];
            for (int i = 0; i < vCount; i++)
                vertices[i] = new Vertex(bindMatrix * pVert[i]);

            //Remap vertex indices and fix normals
            for (int i = 0; i < manager._pointCount; i++, pVInd++)
            {
                *pVInd = pRemap[*pVInd];

                if (pNorms != null)
                    pNorms[i] = bindMatrix.GetRotationMatrix() * pNorms[i];
            }

            return manager;
        }

        static PrimitiveData DecodePrimitives(GeometryEntry geo)
        {
            uint[] pTriarr = null, pLinarr = null;
            uint pTri = 0, pLin = 0;
            long* pInDataList = stackalloc long[12];
            long* pOutDataList = stackalloc long[12];
            int* pData = stackalloc int[16];
            int faces = 0, lines = 0, points = 0;
            uint fIndex = 0, lIndex = 0, temp;

            PrimitiveDecodeCommand* pCmd = (PrimitiveDecodeCommand*)pData;
            byte** pInData = (byte**)pInDataList;
            byte** pOutData = (byte**)pOutDataList;

            PrimitiveManager manager = new PrimitiveManager();

            //Assign vertex source
            foreach (SourceEntry s in geo._sources)
                if (s._id == geo._verticesInput._source)
                {
                    pInData[0] = (byte*)((DataSource)s._arrayData).Address;
                    break;
                }

            foreach (PrimitiveEntry prim in geo._primitives)
            {
                //Get face/line count
                if (prim._type == ColladaPrimitiveType.lines || prim._type == ColladaPrimitiveType.linestrips)
                    lines += prim._faceCount;
                else
                    faces += prim._faceCount;

                //Get point total
                points += prim._pointCount;

                //Signal storage buffers and set type offsets
                foreach (InputEntry inp in prim._inputs)
                {
                    int offset = -1;

                    switch (inp._semantic)
                    {
                        case SemanticType.VERTEX:
                            offset = 0;
                            break;
                        case SemanticType.NORMAL:
                            offset = 1;
                            break;
                        case SemanticType.COLOR:
                            if (inp._set < 2)
                                offset = 2 + inp._set;
                            break;
                        case SemanticType.TEXCOORD:
                            if (inp._set < 8)
                                offset = 4 + inp._set;
                            break;
                    }
                    inp._outputOffset = offset;
                }
            }
            manager._pointCount = points;

            //Create primitives
            if (faces > 0)
            {
                manager._triangles = new GLPrimitive(faces * 3, OpenTK.Graphics.OpenGL.BeginMode.Triangles);
                pTriarr = manager._triangles._indices;
            }
            if (lines > 0)
            {
                manager._lines = new GLPrimitive(lines * 2, OpenTK.Graphics.OpenGL.BeginMode.Lines);
                pLinarr = manager._lines._indices;
            }

            manager._indices = new UnsafeBuffer(points * 2);
            //Create face buffers and assign output pointers
            for (int i = 0; i < 12; i++)
                if (manager._dirty[i])
                {
                    int stride;
                    if (i == 0)
                        stride = 2;
                    else if (i == 1)
                        stride = 12;
                    else if (i < 4)
                        stride = 4;
                    else
                        stride = 8;
                    manager._faceData[i] = new UnsafeBuffer(points * stride);
                    if (i == 0)
                        pOutData[i] = (byte*)manager._indices.Address;
                    else
                        pOutData[i] = (byte*)manager._faceData[i].Address;
                }

            //Decode primitives
            foreach (PrimitiveEntry prim in geo._primitives)
            {
                int count = prim._inputs.Count;
                //Map inputs to command sequence
                foreach (InputEntry inp in prim._inputs)
                {
                    if (inp._outputOffset == -1)
                        pCmd[inp._offset].Cmd = 0;
                    else
                    {
                        pCmd[inp._offset].Cmd = (byte)inp._semantic;
                        pCmd[inp._offset].Index = (byte)inp._outputOffset;

                        //Assign input buffer
                        foreach (SourceEntry src in geo._sources)
                            if (src._id == inp._source)
                            {
                                pInData[inp._outputOffset] = (byte*)((DataSource)src._arrayData).Address;
                                break;
                            }
                    }
                }

                //Decode face data using command list
                foreach (PrimitiveFace f in prim._faces)
                    fixed (ushort* p = f._pointIndices)
                        RunPrimitiveCmd(pInData, pOutData, pCmd, count, p, f._pointCount);

                //Process point indices
                switch (prim._type)
                {
                    case ColladaPrimitiveType.triangles:
                        count = prim._faceCount * 3;
                        while (count-- > 0)
                            pTriarr[pTri++] = fIndex++;
                        break;
                    case ColladaPrimitiveType.trifans:
                    case ColladaPrimitiveType.polygons:
                    case ColladaPrimitiveType.polylist:
                        foreach (PrimitiveFace f in prim._faces)
                        {
                            count = f._pointCount - 2;
                            temp = fIndex;
                            fIndex += 2;
                            while (count-- > 0)
                            {
                                pTriarr[pTri++] = temp;
                                pTriarr[pTri++] = fIndex - 1;
                                pTriarr[pTri++] = fIndex++;
                            }
                        }
                        break;
                    case ColladaPrimitiveType.tristrips:
                        foreach (PrimitiveFace f in prim._faces)
                        {
                            count = f._pointCount;
                            fIndex += 2;
                            for (int i = 2; i < count; i++)
                            {
                                if ((i & 1) == 0)
                                {
                                    pTriarr[pTri++] = fIndex - 2;
                                    pTriarr[pTri++] = fIndex - 1;
                                    pTriarr[pTri++] = fIndex++;
                                }
                                else
                                {
                                    pTriarr[pTri++] = fIndex - 2;
                                    pTriarr[pTri++] = fIndex;
                                    pTriarr[pTri++] = fIndex++ - 1;
                                }
                            }
                        }
                        break;

                    case ColladaPrimitiveType.linestrips:
                        foreach (PrimitiveFace f in prim._faces)
                        {
                            count = f._pointCount - 1;
                            lIndex++;
                            while (count-- > 0)
                            {
                                pLinarr[pLin++] = lIndex - 1;
                                pLinarr[pLin++] = lIndex++;
                            }
                        }
                        break;

                    case ColladaPrimitiveType.lines:
                        foreach (PrimitiveFace f in prim._faces)
                        {
                            count = f._pointCount;
                            while (count-- > 0)
                                pLinarr[pLin++] = lIndex++;
                        }
                        break;
                }
            }
            return data;
        }

        private static void RunPrimitiveCmd(byte** pIn, byte** pOut, PrimitiveDecodeCommand* pCmd, int cmdCount, ushort* pIndex, int count)
        {
            int buffer;
            while (count-- > 0)
                for (int i = 0; i < cmdCount; i++)
                {
                    buffer = pCmd[i].Index;
                    switch ((SemanticType)pCmd[i].Cmd)
                    {
                        case SemanticType.None:
                            *pIndex += 1;
                            break;

                        case SemanticType.VERTEX:
                            //Can't do remap table because weights haven't been assigned yet!
                            *(ushort*)pOut[buffer] = *pIndex++;
                            pOut[buffer] += 2;
                            break;

                        case SemanticType.NORMAL:
                            *(Vector3*)pOut[buffer] = ((Vector3*)pIn[buffer])[*pIndex++];
                            pOut[buffer] += 12;
                            break;

                        case SemanticType.COLOR:
                            float* p = (float*)(pIn[buffer] + (*pIndex++ * 16));
                            byte* p2 = pOut[buffer];
                            for (int x = 0; x < 4; x++)
                                *p2++ = (byte)(*p++ * 255.0f + 0.5f);
                            pOut[buffer] = p2;
                            break;

                        case SemanticType.TEXCOORD:
                            //Flip y axis so coordinates are bottom-up
                            Vector2 v = ((Vector2*)pIn[buffer])[*pIndex++];
                            v._y = 1.0f - v._y;
                            *(Vector2*)pOut[buffer] = v;
                            pOut[buffer] += 8;
                            break;
                    }
                }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct PrimitiveDecodeCommand
        {
            public byte Cmd;
            public byte Index;
            public byte Pad1, Pad2;
        }
    }
}
