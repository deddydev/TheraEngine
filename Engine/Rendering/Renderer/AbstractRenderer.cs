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
        public SceneProcessor Scene { get { return _scene; } }
        public abstract RenderLibrary RenderLibrary { get; }
        public RenderContext CurrentContext { get { return RenderContext.Current; } }
        public Viewport CurrentlyRenderingViewport { get { return Viewport.CurrentlyRendering; } }

        private Dictionary<int, Material> _activeMaterials = new Dictionary<int, Material>();
        private SceneProcessor _scene = new SceneProcessor();
        protected int _programHandle;
        protected Camera _currentCamera;
        private Stack<Rectangle> _renderAreaStack = new Stack<Rectangle>();
        //private static List<DisplayList> _displayLists = new List<DisplayList>();

        public abstract int GenObject(GenType type);
        public abstract int[] GenObjects(GenType type, int count);
        public T[] GenObjects<T>(GenType type, int count) where T : BaseRenderState
        {
            return GenObjects(type, count).Select(x => Activator.CreateInstance(typeof(T), x) as T).ToArray();
        }
        public abstract void DeleteObject(GenType type, int bindingId);
        public abstract void DeleteObjects(GenType type, int[] bindingIds);

        internal int AddActiveMaterial(Material material)
        {
            int id = _activeMaterials.Count;
            _activeMaterials.Add(id, material);
            return id;
        }
        internal void RemoveActiveMaterial(Material material)
        {
            _activeMaterials.Remove(material.BindingId);
        }

        public abstract void SetPointSize(float size);
        public abstract void SetLineSize(float size);

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
        public abstract int GenerateProgram(int[] shaderHandles);
        public virtual void UseProgram(MeshProgram program)
        {
            _programHandle = program != null ? program.BindingId : BaseRenderState.NullBindingId;
            if (_programHandle > BaseRenderState.NullBindingId)
            {
                program._material.SetUniforms();
                Scene.CurrentCamera.SetUniforms();
                Uniform(Models.Materials.Uniform.GetLocation(ECommonUniform.RenderDelta), Engine.RenderDelta);
            }
        }
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
        public void Uniform(string name, Matrix3 p) => Uniform(GetUniformLocation(name), p);
        public void Uniform(string name, Matrix3[] p) => Uniform(GetUniformLocation(name), p);

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

        public void UniformMaterial(int matID, string name, params IUniformable4Int[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        public void UniformMaterial(int matID, string name, params IUniformable4Float[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        public void UniformMaterial(int matID, string name, params IUniformable3Int[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        public void UniformMaterial(int matID, string name, params IUniformable3Float[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        public void UniformMaterial(int matID, string name, params IUniformable2Int[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        public void UniformMaterial(int matID, string name, params IUniformable2Float[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        public void UniformMaterial(int matID, string name, params IUniformable1Int[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        public void UniformMaterial(int matID, string name, params IUniformable1Float[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        public void UniformMaterial(int matID, string name, params int[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        public void UniformMaterial(int matID, string name, params float[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        public void UniformMaterial(int matID, string name, params uint[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        public void UniformMaterial(int matID, string name, params double[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        public void UniformMaterial(int matID, string name, Matrix4 p) => UniformMaterial(matID, GetUniformLocation(name), p);
        public void UniformMaterial(int matID, string name, Matrix4[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        public abstract void UniformMaterial(int matID, int location, params IUniformable4Int[] p);
        public abstract void UniformMaterial(int matID, int location, params IUniformable4Float[] p);
        public void UniformMaterial(int matID, int location, params IUniformable4Double[] p) { throw new NotImplementedException(); }
        public void UniformMaterial(int matID, int location, params IUniformable4UInt[] p) { throw new NotImplementedException(); }
        public void UniformMaterial(int matID, int location, params IUniformable4Bool[] p) { throw new NotImplementedException(); }

        public abstract void UniformMaterial(int matID, int location, params IUniformable3Int[] p);
        public abstract void UniformMaterial(int matID, int location, params IUniformable3Float[] p);
        public void UniformMaterial(int matID, int location, params IUniformable3Double[] p) { throw new NotImplementedException(); }
        public void UniformMaterial(int matID, int location, params IUniformable3UInt[] p) { throw new NotImplementedException(); }
        public void UniformMaterial(int matID, int location, params IUniformable3Bool[] p) { throw new NotImplementedException(); }

        public abstract void UniformMaterial(int matID, int location, params IUniformable2Int[] p);
        public abstract void UniformMaterial(int matID, int location, params IUniformable2Float[] p);
        public void UniformMaterial(int matID, int location, params IUniformable2Double[] p) { throw new NotImplementedException(); }
        public void UniformMaterial(int matID, int location, params IUniformable2UInt[] p) { throw new NotImplementedException(); }
        public void UniformMaterial(int matID, int location, params IUniformable2Bool[] p) { throw new NotImplementedException(); }

        public abstract void UniformMaterial(int matID, int location, params IUniformable1Int[] p);
        public abstract void UniformMaterial(int matID, int location, params IUniformable1Float[] p);
        public void UniformMaterial(int matID, int location, params IUniformable1Double[] p) { throw new NotImplementedException(); }
        public void UniformMaterial(int matID, int location, params IUniformable1UInt[] p) { throw new NotImplementedException(); }
        public void UniformMaterial(int matID, int location, params IUniformable1Bool[] p) { throw new NotImplementedException(); }

        public abstract void UniformMaterial(int matID, int location, params int[] p);
        public abstract void UniformMaterial(int matID, int location, params float[] p);
        public void UniformMaterial(int matID, int location, params double[] p) { throw new NotImplementedException(); }
        public void UniformMaterial(int matID, int location, params uint[] p) { throw new NotImplementedException(); }
        public void UniformMaterial(int matID, int location, params bool[] p) { throw new NotImplementedException(); }

        public abstract void UniformMaterial(int matID, int location, Matrix4 p);
        public abstract void UniformMaterial(int matID, int location, params Matrix4[] p);
        public abstract void UniformMaterial(int matID, int location, Matrix3 p);
        public abstract void UniformMaterial(int matID, int location, params Matrix3[] p);

        public void UniformMaterial(int matID, int location, params IUniformable[] p)
        {
            foreach (IUniformable u in p)
            {

            }
        }

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
        public abstract void CropRenderArea(Rectangle region);
        protected abstract void SetRenderArea(Rectangle region);

        public abstract void BindTextureData(int textureTargetEnum, int mipLevel, int pixelInternalFormatEnum, int width, int height, int pixelFormatEnum, int pixelTypeEnum, VoidPtr data);

        /// <summary>
        /// Draws textures connected to these frame buffer attachments.
        /// </summary>
        public abstract void DrawBuffers(DrawBuffersAttachment[] attachments);
        public abstract void BindFrameBuffer(FramebufferType type, int bindingId);

        public abstract void BindTransformFeedback(int bindingId);
        public abstract void BeginTransformFeedback(FeedbackPrimitiveType type);
        public abstract void EndTransformFeedback();
        /// <summary>
        /// Binds a transform feedback buffer to "out" variables in the shader.
        /// </summary>
        public abstract void TransformFeedbackVaryings(int program, string[] varNames);
    }
    public enum FramebufferType
    {
        Read,
        Write,
        ReadWrite,
    }
    public enum FeedbackPrimitiveType
    {
        Points,
        Lines,
        Triangles,
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
