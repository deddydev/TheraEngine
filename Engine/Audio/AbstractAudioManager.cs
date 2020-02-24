using System.Collections.Generic;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Audio
{
    public abstract class AbstractAudioManager : TObjectSlim
    {
        public abstract AudioInstance CreateNewInstance(IAudioSource source);
        public abstract bool Play(AudioInstance instance);
        public abstract bool Pause(AudioInstance instance);
        public abstract bool Stop(AudioInstance instance);
        public abstract void Destroy(AudioInstance instance);
        public abstract EAudioState GetState(AudioInstance instance);

        public abstract IList<string> SoundDevices { get; }
        public bool CheckErrors { get; set; } = true;

        #region Source

        #region Audio Instance

        public abstract void UpdateSource(AudioInstance instance, bool force = false);

        public void UpdateSourceListenerRelative(AudioInstance instance, bool force = false) 
            => UpdateSourceListenerRelative(instance.ID, instance.ListenerRelative, force);
        public void UpdateSourceLoop(AudioInstance instance, bool force = false)
            => UpdateSourceLoop(instance.ID, instance.Loop, force);

        public void UpdateSourceEfxDirectFilterGainHighFrequencyAuto(AudioInstance instance, bool force = false)
            => UpdateSourceEfxDirectFilterGainHighFrequencyAuto(instance.ID, instance.EfxDirectFilterGainHighFrequencyAuto, force);
        public void UpdateSourceEfxAuxiliarySendFilterGainAuto(AudioInstance instance, bool force = false)
            => UpdateSourceEfxAuxiliarySendFilterGainAuto(instance.ID, instance.EfxAuxiliarySendFilterGainAuto, force);
        public void UpdateSourceEfxAuxiliarySendFilterGainHighFrequencyAuto(AudioInstance instance, bool force = false)
            => UpdateSourceEfxAuxiliarySendFilterGainHighFrequencyAuto(instance.ID, instance.EfxAuxiliarySendFilterGainHighFrequencyAuto, force);

        public void UpdateSourceEfxAirAbsorptionFactor(AudioInstance instance, bool force = false)
            => UpdateSourceEfxAirAbsorptionFactor(instance.ID, instance.EfxAirAbsorptionFactor, force);
        public void UpdateSourceEfxRoomRolloffFactor(AudioInstance instance, bool force = false)
            => UpdateSourceEfxRoomRolloffFactor(instance.ID, instance.EfxRoomRolloffFactor, force);
        public void UpdateSourceEfxConeOuterGainHighFrequency(AudioInstance instance, bool force = false)
            => UpdateSourceEfxConeOuterGainHighFrequency(instance.ID, instance.EfxRoomRolloffFactor, force);

        public void UpdateSourceConeInnerAngle(AudioInstance instance, bool force = false)
            => UpdateSourceConeInnerAngle(instance.ID, instance.ConeInnerAngle, force);
        public void UpdateSourceConeOuterAngle(AudioInstance instance, bool force = false)
            => UpdateSourceConeOuterAngle(instance.ID, instance.ConeOuterAngle, force);
        public void UpdateSourceConeOuterGain(AudioInstance instance, bool force = false)
            => UpdateSourceConeOuterGain(instance.ID, instance.ConeOuterGain, force);

        public void UpdateSourcePitch(AudioInstance instance, bool force = false)
            => UpdateSourcePitch(instance.ID, instance.Pitch, force);
        public void UpdateSourceGain(AudioInstance instance, bool force = false)
            => UpdateSourceGain(instance.ID, instance.Gain, force);
        public void UpdateSourceMinGain(AudioInstance instance, bool force = false)
            => UpdateSourceMinGain(instance.ID, instance.MinGain, force);
        public void UpdateSourceMaxGain(AudioInstance instance, bool force = false)
            => UpdateSourceMaxGain(instance.ID, instance.MaxGain, force);

        public void UpdateSourceReferenceDistance(AudioInstance instance, bool force = false)
            => UpdateSourceReferenceDistance(instance.ID, instance.ReferenceDistance, force);
        public void UpdateSourceRolloffFactor(AudioInstance instance, bool force = false)
            => UpdateSourceRolloffFactor(instance.ID, instance.RolloffFactor, force);
        public void UpdateSourceMaxDistance(AudioInstance instance, bool force = false)
            => UpdateSourceMaxDistance(instance.ID, instance.MaxDistance, force);
        public void UpdateSourcePlaybackOffsetSeconds(AudioInstance instance, bool force = false)
            => UpdateSourcePlaybackOffsetSeconds(instance.ID, instance.PlaybackOffsetSeconds, force);

        public void UpdateSourcePosition(AudioInstance instance, bool force = false)
            => UpdateSourcePosition(instance.ID, instance.Position, force);
        public void UpdateSourceDirection(AudioInstance instance, bool force = false)
            => UpdateSourceDirection(instance.ID, instance.Direction, force);
        public void UpdateSourceVelocity(AudioInstance instance, bool force = false)
            => UpdateSourceVelocity(instance.ID, instance.Velocity, force);

        #endregion

        public abstract void UpdateSourceListenerRelative(int instanceID, bool value, bool force = false);
        public abstract void UpdateSourceLoop(int instanceID, bool value, bool force = false);

        public abstract void UpdateSourceEfxDirectFilterGainHighFrequencyAuto(int instanceID, bool value, bool force = false);
        public abstract void UpdateSourceEfxAuxiliarySendFilterGainAuto(int instanceID, bool value, bool force = false);
        public abstract void UpdateSourceEfxAuxiliarySendFilterGainHighFrequencyAuto(int instanceID, bool value, bool force = false);

        public abstract void UpdateSourceEfxAirAbsorptionFactor(int instanceID, float value, bool force = false);
        public abstract void UpdateSourceEfxRoomRolloffFactor(int instanceID, float value, bool force = false);
        public abstract void UpdateSourceEfxConeOuterGainHighFrequency(int instanceID, float value, bool force = false);

        public abstract void UpdateSourceConeInnerAngle(int instanceID, float value, bool force = false);
        public abstract void UpdateSourceConeOuterAngle(int instanceID, float value, bool force = false);
        public abstract void UpdateSourceConeOuterGain(int instanceID, float value, bool force = false);

        public abstract void UpdateSourcePitch(int instanceID, float value, bool force = false);
        public abstract void UpdateSourceGain(int instanceID, float value, bool force = false);
        public abstract void UpdateSourceMinGain(int instanceID, float value, bool force = false);
        public abstract void UpdateSourceMaxGain(int instanceID, float value, bool force = false);

        public abstract void UpdateSourceReferenceDistance(int instanceID, float value, bool force = false);
        public abstract void UpdateSourceRolloffFactor(int instanceID, float value, bool force = false);
        public abstract void UpdateSourceMaxDistance(int instanceID, float value, bool force = false);
        public abstract void UpdateSourcePlaybackOffsetSeconds(int instanceID, float value, bool force = false);

        public abstract void UpdateSourcePosition(int instanceID, Vec3 value, bool force = false);
        public abstract void UpdateSourceDirection(int instanceID, Vec3 value, bool force = false);
        public abstract void UpdateSourceVelocity(int instanceID, Vec3 value, bool force = false);

        #endregion

        #region Listener

        public abstract void UpdateListener(Vec3 position, Vec3 forward, Vec3 up, Vec3 velocity, float gain, float efxMetersPerUnit = 1.0f, bool force = false);

        public abstract void UpdateListenerPosition(Vec3 position, bool force = false);
        public abstract void UpdateListenerOrientation(Vec3 forward, Vec3 up, bool force = false);
        public abstract void UpdateListenerVelocity(Vec3 velocity, bool force = false);

        public abstract void UpdateListenerGain(float gain, bool force = false);
        public abstract void UpdateListenerEfxMetersPerUnit(float metersPerUnit, bool force = false);

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
