using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Transforms
{
    [TFileDef("Translate-Rotate-Scale Component")]
    public class TRSComponent : TRComponent
    {
        public TRSComponent() : this(Vec3.Zero, Rotator.GetZero(), Vec3.One, true) { }
        public TRSComponent(Vec3 translation, Rotator rotation, Vec3 scale, bool deferLocalRecalc = false) 
            : base(translation, rotation, true)
        {
            _scale = scale;
            _scale.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }

        public void SetTRS(Vec3 translation, Rotator rotation, Vec3 scale)
        {
            _translation = translation;
            _translation.Changed += RecalcLocalTransform;
            _rotation = rotation;
            _rotation.Changed += RecalcLocalTransform;
            _scale = scale;
            _scale.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }

        [TSerialize(nameof(Scale), UseCategory = true, OverrideCategory = "Transform")]
        protected EventVec3 _scale;

        [Category("Transform")]
        public EventVec3 Scale
        {
            get
            {
                if (_matrixChanged)
                    DeriveMatrix();
                return _scale;
            }
            set
            {
                _scale = value;
                _scale.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }

        protected override void DeriveMatrix()
        {
            Transform.DeriveTRS(LocalMatrix, out Vec3 t, out Quat r, out Vec3 s);
            _translation.SetRawNoUpdate(t);
            _scale.SetRawNoUpdate(s);
            _rotation.SetRawNoUpdate(r.ToYawPitchRoll());
        }

        protected internal override void OnDeserialized()
        {
            if (_scale == null)
                _scale = new EventVec3();
            _scale.Changed += RecalcLocalTransform;
            base.OnDeserialized();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4
                r = _rotation.GetMatrix(),
                ir = _rotation.Inverted().GetMatrix();

            Matrix4
                t = _translation.Raw.AsTranslationMatrix(),
                it = (-_translation.Raw).AsTranslationMatrix();

            Matrix4
                s = _scale.Raw.AsScaleMatrix(),
                iS = (1.0f / _scale.Raw).AsScaleMatrix();

            localTransform = t * r * s;
            inverseLocalTransform = iS * ir * it;

            //Engine.PrintLine("Recalculated TRS.");
        }

        [Browsable(false)]
        public override bool IsScalable => true;
        public override void HandleWorldScale(Vec3 delta)
        {
            _scale.Raw += delta;
        }
    }
}
