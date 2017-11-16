using System;
using TheraEngine.Rendering;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Worlds.Actors.Components.Scene.Shapes;

namespace TheraEngine.Worlds.Actors.Components.Scene.Volumes
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
            RenderInfo.RenderPass = ERenderPass3D.OpaqueDeferredLit;
            RenderParams.DepthTest.Enabled = false;

            Translation.Raw = translation;
            Rotation.SetRotations(rotation);
        }

        public override void OnSpawned()
        {
#if EDITOR
            if (!Engine.EditorState.InGameMode)
                Engine.Scene.Add(this);
#endif
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
#if EDITOR
            if (!Engine.EditorState.InGameMode)
                Engine.Scene.Remove(this);
#endif
            base.OnDespawned();
        }
    }
}
