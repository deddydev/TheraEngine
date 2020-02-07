using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Animation
{
    public class BlendSpace1D : TFileObject
    {
        [TSerialize("Poses")]
        SkeletalAnimation[] _poses;
        [TSerialize("BlendAmount")]
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
