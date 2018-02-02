using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Transforms
{
    /// <summary>
    /// Translates first, then rotates. Lags behind by a specified amount for smooth movement.
    /// </summary>
    [FileDef("Lagging Translate-Rotate Component")]
    public class TRLaggedComponent : TranslationLaggedComponent
    {
        public TRLaggedComponent() : this(Vec3.Zero, Rotator.GetZero(), true) { }
        public TRLaggedComponent(Vec3 translation, Rotator rotation, bool deferLocalRecalc = false) : base(translation, true)
        {
            _currentRotation = new Rotator(rotation);
            _desiredRotation = new Rotator(rotation);
            //_currentRotation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }
        public TRLaggedComponent(Vec3 translation, bool deferLocalRecalc = false) : base(translation, true)
        {
            _currentRotation = Rotator.GetZero();
            _desiredRotation = Rotator.GetZero();
            //_currentRotation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }
        public TRLaggedComponent(Rotator rotation, bool deferLocalRecalc = false) : base()
        {
            _currentRotation = new Rotator(rotation);
            _desiredRotation = new Rotator(rotation);
            //_currentRotation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }

        public void SetTR(Vec3 translation, Rotator rotation)
        {
            _currentTranslation = _desiredTranslation = translation;
            //_currentTranslation.Changed += RecalcLocalTransform;
            _currentRotation = new Rotator(rotation);
            _desiredRotation = new Rotator(rotation);
            //_currentRotation.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }

        [TSerialize("CurrentRotation")]
        protected Rotator _currentRotation;
        [TSerialize("DesiredRotation")]
        protected Rotator _desiredRotation;
        protected float _invRotInterpSec = 40.0f;

        [Category("Transform")]
        public Rotator DesiredRotation
        {
            get => _desiredRotation;
            set => _desiredRotation = value ?? new Rotator();
        }
        [Category("Transform")]
        public Rotator CurrentRotation
        {
            get => _currentRotation;
            set
            {
                _currentRotation = value ?? new Rotator();
                //_currentRotation.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }
        [TSerialize]
        [Category("Transform")]
        public float InverseRotationInterpSeconds
        {
            get => _invRotInterpSec;
            set => _invRotInterpSec = value;
        }

        protected internal override void OnDeserialized()
        {
            //_currentRotation.Changed += RecalcLocalTransform;
            base.OnDeserialized();
        }

        protected override void Tick(float delta)
        {
            _currentTranslation.Raw = Interp.InterpCosineTo(_currentTranslation.Raw, _desiredTranslation, delta, _invTransInterpSec);
            _currentRotation.PitchYawRoll = Interp.InterpCosineTo(_currentRotation.PitchYawRoll, _desiredRotation.PitchYawRoll, delta, _invRotInterpSec);
            RecalcLocalTransform();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4 
                r = _currentRotation.GetMatrix(), 
                ir = _currentRotation.Inverted().GetMatrix();

            Matrix4
                t = _currentTranslation.AsTranslationMatrix(), 
                it = (-_currentTranslation).AsTranslationMatrix();

            localTransform = t * r;
            inverseLocalTransform = ir * it;

            //Engine.PrintLine("Recalculated TR.");
        }

        public void TranslateRelative(float x, float y, float z)
            => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            Matrix4 r = _desiredRotation.GetMatrix();
            Matrix4 t = _desiredTranslation.AsTranslationMatrix();
            Matrix4 mtx = t * r * translation.AsTranslationMatrix();
            _desiredTranslation = mtx.GetPoint();
        }
        public void Pivot(float pitch, float yaw, float distance)
            => Pivot(pitch, yaw, _desiredTranslation + GetDesiredForwardDir() * distance);
        public void Pivot(float pitch, float yaw, Vec3 point)
        {
            _desiredTranslation = TMath.RotateAboutPoint(_desiredTranslation, point, new Rotator(pitch, yaw, 0.0f));
            _desiredRotation.AddRotations(pitch, yaw, 0.0f);
        }

        [Browsable(false)]
        public override bool IsRotatable => true;
        public override void HandleWorldRotation(Quat delta)
        {
            Quat q = _currentRotation.ToQuaternion();
            q = q * delta;
            _desiredRotation.SetRotations(q.ToYawPitchRoll());
            base.HandleWorldRotation(delta);
        }
        
        public Vec3 GetForwardDir() => _currentRotation.TransformVector(Vec3.Forward);
        public Vec3 GetRightDir() => _currentRotation.TransformVector(Vec3.Right);
        public Vec3 GetUpDir() => _currentRotation.TransformVector(Vec3.Up);
        public Vec3 GetDesiredForwardDir() => _desiredRotation.TransformVector(Vec3.Forward);
        public Vec3 GetDesiredRightDir() => _desiredRotation.TransformVector(Vec3.Right);
        public Vec3 GetDesiredUpDir() => _desiredRotation.TransformVector(Vec3.Up);
    }
}
