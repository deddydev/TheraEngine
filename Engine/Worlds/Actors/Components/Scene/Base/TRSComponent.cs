using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Components
{
    public class TRSComponent : TRComponent
    {
        public TRSComponent() : base()
        {
            _scale = Vec3.One;
            _scale.Changed += RecalcLocalTransform;
        }
        protected EventVec3 _scale;
        public EventVec3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
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
                it = Matrix4.CreateTranslation(-_translation);

            Matrix4
                s = Matrix4.CreateScale(_scale),
                iS = Matrix4.CreateScale(1.0f / _scale);

            SetLocalTransforms(t * r * s, iS * ir * it);
        }
    }
}
