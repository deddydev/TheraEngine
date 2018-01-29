using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Components.Scene.Transforms
{
    public abstract class OriginRebasableComponent : SceneComponent
    {
        /// <summary>
        /// This is to handle translating the root component of an actor when the world's origin is changed.
        /// </summary>
        /// <param name="newOrigin">The translation of the new origin relative to the current origin. 
        /// Subtract this value from a world translation to correct it.</param>
        protected internal abstract void OriginRebased(Vec3 newOrigin);
    }
}
