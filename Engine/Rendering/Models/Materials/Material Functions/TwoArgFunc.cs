using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class TwoArgFunc : OneOutFunc
    {
        protected GLTypeName _aType, _bType;
        public TwoArgFunc(GLTypeName aType, GLTypeName bType, GLTypeName outType) : base(outType)
        {
            _aType = aType;
            _bType = bType;
        }

        protected override List<GLArgument> GetArguments()
        {
            return new List<GLArgument>()
            {
                new GLArgument(_aType, "A", this),
                new GLArgument(_bType, "B", this),
            };
        }

        /// <summary>
        /// Provides an option to compare as such: {0} {compare} {1}
        /// If you wish to override this default, override GetOperation instead.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetOperator() { return "UNSPECIFIED"; }
        protected override string GetOperation()
        {
            return base.GetOperation() + "{0} " + GetOperator() + " {1};";
        }
    }
}
