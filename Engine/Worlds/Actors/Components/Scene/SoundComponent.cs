using CustomEngine.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors
{
    public class SoundComponent : SceneComponent
    {
        SoundFile _file;
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            throw new NotImplementedException();
        }
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();

        }
        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            throw new NotImplementedException();
        }
    }
}
