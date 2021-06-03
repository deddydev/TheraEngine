using TheraEngine.Rendering;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Volumes
{
    public class BlockingVolumeComponent : BoxComponent
    {
        public BlockingVolumeComponent()
            : this(Vec3.Half, Vec3.Zero, Rotator.GetZero(), 0, 0) { }
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
            RenderCommand.RenderPass = ERenderPass.OpaqueForward;
            //RenderParams.DepthTest.Enabled = false;

            Transform.Translation.Value = translation;
            Transform.Rotation.Value = rotation.ToQuaternion();
        }
    }
}
