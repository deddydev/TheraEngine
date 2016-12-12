using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Components
{
    public class GenericSceneComponent : PositionRotationComponent
    {
        public GenericSceneComponent() : base() { }

        private Vec3 _scale = Vec3.One;

        public Vec3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                RecalcLocalTransform();
            }
        }
        public override void RecalcLocalTransform()
        {
            base.RecalcLocalTransform();
            _localTransform = _localTransform * Matrix4.CreateScale(_scale);
            _invLocalTransform = Matrix4.CreateScale(1.0f / _scale) * _invLocalTransform;
            RecalcGlobalTransform();
        }
    }
}
