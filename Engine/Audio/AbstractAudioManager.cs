using TheraEngine.Components.Scene;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Audio
{
    public abstract class AbstractAudioManager
    {
        public abstract AudioInstance Play(IAudioSource source);
        public abstract bool Stop(AudioInstance instance);
        public abstract bool Pause(AudioInstance instance);
        public abstract EAudioState GetState(AudioInstance instance);
        
        public abstract void ApplyParameters(AudioInstance instance, AudioParameters param, bool initialPlay);

        #region Source
        public abstract void UpdateSourceListenerRelative(AudioInstance instance, bool force = false);
        public abstract void UpdateSourceLoop(AudioInstance instance, bool force = false);

        public abstract void UpdateSourceEfxDirectFilterGainHighFrequencyAuto(AudioInstance instance, bool force = false);
        public abstract void UpdateSourceEfxAuxiliarySendFilterGainAuto(AudioInstance instance, bool force = false);
        public abstract void UpdateSourceEfxAuxiliarySendFilterGainHighFrequencyAuto(AudioInstance instance, bool force = false);

        public abstract void UpdateSourceEfxAirAbsorptionFactor(AudioInstance instance, bool force = false);
        public abstract void UpdateSourceEfxRoomRolloffFactor(AudioInstance instance, bool force = false);
        public abstract void UpdateSourceEfxConeOuterGainHighFrequency(AudioInstance instance, bool force = false);
        #endregion

        #region Listener
        public abstract void UpdateListenerPosition(Vec3 position, bool force = false);
        public abstract void UpdateListenerOrientation(Vec3 forward, Vec3 up, bool force = false);
        public abstract void UpdateListenerVelocity(Vec3 velocity, bool force = false);
        public abstract void UpdateListenerGain(float gain, bool force = false);
        public abstract void UpdateListenerEfxMetersPerUnit(float metersPerUnit, bool force = false);
        public abstract void UpdateListener(Vec3 position, Vec3 forward, Vec3 up, Vec3 velocity, float gain, float efxMetersPerUnit = 1.0f, bool force = false);
        #endregion
    }
    public enum EAudioState
    {
        Initial,
        Playing,
        Paused,
        Stopped,
    }
}
