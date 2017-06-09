using System;
using CustomEngine.Files;
using System.ComponentModel;
using System.Collections.Generic;

namespace CustomEngine.Audio
{
    public class SoundFile : FileObject
    {
        private string _path;
        protected WaveFile _waveFile;
        protected internal int _bufferId;
        protected List<int> _sourceIds = new List<int>();

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

        public int PlayingCount => _sourceIds.Count;

        public int Play()
        {
            int soundId = Engine.AudioManager.Play(this);
            _sourceIds.Add(soundId);
            return soundId;
        }
        public int Play(AudioSourceParameters param)
        {
            int soundId = Engine.AudioManager.Play(this, param);
            _sourceIds.Add(soundId);
            return soundId;
        }
    }
}
