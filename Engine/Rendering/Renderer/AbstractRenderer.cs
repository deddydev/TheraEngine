using CustomEngine.Rendering.Cameras;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CustomEngine.Rendering
{
    /// <summary>
    /// This class is meant to be overridden with an implementation such as OpenTK or DirectX
    /// </summary>
    public abstract class AbstractRenderer
    {
        private SceneProcessor _scene = new SceneProcessor();
        public SceneProcessor Scene { get { return _scene; } }

        public abstract RenderLibrary RenderLibrary { get; }
        public RenderContext CurrentContext { get { return RenderContext.Current; } }

        public Viewport CurrentlyRenderingViewport { get { return Viewport.CurrentlyRendering; } }

        protected Camera _currentCamera;
        protected int _programHandle;
        private static List<DisplayList> _displayLists = new List<DisplayList>();
        private Stack<Rectangle> _renderAreaStack = new Stack<Rectangle>();
        protected Stack<Matrix4> _modelMatrix, _textureMatrix, _colorMatrix;
        protected MtxMode _matrixMode;

        public T[] GenObjects<T>(GenType type, int count) where T : BaseRenderState
        {
            return GenObjects(type, count).Select(x => Activator.CreateInstance(typeof(T), x) as T).ToArray();
        }

        public void SetCommonUniforms()
        {
            Scene.CurrentCamera.SetUniforms();
        }

        public abstract int GenObject(GenType type);
        public abstract int[] GenObjects(GenType type, int count);
        public abstract void DeleteObject(GenType type, int bindingId);
        public abstract void DeleteObjects(GenType type, int[] bindingIds);

        #region Shapes
        public void DrawBoxWireframe(Box box) { DrawBoxWireframe(box.Minimum, box.Maximum); }
        public void DrawBoxSolid(Box box) { DrawBoxSolid(box.Minimum, box.Maximum); }

        public abstract void DrawBoxWireframe(Vec3 min, Vec3 max);
        public abstract void DrawBoxSolid(Vec3 min, Vec3 max);

        public void DrawCapsuleWireframe(Capsule capsule) { DrawCapsuleWireframe(capsule.Radius, capsule.HalfHeight); }
        public void DrawCapsuleSolid(Capsule capsule) { DrawCapsuleSolid(capsule.Radius, capsule.HalfHeight); }
        public abstract void DrawCapsuleWireframe(float radius, float halfHeight);
        public abstract void DrawCapsuleSolid(float radius, float halfHeight);

        public void DrawSphereWireframe(Sphere sphere) { DrawSphereWireframe(sphere.Radius); }
        public void DrawSphereSolid(Sphere sphere) { DrawSphereSolid(sphere.Radius); }
        public abstract void DrawSphereWireframe(float radius);
        public abstract void DrawSphereSolid(float radius);
        #endregion

        #region Matrices
        public Matrix4 ModelMatrix { get { return _modelMatrix.Count > 0 ? _modelMatrix.Peek() : Matrix4.Identity; } }
        public Matrix4 TextureMatrix { get { return _textureMatrix.Count > 0 ? _textureMatrix.Peek() : Matrix4.Identity; } }
        public Matrix4 ColorMatrix { get { return _colorMatrix.Count > 0 ? _colorMatrix.Peek() : Matrix4.Identity; } }
        Stack<Matrix4> CurrentMatrixStack
        {
            get
            {
                switch (_matrixMode)
                {
                    case MtxMode.Model:
                        return _modelMatrix;
                    case MtxMode.Color:
                        return _modelMatrix;
                    case MtxMode.Texture:
                        return _modelMatrix;
                }
                return null;
            }
        }
        public void PushMatrix()
        {
            var stack = CurrentMatrixStack;
            if (stack == null)
                return;
            if (stack.Count > 0)
            {
                Matrix4 current = stack.Peek();
                stack.Push(current);
            }
            else
                stack.Push(Matrix4.Identity);
        }
        public void PopMatrix()
        {
            var stack = CurrentMatrixStack;
            if (stack != null && stack.Count > 0)
                stack.Pop();
        }
        public void MultMatrix(Matrix4 matrix)
        {
            var stack = CurrentMatrixStack;
            if (stack == null)
                return;
            if (stack.Count > 0)
            {
                Matrix4 current = stack.Pop();
                stack.Push(matrix * current);
            }
            else
                stack.Push(matrix);
        }
        public Matrix4 GetCurrentMatrix()
        {
            var stack = CurrentMatrixStack;
            return stack.Count > 0 ? stack.Peek() : Matrix4.Identity;
        }
        public void MatrixMode(MtxMode mode)
        {
            _matrixMode = mode;
        }
        public void SetIdentity() { SetMatrix(Matrix4.Identity); }
        public void SetMatrix(Matrix4 matrix)
        {
            var stack = CurrentMatrixStack;
            if (stack == null)
                return;
            if (stack.Count > 0)
                stack.Pop();
            stack.Push(matrix);
        }
        #endregion

        #region Display Lists
        public abstract int CreateDisplayList();
        public abstract void BeginDisplayList(int id, DisplayListMode mode);
        public abstract void EndDisplayList();
        public abstract void CallDisplayList(int id);
        public abstract void DeleteDisplayList(int id);
        #endregion

        #region Drawing
        public abstract void Begin(EPrimitive type);
        public abstract void Vertex3(Vec3 value);
        public abstract void Vertex2(Vec2 value);
        public abstract void Normal3(Vec3 value);
        public abstract void TexCoord2(Vec2 value);
        public abstract void MultiTexCoord2(int unit, Vec2 value);
        public abstract void Color4(ColorF4 value);
        public abstract void Color3(ColorF3 value);
        public abstract void End();
        
        public abstract void SetPointSize(float size);
        public abstract void SetLineSize(float size);
        #endregion

        #region Shaders
        /*
         * ---Shader initialization routine---
         * 
         * Per shader:
         * - GL.CreateShader
         * - GL.ShaderSource
         * - GL.CompileShader
         * - optional debug: GL.GetShader, GL.GetShaderInfoLog
         * 
         * GL.CreateProgram
         * GL.AttachShader(s)
         * GL.LinkProgram
         * 
         * ---Render usage---
         * 
         * GL.UseProgram
         * 
         * Per Uniform:
         * GL.GetUniformLocation
         * GL.Uniform
         * 
         */

        public abstract void SetBindFragDataLocation(int bindingId, int location, string name);
        public abstract void SetShaderMode(ShaderMode type);
        /// <summary>
        /// Creates a new shader.
        /// </summary>
        /// <param name="type">The type of shader</param>
        /// <param name="source">The code for the shader</param>
        /// <returns>The shader's handle.</returns>
        public abstract int GenerateShader(string source);
        /// <summary>
        /// Creates a new shader program with the given shaders.
        /// </summary>
        /// <param name="shaderHandles">The handles of the shaders for this program to use.</param>
        /// <returns></returns>
        public abstract int GenerateProgram(int[] shaderHandles, params VertexAttribInfo[] inAttributes);

        public virtual void UseMaterial(int handle) { _programHandle = handle; }
        public virtual void DeleteProgram(int handle)
        {
            if (_programHandle == handle)
                _programHandle = 0;
        }

        public abstract int GetAttribLocation(string name);
        public abstract int GetUniformLocation(string name);

        public void Uniform(string name, params IUniformable4Int[] p) => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params IUniformable4Float[] p) => Uniform(GetUniformLocation(name), p);

        public void Uniform(string name, params IUniformable3Int[] p) => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params IUniformable3Float[] p) => Uniform(GetUniformLocation(name), p);

        public void Uniform(string name, params IUniformable2Int[] p) => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params IUniformable2Float[] p) => Uniform(GetUniformLocation(name), p);

        public void Uniform(string name, params IUniformable1Int[] p) => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params IUniformable1Float[] p) => Uniform(GetUniformLocation(name), p);

        public void Uniform(string name, params int[] p) => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params float[] p) => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params uint[] p) => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, params double[] p) => Uniform(GetUniformLocation(name), p);

        public void Uniform(string name, Matrix4 p) => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, Matrix4[] p) => Uniform(GetUniformLocation(name), p);

        public abstract void Uniform(int location, params IUniformable4Int[] p);
        public abstract void Uniform(int location, params IUniformable4Float[] p);
        public void Uniform(int location, params IUniformable4Double[] p) { throw new NotImplementedException(); }
        public void Uniform(int location, params IUniformable4UInt[] p) { throw new NotImplementedException(); }
        public void Uniform(int location, params IUniformable4Bool[] p) { throw new NotImplementedException(); }

        public abstract void Uniform(int location, params IUniformable3Int[] p);
        public abstract void Uniform(int location, params IUniformable3Float[] p);
        public void Uniform(int location, params IUniformable3Double[] p) { throw new NotImplementedException(); }
        public void Uniform(int location, params IUniformable3UInt[] p) { throw new NotImplementedException(); }
        public void Uniform(int location, params IUniformable3Bool[] p) { throw new NotImplementedException(); }

        public abstract void Uniform(int location, params IUniformable2Int[] p);
        public abstract void Uniform(int location, params IUniformable2Float[] p);
        public void Uniform(int location, params IUniformable2Double[] p) { throw new NotImplementedException(); }
        public void Uniform(int location, params IUniformable2UInt[] p) { throw new NotImplementedException(); }
        public void Uniform(int location, params IUniformable2Bool[] p) { throw new NotImplementedException(); }

        public abstract void Uniform(int location, params IUniformable1Int[] p);
        public abstract void Uniform(int location, params IUniformable1Float[] p);
        public void Uniform(int location, params IUniformable1Double[] p) { throw new NotImplementedException(); }
        public void Uniform(int location, params IUniformable1UInt[] p) { throw new NotImplementedException(); }
        public void Uniform(int location, params IUniformable1Bool[] p) { throw new NotImplementedException(); }

        public abstract void Uniform(int location, params int[] p);
        public abstract void Uniform(int location, params float[] p);
        public void Uniform(int location, params double[] p) { throw new NotImplementedException(); }
        public void Uniform(int location, params uint[] p) { throw new NotImplementedException(); }
        public void Uniform(int location, params bool[] p) { throw new NotImplementedException(); }

        public abstract void Uniform(int location, Matrix4 p);
        public abstract void Uniform(int location, params Matrix4[] p);
        public abstract void Uniform(int location, Matrix3 p);
        public abstract void Uniform(int location, params Matrix3[] p);

        public void Uniform(int matID, string name, params IUniformable4Int[] p) => Uniform(matID, GetUniformLocation(name), p);
        public void Uniform(int matID, string name, params IUniformable4Float[] p) => Uniform(matID, GetUniformLocation(name), p);

        public void Uniform(int matID, string name, params IUniformable3Int[] p) => Uniform(matID, GetUniformLocation(name), p);
        public void Uniform(int matID, string name, params IUniformable3Float[] p) => Uniform(matID, GetUniformLocation(name), p);

        public void Uniform(int matID, string name, params IUniformable2Int[] p) => Uniform(matID, GetUniformLocation(name), p);
        public void Uniform(int matID, string name, params IUniformable2Float[] p) => Uniform(matID, GetUniformLocation(name), p);

        public void Uniform(int matID, string name, params IUniformable1Int[] p) => Uniform(matID, GetUniformLocation(name), p);
        public void Uniform(int matID, string name, params IUniformable1Float[] p) => Uniform(matID, GetUniformLocation(name), p);

        public void Uniform(int matID, string name, params int[] p) => Uniform(matID, GetUniformLocation(name), p);
        public void Uniform(int matID, string name, params float[] p) => Uniform(matID, GetUniformLocation(name), p);
        public void Uniform(int matID, string name, params uint[] p) => Uniform(matID, GetUniformLocation(name), p);
        public void Uniform(int matID, string name, params double[] p) => Uniform(matID, GetUniformLocation(name), p);

        public void Uniform(int matID, string name, Matrix4 p) => Uniform(matID, GetUniformLocation(name), p);
        public void Uniform(int matID, string name, Matrix4[] p) => Uniform(matID, GetUniformLocation(name), p);

        public abstract void Uniform(int matID, int location, params IUniformable4Int[] p);
        public abstract void Uniform(int matID, int location, params IUniformable4Float[] p);
        public void Uniform(int matID, int location, params IUniformable4Double[] p) { throw new NotImplementedException(); }
        public void Uniform(int matID, int location, params IUniformable4UInt[] p) { throw new NotImplementedException(); }
        public void Uniform(int matID, int location, params IUniformable4Bool[] p) { throw new NotImplementedException(); }

        public abstract void Uniform(int matID, int location, params IUniformable3Int[] p);
        public abstract void Uniform(int matID, int location, params IUniformable3Float[] p);
        public void Uniform(int matID, int location, params IUniformable3Double[] p) { throw new NotImplementedException(); }
        public void Uniform(int matID, int location, params IUniformable3UInt[] p) { throw new NotImplementedException(); }
        public void Uniform(int matID, int location, params IUniformable3Bool[] p) { throw new NotImplementedException(); }

        public abstract void Uniform(int matID, int location, params IUniformable2Int[] p);
        public abstract void Uniform(int matID, int location, params IUniformable2Float[] p);
        public void Uniform(int matID, int location, params IUniformable2Double[] p) { throw new NotImplementedException(); }
        public void Uniform(int matID, int location, params IUniformable2UInt[] p) { throw new NotImplementedException(); }
        public void Uniform(int matID, int location, params IUniformable2Bool[] p) { throw new NotImplementedException(); }

        public abstract void Uniform(int matID, int location, params IUniformable1Int[] p);
        public abstract void Uniform(int matID, int location, params IUniformable1Float[] p);
        public void Uniform(int matID, int location, params IUniformable1Double[] p) { throw new NotImplementedException(); }
        public void Uniform(int matID, int location, params IUniformable1UInt[] p) { throw new NotImplementedException(); }
        public void Uniform(int matID, int location, params IUniformable1Bool[] p) { throw new NotImplementedException(); }

        public abstract void Uniform(int matID, int location, params int[] p);
        public abstract void Uniform(int matID, int location, params float[] p);
        public void Uniform(int matID, int location, params double[] p) { throw new NotImplementedException(); }
        public void Uniform(int matID, int location, params uint[] p) { throw new NotImplementedException(); }
        public void Uniform(int matID, int location, params bool[] p) { throw new NotImplementedException(); }

        public abstract void Uniform(int matID, int location, Matrix4 p);
        public abstract void Uniform(int matID, int location, params Matrix4[] p);
        public abstract void Uniform(int matID, int location, Matrix3 p);
        public abstract void Uniform(int matID, int location, params Matrix3[] p);

        #endregion

        public abstract void Clear(BufferClear mask);
        public abstract float GetDepth(float x, float y);

        public virtual void PushRenderArea(Rectangle region)
        {
            _renderAreaStack.Push(region);
            SetRenderArea(region);
        }
        public virtual void PopRenderArea()
        {
            if (_renderAreaStack.Count > 0)
            {
                _renderAreaStack.Pop();
                if (_renderAreaStack.Count > 0)
                    SetRenderArea(_renderAreaStack.Peek());
            }
        }
        protected abstract void SetRenderArea(Rectangle region);
        public abstract void CropRenderArea(Rectangle region);

        public abstract void BindTextureData(int textureTargetEnum, int mipLevel, int pixelInternalFormatEnum, int width, int height, int pixelFormatEnum, int pixelTypeEnum, VoidPtr data);

        /// <summary>
        /// Draws textures connected to these frame buffer attachments.
        /// </summary>
        public abstract void DrawBuffers(DrawBuffersAttachment[] attachments);
        public abstract void BindFrameBuffer(FramebufferType type, int bindingId);
    }
    public enum FramebufferType
    {
        Read,
        Write,
        ReadWrite,
    }
    public enum MtxMode
    {
        Model,
        Texture,
        Color
    }
    [Flags]
    public enum BufferClear
    {
        Color = 1,
        Depth = 2,
        Stencil = 4,
        Accum = 8,
    }
    public enum DisplayListMode
    {
        //Means this displaylist should not be applied until it is called.
        Compile,
        //Means this displaylist should be applied while compiling.
        CompileAndExecute,
    }
    public enum EPrimitive
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
        Quads = 7,
        QuadStrip = 8,
    }
    public enum DrawBuffersAttachment : ushort
    {
        ColorAttachment0,
        ColorAttachment1,
        ColorAttachment2,
        ColorAttachment3,
        ColorAttachment4,
        ColorAttachment5,
        ColorAttachment6,
        ColorAttachment7,
        ColorAttachment8,
        ColorAttachment9,
        ColorAttachment10,
        ColorAttachment11,
        ColorAttachment12,
        ColorAttachment13,
        ColorAttachment14,
        ColorAttachment15,
        DepthAttachement,
    }
}
