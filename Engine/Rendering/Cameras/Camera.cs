using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Cameras
{
    public interface ICamera : IFileObject, I3DRenderable
    {
        event OwningComponentChange OwningComponentChanged;
        event Action TransformChanged;
        event Action ProjectionChanged;

        float Width { get; }
        float Height { get; }
        float NearZ { get; set; }
        float FarZ { get; set; }

        Matrix4 ProjectionMatrix { get; }
        Matrix4 InverseProjectionMatrix { get; }
        Matrix4 CameraToWorldSpaceMatrix { get; }
        Matrix4 WorldToCameraSpaceMatrix { get; }
        Matrix4 ComponentToCameraSpaceMatrix { get; }
        Matrix4 CameraToComponentSpaceMatrix { get; set; }
        Matrix4 WorldToCameraProjSpaceMatrix { get; }
        Matrix4 CameraProjToWorldSpaceMatrix { get; }

        Vec3 WorldPoint { get; }

        IFrustum Frustum { get; }
        IFrustum UntransformedFrustum { get; }

        Vec2 Origin { get; }
        Vec2 Dimensions { get; }

        List<Viewport> Viewports { get; set; }
        bool IsActiveRenderCamera { get; set; }

        SceneComponent OwningComponent { get; set; }

        Vec3 WorldToScreen(Vec3 worldPoint);
        void WorldToScreen(Vec3 worldPoint, out Vec2 screenPoint, out float depth);
        void WorldToScreen(Vec3 worldPoint, out float x, out float y, out float depth);

        Vec3 ScreenToWorld(Vec2 screenPoint, float depth);
        Vec3 ScreenToWorld(float x, float y, float depth);
        Vec3 ScreenToWorld(Vec3 screenPoint);

        Vec3 RotateVector(Vec3 dir);
        void RebaseOrigin(Vec3 newOrigin);
        Vec3 RotateVectorInverse(Vec3 dir);

        Vec3 RightVector { get; }
        Vec3 UpVector { get; }
        Vec3 ForwardVector { get; }

        void SetUniforms(RenderProgram program);
        Segment GetWorldSegment(Vec2 screenPoint);
        Ray GetWorldRay(Vec2 screenPoint);
        Vec3 GetPointAtDepth(Vec2 screenPoint, float depth);
        Vec3 GetPointAtDistance(Vec2 screenPoint, float distance);

        void Render(bool shadowPass);
        Plane GetScreenPlane();
        Vec3 GetScreenPlaneOriginDistance();
        float DistanceFromScreenPlane(Vec3 point);
        Vec3 ClosestPointOnScreenPlane(Vec3 point);
        float DistanceFromWorldPoint(Vec3 point);
        float DistanceFromWorldPointFast(Vec3 point);
        float DistanceScale(Vec3 point, float radius = 1.0f);

        void Resize(float width, float height);
        void SetBloomUniforms(RenderProgram program);
        void SetAmbientOcclusionUniforms(RenderProgram program);
        void SetPostProcessUniforms(RenderProgram program);

        bool UsesAutoExposure { get; }
    }
    public delegate void OwningComponentChange(SceneComponent previous, SceneComponent current);
    public abstract class Camera : TFileObject, ICamera
    {
        protected Camera()
        {
            _renderCommand = new RenderCommandMethod3D(ERenderPass.OpaqueForward, Render);
            _transformedFrustum = new Frustum();
        }

        public event OwningComponentChange OwningComponentChanged;
        public event Action TransformChanged;
        public event Action ProjectionChanged;

        public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D(false, true);

        public abstract float NearZ { get; set; }
        public abstract float FarZ { get; set; }

        [Browsable(false)]
        public Matrix4 ProjectionMatrix => _projectionMatrix;
        [Browsable(false)]
        public Matrix4 InverseProjectionMatrix => _projectionInverse;

        /// <summary>
        /// Transformation from the space relative to the camera to space relative to the world.
        /// Also can be considered the camera's world transform.
        /// </summary>
        [Browsable(false)]
        public Matrix4 CameraToWorldSpaceMatrix => _owningComponent?.WorldMatrix ?? _cameraToWorldSpaceMatrix;

        /// <summary>
        /// Transformation from the space relative to the world to space relative to the camera.
        /// </summary>
        [Browsable(false)]
        public Matrix4 WorldToCameraSpaceMatrix => _owningComponent?.InverseWorldMatrix ?? _worldToCameraSpaceMatrix;

        [Browsable(false)]
        public Matrix4 ComponentToCameraSpaceMatrix => _worldToCameraSpaceMatrix;
        [Browsable(false)]
        public virtual Matrix4 CameraToComponentSpaceMatrix
        {
            get => _cameraToWorldSpaceMatrix;
            set
            {
                _cameraToWorldSpaceMatrix = value;
                _worldToCameraSpaceMatrix = _cameraToWorldSpaceMatrix.Inverted();
                OnTransformChanged();
            }
        }

        [Browsable(false)]
        [Category("Camera")]
        public virtual Vec3 WorldPoint => _owningComponent?.WorldMatrix.Translation ?? Vec3.Zero;
        
        [Browsable(false)]
        public IFrustum Frustum => _transformedFrustum;
        [Browsable(false)]
        public IFrustum UntransformedFrustum => _untransformedFrustum;

        public abstract float Width { get; }
        public abstract float Height { get; }
        [Browsable(false)]
        public abstract Vec2 Origin { get; }
        [Browsable(false)]
        public Vec2 Dimensions => new Vec2(Width, Height);

        [Browsable(false)]
        public List<Viewport> Viewports { get; set; } = new List<Viewport>();
        [Browsable(false)]
        public bool IsActiveRenderCamera { get; set; } = false;

        [Browsable(false)]
        public virtual SceneComponent OwningComponent
        {
            get => _owningComponent;
            set
            {
                SceneComponent prev = _owningComponent;
                if (_owningComponent != null)
                    _owningComponent.WorldTransformChanged -= _owningComponent_WorldTransformChanged;
                _owningComponent = value;
                if (_owningComponent != null)
                    _owningComponent.WorldTransformChanged += _owningComponent_WorldTransformChanged;
                OwningComponentChanged?.Invoke(prev, _owningComponent);
                UpdateTransformedFrustum();
            }
        }
        
        protected SceneComponent _owningComponent;
        internal Vec3 _projectionRange;
        internal Vec3 _projectionOrigin;
        protected IFrustum _untransformedFrustum, _transformedFrustum;
        protected bool _updating = false;
        protected Matrix4
            _projectionMatrix = Matrix4.Identity,
            _projectionInverse = Matrix4.Identity,
            _cameraToWorldSpaceMatrix = Matrix4.Identity,
            _worldToCameraSpaceMatrix = Matrix4.Identity,
            _worldToCameraProjSpaceMatrix = Matrix4.Identity,
            _cameraProjToWorldSpaceMatrix = Matrix4.Identity;
        protected bool _matrixInvalidated = false;
        
        private void _owningComponent_WorldTransformChanged(ISceneComponent comp)
        {
            //_forwardInvalidated = true;
            //_upInvalidated = true;
            //_rightInvalidated = true;
            UpdateTransformedFrustum();
            if (!_updating)
                OnTransformChanged();
        }

        public void WorldToScreen(Vec3 worldPoint, out Vec2 screenPoint, out float depth)
        {
            Vec3 xyd = WorldToScreen(worldPoint);
            screenPoint = xyd.Xy;
            depth = xyd.Z;
        }
        public void WorldToScreen(Vec3 worldPoint, out float x, out float y, out float depth)
        {
            Vec3 xyd = WorldToScreen(worldPoint);
            x = xyd.X;
            y = xyd.Y;
            depth = xyd.Z;
        }
        /// <summary>
        /// Returns an X, Y coordinate relative to the camera's Origin,
        /// with Z being the normalized depth (0.0f - 1.0f) from NearDepth (0.0f) to FarDepth (1.0f).
        /// </summary>
        public Vec3 WorldToScreen(Vec3 point) 
            => _projectionRange * (((point * WorldToCameraProjSpaceMatrix) + Vec3.One) * Vec3.Half);
        /// <summary>
        /// Takes an X, Y coordinate relative to the camera's Origin along with the normalized depth (0.0f - 1.0f) from NearDepth (0.0f) to FarDepth (1.0f), and returns a position in the world.
        /// </summary>
        public Vec3 ScreenToWorld(Vec2 point, float depth)
            => ScreenToWorld(point.X, point.Y, depth);
        /// <summary>
        /// Takes an X, Y coordinate relative to the camera's Origin along with the normalized depth (0.0f - 1.0f) from NearDepth (0.0f) to FarDepth (1.0f), and returns a position in the world.
        /// </summary>
        public Vec3 ScreenToWorld(float x, float y, float depth)
            => ScreenToWorld(new Vec3(x, y, depth));
        /// <summary>
        /// Takes an X, Y coordinate relative to the camera's Origin, with Z being the normalized depth (0.0f - 1.0f) from NearDepth (0.0f) to FarDepth (1.0f), and returns a position in the world.
        /// </summary>
        public Vec3 ScreenToWorld(Vec3 screenPoint)
            => ((screenPoint / _projectionRange) / Vec3.Half - Vec3.One) * CameraProjToWorldSpaceMatrix;

        protected virtual void UpdateTransformedFrustum()
            => _transformedFrustum.TransformedVersionOf(_untransformedFrustum ?? (_untransformedFrustum = CreateUntransformedFrustum()), CameraToWorldSpaceMatrix);
        
        /// <summary>
        /// Rotates the given vector by the camera's rotation. Does not normalize the returned vector.
        /// For example, if given world-space forward, will return camera-space forward.
        /// </summary>
        /// <param name="dir">The vector to rotate.</param>
        /// <returns>The rotated vector. Not normalized.</returns>
        public Vec3 RotateVector(Vec3 dir)
            => Vec3.TransformVector(dir, CameraToWorldSpaceMatrix);

        public abstract void RebaseOrigin(Vec3 newOrigin);

        /// <summary>
        /// Rotates the given vector by the camera's rotation. Does not normalize the returned vector.
        /// For example, if given world-space forward, will return camera-space forward.
        /// </summary>
        /// <param name="dir">The vector to rotate.</param>
        /// <returns>The rotated vector. Not normalized.</returns>
        public Vec3 RotateVectorInverse(Vec3 dir)
            => Vec3.TransformVector(dir, CameraToWorldSpaceMatrix);

        /// <summary>
        /// Returns the right direction of the camera in world space.
        /// </summary>
        [Browsable(false)]
        public Vec3 RightVector => CameraToWorldSpaceMatrix.RightVec;
        /// <summary>
        /// Returns the up direction of the camera in world space.
        /// </summary>
        [Browsable(false)]
        public Vec3 UpVector => CameraToWorldSpaceMatrix.UpVec;
        /// <summary>
        /// Returns the forward direction of the camera in world space.
        /// </summary>
        [Browsable(false)]
        public Vec3 ForwardVector => -CameraToWorldSpaceMatrix.ForwardVec;

        [Browsable(false)]
        public Matrix4 WorldToCameraProjSpaceMatrix
        {
            get
            {
                if (_matrixInvalidated)
                {
                    _worldToCameraProjSpaceMatrix = ProjectionMatrix * WorldToCameraSpaceMatrix;
                    _cameraProjToWorldSpaceMatrix = CameraToWorldSpaceMatrix * InverseProjectionMatrix;
                    _matrixInvalidated = false;
                }
                return _worldToCameraProjSpaceMatrix;
            }
        }
        [Browsable(false)]
        public Matrix4 CameraProjToWorldSpaceMatrix
        {
            get
            {
                if (_matrixInvalidated)
                {
                    _worldToCameraProjSpaceMatrix = ProjectionMatrix * WorldToCameraSpaceMatrix;
                    _cameraProjToWorldSpaceMatrix = CameraToWorldSpaceMatrix * InverseProjectionMatrix;
                    _matrixInvalidated = false;
                }
                return _cameraProjToWorldSpaceMatrix;
            }
        }

        protected void OnTransformChanged(bool updateTransformedFrustum = true)
        {
            _matrixInvalidated = true;
            if (updateTransformedFrustum)
                UpdateTransformedFrustum();
            _updating = true;
            TransformChanged?.Invoke();
            _updating = false;
        }

//        internal static string ShaderDecl()
//        {
//            return @"
//uniform vec3 CameraPosition;
//uniform vec3 CameraForward;
//uniform float CameraNearZ;
//uniform float CameraFarZ;
//uniform float ScreenWidth;
//uniform float ScreenHeight;
//uniform float ScreenOrigin;
//uniform float ProjOrigin;
//uniform float ProjRange;
//uniform mat4 WorldToCameraSpaceMatrix;
//uniform mat4 CameraToWorldSpaceMatrix;
//uniform mat4 ProjMatrix;
//uniform mat4 InvProjMatrix;";
//        }

        public virtual void SetUniforms(RenderProgram program)
        {
            program.Uniform(EEngineUniform.WorldToCameraSpaceMatrix,    WorldToCameraSpaceMatrix);
            program.Uniform(EEngineUniform.ProjMatrix,                  ProjectionMatrix);
            program.Uniform(EEngineUniform.CameraToWorldSpaceMatrix,    CameraToWorldSpaceMatrix);
            program.Uniform(EEngineUniform.InvProjMatrix,               InverseProjectionMatrix);
            program.Uniform(EEngineUniform.ScreenWidth,                 Width);
            program.Uniform(EEngineUniform.ScreenHeight,                Height);
            program.Uniform(EEngineUniform.ScreenOrigin,                Origin);
            program.Uniform(EEngineUniform.CameraPosition,              WorldPoint);
        }
        
        protected abstract void OnCreateTransform(
            out Matrix4 cameraToWorldSpaceMatrix,
            out Matrix4 worldToCameraSpaceMatrix);

        protected abstract void OnCalculateProjection(
            out Matrix4 projMatrix,
            out Matrix4 inverseProjMatrix);
        
        [TPostDeserialize]
        protected void CalculateProjection()
        {
            if (_updating)
                return;

            OnCalculateProjection(out _projectionMatrix, out _projectionInverse);

            _projectionRange = new Vec3(Dimensions, 1.0f);
            _projectionOrigin = new Vec3(Origin, 0.0f);

            _untransformedFrustum = CreateUntransformedFrustum();
            UpdateTransformedFrustum();

            _matrixInvalidated = true;
            ProjectionChanged?.Invoke();
        }
        protected void CreateTransform()
        {
            if (_updating)
                return;

            OnCreateTransform(out _cameraToWorldSpaceMatrix, out _worldToCameraSpaceMatrix);
            OnTransformChanged();
        }

        protected abstract IFrustum CreateUntransformedFrustum();

        protected void BeginUpdate() 
            =>_updating = true;
        protected void EndUpdate()
        {
            _updating = false;
            TransformChanged?.Invoke();
        }

        public Segment GetWorldSegment(Vec2 screenPoint)
        {
            screenPoint.Clamp(Vec2.Zero, new Vec2(Width, Height));
            Vec3 start = ScreenToWorld(screenPoint, 0.0f);
            Vec3 end = ScreenToWorld(screenPoint, 1.0f);
            return new Segment(start, end);
        }
        public Ray GetWorldRay(Vec2 screenPoint)
        {
            Vec3 start = ScreenToWorld(screenPoint, 0.0f);
            Vec3 end = ScreenToWorld(screenPoint, 1.0f);
            return new Ray(start, end - start);
        }
        public Vec3 GetPointAtDepth(Vec2 screenPoint, float depth)
            => ScreenToWorld(screenPoint, depth);
        public Vec3 GetPointAtDistance(Vec2 screenPoint, float distance)
            => GetWorldSegment(screenPoint).PointAtLineDistance(distance);

        public virtual void Render(bool shadowPass)
        {
            _transformedFrustum.Render(shadowPass);
        }
        public Plane GetScreenPlane()
            => new Plane(WorldPoint, ForwardVector);

        public Vec3 GetScreenPlaneOriginDistance()
            => Plane.ComputeDistance(WorldPoint, ForwardVector);

        public float DistanceFromScreenPlane(Vec3 point)
        {
            Vec3 forward = ForwardVector;
            return Collision.DistancePlanePoint(forward, Plane.ComputeDistance(WorldPoint, forward), point);
        }
        public Vec3 ClosestPointOnScreenPlane(Vec3 point)
        {
            Vec3 forward = ForwardVector;
            return Collision.ClosestPlanePointToPoint(forward, Plane.ComputeDistance(WorldPoint, forward), point);
        }
        public float DistanceFromWorldPoint(Vec3 point)
            => WorldPoint.DistanceTo(point);
        public float DistanceFromWorldPointFast(Vec3 point)
            => WorldPoint.DistanceToFast(point);

        /// <summary>
        /// Returns a uniform scale that will keep the given radius the same size on the screen at the given point.
        /// </summary>
        /// <param name="point">The location of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>The scale on the screen.</returns>
        public virtual float DistanceScale(Vec3 point, float radius = 1.0f)
        {
            //Using distance to the screen plane
            //instead of distance to the camera's world point
            //returns expected results.
            //Otherwise scale will increase on edges of screen
            float distance = DistanceFromScreenPlane(point);
            return distance * radius * 0.1f;
        }

        RenderCommandMethod3D _renderCommand;
        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            passes.Add(_renderCommand);
        }

        //Child camera types must override this
        public virtual void Resize(float width, float height)
            => CalculateProjection();

        public abstract void SetBloomUniforms(RenderProgram program);
        public abstract void SetAmbientOcclusionUniforms(RenderProgram program);
        public abstract void SetPostProcessUniforms(RenderProgram program);

        public abstract bool UsesAutoExposure { get; }

        public abstract void UpdateExposure(TexRef2D texture);
    }
}
