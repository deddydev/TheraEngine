using TheraEngine.Files;
using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using TheraEngine.Worlds.Actors;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.Cameras
{
    public delegate void OwningComponentChange(CameraComponent previous, CameraComponent current);
    public abstract class Camera : FileObject, I3DRenderable
    {
        private RenderInfo3D _renderInfo = new RenderInfo3D(ERenderPassType3D.OpaqueForward, null, false, false);
        public RenderInfo3D RenderInfo => _renderInfo;
        [Browsable(false)]
        public Shape CullingVolume => _transformedFrustum.CullingVolume;
        [Browsable(false)]
        public IOctreeNode OctreeNode
        {
            get => _transformedFrustum.OctreeNode;
            set => _transformedFrustum.OctreeNode = value;
        }

        public event OwningComponentChange OwningComponentChanged;
        public delegate void TranslationChange(Vec3 oldTranslation);
        public delegate void RotationChange(Rotator oldRotation);

        public Camera() 
            : this(16.0f, 9.0f) { }
        public Camera(float width, float height)
            : this(width, height, 1.0f, 10000.0f) { }
        public Camera(float width, float height, float nearZ, float farZ)
            : this(width, height, nearZ, farZ, Vec3.Zero, Rotator.GetZero()) { }
        public Camera(float width, float height, float nearZ, float farZ, Vec3 point, Rotator rotation)
        {
            _postProcessSettings = new PostProcessSettings();
            _transformedFrustum = new Frustum();
            _localRotation = rotation;
            _localPoint = point;
            _nearZ = nearZ;
            _farZ = farZ;
            Resize(width, height);
            _viewTarget = null;
            _localRotation.Changed += CreateTransform;
            _localPoint.Changed += PositionChanged;
            PositionChanged();
        }

        [Browsable(false)]
        public Matrix4 ProjectionMatrix => _projectionMatrix;
        [Browsable(false)]
        public Matrix4 InverseProjectionMatrix => _projectionInverse;
        [Browsable(false)]
        public Matrix4 CameraToWorldSpaceMatrix
        {
            get => _owningComponent != null ? _owningComponent.WorldMatrix : _cameraToWorldSpaceMatrix;
            //set
            //{
            //    if (_owningComponent != null)
            //    {
            //        _owningComponent.WorldMatrix = value;
            //        _transform = _owningComponent.LocalMatrix;
            //        _invTransform = _owningComponent.InverseLocalMatrix;
            //    }
            //    else
            //    {
            //        _transform = value;
            //        _invTransform = _transform.Inverted();
            //    }
            //}
        }
        [Browsable(false)]
        public Matrix4 WorldToCameraSpaceMatrix
        {
            get => _owningComponent != null ? _owningComponent.InverseWorldMatrix : _worldToCameraSpaceMatrix;
            //set
            //{
            //    if (_owningComponent != null)
            //    {
            //        _owningComponent.InverseWorldMatrix = value;
            //        _transform = _owningComponent.LocalMatrix;
            //        _invTransform = _owningComponent.InverseLocalMatrix;
            //    }
            //    else
            //    {
            //        _invTransform = value;
            //        _transform = _invTransform.Inverted();
            //    }
            //}
        }
        [Browsable(false)]
        public Matrix4 ComponentToCameraSpaceMatrix => _worldToCameraSpaceMatrix;
        [Browsable(false)]
        public Matrix4 CameraToComponentSpaceMatrix
        {
            get => _cameraToWorldSpaceMatrix;
            internal set
            {
                _localPoint.Raw = _cameraToWorldSpaceMatrix.GetPoint();
                _localRotation.SetRotations(_cameraToWorldSpaceMatrix.GetRotationMatrix4().ExtractRotation().ToYawPitchRoll());
                _cameraToWorldSpaceMatrix = value;
                _worldToCameraSpaceMatrix = _cameraToWorldSpaceMatrix.Inverted();
                UpdateTransformedFrustum();
                OnTransformChanged();
            }
        }
        [DisplayName("Near Distance")]
        [Category("Camera")]
        public float NearZ
        {
            get => _nearZ;
            set
            {
                _nearZ = value;
                CalculateProjection();
            }
        }
        [DisplayName("Far Distance")]
        [Category("Camera")]
        public float FarZ
        {
            get => _farZ;
            set
            {
                _farZ = value;
                CalculateProjection();
            }
        }
        [DisplayName("World Point")]
        [Category("Camera")]
        public Vec3 WorldPoint
        {
            get => _owningComponent != null ? _owningComponent.WorldMatrix.GetPoint() : _localPoint.Raw;
        }
        [DisplayName("Local Point")]
        [Category("Camera")]
        public EventVec3 LocalPoint
        {
            get => _localPoint;
            set
            {
                _localPoint = value;
                _localPoint.Changed += PositionChanged;
                PositionChanged();
            }
        }
        [DisplayName("Local Rotation")]
        [Category("Camera")]
        public Rotator LocalRotation
        {
            get => _localRotation;
            set => _localRotation.SetRotations(value);
        }
        [DisplayName("Post-Processing")]
        [Category("Camera")]
        public PostProcessSettings PostProcessSettings
        {
            get => _postProcessSettings;
            set => _postProcessSettings = value ?? new PostProcessSettings();
        }

        public abstract float Width { get; set; }
        public abstract float Height { get; set; }
        public abstract Vec2 Origin { get; }
        public Vec2 Dimensions => new Vec2(Width, Height);

        [Browsable(false)]
        public List<Viewport> Viewports
        {
            get => _viewports;
            internal set => _viewports = value;
        }
        [Browsable(false)]
        public bool IsActiveRenderCamera { get => _isActive; internal set => _isActive = value; }

        [Browsable(false)]
        public CameraComponent OwningComponent
        {
            get => _owningComponent;
            set
            {
                CameraComponent prev = _owningComponent;
                if (_owningComponent != null)
                    _owningComponent.WorldTransformChanged -= _owningComponent_WorldTransformChanged;
                _owningComponent = value;
                if (_owningComponent != null)
                    _owningComponent.WorldTransformChanged += _owningComponent_WorldTransformChanged;
                OwningComponentChanged?.Invoke(prev, _owningComponent);
                UpdateTransformedFrustum();
            }
        }
        [Category("Camera")]
        [DisplayName("View Target")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public EventVec3 ViewTarget
        {
            get => _viewTarget;
            set
            {
                if (_viewTarget != null)
                    _viewTarget.Changed -= _viewTarget_Changed;
                _viewTarget = value;
                if (_viewTarget != null)
                    _viewTarget.Changed += _viewTarget_Changed;
                _viewTarget_Changed();
            }
        }

        [Browsable(false)]
        public ERenderPassType3D RenderPass => throw new NotImplementedException();
        [Browsable(false)]
        public float RenderOrder => throw new NotImplementedException();

        private CameraComponent _owningComponent;
        private List<Viewport> _viewports = new List<Viewport>();
        private bool _isActive = false;
        internal Vec3 _projectionRange;
        internal Vec3 _projectionOrigin;
        protected Frustum _untransformedFrustum, _transformedFrustum;
        private EventVec3 _viewTarget = null;
        protected bool _updating = false;
        protected Matrix4
            _projectionMatrix = Matrix4.Identity,
            _projectionInverse = Matrix4.Identity,
            _cameraToWorldSpaceMatrix = Matrix4.Identity,
            _worldToCameraSpaceMatrix = Matrix4.Identity;
        private Vec3
            _forwardDirection = Vec3.Forward,
            _upDirection = Vec3.Up,
            _rightDirection = Vec3.Right;
        private bool
            _forwardInvalidated = true,
            _upInvalidated = true,
            _rightInvalidated = true;

        [Serialize("Point")]
        protected EventVec3 _localPoint;
        [Serialize("Rotation")]
        protected Rotator _localRotation;
        [Serialize("NearZ")]
        protected float _nearZ;
        [Serialize("FarZ")]
        protected float _farZ;
        [Serialize("PostProcessSettings")]
        private PostProcessSettings _postProcessSettings;

        private void _viewTarget_Changed()
            => SetRotationWithTarget(_viewTarget.Raw);
        private void _owningComponent_WorldTransformChanged()
        {
            _forwardInvalidated = true;
            _upInvalidated = true;
            _rightInvalidated = true;
            UpdateTransformedFrustum();
            if (!_updating)
                OnTransformChanged();
        }

        /// <summary>
        /// Returns an X, Y coordinate relative to the camera's Origin, with Z being the normalized depth from NearDepth to FarDepth.
        /// </summary>
        public Vec3 WorldToScreen(Vec3 point)
            => _projectionRange * (((point * (ProjectionMatrix * WorldToCameraSpaceMatrix)) + 1.0f) / 2.0f);
        public Vec3 ScreenToWorld(Vec2 point, float depth)
            => ScreenToWorld(point.X, point.Y, depth);
        public Vec3 ScreenToWorld(float x, float y, float depth)
            => ScreenToWorld(new Vec3(x, y, depth));
        /// <summary>
        /// Takes an X, Y coordinate relative to the camera's Origin, with Z being the normalized depth from NearDepth to FarDepth, and returns a position in the world.
        /// </summary>
        public Vec3 ScreenToWorld(Vec3 screenPoint)
            => ((screenPoint / _projectionRange) * 2.0f - 1.0f) * (CameraToWorldSpaceMatrix * InverseProjectionMatrix);
        
        protected virtual void PositionChanged()
        {
            if (_viewTarget != null)
                _localRotation.SetRotationsNoUpdate((_viewTarget.Raw - _localPoint).LookatAngles());
            CreateTransform();
        }
        protected virtual void CreateTransform()
        {
            Matrix4 rotMatrix = _localRotation.GetMatrix();
            _cameraToWorldSpaceMatrix = Matrix4.CreateTranslation(_localPoint.Raw) * rotMatrix;
            _worldToCameraSpaceMatrix = _localRotation.GetInverseMatrix() * Matrix4.CreateTranslation(-_localPoint.Raw);
            UpdateTransformedFrustum();
            OnTransformChanged();
        }
        protected void UpdateTransformedFrustum()
            => _transformedFrustum.TransformedVersionOf(_untransformedFrustum, CameraToWorldSpaceMatrix);

        public abstract float DistanceScale(Vec3 point, float radius);
        public abstract void Zoom(float amount);

        public void TranslateRelative(float x, float y, float z) 
            => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            _cameraToWorldSpaceMatrix = _cameraToWorldSpaceMatrix * Matrix4.CreateTranslation(translation);
            _worldToCameraSpaceMatrix = Matrix4.CreateTranslation(-translation) * _worldToCameraSpaceMatrix;
            _localPoint.SetRawNoUpdate(_cameraToWorldSpaceMatrix.GetPoint());
            if (_viewTarget != null)
                SetRotationWithTarget(_viewTarget.Raw);
            else
            {
                UpdateTransformedFrustum();
                OnTransformChanged();
            }
        }

        public void TranslateAbsolute(float x, float y, float z) 
            => TranslateAbsolute(new Vec3(x, y, z));
        public void TranslateAbsolute(Vec3 translation)
            => _localPoint.Raw += translation;
        
        public Vec3 RotateVector(Vec3 dir)
            => Vec3.TransformVector(dir, CameraToWorldSpaceMatrix).NormalizedFast();
        public Vec3 GetUpVector()
        {
            if (_upInvalidated)
            {
                _upDirection = RotateVector(Vec3.Up);
                _upInvalidated = false;
            }
            return _upDirection;
        }
        public Vec3 GetForwardVector()
        {
            if (_forwardInvalidated)
            {
                _forwardDirection = RotateVector(Vec3.Forward);
                _forwardInvalidated = false;
            }
            return _forwardDirection;
        }
        public Vec3 GetRightVector()
        {
            if (_rightInvalidated)
            {
                _rightDirection = RotateVector(Vec3.Right);
                _rightInvalidated = false;
            }
            return _rightDirection;
        }

        public void AddRotation(float pitch, float yaw) 
            => AddRotation(pitch, yaw, 0.0f);
        public void AddRotation(float pitch, float yaw, float roll) 
            => SetRotation(
                _localRotation.Pitch + pitch,
                _localRotation.Yaw + yaw, 
                _localRotation.Roll + roll);

        public void SetRotation(Rotator r) 
            => _localRotation.SetRotations(r);
        public void SetRotation(Vec3 pitchYawRoll)
            => _localRotation.PitchYawRoll = pitchYawRoll;
        public void SetRotation(float pitch, float yaw, float roll)
            => _localRotation.SetRotations(pitch, yaw, roll);
        public void SetRotationWithTarget(Vec3 target)
        {
            //if (_owningComponent != null)
            //    target = Vec3.TransformPosition(target, _owningComponent.InverseWorldMatrix);
            SetRotation((target - _localPoint).LookatAngles());
        }
        public void Pivot(float y, float x, float radius)
        {
            BeginUpdate();
            Zoom(-radius);
            AddRotation(y, x);
            Zoom(radius);
            EndUpdate();
        }
        
        public void Reset()
        {
            _localPoint.Raw = Vec3.Zero;
            _localRotation.YawPitchRoll = Vec3.Zero;
        }
        public void SaveCurrentTransform() { }

        public event TranslationChange TranslationChanged;
        public event RotationChange RotationChanged;
        //public event Action Resized;
        public event Action TransformChanged;
        public event Action ProjectionChanged;

        protected void OnTransformChanged()
        {
            _forwardInvalidated = _upInvalidated = _rightInvalidated = true;
            _updating = true;
            TransformChanged?.Invoke();
            _updating = false;
        }
        protected void OnTranslationChanged(Vec3 oldTranslation) 
            => TranslationChanged?.Invoke(oldTranslation);
        protected void OnRotationChanged(Rotator oldRotation)
            => RotationChanged?.Invoke(oldRotation);
        
        internal static string ShaderDecl()
        {
            return @"
uniform vec3 CameraPosition;
uniform vec3 CameraForward;
uniform float CameraNearZ;
uniform float CameraFarZ;
uniform float ScreenWidth;
uniform float ScreenHeight;
uniform float ScreenOrigin;
uniform float ProjOrigin;
uniform float ProjRange;
uniform mat4 WorldToCameraSpaceMatrix;
uniform mat4 CameraToWorldSpaceMatrix;
uniform mat4 ProjMatrix;
uniform mat4 InvProjMatrix;";
        }

        public virtual void SetUniforms(int programBindingId)
        {
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.WorldToCameraSpaceMatrix),    WorldToCameraSpaceMatrix);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.ProjMatrix),                  ProjectionMatrix);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.CameraToWorldSpaceMatrix),    CameraToWorldSpaceMatrix);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.InvProjMatrix),               InverseProjectionMatrix);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.ScreenWidth),                 Width);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.ScreenHeight),                Height);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.ScreenOrigin),                Origin);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.CameraNearZ),                 NearZ);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.CameraFarZ),                  FarZ);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.CameraPosition),              WorldPoint);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.CameraForward),               GetForwardVector());
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.CameraUp),                    GetUpVector());
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.CameraRight),                 GetRightVector());
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.ProjOrigin),                  _projectionOrigin);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.ProjRange),                   _projectionRange);
        }

        [PostDeserialize]
        protected virtual void CalculateProjection()
        {
            _projectionRange = new Vec3(Dimensions, 1.0f);
            _projectionOrigin = new Vec3(Origin, 0.0f);
            _untransformedFrustum = CreateUntransformedFrustum();
            UpdateTransformedFrustum();
            ProjectionChanged?.Invoke();
        }
        //Child camera types must override this
        public virtual void Resize(float width, float height)
            => CalculateProjection();
        protected abstract Frustum CreateUntransformedFrustum();
        public Frustum GetFrustum() 
            => _transformedFrustum;
        private void BeginUpdate() 
            =>_updating = true;
        private void EndUpdate()
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

        public void Render()
        {
            _transformedFrustum.Render();
            if (_viewTarget != null)
                Engine.Renderer.RenderLine(WorldPoint, _viewTarget.Raw, Color.DarkGray, 10.0f);
        }
    }
}
