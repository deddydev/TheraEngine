using System;
using CustomEngine.Rendering.Models.Meshes;
using OpenTK.Graphics.OpenGL;

namespace CustomEngine.Rendering
{
    public unsafe class GLRenderer : AbstractRenderer
    {
        public static GLRenderer Instance = new GLRenderer();
        public GLRenderer()
        {
            
        }

        #region Shapes
        public override void DrawBoxWireframe(System.Vec3 min, System.Vec3 max)
        {
            GL.Begin(PrimitiveType.LineStrip);

            GL.Vertex3(max.X, max.Y, max.Z);
            GL.Vertex3(max.X, max.Y, min.Z);
            GL.Vertex3(min.X, max.Y, min.Z);
            GL.Vertex3(min.X, min.Y, min.Z);
            GL.Vertex3(min.X, min.Y, max.Z);
            GL.Vertex3(max.X, min.Y, max.Z);
            GL.Vertex3(max.X, max.Y, max.Z);

            GL.End();

            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(min.X, max.Y, max.Z);
            GL.Vertex3(max.X, max.Y, max.Z);
            GL.Vertex3(min.X, max.Y, max.Z);
            GL.Vertex3(min.X, min.Y, max.Z);
            GL.Vertex3(min.X, max.Y, max.Z);
            GL.Vertex3(min.X, max.Y, min.Z);
            GL.Vertex3(max.X, min.Y, min.Z);
            GL.Vertex3(min.X, min.Y, min.Z);
            GL.Vertex3(max.X, min.Y, min.Z);
            GL.Vertex3(max.X, max.Y, min.Z);
            GL.Vertex3(max.X, min.Y, min.Z);
            GL.Vertex3(max.X, min.Y, max.Z);

            GL.End();
        }
        public override void DrawBoxSolid(System.Vec3 min, System.Vec3 max)
        {
            GL.Begin(PrimitiveType.QuadStrip);
            
            GL.Vertex3(min.X, min.Y, min.Z);
            GL.Vertex3(min.X, max.Y, min.Z);
            GL.Vertex3(max.X, min.Y, min.Z);
            GL.Vertex3(max.X, max.Y, min.Z);
            GL.Vertex3(max.X, min.Y, max.Z);
            GL.Vertex3(max.X, max.Y, max.Z);
            GL.Vertex3(min.X, min.Y, max.Z);
            GL.Vertex3(min.X, max.Y, max.Z);
            GL.Vertex3(min.X, min.Y, min.Z);
            GL.Vertex3(min.X, max.Y, min.Z);

            GL.End();

            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(min.X, max.Y, min.Z);
            GL.Vertex3(min.X, max.Y, max.Z);
            GL.Vertex3(max.X, max.Y, max.Z);
            GL.Vertex3(max.X, max.Y, min.Z);

            GL.Vertex3(min.X, min.Y, min.Z);
            GL.Vertex3(min.X, min.Y, max.Z);
            GL.Vertex3(max.X, min.Y, max.Z);
            GL.Vertex3(max.X, min.Y, min.Z);

            GL.End();
        }
        public override void DrawCapsuleWireframe(System.Single radius, System.Single halfHeight)
        {
            throw new NotImplementedException();
        }
        public override void DrawCapsuleSolid(System.Single radius, System.Single halfHeight)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override void SetPointSize(System.Single size)
        {
            GL.PointSize(size);
        }
        public override void SetLineSize(System.Single size)
        {
            GL.LineWidth(size);
        }
        public override void CompileShader(System.String shader)
        {
            
        }

        #region Matrices
        public override void PushMatrix()
        {
            GL.PushMatrix();
        }
        public override void PopMatrix()
        {
            GL.PopAttrib();
        }
        public override void MultMatrix(System.Matrix4 matrix)
        {
            OpenTK.Matrix4 m = GLMat4(matrix);
            GL.MultMatrix(ref m);
        }
        public override void Translate(float x, float y, float z)
        {
            GL.Translate(x, y, z);
        }
        public override void Scale(float x, float y, float z)
        {
            GL.Scale(x, y, z);
        }
        public override void Rotate(float roll, float pitch, float yaw)
        {
            //TODO: which method is faster?
            //Method 1:
            System.Matrix4 rotX = System.Matrix4.CreateRotationX(roll);
            System.Matrix4 rotY = System.Matrix4.CreateRotationY(pitch);
            System.Matrix4 rotZ = System.Matrix4.CreateRotationZ(yaw);
            MultMatrix(rotX * rotY * rotZ);

            //Method 2:
            //Rotate(Quaternion.FromEulerAngles(pitch, yaw, roll));
        }
        public override void Rotate(System.Quaternion rotation)
        {
            MultMatrix(System.Matrix4.CreateFromQuaternion(rotation));
        }

        public override void DrawSphereWireframe(float radius)
        {
            throw new NotImplementedException();
        }

        public override void DrawSphereSolid(float radius)
        {
            throw new NotImplementedException();
        }

        public override void Clear(BufferClear clearBufferMask)
        {
            throw new NotImplementedException();
        }

        public override void MatrixMode(MtxMode modelview)
        {
            throw new NotImplementedException();
        }

        public override void LoadMatrix(System.Matrix4 matrix)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Conversion
        private OpenTK.Matrix4 GLMat4(System.Matrix4 matrix4)
        {
            //OpenTK.Matrix4 m = new OpenTK.Matrix4();
            //float* sPtr = (float*)&m;
            //float* dPtr = (float*)&matrix4;
            //for (int i = 0; i < 16; ++i)
            //    *dPtr++ = *sPtr++;
            //return m;
            return *(OpenTK.Matrix4*)&matrix4;
        }
        private OpenTK.Vector4 GLVec4(System.Vec4 vec4)
        {
            return *(OpenTK.Vector4*)&vec4;
        }
        private OpenTK.Vector3 GLVec3(System.Vec3 vec3)
        {
            return *(OpenTK.Vector3*)&vec3;
        }
        private OpenTK.Vector2 GLVec2(System.Vec2 vec2)
        {
            return *(OpenTK.Vector2*)&vec2;
        }
        private OpenTK.Quaternion GLQuat(System.Quaternion quat)
        {
            return *(OpenTK.Quaternion*)&quat;
        }
        #endregion

        public override float GetDepth(float x, float y)
        {
            float val = 0;
            GL.ReadPixels((int)x, (int)(Engine.CurrentPanel.Height - y), 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref val);
            return val;
        }

        public override int CreateDisplayList()
        {
            return GL.GenLists(1);
        }
        public override void BeginDisplayList(int id, DisplayListMode mode)
        {
            GL.NewList(id, mode == DisplayListMode.Compile ? ListMode.Compile : ListMode.CompileAndExecute);
        }
        public override void EndDisplayList()
        {
            GL.EndList();
        }
        public override void CallDisplayList(int id)
        {
            GL.CallList(id);
        }
        public override void DeleteDisplayList(int id)
        {
            GL.DeleteLists(id, 1);
        }
        public override void Begin(EPrimitive type)
        {
            GL.Begin((PrimitiveType)(int)type);
        }
        public override void Vertex3(System.Vec3 value)
        {
            GL.Vertex3(value.X, value.Y, value.Z);
        }
        public override void Vertex2(System.Vec2 value)
        {
            GL.Vertex2(value.X, value.Y);
        }
        public override void Normal3(System.Vec3 value)
        {
            GL.Normal3(value.X, value.Y, value.Z);
        }
        public override void TexCoord2(System.Vec2 value)
        {
            GL.TexCoord2(value.X, value.Y);
        }
        public override void MultiTexCoord2(int unit, System.Vec2 value)
        {
            GL.MultiTexCoord2(TextureUnit.Texture0 + unit, value.X, value.Y);
        }
        public override void Color4(ColorF4 value)
        {
            GL.Color4(value.R, value.G, value.B, value.A);
        }
        public override void Color3(ColorF3 value)
        {
            GL.Color3(value.R, value.G, value.B);
        }
        public override void End()
        {
            GL.End();
        }

        public override void AttachShader(int programHandle, int shaderHandle)
        {
            GL.AttachShader(programHandle, shaderHandle);
        }

        public override void LinkProgram(int programHandle)
        {
            GL.LinkProgram(programHandle);
        }

        public override void PrepareMesh(Mesh mesh)
        {
            GL.BindVertexArray(mesh._vao);

            // layout(location = 0) in vec3 position;
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribFormat(0, 3, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(0, 0); // bind to first vertex buffer

            // layout(location = 2) in vec3 normal;
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribFormat(2, 3, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(2, 1); // bind to second vertex buffer

            GL.BindVertexArray(0);
        }

        public override void RenderMesh(Mesh mesh)
        {
            GL.BindVertexArray(mesh._vao);

            GL.BindVertexBuffers(0, mesh._bufferCount, );
            GL.DrawArrays(PrimitiveType.Triangles, 0, mesh._faces.Count * 3);

            GL.BindVertexArray(0);
        }
    }
}
