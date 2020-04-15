using System;
using System.Drawing;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using static TheraEngine.Core.EngineVR;

namespace TheraEngine.Components.Scene
{
    [TFileDef("VR Component")]
    public class VRDeviceComponent : OriginRebasableComponent, I3DRenderable
    {
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, false);
        IRenderInfo3D I3DRenderable.RenderInfo => RenderInfo;

        public int DeviceIndex { get; set; }
        public DeviceInfo Device => Devices[DeviceIndex];

        public VRDeviceComponent() 
        {
            Rendering.Models.Mesh pointData = Rendering.Models.Mesh.Create(Vec3.Zero);
            TMaterial mat = TMaterial.CreateUnlitColorMaterialForward(Color.Red);
            mat.RenderParams = new RenderingParameters { PointSize = 5.0f };
            _rcPoint.Mesh = new PrimitiveManager(pointData, mat);
            _rcPoint.WorldMatrix = Matrix4.Identity;
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Device.UpdatePose.DeviceToWorldMatrix.Inverted();
            inverseLocalTransform = Device.UpdatePose.DeviceToWorldMatrix;
        }
        protected internal override void OnOriginRebased(Vec3 newOrigin)
        {

        }
        protected override void OnWorldTransformChanged(bool recalcChildWorldTransformsNow = true)
        {
            _rcPoint.WorldMatrix = WorldMatrix;
            base.OnWorldTransformChanged(recalcChildWorldTransformsNow);
        }

        private readonly RenderCommandMesh3D _rcPoint = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        void I3DRenderable.AddRenderables(RenderPasses passes, ICamera camera)
        {
            passes.Add(_rcPoint);
        }
    }
}
