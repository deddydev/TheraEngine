using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class MaterialUniform : MaterialFunction
    {
        public GLVar InputUniform { get { return OutputArguments[0]; } }

        public MaterialUniform(GLVar inputUniform) : base()
        {
            AddOutput(inputUniform);
        }
        /// <summary>
        /// Output variable name will already be declared outside main body
        /// No need for any operations
        /// </summary>
        protected override string GetOperation()
        {
            return "";
        }
    }
}
