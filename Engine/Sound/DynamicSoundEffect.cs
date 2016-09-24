using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
