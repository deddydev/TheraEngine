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
    public abstract class TransformableCamera : Camera
    {
        public delegate void TranslationChange(Vec3 oldTranslation);
        public delegate void RotationChange(Rotator oldRotation);

        protected TransformableCamera() 
            : this(16.0f, 9.0f) { }
        protected TransformableCamera(float width, float height)
            : this(width, height, 1.0f, 10000.0f) { }
        protected TransformableCamera(float width, float height, float nearZ, float farZ)
            : this(width, height, nearZ, farZ, Vec3.Zero, EventQuat.Identity) { }
        protected TransformableCamera(float width, float height, float nearZ, float farZ, Vec3 point, EventQuat rotation) : base(nearZ, farZ)
        {
            _rotation = rotation;
            _translation = point;

            Resize(width, height);

            _viewTarget = null;
            _rotation.Changed += CreateTransform;
            _translation.Changed += PositionChanged;

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
                _translation.Value = _cameraToWorldSpaceMatrix.Translation;
                _rotation.Value = _cameraToWorldSpaceMatrix.GetRotationMatrix4().ExtractRotation();
                OnTransformChanged();
            }
        }

        [Browsable(false)]
        [Category("Camera")]
        public override Vec3 WorldPoint => _owningComponent?.WorldMatrix.Value.Translation ?? _translation.Value;
        
        [Category("Camera")]
        public EventVec3 Translation
        {
            get => _translation;
            set
            {
                if (_translation != null)
                    _translation.Changed -= PositionChanged;
                _translation = value ?? new EventVec3();
                _translation.Changed += PositionChanged;
                PositionChanged();
            }
        }
        [Category("Camera")]
        public EventQuat Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != null)
                    _rotation.Changed -= CreateTransform;
                _rotation = value ?? new EventQuat();
                _rotation.Changed += CreateTransform;
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
        protected EventVec3 _translation;
        [TSerialize("Rotation")]
        protected EventQuat _rotation;
        //[TSerialize("PostProcessSettings")]
        //private LocalFileRef<PostProcessSettings> _postProcessSettingsRef;

        private void ViewTargetChanged()
            => SetRotationWithTarget(_viewTarget.Value);

        public override void RebaseOrigin(Vec3 newOrigin)
            => TranslateAbsolute(-newOrigin);

        public virtual void PositionChanged()
        {
            if (_viewTarget != null)
            {
                Vec3 dir = (_viewTarget.Value - _translation).Normalized();
                Quat q = dir.LookatAngles().ToQuaternion();
                //Vec3 rotationAxis = dir ^ Vec3.Up;
                //Quat yaw = Quat.FromAxisAngleDeg(Vec3.Up, new Vec3(dir.X, 0.0f, dir.Z).Normalized() | Vec3.Right);
                //Quat pitch = Quat.FromAxisAngleDeg(rotationAxis, dir | rotationAxis);
                ////Vec3 floorAxis = rotationAxis ^ dir;
                ////float angle = (dir | floorAxis);
                ////Quat q = Quat.FromAxisAngleDeg(rotationAxis, angle);
                _rotation.SetValueSilent(q);
            }
            CreateTransform();
        }
        protected override void OnCreateTransform(out Matrix4 cameraToWorldSpaceMatrix, out Matrix4 worldToCameraSpaceMatrix)
        {
            cameraToWorldSpaceMatrix = _translation.AsTranslationMatrix() * Matrix4.CreateFromQuaternion(_rotation);
            worldToCameraSpaceMatrix = Matrix4.CreateFromQuaternion(_rotation.Value.Inverted()) * _translation.AsInverseTranslationMatrix();
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
            _translation.SetValueSilent(_cameraToWorldSpaceMatrix.Translation);
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
            => _translation.Value += translation;
        
        /// <summary>
        /// Increments the camera's pitch and yaw rotations.
        /// </summary>
        public void AddRotation(float pitch, float yaw) 
            => AddRotation(pitch, yaw, 0.0f);
        /// <summary>
        /// Increments the camera's pitch, yaw and roll rotations.
        /// </summary>
        public void AddRotation(float pitch, float yaw, float roll) 
            => _rotation.Value *= Quat.Euler(pitch, yaw, roll);

        public void SetRotationsNoUpdate(Rotator r)
            => _rotation.SetValueSilent(r.ToQuaternion());
        public void SetRotation(Rotator r) 
            => _rotation.Value = r.ToQuaternion();

        public void SetRotationsNoUpdate(Quat r)
            => _rotation.SetValueSilent(r);
        public void SetRotation(Quat r)
            => _rotation.Value = r;

        public void SetRotation(Vec3 pitchYawRoll)
            => _rotation.Value = Quat.Euler(pitchYawRoll.X, pitchYawRoll.Y, pitchYawRoll.Z);
        public void SetRotation(float pitch, float yaw, float roll)
            => _rotation.Value = Quat.Euler(pitch, yaw, roll);
        public void SetRotationWithTarget(Vec3 target)
        {
            //if (_owningComponent != null)
            //    target = Vec3.TransformPosition(target, _owningComponent.InverseWorldMatrix);
            SetRotation((target - _translation).LookatAngles());
        }
        
        public void Reset()
        {
            _translation.Value = Vec3.Zero;
            _rotation.Value = Quat.Identity;
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
        
        public void Pivot(float pitch, float yaw, float distance)
            => ArcBallRotate(pitch, yaw, Translation.Value + ForwardVector * distance);
        public void ArcBallRotate(float pitch, float yaw, Vec3 origin)
        {
            Translation.Value = TMath.ArcballTranslation(pitch, yaw, origin, Translation.Value, RightVector);
            Rotation.Value *= Quat.Euler(pitch, yaw, 0.0f);
        }
    }
}
