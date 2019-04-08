using AppDomainToolkit;
using System;
using System.Linq;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    [Serializable]
    public class TConstructorInfo
    {
        public bool IsGenericMethodDefinition;
        public bool IsSecurityCritical;
        public bool IsSecuritySafeCritical;
        public bool IsSecurityTransparent;
        public bool IsPublic;
        public bool IsPrivate;
        public bool IsFamily;
        public bool IsAssembly;
        public bool IsFamilyAndAssembly;
        public bool IsFamilyOrAssembly;
        public bool IsStatic;
        public bool IsFinal;
        public bool IsVirtual;
        public bool IsHideBySig;
        public bool IsAbstract;
        public bool IsGenericMethod;
        public bool ContainsGenericParameters;
        public bool IsConstructor;
        public CallingConventions CallingConvention;
        public MethodAttributes Attributes;
        public RuntimeMethodHandle MethodHandle;
        public MethodImplAttributes MethodImplementationFlags;
        public bool IsSpecialName;
        public TParameterInfo[] Parameters;

        public TConstructorInfo() { }
        public TConstructorInfo(ConstructorInfo info)
        {
            IsGenericMethodDefinition = info.IsGenericMethodDefinition;
            IsSecurityCritical = info.IsSecurityCritical;
            IsSecuritySafeCritical = info.IsSecuritySafeCritical;
            IsSecurityTransparent = info.IsSecurityTransparent;
            IsPublic = info.IsPublic;
            IsPrivate = info.IsPrivate;
            IsFamily = info.IsFamily;
            IsAssembly = info.IsAssembly;
            IsFamilyAndAssembly = info.IsFamilyAndAssembly;
            IsFamilyOrAssembly = info.IsFamilyOrAssembly;
            IsStatic = info.IsStatic;
            IsFinal = info.IsFinal;
            IsVirtual = info.IsVirtual;
            IsHideBySig = info.IsHideBySig;
            IsAbstract = info.IsAbstract;
            IsGenericMethod = info.IsGenericMethod;
            ContainsGenericParameters = info.ContainsGenericParameters;
            IsConstructor = info.IsConstructor;
            CallingConvention = info.CallingConvention;
            Attributes = info.Attributes;
            MethodHandle = info.MethodHandle;
            MethodImplementationFlags = info.MethodImplementationFlags;
            IsSpecialName = info.IsSpecialName;

            Parameters = info.GetParameters().Select(x => new TParameterInfo(x)).ToArray();
        }

        public bool ParameterTypesMatch(TType[] args)
        {
            if (args.Length != Parameters.Length)
                return false;

            for (int i = 0; i < args.Length; ++i)
                if (Parameters[i].ParameterType != args[i])
                    return false;
            
            return true;
        }
        public object Construct(TType owningType, params object[] args)
        {
            int index = owningType.Constructors.IndexOf(this);
            return RemoteFunc.Invoke(owningType.RemoteDomain,
                owningType, args, index,
                (ttype, args2, index2) =>
            {
                Type genType = ttype.CreateType();
                ConstructorInfo constructorInfo = genType.GetConstructors()[index2];
                return constructorInfo.Invoke(args2);
            });
        }
    }
}
