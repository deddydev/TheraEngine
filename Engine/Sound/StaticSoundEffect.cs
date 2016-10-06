using OpenTK;
using System.Reflection;

namespace CustomEngine.Sound
{
    public class StaticSoundEffect : SoundDataBase
    {
        Vector3 worldPosition;
        float loudness;

        public bool Looped
        {
            get { return _looped; }
            set
            {
                _looped = value;
            }
        }

        public void Play()
        {

        }
    }
}
