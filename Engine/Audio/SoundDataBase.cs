using System;
using CustomEngine.Files;

namespace CustomEngine.Audio
{
    public abstract class SoundDataBase : FileObject
    {
        [Serialize("Looped")]
        protected bool _looped;
        [Serialize("WaveFile", External = true)]
        protected WaveFile _waveFile;
        protected internal int _bindingId;
        
        public virtual bool Looped
        {
            get => _looped;
            set => _looped = value;
        }
        public WaveFile WaveFile
        {
            get => _waveFile;
            set => _waveFile = value;
        }
        public int BufferId { get; internal set; }
        public int SourceId { get; internal set; }

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
