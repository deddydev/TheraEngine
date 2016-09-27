using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Animation
{
    public class AnimationBoolNode : PropertyAnimation<BoolKeyframe>, IEnumerable<BoolKeyframe>
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

        public override void Append(PropertyAnimation<BoolKeyframe> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<BoolKeyframe> GetEnumerator()
        {
            return ((IEnumerable<BoolKeyframe>)_keyframes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<BoolKeyframe>)_keyframes).GetEnumerator();
        }
    }
    public class BoolKeyframe : Keyframe
    {
        protected bool _value;

        public new BoolKeyframe Next { get { return _next as BoolKeyframe; } set { _next = value; } }
        public new BoolKeyframe Prev { get { return _prev as BoolKeyframe; } set { _prev = value; } }

    }
}
