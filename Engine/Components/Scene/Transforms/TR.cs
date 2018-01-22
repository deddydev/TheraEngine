﻿using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Worlds.Actors.Components.Scene.Transforms
{
    /// <summary>
    /// Translates first, then rotates.
    /// </summary>
    [FileDef("Translate-Rotate Component")]
    public class TRComponent : TranslationComponent
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
            get => _rotation;
            set
            {
                _rotation = value ?? new Rotator();
                _rotation.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }

        protected internal override void OnDeserialized()
        {
            _rotation.Changed += RecalcLocalTransform;
            base.OnDeserialized();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4 
                r = Matrix4.CreateFromRotator(_rotation), 
                ir = Matrix4.CreateFromRotator(_rotation.Inverted());

            Matrix4
                t = Matrix4.CreateTranslation(_translation), 
                it = Matrix4.CreateTranslation(-_translation);

            localTransform = t * r;
            inverseLocalTransform = ir * it;

            //Engine.PrintLine("Recalculated TR.");
        }
        public void TranslateRelative(float x, float y, float z)
            => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            _localTransform = LocalMatrix * Matrix4.CreateTranslation(translation);
            _inverseLocalTransform = Matrix4.CreateTranslation(-translation) * InverseLocalMatrix;
            _translation.SetRawNoUpdate(LocalMatrix.GetPoint());
            RecalcGlobalTransform();
        }
        
        [Browsable(false)]
        public override bool IsRotatable => true;
        public override void HandleWorldRotation(Quat delta)
        {
            Quat q = _rotation.ToQuaternion();
            q = q * delta;
            _rotation.SetRotations(q.ToYawPitchRoll());
            base.HandleWorldRotation(delta);
        }
    }
}
