using System;

namespace TheraEngine.Components.Scene
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
