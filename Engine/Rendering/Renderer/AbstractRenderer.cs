using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering.Cameras;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Worlds.Actors.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace CustomEngine.Rendering
{
    /// <summary>
    /// This class is meant to be overridden with an implementation such as OpenTK or DirectX
    /// </summary>
    public abstract class AbstractRenderer
    {
        public const float DefaultPointSize = 5.0f;
        public const float DefaultLineSize = 5.0f;

        private static Camera _currentCamera;
        public static Camera CurrentCamera
        {
            get => _currentCamera;
            set
            {
                if (_currentCamera != null)
                    _currentCamera._isActive = false;
                _currentCamera = value;
                if (_currentCamera != null)
                    _currentCamera._isActive = true;
            }
        }

        internal SceneProcessor Scene => _scene;
        public abstract RenderLibrary RenderLibrary { get; }
        public RenderContext CurrentContext => RenderContext.Current;
        public Viewport CurrentlyRenderingViewport => Viewport.CurrentlyRendering;

        protected static MeshProgram _currentMeshProgram;
        protected static IPrimitiveManager _currentPrimitiveManager;
        protected static Dictionary<int, Material> _activeMaterials = new Dictionary<int, Material>();
        protected static SceneProcessor _scene = new SceneProcessor();
        protected static Dictionary<string, IPrimitiveManager> _debugPrimitives = new Dictionary<string, IPrimitiveManager>();
        protected static PrivateFontCollection _fonts;
        private Stack<BoundingRectangle> _renderAreaStack = new Stack<BoundingRectangle>();

        public abstract void SetActiveTexture(int unit);

        #region Debug Primitives

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
            WireQuad,
            SolidQuad,
            //WireCapsuleMiddleCylinder,
            //WireCapsuleHalfSphere,
            //SolidCapsuleMiddleCylinder,
            //SolidCapsuleHalfSphere,
            WireCylinder,
            SolidCylinder,
            WireCone,
            SolidCone,
        }

        public IPrimitiveManager CacheDebugPrimitive(string name, DebugPrimitiveType type)
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
                    data = BoundingBox.WireframeMesh(new Vec3(-1.0f), new Vec3(1.0f));
                    break;
                case DebugPrimitiveType.SolidBox:
                    data = BoundingBox.SolidMesh(new Vec3(-1.0f), new Vec3(1.0f));
                    break;
                case DebugPrimitiveType.WireQuad:
                    data = PrimitiveData.FromLineList(new PrimitiveBufferInfo(), VertexQuad.YUpQuad(2.0f).ToLines());
                    break;
                case DebugPrimitiveType.SolidQuad:
                    data = PrimitiveData.FromQuads(Culling.None, new PrimitiveBufferInfo(), VertexQuad.YUpQuad(2.0f));
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

        //public abstract void RenderPoint(Vec3 position, ColorF4 color, float pointSize = DefaultPointSize);
        //public abstract void RenderLine(Vec3 start, Vec3 end, ColorF4 color, float lineWidth = DefaultLineSize);
        //public abstract void RenderLineLoop(bool closedLoop, params Vec3[] points);
        //public abstract void RenderLineLoop(bool closedLoop, PropAnimVec3 points);
        public virtual void RenderPoint(string name, Vec3 position, ColorF4 color, float pointSize = DefaultPointSize)
        {
            SetPointSize(pointSize);
            IPrimitiveManager m = CacheDebugPrimitive(name, DebugPrimitiveType.Point);
            m.Parameter<GLVec4>(0).Value = color;
            m.Render(Matrix4.CreateTranslation(position));
        }
        public virtual unsafe void RenderLine(string name, Vec3 start, Vec3 end, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            IPrimitiveManager m = CacheDebugPrimitive(name, DebugPrimitiveType.Line);
            m.Parameter<GLVec4>(0).Value = color;
            ((Vec3*)m.Data[0].Address)[1] = end - start;
            m.Render(Matrix4.CreateTranslation(start), Matrix3.Identity);
        }
        public virtual void RenderQuad(string name, Vec3 position, Vec3 normal, Vec2 halfExtents, bool solid, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            IPrimitiveManager m = CacheDebugPrimitive(name, solid ? DebugPrimitiveType.SolidQuad : DebugPrimitiveType.WireQuad);
            Quat lookat = Quat.BetweenVectors(Vec3.Up, normal);
            Matrix4 mtx = Matrix4.CreateTranslation(position) * Matrix4.CreateFromQuaternion(lookat) * Matrix4.CreateScale(halfExtents.X, 1.0f, halfExtents.Y);
            m.Render(mtx, mtx.Inverted().Transposed().GetRotationMatrix3());
        }
        public virtual void RenderSphere(string name, Vec3 center, float radius, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            //radius doesn't need to be multiplied by 2.0f; the sphere is already 2.0f in diameter
            Matrix4 mtx = Matrix4.CreateTranslation(center) * Matrix4.CreateScale(radius);
            IPrimitiveManager m = CacheDebugPrimitive(name, solid ? DebugPrimitiveType.SolidSphere : DebugPrimitiveType.WireSphere);
            m.Parameter<GLVec4>(0).Value = color;
            m.Render(mtx, Matrix3.Identity);
        }
        public virtual void RenderAABB(string name, Vec3 halfExtents, Vec3 translation, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
            => RenderBox(name, halfExtents, Matrix4.CreateTranslation(translation), solid, color, lineWidth);
        public virtual void RenderBox(string name, Vec3 halfExtents, Matrix4 transform, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            IPrimitiveManager m = CacheDebugPrimitive(name, solid ? DebugPrimitiveType.SolidBox : DebugPrimitiveType.WireBox);
            m.Parameter<GLVec4>(0).Value = color;
            //halfExtents doesn't need to be multiplied by 2.0f; the box is already 1.0f in each direction of each dimension (2.0f extents)
            transform = transform * Matrix4.CreateScale(halfExtents);
            m.Render(transform, transform.Inverted().Transposed().GetRotationMatrix3());
        }
        public void RenderCapsule(string name, Matrix4 transform, Vec3 localUpAxis, float radius, float halfHeight, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            IPrimitiveManager mCyl = null, mTop = null, mBot = null;
            string cylStr = name + "_CYLINDER";
            string topStr = name + "_TOPHALF";
            string botStr = name + "_BOTTOMHALF";
            if (_debugPrimitives.ContainsKey(cylStr))
                mCyl = _debugPrimitives[cylStr];
            if (_debugPrimitives.ContainsKey(topStr))
                mTop = _debugPrimitives[topStr];
            if (_debugPrimitives.ContainsKey(botStr))
                mBot = _debugPrimitives[botStr];
            if (mCyl == null || mTop == null || mBot == null)
            {
                BaseCapsule.WireframeMeshParts(
                    Vec3.Zero, Vec3.Up, 1.0f, 1.0f, 30, 
                    out PrimitiveData cylData, out PrimitiveData topData, out PrimitiveData botData);
                if (mCyl == null)
                    mCyl = AssignDebugPrimitive(cylStr, new PrimitiveManager(cylData, Material.GetUnlitColorMaterial()));
                if (mTop == null)
                    mTop = AssignDebugPrimitive(topStr, new PrimitiveManager(topData, Material.GetUnlitColorMaterial()));
                if (mBot == null)
                    mBot = AssignDebugPrimitive(botStr, new PrimitiveManager(botData, Material.GetUnlitColorMaterial()));
            }
            Matrix4 axisRotation = Matrix4.CreateFromQuaternion(Quat.BetweenVectors(Vec3.Up, localUpAxis));
            Matrix4 radiusMtx = Matrix4.CreateScale(radius);
            mCyl.Parameter<GLVec4>(0).Value = color;
            mTop.Parameter<GLVec4>(0).Value = color;
            mBot.Parameter<GLVec4>(0).Value = color;
            Matrix4 cylTransform = transform * axisRotation * Matrix4.CreateScale(radius, halfHeight, radius);
            Matrix4 topTransform = transform * axisRotation * Matrix4.CreateTranslation(0.0f, halfHeight, 0.0f) * radiusMtx;
            Matrix4 botTransform = transform * axisRotation * Matrix4.CreateTranslation(0.0f, -halfHeight, 0.0f) * radiusMtx;
            mCyl.Render(cylTransform, Matrix3.Identity);
            mTop.Render(topTransform, Matrix3.Identity);
            mBot.Render(botTransform, Matrix3.Identity);
        }
        public void RenderCylinder(string name, Matrix4 transform, Vec3 localUpAxis, float radius, float halfHeight, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            throw new NotImplementedException();
        }
        public void RenderCone(string name, Matrix4 transform, float radius, float height, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Objects
        public abstract int[] CreateObjects(GenType type, int count);
        public abstract int[] CreateTextures(ETexTarget target, int count);
        public abstract int[] CreateQueries(int type, int count);
        public T[] CreateObjects<T>(GenType type, int count) where T : BaseRenderState
        {
            return CreateObjects(type, count).Select(x => Activator.CreateInstance(typeof(T), x) as T).ToArray();
        }
        public abstract void DeleteObject(GenType type, int bindingId);
        public abstract void DeleteObjects(GenType type, int[] bindingIds);
        #endregion

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

        public virtual void BindPrimitiveManager(IPrimitiveManager manager)
        {
            _currentPrimitiveManager = manager;
        }
        public void RenderPrimitiveManager(IPrimitiveManager manager, bool preservePreviouslyBound = true)
        {
            IPrimitiveManager prev = _currentPrimitiveManager;
            BindPrimitiveManager(manager);
            RenderCurrentPrimitiveManager();
            BindPrimitiveManager(preservePreviouslyBound ? prev : null);
        }
        public abstract void RenderCurrentPrimitiveManager();
        public abstract void LinkRenderIndices(IPrimitiveManager manager, VertexBuffer indexBuffer);
        public abstract void InitializeBuffer(VertexBuffer buffer);
        public abstract void PushBufferData(VertexBuffer buffer);
        public abstract void MapBufferData(VertexBuffer buffer);
        public abstract void UnmapBufferData(VertexBuffer buffer);

        public abstract void Clear(BufferClear mask);
        public abstract void Cull(Culling culling);
        public abstract void SetPointSize(float size);
        public abstract void SetLineSize(float size);
        public abstract int GetStencilIndex(float x, float y);
        public abstract float GetDepth(float x, float y);

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

        //public void UniformMaterial(int matID, string name, params IUniformable4Int[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        //public void UniformMaterial(int matID, string name, params IUniformable4Float[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        //public void UniformMaterial(int matID, string name, params IUniformable3Int[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        //public void UniformMaterial(int matID, string name, params IUniformable3Float[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        //public void UniformMaterial(int matID, string name, params IUniformable2Int[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        //public void UniformMaterial(int matID, string name, params IUniformable2Float[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        //public void UniformMaterial(int matID, string name, params IUniformable1Int[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        //public void UniformMaterial(int matID, string name, params IUniformable1Float[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        //public void UniformMaterial(int matID, string name, params int[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        //public void UniformMaterial(int matID, string name, params float[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        //public void UniformMaterial(int matID, string name, params uint[] p) => UniformMaterial(matID, GetUniformLocation(name), p);
        //public void UniformMaterial(int matID, string name, params double[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        //public void UniformMaterial(int matID, string name, Matrix4 p) => UniformMaterial(matID, GetUniformLocation(name), p);
        //public void UniformMaterial(int matID, string name, Matrix4[] p) => UniformMaterial(matID, GetUniformLocation(name), p);

        //public abstract void UniformMaterial(int matID, int location, params IUniformable4Int[] p);
        //public abstract void UniformMaterial(int matID, int location, params IUniformable4Float[] p);
        //public void UniformMaterial(int matID, int location, params IUniformable4Double[] p) { throw new NotImplementedException(); }
        //public void UniformMaterial(int matID, int location, params IUniformable4UInt[] p) { throw new NotImplementedException(); }
        //public void UniformMaterial(int matID, int location, params IUniformable4Bool[] p) { throw new NotImplementedException(); }

        //public abstract void UniformMaterial(int matID, int location, params IUniformable3Int[] p);
        //public abstract void UniformMaterial(int matID, int location, params IUniformable3Float[] p);
        //public void UniformMaterial(int matID, int location, params IUniformable3Double[] p) { throw new NotImplementedException(); }
        //public void UniformMaterial(int matID, int location, params IUniformable3UInt[] p) { throw new NotImplementedException(); }
        //public void UniformMaterial(int matID, int location, params IUniformable3Bool[] p) { throw new NotImplementedException(); }

        //public abstract void UniformMaterial(int matID, int location, params IUniformable2Int[] p);
        //public abstract void UniformMaterial(int matID, int location, params IUniformable2Float[] p);
        //public void UniformMaterial(int matID, int location, params IUniformable2Double[] p) { throw new NotImplementedException(); }
        //public void UniformMaterial(int matID, int location, params IUniformable2UInt[] p) { throw new NotImplementedException(); }
        //public void UniformMaterial(int matID, int location, params IUniformable2Bool[] p) { throw new NotImplementedException(); }

        //public abstract void UniformMaterial(int matID, int location, params IUniformable1Int[] p);
        //public abstract void UniformMaterial(int matID, int location, params IUniformable1Float[] p);
        //public void UniformMaterial(int matID, int location, params IUniformable1Double[] p) { throw new NotImplementedException(); }
        //public void UniformMaterial(int matID, int location, params IUniformable1UInt[] p) { throw new NotImplementedException(); }
        //public void UniformMaterial(int matID, int location, params IUniformable1Bool[] p) { throw new NotImplementedException(); }

        //public abstract void UniformMaterial(int matID, int location, params int[] p);
        //public abstract void UniformMaterial(int matID, int location, params float[] p);
        //public void UniformMaterial(int matID, int location, params double[] p) { throw new NotImplementedException(); }
        //public void UniformMaterial(int matID, int location, params uint[] p) { throw new NotImplementedException(); }
        //public void UniformMaterial(int matID, int location, params bool[] p) { throw new NotImplementedException(); }

        //public abstract void UniformMaterial(int matID, int location, Matrix4 p);
        //public abstract void UniformMaterial(int matID, int location, params Matrix4[] p);
        //public abstract void UniformMaterial(int matID, int location, Matrix3 p);
        //public abstract void UniformMaterial(int matID, int location, params Matrix3[] p);

        //public void UniformMaterial(int matID, int location, params IUniformable[] p)
        //{
        //    foreach (IUniformable u in p)
        //    {

        //    }
        //}

        #endregion

        #region Render area
        public abstract void CropRenderArea(BoundingRectangle region);
        protected abstract void SetRenderArea(BoundingRectangle region);
        public virtual void PushRenderArea(BoundingRectangle region)
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
        #endregion

        public abstract void TexParameter(ETexTarget texTarget, ETexParamName texParam, float paramData);
        public abstract void TexParameter(ETexTarget texTarget, ETexParamName texParam, int paramData);
        public abstract void PushTextureData(
            ETexTarget texTarget, 
            int mipLevel, 
            EPixelInternalFormat internalFormat,
            int width,
            int height,
            EPixelFormat pixelFormat,
            EPixelType type,
            VoidPtr data);
        public abstract void PushTextureData(
            ETexTarget texTarget,
            int mipLevel,
            EPixelInternalFormat internalFormat,
            int width,
            int height,
            EPixelFormat pixelFormat,
            EPixelType type,
            byte[] data);
        public abstract void BindTexture(ETexTarget texTarget, int bindingId);
        
        //GL.TexImage2D((TextureTarget)textureTargetEnum, mipLevel, (OpenTK.Graphics.OpenGL.PixelInternalFormat)pixelInternalFormatEnum, width, height, 0, (OpenTK.Graphics.OpenGL.PixelFormat)pixelFormatEnum, (PixelType)pixelTypeEnum, data);

        #region Frame Buffers
        public abstract void AttachTextureToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment, int textureBindingId, int mipLevel);
        public abstract void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, ETexTarget texTarget, int textureBindingId, int mipLevel);
        public abstract void SetDrawBuffer(DrawBuffersAttachment attachment);
        public abstract void SetDrawBuffer(int bindingId, DrawBuffersAttachment attachment);
        public abstract void SetDrawBuffers(DrawBuffersAttachment[] attachments);
        public abstract void SetDrawBuffers(int bindingId, DrawBuffersAttachment[] attachments);
        public abstract void SetReadBuffer(DrawBuffersAttachment attachment);
        public abstract void SetReadBuffer(int bindingId, DrawBuffersAttachment attachment);
        public abstract void BindFrameBuffer(EFramebufferTarget type, int bindingId);
        public abstract void BindRenderBuffer(int bindingId);
        public abstract void RenderbufferStorage(ERenderBufferStorage storage, int width, int height);
        public abstract void FramebufferRenderBuffer(EFramebufferTarget target, EFramebufferAttachment attachement, int renderBufferBindingId);
        public abstract void FramebufferRenderBuffer(int frameBufferBindingId, EFramebufferAttachment attachement, int renderBufferBindingId);
        public abstract void BlitFrameBuffer(
          int readBufferId, int writeBufferId,
          int srcX0, int srcY0,
          int srcX1, int srcY1,
          int dstX0, int dstY0,
          int dstX1, int dstY1,
          EClearBufferMask mask,
          EBlitFramebufferFilter filter);
        #endregion

        public abstract void BindTransformFeedback(int bindingId);
        public abstract void BeginTransformFeedback(FeedbackPrimitiveType type);
        public abstract void EndTransformFeedback();
        /// <summary>
        /// Binds a transform feedback buffer to "out" variables in the shader.
        /// </summary>
        public abstract void TransformFeedbackVaryings(int program, string[] varNames);

        public abstract Bitmap GetScreenshot(Rectangle region, bool withTransparency);
    }
    public enum ERenderBufferStorage
    {
        DepthComponent = 6402,
        R3G3B2 = 10768,
        Alpha4 = 32827,
        Alpha8 = 32828,
        Alpha12 = 32829,
        Alpha16 = 32830,
        Rgb4 = 32847,
        Rgb5 = 32848,
        Rgb8 = 32849,
        Rgb10 = 32850,
        Rgb12 = 32851,
        Rgb16 = 32852,
        Rgba2 = 32853,
        Rgba4 = 32854,
        Rgba8 = 32856,
        Rgb10A2 = 32857,
        Rgba12 = 32858,
        Rgba16 = 32859,
        DepthComponent16 = 33189,
        DepthComponent24 = 33190,
        DepthComponent32 = 33191,
        R8 = 33321,
        R16 = 33322,
        Rg8 = 33323,
        Rg16 = 33324,
        R16f = 33325,
        R32f = 33326,
        Rg16f = 33327,
        Rg32f = 33328,
        R8i = 33329,
        R8ui = 33330,
        R16i = 33331,
        R16ui = 33332,
        R32i = 33333,
        R32ui = 33334,
        Rg8i = 33335,
        Rg8ui = 33336,
        Rg16i = 33337,
        Rg16ui = 33338,
        Rg32i = 33339,
        Rg32ui = 33340,
        DepthStencil = 34041,
        Rgba32f = 34836,
        Rgb32f = 34837,
        Rgba16f = 34842,
        Rgb16f = 34843,
        Depth24Stencil8 = 35056,
        R11fG11fB10f = 35898,
        Rgb9E5 = 35901,
        Srgb8 = 35905,
        Srgb8Alpha8 = 35907,
        DepthComponent32f = 36012,
        Depth32fStencil8 = 36013,
        StencilIndex1 = 36166,
        StencilIndex1Ext = 36166,
        StencilIndex4 = 36167,
        StencilIndex4Ext = 36167,
        StencilIndex8 = 36168,
        StencilIndex8Ext = 36168,
        StencilIndex16 = 36169,
        StencilIndex16Ext = 36169,
        Rgba32ui = 36208,
        Rgb32ui = 36209,
        Rgba16ui = 36214,
        Rgb16ui = 36215,
        Rgba8ui = 36220,
        Rgb8ui = 36221,
        Rgba32i = 36226,
        Rgb32i = 36227,
        Rgba16i = 36232,
        Rgb16i = 36233,
        Rgba8i = 36238,
        Rgb8i = 36239,
        Rgb10A2ui = 36975
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
        Color   = 1,
        Depth   = 2,
        Stencil = 4,
        Accum   = 8,
    }
    //public enum DisplayListMode
    //{
    //    //Means this displaylist should not be applied until it is called.
    //    Compile,
    //    //Means this displaylist should be applied while compiling.
    //    CompileAndExecute,
    //}
    public enum EPrimitive
    {
        Points          = 0,
        Lines           = 1,
        LineLoop        = 2,
        LineStrip       = 3,
        Triangles       = 4,
        TriangleStrip   = 5,
        TriangleFan     = 6,
        Quads           = 7,
        QuadStrip       = 8,
    }
}
