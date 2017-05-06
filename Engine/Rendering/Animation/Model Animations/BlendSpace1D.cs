using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class BlendSpace1D : FileObject
    {
        [Serialize("Poses")]
        ModelAnimation[] _poses;
        [Serialize("BlendAmount")]
        private float _blendAmount = 0.0f;

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
