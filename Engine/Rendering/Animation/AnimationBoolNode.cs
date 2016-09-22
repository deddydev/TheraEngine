using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class AnimationBoolNode : PropertyAnim
    {
        KeyframeTrack<AnimBoolKeyFrame> _keyframes;
        bool[] _baked;

        public override IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_keyframes).GetEnumerator();
        }

        public override void Bake()
        {

        }

        public override void Resize(int newSize)
        {
            throw new NotImplementedException();
        }

        public override void Stretch(int newSize)
        {
            throw new NotImplementedException();
        }

        public override void Append(PropertyAnim other)
        {
            throw new NotImplementedException();
        }
    }
    public class AnimBoolKeyFrame : AnimKeyFrame
    {
        protected bool _value;

        public new AnimBoolKeyFrame Next { get { return _next as AnimBoolKeyFrame; } set { _next = value; } }
        public new AnimBoolKeyFrame Prev { get { return _prev as AnimBoolKeyFrame; } set { _prev = value; } }

    }
}
