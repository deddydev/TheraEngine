using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace TheraEngine.Core.Reflection
{
    public abstract class MethodBaseProxy : MemberInfoProxy
    {
        public static MethodBaseProxy Get(MethodBase methodBase)
        {
            switch (methodBase)
            {
                case ConstructorInfo constructorInfo:
                    return ConstructorInfoProxy.Get(constructorInfo);
                case MethodInfo methodInfo:
                    return MethodInfoProxy.Get(methodInfo);
                default:
                    return null;
            }
        }
        
        private MethodBase Value { get; set; }

        //public MethodBaseProxy() { }
        protected MethodBaseProxy(MethodBase value) : base(value) => Value = value;

        public override bool IsGridViewable => IsPublic;
        public override bool IsGridWriteable => false;
        //
        // Summary:
        //     Gets a value indicating whether the method is a generic method definition.
        //
        // Returns:
        //     true if the current System.Reflection.MethodBase object represents the definition
        //     of a generic method; otherwise, false.
        public bool IsGenericMethodDefinition
            => Value.IsGenericMethodDefinition;
        //
        // Summary:
        //     Gets a value that indicates whether the current method or constructor is security-critical
        //     or security-safe-critical at the current trust level, and therefore can perform
        //     critical operations.
        //
        // Returns:
        //     true if the current method or constructor is security-critical or security-safe-critical
        //     at the current trust level; false if it is transparent.
        public bool IsSecurityCritical
            => Value.IsSecurityCritical;
        //
        // Summary:
        //     Gets a value that indicates whether the current method or constructor is security-safe-critical
        //     at the current trust level; that is, whether it can perform critical operations
        //     and can be accessed by transparent code.
        //
        // Returns:
        //     true if the method or constructor is security-safe-critical at the current trust
        //     level; false if it is security-critical or transparent.
        public bool IsSecuritySafeCritical
            => Value.IsSecuritySafeCritical;
        //
        // Summary:
        //     Gets a value that indicates whether the current method or constructor is transparent
        //     at the current trust level, and therefore cannot perform critical operations.
        //
        // Returns:
        //     true if the method or constructor is security-transparent at the current trust
        //     level; otherwise, false.
        public bool IsSecurityTransparent
            => Value.IsSecurityTransparent;
        //
        // Summary:
        //     Gets a value indicating whether this is a public method.
        //
        // Returns:
        //     true if this method is public; otherwise, false.
        public bool IsPublic
            => Value.IsPublic;
        //
        // Summary:
        //     Gets a value indicating whether this member is private.
        //
        // Returns:
        //     true if access to this method is restricted to other members of the class itself;
        //     otherwise, false.
        public bool IsPrivate
            => Value.IsPrivate;
        //
        // Summary:
        //     Gets a value indicating whether the visibility of this method or constructor
        //     is described by System.Reflection.MethodAttributes.Family; that is, the method
        //     or constructor is visible only within its class and derived classes.
        //
        // Returns:
        //     true if access to this method or constructor is exactly described by System.Reflection.MethodAttributes.Family;
        //     otherwise, false.
        public bool IsFamily
            => Value.IsFamily;
        //
        // Summary:
        //     Gets a value indicating whether the potential visibility of this method or constructor
        //     is described by System.Reflection.MethodAttributes.Assembly; that is, the method
        //     or constructor is visible at most to other types in the same assembly, and is
        //     not visible to derived types outside the assembly.
        //
        // Returns:
        //     true if the visibility of this method or constructor is exactly described by
        //     System.Reflection.MethodAttributes.Assembly; otherwise, false.
        public bool IsAssembly
            => Value.IsAssembly;
        //
        // Summary:
        //     Gets a value indicating whether the visibility of this method or constructor
        //     is described by System.Reflection.MethodAttributes.FamANDAssem; that is, the
        //     method or constructor can be called by derived classes, but only if they are
        //     in the same assembly.
        //
        // Returns:
        //     true if access to this method or constructor is exactly described by System.Reflection.MethodAttributes.FamANDAssem;
        //     otherwise, false.
        public bool IsFamilyAndAssembly
            => Value.IsFamilyAndAssembly;
        //
        // Summary:
        //     Gets a value indicating whether the potential visibility of this method or constructor
        //     is described by System.Reflection.MethodAttributes.FamORAssem; that is, the method
        //     or constructor can be called by derived classes wherever they are, and by classes
        //     in the same assembly.
        //
        // Returns:
        //     true if access to this method or constructor is exactly described by System.Reflection.MethodAttributes.FamORAssem;
        //     otherwise, false.
        public bool IsFamilyOrAssembly
            => Value.IsFamilyOrAssembly;
        //
        // Summary:
        //     Gets a value indicating whether the method is static.
        //
        // Returns:
        //     true if this method is static; otherwise, false.
        public bool IsStatic
            => Value.IsStatic;
        //
        // Summary:
        //     Gets a value indicating whether this method is final.
        //
        // Returns:
        //     true if this method is final; otherwise, false.
        public bool IsFinal
            => Value.IsFinal;
        //
        // Summary:
        //     Gets a value indicating whether the method is virtual.
        //
        // Returns:
        //     true if this method is virtual; otherwise, false.
        public bool IsVirtual
            => Value.IsVirtual;
        //
        // Summary:
        //     Gets a value indicating whether only a member of the same kind with exactly the
        //     same signature is hidden in the derived class.
        //
        // Returns:
        //     true if the member is hidden by signature; otherwise, false.
        public bool IsHideBySig
            => Value.IsHideBySig;
        //
        // Summary:
        //     Gets a value indicating whether the method is abstract.
        //
        // Returns:
        //     true if the method is abstract; otherwise, false.
        public bool IsAbstract
            => Value.IsAbstract;
        //
        // Summary:
        //     Gets a value indicating whether the method is generic.
        //
        // Returns:
        //     true if the current System.Reflection.MethodBase represents a generic method;
        //     otherwise, false.
        public bool IsGenericMethod
            => Value.IsGenericMethod;
        //
        // Summary:
        //     Gets a value indicating whether the generic method contains unassigned generic
        //     type parameters.
        //
        // Returns:
        //     true if the current System.Reflection.MethodBase object represents a generic
        //     method that contains unassigned generic type parameters; otherwise, false.
        public bool ContainsGenericParameters
            => Value.ContainsGenericParameters;
        //
        // Summary:
        //     Gets a value indicating whether the method is a constructor.
        //
        // Returns:
        //     true if this method is a constructor represented by a System.Reflection.ConstructorInfo
        //     object (see note in Remarks about System.Reflection.Emit.ConstructorBuilder objects);
        //     otherwise, false.
        [ComVisible(true)]
        public bool IsConstructor
            => Value.IsConstructor;
        //
        // Summary:
        //     Gets a value indicating the calling conventions for this method.
        //
        // Returns:
        //     The System.Reflection.CallingConventions for this method.
        public CallingConventions CallingConvention
            => Value.CallingConvention;
        //
        // Summary:
        //     Gets the attributes associated with this method.
        //
        // Returns:
        //     One of the System.Reflection.MethodAttributes values.
        public MethodAttributes Attributes
            => Value.Attributes;
        //
        // Summary:
        //     Gets a handle to the internal metadata representation of a method.
        //
        // Returns:
        //     A System.RuntimeMethodHandle object.
        public RuntimeMethodHandle MethodHandle 
            => Value.MethodHandle;
        //
        // Summary:
        //     Gets the System.Reflection.MethodImplAttributes flags that specify the attributes
        //     of a method implementation.
        //
        // Returns:
        //     The method implementation flags.
        public MethodImplAttributes MethodImplementationFlags 
            => Value.MethodImplementationFlags;
        //
        // Summary:
        //     Gets a value indicating whether this method has a special name.
        //
        // Returns:
        //     true if this method has a special name; otherwise, false.
        public bool IsSpecialName 
            => Value.IsSpecialName;
        //
        // Summary:
        //     Returns a value that indicates whether this instance is equal to a specified
        //     object.
        //
        // Parameters:
        //   obj:
        //     An object to compare with this instance, or null.
        //
        // Returns:
        //     true if obj equals the type and value of this instance; otherwise, false.
        public override bool Equals(object obj)
            => Value.Equals(obj);
        //
        // Summary:
        //     Returns an array of System.Type objects that represent the type arguments of
        //     a generic method or the type parameters of a generic method definition.
        //
        // Returns:
        //     An array of System.Type objects that represent the type arguments of a generic
        //     method or the type parameters of a generic method definition. Returns an empty
        //     array if the current method is not a generic method.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The current object is a System.Reflection.ConstructorInfo. Generic constructors
        //     are not supported in the .NET Framework version 2.0. This exception is the default
        //     behavior if this method is not overridden in a derived class.
        [ComVisible(true)]
        public TypeProxy[] GetGenericArguments()
            => Value.GetGenericArguments().Cast<TypeProxy>().ToArray();
        //
        // Summary:
        //     Returns the hash code for this instance.
        //
        // Returns:
        //     A 32-bit signed integer hash code.
        public override int GetHashCode()
            => Value.GetHashCode();

        //
        // Summary:
        //     When overridden in a derived class, gets a System.Reflection.MethodBody object
        //     that provides access to the MSIL stream, local variables, and exceptions for
        //     the current method.
        //
        // Returns:
        //     A System.Reflection.MethodBody object that provides access to the MSIL stream,
        //     local variables, and exceptions for the current method.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     This method is invalid unless overridden in a derived class.
        [SecuritySafeCritical]
        public MethodBodyProxy GetMethodBody()
            => Value.GetMethodBody();
        //
        // Summary:
        //     When overridden in a derived class, returns the System.Reflection.MethodImplAttributes
        //     flags.
        //
        // Returns:
        //     The MethodImplAttributes flags.
        public MethodImplAttributes GetMethodImplementationFlags()
            => Value.GetMethodImplementationFlags();
        //
        // Summary:
        //     When overridden in a derived class, gets the parameters of the specified method
        //     or constructor.
        //
        // Returns:
        //     An array of type ParameterInfo containing information that matches the signature
        //     of the method (or constructor) reflected by this MethodBase instance.
        public ParameterInfoProxy[] GetParameters()
            => Value.GetParameters().Select(x => ParameterInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Invokes the method or constructor represented by the current instance, using
        //     the specified parameters.
        //
        // Parameters:
        //   obj:
        //     The object on which to invoke the method or constructor. If a method is static,
        //     this argument is ignored. If a constructor is static, this argument must be null
        //     or an instance of the class that defines the constructor.
        //
        //   parameters:
        //     An argument list for the invoked method or constructor. This is an array of objects
        //     with the same number, order, and type as the parameters of the method or constructor
        //     to be invoked. If there are no parameters, parameters should be null.If the method
        //     or constructor represented by this instance takes a ref parameter (ByRef in Visual
        //     Basic), no special attribute is required for that parameter in order to invoke
        //     the method or constructor using this function. Any object in this array that
        //     is not explicitly initialized with a value will contain the default value for
        //     that object type. For reference-type elements, this value is null. For value-type
        //     elements, this value is 0, 0.0, or false, depending on the specific element type.
        //
        // Returns:
        //     An object containing the return value of the invoked method, or null in the case
        //     of a constructor.Elements of the parameters array that represent parameters declared
        //     with the ref or out keyword may also be modified.
        //
        // Exceptions:
        //   T:System.Reflection.TargetException:
        //     In the .NET for Windows Store apps or the Portable Class Library, catch System.Exception
        //     instead.The obj parameter is null and the method is not static.-or- The method
        //     is not declared or inherited by the class of obj. -or-A static constructor is
        //     invoked, and obj is neither null nor an instance of the class that declared the
        //     constructor.
        //
        //   T:System.ArgumentException:
        //     The elements of the parametersarray do not match the signature of the method
        //     or constructor reflected by this instance.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     The invoked method or constructor throws an exception. -or- The current instance
        //     is a System.Reflection.Emit.DynamicMethod that contains unverifiable code. See
        //     the "Verification" section in Remarks for System.Reflection.Emit.DynamicMethod.
        //
        //   T:System.Reflection.TargetParameterCountException:
        //     The parameters array does not have the correct number of arguments.
        //
        //   T:System.MethodAccessException:
        //     In the .NET for Windows Store apps or the Portable Class Library, catch the base
        //     class exception, System.MemberAccessException, instead.The caller does not have
        //     permission to execute the method or constructor that is represented by the current
        //     instance.
        //
        //   T:System.InvalidOperationException:
        //     The type that declares the method is an open generic type. That is, the System.Type.ContainsGenericParameters
        //     property returns true for the declaring type.
        //
        //   T:System.NotSupportedException:
        //     The current instance is a System.Reflection.Emit.MethodBuilder.
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object Invoke(object obj, object[] parameters)
            => Value.Invoke(obj, parameters);
        //
        // Summary:
        //     When overridden in a derived class, invokes the reflected method or constructor
        //     with the given parameters.
        //
        // Parameters:
        //   obj:
        //     The object on which to invoke the method or constructor. If a method is static,
        //     this argument is ignored. If a constructor is static, this argument must be null
        //     or an instance of the class that defines the constructor.
        //
        //   invokeAttr:
        //     A bitmask that is a combination of 0 or more bit flags from System.Reflection.BindingFlags.
        //     If binder is null, this parameter is assigned the value System.Reflection.BindingFlags.Default;
        //     thus, whatever you pass in is ignored.
        //
        //   binder:
        //     An object that enables the binding, coercion of argument types, invocation of
        //     members, and retrieval of MemberInfo objects via reflection. If binder is null,
        //     the default binder is used.
        //
        //   parameters:
        //     An argument list for the invoked method or constructor. This is an array of objects
        //     with the same number, order, and type as the parameters of the method or constructor
        //     to be invoked. If there are no parameters, this should be null.If the method
        //     or constructor represented by this instance takes a ByRef parameter, there is
        //     no special attribute required for that parameter in order to invoke the method
        //     or constructor using this function. Any object in this array that is not explicitly
        //     initialized with a value will contain the default value for that object type.
        //     For reference-type elements, this value is null. For value-type elements, this
        //     value is 0, 0.0, or false, depending on the specific element type.
        //
        //   culture:
        //     An instance of CultureInfo used to govern the coercion of types. If this is null,
        //     the CultureInfo for the current thread is used. (This is necessary to convert
        //     a String that represents 1000 to a Double value, for example, since 1000 is represented
        //     differently by different cultures.)
        //
        // Returns:
        //     An Object containing the return value of the invoked method, or null in the case
        //     of a constructor, or null if the method's return type is void. Before calling
        //     the method or constructor, Invoke checks to see if the user has access permission
        //     and verifies that the parameters are valid.Elements of the parameters array that
        //     represent parameters declared with the ref or out keyword may also be modified.
        //
        // Exceptions:
        //   T:System.Reflection.TargetException:
        //     The obj parameter is null and the method is not static.-or- The method is not
        //     declared or inherited by the class of obj. -or-A static constructor is invoked,
        //     and obj is neither null nor an instance of the class that declared the constructor.
        //
        //   T:System.ArgumentException:
        //     The type of the parameters parameter does not match the signature of the method
        //     or constructor reflected by this instance.
        //
        //   T:System.Reflection.TargetParameterCountException:
        //     The parameters array does not have the correct number of arguments.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     The invoked method or constructor throws an exception.
        //
        //   T:System.MethodAccessException:
        //     The caller does not have permission to execute the method or constructor that
        //     is represented by the current instance.
        //
        //   T:System.InvalidOperationException:
        //     The type that declares the method is an open generic type. That is, the System.Type.ContainsGenericParameters
        //     property returns true for the declaring type.
        public object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
            => Value.Invoke(obj, invokeAttr, binder, parameters, culture);

    }
}
