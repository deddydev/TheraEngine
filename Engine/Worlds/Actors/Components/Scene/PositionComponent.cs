using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Components
{
    public class PositionComponent : SceneComponent
    {
        protected Vec3 _translation;
        public Vec3 Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                RecalcLocalTransform();
            }
        }

        public PositionComponent() : base() { }
        public override void RecalcLocalTransform()
        {
            _localTransform = Matrix4.CreateTranslation(_translation);
            _invLocalTransform = Matrix4.CreateTranslation(-_translation);
            RecalcGlobalTransform();
        }
    }
}
