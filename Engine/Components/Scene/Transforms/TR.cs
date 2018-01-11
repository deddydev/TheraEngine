using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Worlds.Actors.Components.Scene.Transforms
{
    /// <summary>
    /// Translates first, then rotates.
    /// </summary>
    [FileDef("Translate-Rotate Component")]
    public class TRComponent : PositionComponent
    {
        public TRComponent() : base()
        {
            _rotation = new Rotator();
            _rotation.Changed += RecalcLocalTransform;
        }
        public TRComponent(Vec3 translation, Rotator rotation)
        {
            SetTR(translation, rotation);
        }
        public void SetTR(Vec3 translation, Rotator rotation)
        {
            _translation = translation;
            _translation.Changed += RecalcLocalTransform;
            _rotation = rotation;
            _rotation.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }

        [TSerialize("Rotation")]
        protected Rotator _rotation;

        [Category("Transform")]
        public Rotator Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
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
        }
        public void TranslateRelative(float x, float y, float z)
            => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            _localTransform = LocalMatrix * Matrix4.CreateTranslation(translation);
            _inverseLocalTransform = Matrix4.CreateTranslation(-translation) * InverseLocalMatrix;
            _translation = LocalMatrix.GetPoint();
            RecalcGlobalTransform();
        }
        public override void HandleLocalRotation(Quat delta)
        {
            Quat q = _rotation.ToQuaternion();
            q = q * delta;
            _rotation.SetRotations(q.ToYawPitchRoll());
            base.HandleLocalRotation(delta);
        }
    }
}
