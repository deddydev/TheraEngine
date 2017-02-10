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
        private PrimitiveManager
            _wireSphere,
            _wireBox,
            _wireCapsule,
            _wireCylinder,
            _wireCone,
            _wireFrustum,
            _wirePlane;
        private PrimitiveManager 
            _solidSphere,
            _solidBox,
            _solidCapsule,
            _solidCylinder,
            _solidCone,
            _solidFrustum,
            _solidPlane;
        private PrimitiveManager
            _line, _point;

        public void CachePoint()
        {
            _point = new PrimitiveManager(PrimitiveData.FromPoints(Vec3.Zero), Material.GetDefaultMaterial());
        }
        public void CacheLine()
        {
            VertexLine line = new VertexLine(new Vertex(Vec3.Zero), new Vertex(Vec3.Forward));
            _line = new PrimitiveManager(PrimitiveData.FromLines(new PrimitiveBufferInfo() { _hasNormals = false, _texcoordCount = 0 }, line), Material.GetDefaultMaterial());
        }
        public void CacheWireframeSphere()
        {
            _wireSphere = new PrimitiveManager(
                Sphere.WireframeMesh(Vec3.Zero, 1.0f, 10.0f),
                Material.GetDefaultMaterial());
        }
        public void CacheSolidSphere()
        {
            _solidSphere = new PrimitiveManager(
                Sphere.SolidMesh(Vec3.Zero, 1.0f, 10.0f),
                Material.GetDefaultMaterial());
        }
        public void CacheWireframePlane()
        {
            _wirePlane = new PrimitiveManager(
                Plane.WireframeMesh(Vec3.Zero, Rotator.GetZero(), 1.0f, 1.0f),
                Material.GetDefaultMaterial());
        }
        public void CacheSolidPlane()
        {
            _solidPlane = new PrimitiveManager(
                Plane.SolidMesh(Vec3.Zero, Rotator.GetZero(), 1.0f, 1.0f, Culling.None),
                Material.GetDefaultMaterial());
        }

        public void RenderAABB(Vec3 halfExtents, Vec3 translation, bool solid) => RenderBox(halfExtents, Matrix4.CreateTranslation(translation), solid);
        public void RenderCapsule(Vec3 center, Vec3 axis, float topHeight, float topRadius, float bottomHeight, float bottomRadius, Matrix4 transform, bool solid)
        {
            Vec3 normal = axis.GetSafeNormal();
            RenderCapsule(center + normal * topHeight, center - normal * bottomHeight, topRadius, bottomRadius, transform, solid);
        }
        public void RenderCapsule(Vec3 topPoint, Vec3 bottomPoint, float topRadius, float bottomRadius, Matrix4 transform, bool solid)
            => RenderCapsule(Vec3.TransformPosition(topPoint, transform), Vec3.TransformPosition(bottomPoint, transform), topRadius, bottomRadius, solid);
        public void RenderCylinder(Vec3 center, Vec3 axis, float topHeight, float topRadius, float bottomHeight, float bottomRadius, Matrix4 transform, bool solid)
        {
            Vec3 normal = axis.GetSafeNormal();
            RenderCylinder(center + normal * topHeight, center - normal * bottomHeight, topRadius, bottomRadius, transform, solid);
        }
        public void RenderCylinder(Vec3 topPoint, Vec3 bottomPoint, float topRadius, float bottomRadius, Matrix4 transform, bool solid)
            => RenderCylinder(Vec3.TransformPosition(topPoint, transform), Vec3.TransformPosition(bottomPoint, transform), topRadius, bottomRadius, solid);
        public void RenderCone(Vec3 center, Vec3 axis, float topHeight, float bottomHeight, float bottomRadius, Matrix4 transform, bool solid)
        {
            Vec3 normal = axis.GetSafeNormal();
            RenderCone(center + normal * topHeight, center - normal * bottomHeight, bottomRadius, transform, solid);
        }
        public void RenderCone(Vec3 topPoint, Vec3 bottomPoint, float bottomRadius, Matrix4 transform, bool solid)
            => RenderCone(Vec3.TransformPosition(topPoint, transform), Vec3.TransformPosition(bottomPoint, transform), bottomRadius, solid);

        public void RenderLine(Vec3 start, Vec3 end)
        {
            Matrix4 scale = Matrix4.CreateScale(new Vec3((end - start).LengthFast, 1.0f, 1.0f));
            Rotator r = end.LookatAngles(start);
            Matrix4 rotation = r.GetMatrix();
            Matrix4 position = Matrix4.CreateTranslation(start);
            if (_line == null)
                CacheLine();
            _line.Render(position * scale * rotation);
        }
        public void RenderPlane(Vec3 position, Vec3 normal, Vec2 dimensions, bool solid)
        {
            Matrix4 mtx = Matrix4.CreateTranslation(position) * normal.LookatAngles().GetMatrix();
            if (solid)
            {
                if (_solidPlane == null)
                    CacheSolidPlane();
                _solidPlane.Render(mtx, Matrix3.Identity);
            }
            else
            {
                if (_wirePlane == null)
                    CacheWireframePlane();
                _wirePlane.Render(mtx, Matrix3.Identity);
            }
        }
        public void RenderSphere(Vec3 center, float radius, bool solid)
        {
            Matrix4 mtx = Matrix4.CreateTranslation(center) * Matrix4.CreateScale(radius * 0.5f);
            if (solid)
            {
                if (_solidSphere == null)
                    CacheSolidSphere();
                _solidSphere.Render(mtx, Matrix3.Identity);
            }
            else
            {
                if (_wireSphere == null)
                    CacheWireframeSphere();
                _wireSphere.Render(mtx, Matrix3.Identity);
            }
        }
        public void RenderBox(Vec3 halfExtents, Matrix4 transform, bool solid)
        {
            Vec3 scale = halfExtents * 2.0f;
            transform = transform * Matrix4.CreateScale(scale);
            if (solid)
                _solidBox.Render(transform);
            else
                _wireBox.Render(transform);
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
        public void RenderFrustum(Frustum f)
        {
            throw new NotImplementedException();
        }

        protected int _programHandle;
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
            _programHandle = program != null ? program.BindingId : BaseRenderState.NullBindingId;
            if (_programHandle > BaseRenderState.NullBindingId)
            {
                program?.SetUniforms();
                Scene.SetUniforms();
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
