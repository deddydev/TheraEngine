using System;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors
{
    public class BlockingVolumeComponent : BoxComponent
    {
        public BlockingVolumeComponent(Vec3 halfExtents, Vec3 translation, Rotator rotation, CustomCollisionGroup collisionGroup, CustomCollisionGroup collidesWith)
            : base(halfExtents, new PhysicsConstructionInfo()
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
            RenderInfo.RenderPass = ERenderPassType3D.OnTopForward;
            RenderParams.DepthTest.Enabled = false;

            Translation.Raw = translation;
            Rotation.SetRotations(rotation);
        }

#if EDITOR
        public override void OnSpawned()
        {
            if (!Engine.EditorState.InGameMode)
                Engine.Scene.Add(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (!Engine.EditorState.InGameMode)
                Engine.Scene.Remove(this);
            base.OnDespawned();
        }
#endif
    }
}
