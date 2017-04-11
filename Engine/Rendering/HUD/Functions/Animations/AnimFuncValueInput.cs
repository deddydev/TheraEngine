﻿using System.Linq;

namespace CustomEngine.Rendering.Animation
{
    public class AnimFuncValueInput : FuncValueInput<AnimFuncValueOutput, AnimationFunction>
    {
        public AnimFuncValueInput(string name, params AnimArgType[] types)
            : base(name, types.Select(x => (int)x).ToArray()) { }
        public AnimFuncValueInput(string name, AnimationFunction parent, params AnimArgType[] types)
            : base(name, parent, types.Select(x => (int)x).ToArray()) { }
        public AnimFuncValueInput(string name, AnimFuncValueOutput linkedMultiArg)
            : base(name, linkedMultiArg) { }
        public AnimFuncValueInput(string name, AnimationFunction parent, AnimFuncValueOutput linkedMultiArg)
            : base(name, parent, linkedMultiArg) { }
    }
}
