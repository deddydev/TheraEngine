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
            _scale = new MonitoredVec3(Vec3.One);
            _scale.Changed += RecalcLocalTransform;
        }

        private MonitoredVec3 _scale;
        public MonitoredVec3 Scale
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
            _inverseLocalTransform = Matrix4.CreateScale(1.0f / _scale.Value) * _inverseLocalTransform;
            RecalcGlobalTransform();
        }
    }
}
