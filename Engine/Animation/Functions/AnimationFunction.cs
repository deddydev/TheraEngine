﻿using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Animation
{
    public abstract class AnimationFunction
        : Function<AnimFuncValueInput, AnimFuncValueOutput, AnimFuncExecInput, AnimFuncExecOutput>
    {
        public bool HasExecuted => _results != null;
        private object[] _results = null;
        public T GetOutputValue<T>(int index)
        {
            if (_results is null)
            {

            }
            return (T)_results[index];
        }
        public AnimationFunction() : base()
        {

        }
        protected abstract void Execute(AnimationTree output, ISkeleton skeleton, object[] input);
    }
    public enum EAnimArgType : int
    {
        Invalid = -1,
        String,
        Enum,
        Integer,
        Decimal,
        Vec2,
        Vec3,
        Vec4,
        Matrix3,
        Matrix4,
        Bone,
        Skeleton,
        Rotator,
        Quaternion,
    }
}
