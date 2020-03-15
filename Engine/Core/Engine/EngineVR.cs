using System;
using TheraEngine.Components.Scene;
using TheraEngine.Rendering.Models.Materials.Textures;
using Valve.VR;
using ETextureType = Valve.VR.ETextureType;

namespace TheraEngine.Core
{
    public delegate void DelRenderEye(RenderTex2D target);
    public class EngineVR
    {
        public EyeHandler LeftEye { get; } = new EyeHandler();
        public EyeHandler RightEye { get; } = new EyeHandler();
        public VRComponent HMDViewComponent { get; set; }

        TrackedDevicePose_t[] _renderPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        TrackedDevicePose_t[] _updatePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        Matrix4[] _renderTransforms = new Matrix4[OpenVR.k_unMaxTrackedDeviceCount];
        Matrix4[] _updateTransforms = new Matrix4[OpenVR.k_unMaxTrackedDeviceCount];
        public Matrix4 HMDToWorldTransform { get; private set; }

        private bool _isActive = false;
        private bool _seatedMode = false;

        public bool IsActive 
        {
            get => _isActive;
            private set => _isActive = value;
        }
        public bool SeatedMode
        {
            get => _seatedMode;
            set => _seatedMode = value;
        }

        private ETrackingUniverseOrigin TrackingOrigin
            => SeatedMode
                ? ETrackingUniverseOrigin.TrackingUniverseSeated
                : ETrackingUniverseOrigin.TrackingUniverseStanding;

        public EVRInitError Initialize()
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

            return peError;
        }

        private void OnStarted()
        {
            OpenVR.Compositor.SetTrackingSpace(TrackingOrigin);
        }

        public void Shutdown()
        {
            if (!IsActive)
                return;

            OpenVR.Shutdown();
            IsActive = false;
        }
        public void Render()
        {
            PreRender();
            //TODO: stereo render to both texture targets in parallel?
            LeftEye.DoRenderEye();
            RightEye.DoRenderEye();
        }
        private void PreRender()
        {
            OpenVR.Compositor.WaitGetPoses(_renderPoses, _updatePoses);

            //string m_strPoseClasses = "";
            for (uint nDevice = 0; nDevice < OpenVR.k_unMaxTrackedDeviceCount; ++nDevice)
            {
                if (!_renderPoses[nDevice].bPoseIsValid)
                    continue;
                
                _renderTransforms[nDevice] = _renderPoses[nDevice].mDeviceToAbsoluteTracking;
                //if (m_rDevClassChar[nDevice] == 0)
                //{
                //    m_rDevClassChar[nDevice] = (OpenVR.System.GetTrackedDeviceClass(nDevice)) switch
                //    {
                //        vr::TrackedDeviceClass_Controller => 'C',
                //        vr::TrackedDeviceClass_HMD => 'H',
                //        vr::TrackedDeviceClass_Invalid => 'I',
                //        vr::TrackedDeviceClass_GenericTracker => 'G',
                //        vr::TrackedDeviceClass_TrackingReference => 'T',
                //        _ => '?',
                //    };
                //}
                //m_strPoseClasses += m_rDevClassChar[nDevice];
            }

            if (_renderPoses[OpenVR.k_unTrackedDeviceIndex_Hmd].bPoseIsValid)
                HMDViewComponent.WorldMatrix = _renderTransforms[OpenVR.k_unTrackedDeviceIndex_Hmd];
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
