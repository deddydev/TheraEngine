using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLSubVar : GLVar
    {
        protected string _accessorSyntax;

        public GLSubVar(GLTypeName typeName, string userName, string accessorSyntax) 
            : base(typeName, userName)
        {
            _accessorSyntax = accessorSyntax;
        }
    }
}
