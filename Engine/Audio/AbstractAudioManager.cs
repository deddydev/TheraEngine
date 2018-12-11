using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Audio
{
    public abstract class AbstractAudioManager
    {
        /// <summary>
        /// Plays a sound file using its id.
        /// </summary>
        /// <param name="soundId"></param>
        /// <returns></returns>
        public abstract bool Play(int soundId);
        /// <summary>
        /// Plays the sound file.
        /// </summary>
        /// <param name="sound">The sound to play.</param>
        /// <returns>An unique identifier value for the given sound.</returns>
        public abstract int Play(AudioFile sound);
        /// <summary>
        /// Plays the sound file using the provided parameters.
        /// </summary>
        /// <param name="sound">The sound to play.</param>
        /// <param name="param">The parameters to use the play the sound.</param>
        /// <returns>An unique identifier value for the given sound.</returns>
        public abstract int Play(AudioFile sound, AudioParameters param);
        public abstract bool Stop(int soundId);
        public abstract bool Pause(int soundId);
        public abstract void Update(int soundId, AudioParameters param);
        public abstract AudioState GetState(int soundId);

        public abstract void UpdateListener(
            Vec3 position, 
            Vec3 forward, 
            Vec3 up, 
            Vec3 velocity,
            float gain, 
            float efxMetersPerUnit = 1.0f);
    }
    public enum AudioState
    {
        Initial,
        Playing,
        Paused,
        Stopped,
    }
}
