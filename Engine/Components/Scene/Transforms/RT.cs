using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Transforms
{
    /// <summary>
    /// Rotates first, then translates.
    /// </summary>
    [TFileDef("Rotation-Translation Component")]
    public class RTComponent : RotationComponent
    {
        public RTComponent() : this(Rotator.GetZero(), Vec3.Zero, true) { }
        public RTComponent(Rotator rotation, Vec3 translation, bool deferLocalRecalc = false) : base(rotation, true)
        {
            _translation = translation;
            _translation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }

        [TSerialize(nameof(Translation), UseCategory = true, OverrideCategory = "Transform")]
        protected EventVec3 _translation;
        
        [Category("Transform")]
        public EventVec3 Translation
        {
            get => _translation;
            set
            {
                _translation = value ?? new EventVec3();
                _translation.Changed += RecalcLocalTransform;
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
    }
}
