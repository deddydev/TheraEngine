using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Transforms
{
    /// <summary>
    /// Interface for components that can be transformed via camera controls.
    /// </summary>
    public interface ICameraTransformable
    {
        Rotator Rotation { get; set; }
        EventVec3 Translation { get; set; }
        Vec3 WorldPoint { get; }

        /// <summary>
        /// Transforms the translation value relative to the current rotation.
        /// </summary>
        void TranslateRelative(Vec3 delta);
        /// <summary>
        /// Transforms the translation value relative to the current rotation.
        /// </summary>
        void TranslateRelative(float dX, float dY, float dZ);
        /// <summary>
        /// Rotates the translation and rotation values around a point
        /// that is a certain distance from the translation point in the direction of the rotation.
        /// </summary>
        void Pivot(float pitch, float yaw, float distance);
        /// <summary>
        /// Rotates the translation and rotation values around a specific focus point.
        /// </summary>
        void ArcBallRotate(float pitch, float yaw, Vec3 origin);
    }
    /// <summary>
    /// Translates first, then rotates.
    /// </summary>
    [FileDef("Translate-Rotate Component")]
    public class TRComponent : TranslationComponent, ICameraTransformable
    {
        public TRComponent() : this(Vec3.Zero, Rotator.GetZero(), true) { }
        public TRComponent(Vec3 translation, Rotator rotation, bool deferLocalRecalc = false) : base(translation, true)
        {
            _rotation = rotation;
            _rotation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }
        public TRComponent(Vec3 translation, bool deferLocalRecalc = false) : base(translation, true)
        {
            _rotation = Rotator.GetZero();
            _rotation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }
        public TRComponent(Rotator rotation, bool deferLocalRecalc = false) : base()
        {
            _rotation = rotation;
            _rotation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }

        public void SetTR(Vec3 translation, Rotator rotation)
        {
            _translation = translation;
            _translation.Changed += RecalcLocalTransform;
            _rotation = rotation;
            _rotation.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }

        [TSerialize("Rotation", UseCategory = true, OverrideXmlCategory = "Transform")]
        protected Rotator _rotation;

        [Category("Transform")]
        public Rotator Rotation
        {
            get
            {
                if (_matrixChanged)
                    DeriveMatrix();
                return _rotation;
            }
            set
            {
                _rotation = value ?? new Rotator();
                _rotation.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }

        protected override void DeriveMatrix()
        {
            Transform.DeriveTR(LocalMatrix, out Vec3 t, out Quat r);
            _translation.SetRawNoUpdate(t);
            _rotation.SetRotationsNoUpdate(r.ToYawPitchRoll());
        }

        protected internal override void OnDeserialized()
        {
            _rotation.Changed += RecalcLocalTransform;
            base.OnDeserialized();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4 
                r = _rotation.GetMatrix(), 
                ir = _rotation.Inverted().GetMatrix();

            Matrix4
                t = _translation.AsTranslationMatrix(), 
                it = (-_translation).AsTranslationMatrix();

            localTransform = t * r;
            inverseLocalTransform = ir * it;

            //Engine.PrintLine("Recalculated TR.");
        }

        #region Camera Translation
        public void TranslateRelative(float x, float y, float z)
            => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            _localTransform = LocalMatrix * translation.AsTranslationMatrix();
            _inverseLocalTransform = (-translation).AsTranslationMatrix() * InverseLocalMatrix;
            _translation.SetRawNoUpdate(LocalMatrix.Translation);
            RecalcWorldTransform();
        }
        public void Pivot(float pitch, float yaw, float distance)
            => ArcBallRotate(pitch, yaw, _translation + GetLocalForwardDir() * distance);
        public void ArcBallRotate(float pitch, float yaw, Vec3 focusPoint)
        {
            //"Arcball" rotation
            //All rotation is done within local component space
            _translation.Raw = TMath.ArcballTranslation(pitch, yaw, focusPoint, _translation.Raw, GetLocalRightDir());
            _rotation.AddRotations(pitch, yaw, 0.0f);
        }
        #endregion

        [Browsable(false)]
        public override bool IsRotatable => true;
        public override void HandleWorldRotation(Quat delta)
        {
            Quat q = _rotation.ToQuaternion();
            q = q * delta;
            _rotation.SetRotations(q.ToYawPitchRoll());
            base.HandleWorldRotation(delta);
        }

        public Vec3 GetLocalRightDir() => LocalMatrix.Row0.Xyz;
        public Vec3 GetLocalUpDir() => LocalMatrix.Row1.Xyz;
        public Vec3 GetLocalForwardDir() => LocalMatrix.Row2.Xyz;

        public Vec3 GetWorldRightDir() => WorldMatrix.Row0.Xyz;
        public Vec3 GetWorldUpDir() => WorldMatrix.Row1.Xyz;
        public Vec3 GetWorldForwardDir() => WorldMatrix.Row2.Xyz;
    }
}
