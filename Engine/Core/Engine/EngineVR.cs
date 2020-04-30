using System;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering;
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
        public static EyeHandler LeftEye { get; } = new EyeHandler() { IsLeftEye = true };
        public static EyeHandler RightEye { get; } = new EyeHandler() { IsLeftEye = false };

        public static event Action<int> DeviceSet;

        public class DevicePoseInfo : TObjectSlim
        {
            public event Action<DevicePoseInfo> ValidPoseChanged;
            public event Action<DevicePoseInfo> IsConnectedChanged;
            public event Action<DevicePoseInfo> Updated;

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

                Updated?.Invoke(this);
            }
        }
        public class DeviceInfo : TObjectSlim
        {
            public DeviceInfo(int index)
            {
                Index = index;
                uint idx = (uint)index;
                Class = OpenVR.System.GetTrackedDeviceClass(idx);

                ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
                int outInt32;

                switch (Class)
                {
                    case ETrackedDeviceClass.HMD:
                        break;

                    case ETrackedDeviceClass.Controller:
                        outInt32 = OpenVR.System.GetInt32TrackedDeviceProperty(idx, ETrackedDeviceProperty.Prop_ControllerRoleHint_Int32, ref error);
                        if (VerifyNoPropertyError(error))
                            ControllerType = (ETrackedControllerRole)outInt32;
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
            }

            private bool VerifyNoPropertyError(ETrackedPropertyError error)
            {
                return error == ETrackedPropertyError.TrackedProp_Success;
            }

            public int Index { get; }
            public ETrackedDeviceClass Class { get; } = ETrackedDeviceClass.Invalid;
            public ETrackedControllerRole ControllerType { get; } = ETrackedControllerRole.Invalid;
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
            uint w = 0u, h = 0u;
            OpenVR.System.GetRecommendedRenderTargetSize(ref w, ref h);
            Engine.Out($"Eye resolution: {w}x{h}");
            LeftEye.Viewport.Resize((int)w, (int)h);
            RightEye.Viewport.Resize((int)w, (int)h);
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
                VRCamera left = VRComponent.LeftEye.Camera as VRCamera;
                VRCamera right = VRComponent.RightEye.Camera as VRCamera;

                UpdateCamera(VRComponent.LeftEye, left, LeftEye);
                left.PropertyChanged += LeftEyeCamera_PropertyChanged;

                UpdateCamera(VRComponent.RightEye, right, RightEye);
                right.PropertyChanged += RightEyeCamera_PropertyChanged;
            }
        }

        private static void UpdateCamera(CameraComponent comp, VRCamera rc, EyeHandler handler)
        {
            if (rc is null || handler is null)
                return;

            Matrix4 tfm = handler.GetEyeToHeadTransform();
            rc.CameraToComponentSpaceMatrix = tfm;

            Matrix4 prj = handler.GetEyeProjectionMatrix(rc.NearZ, rc.FarZ);
            rc.SetProjectionMatrix(prj);

            handler.Viewport.AttachedCamera = rc;

            //Engine.Out($"Right eye: [{tfm}] [{prj}]");
        }

        private static void LeftEyeCamera_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => CamPropChanged(VRComponent.LeftEye.Camera as VRCamera, LeftEye, e.PropertyName);

        private static void RightEyeCamera_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => CamPropChanged(VRComponent.RightEye.Camera as VRCamera, RightEye, e.PropertyName);

        private static void CamPropChanged(VRCamera cam, EyeHandler eye, string propertyName)
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
            //UpdateLogicPoses();
            LeftEye.PreRenderUpdate();
            RightEye.PreRenderUpdate();
        }
        public static void SwapBuffers()
        {
            UpdateAllPoses();
            LeftEye.PreRenderSwap();
            RightEye.PreRenderSwap();
        }
        public static void Render()
        {
            //TODO: stereo render to both texture targets in parallel?
            OpenVR.Compositor.WaitGetPoses(_renderPoses, _updatePoses);
            //UpdateRenderPoses();
            LeftEye.Render();
            RightEye.Render();
            LeftEye.Submit();
            RightEye.Submit();
        }
        private static void UpdateAllPoses()
        {
            for (int nDevice = 0; nDevice < Devices.Length; ++nDevice)
            {
                var uPose = _updatePoses[nDevice];
                if (uPose.bPoseIsValid)
                {
                    var device = Devices[nDevice];
                    if (device is null)
                    {
                        Devices[nDevice] = device = new DeviceInfo(nDevice);
                        DeviceSet?.Invoke(nDevice);
                    }
                    device.UpdatePose.Update(uPose);

                    //Engine.Out($"Device {nDevice} update pose: {device.UpdatePose.DeviceToWorldMatrix}");
                }

                var rPose = _renderPoses[nDevice];
                if (rPose.bPoseIsValid)
                {
                    var device = Devices[nDevice];
                    if (device is null)
                    {
                        Devices[nDevice] = device = new DeviceInfo(nDevice);
                        DeviceSet?.Invoke(nDevice);
                    }
                    device.RenderPose.Update(rPose);

                    //Engine.Out($"Device {nDevice} render pose: {device.UpdatePose.DeviceToWorldMatrix}");
                }
            }
        }
        private static void UpdateLogicPoses()
        {
            for (int nDevice = 0; nDevice < Devices.Length; ++nDevice)
            {
                var uPose = _updatePoses[nDevice];
                if (uPose.bPoseIsValid)
                {
                    var device = Devices[nDevice];
                    if (device is null)
                    {
                        Devices[nDevice] = device = new DeviceInfo(nDevice);
                        DeviceSet?.Invoke(nDevice);
                    }
                    device.UpdatePose.Update(uPose);

                    //Engine.Out($"Device {nDevice} update pose: {device.UpdatePose.DeviceToWorldMatrix}");
                }
            }
        }
        private static void UpdateRenderPoses()
        {
            for (int nDevice = 0; nDevice < Devices.Length; ++nDevice)
            {
                var rPose = _renderPoses[nDevice];
                if (rPose.bPoseIsValid)
                {
                    var device = Devices[nDevice];
                    if (device is null)
                    {
                        Devices[nDevice] = device = new DeviceInfo(nDevice);
                        DeviceSet?.Invoke(nDevice);
                    }
                    device.RenderPose.Update(rPose);

                    //Engine.Out($"Device {nDevice} render pose: {device.UpdatePose.DeviceToWorldMatrix}");
                }
            }
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
                Engine.Out($"OpenVR compositor error: {error}");
            return hasError;
        }

        public class EyeHandler
        {
            public VRViewport Viewport { get; } = new VRViewport();
            public EVREye EyeTarget { get; set; }
            public bool IsLeftEye
            {
                get => EyeTarget == EVREye.Eye_Left;
                set => EyeTarget = value ? EVREye.Eye_Left : EVREye.Eye_Right;
            }

            public Matrix4 GetEyeProjectionMatrix(float nearZ, float farZ)
                => OpenVR.System.GetProjectionMatrix(EyeTarget, nearZ, farZ);

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
                eColorSpace = EColorSpace.Auto,
                eType = ETextureType.OpenGL,
            };
            public void PreRenderUpdate()
                => Viewport.PreRenderUpdate();
            public void PreRenderSwap()
                => Viewport.PreRenderSwap();
            public void Render()
            {
                _eyeTex.handle = Viewport.VRRender();
                Engine.Out($"Rendered {(IsLeftEye ? "left" : "right")} eye");
            }
            public void Submit()
            {
                CheckError(OpenVR.Compositor.Submit(EyeTarget, ref _eyeTex, ref _eyeTexBounds, EVRSubmitFlags.Submit_Default));
                Engine.Out($"Submitted {(IsLeftEye ? "left" : "right")} eye");
            }
        }
    }
}
