using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// This class is meant to be overridden with an implementation such as OpenTK or DirectX
    /// </summary>
    public abstract class AbstractRenderer
    {
        public const float DefaultPointSize = 5.0f;
        public const float DefaultLineSize = 1.0f;
        
        public Camera CurrentCamera => _cameraStack.Count == 0 ? null : _cameraStack.Peek();
        public Scene2D Current2DScene => _2dSceneStack.Count == 0 ? null : _2dSceneStack.Peek();
        public Scene3D Current3DScene => _3dSceneStack.Count == 0 ? null : _3dSceneStack.Peek();

        /// <summary>
        /// Set this to force every mesh to render with this material.
        /// </summary>
        public TMaterial MaterialOverride { get; set; }
        public abstract ERenderLibrary RenderLibrary { get; }
        public RenderContext CurrentContext => RenderContext.Captured;
        public Viewport CurrentlyRenderingViewport => Viewport.CurrentlyRendering;

        protected IPrimitiveManager _currentPrimitiveManager;
        private Stack<BoundingRectangle> _renderAreaStack = new Stack<BoundingRectangle>();
        private Stack<Camera> _cameraStack = new Stack<Camera>();
        private Stack<Scene2D> _2dSceneStack = new Stack<Scene2D>();
        private Stack<Scene3D> _3dSceneStack = new Stack<Scene3D>();

        #region Push / Pop Current
        internal void PushCurrent3DScene(Scene3D scene)
        {
            _3dSceneStack.Push(scene);
        }
        internal void PopCurrent3DScene()
        {
            _3dSceneStack.Pop();
        }
        internal void PushCurrent2DScene(Scene2D scene)
        {
            _2dSceneStack.Push(scene);
        }
        internal void PopCurrent2DScene()
        {
            _2dSceneStack.Pop();
        }
        internal void PushCamera(Camera camera)
        {
            Camera c = CurrentCamera;

            if (c != null)
                c.IsActiveRenderCamera = false;

            //Pushing a null camera is okay
            _cameraStack.Push(camera);

            if (camera != null)
                camera.IsActiveRenderCamera = true;
        }
        internal void PopCamera()
        {
            Camera c = _cameraStack.Pop();

            if (c != null)
                c.IsActiveRenderCamera = false;

            c = CurrentCamera;

            if (c != null)
                c.IsActiveRenderCamera = true;
        }
        #endregion

        #region Debug Primitives

        //public class DebugPrimitive : I3DRenderable
        //{
        //    private Matrix4 _modelMatrix;
        //    private string _name;
        //    private float _size;
        //    private IPrimitiveManager _manager;
        //    private DebugPrimitiveType _type;
        //    private IOctreeNode _renderNode;
        //    private bool _isRendering;
        //    private ColorF4 _color;

        //    public bool HasTransparency => false;
        //    public Shape CullingVolume => null;

        //    public IOctreeNode OctreeNode { get => _renderNode; set => _renderNode = value; }
        //    public bool IsRendering { get => _isRendering; set => _isRendering = value; }
        //    public DebugPrimitiveType Type { get => _type; set => _type = value; }
        //    public string Name { get => _name; set => _name = value; }
        //    public IPrimitiveManager Manager { get => _manager; set => _manager = value; }
        //    public float Size { get => _size; set => _size = value; }
        //    public ColorF4 Color { get => _color; set => _color = value; }
        //    public Matrix4 ModelMatrix { get => _modelMatrix; set => _modelMatrix = value; }

        //    public void Render()
        //    {
        //        if (_type == DebugPrimitiveType.Point)
        //            Engine.Renderer.SetPointSize(Size);
        //        else if (_type == DebugPrimitiveType.Line)
        //            Engine.Renderer.SetLineSize(Size);
        //        _manager.Parameter<GLVec4>(0).Value = _color;
        //        _manager.Render(ModelMatrix, Matrix3.Identity);
        //    }
        //}

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
            
            WireQuad,
            SolidQuad,
            
            WireCircle,
            SolidCircle,

            WireSphere,
            SolidSphere,

            WireBox,
            SolidBox,


            WireCylinder,
            SolidCylinder,

            WireCone,
            SolidCone,
        }

        protected static Dictionary<string, IPrimitiveManager> _debugPrimitives = new Dictionary<string, IPrimitiveManager>();
        private readonly IPrimitiveManager[] _debugPrims = new IPrimitiveManager[14];

        public IPrimitiveManager GetDebugPrimitive(DebugPrimitiveType type)
        {
            IPrimitiveManager mesh = _debugPrims[(int)type];
            if (mesh != null)
                return mesh;
            else
            {
                TMaterial mat = TMaterial.CreateUnlitColorMaterialForward();
                RenderingParameters p = new RenderingParameters();
                p.DepthTest.Enabled = ERenderParamUsage.Enabled;
                mat.RenderParamsRef = p;
                return _debugPrims[(int)type] = new PrimitiveManager(GetPrimData(type), mat);
            }
        }
        private PrimitiveData GetPrimData(DebugPrimitiveType type)
        {
            switch (type)
            {
                case DebugPrimitiveType.Point:
                    return PrimitiveData.FromPoints(Vec3.Zero);
                case DebugPrimitiveType.Line: VertexLine line = new VertexLine(new Vertex(Vec3.Zero), new Vertex(Vec3.Forward));
                    return PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), line);

                case DebugPrimitiveType.WireSphere: return Sphere.WireframeMesh(Vec3.Zero, 1.0f, 60); //Diameter is set to 2.0f on purpose
                case DebugPrimitiveType.SolidSphere: return Sphere.SolidMesh(Vec3.Zero, 1.0f, 30); //Diameter is set to 2.0f on purpose

                case DebugPrimitiveType.WireBox: return BoundingBox.WireframeMesh(-1.0f, 1.0f);
                case DebugPrimitiveType.SolidBox: return BoundingBox.SolidMesh(-1.0f, 1.0f);

                case DebugPrimitiveType.WireCircle: return Circle3D.WireframeMesh(1.0f, Vec3.UnitY, Vec3.Zero, 20); //Diameter is set to 2.0f on purpose
                case DebugPrimitiveType.SolidCircle: return Circle3D.SolidMesh(1.0f, Vec3.UnitY, Vec3.Zero, 20); //Diameter is set to 2.0f on purpose

                case DebugPrimitiveType.WireQuad: return PrimitiveData.FromLineList(VertexShaderDesc.JustPositions(), VertexQuad.PosYQuad(1.0f, false, false).ToLines());
                case DebugPrimitiveType.SolidQuad: return PrimitiveData.FromQuads(VertexShaderDesc.PosNormTex(), VertexQuad.PosYQuad(1.0f, false, false));

                case DebugPrimitiveType.WireCone: return Cone.WireMesh(Vec3.Zero, Vec3.Forward, 1.0f, 1.0f, 20);
                case DebugPrimitiveType.SolidCone: return Cone.SolidMesh(Vec3.Zero, Vec3.Forward, 1.0f, 1.0f, 20, true);
            }
            return null;
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
        public virtual void RenderPoint(Vec3 position, ColorF4 color, bool depthTestEnabled = true, float pointSize = DefaultPointSize)
        {
            IPrimitiveManager m = GetDebugPrimitive(DebugPrimitiveType.Point);
            m.Parameter<ShaderVec4>(0).Value = color;
            m.Material.RenderParams.DepthTest.Enabled = depthTestEnabled ? ERenderParamUsage.Enabled : ERenderParamUsage.Disabled;
            Matrix4 modelMatrix = Matrix4.CreateTranslation(position);
            //if (Engine.MainThreadID != Thread.CurrentThread.ManagedThreadId)
            //    Engine.Scene.AddDebugPrimitive(new DebugPrimitive() { Manager = m, ModelMatrix = modelMatrix, Color = color, Type = DebugPrimitiveType.Point });
            //else
            //{
                SetPointSize(pointSize);
                m.Render(modelMatrix);
            //}
        }

        public virtual unsafe void RenderLine(Vec3 start, Vec3 end, ColorF4 color, bool depthTestEnabled = true, float lineWidth = DefaultLineSize)
        {
            IPrimitiveManager m = GetDebugPrimitive(DebugPrimitiveType.Line);
            m.Parameter<ShaderVec4>(0).Value = color;
            m.Material.RenderParams.LineWidth = lineWidth;
            m.Material.RenderParams.DepthTest.Enabled = depthTestEnabled ? ERenderParamUsage.Enabled : ERenderParamUsage.Disabled;
            //((Vec3*)m.Data[0].Address)[1] = end - start;
            Matrix4 modelMatrix = Matrix4.CreateTranslation(start) * end.LookatAngles(start).GetMatrix() * Matrix4.CreateScale(end.DistanceTo(start));
            SetLineSize(lineWidth);
            m.Render(modelMatrix, Matrix3.Identity);
        }
        public static readonly Vec3 UIPositionBias = new Vec3(0.0f, 0.0f, 0.1f);
        public static readonly Rotator UIRotation = new Rotator(90.0f, 0.0f, 0.0f, ERotationOrder.YPR);
        public virtual void RenderCircle(Vec3 centerTranslation, Rotator rotation, float radius, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            IPrimitiveManager m = GetDebugPrimitive(solid ? DebugPrimitiveType.SolidCircle : DebugPrimitiveType.WireCircle);
            m.Parameter<ShaderVec4>(0).Value = color;
            Matrix4 mtx = Matrix4.CreateTranslation(centerTranslation) * Matrix4.CreateFromRotator(rotation) * Matrix4.CreateScale(radius, 1.0f, radius);
            m.Render(mtx, mtx.Inverted().Transposed().GetRotationMatrix3());
        }
        public virtual void RenderQuad(Vec3 centerTranslation, Rotator rotation, Vec2 extents, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            IPrimitiveManager m = GetDebugPrimitive(solid ? DebugPrimitiveType.SolidQuad : DebugPrimitiveType.WireQuad);
            m.Parameter<ShaderVec4>(0).Value = color;
            Matrix4 mtx = Matrix4.CreateTranslation(centerTranslation) * Matrix4.CreateFromRotator(rotation) * Matrix4.CreateScale(extents.X, 1.0f, extents.Y);
            m.Render(mtx, mtx.Inverted().Transposed().GetRotationMatrix3());
        }
        public virtual void RenderSphere(Vec3 center, float radius, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            //radius doesn't need to be multiplied by 2.0f; the sphere is already 2.0f in diameter
            Matrix4 mtx = Matrix4.CreateTranslation(center) * Matrix4.CreateScale(radius);
            IPrimitiveManager m = GetDebugPrimitive(solid ? DebugPrimitiveType.SolidSphere : DebugPrimitiveType.WireSphere);
            m.Parameter<ShaderVec4>(0).Value = color;
            m.Render(mtx, mtx.Inverted().Transposed().GetRotationMatrix3());
        }
        
        public virtual void RenderAABB(Vec3 halfExtents, Vec3 translation, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
            => RenderBox(halfExtents, translation.AsTranslationMatrix(), solid, color, lineWidth);
        public virtual void RenderBox(Vec3 halfExtents, Matrix4 transform, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            IPrimitiveManager mesh = GetDebugPrimitive(solid ? DebugPrimitiveType.SolidBox : DebugPrimitiveType.WireBox);
            mesh.Parameter<ShaderVec4>(0).Value = color;
            //halfExtents doesn't need to be multiplied by 2.0f; the box is already 1.0f in each direction of each dimension (2.0f extents)
            transform = transform * halfExtents.AsScaleMatrix();
            mesh.Render(transform, transform.Inverted().Transposed().GetRotationMatrix3());
        }
        public void RenderCapsule(Matrix4 transform, Vec3 localUpAxis, float radius, float halfHeight, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            IPrimitiveManager mCyl = null, mTop = null, mBot = null;
            string cylStr = "_CYLINDER";
            string topStr = "_TOPHALF";
            string botStr = "_BOTTOMHALF";
            if (_debugPrimitives.ContainsKey(cylStr))
                mCyl = _debugPrimitives[cylStr];
            if (_debugPrimitives.ContainsKey(topStr))
                mTop = _debugPrimitives[topStr];
            if (_debugPrimitives.ContainsKey(botStr))
                mBot = _debugPrimitives[botStr];
            if (mCyl == null || mTop == null || mBot == null)
            {
                Capsule.WireframeMeshParts(
                    Vec3.Zero, Vec3.Up, 1.0f, 1.0f, 30, 
                    out PrimitiveData cylData, out PrimitiveData topData, out PrimitiveData botData);
                if (mCyl == null)
                    mCyl = AssignDebugPrimitive(cylStr, new PrimitiveManager(cylData, TMaterial.CreateUnlitColorMaterialForward()));
                if (mTop == null)
                    mTop = AssignDebugPrimitive(topStr, new PrimitiveManager(topData, TMaterial.CreateUnlitColorMaterialForward()));
                if (mBot == null)
                    mBot = AssignDebugPrimitive(botStr, new PrimitiveManager(botData, TMaterial.CreateUnlitColorMaterialForward()));
            }
            Matrix4 axisRotation = Matrix4.CreateFromQuaternion(Quat.BetweenVectors(Vec3.Up, localUpAxis));
            Matrix4 radiusMtx = Matrix4.CreateScale(radius);
            mCyl.Parameter<ShaderVec4>(0).Value = color;
            mTop.Parameter<ShaderVec4>(0).Value = color;
            mBot.Parameter<ShaderVec4>(0).Value = color;
            Matrix4 cylTransform = transform * axisRotation * Matrix4.CreateScale(radius, halfHeight, radius);
            Matrix4 topTransform = transform * axisRotation * Matrix4.CreateTranslation(0.0f, halfHeight, 0.0f) * radiusMtx;
            Matrix4 botTransform = transform * axisRotation * Matrix4.CreateTranslation(0.0f, -halfHeight, 0.0f) * radiusMtx;
            mCyl.Render(cylTransform, Matrix3.Identity);
            mTop.Render(topTransform, Matrix3.Identity);
            mBot.Render(botTransform, Matrix3.Identity);
        }

        public void RenderCylinder(Matrix4 transform, Vec3 localUpAxis, float radius, float halfHeight, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            throw new NotImplementedException();
        }
        public void RenderCone(Matrix4 transform, Vec3 localUpAxis, float radius, float height, bool solid, ColorF4 color, float lineWidth = DefaultLineSize)
        {
            SetLineSize(lineWidth);
            IPrimitiveManager m = GetDebugPrimitive(solid ? DebugPrimitiveType.SolidCone : DebugPrimitiveType.WireCone);
            m.Parameter<ShaderVec4>(0).Value = color;
            transform = transform * localUpAxis.LookatAngles().GetMatrix() * Matrix4.CreateScale(radius, radius, height);
            m.Render(transform, Matrix3.Identity);
        }

        #endregion

        #region Objects

        public abstract int[] CreateObjects(EObjectType type, int count);
        public int CreateObject(EObjectType type) => CreateObjects(type, 1)[0];

        public abstract int[] CreateTextures(ETexTarget target, int count);
        public int CreateTexture(ETexTarget target) => CreateTextures(target, 1)[0];
        
        public abstract int[] CreateQueries(EQueryTarget type, int count);
        public int CreateQuery(EQueryTarget type) => CreateQueries(type, 1)[0];

        public T[] CreateObjects<T>(EObjectType type, int count) where T : BaseRenderObject
            => CreateObjects(type, count).Select(x => Activator.CreateInstance(typeof(T), x) as T).ToArray();

        public abstract void DeleteObject(EObjectType type, int bindingId);
        public abstract void DeleteObjects(EObjectType type, int[] bindingIds);

        #endregion

        #region Primitive Management
        public virtual void BindPrimitiveManager(IPrimitiveManager manager)
        {
            _currentPrimitiveManager = manager;
        }
        public void RenderPrimitiveManager(IPrimitiveManager manager, bool preservePreviouslyBound = true, int instances = 1)
        {
            IPrimitiveManager prev = _currentPrimitiveManager;
            BindPrimitiveManager(manager);
            RenderCurrentPrimitiveManager(instances);
            BindPrimitiveManager(preservePreviouslyBound ? prev : null);
        }
        public abstract void RenderCurrentPrimitiveManager(int instances);
        #endregion

        #region Data Buffers
        public abstract void LinkRenderIndices(IPrimitiveManager manager, DataBuffer indexBuffer);
        public abstract void BindBufferBase(EBufferRangeTarget rangeTarget, int blockIndex, int bufferBindingId);
        public abstract void InitializeBuffer(DataBuffer buffer);
        public abstract void PushBufferData(DataBuffer buffer);
        public abstract void PushBufferSubData(DataBuffer buffer, int offset, int length);
        public abstract void MapBufferData(DataBuffer buffer);
        public abstract void UnmapBufferData(DataBuffer buffer);
        public abstract void AttributeDivisor(int attributeLocation, int divisor);
        #endregion

        public abstract int GetUniformBlockIndex(int programBindingId, string name);

        public abstract void ClearColor(ColorF4 color);
        public abstract void Clear(EFBOTextureType mask);
        public abstract void SetFaceCulling(ECulling culling);
        public abstract void SetPointSize(float size);
        public abstract void SetLineSize(float size);

        public abstract byte GetStencilIndex(float x, float y);
        public abstract float GetDepth(float x, float y);

        public void ColorMask(BoolVec4 rgba) => ColorMask(rgba.X, rgba.Y, rgba.Z, rgba.W);
        public abstract void ColorMask(bool r, bool g, bool b, bool a);


        [Conditional("DEBUG")]
        public abstract void CheckErrors();

        public abstract Bitmap GetScreenshot(Rectangle region, bool withTransparency);
        
        public abstract void BeginConditionalRender(RenderQuery query, EConditionalRenderType type);
        public abstract void EndConditionalRender();

        #region Synchronization
        public abstract void MemoryBarrier(EMemoryBarrierFlags flags);
        public abstract void MemoryBarrierByRegion(EMemoryBarrierRegionFlags flags);
        public abstract EWaitSyncStatus ClientWaitSync(IntPtr sync, long timeout);
        public abstract void WaitSync(IntPtr sync, long timeout);
        public abstract IntPtr FenceSync();
        #endregion

        #region Shaders

        public abstract void SetBindFragDataLocation(int bindingId, int location, string name);
        public abstract void SetShaderMode(EGLSLType type);
        public abstract void SetShaderSource(int bindingId, params string[] sources);
        public abstract bool CompileShader(int bindingId, out string info);
        public abstract int GenerateProgram(bool separable);
        public abstract void AttachShader(int shaderBindingId, int programBindingId);
        public abstract void DetachShader(int shaderBindingId, int programBindingId);
        public abstract bool LinkProgram(int bindingId, out string info);
        public abstract void ActiveShaderProgram(int pipelineBindingId, int programBindingId);
        public abstract void SetProgramParameter(int programBindingId, EProgParam parameter, int value);
        public abstract void UseProgram(int programBindingId);

        public abstract void BindPipeline(int pipelineBindingId);
        public abstract void SetPipelineStage(int pipelineBindingId, EProgramStageMask mask, int programBindingId);

        public abstract void DispatchCompute(int numGroupsX, int numGroupsY, int numGroupsZ);
        public abstract void DispatchComputeIndirect(int offset);

        /// <summary>
        /// Sets various render parameters before rendering a mesh such as culling and blending.
        /// </summary>
        public abstract void ApplyRenderParams(RenderingParameters r);
        
        /// <summary>
        /// Retrieves the attribute location from the program.
        /// Caches if found.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetAttribLocation(RenderProgram program, string name)
            => program.GetAttributeLocation(name);
        /// <summary>
        /// Retrieves the attribute location from the program.
        /// </summary>
        /// <param name="programBindingId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal abstract int OnGetAttribLocation(int programBindingId, string name);

        /// <summary>
        /// Retrieves the uniform location from the program.
        /// Caches if found.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        //public int GetUniformLocation(RenderProgram program, string name)
        //    => program.GetCachedUniformLocation(name);
        /// <summary>
        /// Retrieves the uniform location from the program.
        /// </summary>
        internal abstract int OnGetUniformLocation(int programBindingId, string name);
        public abstract void UniformBlockBinding(int program, int uniformBlockIndex, int uniformBlockBinding);
        //public void Uniform(string name, params IUniformable4Int[] p) => Uniform(GetUniformLocation(name), p);
        //public void Uniform(string name, params IUniformable4Float[] p) => Uniform(GetUniformLocation(name), p);

        //public void Uniform(string name, params IUniformable3Int[] p) => Uniform(GetUniformLocation(name), p);
        //public void Uniform(string name, params IUniformable3Float[] p) => Uniform(GetUniformLocation(name), p);

        //public void Uniform(string name, params IUniformable2Int[] p) => Uniform(GetUniformLocation(name), p);
        //public void Uniform(string name, params IUniformable2Float[] p) => Uniform(GetUniformLocation(name), p);

        //public void Uniform(string name, params IUniformable1Int[] p) => Uniform(GetUniformLocation(name), p);
        //public void Uniform(string name, params IUniformable1Float[] p) => Uniform(GetUniformLocation(name), p);

        //public void Uniform(string name, params int[] p) => Uniform(GetUniformLocation(name), p);
        //public void Uniform(string name, params float[] p) => Uniform(GetUniformLocation(name), p);
        //public void Uniform(string name, params uint[] p) => Uniform(GetUniformLocation(name), p);
        //public void Uniform(string name, params double[] p) => Uniform(GetUniformLocation(name), p);

        //public void Uniform(string name, Matrix4 p) => Uniform(GetUniformLocation(name), p);
        //public void Uniform(string name, Matrix4[] p) => Uniform(GetUniformLocation(name), p);
        //public void Uniform(string name, Matrix3 p) => Uniform(GetUniformLocation(name), p);
        //public void Uniform(string name, Matrix3[] p) => Uniform(GetUniformLocation(name), p);

        //public abstract void Uniform(int location, params IUniformable4Int[] p);
        //public abstract void Uniform(int location, params IUniformable4Float[] p);
        //public void Uniform(int location, params IUniformable4Double[] p) { throw new NotImplementedException(); }
        //public void Uniform(int location, params IUniformable4UInt[] p) { throw new NotImplementedException(); }
        //public void Uniform(int location, params IUniformable4Bool[] p) { throw new NotImplementedException(); }

        //public abstract void Uniform(int location, params IUniformable3Int[] p);
        //public abstract void Uniform(int location, params IUniformable3Float[] p);
        //public void Uniform(int location, params IUniformable3Double[] p) { throw new NotImplementedException(); }
        //public void Uniform(int location, params IUniformable3UInt[] p) { throw new NotImplementedException(); }
        //public void Uniform(int location, params IUniformable3Bool[] p) { throw new NotImplementedException(); }

        //public abstract void Uniform(int location, params IUniformable2Int[] p);
        //public abstract void Uniform(int location, params IUniformable2Float[] p);
        //public void Uniform(int location, params IUniformable2Double[] p) { throw new NotImplementedException(); }
        //public void Uniform(int location, params IUniformable2UInt[] p) { throw new NotImplementedException(); }
        //public void Uniform(int location, params IUniformable2Bool[] p) { throw new NotImplementedException(); }

        //public abstract void Uniform(int location, params IUniformable1Int[] p);
        //public abstract void Uniform(int location, params IUniformable1Float[] p);
        //public void Uniform(int location, params IUniformable1Double[] p) { throw new NotImplementedException(); }
        //public void Uniform(int location, params IUniformable1UInt[] p) { throw new NotImplementedException(); }
        //public void Uniform(int location, params IUniformable1Bool[] p) { throw new NotImplementedException(); }

        //public abstract void Uniform(int location, params int[] p);
        //public abstract void Uniform(int location, params float[] p);
        //public void Uniform(int location, params double[] p) { throw new NotImplementedException(); }
        //public void Uniform(int location, params uint[] p) { throw new NotImplementedException(); }
        //public void Uniform(int location, params bool[] p) { throw new NotImplementedException(); }

        //public abstract void Uniform(int location, Matrix4 p);
        //public abstract void Uniform(int location, params Matrix4[] p);
        //public abstract void Uniform(int location, Matrix3 p);
        //public abstract void Uniform(int location, params Matrix3[] p);

        //TODO: cache GetUniformLocation results and don't call again.
        //Only call GetUniformLocation after a program is compiled
        //or after a shader variable name changes 

        //public void Uniform(RenderProgram program, string name, params IUniformable4Int[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);
        //public void Uniform(RenderProgram program, string name, params IUniformable4Float[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);

        //public void Uniform(RenderProgram program, string name, params IUniformable3Int[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);
        //public void Uniform(RenderProgram program, string name, params IUniformable3Float[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);

        //public void Uniform(RenderProgram program, string name, params IUniformable2Int[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);
        //public void Uniform(RenderProgram program, string name, params IUniformable2Float[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);

        //public void Uniform(RenderProgram program, string name, params IUniformable1Int[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);
        //public void Uniform(RenderProgram program, string name, params IUniformable1Float[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);

        //public void Uniform(RenderProgram program, string name, params int[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);
        //public void Uniform(RenderProgram program, string name, params float[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);
        //public void Uniform(RenderProgram program, string name, params uint[] p)
        //    => ProgramUniform(program, GetUniformLocation(program, name), p);
        //public void Uniform(RenderProgram program, string name, params double[] p)
        //    => ProgramUniform(program, GetUniformLocation(program, name), p);

        //public void Uniform(RenderProgram program, string name, Matrix4 p)
        //    => Uniform(program, GetUniformLocation(program, name), p);
        //public void Uniform(RenderProgram program, string name, Matrix4[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);
        //public void Uniform(RenderProgram program, string name, Matrix3 p)
        //    => Uniform(program, GetUniformLocation(program, name), p);
        //public void Uniform(RenderProgram program, string name, Matrix3[] p)
        //    => Uniform(program, GetUniformLocation(program, name), p);

        internal abstract void Uniform(int programBindingId, int location, params IUniformable4Int[] p);
        internal abstract void Uniform(int programBindingId, int location, params IUniformable4Float[] p);
        internal abstract void Uniform(int programBindingId, int location, params IUniformable4Double[] p);
        internal abstract void Uniform(int programBindingId, int location, params IUniformable4UInt[] p);
        internal abstract void Uniform(int programBindingId, int location, params IUniformable4Bool[] p);

        internal abstract void Uniform(int programBindingId, int location, params IUniformable3Int[] p);
        internal abstract void Uniform(int programBindingId, int location, params IUniformable3Float[] p);
        internal void Uniform(int programBindingId, int location, params IUniformable3Double[] p) { throw new NotImplementedException(); }
        internal void Uniform(int programBindingId, int location, params IUniformable3UInt[] p) { throw new NotImplementedException(); }
        internal void Uniform(int programBindingId, int location, params IUniformable3Bool[] p) { throw new NotImplementedException(); }

        internal abstract void Uniform(int programBindingId, int location, params IUniformable2Int[] p);
        internal abstract void Uniform(int programBindingId, int location, params IUniformable2Float[] p);
        internal void Uniform(int programBindingId, int location, params IUniformable2Double[] p) { throw new NotImplementedException(); }
        internal void Uniform(int programBindingId, int location, params IUniformable2UInt[] p) { throw new NotImplementedException(); }
        internal void Uniform(int programBindingId, int location, params IUniformable2Bool[] p) { throw new NotImplementedException(); }

        internal abstract void Uniform(int programBindingId, int location, params IUniformable1Int[] p);
        internal abstract void Uniform(int programBindingId, int location, params IUniformable1Float[] p);
        internal void Uniform(int programBindingId, int location, params IUniformable1Double[] p) { throw new NotImplementedException(); }
        internal void Uniform(int programBindingId, int location, params IUniformable1UInt[] p) { throw new NotImplementedException(); }
        internal void Uniform(int programBindingId, int location, params IUniformable1Bool[] p) { throw new NotImplementedException(); }

        internal abstract void Uniform(int programBindingId, int location, params int[] p);
        internal abstract void Uniform(int programBindingId, int location, params float[] p);
        internal void Uniform(int programBindingId, int location, params double[] p) { throw new NotImplementedException(); }
        internal void Uniform(int programBindingId, int location, params uint[] p) { throw new NotImplementedException(); }
        internal void Uniform(int programBindingId, int location, params bool[] p) { throw new NotImplementedException(); }

        internal abstract void Uniform(int programBindingId, int location, Matrix4 p);
        internal abstract void Uniform(int programBindingId, int location, params Matrix4[] p);
        internal abstract void Uniform(int programBindingId, int location, Matrix3 p);
        internal abstract void Uniform(int programBindingId, int location, params Matrix3[] p);
        
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

        #region Textures
        public abstract void SetMipmapParams(int bindingId, int minLOD, int maxLOD, int largestMipmapLevel, int smallestAllowedMipmapLevel);
        public abstract void SetMipmapParams(ETexTarget target, int minLOD, int maxLOD, int largestMipmapLevel, int smallestAllowedMipmapLevel);
        public abstract void GenerateMipmap(int bindingId);
        public abstract void GenerateMipmap(ETexTarget target);
        public abstract void ClearTexImage(int bindingId, int level, EPixelFormat format, EPixelType type, VoidPtr clearColor);
        public void ClearTexImage(int bindingId, int level, ColorF4 color) => ClearTexImage(bindingId, level, EPixelFormat.Rgba, EPixelType.Float, color.Address);
        public void ClearTexImage(int bindingId, int level, ColorF3 color) => ClearTexImage(bindingId, level, EPixelFormat.Rgb, EPixelType.Float, color.Address);
        public void ClearTexImage(int bindingId, int level, RGBAPixel color) => ClearTexImage(bindingId, level, EPixelFormat.Rgba, EPixelType.Byte, color.Address);
        public abstract void SetActiveTexture(int unit);
        public abstract void GetTexImage<T>(ETexTarget target, int level, EPixelFormat pixelFormat, EPixelType pixelType, T[] pixels) where T : unmanaged;
        public abstract void GetTexImage(ETexTarget target, int level, EPixelFormat pixelFormat, EPixelType pixelType, IntPtr pixels);
        public abstract void GetTexImage<T>(int textureBindingId, int level, EPixelFormat pixelFormat, EPixelType pixelType, T[] pixels) where T : unmanaged;
        public abstract void GetTexImage(int textureBindingId, int level, EPixelFormat pixelFormat, EPixelType pixelType, int bufSize, IntPtr pixels);
        public abstract void TexParameter(ETexTarget texTarget, ETexParamName texParam, float paramData);
        public abstract void TexParameter(ETexTarget texTarget, ETexParamName texParam, int paramData);
        public abstract void TexParameter(int texBindingId, ETexParamName texParam, float paramData);
        public abstract void TexParameter(int texBindingId, ETexParamName texParam, int paramData);
        public abstract void PushTextureData(
            ETexTarget texTarget, 
            int mipLevel, 
            EPixelInternalFormat internalFormat,
            int width,
            int height,
            EPixelFormat pixelFormat,
            EPixelType type,
            VoidPtr data);
        public abstract void PushTextureData<T>(
            ETexTarget texTarget,
            int mipLevel,
            EPixelInternalFormat internalFormat,
            int width,
            int height,
            EPixelFormat pixelFormat,
            EPixelType type,
            T[] data) where T : struct;
        public abstract void PushTextureSubData(
            ETexTarget texTarget,
            int mipLevel,
            int xOffset,
            int yOffset,
            int width,
            int height,
            EPixelFormat format,
            EPixelType type,
            VoidPtr data);
        public abstract void PushTextureSubData<T>(
            ETexTarget texTarget,
            int mipLevel,
            int xOffset,
            int yOffset,
            int width,
            int height,
            EPixelFormat format,
            EPixelType type,
            T[] data) where T : struct;
        public abstract void SetTextureStorage(
            ETexTarget2D texTarget,
            int mipLevels,
            ESizedInternalFormat internalFormat,
            int width,
            int height);
        public abstract void SetTextureStorage(
            int bindingId,
            int mipLevels,
            ESizedInternalFormat internalFormat,
            int width,
            int height);
        public abstract void BindTexture(ETexTarget texTarget, int bindingId);
        public abstract void TextureView(int bindingId, ETexTarget target, int origTextureId, EPixelInternalFormat fmt, int minLevel, int numLevels, int minLayer, int numLayers);
        #endregion

        #region Frame Buffers

        public abstract void BindFrameBuffer(EFramebufferTarget type, int bindingId);
        public abstract void CheckFrameBufferErrors();
        public abstract void AttachTextureToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment, int textureBindingId, int mipLevel);
        public abstract void AttachTextureToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment, int textureBindingId, int mipLevel, int layer);
        public abstract void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, ETexTarget texTarget, int textureBindingId, int mipLevel);
        public abstract void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, int textureBindingId, int mipLevel);
        public abstract void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, int textureBindingId, int mipLevel, int layer);

        public abstract void SetDrawBuffer(EDrawBuffersAttachment attachment);
        public abstract void SetDrawBuffer(int bindingId, EDrawBuffersAttachment attachment);
        public abstract void SetDrawBuffers(EDrawBuffersAttachment[] attachments);
        public abstract void SetDrawBuffers(int bindingId, EDrawBuffersAttachment[] attachments);

        public abstract void SetReadBuffer(EDrawBuffersAttachment attachment);
        public abstract void SetReadBuffer(int bindingId, EDrawBuffersAttachment attachment);
        
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
        public abstract void BlitFrameBuffer(
          int srcX0, int srcY0,
          int srcX1, int srcY1,
          int dstX0, int dstY0,
          int dstX1, int dstY1,
          EClearBufferMask mask,
          EBlitFramebufferFilter filter);
        #endregion

        #region Transform Feedback
        public abstract void BindTransformFeedback(int bindingId);
        public abstract void BeginTransformFeedback(FeedbackPrimitiveType type);
        public abstract void EndTransformFeedback();
        /// <summary>
        /// Binds a transform feedback buffer to "out" variables in the shader.
        /// </summary>
        public abstract void TransformFeedbackVaryings(int program, string[] varNames);
        #endregion

        #region Queries
        public abstract void BeginQuery(int bindingId, EQueryTarget target);
        public abstract void EndQuery(EQueryTarget target);
        public abstract void QueryCounter(int bindingId);
        public abstract int GetQueryObjectInt(int bindingId, EGetQueryObject obj);
        public abstract long GetQueryObjectLong(int bindingId, EGetQueryObject obj);
        #endregion

        #region Blending
        public abstract void BlendColor(ColorF4 color);
        public abstract void BlendFunc(EBlendingFactor srcFactor, EBlendingFactor destFactor);
        public abstract void BlendFuncSeparate(EBlendingFactor srcFactorRGB, EBlendingFactor destFactorRGB, EBlendingFactor srcFactorAlpha, EBlendingFactor destFactorAlpha);
        public abstract void BlendEquation(EBlendEquationMode rgb, EBlendEquationMode alpha);
        public abstract void BlendEquationSeparate(EBlendEquationMode rgb, EBlendEquationMode alpha);
        #endregion

        #region Depth Test
        public abstract void EnableDepthTest(bool enabled);
        public abstract void ClearDepth(float defaultDepth);
        public abstract void AllowDepthWrite(bool allow);
        public abstract void DepthFunc(EComparison func);
        public abstract void DepthRange(double near, double far);
        #endregion

        #region Stencil
        public abstract void ClearStencil(int value);
        public abstract void StencilMask(int value);
        public abstract void StencilOp(EStencilOp fail, EStencilOp zFail, EStencilOp zPass);
        #endregion
    }
}
