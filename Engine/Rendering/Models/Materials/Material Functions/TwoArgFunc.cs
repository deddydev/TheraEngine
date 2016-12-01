using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class TwoArgFunc : MaterialFunction
    {
        protected GLTypeName _aType, _bType, _outType;
        public TwoArgFunc(GLTypeName aType, GLTypeName bType, GLTypeName outType)
        {
            _aType = aType;
            _bType = bType;
            _outType = outType;
            _inline = true;
        }

        /// <summary>
        /// Provides an option to compare as such: {0} {compare} {1}
        /// If you wish to override this default, override GetOperation instead.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetOperator() { return "UNSPECIFIED"; }
        protected override string GetOperation()
        {
            return "{0} " + GetOperator() + " {1};";
        }
    }
}
