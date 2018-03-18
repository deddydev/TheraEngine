using System.Collections.Generic;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Animation
{
    public class IfFunction : AnimationFunction
    {
        protected override void Execute(AnimationContainer output, Skeleton skeleton, object[] input)
        {
            if ((bool)input[0])
            {

            }
        }
        protected override AnimFuncValueInput[] GetValueInputs()
        {
            return new AnimFuncValueInput[]
            {
                new AnimFuncValueInput("Condition")
            };
        }

    }
}
