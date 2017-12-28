using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Worlds.Actors.Components.Scene.Transforms
{
    /// <summary>
    /// Rotates first, then translates.
    /// </summary>
    [FileDef("Rotation-Translation Component")]
    public class RTComponent : PositionComponent
    {
        public RTComponent() : base()
        {
            Rotation = new Rotator();
        }
        public RTComponent(Rotator rotation, Vec3 translation)
        {
            SetRT(rotation, translation);
        }
        public void SetRT(Rotator rotation, Vec3 translation)
        {
            _rotation = rotation;
            _rotation.Changed += RecalcLocalTransform;
            _translation = translation;
            _translation.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }

        [TSerialize("Rotation")]
        protected Rotator _rotation;

        [Category("Transform")]
        public Rotator Rotation
        {
            get { return _rotation; }
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

            localTransform = r * t;
            inverseLocalTransform = it * ir;
        }
        protected internal override void OriginRebased(Vec3 newOrigin) { }
    }
}
