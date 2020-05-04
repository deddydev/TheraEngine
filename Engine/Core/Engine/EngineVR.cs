using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Rendering.Scene;
using Valve.VR;
using ETextureType = Valve.VR.ETextureType;

namespace TheraEngine.Core
{
    public delegate void DelRenderEye(RenderTex2D target);
    public static class EngineVR
    {
        public static VRViewport LeftEye { get; } = new VRViewport() { IsLeftEye = true };
        public static VRViewport RightEye { get; } = new VRViewport() { IsLeftEye = false };

        /// <summary>
        /// Called before a device is set at the given index.
        /// </summary>
        public static event Action<int> PreDeviceSet;
        /// <summary>
        /// Called after a device is set at the given index.
        /// </summary>
        public static event Action<int> PostDeviceSet;
        /// <summary>
        /// Contains information about a specific tracked pose (render or update).
        /// </summary>
        public class DevicePoseInfo : TObjectSlim
        {
            public event Action<DevicePoseInfo> ValidPoseChanged;
            public event Action<DevicePoseInfo> IsConnectedChanged;
            public event Action<DevicePoseInfo> Updated;

            public ETrackingResult State { get; private set; }
            public Matrix4 DeviceToWorldMatrix { get; private set; }

            public Vec3 Velocity { get; private set; }
            public Vec3 LastVelocity { get; private set; }

            public Vec3 AngularVelocity { get; private set; }
            public Vec3 LastAngularVelocity { get; private set; }

            public bool ValidPose { get; private set; }
            public bool IsConnected { get; private set; }

            public void Update(TrackedDevicePose_t pose)
            {
                State = pose.eTrackingResult;
                DeviceToWorldMatrix = pose.mDeviceToAbsoluteTracking;

                LastVelocity = Velocity;
                Velocity = pose.vVelocity;

                LastAngularVelocity = AngularVelocity;
                AngularVelocity = pose.vAngularVelocity;

                bool validChanged = ValidPose != pose.bPoseIsValid;
                ValidPose = pose.bPoseIsValid;

                bool isConnectedChanged = IsConnected != pose.bDeviceIsConnected;
                IsConnected = pose.bDeviceIsConnected;

                if (validChanged)
                    ValidPoseChanged?.Invoke(this);

                if (isConnectedChanged)
                    IsConnectedChanged?.Invoke(this);

                Updated?.Invoke(this);
            }
        }
        /// <summary>
        /// Contains pose information for the HMD, controllers, trackers, and base stations.
        /// </summary>
        public class VRDevice : TObjectSlim
        {
            public VRDevice(int index)
            {
                Index = index;
                uint idx = (uint)index;
                Class = OpenVR.System.GetTrackedDeviceClass(idx);

                ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
                int outInt32;

                Trace.WriteLine($"Instantiated VR device : {Class}");

                switch (Class)
                {
                    case ETrackedDeviceClass.HMD:
                        break;

                    case ETrackedDeviceClass.Controller:
                        outInt32 = OpenVR.System.GetInt32TrackedDeviceProperty(idx, ETrackedDeviceProperty.Prop_ControllerRoleHint_Int32, ref error);
                        if (VerifyNoPropertyError(error))
                        {
                            ControllerType = (ETrackedControllerRole)outInt32;
                            Trace.WriteLine($"{Class} role : {ControllerType}");
                        }
                        break;

                    case ETrackedDeviceClass.GenericTracker:

                        break;

                    case ETrackedDeviceClass.TrackingReference:

                        break;

                    case ETrackedDeviceClass.DisplayRedirect:

                        break;

                    default:
                    case ETrackedDeviceClass.Invalid:
                        break;
                }

                StringBuilder result = new StringBuilder(64);
                OpenVR.System.GetStringTrackedDeviceProperty((uint)index, ETrackedDeviceProperty.Prop_ControllerType_String, result, 64, ref error);
                if (VerifyNoPropertyError(error))
                {
                    Type = result.ToString();
                    Trace.WriteLine($"{Class} type : {Type}");
                }
            }

            private bool VerifyNoPropertyError(ETrackedPropertyError error) 
                => error == ETrackedPropertyError.TrackedProp_Success;

            public int Index { get; }
            public string Type { get;  set; }
            public ETrackedDeviceClass Class { get; } = ETrackedDeviceClass.Invalid;
            public ETrackedControllerRole ControllerType { get; } = ETrackedControllerRole.Invalid;
            public DevicePoseInfo RenderPose { get; } = new DevicePoseInfo();
            public DevicePoseInfo UpdatePose { get; } = new DevicePoseInfo();
        }

        //hmd = indexhmd
        //controllers = knuckles
        //waist = vive_tracker_waist
        //left foot = vive_tracker_left_foot
        //right foot = vive_tracker_right_foot

        public static VRDevice GetDevice(ETrackedDeviceClass deviceClass, string deviceType)
            => DeviceDictionary[deviceClass].FirstOrDefault(x => string.Equals(deviceType, x.Type, StringComparison.CurrentCultureIgnoreCase));

        public static List<VRDevice> GetDevices(ETrackedDeviceClass deviceClass)
            => DeviceDictionary[deviceClass];

        private static Dictionary<ETrackedDeviceClass, List<VRDevice>> DeviceDictionary { get; }
            = new Dictionary<ETrackedDeviceClass, List<VRDevice>>()
            {
                { ETrackedDeviceClass.HMD, new List<VRDevice>() },
                { ETrackedDeviceClass.TrackingReference, new List<VRDevice>() },
                { ETrackedDeviceClass.Controller, new List<VRDevice>() },
                { ETrackedDeviceClass.GenericTracker, new List<VRDevice>() },
                { ETrackedDeviceClass.DisplayRedirect, new List<VRDevice>() },
            };

        public static VRDevice[] Devices { get; } = new VRDevice[OpenVR.k_unMaxTrackedDeviceCount];

        public static TrackedDevicePose_t[] _renderPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        public static TrackedDevicePose_t[] _updatePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        
        public static Matrix4 HMDToWorldTransform { get; private set; }

        private static bool _isActive = false;
        private static bool _seatedMode = false;

        public static bool IsActive 
        {
            get => _isActive;
            private set => _isActive = value;
        }
        public static bool SeatedMode
        {
            get => _seatedMode;
            set
            {
                if (_seatedMode == value)
                    return;

                _seatedMode = value;

                if (IsActive)
                    OpenVR.Compositor.SetTrackingSpace(TrackingOrigin);
            }
        }

        private static ETrackingUniverseOrigin TrackingOrigin
            => SeatedMode
                ? ETrackingUniverseOrigin.TrackingUniverseSeated
                : ETrackingUniverseOrigin.TrackingUniverseStanding;

        public static EVRInitError Initialize()
        {
            if (IsActive)
                return EVRInitError.None;

            if (!OpenVR.IsRuntimeInstalled())
            {
                Engine.Out("VR runtime not installed.");
                return EVRInitError.Init_InstallationNotFound;
            }
            if (!OpenVR.IsHmdPresent())
            {
                Engine.Out("VR headset not found.");
                return EVRInitError.Init_HmdNotFound;
            }

            EVRInitError peError = EVRInitError.None;

            try
            {
                if (OpenVR.Init(ref peError, EVRApplicationType.VRApplication_Scene) is null)
                    Engine.LogWarning(peError.ToString());
                else
                    Engine.Out("OpenVR initialized successfully.");
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }

            if (peError == EVRInitError.None)
                OnStarted();

            IsActive = peError == EVRInitError.None;
            return peError;
        }

        private static void OnStarted()
        {
            OpenVR.Compositor.ForceInterleavedReprojectionOn(true);
            OpenVR.Compositor.SetTrackingSpace(TrackingOrigin);
            uint w = 0u, h = 0u;
            OpenVR.System.GetRecommendedRenderTargetSize(ref w, ref h);
            Engine.Out($"Eye resolution: {w}x{h}");
            LeftEye.Resize((int)w, (int)h);
            RightEye.Resize((int)w, (int)h);
            CreateContext();
        }

        public static void LinkToWorldManager(int worldManagerId)
            => Engine.DomainProxy.LinkVRToWorldManager(worldManagerId);

        public static void UnlinkFromWorldManager()
            => Engine.DomainProxy.UnlinkVRFromWorldManager();

        /// <summary>
        /// Arguments to pass into the constructor of the render handler.
        /// All arguments must be serializable.
        /// </summary>
        public static object[] HandlerArgs { get; set; } = new object[0];
        public static void CreateContext()
        {
            Instance_DomainProxyUnset(Engine.DomainProxy);
            Instance_DomainProxySet(Engine.DomainProxy);
        }

        private static void Instance_DomainProxyUnset(EngineDomainProxy proxy)
        {
            VRComponent = null;

            proxy?.UnregisterVRContext();
        }
        private static void Instance_DomainProxySet(EngineDomainProxy proxy)
        {
            proxy?.RegisterVRContext();

            VRComponent = (proxy?.VRContext?.Handler as VRRenderHandler)?.VRComponent;

            if (VRComponent is null)
                Engine.Out("VRComponent should not be null!");
            else
            {
                VRCamera left = VRComponent.LeftEye.Camera;
                VRCamera right = VRComponent.RightEye.Camera;

                UpdateCamera(left, LeftEye);
                left.PropertyChanged += LeftEyeCamera_PropertyChanged;

                UpdateCamera(right, RightEye);
                right.PropertyChanged += RightEyeCamera_PropertyChanged;
            }
        }

        private static void UpdateCamera(VRCamera cam, VRViewport eye)
        {
            if (cam is null || eye is null)
                return;

            Matrix4 tfm = eye.GetEyeToHeadTransform();
            cam.CameraToComponentSpaceMatrix = tfm;

            Matrix4 prj = eye.GetEyeProjectionMatrix(cam.NearZ, cam.FarZ);
            cam.SetProjectionMatrix(prj);

            eye.AttachedCamera = cam;
        }

        private static void LeftEyeCamera_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => CamPropChanged(VRComponent.LeftEye.Camera, LeftEye, e.PropertyName);

        private static void RightEyeCamera_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => CamPropChanged(VRComponent.RightEye.Camera, RightEye, e.PropertyName);

        private static void CamPropChanged(VRCamera cam, VRViewport eye, string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Camera.NearZ):
                case nameof(Camera.FarZ):
                    cam.SetProjectionMatrix(eye.GetEyeProjectionMatrix(cam.NearZ, cam.FarZ));
                    break;
            }
        }

        private static VRPlaySpaceComponent VRComponent { get; set; }

        public static void Shutdown()
        {
            if (!IsActive)
                return;

            OpenVR.Shutdown();
            IsActive = false;
        }

        public static void PreRenderUpdate()
        {
            UpdateLogicPoses();
            LeftEye.PreRenderUpdate();
            RightEye.PreRenderUpdate();
        }
        public static void SwapBuffers()
        {
            OpenVR.Compositor.WaitGetPoses(_renderPoses, _updatePoses);
            LeftEye.PreRenderSwap();
            RightEye.PreRenderSwap();
        }
        public static void Render()
        {
            UpdateRenderPoses();
            //TODO: stereo render to both texture targets in parallel?
            LeftEye.Render();
            RightEye.Render();
            LeftEye.Submit();
            RightEye.Submit();
            OpenVR.Compositor.PostPresentHandoff();
        }
        private static void UpdateAllPoses()
        {
            for (int nDevice = 0; nDevice < Devices.Length; ++nDevice)
            {
                GetOrCacheDevice(_updatePoses, nDevice, out TrackedDevicePose_t uPose)?.UpdatePose?.Update(uPose);
                GetOrCacheDevice(_renderPoses, nDevice, out TrackedDevicePose_t rPose)?.RenderPose?.Update(rPose);
            }
        }
        private static void UpdateLogicPoses()
        {
            for (int nDevice = 0; nDevice < Devices.Length; ++nDevice)
                GetOrCacheDevice(_updatePoses, nDevice, out TrackedDevicePose_t pose)?.UpdatePose?.Update(pose);
        }
        private static void UpdateRenderPoses()
        {
            for (int nDevice = 0; nDevice < Devices.Length; ++nDevice)
                GetOrCacheDevice(_renderPoses, nDevice, out TrackedDevicePose_t pose)?.RenderPose?.Update(pose);
        }

        private static VRDevice GetOrCacheDevice(TrackedDevicePose_t[] poses, int nDevice, out TrackedDevicePose_t pose)
        {
            pose = poses[nDevice];
            VRDevice device = Devices[nDevice];

            if (pose.bDeviceIsConnected)
            {
                if (device is null)
                {
                    PreDeviceSet?.Invoke(nDevice);
                    Devices[nDevice] = device = new VRDevice(nDevice);
                    DeviceDictionary[device.Class].Add(device);
                    PostDeviceSet?.Invoke(nDevice);
                }
            }
            else
            {
                if (device != null)
                {
                    PreDeviceSet?.Invoke(nDevice);
                    DeviceDictionary[device.Class].Remove(device);
                    Devices[nDevice] = device = null;
                    PostDeviceSet?.Invoke(nDevice);
                }
            }

            return device;
        }

        /// <summary>
        /// Wrap this around OpenVR calls that return the compositor error enum.
        /// Returns true and prints out a warning if an error has occurred.
        /// </summary>
        /// <param name="error">The error to check.</param>
        /// <returns></returns>
        public static bool CheckError(EVRCompositorError error)
        {
            bool hasError = error != EVRCompositorError.None;
            if (hasError)
                Engine.LogWarning($"OpenVR compositor error: {error}");
            return hasError;
        }
    }
}
