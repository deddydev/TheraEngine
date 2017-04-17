using System;
using System.Collections.Generic;

namespace CustomEngine.Audio
{
    public abstract class AbstractAudioManager
    {
        public abstract void Play(SoundFile sound, AudioParameters param);
        public abstract void Stop(SoundFile sound);
        public abstract void Pause(SoundFile sound);
        public abstract void Play(SoundFile sound);
        public abstract void Update(SoundFile sound, AudioParameters param);
        public abstract AudioState GetState(SoundFile sound);
    }
    public enum AudioState
    {
        Initial,
        Playing,
        Paused,
        Stopped,
    }
}
