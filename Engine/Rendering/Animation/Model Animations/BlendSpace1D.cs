using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class BlendSpace1D : FileObject
    {
        [Serialize("Poses")]
        AnimationContainer[] _poses;
        [Serialize("BlendAmount")]
        private float _blendAmount = 0.0f;

        AnimationContainer _result;

        public float BlendAmount
        {
            get => _blendAmount;
            set
            {
                _blendAmount = value;
            }
        }
    }
}
