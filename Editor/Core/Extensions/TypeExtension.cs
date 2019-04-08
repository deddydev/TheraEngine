using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEditor.Core.Extensions
{
    public static class TypeExtension
    {
        public static bool FitsConstraints(this Type t, EGenericVarianceFlag gvf, ETypeConstraintFlag tcf)
        {
            if (gvf != EGenericVarianceFlag.None)
                throw new Exception();

            switch (tcf)
            {
                case ETypeConstraintFlag.Class:
                    return t.IsClass;
                case ETypeConstraintFlag.NewClass:
                    return t.IsClass && t.GetConstructor(new Type[0]) != null;
                case ETypeConstraintFlag.NewStructOrClass:
                    return t.GetConstructor(new Type[0]) != null;
                case ETypeConstraintFlag.Struct:
                    return t.IsValueType;
            }
            return true;
        }
    }
}
