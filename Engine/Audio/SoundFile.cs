using System;
using CustomEngine.Files;
using System.ComponentModel;

namespace CustomEngine.Audio
{
    public class SoundFile : FileObject
    {
        private string _path;
        protected WaveFile _waveFile;
        protected internal int _bufferId, _sourceId;

        public SoundFile()
        {
            _path = null;
            _waveFile = null;
        }
        public SoundFile(string path)
        {
            _path = path;
            _waveFile = null;
        }

        public WaveFile WaveFile
        {
            get
            {
                return _waveFile ?? (_waveFile = new WaveFile(_path));
            }
        }

        public int BufferId
        {
            get => _bufferId;
            internal set => _bufferId = value;
        }
        public int SourceId
        {
            get => _sourceId;
            internal set => _sourceId = value;
        }
        [Serialize]
        public string SoundPath
        {
            get => _path;
            set
            {
                _path = value;
                _waveFile = null;//new WaveFile(_path);
            }
        }

        public void Update(AudioSourceParameters param)
        {
            Engine.AudioManager.Update(this, param);
        }
        public void Play()
        {
            Engine.AudioManager.Play(this);
        }
        public void Play(AudioSourceParameters param)
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
