using AppDomainToolkit;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;

namespace TheraEngine.Core.Reflection
{
    [Serializable]
    public class TType
    {
        public static ConcurrentDictionary<string, TType> Types = new ConcurrentDictionary<string, TType>();

        [NonSerialized]
        public AppDomain RemoteDomain;

        public string FileUserFriendlyName;
        public string FileDescription;

        public string FileExtension;
        public string[] ImportableExtensions;
        public string[] ExportableExtensions;

        public string[] Extensions3rdParty;

        public bool Is3rdPartyFileObject;
        public bool IsProprietaryFileObject;

        public string FriendlyName;
        public string Name;
        public bool IsVisible;
        public bool IsNotPublic;
        public bool IsPublic;
        public bool IsNestedPublic;
        public bool IsNestedAssembly;
        public bool IsNestedFamily;
        public TypeAttributes Attributes;
        public bool IsNestedFamANDAssem;
        public bool IsNestedFamORAssem;
        public bool IsAutoLayout;
        public bool IsNestedPrivate;
        public bool IsNested;
        public string Namespace;
        public TType BaseType;
        public string AssemblyQualifiedName;
        public bool IsLayoutSequential;
        public string FullName;
        public RuntimeTypeHandle TypeHandle;
        public Guid GUID;
        public string AssemblyFullName;
        public TType ReflectedType;
        public TType DeclaringType;
        public TType UnderlyingSystemType;

        public TType[] Interfaces;
        public TConstructorInfo[] Constructors;
        public string[] AttributeTypeAssemblyQualifiedNames;
        public bool IsExplicitLayout;
        public bool IsValueType;
        public bool IsInterface;
        public bool IsSecuritySafeCritical;
        public bool IsSecurityCritical;
        public TType[] GenericTypeArguments;
        public bool IsMarshalByRef;
        public bool IsContextful;
        public bool HasElementType;
        public bool IsCOMObject;
        public bool IsPrimitive;
        public bool IsPointer;
        public bool IsByRef;
        public bool ContainsGenericParameters;
        public int GenericParameterPosition;
        public GenericParameterAttributes GenericParameterAttributes;
        public bool IsGenericParameter;
        public bool IsConstructedGenericType;
        public bool IsGenericTypeDefinition;
        public bool IsGenericType;
        public bool IsArray;
        public bool IsAutoClass;
        public bool IsUnicodeClass;
        public bool IsAnsiClass;
        public bool IsSerializable;
        public bool IsImport;
        public bool IsSpecialName;
        public bool IsEnum;
        public bool IsSealed;
        public bool IsAbstract;
        public MemberTypes MemberType;
        public bool IsClass;
        public bool IsSecurityTransparent;

        //public Assembly Assembly;
        //public Module Module;
        //public StructLayoutAttribute StructLayoutAttribute;
        //public virtual MethodBase DeclaringMethod;
        //public ConstructorInfo TypeInitializer;

        public TType() { }

        public static TType From(Type type)
            => Types.GetOrAdd(type.AssemblyQualifiedName, k => new TType(type));
        
        private TType(Type type)
        {
            if (type is null)
                throw new InvalidOperationException();

            RemoteDomain = AppDomain.CurrentDomain;

            IsProprietaryFileObject = false;
            Is3rdPartyFileObject = false;
            var attribs = type.GetCustomAttributes();
            foreach (var attrib in attribs)
            {
                if (attrib is TFileExt fileExt)
                {
                    FileExtension = fileExt.Extension;
                    ImportableExtensions = fileExt.ImportableExtensions;
                    ExportableExtensions = fileExt.ExportableExtensions;
                    IsProprietaryFileObject = true;
                }
                else if (attrib is TFileDef fileDef)
                {
                    FileUserFriendlyName = fileDef.UserFriendlyName;
                    FileDescription = fileDef.Description;
                    IsProprietaryFileObject = true;
                }
                else if (attrib is TFile3rdPartyExt file3rd)
                {
                    Extensions3rdParty = file3rd.Extensions;
                    Is3rdPartyFileObject = true;
                }
            }

            FriendlyName = type.GetFriendlyName();
            FullName = type.FullName;
            AssemblyFullName = type.Assembly.FullName;
            Name = type.Name;
            IsVisible = type.IsVisible;
            IsNotPublic = type.IsNotPublic;
            IsPublic = type.IsPublic;
            IsNestedPublic = type.IsNestedPublic;
            IsNestedAssembly = type.IsNestedAssembly;
            IsNestedFamily = type.IsNestedFamily;
            Attributes = type.Attributes;
            IsNestedFamANDAssem = type.IsNestedFamANDAssem;
            IsNestedFamORAssem = type.IsNestedFamORAssem;
            IsAutoLayout = type.IsAutoLayout;
            IsNestedPrivate = type.IsNestedPrivate;
            IsNested = type.IsNested;
            Namespace = type.Namespace;
            AssemblyQualifiedName = type.AssemblyQualifiedName;
            IsLayoutSequential = type.IsLayoutSequential;
            FullName = type.FullName;
            TypeHandle = type.TypeHandle;
            GUID = type.GUID;
            IsExplicitLayout = type.IsExplicitLayout;
            IsValueType = type.IsValueType;
            IsInterface = type.IsInterface;
            IsSecuritySafeCritical = type.IsSecuritySafeCritical;
            IsSecurityCritical = type.IsSecurityCritical;
            GenericTypeArguments = type.GenericTypeArguments.Select(x => new TType(x)).ToArray();
            IsMarshalByRef = type.IsMarshalByRef;
            IsContextful = type.IsContextful;
            HasElementType = type.HasElementType;
            IsCOMObject = type.IsCOMObject;
            IsPrimitive = type.IsPrimitive;
            IsPointer = type.IsPointer;
            IsByRef = type.IsByRef;
            ContainsGenericParameters = type.ContainsGenericParameters;
            IsGenericParameter = type.IsGenericParameter;
            GenericParameterPosition = type.IsGenericParameter ? type.GenericParameterPosition : -1;
            GenericParameterAttributes = type.GenericParameterAttributes;
            IsConstructedGenericType = type.IsConstructedGenericType;
            IsGenericTypeDefinition = type.IsGenericTypeDefinition;
            IsGenericType = type.IsGenericType;
            IsArray = type.IsArray;
            IsAutoClass = type.IsAutoClass;
            IsUnicodeClass = type.IsUnicodeClass;
            IsAnsiClass = type.IsAnsiClass;
            IsSerializable = type.IsSerializable;
            IsImport = type.IsImport;
            IsSpecialName = type.IsSpecialName;
            IsEnum = type.IsEnum;
            IsSealed = type.IsSealed;
            IsAbstract = type.IsAbstract;
            MemberType = type.MemberType;
            IsClass = type.IsClass;
            IsSecurityTransparent = type.IsSecurityTransparent;

            BaseType = type.BaseType is null ? null : new TType(type.BaseType);
            ReflectedType = type.ReflectedType is null ? null : new TType(type.ReflectedType);
            DeclaringType = type.DeclaringType is null ? null : new TType(type.DeclaringType);
            UnderlyingSystemType = type.UnderlyingSystemType is null || 
                type.UnderlyingSystemType.AssemblyQualifiedName.EqualsInvariant(AssemblyQualifiedName)
                ? null : new TType(type.UnderlyingSystemType);

            Interfaces = type.GetInterfaces().Select(x => From(x)).ToArray();
            Constructors = type.GetConstructors().Select(x => new TConstructorInfo(x)).ToArray();
            AttributeTypeAssemblyQualifiedNames = type.GetCustomAttributes(false).Select(x => x.GetType().AssemblyQualifiedName).ToArray();
        }

        public static bool operator ==(TType left, TType right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }
        public static bool operator !=(TType left, TType right)
        {
            if (left is null)
                return !(right is null);
            return !left.Equals(right);
        }
        public static bool operator ==(TType left, Type right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }
        public static bool operator !=(TType left, Type right)
        {
            if (left is null)
                return !(right is null);
            return !left.Equals(right);
        }
        public static bool operator ==(Type left, TType right)
        {
            if (left is null)
                return right is null;
            return right.Equals(left);
        }
        public static bool operator !=(Type left, TType right)
        {
            if (left is null)
                return !(right is null);
            return !right.Equals(left);
        }
        public override bool Equals(object obj)
        {
            var ttype = obj as TType;
            var type = obj as Type;
            if (!(ttype is null))
                return AssemblyQualifiedName.EqualsInvariant(ttype.AssemblyQualifiedName);
            if (!(type is null))
                return AssemblyQualifiedName.EqualsInvariant(type.AssemblyQualifiedName);
            return false;
        }
        public override int GetHashCode() 
            => AssemblyQualifiedName.GetHashCode();

        public Type CreateType()
            => Type.GetType(AssemblyQualifiedName);
        
        public ObjectHandle CreateInstanceFromRemote()
            => RemoteDomain.CreateInstance(AssemblyFullName, FullName);
        public object CreateInstanceAndUnwrapFromRemote()
            => RemoteDomain.CreateInstanceAndUnwrap(AssemblyFullName, FullName);

        public bool FitsConstraints(EGenericVarianceFlag gvf, ETypeConstraintFlag tcf)
        {
            if (gvf != EGenericVarianceFlag.None)
                throw new Exception();

            switch (tcf)
            {
                case ETypeConstraintFlag.Class:
                    return IsClass;
                case ETypeConstraintFlag.NewClass:
                    return IsClass && GetConstructor(new TType[0]) != null;
                case ETypeConstraintFlag.NewStructOrClass:
                    return GetConstructor(new TType[0]) != null;
                case ETypeConstraintFlag.Struct:
                    return IsValueType;
            }
            return true;
        }

        private TConstructorInfo GetConstructor(params TType[] args)
            => Constructors.FirstOrDefault(x => x.ParameterTypesMatch(args));
        
        public bool IsAssignableTo(Type other)
        {
            TType type = this;
            while (!(type is null))
            {
                if (type == other || type.Interfaces.Any(x => x == other))
                    return true;
                type = type.BaseType;
            }
            return false;
        }
        public bool IsAssignableTo(TType other)
        {
            TType type = this;
            while (!(type is null))
            {
                if (type == other || type.Interfaces.Any(x => x == other))
                    return true;
                type = type.BaseType;
            }
            return false;
        }
        public TType MakeGenericType(params TType[] typeArguments)
        {
            return RemoteFunc.Invoke(RemoteDomain, this, (ttype) =>
            {
                Type genType = ttype.CreateType();
                Type[] argTypes = typeArguments.Select(t => t.CreateType()).ToArray();
                return new TType(genType?.MakeGenericType(argTypes));
            });
        }

        public bool HasAttribute(string assemblyQualifiedName) 
            => AttributeTypeAssemblyQualifiedNames.Contains(
                assemblyQualifiedName, StringComparison.InvariantCulture);

        public bool IsSubclassOf(Type other)
        {
            TType type = BaseType;
            while (!(type is null))
            {
                if (type == other)
                    return true;
                type = type.BaseType;
            }
            return false;
        }
        public void GetGenericParameterConstraints(out EGenericVarianceFlag gvf, out ETypeConstraintFlag tcf)
        {
            GenericParameterAttributes gpa = GenericParameterAttributes;
            GenericParameterAttributes variance = gpa & GenericParameterAttributes.VarianceMask;
            GenericParameterAttributes constraints = gpa & GenericParameterAttributes.SpecialConstraintMask;

            gvf = EGenericVarianceFlag.None;
            tcf = ETypeConstraintFlag.None;

            if (variance != GenericParameterAttributes.None)
            {
                if ((variance & GenericParameterAttributes.Covariant) != 0)
                    gvf = EGenericVarianceFlag.CovariantOut;
                else
                    gvf = EGenericVarianceFlag.ContravariantIn;
            }

            if (constraints != GenericParameterAttributes.None)
            {
                if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
                    tcf = ETypeConstraintFlag.Struct;
                else
                {
                    if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
                        tcf = ETypeConstraintFlag.NewStructOrClass;
                    if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
                    {
                        if (tcf == ETypeConstraintFlag.NewStructOrClass)
                            tcf = ETypeConstraintFlag.NewClass;
                        else
                            tcf = ETypeConstraintFlag.Class;
                    }
                }
            }
        }

        //public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) => throw new NotImplementedException();
        //protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => throw new NotImplementedException();
        //public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => throw new NotImplementedException();
        //protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => throw new NotImplementedException();
        //public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => throw new NotImplementedException();
        //public override FieldInfo GetField(string name, BindingFlags bindingAttr) => throw new NotImplementedException();
        //public override FieldInfo[] GetFields(BindingFlags bindingAttr) => throw new NotImplementedException();
        //public override Type GetInterface(string name, bool ignoreCase) => throw new NotImplementedException();
        //public override Type[] GetInterfaces() => throw new NotImplementedException();
        //public override EventInfo GetEvent(string name, BindingFlags bindingAttr) => throw new NotImplementedException();
        //public override EventInfo[] GetEvents(BindingFlags bindingAttr) => throw new NotImplementedException();
        //protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) => throw new NotImplementedException();
        //public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => throw new NotImplementedException();
        //public override Type[] GetNestedTypes(BindingFlags bindingAttr) => throw new NotImplementedException();
        //public override Type GetNestedType(string name, BindingFlags bindingAttr) => throw new NotImplementedException();
        //public override MemberInfo[] GetMembers(BindingFlags bindingAttr) => throw new NotImplementedException();
        //protected override TypeAttributes GetAttributeFlagsImpl() => throw new NotImplementedException();
        //protected override bool IsArrayImpl() => throw new NotImplementedException();
        //protected override bool IsByRefImpl() => throw new NotImplementedException();
        //protected override bool IsPointerImpl() => throw new NotImplementedException();
        //protected override bool IsPrimitiveImpl() => throw new NotImplementedException();
        //protected override bool IsCOMObjectImpl() => throw new NotImplementedException();
        //public override Type GetElementType() => throw new NotImplementedException();
        //protected override bool HasElementTypeImpl() => throw new NotImplementedException();
        //public override object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException();
        //public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotImplementedException();
        //public override bool IsDefined(Type attributeType, bool inherit) => throw new NotImplementedException();
    }
}
