using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine.Rendering.Animation
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
