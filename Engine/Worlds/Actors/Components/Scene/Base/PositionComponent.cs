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
        public PositionComponent() : base()
        {
            _translation = new MonitoredVec3();
            _translation.Changed += RecalcLocalTransform;
        }

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
        public override void RecalcLocalTransform()
        {
            _localTransform = Matrix4.CreateTranslation(_translation);
            _inverseLocalTransform = Matrix4.CreateTranslation(-_translation.Value);
            RecalcGlobalTransform();
        }
        internal override void OriginRebased(Vec3 newOrigin)
        {
            Translation -= newOrigin;
        }
    }
}
