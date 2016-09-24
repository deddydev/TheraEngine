using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class AnimationStringNode : PropertyAnimation<AnimStringKeyFrame>, IEnumerable<AnimStringKeyFrame>
    {
        string[] _baked;

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

        public override void Append(PropertyAnimation<AnimStringKeyFrame> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<AnimStringKeyFrame> GetEnumerator()
        {
            return ((IEnumerable<AnimStringKeyFrame>)_keyframes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<AnimStringKeyFrame>)_keyframes).GetEnumerator();
        }
    }
    public class AnimStringKeyFrame : Keyframe
    {
        protected string _value;

        public new AnimStringKeyFrame Next { get { return _next as AnimStringKeyFrame; } set { _next = value; } }
        public new AnimStringKeyFrame Prev { get { return _prev as AnimStringKeyFrame; } set { _prev = value; } }

    }
}
