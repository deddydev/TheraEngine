using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEditor.Core.Extensions
{
    public static class TypeExtension
    {
        public static bool FitsConstraints(this Type t, GenericVarianceFlag gvf, TypeConstraintFlag tcf)
        {
            if (gvf != GenericVarianceFlag.None)
                throw new Exception();

            switch (tcf)
            {
                case TypeConstraintFlag.Class:
                    return t.IsClass;
                case TypeConstraintFlag.NewClass:
                    return t.IsClass && t.GetConstructor(new Type[0]) != null;
                case TypeConstraintFlag.NewStructOrClass:
                    return t.GetConstructor(new Type[0]) != null;
                case TypeConstraintFlag.Struct:
                    return t.IsValueType;
            }
            return true;
        }
    }
}
