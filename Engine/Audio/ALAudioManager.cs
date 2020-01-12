using Extensions;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Audio
{
    public class ALAudioManager : AbstractAudioManager, IDisposable
    {
        private const int MaxOpenALSources = 32;

        public ALAudioManager() : base()
        {
            LazyContext = new Lazy<AudioContext>(GenerateContext, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private AudioContext GenerateContext()
        {
            IList<string> devices = AudioContext.AvailableDevices;
            Engine.Out("Available audio devices: " + string.Join(", ", devices));
            Engine.Out("Default audio device: " + AudioContext.DefaultDevice);

            AudioContext ctx = new AudioContext(AudioContext.DefaultDevice);
            ctx.MakeCurrent();

            string version = AL.Get(ALGetString.Version);
            string vendor = AL.Get(ALGetString.Vendor);
            string renderer = AL.Get(ALGetString.Renderer);
            string extensions = AL.Get(ALGetString.Extensions);
            Engine.Out($"OpenAL {version}, {vendor} [{renderer}]");
            Engine.Out($"OpenAL Extensions: {extensions}");

            _efx = new EffectsExtension();

            AL.DistanceModel(ALDistanceModel.LinearDistanceClamped);

            return ctx;
        }

        private EffectsExtension _efx;
        private readonly int[] _sourceBuffers = new int[MaxOpenALSources];
        
        public enum EAudioPriorityType
        {
            /// <summary>
            /// Lowest priority, played and stopped according to demand.
            /// </summary>
            SFX,
            /// <summary>
            /// Medium priority, unloaded only for incoming music but not for incoming sfx.
            /// </summary>
            Ambient,
            /// <summary>
            /// Highest priority, never unloaded for other types.
            /// </summary>
            Music,
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
        public override AudioInstance Play(IAudioSource source)
        {
            var audio = source?.Audio;

            var param = source.Parameters;

            Engine.Out($"Playing audio {audio?.Name}...");

            int sourceID = AllocateNewSource();
            int bufferID = AL.GenBuffer();
            CheckError();

            if (audio.BufferId > 0)
            {
                AL.DeleteBuffer(audio.BufferId);
                CheckError();
            }

            audio.BufferId = bufferID;

            AudioInstance instance = new AudioInstance(sourceID, param);

            ALFormat format = GetSoundFormat(audio.Channels, audio.BitsPerSample);

            //if (audio.UseStreaming)
            //{
            //    while (audio.GetNextStreamChunk(out byte[] buffer))
            //    {
            //        AL.BufferData(audio.BufferId, format, buffer, buffer.Length, audio.SampleRate);
            //        CheckError();

            //        AL.SourceQueueBuffer(sourceID, bufferID);

            //        AL.GetSource(instance.ID)
            //        AL.SourceUnqueueBuffers();
            //    }
            //}
            //else
            {
                byte[] data = audio?.Samples;
                if (data is null || data.Length == 0)
                    return null;

                AL.BufferData(audio.BufferId, format, data, data.Length, audio.SampleRate);
                CheckError();
                
                AL.BindBufferToSource(sourceID, audio.BufferId);
            }

            CheckError();

            UpdateSource(instance, false);
            Play(instance);

            source.Instance = instance;

            return instance;
        }

        private int AllocateNewSource()
        {
            int id = AL.GenSource();
            CheckError();
            return id;
        }

        private void CheckError()
        {
            Context?.MakeCurrent();

            ALError error = AL.GetError();
            if (error != ALError.NoError)
                throw new InvalidOperationException("OpenAL error: " + error.ToString());
        }

        public override IList<string> SoundDevices => AudioContext.AvailableDevices;

        public AudioContext Context => LazyContext?.Value;
        private Lazy<AudioContext> LazyContext { get; set; }

        public override bool Play(AudioInstance instance)
        {
            CheckError();
            EAudioState state = GetState(instance);
            if (state == EAudioState.Playing)
                return true;

            AL.SourcePlay(instance.ID);
            CheckError();
            
            return GetState(instance) == EAudioState.Playing;
        }
        public override bool Pause(AudioInstance instance)
        {
            CheckError();
            EAudioState state = GetState(instance);
            if (state == EAudioState.Paused)
                return true;

            AL.SourcePause(instance.ID);
            CheckError();

            return GetState(instance) == EAudioState.Paused;
        }
        public override bool Stop(AudioInstance instance)
        {
            CheckError();
            EAudioState state = GetState(instance);
            if (state == EAudioState.Stopped)
                return true;

            AL.SourceStop(instance.ID);
            CheckError();

            return GetState(instance) == EAudioState.Stopped;
        }
        public override void Destroy(AudioInstance instance)
        {
            CheckError();
            AL.DeleteSource(instance.ID);
            CheckError();
            instance.Valid = false;
        }

        public override EAudioState GetState(AudioInstance instance)
            => EAudioState.Initial + ((int)AL.GetSourceState(instance.ID) - (int)ALSourceState.Initial);
        
        public override void UpdateListenerPosition(Vec3 position, bool force = false)
        {
            CheckError();

            AL.GetListener(ALListener3f.Position, out Vector3 currentPosition);
            CheckError();

            if (force || (currentPosition | position) < 1.0f)
            {
                AL.Listener(ALListener3f.Position, position.X, position.Y, position.Z);
                CheckError();
            }
        }
        public override void UpdateListenerOrientation(Vec3 forward, Vec3 up, bool force = false)
        {
            CheckError();

            AL.GetListener(ALListenerfv.Orientation, out Vector3 currentForward, out Vector3 currentUp);
            CheckError();

            if (force || (forward | currentForward) < 1.0f || (up | currentUp) < 1.0f)
            {
                float[] values = new float[]
                {
                    forward.X, forward.Y, forward.Z,
                    up.X, up.Y, up.Z
                };
                AL.Listener(ALListenerfv.Orientation, ref values);
                CheckError();
            }
        }
        public override void UpdateListenerVelocity(Vec3 velocity, bool force = false)
        {
            CheckError();

            AL.GetListener(ALListener3f.Velocity, out Vector3 currentVelocity);
            CheckError();

            if (force || (currentVelocity | velocity) < 1.0f)
            {
                AL.Listener(ALListener3f.Velocity, velocity.X, velocity.Y, velocity.Z);
                CheckError();
            }
        }
        public override void UpdateListenerGain(float gain, bool force = false)
        {
            CheckError();

            AL.GetListener(ALListenerf.Gain, out float currentGain);
            CheckError();

            if (force || !currentGain.EqualTo(gain))
            {
                AL.Listener(ALListenerf.Gain, gain);
                CheckError();
            }
        }
        public override void UpdateListenerEfxMetersPerUnit(float metersPerUnit, bool force = false)
        {
            CheckError();

            AL.GetListener(ALListenerf.EfxMetersPerUnit, out float currentMetersPerUnit);
            CheckError();

            if (force || !currentMetersPerUnit.EqualTo(metersPerUnit))
            {
                AL.Listener(ALListenerf.EfxMetersPerUnit, metersPerUnit);
                CheckError();
            }
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
            //Engine.PrintLine("Updating audio listener.");
            UpdateListenerPosition(position, force);
            UpdateListenerOrientation(forward, up, force);
            UpdateListenerVelocity(velocity, force);
            UpdateListenerGain(gain, force);
            UpdateListenerEfxMetersPerUnit(efxMetersPerUnit, force);
        }
        private void UpdateAudioParam(int instanceID, bool param, ALSourceb dest, bool force = false)
        {
            CheckError();

            if (!force)
            {
                AL.GetSource(instanceID, dest, out bool currentValue);
                CheckError();
                force = param != currentValue;
                if (force)
                    Engine.Out($"Audio {instanceID}: {dest} {currentValue} -> {param}");
            }
            else
                Engine.Out($"Audio {instanceID}: {dest} = {param}");

            if (force)
            {
                AL.Source(instanceID, dest, param);
                CheckError();
            }
        }
        private void UpdateAudioParam(int instanceID, float param, ALSourcef dest, bool force = false)
        {
            CheckError();

            if (!force)
            {
                AL.GetSource(instanceID, dest, out float currentValue);
                CheckError();
                force = !param.EqualTo(currentValue);
                if (force)
                    Engine.Out($"Audio {instanceID}: {dest} {currentValue} -> {param}");
            }
            else
                Engine.Out($"Audio {instanceID}: {dest} = {param}");

            if (force)
            {
                AL.Source(instanceID, dest, param);
                CheckError();
            }
        }
        private void UpdateAudioParam(int instanceID, Vec3 param, ALSource3f dest, bool force = false)
        {
            CheckError();

            if (!force)
            {
                AL.GetSource(instanceID, dest, out Vector3 currentValue);
                CheckError();
                Vec3 diff = param - (Vec3)currentValue;
                force = diff.Length > float.Epsilon;
                if (force)
                    Engine.Out($"Audio {instanceID}: {dest} {currentValue} -> {param}");
            }
            else
                Engine.Out($"Audio {instanceID}: {dest} = {param}");

            if (force)
            {
                AL.Source(instanceID, dest, param.X, param.Y, param.Z);
                CheckError();
            }
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

        public void Dispose()
        {
            if (LazyContext.IsValueCreated)
            {
                Context?.Dispose();
                LazyContext = null;
            }
        }
    }
}
