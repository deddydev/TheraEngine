﻿using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Animation
{
    public class AnimFuncExecOutput : FuncExecOutput<AnimFuncExecInput, AnimationFunction>
    {
        public AnimFuncExecOutput(string name)
            : base(name) { }
        public AnimFuncExecOutput(string name, AnimationFunction parent)
            : base(name, parent) { }
    }
}
