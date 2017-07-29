using System;
using System.ComponentModel;

namespace TheraEngine.Worlds.Actors
{
    public class TRSComponent : TRComponent
    {
        public TRSComponent() : base()
        {
            _scale = Vec3.One;
            _scale.Changed += RecalcLocalTransform;
        }
        public TRSComponent(Vec3 translation, Rotator rotation, Vec3 scale)
        {
            SetTRS(translation, rotation, scale);
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

        protected EventVec3 _scale;
        
        public EventVec3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                _scale.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4
                r = Matrix4.CreateFromRotator(_rotation),
                ir = Matrix4.CreateFromRotator(_rotation.Inverted());

            Matrix4
                t = Matrix4.CreateTranslation(_translation),
                it = Matrix4.CreateTranslation(-_translation);

            Matrix4
                s = Matrix4.CreateScale(_scale),
                iS = Matrix4.CreateScale(1.0f / _scale);

            localTransform = t * r * s;
            inverseLocalTransform = iS * ir * it;
        }
    }
}
