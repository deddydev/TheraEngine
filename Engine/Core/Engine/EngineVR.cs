using System;
using Valve.VR;

namespace TheraEngine.Core
{
    public class EngineVR
    {
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

            return peError;
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
    }
}
