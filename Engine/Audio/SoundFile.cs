﻿using TheraEngine.Files;
using System.ComponentModel;
using System.Collections.Generic;

namespace TheraEngine.Audio
{
    public class SoundFile : TFileObject
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

        public WaveFile WaveFile => _waveFile ?? (_waveFile = new WaveFile(_path));

        public int BufferId
        {
            get => _bufferId;
            internal set => _bufferId = value;
        }
        [TSerialize]
        public string SoundPath
        {
            get => _path;
            set
            {
                _path = value;
                _waveFile = null;//new WaveFile(_path);
            }
        }

        public void StopAllInstances()
        {
            foreach (int id in _sourceIds)
                Engine.Audio.Stop(id);
            _sourceIds.Clear();
        }
        public void StopInstance(int index)
        {
            if (_sourceIds.IndexInRange(index))
            {
                Engine.Audio.Stop(_sourceIds[index]);
                _sourceIds.RemoveAt(index);
            }
        }

        public int PlayingCount => _sourceIds.Count;

        public int Play(int priority)
        {
            int soundId = Engine.Audio.Play(this);
            _sourceIds.Add(soundId);
            return soundId;
        }
        public int Play(AudioSourceParameters param, int priority)
        {
            int soundId = Engine.Audio.Play(this, param);
            _sourceIds.Add(soundId);
            return soundId;
        }
    }
}
