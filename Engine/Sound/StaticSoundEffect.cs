using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                Changed(MethodBase.GetCurrentMethod());
            }
        }

        public void Play()
        {

        }
    }
}
