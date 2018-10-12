using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public class PropAnimBool : PropAnimKeyframed<BoolKeyframe>, IEnumerable<BoolKeyframe>
    {
        private DelGetValue<bool> _getValue;
        
        [TSerialize(Condition = "IsBaked")]
        private bool[] _baked = null;
        /// <summary>
        /// The default value to return when no keyframes are set.
        /// </summary>
        [Category(PropAnimCategory)]
        [TSerialize(Condition = "!IsBaked")]
        public bool DefaultValue { get; set; } = false;

        public PropAnimBool() : base(0.0f, false) { }
        public PropAnimBool(float lengthInSeconds, bool looped, bool useKeyframes)
            : base(lengthInSeconds, looped, useKeyframes) { }
        public PropAnimBool(int frameCount, float FPS, bool looped, bool useKeyframes) 
            : base(frameCount, FPS, looped, useKeyframes) { }

        protected override void BakedChanged()
            => _getValue = !IsBaked ? (DelGetValue<bool>)GetValueKeyframed : GetValueBaked;

        private bool _value = false;
        protected override object GetCurrentValueGeneric() => _value;
        public bool GetValue(float second)
            => _getValue(second);
        protected override object GetValueGeneric(float second)
            => _getValue(second);
        public bool GetValueBaked(float second)
            => _baked[(int)Math.Floor(second * BakedFramesPerSecond)];
        public bool GetValueBaked(int frameIndex)
            => _baked[frameIndex];
        public bool GetValueKeyframed(float frameIndex)
        {
            BoolKeyframe key = _keyframes.GetKeyBefore(frameIndex);
            if (key != null)
                return key.Value;
            return DefaultValue;
        }

        public override void Bake(float framesPerSecond)
        {
            _bakedFPS = framesPerSecond;
            _bakedFrameCount = (int)Math.Ceiling(LengthInSeconds * framesPerSecond);
            _baked = new bool[BakedFrameCount];
            for (int i = 0; i < BakedFrameCount; ++i)
                _baked[i] = GetValueKeyframed(i);
        }

        protected override void OnProgressed(float delta)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolKeyframe : Keyframe, IStepKeyframe
    {
        public BoolKeyframe() { }
        public BoolKeyframe(int frameIndex, float FPS, bool value)
            : this(frameIndex / FPS, value) { }
        public BoolKeyframe(float second, bool value) : base()
        {
            Second = second;
            Value = value;
        }
        
        [TSerialize(NodeType = ENodeType.Attribute)]
        public bool Value { get; set; }

        [Browsable(false)]
        public new BoolKeyframe Next
        {
            get => _next as BoolKeyframe;
            set => _next = value;
        }
        [Browsable(false)]
        public new BoolKeyframe Prev
        {
            get => _prev as BoolKeyframe;
            set => _prev = value;
        }

        public override void ReadFromString(string str)
        {
            int spaceIndex = str.IndexOf(' ');
            Second = float.Parse(str.Substring(0, spaceIndex));
            Value = bool.Parse(str.Substring(spaceIndex + 1));
        }
        public override string WriteToString()
        {
            return string.Format("{0} {1}", Second, Value);
        }
    }
}
