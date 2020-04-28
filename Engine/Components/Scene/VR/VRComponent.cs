using System;
using System.Collections.Generic;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene
{
    [TFileDef("VR Component")]
    public class VRComponent : OriginRebasableComponent
    {
        public VRComponent()
        {
            HMD = new VRDeviceComponent() { AllowRemoval = false, DeviceIndex = 0 };
            LeftEye = new CameraComponent(new VRCamera()) { AllowRemoval = false };
            RightEye = new CameraComponent(new VRCamera()) { AllowRemoval = false };
            LeftHand = new VRDeviceComponent() { AllowRemoval = false, DeviceIndex = 3 };
            RightHand = new VRDeviceComponent() { AllowRemoval = false, DeviceIndex = 4 };

            ChildComponents.Add(HMD);
            HMD.ChildComponents.Add(LeftEye);
            HMD.ChildComponents.Add(RightEye);
            ChildComponents.Add(LeftHand);
            ChildComponents.Add(RightHand);

            TrackerPositions.PostAnythingAdded += TrackerPositions_PostAnythingAdded;
            TrackerPositions.PostAnythingRemoved += TrackerPositions_PostAnythingRemoved;
        }

        private void TrackerPositions_PostAnythingAdded(VRDeviceComponent item) => ChildComponents.Add(item);
        private void TrackerPositions_PostAnythingRemoved(VRDeviceComponent item) => ChildComponents.Remove(item);
        protected override void OnChildRemoved(ISceneComponent item)
        {
            if (item is VRDeviceComponent trc)
                TrackerPositions.Remove(trc);

            base.OnChildRemoved(item);
        }
        protected override void OnChildAdded(ISceneComponent item)
        {
            base.OnChildAdded(item);

            if (item is VRDeviceComponent trc)
                TrackerPositions.Add(trc);
        }

        public VRDeviceComponent HMD { get; set; }
        public CameraComponent LeftEye { get; }
        public CameraComponent RightEye { get; }
        public VRDeviceComponent LeftHand { get; set; }
        public VRDeviceComponent RightHand { get; set; }
        public EventList<VRDeviceComponent> TrackerPositions { get; } = new EventList<VRDeviceComponent>(false, false);

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.Identity;
            inverseLocalTransform = Matrix4.Identity;
        }
        protected internal override void OnOriginRebased(Vec3 newOrigin)
        {

        }
    }
}
