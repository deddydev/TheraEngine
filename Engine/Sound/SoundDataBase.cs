using System;

namespace CustomEngine.Sound
{
    public abstract class SoundDataBase : FileObject
    {
        protected bool _looped;

        public void Play()
        {
            Engine.AudioManager.Play(source);
        }
        public void Stop()
        {
            Engine.AudioManager.Stop(source);
        }
        public void Pause()
        {
            Engine.AudioManager.Pause(source);
        }
    }
}
