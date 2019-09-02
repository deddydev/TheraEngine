using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Animation
{
    public class AnimFuncValueInput : FuncValueInput<AnimFuncValueOutput, AnimationFunction>
    {
        public AnimFuncValueInput(string name, AnimationFunction parent) : base(name, parent) { }
    }
}
