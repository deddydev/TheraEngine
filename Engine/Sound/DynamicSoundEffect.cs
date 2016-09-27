using OpenTK;
using System.Reflection;

namespace CustomEngine.Sound
{
    public class DynamicSoundEffect : SoundDataBase
    {
        public bool Looped
        {
            get { return _looped; }
            set
            {
                _looped = value;
                Changed(MethodBase.GetCurrentMethod());
            }
        }
        public void Play(Vector3 worldPosition, float loudness)
        {
            
        }
    }
}
