using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class AnimationInterpNode : PropertyAnimation<InterpKeyframe>, IEnumerable<InterpKeyframe>
    {
        float[] _baked;

        public AnimationInterpNode()
        {
            _keyframes = new KeyframeTrack<InterpKeyframe>(this);
        }
        public float GetValueGame(int frameIndex)
        {
            return _baked[frameIndex];
        }
        public float GetValueEditor(float frameIndex)
        {
            InterpKeyframe key = _keyframes.GetKeyBefore(frameIndex);
            if (key != null)
                return key.Interpolate(frameIndex);
            throw new Exception("Invalid frame index.");
        }
        /// <summary>
        /// Bakes the interpolated data for fastest access by the game.
        /// </summary>
        public override void Bake()
        {
            _baked = new float[FrameCount];
            for (int i = 0; i < FrameCount; ++i)
                _baked[i] = GetValueEditor(i);
        }

        public override void Resize(int newSize)
        {
            throw new NotImplementedException();
        }

        public override void Stretch(int newSize)
        {
            throw new NotImplementedException();
        }

        public override void Append(PropertyAnimation<InterpKeyframe> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<InterpKeyframe> GetEnumerator()
        {
            return ((IEnumerable<InterpKeyframe>)_keyframes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<InterpKeyframe>)_keyframes).GetEnumerator();
        }
    }
    public class InterpKeyframe : Keyframe
    {
        protected float _inValue;
        protected float _inTangent;

        protected float _outValue;
        protected float _outTangent;

        public float InValue { get { return _inValue; } set { _inValue = value; } }
        public float OutValue { get { return _outValue; } set { _outValue = value; } }

        public float InTanget { get { return _inTangent; } set { _inTangent = value; } }
        public float OutTangent { get { return _outTangent; } set { _outTangent = value; } }

        public new InterpKeyframe Next { get { return _next as InterpKeyframe; } set { _next = value; } }
        public new InterpKeyframe Prev { get { return _prev as InterpKeyframe; } set { _prev = value; } }

        public float Interpolate(float frameIndex)
        {
            if (frameIndex < _frameIndex)
                return Prev.Interpolate(frameIndex);
            if (frameIndex > _next._frameIndex)
                return Next.Interpolate(frameIndex);

            float t = (frameIndex - _frameIndex) / (_next._frameIndex - _frameIndex);
            float t2 = t * t;
            float t3 = t2 * t;
            return (2.0f * t3 - 3.0f * t2 + 1.0f) * _outValue +
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
