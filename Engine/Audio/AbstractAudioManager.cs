using System;
using System.Collections.Generic;

namespace CustomEngine.Audio
{
    public abstract class AbstractAudioManager
    {
        public abstract void Play(SoundDataBase sound, AudioParameters param);
        public abstract void Stop(SoundDataBase sound);
        public abstract void Pause(SoundDataBase sound);
    }
}
