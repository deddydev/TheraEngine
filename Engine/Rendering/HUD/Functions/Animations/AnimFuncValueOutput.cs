using System.Linq;

namespace CustomEngine.Rendering.Animation
{
    public class AnimFuncValueOutput : FuncValueOutput<AnimFuncValueInput, AnimationFunction>
    {
        public AnimFuncValueOutput(string name, params AnimArgType[] types)
            : base(name, types.Select(x => (int)x).ToArray()) { }
        public AnimFuncValueOutput(string name, AnimationFunction parent, params AnimArgType[] types)
            : base(name, parent, types.Select(x => (int)x).ToArray()) { }
        public AnimFuncValueOutput(string name, AnimFuncValueInput linkedMultiArg)
            : base(name, linkedMultiArg) { }
        public AnimFuncValueOutput(string name, AnimationFunction parent, AnimFuncValueInput linkedMultiArg)
            : base(name, parent, linkedMultiArg) { }
    }
}
