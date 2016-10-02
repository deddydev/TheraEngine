using System;
using System.Drawing;
using System.Windows.Forms;

using SlimDX;
using SlimDX.DXGI;
using SlimDX.Direct3D11;
using SlimDX.Direct2D;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;
using FactoryD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using CustomEngine.Rendering.DirectX;

namespace CustomEngine.Rendering
{
    public unsafe class DXRenderer : AbstractRenderer
    {
        public static DXRenderer Instance = new DXRenderer();
        public DXRenderer()
        {

        }

        public override void Clear(BufferClear clearBufferMask)
        {
            throw new NotImplementedException();
        }

        public override void CompileShader(string shader)
        {
            throw new NotImplementedException();
        }

        public override void DrawBoxSolid(System.Vector3 min, System.Vector3 max)
        {
            throw new NotImplementedException();
        }

        public override void DrawBoxWireframe(System.Vector3 min, System.Vector3 max)
        {
            throw new NotImplementedException();
        }

        public override void DrawCapsuleSolid(float radius, float halfHeight)
        {
            throw new NotImplementedException();
        }

        public override void DrawCapsuleWireframe(float radius, float halfHeight)
        {
            throw new NotImplementedException();
        }

        public override void DrawSphereSolid(System.Single radius)
        {
            throw new NotImplementedException();
        }

        public override void DrawSphereWireframe(System.Single radius)
        {
            throw new NotImplementedException();
        }

        public override void LoadMatrix(System.Matrix4 matrix)
        {
            throw new NotImplementedException();
        }

        public override void MatrixMode(MtxMode modelview)
        {
            throw new NotImplementedException();
        }

        public override void MultMatrix(System.Matrix4 matrix)
        {
            throw new NotImplementedException();
        }

        public override void PopMatrix()
        {
            throw new NotImplementedException();
        }

        public override void PushMatrix()
        {
            throw new NotImplementedException();
        }

        public override void Rotate(System.Quaternion rotation)
        {
            throw new NotImplementedException();
        }

        public override void Rotate(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public override void Scale(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public override void SetLineSize(float size)
        {
            throw new NotImplementedException();
        }

        public override void SetPointSize(float size)
        {
            throw new NotImplementedException();
        }

        public override void Translate(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        #region Conversion
        private SlimDX.Matrix DXMat4(System.Matrix4 matrix4)
        {
            //OpenTK.Matrix4 m = new OpenTK.Matrix4();
            //float* sPtr = (float*)&m;
            //float* dPtr = (float*)&matrix4;
            //for (int i = 0; i < 16; ++i)
            //    *dPtr++ = *sPtr++;
            //return m;
            return *(SlimDX.Matrix*)&matrix4;
        }
        private SlimDX.Vector4 DXVec4(System.Vector4 vec4)
        {
            return *(SlimDX.Vector4*)&vec4;
        }
        private SlimDX.Vector3 DXVec3(System.Vector3 vec3)
        {
            return *(SlimDX.Vector3*)&vec3;
        }
        private SlimDX.Vector2 DXVec2(System.Vector2 vec2)
        {
            return *(SlimDX.Vector2*)&vec2;
        }
        private SlimDX.Quaternion DXQuat(System.Quaternion quat)
        {
            return *(SlimDX.Quaternion*)&quat;
        }
        #endregion

        public override float GetDepth(float x, float y)
        {
            throw new NotImplementedException();
        }

        public override int CreateDisplayList()
        {
            throw new NotImplementedException();
        }

        public override void BeginDisplayList(int id, DisplayListMode mode)
        {
            throw new NotImplementedException();
        }

        public override void EndDisplayList()
        {
            throw new NotImplementedException();
        }

        public override void CallDisplayList(int id)
        {
            throw new NotImplementedException();
        }

        public override void DeleteDisplayList(int id)
        {
            throw new NotImplementedException();
        }

        public override void Begin(EPrimitive type)
        {
            throw new NotImplementedException();
        }

        public override void Vertex3(System.Vector3 value)
        {
            throw new NotImplementedException();
        }

        public override void Vertex2(System.Vector2 value)
        {
            throw new NotImplementedException();
        }

        public override void Normal3(System.Vector3 value)
        {
            throw new NotImplementedException();
        }

        public override void TexCoord2(System.Vector2 value)
        {
            throw new NotImplementedException();
        }

        public override void MultiTexCoord2(int unit, System.Vector2 value)
        {
            throw new NotImplementedException();
        }

        public override void End()
        {
            throw new NotImplementedException();
        }

        public override void AttachShader(int programHandle, int shaderHandle)
        {
            throw new NotImplementedException();
        }

        public override void LinkProgram(int programHandle)
        {
            throw new NotImplementedException();
        }
    }
}
