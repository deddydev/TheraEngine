using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Rendering.Animation
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
