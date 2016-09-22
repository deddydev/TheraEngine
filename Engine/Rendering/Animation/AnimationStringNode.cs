using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class AnimationStringNode : PropertyAnim
    {
        KeyframeTrack<AnimStringKeyFrame> _keyframes;
        string[] _baked;

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
    public class AnimStringKeyFrame : AnimKeyFrame
    {
        protected string _value;

        public new AnimStringKeyFrame Next { get { return _next as AnimStringKeyFrame; } set { _next = value; } }
        public new AnimStringKeyFrame Prev { get { return _prev as AnimStringKeyFrame; } set { _prev = value; } }

    }
}
