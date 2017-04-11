﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    /// <summary>
    /// a * (1 − t) + b * t. 
    /// </summary>
    [FunctionDefinition(
                "Math",
                "Linear Interpolate",
                "Linearly interpolates between A and B using Time: A + (B - A) * Time",
                "lerp mix linear interpolate blend")]
    public class LerpFunc : MaterialFunction
    {
        MatFuncValueInput A, B, Time;
        MatFuncValueOutput Result;

        public LerpFunc() : base(true) { }
        protected override string GetOperation() { return "mix({0}, {1}, {2})"; }
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            A = new MatFuncValueInput("A", FloatingPointTypes);
            B = new MatFuncValueInput("B", A);
            Time = new MatFuncValueInput("Time", GLTypeName._float);

            return new List<MatFuncValueInput>() { A, B, Time };
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            Result = new MatFuncValueOutput("Result", A);
            return new List<MatFuncValueOutput>() { Result };
        }
    }
}
