using System;
using CustomEngine.Rendering.Models;
using OpenTK.Graphics.OpenGL;
using CustomEngine.Rendering.Models.Materials;
using System.Linq;

namespace CustomEngine.Rendering
{
    public unsafe class GLRenderer : AbstractRenderer
    {
        // https://www.opengl.org/wiki/Rendering_Pipeline_Overview

        public static GLRenderer Instance = new GLRenderer();
        public GLRenderer() { }

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

        #region Display Lists
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
        #endregion

        #region Drawing
        public override void Begin(EPrimitive type)
        {
            GL.Begin((PrimitiveType)(int)type);
        }
        public override void Vertex3(Vec3 value)
        {
            GL.Vertex3(value.X, value.Y, value.Z);
        }
        public override void Vertex2(Vec2 value)
        {
            GL.Vertex2(value.X, value.Y);
        }
        public override void Normal3(Vec3 value)
        {
            GL.Normal3(value.X, value.Y, value.Z);
        }
        public override void TexCoord2(Vec2 value)
        {
            GL.TexCoord2(value.X, value.Y);
        }
        public override void MultiTexCoord2(int unit, Vec2 value)
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
        public override void RenderMesh(Mesh mesh)
        {

        }
        public override void SetPointSize(float size)
        {
            GL.PointSize(size);
        }
        public override void SetLineSize(float size)
        {
            GL.LineWidth(size);
        }

        #endregion

        #region Shaders
        public override int GenerateShader(Shader shader)
        {
            OpenTK.Graphics.OpenGL.ShaderType sType;
            switch (shader._type)
            {
                case Models.Materials.ShaderType.Fragment:
                    sType = OpenTK.Graphics.OpenGL.ShaderType.FragmentShader;
                    break;
                case Models.Materials.ShaderType.Vertex:
                    sType = OpenTK.Graphics.OpenGL.ShaderType.VertexShader;
                    break;
                case Models.Materials.ShaderType.Geometry:
                    sType = OpenTK.Graphics.OpenGL.ShaderType.GeometryShader;
                    break;
                case Models.Materials.ShaderType.TessControl:
                    sType = OpenTK.Graphics.OpenGL.ShaderType.TessControlShader;
                    break;
                case Models.Materials.ShaderType.TessEvaluation:
                    sType = OpenTK.Graphics.OpenGL.ShaderType.TessEvaluationShader;
                    break;
                case Models.Materials.ShaderType.Compute:
                    sType = OpenTK.Graphics.OpenGL.ShaderType.ComputeShader;
                    break;
                default:
                    return -1;
            }
            int handle = GL.CreateShader(sType);
            GL.ShaderSource(handle, shader._source);
            GL.CompileShader(handle);
#if DEBUG
            int status;
            GL.GetShader(handle, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                string info;
                GL.GetShaderInfoLog(handle, out info);
                Console.WriteLine(info + "\n\n");

                //Split the source by new lines
                string[] s = shader._source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                //Add the line number to the source so we can go right to errors on specific lines
                int lineNumber = 1;
                foreach (string line in s)
                    Console.WriteLine(String.Format("{0}: {1}", (lineNumber++).ToString().PadLeft(s.Length.ToString().Length, '0'), line));

                Console.WriteLine("\n\n");
            }
#endif
            return handle;
        }
        public override int GenerateProgram(params int[] shaderHandles)
        {
            int handle = GL.CreateProgram();
            foreach (int i in shaderHandles)
                GL.AttachShader(handle, i);
            GL.LinkProgram(handle);
            //We don't need these anymore now that they're part of the program
            foreach (int i in shaderHandles)
            {
                GL.DetachShader(handle, i);
                GL.DeleteShader(i);
            }
            return handle;
        }
        public override void UseProgram(int handle)
        {
            base.UseProgram(handle);
            GL.UseProgram(_programHandle);
        }

        #region Uniforms
        public override void Uniform(string name, params IUniformable4Int[] p)
        {
            const int count = 4;

            int u = GL.GetUniformLocation(_programHandle, name);
            if (u < 0)
                return;

            int[] values = new int[p.Length << 2];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i << 2 + x] = p[i].Data[x];

            GL.Uniform3(u, p.Length, values);
        }
        public override void Uniform(string name, params IUniformable4Float[] p)
        {
            const int count = 4;

            int u = GL.GetUniformLocation(_programHandle, name);
            if (u < 0)
                return;

            float[] values = new float[p.Length << 2];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i << 2 + x] = p[i].Data[x];

            GL.Uniform3(u, p.Length, values);
        }
        public override void Uniform(string name, params IUniformable3Int[] p)
        {
            const int count = 3;

            int u = GL.GetUniformLocation(_programHandle, name);
            if (u < 0)
                return;

            int[] values = new int[p.Length * 3];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i * 3 + x] = p[i].Data[x];

            GL.Uniform3(u, p.Length, values);
        }
        public override void Uniform(string name, params IUniformable3Float[] p)
        {
            const int count = 3;

            int u = GL.GetUniformLocation(_programHandle, name);
            if (u < 0)
                return;

            float[] values = new float[p.Length * 3];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i * 3 + x] = p[i].Data[x];

            GL.Uniform3(u, p.Length, values);
        }
        public override void Uniform(string name, params IUniformable2Int[] p)
        {
            const int count = 2;

            int u = GL.GetUniformLocation(_programHandle, name);
            if (u < 0)
                return;

            int[] values = new int[p.Length << 1];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i << 1 + x] = p[i].Data[x];

            GL.Uniform2(u, p.Length, values);
        }
        public override void Uniform(string name, params IUniformable2Float[] p)
        {
            const int count = 2;

            int u = GL.GetUniformLocation(_programHandle, name);
            if (u < 0)
                return;

            float[] values = new float[p.Length << 1];
            
            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i << 1 + x] = p[i].Data[x];

            GL.Uniform2(u, p.Length, values);
        }
        public override void Uniform(string name, params IUniformable1Int[] p)
        {
            int u = GL.GetUniformLocation(_programHandle, name);
            if (u > -1) GL.Uniform1(u, p.Length, p.Select(x => *x.Data).ToArray());
        }
        public override void Uniform(string name, params IUniformable1Float[] p)
        {
            int u = GL.GetUniformLocation(_programHandle, name);
            if (u > -1) GL.Uniform1(u, p.Length, p.Select(x => *x.Data).ToArray());
        }
        public override void Uniform(string name, params int[] p)
        {
            int u = GL.GetUniformLocation(_programHandle, name);
            if (u > -1) GL.Uniform1(u, p.Length, p);
        }
        public override void Uniform(string name, params float[] p)
        {
            int u = GL.GetUniformLocation(_programHandle, name);
            if (u > -1) GL.Uniform1(u, p.Length, p);
        }
        #endregion

        #endregion

        public override float GetDepth(float x, float y)
        {
            float val = 0;
            GL.ReadPixels((int)x, (int)(Engine.CurrentPanel.Height - y), 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref val);
            return val;
        }
    }
}
