using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Audio
{
    public class ALAudioManager : AbstractAudioManager
    {
        private const int MaxOpenALSources = 32;

        private AudioContext _context;
        private readonly EffectsExtension _efx;
        private readonly int[] _sourceBuffer = new int[MaxOpenALSources];

        public ALAudioManager()
        {
            //IList<string> devices = AudioContext.AvailableDevices;
            _context = new AudioContext(AudioContext.DefaultDevice, 0, 0, true, true, AudioContext.MaxAuxiliarySends.UseDriverDefault);
            _context.MakeCurrent();
            _efx = new EffectsExtension();
            AL.DistanceModel(ALDistanceModel.LinearDistanceClamped);
        }
        ~ALAudioManager()
        {
            _context.Dispose();
        }
        private static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }
        private void UpdateAudioParam(int instanceID, bool param, ALSourceb dest, bool force = false)
        {
            if (!force)
            {
                AL.GetSource(instanceID, dest, out bool currentValue);
                force = param != currentValue;
            }

            if (force)
                AL.Source(instanceID, dest, param);
        }
        private void UpdateAudioParam(int instanceID, float param, ALSourcef dest, bool force = false)
        {
            if (!force)
            {
                AL.GetSource(instanceID, dest, out float currentValue);
                force = !param.EqualTo(currentValue);
            }

            if (force)
                AL.Source(instanceID, dest, param);
        }
        private void UpdateAudioParam(int instanceID, Vec3 param, ALSource3f dest, bool force = false)
        {
            if (!force)
            {
                AL.GetSource(instanceID, dest, out Vector3 currentValue);
                force = (param | currentValue) < 1.0f;
            }

            if (force)
                AL.Source(instanceID, dest, param.X, param.Y, param.Z);
        }

        public override void UpdateSource(AudioInstance instance, bool force = false)
        {
            UpdateSourceListenerRelative(instance, force);
            UpdateSourceLoop(instance, force);

            UpdateSourceEfxDirectFilterGainHighFrequencyAuto(instance, force);
            UpdateSourceEfxAuxiliarySendFilterGainAuto(instance, force);
            UpdateSourceEfxAuxiliarySendFilterGainHighFrequencyAuto(instance, force);

            UpdateSourceEfxAirAbsorptionFactor(instance, force);
            UpdateSourceEfxRoomRolloffFactor(instance, force);
            UpdateSourceEfxConeOuterGainHighFrequency(instance, force);

            UpdateSourceConeInnerAngle(instance, force);
            UpdateSourceConeOuterAngle(instance, force);
            UpdateSourceConeOuterGain(instance, force);

            UpdateSourcePitch(instance, force);
            UpdateSourceGain(instance, force);
            UpdateSourceMinGain(instance, force);
            UpdateSourceMaxGain(instance, force);

            UpdateSourceReferenceDistance(instance, force);
            UpdateSourceRolloffFactor(instance, force);
            UpdateSourceMaxDistance(instance, force);
            UpdateSourcePlaybackOffsetSeconds(instance, force);

            UpdateSourcePosition(instance, force);
            UpdateSourceDirection(instance, force);
            UpdateSourceVelocity(instance, force);
        }
        
        public override void UpdateSourceListenerRelative(int instanceID, bool value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourceb.SourceRelative, force);
        public override void UpdateSourceLoop(int instanceID, bool value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourceb.Looping, force);
        
        public override void UpdateSourceEfxDirectFilterGainHighFrequencyAuto(int instanceID, bool value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, force);
        public override void UpdateSourceEfxAuxiliarySendFilterGainAuto(int instanceID, bool value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourceb.EfxAuxiliarySendFilterGainAuto, force);
        public override void UpdateSourceEfxAuxiliarySendFilterGainHighFrequencyAuto(int instanceID, bool value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, force);

        public override void UpdateSourceEfxAirAbsorptionFactor(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.EfxAirAbsorptionFactor, force);
        public override void UpdateSourceEfxRoomRolloffFactor(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.EfxRoomRolloffFactor, force);
        public override void UpdateSourceEfxConeOuterGainHighFrequency(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.EfxConeOuterGainHighFrequency, force);

        public override void UpdateSourceConeInnerAngle(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.ConeInnerAngle, force);
        public override void UpdateSourceConeOuterAngle(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.ConeOuterAngle, force);
        public override void UpdateSourceConeOuterGain(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.ConeOuterGain, force);

        public override void UpdateSourcePitch(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.Pitch, force);
        public override void UpdateSourceGain(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.Gain, force);
        public override void UpdateSourceMinGain(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.MinGain, force);
        public override void UpdateSourceMaxGain(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.MaxGain, force);
        
        public override void UpdateSourceReferenceDistance(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.ReferenceDistance, force);
        public override void UpdateSourceRolloffFactor(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.RolloffFactor, force);
        public override void UpdateSourceMaxDistance(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.MaxDistance, force);
        public override void UpdateSourcePlaybackOffsetSeconds(int instanceID, float value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSourcef.SecOffset, force);

        public override void UpdateSourcePosition(int instanceID, Vec3 value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSource3f.Position, force);
        public override void UpdateSourceDirection(int instanceID, Vec3 value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSource3f.Direction, force);
        public override void UpdateSourceVelocity(int instanceID, Vec3 value, bool force = false)
            => UpdateAudioParam(instanceID, value, ALSource3f.Velocity, force);
        
        public override AudioInstance Play(IAudioSource source)
        {
            var audio = source.Audio;
            var param = source.Parameters;
            
            byte[] data = audio?.Samples;
            
            if (audio.Instances.Count == 0)
            {
                audio.BufferId = AL.GenBuffer();
                AL.BufferData(audio.BufferId,
                    GetSoundFormat(audio.Channels, audio.BitsPerSample),
                    data, data.Length, audio.SampleRate);
            }

            int instanceID = AL.GenSource();

            AudioInstance instance = new AudioInstance(instanceID, param);
            AL.Source(instance.ID, ALSourcei.Buffer, audio.BufferId);

            instance.UpdateAllParameters(true);
            instance.Play();

            audio.Instances.Add(instance);

            return instance;

            //do
            //{
            //    Thread.Sleep(250);
            //    AL.GetSource(sound.SourceId, ALGetSourcei.SourceState, out state);
            //}
            //while ((ALSourceState)state == ALSourceState.Playing);
        }
        public override bool Play(AudioInstance instance)
        {
            AL.SourcePlay(instance.ID);
            return GetState(instance) == EAudioState.Playing;
        }
        public override bool Pause(AudioInstance instance)
        {
            AL.SourcePause(instance.ID);
            return GetState(instance) == EAudioState.Paused;
        }
        public override bool Stop(AudioInstance instance)
        {
            AL.SourceStop(instance.ID);
            return GetState(instance) == EAudioState.Stopped;
        }
        public override void Destroy(AudioInstance instance)
        {
            AL.DeleteSource(instance.ID);
            instance.Valid = false;
        }

        public override EAudioState GetState(AudioInstance instance)
            => EAudioState.Initial + ((int)AL.GetSourceState(instance.ID) - (int)ALSourceState.Initial);
        
        public override void UpdateListenerPosition(Vec3 position, bool force = false)
        {
            AL.GetListener(ALListener3f.Position, out Vector3 currentPosition);
            if (force || (currentPosition | position) < 1.0f)
                AL.Listener(ALListener3f.Position, position.X, position.Y, position.Z);
        }
        public override void UpdateListenerOrientation(Vec3 forward, Vec3 up, bool force = false)
        {
            AL.GetListener(ALListenerfv.Orientation, out Vector3 currentForward, out Vector3 currentUp);
            if (force || (forward | currentForward) < 1.0f || (up | currentUp) < 1.0f)
            {
                float[] values = new float[]
                {
                    forward.X, forward.Y, forward.Z,
                    up.X, up.Y, up.Z
                };
                AL.Listener(ALListenerfv.Orientation, ref values);
            }
        }
        public override void UpdateListenerVelocity(Vec3 velocity, bool force = false)
        {
            AL.GetListener(ALListener3f.Velocity, out Vector3 currentVelocity);
            if (force || (currentVelocity | velocity) < 1.0f)
                AL.Listener(ALListener3f.Velocity, velocity.X, velocity.Y, velocity.Z);
        }
        public override void UpdateListenerGain(float gain, bool force = false)
        {
            AL.GetListener(ALListenerf.Gain, out float currentGain);
            if (force || !currentGain.EqualTo(gain))
                AL.Listener(ALListenerf.Gain, gain);
        }
        public override void UpdateListenerEfxMetersPerUnit(float metersPerUnit, bool force = false)
        {
            AL.GetListener(ALListenerf.EfxMetersPerUnit, out float currentMetersPerUnit);
            if (force || !currentMetersPerUnit.EqualTo(metersPerUnit))
                AL.Listener(ALListenerf.EfxMetersPerUnit, metersPerUnit);
        }
        public override void UpdateListener(
            Vec3 position,
            Vec3 forward,
            Vec3 up,
            Vec3 velocity,
            float gain, 
            float efxMetersPerUnit = 1.0f,
            bool force = false)
        {
            UpdateListenerPosition(position, force);
            UpdateListenerOrientation(forward, up, force);
            UpdateListenerVelocity(velocity, force);
            UpdateListenerGain(gain, force);
            UpdateListenerEfxMetersPerUnit(efxMetersPerUnit, force);
        }
    }
}
