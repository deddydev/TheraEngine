using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Animation
{
    public class AnimFuncValueOutput : FuncValueOutput<AnimFuncValueInput, AnimationFunction>
    {
        public AnimFuncValueOutput(string name, AnimationFunction parent) : base(name, parent) { }
    }
}
