using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class AnimationStringNode : PropertyAnimation<StringKeyframe>, IEnumerable<StringKeyframe>
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

        public override void Append(PropertyAnimation<StringKeyframe> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<StringKeyframe> GetEnumerator()
        {
            return ((IEnumerable<StringKeyframe>)_keyframes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<StringKeyframe>)_keyframes).GetEnumerator();
        }
    }
    public class StringKeyframe : Keyframe
    {
        protected string _value;

        public new StringKeyframe Next { get { return _next as StringKeyframe; } set { _next = value; } }
        public new StringKeyframe Prev { get { return _prev as StringKeyframe; } set { _prev = value; } }
    }
}
