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

        public GLSubVar(
            GLTypeName typeName, 
            string name, 
            string accessorSyntax,
            IGLVarOwner owner) 
            : base(typeName, name, owner)
        {
            _accessorSyntax = accessorSyntax;
        }

        public override string AccessorTree()
        {
            string s = Name;
            IGLVarOwner owner = _owner;
            while (owner != null && owner is GLSubVar)
            {
                GLSubVar owningVar = (GLSubVar)owner;
                s = owningVar.Name + s;
                owner = owningVar.Owner;
            }
            return s;
        }
    }
}
