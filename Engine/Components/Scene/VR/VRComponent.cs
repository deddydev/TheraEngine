using System;
using System.Collections.Generic;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using Valve.VR;

namespace TheraEngine.Components.Scene
{
    [TFileDef("VR Component")]
    public class VRComponent : OriginRebasableComponent
    {
        public VRComponent()
        {
            LeftEye = new CameraComponent(new VRCamera() { NearZ = 0.1f, FarZ = 10000.0f }) { AllowRemoval = false };
            RightEye = new CameraComponent(new VRCamera() { NearZ = 0.1f, FarZ = 10000.0f }) { AllowRemoval = false };

            for (int i = 0; i < EngineVR.Devices.Length; ++i)
                EngineVR_DeviceSet(i);

            EngineVR.DeviceSet += EngineVR_DeviceSet;
        }

        public Dictionary<ETrackedDeviceClass, List<VRDeviceComponent>> Devices { get; }
            = new Dictionary<ETrackedDeviceClass, List<VRDeviceComponent>>();

        private void EngineVR_DeviceSet(int index)
        {
            var device = EngineVR.Devices[index];
            if (device is null)
                return;

            VRDeviceComponent comp = new VRDeviceComponent()
            {
                AllowRemoval = false,
                DeviceIndex = index
            };
            ChildComponents.Add(comp);

            var dclass = device.Class;
            if (Devices.ContainsKey(dclass))
                Devices[dclass].Add(comp);
            else
                Devices.Add(dclass, new List<VRDeviceComponent>() { comp });

            switch (dclass)
            {
                case ETrackedDeviceClass.HMD:
                    {
                        HMD = comp;
                        comp.ChildComponents.Add(LeftEye);
                        comp.ChildComponents.Add(RightEye);
                    }
                    break;
                case ETrackedDeviceClass.Controller:
                    {
                        switch (device.ControllerType)
                        {
                            default:
                            case ETrackedControllerRole.Invalid:
                                break;

                            case ETrackedControllerRole.LeftHand:
                                LeftHand = comp;
                                break;

                            case ETrackedControllerRole.RightHand:
                                RightHand = comp;
                                break;
                        }
                    }
                    break;
                case ETrackedDeviceClass.GenericTracker:
                    _trackers.Add(comp);
                    break;
            }
        }

        public VRDeviceComponent HMD { get; private set; }
        public CameraComponent LeftEye { get; private set; }
        public CameraComponent RightEye { get; private set; }
        public VRDeviceComponent LeftHand { get; private set; }
        public VRDeviceComponent RightHand { get; private set; }

        private List<VRDeviceComponent> _trackers = new List<VRDeviceComponent>();
        public IReadOnlyList<VRDeviceComponent> Trackers => _trackers;

        protected internal override void OnOriginRebased(Vec3 newOrigin)
        {

        }
    }
}
