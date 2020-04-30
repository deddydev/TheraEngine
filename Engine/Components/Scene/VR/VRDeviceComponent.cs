using System;
using System.Drawing;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Components.Scene
{
    [TFileDef("VR Component")]
    public class VRDeviceComponent : OriginRebasableComponent, I3DRenderable
    {
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, false);
        IRenderInfo3D I3DRenderable.RenderInfo => RenderInfo;

        public int DeviceIndex { get; set; }
        public EngineVR.DeviceInfo Device => EngineVR.Devices[DeviceIndex];

        public VRDeviceComponent() 
        {
            Rendering.Models.Mesh pointData = Rendering.Models.Mesh.Create(Vec3.Zero);

            TMaterial mat = TMaterial.CreateUnlitColorMaterialForward(Color.Red);
            mat.RenderParams = new RenderingParameters { PointSize = 20.0f };

            _rcPoint.Mesh = new MeshRenderer(pointData, mat);

            //EngineVR.DeviceSet += EngineVR_DeviceSet;
        }

        //private void EngineVR_DeviceSet(int obj)
        //{
        //    if (obj == DeviceIndex && Device != null)
        //    {
        //        RegisterDeviceEvents();
        //    }
        //}

        private void RenderPose_Updated(EngineVR.DevicePoseInfo obj)
        {
            _rcPoint.WorldMatrix = ParentWorldMatrix * obj.DeviceToWorldMatrix;
        }

        private void UpdatePose_Updated(EngineVR.DevicePoseInfo obj)
        {
            RecalcLocalTransform();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            if (Device is null)
            {
                localTransform = Matrix4.Identity;
                inverseLocalTransform = Matrix4.Identity;
            }
            else
            {
                localTransform = Device.UpdatePose.DeviceToWorldMatrix;
                inverseLocalTransform = Device.UpdatePose.DeviceToWorldMatrix.Inverted();
            }
        }
        protected internal override void OnOriginRebased(Vec3 newOrigin)
        {

        }
        protected override void OnWorldTransformChanged(bool recalcChildWorldTransformsNow = true)
        {
            //_rcPoint.WorldMatrix = WorldMatrix;
            base.OnWorldTransformChanged(recalcChildWorldTransformsNow);
        }

        private readonly RenderCommandMesh3D _rcPoint = new RenderCommandMesh3D(ERenderPass.OpaqueForward);
        void I3DRenderable.AddRenderables(RenderPasses passes, ICamera camera)
        {
            passes.Add(_rcPoint);
        }

        internal void RegisterDeviceEvents()
        {
            Device.UpdatePose.Updated += UpdatePose_Updated;
            Device.RenderPose.Updated += RenderPose_Updated;
        }
    }
}
