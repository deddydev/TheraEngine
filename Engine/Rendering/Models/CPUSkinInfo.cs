using System;

namespace CustomEngine.Rendering.Models
{
    public class CPUSkinInfo
    {
        int _vertexCount;
        IVec4[] _matrixIds;
        Vec4[] _matrixWeights;
        Vec3[] _basePositions, _baseNormals, _baseBinormals, _baseTangents;
        VertexBuffer _positions, _normals, _binormals, _tangents;
        public CPUSkinInfo(PrimitiveData data, Vec4[] matrixWeights, IVec4[] matrixIds)
        {
            (_positions = data[BufferType.Position])?.GetData(out _basePositions, false);
            (_normals = data[BufferType.Normal])?.GetData(out _baseNormals, false);
            (_binormals = data[BufferType.Binormal])?.GetData(out _baseBinormals, false);
            (_tangents = data[BufferType.Tangent])?.GetData(out _baseTangents, false);
            _matrixIds = matrixIds;
            _matrixWeights = matrixWeights;
            _vertexCount = _positions.ElementCount;
        }
        public unsafe void UpdatePNBT(Matrix4[] boneMatrices, Matrix3[] boneMatricesIT)
        {
            for (int i = 0; i < _vertexCount; ++i)
            {
                IVec4 ids = _matrixIds[i];
                Vec4 weights = _matrixWeights[i];
                Matrix4 finalMatrix = new Matrix4();
                Matrix3 finalMatrixIT = new Matrix3();
                for (int j = 0; j < 4; ++j)
                {
                    finalMatrix += boneMatrices[ids[j]] * weights[j];
                    finalMatrixIT += boneMatricesIT[ids[j]] * weights[j];
                }
                ((Vec3*)_positions.Data)[i] = _basePositions[i] * finalMatrix;
                if (_normals != null)
                    ((Vec3*)_normals.Data)[i] = _baseNormals[i] * finalMatrixIT;
                if (_binormals != null)
                    ((Vec3*)_binormals.Data)[i] = _baseBinormals[i] * finalMatrixIT;
                if (_tangents != null)
                    ((Vec3*)_tangents.Data)[i] = _baseTangents[i] * finalMatrixIT;
            }
        }
    }
}