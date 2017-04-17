using System;
using CustomEngine.Worlds.Actors;

namespace CustomEngine.Audio
{
    public abstract class AbstractAudioManager
    {
        public abstract void Play(SoundFile sound, AudioSourceParameters param);
        public abstract void Stop(SoundFile sound);
        public abstract void Pause(SoundFile sound);
        public abstract void Play(SoundFile sound);
        public abstract void Update(SoundFile sound, AudioSourceParameters param);
        public abstract AudioState GetState(SoundFile sound);
        public abstract void UpdateListener(PlayerIndex player, Vec3 position, Vec3 forward, Vec3 up, Vec3 velocity, float gain, float efxMetersPerUnit = 1.0f);
    }
    public enum AudioState
    {
        Initial,
        Playing,
        Paused,
        Stopped,
    }
}
