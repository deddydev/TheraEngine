using Extensions;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
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
            PlayingInstances = new EventList<AudioInstance>();
            PlayingInstances.PreAnythingAdded += PlayingInstances_PreAnythingAdded;
            PlayingInstances.PostAnythingRemoved += PlayingInstances_PostAnythingRemoved;
        }

        public EventList<AudioInstance> PlayingInstances { get; }

        private bool PlayingInstances_PreAnythingAdded(AudioInstance item)
        {
            if (PlayingInstances.Count == 0)
                Engine.RegisterTick(
                    ETickGroup.DuringPhysics,
                    ETickOrder.Timers,
                    UpdateAudioTick,
                    Input.Devices.EInputPauseType.TickAlways);

            return true;

        }

        private void PlayingInstances_PostAnythingRemoved(AudioInstance item)
        {
            if (PlayingInstances.Count == 0)
                Engine.UnregisterTick(
                    ETickGroup.DuringPhysics,
                    ETickOrder.Timers,
                    UpdateAudioTick,
                    Input.Devices.EInputPauseType.TickAlways);
        }

        private AudioContext GenerateContext()
        {
            try
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
            catch //(DllNotFoundException ex)
            {
                Engine.LogWarning("OpenAL is not installed.");
                return null;
            }
        }

        private EffectsExtension _efx;
        //private readonly int[] _sourceBuffers = new int[MaxOpenALSources];
        
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
            => channels switch
            {
                1 => bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16,
                2 => bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16,
                _ => throw new NotSupportedException("The specified sound format is not supported."),
            };

        public override AudioInstance CreateNewInstance(IAudioSource source)
        {
            if (source is null)
                return null;

            var audio = source.Audio;
            if (audio is null)
                return null;

            int sourceID = AL.GenSource();
            CheckError();

            AudioInstance instance = new AudioInstance(audio, sourceID, source.Parameters);

            if (audio.UseStreaming)
                InitializeStreaming(audio, instance);
            else
                FillSource(audio, sourceID);
            
            UpdateSource(instance, false);
            instance.Valid = true;

            return instance;
        }

        private void InitializeStreaming(AudioFile audio, AudioInstance instance)
        {
            instance.StreamingSource = audio;

            //TODO: dynamic buffered chunk count
            int bufferWindowSize = audio.StreamingMaxBufferedChunks;
            instance.BufferIDs = AL.GenBuffers(bufferWindowSize);

            while (--bufferWindowSize >= 0)
                QueueStreamingBuffer(instance.BufferIDs[bufferWindowSize], instance);
        }
        private void FillSource(AudioFile audio, int sourceID)
        {
            byte[] data = audio?.Samples;
            if (data is null || data.Length == 0)
                return;
            
            int bufferID = AL.GenBuffer();
            CheckError();

            FillBuffer(bufferID, audio, data);

            AL.BindBufferToSource(sourceID, bufferID);
            CheckError();
        }
        private void FillBuffer(int bufferID, AudioFile audio, byte[] chunk)
        {
            ALFormat format = GetSoundFormat(audio.Channels, audio.BitsPerSample);
            AL.BufferData(bufferID, format, chunk, chunk.Length, audio.SampleRate);
            CheckError();
        }
        private void UpdateAudioTick(float delta)
        {
            for (int i = 0; i < PlayingInstances.Count; i++)
            {
                AudioInstance instance = PlayingInstances[i];
                ALSourceState state = AL.GetSourceState(instance.ID);
                if (state == ALSourceState.Stopped)
                {
                    instance.OnStopped();
                    PlayingInstances.RemoveAt(i--);
                }
                else if (instance.IsStreaming)
                {
                    AL.GetSource(instance.ID, ALGetSourcei.BuffersProcessed, out int releasedBufferCount);
                    if (releasedBufferCount > 0)
                    {
                        Engine.Out($"OPENAL: {releasedBufferCount} streaming buffers released.");

                        instance.TotalBuffersProcessed += releasedBufferCount;

                        // For each processed buffer, remove it from the source queue, read the next chunk of
                        // audio data from the file, fill the buffer with new data, and add it to the source queue
                        while (releasedBufferCount-- > 0)
                            QueueStreamingBuffer(AL.SourceUnqueueBuffer(instance.ID), instance);
                    }
                }
            }
        }

        private void QueueStreamingBuffer(int bufferID, AudioInstance instance)
        {
            var audio = instance.SourceFile;
            if (audio is null)
                return;

            int chunkSize = audio.StreamingChunkSize;
            byte[] chunk = instance.StreamingSamplesProperty.ReadRaw(
                instance.NextBufferIndex++ * chunkSize,
                chunkSize,
                audio.FileMapStream);

            if (chunk is null || chunk.Length == 0)
                return;

            FillBuffer(bufferID, audio, chunk);

            AL.SourceQueueBuffer(instance.ID, bufferID);
            Engine.Out($"OPENAL: queued streaming buffer of size {chunk.Length}");
        }

        public override IList<string> SoundDevices => AudioContext.AvailableDevices;

        public AudioContext Context => LazyContext?.Value;
        private Lazy<AudioContext> LazyContext { get; set; }

        #region State
        public override EAudioState GetState(AudioInstance instance)
            => EAudioState.Initial + ((int)AL.GetSourceState(instance.ID) - (int)ALSourceState.Initial);

        public override bool Play(AudioInstance instance)
        {
            CheckError();
            EAudioState state = GetState(instance);
            if (state == EAudioState.Playing)
                return true;

            Engine.Out($"OPENAL: Playing audio {instance.Name}...");
            AL.SourcePlay(instance.ID);
            CheckError();

            PlayingInstances.Add(instance);

            return GetState(instance) == EAudioState.Playing;
        }

        public override bool Pause(AudioInstance instance)
        {
            CheckError();
            EAudioState state = GetState(instance);
            if (state == EAudioState.Paused)
                return true;

            Engine.Out($"OPENAL: Pausing audio {instance.Name}...");
            AL.SourcePause(instance.ID);
            CheckError();

            PlayingInstances.Remove(instance);

            return GetState(instance) == EAudioState.Paused;
        }
        public override bool Stop(AudioInstance instance)
        {
            CheckError();
            EAudioState state = GetState(instance);
            if (state == EAudioState.Stopped)
                return true;

            Engine.Out($"OPENAL: Stopping audio {instance.Name}...");
            AL.SourceStop(instance.ID);
            CheckError();

            PlayingInstances.Remove(instance);

            return GetState(instance) == EAudioState.Stopped;
        }
        public override void Destroy(AudioInstance instance)
        {
            Stop(instance);

            CheckError();
            Engine.Out($"OPENAL: Destroying audio {instance.Name}...");
            AL.DeleteSource(instance.ID);
            CheckError();
            instance.Valid = false;
        }
        #endregion

        #region Properties
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
        #endregion

        private void CheckError()
        {
            if (!CheckErrors)
                return;

            Context?.MakeCurrent();

            ALError error = AL.GetError();
            if (error != ALError.NoError)
                throw new InvalidOperationException("OpenAL error: " + error.ToString());
        }

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
