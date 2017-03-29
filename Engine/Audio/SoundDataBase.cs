using System;
using CustomEngine.Files;

namespace CustomEngine.Audio
{
    public abstract class SoundDataBase : FileObject
    {
        protected bool _looped;

        public virtual bool Looped
        {
            get { return _looped; }
            set { _looped = value; }
        }

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
