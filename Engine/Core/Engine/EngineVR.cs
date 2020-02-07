using System;
using Valve.VR;

namespace TheraEngine.Core
{
    public class EngineVR
    {
        public ETrackingUniverseOrigin TrackingOrigin { get; set; } = ETrackingUniverseOrigin.TrackingUniverseSeated;

        public bool IsActive { get; private set; }

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
                    Engine.Out("VR system initialized successfully.");
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
            OpenVR.Compositor.SetTrackingSpace(ETrackingUniverseOrigin.TrackingUniverseSeated);
        }

        public void Shutdown()
        {
            if (!IsActive)
                return;

            OpenVR.Shutdown();
            IsActive = false;
        }

        public Matrix4 GetEyeToHeadTransform(bool leftEye) 
            => OpenVR.System.GetEyeToHeadTransform(leftEye ? EVREye.Eye_Left : EVREye.Eye_Right);

        public void Render()
        {
            //TrackedDevicePose_t[] render = new TrackedDevicePose_t[];
            //TrackedDevicePose_t[] game = new TrackedDevicePose_t[];
            //OpenVR.Compositor.WaitGetPoses(render, game);

            //Texture_t tex = new Texture_t
            //{
            //    eColorSpace = EColorSpace.Gamma,
            //    eType = ETextureType.OpenGL
            //};
            //VRTextureBounds_t bounds = new VRTextureBounds_t();
            //OpenVR.Compositor.Submit(EVREye.Eye_Left, ref tex, ref bounds, EVRSubmitFlags.Submit_Default);
            //OpenVR.System.GetDeviceToAbsoluteTrackingPose(TrackingOrigin, 0.0f, game);
        }
    }
}
