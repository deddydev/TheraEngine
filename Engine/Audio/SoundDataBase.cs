using System;
using CustomEngine.Files;

namespace CustomEngine.Audio
{
    public abstract class SoundDataBase : FileObject
    {
        [Serialize("Path")]
        protected WaveFile _waveFile;
        protected internal int _bindingId;
        
        public WaveFile WaveFile
        {
            get => _waveFile;
            set => _waveFile = value;
        }

        public int BufferId { get; internal set; }
        public int SourceId { get; internal set; }

        public void Update(AudioParameters param)
        {
            Engine.AudioManager.Update(this, param);
        }
        public void Play()
        {
            Engine.AudioManager.Play(this);
        }
        public void Play(AudioParameters param)
        {
            Engine.AudioManager.Play(this, param);
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
