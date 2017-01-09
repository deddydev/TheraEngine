using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Components
{
    public class TRComponent : PositionComponent
    {
        public TRComponent() : base()
        {
            _rotation = new Rotator();
            _rotation.Changed += RecalcLocalTransform;
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
        protected override void RecalcLocalTransform()
        {
            Matrix4 
                r = Matrix4.CreateFromRotator(_rotation), 
                ir = Matrix4.CreateFromRotator(_rotation.Inverted());

            Matrix4
                t = Matrix4.CreateTranslation(_translation), 
                it = Matrix4.CreateTranslation(-_translation.Value);

            SetLocalTransforms(t * r, ir * it);
        }
        public void TranslateRelative(Vec3 translation)
        {
            SetLocalTransforms(
                LocalMatrix * Matrix4.CreateTranslation(translation),
                Matrix4.CreateTranslation(-translation) * InverseLocalMatrix);
            _translation = LocalMatrix.GetPoint();
        }
        internal override void OriginRebased(Vec3 newOrigin)
        {
            Translation -= newOrigin;
        }
    }
}
