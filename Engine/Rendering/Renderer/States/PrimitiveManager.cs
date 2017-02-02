using CustomEngine.Rendering.Models.Materials;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    /// <summary>
    /// Used to render raw primitive data.
    /// </summary>
    public class PrimitiveManager : BaseRenderState
    {
        public int[] _bindingIds;
        public IntPtr[] _offsets;
        public int[] _strides;

        private MeshProgram _program;
        private PrimitiveData _data;
        private VertexBuffer _indexBuffer;
        public DrawElementsType _elementType;
        private Bone[] _utilizedBones;
        private PrimitiveBufferInfo _bufferInfo = new PrimitiveBufferInfo();
        private Material _material;

        public PrimitiveManager() : base(GenType.VertexArray) { }
        public PrimitiveManager(PrimitiveData data, Material material) : base(GenType.VertexArray)
        {
            Data = data;
            _material = material;
        }

        public PrimitiveData Data
        {
            get { return _data; }
            set
            {
                Destroy();
                if (_indexBuffer != null)
                {
                    _indexBuffer.Dispose();
                    _indexBuffer = null;
                }
                _data = value;
                if (_data != null)
                {
                    _indexBuffer = new VertexBuffer("FaceIndices", BufferTarget.ElementArrayBuffer);
                    if (_data._facePoints.Count <= byte.MaxValue)
                    {
                        _elementType = DrawElementsType.UnsignedByte;
                        _indexBuffer.SetDataNumeric(_data.GetIndices().Select(x => (byte)x).ToList());
                    }
                    else if (_data._facePoints.Count <= short.MaxValue)
                    {
                        _elementType = DrawElementsType.UnsignedShort;
                        _indexBuffer.SetDataNumeric(_data.GetIndices().Select(x => (ushort)x).ToList());
                    }
                    else
                    {
                        _elementType = DrawElementsType.UnsignedInt;
                        _indexBuffer.SetDataNumeric(_data.GetIndices());
                    }
                }
            }
        }
        public Material Material
        {
            get { return _material; }
            set
            {
                _material = value;
                if (_program != null)
                    _program.SetMaterial(_material);
            }
        }
        public void SkeletonChanged(Skeleton skeleton)
        {
            _data[BufferType.MatrixIds]?.Dispose();
            _data[BufferType.MatrixWeights]?.Dispose();
            if (skeleton != null)
            {
                _utilizedBones = _data._utilizedBones.Select(x => skeleton.BoneCache[x]).ToArray();

                int infCount = _data._influences.Length;
                IVec4[] matrixIndices = new IVec4[infCount];
                Vec4[] matrixWeights = new Vec4[infCount];

                for (int i = 0; i < infCount; ++i)
                {
                    matrixIndices[i] = new IVec4();
                    matrixWeights[i] = new Vec4();
                    Influence inf = _data._influences[i];
                    for (int j = 0; j < Influence.MaxWeightCount; ++j)
                    {
                        BoneWeight b = inf.Weights[j];
                        if (b == null)
                        {
                            matrixIndices[i][j] = 0;
                            matrixWeights[i][j] = 0.0f;
                        }
                        else
                        {
                            matrixIndices[i][j] = _data._utilizedBones.IndexOf(b.Bone) + 1;
                            matrixWeights[i][j] = b.Weight;
                        }
                    }
                }

                _data.AddBuffer(
                    matrixIndices.ToList(), 
                    new VertexAttribInfo(BufferType.MatrixIds), 
                    false, BufferTarget.ArrayBuffer);

                _data.AddBuffer(
                    matrixWeights.ToList(),
                    new VertexAttribInfo(BufferType.MatrixWeights), 
                    false, BufferTarget.ArrayBuffer);

                _bufferInfo._boneCount = _utilizedBones.Length;
            }
            else
            {
                _bufferInfo._boneCount = 0;
                _utilizedBones = null;
            }
        }
        private void SetSkinningUniforms()
        {
            if (!_bufferInfo.IsWeighted)
                return;

            Matrix4[] positionMatrices = new Matrix4[_utilizedBones.Length + 1];
            Matrix3[] normalMatrices = new Matrix3[_utilizedBones.Length + 1];
            positionMatrices[0] = Matrix4.Identity;
            normalMatrices[0] = Matrix3.Identity;

            for (int i = 1; i < _utilizedBones.Length + 1; ++i)
            {
                Bone b = _utilizedBones[i - 1];
                positionMatrices[i] = b.VertexMatrix;
                normalMatrices[i] = b.VertexMatrixIT;
            }
            
            Engine.Renderer.Uniform(Uniform.BoneMatricesName, positionMatrices.ToArray());
            Engine.Renderer.Uniform(Uniform.BoneMatricesITName, normalMatrices.ToArray());
            //Engine.Renderer.Uniform(Uniform.MorphWeightsName, _morphWeights);
        }
        public unsafe void Render(Matrix4 modelMatrix)
        {
            //TODO: don't invert, transpose, and get rotation matrix here
            Render(modelMatrix, modelMatrix.Inverted().Transposed().GetRotationMatrix3());
        }
        public unsafe void Render(Matrix4 modelMatrix, Matrix3 normalMatrix)
        {
            if (_data == null)
                return;

            if (!IsActive)
                Generate();

            if (_program == null)
                return;

            Engine.Renderer.UseProgram(_program);
            Engine.Renderer.Cull(_data.Culling);
            
            SetSkinningUniforms();
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ModelMatrix), modelMatrix);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.NormalMatrix), normalMatrix);

            if (_program.Textures.Length > 0)
            {
                GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
                for (int i = 0; i < _material.Textures.Count; ++i)
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + i);
                    Engine.Renderer.Uniform("Texture" + i, i);
                    _program.Textures[i].Bind();
                }
            }
            else
                GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);

            GL.BindVertexArray(BindingId);
            GL.DrawElements(_data._type, _indexBuffer.ElementCount, _elementType, 0);
            GL.BindVertexArray(0);

            Engine.Renderer.UseProgram(null);
        }
        protected override void OnGenerated()
        {
            _program = new MeshProgram(_material, _bufferInfo);
            _program.Generate();

            GL.BindVertexArray(BindingId);
            _bindingIds = _data.GenerateBuffers();
            _indexBuffer.Generate();
            GL.BindVertexArray(0);
        }
        protected override void OnDeleted()
        {
            //_data.Dispose();
            _indexBuffer.Dispose();
            _program.Destroy();
        }
    }
}
