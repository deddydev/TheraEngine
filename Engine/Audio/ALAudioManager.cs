using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Threading;
using CustomEngine.Worlds.Actors;

namespace CustomEngine.Audio
{
    public class ALAudioManager : AbstractAudioManager
    {
        private AudioContext _context;
        private EffectsExtension _efx;
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
        public override void Pause(SoundFile sound)
        {
            AL.SourcePause(sound.SourceId);
        }

        private void ApplyParam(int source, UsableValue<bool> param, ALSourceb dest, bool initialPlay)
        {
            bool value = param.GetActualValue();
            AL.GetSource(source, dest, out bool currentValue);
            bool changed = value != currentValue;
            if (changed || initialPlay)
                AL.Source(source, dest, param.Value);
        }
        private void ApplyParam(int source, UsableValue<float> param, ALSourcef dest, bool initialPlay)
        {
            float value = param.GetActualValue();
            AL.GetSource(source, dest, out float currentValue);
            bool changed = value != currentValue;
            if (changed || initialPlay)
                AL.Source(source, dest, param.Value);
        }
        private void ApplyParam(int source, UsableValue<Vec3> param, ALSource3f dest, bool initialPlay)
        {
            Vec3 value = param.GetActualValue();
            AL.GetSource(source, dest, out Vector3 currentValue);
            bool changed = 
                value.X != currentValue.X || 
                value.Y != currentValue.Y || 
                value.Z != currentValue.Z;
            if (changed || initialPlay)
                AL.Source(source, dest, param.Value.X, param.Value.Y, param.Value.Z);
        }

        private void ApplyParameters(int source, AudioSourceParameters param, bool initialPlay)
        {
            ApplyParam(source, param.SourceRelative, ALSourceb.SourceRelative, initialPlay);
            ApplyParam(source, param.Loop, ALSourceb.Looping, initialPlay);
            ApplyParam(source, param.EfxDirectFilterGainHighFrequencyAuto, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, initialPlay);
            ApplyParam(source, param.EfxAuxiliarySendFilterGainAuto, ALSourceb.EfxAuxiliarySendFilterGainAuto, initialPlay);
            ApplyParam(source, param.EfxAuxiliarySendFilterGainHighFrequencyAuto, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, initialPlay);
            ApplyParam(source, param.ConeInnerAngle, ALSourcef.ConeInnerAngle, initialPlay);
            ApplyParam(source, param.ConeOuterAngle, ALSourcef.ConeOuterAngle, initialPlay);
            ApplyParam(source, param.Pitch, ALSourcef.Pitch, initialPlay);
            ApplyParam(source, param.Gain, ALSourcef.Gain, initialPlay);
            ApplyParam(source, param.MinGain, ALSourcef.MinGain, initialPlay);
            ApplyParam(source, param.MaxGain, ALSourcef.MaxGain, initialPlay);
            ApplyParam(source, param.ReferenceDistance, ALSourcef.ReferenceDistance, initialPlay);
            ApplyParam(source, param.RolloffFactor, ALSourcef.RolloffFactor, initialPlay);
            ApplyParam(source, param.ConeOuterGain, ALSourcef.ConeOuterGain, initialPlay);
            ApplyParam(source, param.MaxDistance, ALSourcef.MaxDistance, initialPlay);
            ApplyParam(source, param.SecOffset, ALSourcef.SecOffset, initialPlay);
            ApplyParam(source, param.EfxAirAbsorptionFactor, ALSourcef.EfxAirAbsorptionFactor, initialPlay);
            ApplyParam(source, param.EfxRoomRolloffFactor, ALSourcef.EfxRoomRolloffFactor, initialPlay);
            ApplyParam(source, param.EfxConeOuterGainHighFrequency, ALSourcef.EfxConeOuterGainHighFrequency, initialPlay);
            ApplyParam(source, param.Position, ALSource3f.Position, initialPlay);
            ApplyParam(source, param.Direction, ALSource3f.Direction, initialPlay);
            ApplyParam(source, param.Velocity, ALSource3f.Velocity, initialPlay);
        }

        public override void Update(SoundFile sound, AudioSourceParameters param)
            => ApplyParameters(sound.SourceId, param, false);
        public override void Play(SoundFile sound) => Play(sound, null);
        public override void Play(SoundFile sound, AudioSourceParameters param)
        {
            sound.BufferId = AL.GenBuffer();
            sound.SourceId = AL.GenSource();
            //int state;
            
            byte[] data = sound.WaveFile.SoundData;
            AL.BufferData(sound.BufferId, 
                GetSoundFormat(sound.WaveFile.Channels, sound.WaveFile.BitsPerSample),
                data, data.Length, sound.WaveFile.SampleRate);

            AL.Source(sound.SourceId, ALSourcei.Buffer, sound.BufferId);

            if (param != null)
                ApplyParameters(sound.SourceId, param, true);

            AL.SourcePlay(sound.SourceId);
            
            //do
            //{
            //    Thread.Sleep(250);
            //    AL.GetSource(sound.SourceId, ALGetSourcei.SourceState, out state);
            //}
            //while ((ALSourceState)state == ALSourceState.Playing);
        }
        public override void Stop(SoundFile sound)
        {
            AL.SourceStop(sound.SourceId);
            AL.DeleteSource(sound.SourceId);
            AL.DeleteBuffer(sound.BufferId);
        }
        public override AudioState GetState(SoundFile sound)
        {
            return AudioState.Initial + ((int)AL.GetSourceState(sound.SourceId) - (int)ALSourceState.Initial);
        }
        public override void UpdateListener(PlayerIndex player, Vec3 position, Vec3 forward, Vec3 up, Vec3 velocity, float gain, float efxMetersPerUnit = 1.0f)
        {
            float f;
            Vector3 v;

            AL.GetListener(ALListenerf.EfxMetersPerUnit, out f);
            if (f != efxMetersPerUnit)
                AL.Listener(ALListenerf.EfxMetersPerUnit, efxMetersPerUnit);

            AL.GetListener(ALListenerf.Gain, out f);
            if (f != gain)
                AL.Listener(ALListenerf.Gain, gain);

            AL.GetListener(ALListener3f.Position, out v);
            if (v.X != position.X || v.Y != position.Y || v.Z != position.Z)
                AL.Listener(ALListener3f.Position, position.X, position.Y, position.Z);

            AL.GetListener(ALListener3f.Position, out v);
            if (v.X != velocity.X || v.Y != velocity.Y || v.Z != velocity.Z)
                AL.Listener(ALListener3f.Velocity, velocity.X, velocity.Y, velocity.Z);
            
            AL.GetListener(ALListenerfv.Orientation, out Vector3 fv, out Vector3 uv);
            if (fv.X != forward.X || fv.Y != forward.Y || fv.Z != forward.Z ||
                uv.X != up.X || uv.Y != up.Y || uv.Z != up.Z)
            {
                float[] o = new float[]
                {
                    forward.X, forward.Y, forward.Z,
                    up.X, up.Y, up.Z
                };
                AL.Listener(ALListenerfv.Orientation, ref o);
            }
        }
    }
}
