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
        protected override List<AnimFuncValueInput> GetValueInputs()
        {
            return new List<AnimFuncValueInput>()
            {
                new AnimFuncValueInput("Condition")
            };
        }

    }
}
