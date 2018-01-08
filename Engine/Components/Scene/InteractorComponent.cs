using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Worlds.Actors.Components.Scene
{
    /// <summary>
    /// 
    /// </summary>
    public class InteractorComponent : SceneComponent
    {
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            throw new NotImplementedException();
        }

        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            throw new NotImplementedException();
        }
    }
}
