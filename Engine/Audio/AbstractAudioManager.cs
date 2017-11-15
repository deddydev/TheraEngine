using System;

namespace TheraEngine.Audio
{
    public abstract class AbstractAudioManager
    {
        public abstract bool Play(int soundId);
        public abstract int Play(SoundFile sound);
        public abstract int Play(SoundFile sound, AudioSourceParameters param);
        public abstract bool Stop(int soundId);
        public abstract bool Pause(int soundId);
        public abstract void Update(int soundId, AudioSourceParameters param);
        public abstract AudioState GetState(int soundId);
        public abstract void UpdateListener(Vec3 position, Vec3 forward, Vec3 up, Vec3 velocity, float gain, float efxMetersPerUnit = 1.0f);
    }
    public enum AudioState
    {
        Initial,
        Playing,
        Paused,
        Stopped,
    }
}
