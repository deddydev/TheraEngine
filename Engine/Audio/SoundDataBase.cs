using System;

namespace CustomEngine.Audio
{
    public abstract class SoundDataBase : FileObject
    {
        protected bool _looped;

        public void Play()
        {
            Engine.AudioManager.Play(this);
        }
        public void Stop()
        {
            Engine.AudioManager.Stop(this);
        }
        public void Pause()
        {
            Engine.AudioManager.Pause(this);
        }
    }
}
