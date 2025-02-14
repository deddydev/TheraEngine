﻿using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Animation
{
    public class AnimFuncExecInput : FuncExecInput<AnimFuncExecOutput, AnimationFunction>
    {
        public AnimFuncExecInput(string name)
            : base(name) { }
        public AnimFuncExecInput(string name, AnimationFunction parent)
            : base(name, parent) { }
    }
}
