using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors
{
    /// <summary>
    /// Translates first, then rotates.
    /// </summary>
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

        protected Rotator _rotation;
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
        public void TranslateRelative(Vec3 translation)
        {
            _localTransform = LocalMatrix * Matrix4.CreateTranslation(translation);
            _inverseLocalTransform = Matrix4.CreateTranslation(-translation) * InverseLocalMatrix;
            _translation = LocalMatrix.GetPoint();
        }
        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            Translation -= newOrigin;
        }
    }
}
