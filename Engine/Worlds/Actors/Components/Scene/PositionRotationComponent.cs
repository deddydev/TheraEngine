using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Components
{
    public class TRComponent : SceneComponent
    {
        public TRComponent() : base()
        {
            _rotation = new Rotator();
            _rotation.Changed += RecalcLocalTransform;
            _translation = new MonitoredVec3();
            _translation.Changed += RecalcLocalTransform;
        }

        protected Rotator _rotation;
        protected MonitoredVec3 _translation;

        public MonitoredVec3 Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                RecalcLocalTransform();
            }
        }
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
        public override void RecalcLocalTransform()
        {
            Matrix4 
                r = Matrix4.CreateFromRotator(_rotation), 
                ir = Matrix4.CreateFromRotator(_rotation.Inverted());

            Matrix4
                t = Matrix4.CreateTranslation(_translation), 
                it = Matrix4.CreateTranslation(-(Vec3)_translation);

            _localTransform = t * r;
            _invLocalTransform = ir * it;

            RecalcGlobalTransform();
        }
        public void TranslateRelative(Vec3 translation)
        {
            _localTransform = _localTransform * Matrix4.CreateTranslation(translation);
            _invLocalTransform = Matrix4.CreateTranslation(-translation) * _invLocalTransform;
            _translation = _localTransform.GetPoint();
        }
    }
}
