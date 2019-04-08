using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Cameras
{
    /// <summary>
    /// Base class for your usual perspective and orthographic cameras.
    /// </summary>
    public abstract class TypicalCamera : Camera, ICameraTransformable
    {
        public delegate void TranslationChange(Vec3 oldTranslation);
        public delegate void RotationChange(Rotator oldRotation);

        protected TypicalCamera() 
            : this(16.0f, 9.0f) { }
        protected TypicalCamera(float width, float height)
            : this(width, height, 1.0f, 10000.0f) { }
        protected TypicalCamera(float width, float height, float nearZ, float farZ)
            : this(width, height, nearZ, farZ, Vec3.Zero, Rotator.GetZero()) { }
        protected TypicalCamera(float width, float height, float nearZ, float farZ, Vec3 point, Rotator rotation) : base()
        {
            _postProcessSettingsRef = new PostProcessSettings();
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
        public override Matrix4 CameraToComponentSpaceMatrix
        {
            get => _cameraToWorldSpaceMatrix;
            internal set
            {
                _cameraToWorldSpaceMatrix = value;
                _worldToCameraSpaceMatrix = _cameraToWorldSpaceMatrix.Inverted();
                _localPoint.Raw = _cameraToWorldSpaceMatrix.Translation;
                _localRotation.SetRotations(_cameraToWorldSpaceMatrix.GetRotationMatrix4().ExtractRotation().ToYawPitchRoll());
                OnTransformChanged();
            }
        }
        [DisplayName("Near Distance")]
        [Category("Camera")]
        public override float NearZ
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
        public override float FarZ
        {
            get => _farZ;
            set
            {
                _farZ = value;
                CalculateProjection();
            }
        }

        [Browsable(false)]
        [Category("Camera")]
        public override Vec3 WorldPoint => _owningComponent?.WorldMatrix.Translation ?? _localPoint.Raw;
        
        [Category("Camera")]
        public EventVec3 LocalPoint
        {
            get => _localPoint;
            set
            {
                if (_localPoint != null)
                    _localPoint.Changed -= PositionChanged;
                _localPoint = value ?? new EventVec3();
                _localPoint.Changed += PositionChanged;
                PositionChanged();
            }
        }
        [Category("Camera")]
        public Rotator LocalRotation
        {
            get => _localRotation;
            set
            {
                if (_localRotation != null)
                    _localRotation.Changed -= CreateTransform;
                _localRotation = value ?? new Rotator();
                _localRotation.Changed += CreateTransform;
                CreateTransform();
            }
        }
        [DisplayName("Post-Processing")]
        [Category("Camera")]
        public LocalFileRef<PostProcessSettings> PostProcessRef
        {
            get => _postProcessSettingsRef;
            set => _postProcessSettingsRef = value ?? new PostProcessSettings();
        }
        [Category("Camera")]
        [TSerialize]
        public LocalFileRef<TMaterial> PostProcessMaterial { get; set; }

        [Category("Camera")]
        public EventVec3 ViewTarget
        {
            get => _viewTarget;
            set
            {
                if (_viewTarget != null)
                    _viewTarget.Changed -= ViewTargetChanged;
                _viewTarget = value;
                if (_viewTarget != null)
                {
                    _viewTarget.Changed += ViewTargetChanged;
                    ViewTargetChanged();
                }
            }
        }
        
        protected EventVec3 _viewTarget = null;

        [TSerialize("Point")]
        protected EventVec3 _localPoint;
        [TSerialize("Rotation")]
        protected Rotator _localRotation;
        [TSerialize("NearZ")]
        protected float _nearZ;
        [TSerialize("FarZ")]
        protected float _farZ;
        [TSerialize("PostProcessSettings")]
        private LocalFileRef<PostProcessSettings> _postProcessSettingsRef;

        private void ViewTargetChanged()
            => SetRotationWithTarget(_viewTarget.Raw);
        private void OwningComponentWorldTransformChanged(SceneComponent comp)
        {
            //_forwardInvalidated = true;
            //_upInvalidated = true;
            //_rightInvalidated = true;
            UpdateTransformedFrustum();
            if (!_updating)
                OnTransformChanged();
        }

        public override void RebaseOrigin(Vec3 newOrigin)
            => TranslateAbsolute(-newOrigin);

        public virtual void PositionChanged()
        {
            if (_viewTarget != null)
                _localRotation.SetRotationsNoUpdate((_viewTarget.Raw - _localPoint).LookatAngles());
            CreateTransform();
        }
        protected override void OnCreateTransform(out Matrix4 cameraToWorldSpaceMatrix, out Matrix4 worldToCameraSpaceMatrix)
        {
            cameraToWorldSpaceMatrix = Matrix4.CreateTranslation(_localPoint.Raw) * _localRotation.GetMatrix();
            worldToCameraSpaceMatrix = _localRotation.GetInverseMatrix() * Matrix4.CreateTranslation(-_localPoint.Raw);
        }

        /// <summary>
        /// Translates the camera relative to the camera's rotation.
        /// </summary>
        public void TranslateRelative(float x, float y, float z) 
            => TranslateRelative(new Vec3(x, y, z));
        /// <summary>
        /// Translates the camera relative to the camera's rotation.
        /// </summary>
        public void TranslateRelative(Vec3 translation)
        {
            _cameraToWorldSpaceMatrix = _cameraToWorldSpaceMatrix * translation.AsTranslationMatrix();
            _worldToCameraSpaceMatrix = (-translation).AsTranslationMatrix() * _worldToCameraSpaceMatrix;
            _localPoint.SetRawNoUpdate(_cameraToWorldSpaceMatrix.Translation);
            if (_viewTarget != null)
                SetRotationWithTarget(_viewTarget.Raw);
            else
                OnTransformChanged();
        }

        /// <summary>
        /// Translates the camera relative to the world.
        /// </summary>
        public void TranslateAbsolute(float x, float y, float z) 
            => TranslateAbsolute(new Vec3(x, y, z));
        /// <summary>
        /// Translates the camera relative to the world.
        /// </summary>
        public void TranslateAbsolute(Vec3 translation)
            => _localPoint.Raw += translation;
        
        /// <summary>
        /// Increments the camera's pitch and yaw rotations.
        /// </summary>
        public void AddRotation(float pitch, float yaw) 
            => AddRotation(pitch, yaw, 0.0f);
        /// <summary>
        /// Increments the camera's pitch, yaw and roll rotations.
        /// </summary>
        public void AddRotation(float pitch, float yaw, float roll) 
            => SetRotation(
                _localRotation.Pitch + pitch,
                _localRotation.Yaw + yaw, 
                _localRotation.Roll + roll);

        public void SetRotationsNoUpdate(Rotator r)
            => _localRotation.SetRotationsNoUpdate(r);
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
        
        public void Reset()
        {
            _localPoint.Raw = Vec3.Zero;
            _localRotation.YawPitchRoll = Vec3.Zero;
        }

        public event TranslationChange TranslationChanged;
        public event RotationChange RotationChanged;

        protected void OnTranslationChanged(Vec3 oldTranslation) 
            => TranslationChanged?.Invoke(oldTranslation);
        protected void OnRotationChanged(Rotator oldRotation)
            => RotationChanged?.Invoke(oldRotation);
        
        public override void SetUniforms(RenderProgram program)
        {
            base.SetUniforms(program);
            program.Uniform(EEngineUniform.CameraNearZ, NearZ);
            program.Uniform(EEngineUniform.CameraFarZ, FarZ);
        }
        
        public override void Render()
        {
            base.Render();
            if (_viewTarget != null)
                Engine.Renderer.RenderLine(WorldPoint, _viewTarget.Raw, Color.DarkGray, false, 1.0f);
        }

        Rotator ICameraTransformable.Rotation
        {
            get => LocalRotation;
            set => LocalRotation = value;
        }
        EventVec3 ICameraTransformable.Translation
        {
            get => LocalPoint;
            set => LocalPoint = value;
        }
        
        public void Pivot(float pitch, float yaw, float distance)
            => ArcBallRotate(pitch, yaw, LocalPoint.Raw + ForwardVector * distance);
        public void ArcBallRotate(float pitch, float yaw, Vec3 origin)
        {
            LocalPoint.Raw = TMath.ArcballTranslation(pitch, yaw, origin, LocalPoint.Raw, RightVector);
            LocalRotation.AddRotations(pitch, yaw, 0.0f);
        }

        internal override void SetAmbientOcclusionUniforms(RenderProgram program) 
            => PostProcessRef?.File?.AmbientOcclusion?.SetUniforms(program);
        internal override void SetBloomUniforms(RenderProgram program)
            => PostProcessRef?.File?.Bloom?.SetUniforms(program);
        internal override void SetPostProcessUniforms(RenderProgram program)
            => PostProcessRef?.File?.SetUniforms(program);
    }
}
