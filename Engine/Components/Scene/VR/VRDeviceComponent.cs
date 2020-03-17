using System;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using static TheraEngine.Core.EngineVR;

namespace TheraEngine.Components.Scene
{
    [TFileDef("VR Component")]
    public class VRDeviceComponent : OriginRebasableComponent, I3DRenderable
    {
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, false);
        IRenderInfo3D I3DRenderable.RenderInfo => RenderInfo;

        public int DeviceIndex { get; set; }
        public DeviceInfo Info => Devices[DeviceIndex];

        public VRDeviceComponent()
        {

        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Info.UpdatePose.DeviceToWorldMatrix.Inverted();
            inverseLocalTransform = Info.UpdatePose.DeviceToWorldMatrix;
        }
        protected internal override void OnOriginRebased(Vec3 newOrigin)
        {

        }

        void I3DRenderable.AddRenderables(RenderPasses passes, ICamera camera) => throw new NotImplementedException();
    }
}
