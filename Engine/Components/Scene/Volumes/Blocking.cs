using System;
using TheraEngine.Rendering;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Volumes
{
    public class BlockingVolumeComponent : BoxComponent
    {
        public BlockingVolumeComponent(Vec3 halfExtents, Vec3 translation, Rotator rotation, ushort collisionGroup, ushort collidesWith)
            : base(halfExtents, new TRigidBodyConstructionInfo()
            {
                Mass = 0.0f,
                CollisionEnabled = true,
                SimulatePhysics = false,
                CollisionGroup = collisionGroup,
                CollidesWith = collidesWith,
            })
        {
            RenderInfo.CastsShadows = false;
            RenderInfo.ReceivesShadows = false;
            RenderInfo.RenderPass = ERenderPass.OpaqueForward;
            //RenderParams.DepthTest.Enabled = false;

            Translation.Raw = translation;
            Rotation.SetRotations(rotation);
        }

#if EDITOR

        protected internal override void OnSelectedChanged(bool selected)
        {
            if (IsSpawned)
            {
                if (selected)
                    OwningScene.Add(this);
                else
                    OwningScene.Remove(this);
            }
            base.OnSelectedChanged(selected);
        }
#endif
    }
}
