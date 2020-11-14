using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Transforms
{
    public interface ITRSComponent : ITRComponent
    {
        EventVec3 Scale { get; set; }

        void SetTRS(EventVec3 translation, Rotator rotation, EventVec3 scale);
        void SetTRSRaw(Vec3 translation, Rotator rotation, Vec3 scale);
    }
    [TFileDef("Translate-Rotate-Scale Component")]
    public class TRSComponent : TRComponent, ITRSComponent
    {
        public TRSComponent() : this(Vec3.Zero, Rotator.GetZero(), Vec3.One, true) { }
        public TRSComponent(EventVec3 translation, Rotator rotation, EventVec3 scale, bool deferLocalRecalc = false) 
            : base(translation, rotation, true)
        {
            _scale = scale ?? new EventVec3();
            _scale.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }

        public void SetTRS(EventVec3 translation, Rotator rotation, EventVec3 scale)
        {
            _translation = translation ?? new EventVec3();
            _translation.Changed += RecalcLocalTransform;
            _rotation = rotation ?? new Rotator();
            _rotation.Changed += RecalcLocalTransform;
            _scale = scale ?? new EventVec3();
            _scale.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }
        protected override bool AllowRecalcLocalTransform() => _allowLocalRecalc;
        private bool _allowLocalRecalc = true;
        public void SetTRSRaw(Vec3 translation, Rotator rotation, Vec3 scale)
        {
            _allowLocalRecalc = false;
            _translation.Value = translation;
            _rotation.SetRotations(rotation);
            _scale.Value = scale;
            _allowLocalRecalc = true;
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
            Transform.DeriveTRS(LocalMatrix, out Vec3 t, out Vec3 s, out Quat r);
            _translation.SetRawSilent(t);
            _scale.SetRawSilent(s);
            _rotation.SetRotationsNoUpdate(r.ToRotator());
        }

        protected internal override void OnDeserialized()
        {
            if (_scale is null)
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
                t = _translation.Value.AsTranslationMatrix(),
                it = (-_translation.Value).AsTranslationMatrix();

            Matrix4
                s = _scale.Value.AsScaleMatrix(),
                iS = (1.0f / _scale.Value).AsScaleMatrix();

            localTransform = t * r * s;
            inverseLocalTransform = iS * ir * it;

            //Engine.PrintLine("Recalculated TRS.");
        }

        [Browsable(false)]
        public override bool IsScalable => true;
        public override void HandleScale(Vec3 delta)
        {
            _scale.Value += delta;
        }
    }
}
