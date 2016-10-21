using System;
using System.Collections.Generic;
using CustomEngine.Sound;

namespace CustomEngine.Rendering
{
    public abstract class AbstractAudio
    {
        public abstract void Play(SoundDataBase sound);
        public abstract void Stop(SoundDataBase sound);
        public abstract void Pause(SoundDataBase sound);
    }
}
