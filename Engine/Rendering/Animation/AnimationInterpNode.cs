using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class AnimationInterpNode : PropertyAnim
    {
        KeyframeTrack<AnimInterpKeyFrame> _keyframes;
        float[] _baked;

        public AnimationInterpNode()
        {
            _keyframes = new KeyframeTrack<AnimInterpKeyFrame>(this);
        }

        public float GetOutValue(int frameIndex)
        {
            return _keyframes.GetKeyframe(frameIndex).OutValue;
        }
        public float GetInValue(int frameIndex)
        {
            return _keyframes.GetKeyframe(frameIndex).InValue;
        }

        public override IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_keyframes).GetEnumerator();
        }

        public override void Bake()
        {
            _baked = new float[FrameCount];
            for (int i = 0; i < FrameCount; ++i)
                _baked[i] = GetValue(i);
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
    public class AnimInterpKeyFrame : AnimKeyFrame
    {
        protected float _inValue;
        protected float _inTangent;

        protected float _outValue;
        protected float _outTangent;

        public float InValue { get { return _inValue; } set { _inValue = value; } }
        public float OutValue { get { return _outValue; } set { _outValue = value; } }

        public float InTanget { get { return _inTangent; } set { _inTangent = value; } }
        public float OutTangent { get { return _outTangent; } set { _outTangent = value; } }

        public new AnimInterpKeyFrame Next { get { return _next as AnimInterpKeyFrame; } set { _next = value; } }
        public new AnimInterpKeyFrame Prev { get { return _prev as AnimInterpKeyFrame; } set { _prev = value; } }

        public float Interpolate(int frameIndex)
        {
            float t = (float)(frameIndex - _frameIndex) / (_next.FrameIndex - _frameIndex);
            float t2 = t * t;
            float t3 = t2 * t;
            return (2.0f * t3 - 3.0f * t2 + 1) * _outValue +
                (t3 - 2.0f * t2 + t) * _outTangent +
                (-2.0f * t3 + 3.0f * t2) * Next._inValue +
                (t3 - t2) * Next._inTangent;
        }

        public void AverageKeyframe()
        {
            AverageValues();
            AverageTangents();
        }
        public void AverageTangents()
        {
            _inTangent = _outTangent = (_inTangent + _outTangent) / 2.0f;
        }
        public void AverageValues()
        {
            _inValue = _outValue = (_inValue + _outValue) / 2.0f;
        }
        public void MakeStartLinear()
        {
            _outTangent = (Next.InValue - OutValue) / (Next._frameIndex - _frameIndex);
        }
        public void MakeEndLinear()
        {
            _inTangent = (InValue - Prev.OutValue) / (_frameIndex - Prev._frameIndex);
        }
    }
}
