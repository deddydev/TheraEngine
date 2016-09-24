using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class AnimationBoolNode : PropertyAnimation<AnimBoolKeyFrame>, IEnumerable<AnimBoolKeyFrame>
    {
        bool[] _baked;

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

        public override void Append(PropertyAnimation<AnimBoolKeyFrame> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<AnimBoolKeyFrame> GetEnumerator()
        {
            return ((IEnumerable<AnimBoolKeyFrame>)_keyframes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<AnimBoolKeyFrame>)_keyframes).GetEnumerator();
        }
    }
    public class AnimBoolKeyFrame : Keyframe
    {
        protected bool _value;

        public new AnimBoolKeyFrame Next { get { return _next as AnimBoolKeyFrame; } set { _next = value; } }
        public new AnimBoolKeyFrame Prev { get { return _prev as AnimBoolKeyFrame; } set { _prev = value; } }

    }
}
