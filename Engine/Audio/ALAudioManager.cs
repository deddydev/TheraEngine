using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;

namespace TheraEngine.Audio
{
    public class ALAudioManager : AbstractAudioManager
    {
        private AudioContext _context;
        private EffectsExtension _efx;
        private const int MaxOpenALSources = 32;
        private int[] _sourceBuffer = new int[MaxOpenALSources];

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
        public override bool Pause(int soundId)
        {
            AL.SourcePause(soundId);
            return GetState(soundId) == AudioState.Paused;
        }

        private void ApplyParam(int soundId, UsableValue<bool> param, ALSourceb dest, bool initialPlay)
        {
            bool value = param.GetActualValue();
            AL.GetSource(soundId, dest, out bool currentValue);
            bool changed = value != currentValue;
            if (changed || initialPlay)
                AL.Source(soundId, dest, param.Value);
        }
        private void ApplyParam(int soundId, UsableValue<float> param, ALSourcef dest, bool initialPlay)
        {
            float value = param.GetActualValue();
            AL.GetSource(soundId, dest, out float currentValue);
            bool changed = value != currentValue;
            if (changed || initialPlay)
                AL.Source(soundId, dest, param.Value);
        }
        private void ApplyParam(int soundId, UsableValue<Vec3> param, ALSource3f dest, bool initialPlay)
        {
            Vec3 value = param.GetActualValue();
            AL.GetSource(soundId, dest, out Vector3 currentValue);
            bool changed = 
                value.X != currentValue.X || 
                value.Y != currentValue.Y || 
                value.Z != currentValue.Z;
            if (changed || initialPlay)
                AL.Source(soundId, dest, param.Value.X, param.Value.Y, param.Value.Z);
        }

        private void ApplyParameters(int soundId, AudioSourceParameters param, bool initialPlay)
        {
            ApplyParam(soundId, param.SourceRelative, ALSourceb.SourceRelative, initialPlay);
            ApplyParam(soundId, param.Loop, ALSourceb.Looping, initialPlay);
            ApplyParam(soundId, param.EfxDirectFilterGainHighFrequencyAuto, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, initialPlay);
            ApplyParam(soundId, param.EfxAuxiliarySendFilterGainAuto, ALSourceb.EfxAuxiliarySendFilterGainAuto, initialPlay);
            ApplyParam(soundId, param.EfxAuxiliarySendFilterGainHighFrequencyAuto, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, initialPlay);
            ApplyParam(soundId, param.ConeInnerAngle, ALSourcef.ConeInnerAngle, initialPlay);
            ApplyParam(soundId, param.ConeOuterAngle, ALSourcef.ConeOuterAngle, initialPlay);
            ApplyParam(soundId, param.Pitch, ALSourcef.Pitch, initialPlay);
            ApplyParam(soundId, param.Gain, ALSourcef.Gain, initialPlay);
            ApplyParam(soundId, param.MinGain, ALSourcef.MinGain, initialPlay);
            ApplyParam(soundId, param.MaxGain, ALSourcef.MaxGain, initialPlay);
            ApplyParam(soundId, param.ReferenceDistance, ALSourcef.ReferenceDistance, initialPlay);
            ApplyParam(soundId, param.RolloffFactor, ALSourcef.RolloffFactor, initialPlay);
            ApplyParam(soundId, param.ConeOuterGain, ALSourcef.ConeOuterGain, initialPlay);
            ApplyParam(soundId, param.MaxDistance, ALSourcef.MaxDistance, initialPlay);
            ApplyParam(soundId, param.SecOffset, ALSourcef.SecOffset, initialPlay);
            ApplyParam(soundId, param.EfxAirAbsorptionFactor, ALSourcef.EfxAirAbsorptionFactor, initialPlay);
            ApplyParam(soundId, param.EfxRoomRolloffFactor, ALSourcef.EfxRoomRolloffFactor, initialPlay);
            ApplyParam(soundId, param.EfxConeOuterGainHighFrequency, ALSourcef.EfxConeOuterGainHighFrequency, initialPlay);
            ApplyParam(soundId, param.Position, ALSource3f.Position, initialPlay);
            ApplyParam(soundId, param.Direction, ALSource3f.Direction, initialPlay);
            ApplyParam(soundId, param.Velocity, ALSource3f.Velocity, initialPlay);
        }
        
        public override void Update(int soundId, AudioSourceParameters param)
            => ApplyParameters(soundId, param, false);
        public override int Play(SoundFile sound) => Play(sound, null);
        public override int Play(SoundFile sound, AudioSourceParameters param)
        {
            byte[] data = sound.WaveFile.SoundData;
            if (data == null)
                return -1;
            
            if (sound.PlayingCount == 0)
            {
                sound.BufferId = AL.GenBuffer();
                AL.BufferData(sound.BufferId,
                    GetSoundFormat(sound.WaveFile.Channels, sound.WaveFile.BitsPerSample),
                    data, data.Length, sound.WaveFile.SampleRate);
            }

            int soundId = AL.GenSource();
            AL.Source(soundId, ALSourcei.Buffer, sound.BufferId);

            if (param != null)
                ApplyParameters(soundId, param, true);

            AL.SourcePlay(soundId);
            return soundId;

            //do
            //{
            //    Thread.Sleep(250);
            //    AL.GetSource(sound.SourceId, ALGetSourcei.SourceState, out state);
            //}
            //while ((ALSourceState)state == ALSourceState.Playing);
        }
        public override bool Stop(int soundId)
        {
            AL.SourceStop(soundId);
            AL.DeleteSource(soundId);
            return GetState(soundId) == AudioState.Stopped;
        }
        public override AudioState GetState(int soundId)
        {
            return AudioState.Initial + ((int)AL.GetSourceState(soundId) - (int)ALSourceState.Initial);
        }
        public override void UpdateListener(Vec3 position, Vec3 forward, Vec3 up, Vec3 velocity, float gain, float efxMetersPerUnit = 1.0f)
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

        public override bool Play(int soundId)
        {
            AL.SourcePlay(soundId);
            return GetState(soundId) == AudioState.Playing;
        }
    }
}
