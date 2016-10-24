using System;
using System.Reflection;

namespace CustomEngine.Audio
{
    public class DynamicSoundEffect : SoundDataBase
    {
        public bool Looped
        {
            get { return _looped; }
            set
            {
                _looped = value;
            }
        }
        public void Play(Vec3 worldPosition, float loudness)
        {
            
        }
    }
}
