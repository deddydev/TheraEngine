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
        public override bool Pause(AudioInstance instance)
        {
            AL.SourcePause(instance.ID);
            return GetState(instance) == EAudioState.Paused;
        }

        private void ApplyParam(int instanceID, bool param, ALSourceb dest, bool force = false)
        {
            if (!force)
            {
                AL.GetSource(instanceID, dest, out bool currentValue);
                force = param != currentValue;
            }

            if (force)
                AL.Source(instanceID, dest, param);
        }
        private void ApplyParam(int instanceID, float param, ALSourcef dest, bool force = false)
        {
            if (!force)
            {
                AL.GetSource(instanceID, dest, out float currentValue);
                force = !param.EqualTo(currentValue);
            }

            if (force)
                AL.Source(instanceID, dest, param);
        }
        private void ApplyParam(int instanceID, Vec3 param, ALSource3f dest, bool force = false)
        {
            if (!force)
            {
                AL.GetSource(instanceID, dest, out Vector3 currentValue);
                force = (param | currentValue) < 1.0f;
            }

            if (force)
                AL.Source(instanceID, dest, param.X, param.Y, param.Z);
        }
        
        public override void UpdateSourceListenerRelative(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.ListenerRelative, ALSourceb.SourceRelative, force);
        public override void UpdateSourceLoop(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.Loop, ALSourceb.Looping, force);
        
        public override void UpdateSourceEfxDirectFilterGainHighFrequencyAuto(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.EfxDirectFilterGainHighFrequencyAuto, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, force);
        public override void UpdateSourceEfxAuxiliarySendFilterGainAuto(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.EfxAuxiliarySendFilterGainAuto, ALSourceb.EfxAuxiliarySendFilterGainAuto, force);
        public override void UpdateSourceEfxAuxiliarySendFilterGainHighFrequencyAuto(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.EfxAuxiliarySendFilterGainHighFrequencyAuto, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, force);

        public override void UpdateSourceEfxAirAbsorptionFactor(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.EfxAirAbsorptionFactor, ALSourcef.EfxAirAbsorptionFactor, force);
        public override void UpdateSourceEfxRoomRolloffFactor(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.EfxRoomRolloffFactor, ALSourcef.EfxRoomRolloffFactor, force);
        public override void UpdateSourceEfxConeOuterGainHighFrequency(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.EfxConeOuterGainHighFrequency, ALSourcef.EfxConeOuterGainHighFrequency, force);

        public override void UpdateSourceConeInnerAngle(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.ConeInnerAngle, ALSourcef.ConeInnerAngle, force);
        public override void UpdateSourceConeOuterAngle(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.ConeOuterAngle, ALSourcef.ConeOuterAngle, force);
        public override void UpdateSourceConeOuterGain(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.ConeOuterGain, ALSourcef.ConeOuterGain, force);

        public override void UpdateSourcePitch(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.Pitch, ALSourcef.Pitch, force);
        public override void UpdateSourceGain(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.Gain, ALSourcef.Gain, force);
        public override void UpdateSourceMinGain(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.MinGain, ALSourcef.MinGain, force);
        public override void UpdateSourceMaxGain(AudioInstance instance, bool force = false)
            => ApplyParam(instance.ID, instance.MaxGain, ALSourcef.MaxGain, force);
        
        public override void ApplyParameters(AudioInstance instance, AudioParameters param, bool force = false)
        {
            ApplyParam(instance, param.ListenerRelative, 
                ALSourceb.SourceRelative, force);
            ApplyParam(instance, param.Loop,
                ALSourceb.Looping, force);

            ApplyParam(instance, param.EfxDirectFilterGainHighFrequencyAuto,
                ALSourceb.EfxDirectFilterGainHighFrequencyAuto, force);
            ApplyParam(instance, param.EfxAuxiliarySendFilterGainAuto, 
                ALSourceb.EfxAuxiliarySendFilterGainAuto, force);
            ApplyParam(instance, param.EfxAuxiliarySendFilterGainHighFrequencyAuto,
                ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, force);

            ApplyParam(instance, param.EfxAirAbsorptionFactor,
                ALSourcef.EfxAirAbsorptionFactor, force);
            ApplyParam(instance, param.EfxRoomRolloffFactor,
                ALSourcef.EfxRoomRolloffFactor, force);
            ApplyParam(instance, param.EfxConeOuterGainHighFrequency,
                ALSourcef.EfxConeOuterGainHighFrequency, force);

            ApplyParam(instance, param.ConeInnerAngle,
                ALSourcef.ConeInnerAngle, force);
            ApplyParam(instance, param.ConeOuterAngle, 
                ALSourcef.ConeOuterAngle, force);
            ApplyParam(instance, param.ConeOuterGain,
                ALSourcef.ConeOuterGain, force);

            ApplyParam(instance, param.Pitch,
                ALSourcef.Pitch, force);
            ApplyParam(instance, param.Gain,
                ALSourcef.Gain, force);
            ApplyParam(instance, param.MinGain,
                ALSourcef.MinGain, force);
            ApplyParam(instance, param.MaxGain,
                ALSourcef.MaxGain, force);

            ApplyParam(instance, param.ReferenceDistance, 
                ALSourcef.ReferenceDistance, force);
            ApplyParam(instance, param.RolloffFactor,
                ALSourcef.RolloffFactor, force);

            ApplyParam(instance, param.MaxDistance,
                ALSourcef.MaxDistance, force);
            ApplyParam(instance, param.PlaybackOffsetSeconds, 
                ALSourcef.SecOffset, force);

            ApplyParam(instance, param.Position, 
                ALSource3f.Position, force);
            ApplyParam(instance, param.Direction,
                ALSource3f.Direction, force);
            ApplyParam(instance, param.Velocity, 
                ALSource3f.Velocity, force);
        }
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

            AL.Source(instanceID, ALSourcei.Buffer, audio.BufferId);

            AudioInstance instance = new AudioInstance(instanceID, param);
            if (param != null)
                ApplyParameters(instance, param, true);

            AL.SourcePlay(instanceID);

            audio.Instances.Add(instance);

            return instance;

            //do
            //{
            //    Thread.Sleep(250);
            //    AL.GetSource(sound.SourceId, ALGetSourcei.SourceState, out state);
            //}
            //while ((ALSourceState)state == ALSourceState.Playing);
        }

        public override bool Stop(AudioInstance instance)
        {
            AL.SourceStop(instance.ID);
            AL.DeleteSource(instance.ID);
            return GetState(instance) == EAudioState.Stopped;
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
