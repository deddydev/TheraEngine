using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.ComponentModel;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Cameras
{
    /// <summary>
    /// Base class for your usual perspective and orthographic cameras.
    /// </summary>
    public abstract class TransformableCamera : Camera, ICameraTransformable
    {
        public delegate void TranslationChange(Vec3 oldTranslation);
        public delegate void RotationChange(Rotator oldRotation);

        protected TransformableCamera() 
            : this(16.0f, 9.0f) { }
        protected TransformableCamera(float width, float height)
            : this(width, height, 1.0f, 10000.0f) { }
        protected TransformableCamera(float width, float height, float nearZ, float farZ)
            : this(width, height, nearZ, farZ, Vec3.Zero, Rotator.GetZero()) { }
        protected TransformableCamera(float width, float height, float nearZ, float farZ, Vec3 point, Rotator rotation) : base(nearZ, farZ)
        {
            _localRotation = rotation;
            _localPoint = point;

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
            set
            {
                _cameraToWorldSpaceMatrix = value;
                _worldToCameraSpaceMatrix = _cameraToWorldSpaceMatrix.Inverted();
                _localPoint.Value = _cameraToWorldSpaceMatrix.Translation;
                _localRotation.SetRotations(_cameraToWorldSpaceMatrix.GetRotationMatrix4().ExtractRotation().ToRotator());
                OnTransformChanged();
            }
        }

        [Browsable(false)]
        [Category("Camera")]
        public override Vec3 WorldPoint => _owningComponent?.WorldMatrix.Translation ?? _localPoint.Value;
        
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
        //[TSerialize("PostProcessSettings")]
        //private LocalFileRef<PostProcessSettings> _postProcessSettingsRef;

        private void ViewTargetChanged()
            => SetRotationWithTarget(_viewTarget.Value);
        private void OwningComponentWorldTransformChanged(ISceneComponent comp)
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
                _localRotation.SetRotationsNoUpdate((_viewTarget.Value - _localPoint).LookatAngles());
            CreateTransform();
        }
        protected override void OnCreateTransform(out Matrix4 cameraToWorldSpaceMatrix, out Matrix4 worldToCameraSpaceMatrix)
        {
            cameraToWorldSpaceMatrix = _localPoint.AsTranslationMatrix() * _localRotation.GetMatrix();
            worldToCameraSpaceMatrix = _localRotation.GetInverseMatrix() * _localPoint.AsInverseTranslationMatrix();
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
            _localPoint.SetValueSilent(_cameraToWorldSpaceMatrix.Translation);
            if (_viewTarget != null)
                SetRotationWithTarget(_viewTarget.Value);
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
            => _localPoint.Value += translation;
        
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
            _localPoint.Value = Vec3.Zero;
            _localRotation.YawPitchRoll = Vec3.Zero;
        }

        public event TranslationChange TranslationChanged;
        public event RotationChange RotationChanged;

        protected void OnTranslationChanged(Vec3 oldTranslation) 
            => TranslationChanged?.Invoke(oldTranslation);
        protected void OnRotationChanged(Rotator oldRotation)
            => RotationChanged?.Invoke(oldRotation);
        
        public override void Render(bool shadowPass)
        {
            base.Render(shadowPass);
            if (_viewTarget != null)
                Engine.Renderer.RenderLine(WorldPoint, _viewTarget.Value, Color.DarkGray, false, 1.0f);
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
            => ArcBallRotate(pitch, yaw, LocalPoint.Value + ForwardVector * distance);
        public void ArcBallRotate(float pitch, float yaw, Vec3 origin)
        {
            LocalPoint.Value = TMath.ArcballTranslation(pitch, yaw, origin, LocalPoint.Value, RightVector);
            LocalRotation.AddRotations(pitch, yaw, 0.0f);
        }
    }
}
