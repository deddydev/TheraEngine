using System;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials.Textures;
using Valve.VR;
using ETextureType = Valve.VR.ETextureType;

namespace TheraEngine.Core
{
    public delegate void DelRenderEye(RenderTex2D target);
    public static class EngineVR
    {
        public static EyeHandler LeftEye { get; } = new EyeHandler();
        public static EyeHandler RightEye { get; } = new EyeHandler();
        public static VRComponent VRComponent { get; set; }

        public class DevicePoseInfo
        {
            public event Action<DevicePoseInfo> ValidPoseChanged;
            public event Action<DevicePoseInfo> IsConnectedChanged;

            public ETrackingResult State { get; set; }
            public Matrix4 DeviceToWorldMatrix { get; set; }

            public Vec3 Velocity { get; set; }
            public Vec3 LastVelocity { get; set; }

            public Vec3 AngularVelocity { get; set; }
            public Vec3 LastAngularVelocity { get; set; }

            public bool ValidPose { get; set; }
            public bool IsConnected { get; set; }

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
            }
        }
        public class DeviceInfo
        {
            public DeviceInfo(uint index)
            {
                Index = index;
                Class = OpenVR.System.GetTrackedDeviceClass(index);
            }

            public uint Index { get; }
            public ETrackedDeviceClass Class { get; }
            public DevicePoseInfo RenderPose { get; } = new DevicePoseInfo();
            public DevicePoseInfo UpdatePose { get; } = new DevicePoseInfo();
        }
        
        public static DeviceInfo[] Devices { get; } = new DeviceInfo[OpenVR.k_unMaxTrackedDeviceCount];
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
            OpenVR.Compositor.SetTrackingSpace(TrackingOrigin);
        }

        public static void Shutdown()
        {
            if (!IsActive)
                return;

            OpenVR.Shutdown();
            IsActive = false;
        }
        public static void Render()
        {
            PreRender();
            //TODO: stereo render to both texture targets in parallel?
            LeftEye.DoRenderEye();
            RightEye.DoRenderEye();
        }
        private static void PreRender()
        {
            OpenVR.Compositor.WaitGetPoses(_renderPoses, _updatePoses);

            //string m_strPoseClasses = "";
            for (uint nDevice = 0; nDevice < Devices.Length; ++nDevice)
            {
                var device = Devices[nDevice];
                var rPose = _renderPoses[nDevice];
                var uPose = _updatePoses[nDevice];

                if (uPose.bPoseIsValid)
                {
                    if (device is null)
                        Devices[nDevice] = device = new DeviceInfo(nDevice);

                    device.UpdatePose.Update(uPose);
                }

                if (rPose.bPoseIsValid)
                {
                    if (device is null)
                        Devices[nDevice] = device = new DeviceInfo(nDevice);

                    device.RenderPose.Update(rPose);
                }
            }

            var hmdDevice = Devices[OpenVR.k_unTrackedDeviceIndex_Hmd];
            if (hmdDevice.UpdatePose.ValidPose)
                VRComponent.HMD.WorldMatrix = hmdDevice.UpdatePose.DeviceToWorldMatrix.Inverted();
        }

        /// <summary>
        /// Returns true if an error occurred.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool CheckError(EVRCompositorError error)
        {
            bool hasError = error != EVRCompositorError.None;
            if (hasError)
                Engine.Out(error.ToString());
            return hasError;
        }

        public class EyeHandler
        {
            public event DelRenderEye RenderEye;

            public RenderTex2D EyeTexture { get; set; }
            public EVREye EyeTarget { get; set; }
            public bool IsLeftEye
            {
                get => EyeTarget == EVREye.Eye_Left;
                set => EyeTarget = value ? EVREye.Eye_Left : EVREye.Eye_Right;
            }
            public float NearZ { get; set; } = 0.1f;
            public float FarZ { get; set; } = 1.0f;

            public Matrix4 GetEyeProjectionMatrix()
                => OpenVR.System.GetProjectionMatrix(EyeTarget, NearZ, FarZ);

            public Matrix4 GetEyeToHeadTransform()
                => OpenVR.System.GetEyeToHeadTransform(EyeTarget);

            private VRTextureBounds_t _eyeTexBounds = new VRTextureBounds_t()
            {
                uMin = 0.0f,
                uMax = 1.0f,
                vMin = 0.0f,
                vMax = 1.0f,
            };
            Texture_t _eyeTex = new Texture_t
            {
                eColorSpace = EColorSpace.Gamma,
                eType = ETextureType.OpenGL,
            };
            
            public void DoRenderEye()
            {
                if (EyeTexture is null)
                    return;
                
                RenderEye?.Invoke(EyeTexture);
                _eyeTex.handle = (IntPtr)EyeTexture.BindingId;
                CheckError(OpenVR.Compositor.Submit(EyeTarget, ref _eyeTex, ref _eyeTexBounds, EVRSubmitFlags.Submit_Default));
            }
        }
    }
}
