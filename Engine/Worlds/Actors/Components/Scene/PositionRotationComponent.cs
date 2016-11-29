using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Components
{
    public class PositionRotationComponent : SceneComponent
    {
        protected Rotator _rotation;
        protected Vec3 _translation;
        protected bool _rotateFirst;

        public Vec3 Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                RecalcLocalTransform();
            }
        }
        public Vec3 Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                RecalcLocalTransform();
            }
        }
        public bool RotateFirst
        {
            get { return _rotateFirst; }
            set
            {
                _rotateFirst = value;
                RecalcLocalTransform();
            }
        }

        public PositionRotationComponent() : base() { _rotateFirst = false; }
        public PositionRotationComponent(bool rotateFirst = false) : base() { _rotateFirst = rotateFirst; }
        public override void RecalcLocalTransform()
        {
            Matrix4 
                r = Matrix4.CreateFromRotator(_rotation), 
                ir = Matrix4.CreateFromRotator(_rotation.Inverted());

            Matrix4 
                t = Matrix4.CreateTranslation(_translation), 
                it = Matrix4.CreateTranslation(-_translation);

            if (_rotateFirst)
            {
                _localTransform = r * t;
                _invLocalTransform = it * ir;
            }
            else
            {
                _localTransform = t * r;
                _invLocalTransform = ir * it;
            }
            RecalcGlobalTransform();
        }

        public void TranslateAbsolute(Vec3 translation)
        {
            if (_rotateFirst)
            {
                _localTransform = Matrix4.CreateTranslation(translation) * _localTransform;
                _invLocalTransform = _invLocalTransform * Matrix4.CreateTranslation(-translation);
                _translation = _localTransform.GetPoint();
            }
        }
    }
}
