using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering.Models;

namespace CustomEngine.Rendering.Animation
{
    public class IfFunction : AnimationFunction
    {
        public override object[] Execute(AnimationContainer output, Skeleton skeleton, object[] input)
        {
            if ((bool)input[0])
            {

            }
            return new object[0];
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
