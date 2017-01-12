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
            _translation = Vec3.Zero;
            _translation.Changed += RecalcLocalTransform;
        }

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
        protected override void RecalcLocalTransform()
        {
            SetLocalTransforms(Matrix4.CreateTranslation(_translation), Matrix4.CreateTranslation(-_translation));
        }
        internal override void OriginRebased(Vec3 newOrigin)
        {
            Translation -= newOrigin;
        }
    }
}
