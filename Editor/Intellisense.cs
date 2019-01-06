using System.Collections.Generic;
using System.Reflection;
using TheraEditor.Windows.Forms;

namespace TheraEditor
{
    public sealed class Intellisense
    {
        public enum EDataType
        {
            Class,
            Struct,
            Interface,
            Enum,
            Method,
            Event,
            Delegate,
        }
        /// <summary>
        /// Contains information for where a type is declared.
        /// </summary>
        public class ScriptDeclareInfo
        {
            public string FileDeclaredInPath { get; set; }
            public int LineNumberStart { get; set; }
            public int LineNumberEnd { get; set; }
        }
        public class ScriptArgument
        {
            public string TypeName { get; set; }
            public string ArgumentName { get; set; }
            public bool In { get; }
            public bool Out { get; }
            public bool Ref { get; }

            public ScriptTypeInfo GetTypeInfo()
                => Editor.Instance.Project?.Intellisense?.GetTypeInfo(TypeName);
        }
        public class ScriptConstructionSpecialArgument
        {
            public string MemberName { get; set; }

        }
        public class ScriptAttributeInfo
        {
            public string TypeName { get; set; }
            public string[] ConstructionArguments { get; set; }
            public (string, string)[] PropertyArguments { get; set; }
            public (string, string)[] FieldArguments { get; set; }

            public ScriptTypeInfo GetTypeInfo()
                => Editor.Instance.Project?.Intellisense?.GetTypeInfo(TypeName);
        }
        public class ScriptNamespace
        {
            /// <summary>
            /// The local name of this namespace.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Full parent namespace.
            /// </summary>
            public string ParentNamespace { get; set; }
            /// <summary>
            /// Full child namespaces.
            /// </summary>
            public string ChildNamespaces { get; set; }
            public string[] TypeNames { get; set; }
            /// <summary>
            /// The assembly that contains this namespace.
            /// </summary>
            public AssemblyName AssemblyName { get; set; }

            public ScriptTypeInfo GetTypeNameInfo(int i)
                => Editor.Instance.Project?.Intellisense?.GetTypeInfo(TypeNames[i]);
            public string GetFullNamespacePath()
            {
                if (ParentNamespace == null)
                    return Name;
                else
                    return ParentNamespace + "." + Name;
            }
        }
        public class ScriptMemberInfo
        {
            public string MemberName { get; set; }
            public int PtrCount { get; set; } //*
            public int DerefPtrCount { get; set; } //&
        }
        public abstract class ScriptTypeInfo
        {
            public string TypeName { get; set; }
            public string OwningTypeName { get; set; }
            public string OwningNamespace { get; set; }
            public ScriptDeclareInfo DeclareInfo { get; set; }
            public ScriptAttributeInfo[] Attributes { get; set; }
            public ScriptModifiers Modifiers { get; set; }
            public ScriptMemberInfo MemberInfo { get; set; }

            public abstract EDataType Type { get; }
            public abstract bool CanResideInNamespace { get; }

            public ScriptTypeInfo GetTypeNameInfo()
                => Editor.Instance.Project?.Intellisense?.GetTypeInfo(TypeName);
            public ScriptTypeInfo GetOwningTypeNameInfo()
                => Editor.Instance.Project?.Intellisense?.GetTypeInfo(OwningTypeName);
            public ScriptNamespace GetOwningNamespaceInfo()
                => Editor.Instance.Project?.Intellisense?.GetNamespaceInfo(OwningNamespace);
        }
        public abstract class ScriptClassStructTypeInfo : ScriptTypeInfo
        {
            /// <summary>
            /// Members indexed by member name.
            /// </summary>
            public Dictionary<string, ScriptMemberInfo> Members { get; set; }
            public override bool CanResideInNamespace => true;
        }
        public class ScriptClassTypeInfo : ScriptClassStructTypeInfo
        {
            public override EDataType Type => EDataType.Class;
            public bool IsPartial { get; set; }
            public ScriptDeclareInfo[] PartialDeclareInfos { get; set; }
        }
        public class ScriptStructTypeInfo : ScriptClassStructTypeInfo
        {
            public override EDataType Type => EDataType.Struct;
        }
        public class ScriptEnumTypeInfo : ScriptTypeInfo
        {
            public override EDataType Type => EDataType.Enum;
            public override bool CanResideInNamespace => true;
        }
        public class ScriptInterfaceTypeInfo : ScriptTypeInfo
        {
            public override EDataType Type => EDataType.Interface;
            public override bool CanResideInNamespace => true;
        }
        public class ScriptDelegateTypeInfo : ScriptTypeInfo
        {
            public override EDataType Type => EDataType.Delegate;
            public override bool CanResideInNamespace => true;
        }
        public class ScriptEventTypeInfo : ScriptTypeInfo
        {
            public override EDataType Type => EDataType.Event;
            public override bool CanResideInNamespace => false;
        }
        public class ScriptMethodTypeInfo : ScriptTypeInfo
        {
            public override EDataType Type => EDataType.Method;
            public override bool CanResideInNamespace => false;
            public ScriptMethodBody Body { get; set; }
        }
        public class ScriptMethodLocalVar
        {

        }
        public class ScriptMethodBody
        {

        }
        public class ScriptModifiers
        {
            public bool Public { get; set; }
            public bool Private { get; set; }
            public bool Internal { get; set; }
            public bool Protected { get; set; }
            public bool Abstract { get; set; }
            public bool Virtual { get; set; }
            public bool Override { get; set; }
        }

        public ScriptNamespace GetNamespaceInfo(string ns)
            => Namespaces.ContainsKey(ns) ? Namespaces[ns] : null;
        public ScriptTypeInfo GetTypeInfo(string typeName)
            => Types.ContainsKey(typeName) ? Types[typeName] : null;

        private Dictionary<string, ScriptTypeInfo> Types { get; }
            = new Dictionary<string, ScriptTypeInfo>();
        private Dictionary<string, ScriptNamespace> Namespaces { get; }
            = new Dictionary<string, ScriptNamespace>();

        internal void AnalyzeLibraries()
        {

        }
    }
}
