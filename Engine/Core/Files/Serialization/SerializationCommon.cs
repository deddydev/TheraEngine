using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Files.Serialization
{
    public class TSerializeMemberInfo
    {
        public event Action MemberTypeChanged;

        private Type _memberType;

        public MemberInfo Member { get; set; }
        public Type MemberType
        {
            get => _memberType;
            set
            {
                _memberType = value;
                MemberTypeChanged?.Invoke();
            }
        }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool State { get; set; }
        public bool Config { get; set; }
        public ENodeType NodeType { get; set; }
        public int Order { get; set; }
        public string Condition { get; set; }

        public TSerializeMemberInfo(Type memberType, string name, string category = null, bool state = true, bool config = true, ENodeType nodeType = ENodeType.ChildElement, int order = 0, string condition = null)
        {
            Member = null;
            MemberType = memberType;
            State = state;
            Config = config;
            NodeType = nodeType;
            Order = order;
            Condition = condition;
            
            Name = name ?? SerializationCommon.GetTypeName(MemberType) ?? "null";
            Name = new string(Name.Where(x => !char.IsWhiteSpace(x)).ToArray());

            Category = category;
            if (Category != null)
                Category = SerializationCommon.FixElementName(Category);
        }
        public TSerializeMemberInfo(MemberInfo member)
        {
            Member = member;
            MemberType =
                Member is PropertyInfo propMember ? propMember.PropertyType :
                Member is FieldInfo fieldMember ? fieldMember.FieldType :
                null;

            TSerialize attrib = member?.GetCustomAttribute<TSerialize>();
            Name = attrib?.NameOverride ?? Member?.Name ?? MemberType?.GetFriendlyName() ?? "Object";
            Name = new string(Name.Where(x => !char.IsWhiteSpace(x)).ToArray());

            if (attrib != null)
            {
                Config = attrib.Config;
                State = attrib.State;
                NodeType = attrib.NodeType;
                Order = attrib.Order;
                Condition = attrib.Condition;
            }

            if (attrib == null || !attrib.UseCategory)
                return;

            if (attrib.OverrideCategory != null)
                Category = SerializationCommon.FixElementName(attrib.OverrideCategory);
            else
            {
                CategoryAttribute categoryAttrib = Member?.GetCustomAttribute<CategoryAttribute>();
                if (categoryAttrib != null)
                    Category = SerializationCommon.FixElementName(categoryAttrib.Category);
            }
        }
        public bool AllowSerialize(object obj)
            => Condition == null || ExpressionParser.Evaluate<bool>(Condition, obj);

        public void SetObject(object parentObject, object memberObject)
        {
            if (Member == null)
                return;
            if (Member.MemberType.HasFlag(MemberTypes.Field))
                ((FieldInfo)Member).SetValue(parentObject, memberObject);
            else if (Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo p = (PropertyInfo)Member;
                if (p.CanWrite)
                    p.SetValue(parentObject, memberObject);
                else
                    Engine.LogWarning($"Can't set property {p.Name} in {p.DeclaringType.GetFriendlyName()}.");
            }
        }
        public object GetObject(object parentObject)
        {
            if (parentObject is null)
            {
                Engine.LogWarning($"{nameof(parentObject)} cannot be null.");
                return null;
            }
            if (Member is null)
            {
                Engine.LogWarning($"{nameof(Member)} cannot be null.");
                return null;
            }
            if (Member.MemberType.HasFlag(MemberTypes.Field))
            {
                FieldInfo info = (FieldInfo)Member;
                return info.GetValue(parentObject);
            }
            if (Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo info = (PropertyInfo)Member;
                if (info.CanRead)
                {
                    return info.GetValue(parentObject);
                }
                else
                {
                    Engine.LogWarning($"Can't read property {info.Name} in {info.DeclaringType.GetFriendlyName()}.");
                }
            }
            else
            {
                Engine.LogWarning($"Member {Name} is not a field or property.");
            }
            return null;
        }

        public override string ToString()
        {
            switch (NodeType)
            {
                default:
                case ENodeType.ChildElement:
                    return $"[Element] {Name}";
                case ENodeType.Attribute:
                    return $"[Attribute] {Name}";
                case ENodeType.ElementContent:
                    return $"[Content] {Name}";
            }
        }
    }
    public enum ESerializeType
    {
        Parsable,
        Enum,
        String,
        Struct,
        Class,
        Manual,
    }
    public static class SerializationCommon
    {
        public const string TypeIdent = "AssemblyType";

        internal static string GetTypeName(Type t)
        {
            if (t == null || t.IsInterface)
                return null;
            string name = t.Name;
            if (t.IsGenericType)
            {
                return t.Name.Remove(t.Name.IndexOf('`'))/* + "-"*/;
                //Type[] genericArgs = t.GenericTypeArguments;
                //bool first = true;
                //foreach (Type gt in genericArgs)
                //{
                //    if (first)
                //        first = false;
                //    else
                //        name += ", ";
                //    name += GetTypeName(gt);
                //}
                //name += "-";
            }
            return t.Name;
        }

        public static bool IsSerializableAsString(Type t)
            => t != null &&
            (t.GetInterface(nameof(ISerializableString)) != null ||
            IsPrimitiveType(t, true, true, true) ||
            IsEnum(t) ||
            t.IsValueType ||
            (t.GetInterface(nameof(IList)) != null && IsSerializableAsString(t.DetermineElementType())));

        public static Func<object, string> GetToStringFunc(Type t)
        {
            if (t.GetInterface(nameof(ISerializableString)) != null)
            {
                return new Func<object, string>(value => ((ISerializableString)value).WriteToString());
            }
            else if (IsEnum(t))
            {
                return new Func<object, string>(value => value.ToString().Replace(",", "|").ReplaceWhitespace(""));
            }
            else if (IsPrimitiveType(t, false, true, true))
            {
                return new Func<object, string>(value => value.ToString());
            }
            else if (t.IsValueType)
            {
                return new Func<object, string>(value => GetStructAsBytesString(value));
            }
            else if (t.GetInterface(nameof(IList)) != null)
            {
                Type elementType = t.DetermineElementType();
                if (IsSerializableAsString(elementType))
                {
                    string separator = " ";
                    if (!IsPrimitiveType(elementType))
                        separator = ",";

                    return new Func<object, string>(value =>
                    {
                        IList list = value as IList;
                        var func = GetToStringFunc(elementType);
                        return list.ToStringList(separator, separator, func);
                    });
                }
            }
            else if (t == typeof(Type))
            {
                return new Func<object, string>(value => ((Type)value).AssemblyQualifiedName);
            }
            return null;
        }
        public static bool GetString(object value, Type t, out string result)
        {
            if (t.GetInterface(nameof(ISerializableString)) != null)
            {
                result = ((ISerializableString)value).WriteToString();
                return true;
            }
            else if (IsEnum(t))
            {
                result = value.ToString().Replace(",", "|").ReplaceWhitespace("");
                return true;
            }
            else if (IsPrimitiveType(t, false, true, true))
            {
                result = value.ToString();
                return true;
            }
            else if (t.IsValueType)
            {
                result = GetStructAsBytesString(value);
                return true;
            }
            else if (t.GetInterface(nameof(IList)) != null)
            {
                Type elementType = t.DetermineElementType();
                if (IsSerializableAsString(elementType))
                {
                    string separator = " ";
                    if (!IsPrimitiveType(elementType))
                        separator = ",";

                    IList list = value as IList;
                    var func = GetToStringFunc(elementType);
                    result = /*t.AssemblyQualifiedName + " : " + */list.ToStringList(separator, separator, func);
                    return true;
                }
            }
            else if (t == typeof(Type))
            {
                result = ((Type)value).AssemblyQualifiedName;
                return true;
            }

            result = null;
            return false;
        }
        public static Func<string, object> GetFromStringFunc(Type t)
        {
            if (t.GetInterface(nameof(ISerializableString)) != null)
            {
                return new Func<string, object>(value =>
                {
                    ISerializableString o = (ISerializableString)Activator.CreateInstance(t);
                    o.ReadFromString(value);
                    return o;
                });
            }
            if (IsEnum(t))
            {
                return new Func<string, object>(value =>
                {
                    value = value.ReplaceWhitespace("").Replace("|", ", ");
                    return Enum.Parse(t, value);
                });
            }
            switch (t.Name)
            {
                case "Boolean": return new Func<string, object>(value => Boolean.Parse(value));
                case "SByte": return new Func<string, object>(value => SByte.Parse(value));
                case "Byte": return new Func<string, object>(value => Byte.Parse(value));
                case "Char": return new Func<string, object>(value => Char.Parse(value));
                case "Int16":return new Func<string, object>(value => Int16.Parse(value));
                case "UInt16": return new Func<string, object>(value => UInt16.Parse(value));
                case "Int32": return new Func<string, object>(value => Int32.Parse(value));
                case "UInt32": return new Func<string, object>(value => UInt32.Parse(value));
                case "Int64": return new Func<string, object>(value => Int64.Parse(value));
                case "UInt64": return new Func<string, object>(value => UInt64.Parse(value));
                case "Single": return new Func<string, object>(value => Single.Parse(value));
                case "Double": return new Func<string, object>(value => Double.Parse(value));
                case "Decimal": return new Func<string, object>(value => Decimal.Parse(value));
                case "String": return new Func<string, object>(value => value);
            }
            if (t.IsValueType)
            {
                return new Func<string, object>(value => ParseStructBytesString(t, value));
            }
            if (t.GetInterface(nameof(IList)) != null)
            {
                Type elementType = t.DetermineElementType();
                if (IsSerializableAsString(elementType))
                {
                    char separator = ' ';
                    if (!IsPrimitiveType(elementType))
                        separator = ',';

                    return new Func<string, object>(value =>
                    {
                        string[] values = value.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);

                        IList list;
                        if (t.IsArray)
                            list = Activator.CreateInstance(t, values.Length) as IList;
                        else
                            list = Activator.CreateInstance(t) as IList;

                        var func = GetFromStringFunc(elementType);
                        if (list.IsFixedSize)
                        {
                            for (int i = 0; i < values.Length; ++i)
                                list[i] = func(values[i]);
                        }
                        else
                        {
                            for (int i = 0; i < values.Length; ++i)
                                list.Add(func(values[i]));
                        }
                        return list;
                    });
                }
            }
            return null;
        }
        public static object ParseString(string value, Type t)
        {
            if (t.GetInterface(nameof(ISerializableString)) != null)
            {
                ISerializableString o = (ISerializableString)Activator.CreateInstance(t);
                o.ReadFromString(value);
                return o;
            }
            if (IsEnum(t))
            {
                value = value.ReplaceWhitespace("").Replace("|", ", ");
                return Enum.Parse(t, value);
            }
            switch (t.Name)
            {
                case "Boolean": return Boolean.Parse(value);
                case "SByte":   return SByte.Parse(value);
                case "Byte":    return Byte.Parse(value);
                case "Char":    return Char.Parse(value);
                case "Int16":   return Int16.Parse(value);
                case "UInt16":  return UInt16.Parse(value);
                case "Int32":   return Int32.Parse(value);
                case "UInt32":  return UInt32.Parse(value);
                case "Int64":   return Int64.Parse(value);
                case "UInt64":  return UInt64.Parse(value);
                case "Single":  return Single.Parse(value);
                case "Double":  return Double.Parse(value);
                case "Decimal": return Decimal.Parse(value);
                case "String":  return value;
            }
            if (t.IsValueType)
                return ParseStructBytesString(t, value);
            if (t.GetInterface(nameof(IList)) != null)
            {
                Type elementType = t.DetermineElementType();
                if (IsSerializableAsString(elementType))
                {
                    char separator = ' ';
                    if (!IsPrimitiveType(elementType))
                        separator = ',';
                    
                    string[] values = value.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                    
                    IList list;
                    if (t.IsArray)
                        list = Activator.CreateInstance(t, values.Length) as IList;
                    else
                        list = Activator.CreateInstance(t) as IList;

                    var func = GetFromStringFunc(elementType);
                    if (list.IsFixedSize)
                    {
                        for (int i = 0; i < values.Length; ++i)
                            list[i] = func(values[i]);
                    }
                    else
                    {
                        for (int i = 0; i < values.Length; ++i)
                            list.Add(func(values[i]));
                    }

                    return list;
                }
            }

            throw new InvalidOperationException(t.ToString() + " is not parsable");
        }
        public static string GetStructAsBytesString(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr.ToStringList(" ", " ", s => s.ToString("X2"));
            //foreach (FieldInfo member in structMembers)
            //{
            //    object value = member.GetValue(structObj);
            //}
        }
        public static object ParseStructBytesString(Type type, string structBytes)
        {
            string[] strBytes = structBytes.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] bytes = strBytes.Select(x => byte.Parse(x, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier)).ToArray();
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            object result = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
            handle.Free();
            return result;
        }
        /// <summary>
        /// Collects and returns all public and non public properties that the type and all derived types define as serialized.
        /// Also sorts by attribute first and by the defined order in the attribute.
        /// </summary>
        //public static List<VarInfo> CollectSerializedMembers(Type t)
        //{
        //    BindingFlags retrieveFlags =
        //        BindingFlags.Instance |
        //        BindingFlags.NonPublic | 
        //        BindingFlags.Public | 
        //        BindingFlags.FlattenHierarchy;

        //    var members = t.GetMembersExt(retrieveFlags);
        //    List<VarInfo> fields = members.
        //        Where(x => (x is FieldInfo || x is PropertyInfo) && Attribute.IsDefined(x, typeof(TSerialize))).
        //        Select(x => new VarInfo(x)).
        //        OrderBy(x => (int)x.Attrib.NodeType).ToList();

        //    int attribCount = 0, elementCount = 0, elementStringCount = 0;
        //    foreach (VarInfo info in fields)
        //    {
        //        switch (info.Attrib.NodeType)
        //        {
        //            case ENodeType.SetParentAttribute:
        //                ++attribCount;
        //                break;
        //            case ENodeType.ChildElement:
        //                ++elementCount;
        //                break;
        //            case ENodeType.SetParentElementString:
        //                ++elementStringCount;
        //                break;
        //        }
        //    }

        //    for (int i = 0; i < fields.Count; ++i)
        //    {
        //        VarInfo info = fields[i];
        //        TSerialize s = info.Attrib;
        //        if (s.Order >= 0)
        //        {
        //            int index = s.Order;

        //            if (i < attribCount)
        //                index = index.Clamp(0, attribCount - 1);
        //            else
        //                index = index.Clamp(0, elementCount - 1) + attribCount;

        //            if (index == i)
        //                continue;
        //            fields.RemoveAt(i--);
        //            if (index == fields.Count)
        //                fields.Add(info);
        //            else
        //                fields.Insert(index, info);
        //        }
        //    }
        //    return fields;
        //}
        public static TSerializeMemberInfo[] CollectSerializedMembers(Type type)
        {
            BindingFlags retrieveFlags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy;
            
            MemberInfo[] members = type?.GetMembers(retrieveFlags) ?? new MemberInfo[0];
            List<TSerializeMemberInfo> serMembers = new List<TSerializeMemberInfo>(members.Length);
            
            int elementStringCount = 0;
            int firstElementStringIndex = -1;

            foreach (MemberInfo info in members)
                if ((info is FieldInfo || info is PropertyInfo) && Attribute.IsDefined(info, typeof(TSerialize)))
                {
                    TSerializeMemberInfo serMem = new TSerializeMemberInfo(info);
                    serMembers.Add(serMem);
                    if (serMem.NodeType == ENodeType.ElementContent)
                    {
                        ++elementStringCount;
                        if (elementStringCount == 1)
                        {
                            firstElementStringIndex = serMembers.Count - 1;
                        }
                        else if (firstElementStringIndex >= 0)
                        {
                            serMembers[firstElementStringIndex].NodeType = ENodeType.ChildElement;
                            serMem.NodeType = ENodeType.ChildElement;
                            firstElementStringIndex = -1;
                        }
                        else
                        {
                            serMem.NodeType = ENodeType.ChildElement;
                        }
                    }
                }
            
            return serMembers.OrderBy(x => x.Order).ToArray();
        }
        /// <summary>
        /// Creates an object of the given type.
        /// </summary>
        public static object CreateObject(Type t)
        {
            object o = null;
            try
            {
                if (t == typeof(string))
                    o = string.Empty;
                else
                    o = Activator.CreateInstance(t);
            }
            catch (Exception ex)
            {
                Engine.PrintLine($"Problem constructing {t.GetFriendlyName()}.\n{ex.ToString()}");
                o = FormatterServices.GetUninitializedObject(t);
            }
            return o;
        }
        //public static ESerializeType GetSerializeType(Type t)
        //{
        //    if (t.IsSubclassOf(typeof(TFileObject)) && (TFileObject.GetFileExtension(t)?.ManualXmlConfigSerialize == true))
        //    {
        //        return ESerializeType.Manual;
        //    }
        //    else if (t.GetInterface(nameof(ISerializableString)) != null)
        //    {
        //        return ESerializeType.Parsable;
        //    }
        //    //else if (t.GetInterface(nameof(IList)) != null)
        //    //{
        //    //    return ESerializeType.Array;
        //    //}
        //    //else if (t.GetInterface(nameof(IDictionary)) != null)
        //    //{
        //    //    return ESerializeType.Dictionary;
        //    //}
        //    else if (t.IsEnum)
        //    {
        //        return ESerializeType.Enum;
        //    }
        //    else if (t == typeof(string))
        //    {
        //        return ESerializeType.String;
        //    }
        //    else if (t.IsValueType)
        //    {
        //        return ESerializeType.Struct;
        //    }
        //    else
        //    {
        //        return ESerializeType.Class;
        //    }
        //}
        public static bool IsEnum(Type t)
            => t?.BaseType != null && string.Equals(t.BaseType.Name, "Enum", StringComparison.InvariantCulture);
        public static bool IsPrimitiveType(Type t, bool includeEnums = true, bool includeStrings = true, bool includeBooleans = true)
        {
            if (includeEnums && IsEnum(t))
                return true;
            if (includeStrings && t == typeof(string))
                return true;
            if (includeBooleans && t == typeof(bool))
                return true;
            switch (t.Name)
            {
                case "SByte":
                case "Byte":
                case "Char":
                case "Int16":
                case "UInt16":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                case "Single":
                case "Double":
                case "Decimal":
                    return true;
                default:
                    return false;
            }
        }

        public static string FixElementName(string name)
        {
            //Element names:
            //- are case-sensitive
            //- must start with a letter or underscore
            //- cannot start with the letters xml(or XML, or Xml, etc)
            //- can contain letters, digits, hyphens, underscores, and periods
            //- cannot contain spaces

            string validStartChars = "_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string validChars = validStartChars + "-.1234567890";
            if (name == null)
                throw new Exception("Element name cannot be null.");
            name = name.Replace(" ", "");
            if (name.ToLowerInvariant().StartsWith("xml"))
                name = name.Substring(3);
            if (string.IsNullOrWhiteSpace(name))
                name = "NoName";
            else if (validStartChars.IndexOf(name[0]) < 0)
                name = "_" + name;
            for (int i = 0; i < name.Length; ++i)
                if (!validChars.Contains(name[i]))
                {
                    name = name.Substring(0, i) + name.Substring(i + 1);
                    --i;
                }

            return name;
        }

        /// <summary>
        /// Finds a file name that does not exist within the given directory.
        /// </summary>
        public static string ResolveFileName(string dir, string name, string ext)
        {
            if (!Directory.Exists(dir))
                return name;
            if (!ext.StartsWith("."))
                ext = "." + ext;
            if (string.IsNullOrWhiteSpace(name))
                name = "UnnamedFile";
            string[] files = Directory.GetFiles(dir);
            string number = "";
            int i = 0;
            while (files.Any(x => string.Equals(Path.GetFileName(x), name + number + ext)))
                number = (i++).ToString();
            return name + number + ext;
        }
        /// <summary>
        /// Finds a file name that does not exist within the given directory.
        /// </summary>
        public static string ResolveDirectoryName(string dir, string name)
        {
            if (!Directory.Exists(dir))
                return name;
            if (string.IsNullOrWhiteSpace(name))
                name = "UnnamedFolder";
            string[] dirs = Directory.GetDirectories(dir);
            string number = "";
            int i = 0;
            while (dirs.Any(x => string.Equals(Path.GetFileName(x), name + number)))
                number = (i++).ToString();
            return name + number;
        }
        public static Type CreateType(string typeDeclaration)
        {
            try
            {
                AssemblyQualifiedName asmQualName = new AssemblyQualifiedName(typeDeclaration);
                
                //Assembly asm = Assembly.Load(asmQualName.AssemblyName);
                //Version version = asm.GetName().Version;
                //asmQualName.VersionMinor = version.Minor;
                //asmQualName.VersionBuild = version.Build;
                //asmQualName.VersionRevision = version.Revision;

                //AssemblyName asmName = asmQualName.GetAssemblyName();

                //typeDeclaration = asmQualName.ToString();

                return Type.GetType(typeDeclaration,
                    name =>
                    {
                        var assemblies = //AppDomain.CurrentDomain.GetAssemblies();
                        Engine.EnumAppDomains().SelectMany(x => x.GetAssemblies());
                        return assemblies.FirstOrDefault(z => string.Equals(z.GetName().Name, asmQualName.AssemblyName, StringComparison.InvariantCultureIgnoreCase));
                    },
                    null,
                    true);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            return null;
        }
        public static Type DetermineType(string filePath) => DetermineType(filePath, out EFileFormat format);
        public static unsafe Type DetermineType(string filePath, out EFileFormat format)
        {
            format = TFileObject.GetFormat(filePath, out string ext);
            Type fileType = null;
            try
            {
                switch (format)
                {
                    default:
                    case EFileFormat.ThirdParty:

                        List<Type> types = TFileObject.DetermineThirdPartyTypes(ext).ToList();

                        if (types.Count > 1)
                            for (int i = 0; i < types.Count; ++i)
                            {
                                Type t1 = types[i];
                                for (int x = i + 1; x < types.Count; x++)
                                {
                                    Type t2 = types[x];
                                    if (t1.IsAssignableFrom(t2))
                                    {
                                        types.RemoveAt(i--);
                                        break;
                                    }
                                }
                            }

                        if (types.Count > 1)
                            Engine.PrintLine($"Multiple possible file types found for 3rd party extension '{ext}': {types.ToStringList(", ")}. Assuming the first.");
                        
                        fileType = types.Count > 0 ? types[0] : null;

                        break;
                    case EFileFormat.XML:
                        using (FileMap map = FileMap.FromFile(filePath, FileMapProtect.Read, 0, 0x100))
                        {
                            XMLReader reader = new XMLReader(map.Address, map.Length, true);
                            if (reader.BeginElement() && reader.ReadAttribute() && reader.Name.Equals(TypeIdent, true))
                                fileType = CreateType(reader.Value);
                        }
                        break;
                    case EFileFormat.Binary:
                        using (FileMap map = FileMap.FromFile(filePath, FileMapProtect.Read, 0, 0x100))
                        {
                            FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                            //hdr->Strings->
                            //if (reader.BeginElement() && reader.ReadAttribute() && reader.Name.Equals(SerializationCommon.TypeIdent, true))
                            //    t = Type.GetType(reader.Value, false, false);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Engine.PrintLine(e.ToString());
            }
            //if (fileType == null)
            //    Engine.LogWarning("Cannot determine the type of file at " + filePath + ".");
            return fileType;
        }
    }
}
