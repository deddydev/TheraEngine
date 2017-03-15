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
        public SceneProcessor Scene => _scene;
        public abstract RenderLibrary RenderLibrary { get; }
        public RenderContext CurrentContext => RenderContext.Current;
        public Viewport CurrentlyRenderingViewport => Viewport.CurrentlyRendering;

        protected MeshProgram _currentMeshProgram;
        protected PrimitiveManager _currentPrimitiveManager;
        protected Dictionary<int, Material> _activeMaterials = new Dictionary<int, Material>();
        protected SceneProcessor _scene = new SceneProcessor();
        protected Dictionary<string, PrimitiveManager> _debugPrimitives = new Dictionary<string, PrimitiveManager>();

        private PrimitiveManager AssignDebugPrimitive(string name, PrimitiveManager m)
        {
            if (!_debugPrimitives.ContainsKey(name))
                _debugPrimitives.Add(name, m);
            else
                _debugPrimitives[name] = m;
            return m;
        }

        public enum DebugPrimitiveType
        {
            Point,
            Line,
            WireSphere,
            SolidSphere,
            WireBox,
            SolidBox,
        }

        public PrimitiveManager CacheDebugPrimitive(string name, DebugPrimitiveType type)
        {
            if (_debugPrimitives.ContainsKey(name))
                return _debugPrimitives[name];
            PrimitiveData data = null;
            switch (type)
            {
                case DebugPrimitiveType.Point:
                    data = PrimitiveData.FromPoints(Vec3.Zero);
                    break;
                case DebugPrimitiveType.Line:
                    VertexLine line = new VertexLine(new Vertex(Vec3.Zero), new Vertex(Vec3.Forward));
                    data = PrimitiveData.FromLines(new PrimitiveBufferInfo()
                    { _hasNormals = false, _texcoordCount = 0 }, line);
                    break;
                case DebugPrimitiveType.WireSphere:
                    //Diameter is set to 2.0f on purpose
                    data = Sphere.WireframeMesh(Vec3.Zero, 1.0f, 60);
                    break;
                case DebugPrimitiveType.SolidSphere:
                    //Diameter is set to 2.0f on purpose
                    data = Sphere.SolidMesh(Vec3.Zero, 1.0f, 30.0f);
                    break;
                case DebugPrimitiveType.WireBox:
                    data = BoundingBox.WireframeMesh(new Vec3(-0.5f), new Vec3(0.5f));
                    break;
                case DebugPrimitiveType.SolidBox:
                    data = BoundingBox.SolidMesh(new Vec3(-0.5f), new Vec3(0.5f));
                    break;
            }
            return AssignDebugPrimitive(name, new PrimitiveManager(data, Material.GetUnlitColorMaterial()));
        }
        //public void CacheWireframePlane()
        //{
        //    _wirePlane = new PrimitiveManager(
        //        Plane.WireframeMesh(Vec3.Zero, Rotator.GetZero(), 1.0f, 1.0f),
        //        Material.GetUnlitColorMaterial());
        //}
        //public void CacheSolidPlane()
        //{
        //    _solidPlane = new PrimitiveManager(
        //        Plane.SolidMesh(Vec3.Zero, Rotator.GetZero(), 1.0f, 1.0f, Culling.None),
        //        Material.GetUnlitColorMaterial());
        //}

        //public void RenderCapsule(string name, Vec3 center, Vec3 axis, float topHeight, float topRadius, float bottomHeight, float bottomRadius, Matrix4 transform, bool solid, ColorF4 color)
        //{
        //    Vec3 normal = axis.GetSafeNormal();
        //    RenderCapsule(name, center + normal * topHeight, center - normal * bottomHeight, topRadius, bottomRadius, transform, solid, color);
        //}
        //public void RenderCapsule(string name, Vec3 topPoint, Vec3 bottomPoint, float topRadius, float bottomRadius, Matrix4 transform, bool solid, ColorF4 color)
        //    => RenderCapsule(name, Vec3.TransformPosition(topPoint, transform), Vec3.TransformPosition(bottomPoint, transform), topRadius, bottomRadius, solid, color);
        //public void RenderCylinder(string name, Vec3 center, Vec3 axis, float topHeight, float topRadius, float bottomHeight, float bottomRadius, Matrix4 transform, bool solid, ColorF4 color)
        //{
        //    Vec3 normal = axis.GetSafeNormal();
        //    RenderCylinder(name, center + normal * topHeight, center - normal * bottomHeight, topRadius, bottomRadius, transform, solid, color);
        //}
        //public void RenderCylinder(string name, Vec3 topPoint, Vec3 bottomPoint, float topRadius, float bottomRadius, Matrix4 transform, bool solid, ColorF4 color)
        //    => RenderCylinder(name, Vec3.TransformPosition(topPoint, transform), Vec3.TransformPosition(bottomPoint, transform), topRadius, bottomRadius, solid, color);
        //public void RenderCone(string name, Vec3 center, Vec3 axis, float topHeight, float bottomHeight, float bottomRadius, Matrix4 transform, bool solid, ColorF4 color)
        //{
        //    Vec3 normal = axis.GetSafeNormal();
        //    RenderCone(name, center + normal * topHeight, center - normal * bottomHeight, bottomRadius, transform, solid, color);
        //}
        //public void RenderCone(string name, Vec3 topPoint, Vec3 bottomPoint, float bottomRadius, Matrix4 transform, bool solid, ColorF4 color)
        //    => RenderCone(name, Vec3.TransformPosition(topPoint, transform), bottomPoint * transform, bottomRadius, solid, color);

        public void RenderPoint(string name, Vec3 position, float size, ColorF4 color)
        {
            SetPointSize(size);
            PrimitiveManager m = CacheDebugPrimitive(name, DebugPrimitiveType.Point);
            m.Parameter<GLVec4>(0).Value = color;
            m.Render(Matrix4.CreateTranslation(position));
        }
        public unsafe void RenderLine(string name, Vec3 start, Vec3 end, float size, ColorF4 color)
        {
            SetLineSize(size);
            PrimitiveManager m = CacheDebugPrimitive(name, DebugPrimitiveType.Line);
            m.Parameter<GLVec4>(0).Value = color;
            ((Vec3*)m.Data[0].Address)[1] = end - start;
            m.Render(Matrix4.CreateTranslation(start), Matrix3.Identity);
        }
        public void RenderPlane(Vec3 position, Vec3 normal, Vec2 dimensions, bool solid)
        {
            throw new NotImplementedException();
            //Matrix4 mtx = Matrix4.CreateTranslation(position) * normal.LookatAngles().GetMatrix();
            //if (solid)
            //{
            //    if (_solidPlane == null)
            //        CacheSolidPlane();
            //    _solidPlane.Render(mtx, Matrix3.Identity);
            //}
            //else
            //{
            //    if (_wirePlane == null)
            //        CacheWireframePlane();
            //    _wirePlane.Render(mtx, Matrix3.Identity);
            //}
        }
        public void RenderSphere(string name, Vec3 center, float radius, bool solid, ColorF4 color)
        {
            //Radius doesn't need to be multiplied by 2.0f; the sphere is already 2.0f in diameter
            Matrix4 mtx = Matrix4.CreateTranslation(center) * Matrix4.CreateScale(radius);
            PrimitiveManager m = CacheDebugPrimitive(name, solid ? DebugPrimitiveType.SolidSphere : DebugPrimitiveType.WireSphere);
            m.Parameter<GLVec4>(0).Value = color;
            m.Render(mtx, Matrix3.Identity);
        }
        public void RenderAABB(string name, Vec3 halfExtents, Vec3 translation, bool solid, ColorF4 color)
            => RenderBox(name, halfExtents, Matrix4.CreateTranslation(translation), solid, color);
        public void RenderBox(string name, Vec3 halfExtents, Matrix4 transform, bool solid, ColorF4 color)
        {
            PrimitiveManager m = CacheDebugPrimitive(name, solid ? DebugPrimitiveType.SolidBox : DebugPrimitiveType.WireBox);
            m.Parameter<GLVec4>(0).Value = color;
            transform = transform * Matrix4.CreateScale(halfExtents * 2.0f);
            m.Render(transform, transform.Inverted().Transposed().GetRotationMatrix3());
        }
        public void RenderCapsule(Vec3 topPoint, Vec3 bottomPoint, float topRadius, float bottomRadius, bool solid)
        {
            throw new NotImplementedException();
        }
        public void RenderCylinder(Vec3 topPoint, Vec3 bottomPoint, float topRadius, float bottomRadius, bool solid)
        {
            throw new NotImplementedException();
        }
        public void RenderCone(Vec3 topPoint, Vec3 bottomPoint, float bottomRadius, bool solid)
        {
            throw new NotImplementedException();
        }

        private Stack<Rectangle> _renderAreaStack = new Stack<Rectangle>();

        //private static List<DisplayList> _displayLists = new List<DisplayList>();

        public abstract int[] CreateObjects(GenType type, int count);
        public abstract int[] CreateTextures(int target, int count);
        public abstract int[] CreateQueries(int type, int count);
        public T[] CreateObjects<T>(GenType type, int count) where T : BaseRenderState
        {
            return CreateObjects(type, count).Select(x => Activator.CreateInstance(typeof(T), x) as T).ToArray();
        }
        public abstract void DeleteObject(GenType type, int bindingId);
        public abstract void DeleteObjects(GenType type, int[] bindingIds);

        internal int AddActiveMaterial(Material material)
        {
            int id = _activeMaterials.Count;
            _activeMaterials.Add(id, material);
            return id;
        }

        public virtual void BindPrimitiveManager(PrimitiveManager manager)
        {
            _currentPrimitiveManager = manager;
        }
        public void RenderPrimitiveManager(PrimitiveManager manager, bool preservePreviouslyBound = true)
        {
            PrimitiveManager prev = _currentPrimitiveManager;
            BindPrimitiveManager(manager);
            RenderCurrentPrimitiveManager();
            BindPrimitiveManager(preservePreviouslyBound ? prev : null);
        }
        public abstract void RenderCurrentPrimitiveManager();
        public abstract void LinkRenderIndices(PrimitiveManager manager, VertexBuffer indexBuffer);
        public abstract void InitializeBuffer(VertexBuffer buffer);
        public abstract void PushBufferData(VertexBuffer buffer);
        public abstract void MapBufferData(VertexBuffer buffer);
        public abstract void UnmapBufferData(VertexBuffer buffer);

        internal void RemoveActiveMaterial(Material material)
        {
            _activeMaterials.Remove(material.BindingId);
        }

        public abstract void Cull(Culling culling);
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
        public abstract int GenerateProgram(int[] shaderHandles, PrimitiveBufferInfo info);
        public virtual void UseProgram(MeshProgram program)
        {
            _currentMeshProgram = program;
            if (_currentMeshProgram != null)
            {
                _currentMeshProgram?.SetUniforms();
                Scene.SetUniforms();
                Uniform(Models.Materials.Uniform.GetLocation(ECommonUniform.RenderDelta), Engine.RenderDelta);
            }
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
