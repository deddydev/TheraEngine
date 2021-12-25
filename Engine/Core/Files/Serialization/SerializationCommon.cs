using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Tools;
using Extensions;
using TheraEngine.ComponentModel;

namespace TheraEngine.Core.Files.Serialization
{
    public class TSerializeMemberInfo : TObject
    {
        public event Action MemberTypeChanged;

        private TypeProxy _memberType;

        public MemberInfoProxy Member { get; set; }
        public TypeProxy MemberType
        {
            get => _memberType;
            set
            {
                _memberType = value;
                MemberTypeChanged?.Invoke();
            }
        }
        public string Category { get; set; }
        public bool State { get; set; }
        public bool Config { get; set; }
        public ENodeType NodeType { get; set; }
        public int Order { get; set; }
        public string Condition { get; set; }
        public bool DeserializeAsync { get; set; }
        public bool IsStreamable { get; set; }

        public TSerializeMemberInfo(TypeProxy memberType, string name, string category = null, bool state = true, bool config = true, ENodeType nodeType = ENodeType.ChildElement, int order = 0, string condition = null)
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
        public TSerializeMemberInfo(MemberInfoProxy member)
        {
            Member = member;
            MemberType =
                Member is PropertyInfoProxy propMember ? propMember.PropertyType :
                Member is FieldInfoProxy fieldMember ? fieldMember.FieldType :
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
                DeserializeAsync = attrib.DeserializeAsync;
                IsStreamable = attrib.IsStreamable;
            }

            if (attrib is null || !attrib.UseCategory)
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
            => Condition is null || ExpressionParser.Evaluate<bool>(Condition, obj);

        public void SetObject(object parentObject, object memberObject)
        {
            if (Member is null)
                return;
            if (Member.MemberType.HasFlag(MemberTypes.Field))
            {
                FieldInfoProxy f = (FieldInfoProxy)Member;
                try
                {
                    f.SetValue(parentObject, memberObject);
                }
                catch (Exception ex)
                {
                    Engine.LogException(ex);
                }
            }
            else if (Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfoProxy p = (PropertyInfoProxy)Member;
                if (p.CanWrite)
                {
                    try
                    {
                        p.SetValue(parentObject, memberObject);
                    }
                    catch (Exception ex)
                    {
                        Engine.LogException(ex);
                    }
                }
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
                FieldInfo info = (FieldInfoProxy)Member;
                return info.GetValue(parentObject);
            }
            if (Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo info = (PropertyInfoProxy)Member;
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

        internal static string GetTypeName(TypeProxy t)
        {
            if (t is null)
                return null;

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
                        return list.ToStringListGeneric(separator, separator, func);
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
                    result = /*t.AssemblyQualifiedName + " : " + */list.ToStringListGeneric(separator, separator, func);
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
                    ISerializableString o = (ISerializableString)CreateInstance(t);
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
                            list = CreateInstance(t, values.Length) as IList;
                        else
                            list = CreateInstance(t) as IList;

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
                        list = CreateInstance(t, values.Length) as IList;
                    else
                        list = CreateInstance(t) as IList;

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
        public static object ParseStructBytesString(TypeProxy type, string structBytes)
        {
            string[] strBytes = structBytes.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] bytes = strBytes.Select(x => byte.Parse(x, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier)).ToArray();
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            object result = type.PtrToStructure(handle.AddrOfPinnedObject());
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
        public static (int Count, IEnumerable<TSerializeMemberInfo> Values) CollectSerializedMembers(TypeProxy type)
        {
            BindingFlags retrieveFlags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy;

            MemberInfoProxy[] members = type?.GetMembers(retrieveFlags) ?? new MemberInfoProxy[0];
            ConcurrentDictionary<int, TSerializeMemberInfo> serMembers = new ConcurrentDictionary<int, TSerializeMemberInfo>();
            ConcurrentDictionary<int, TSerializeMemberInfo> contentMembers = new ConcurrentDictionary<int, TSerializeMemberInfo>();
            
            Parallel.For(0, members.Length, i =>
            {
                var info = members[i];
                if (!((info is FieldInfoProxy || info is PropertyInfoProxy) && info.IsAttributeDefined(typeof(TSerialize))))
                    return;
                
                TSerializeMemberInfo serMem = new TSerializeMemberInfo(info);
                serMembers.AddOrUpdate(i, serMem, (r, s) => serMem);

                if (serMem.NodeType == ENodeType.ElementContent)
                    contentMembers.AddOrUpdate(i, serMem, (r, s) => serMem);
            });

            if (contentMembers.Count > 1)
                foreach (var mem in contentMembers.Values)
                    mem.NodeType = ENodeType.ChildElement;
            
            var values = serMembers.OrderBy(x => x.Key).Select(x => x.Value).OrderBy(x => x.Order);
            return (serMembers.Values.Count, values);
        }
        /// <summary>
        /// Creates an object of the given type.
        /// </summary>
        public static object CreateInstance(Type t, params object[] args)
        {
            //Engine.PrintLine($"Creating type {t.GetFriendlyName()}");
            object obj = null;
            //if (t == typeof(string))
            //    obj = string.Empty;
            //else
            //{
                Assembly assembly = t.Assembly;
                //Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                var domain = AppDomain.CurrentDomain;
                //var domains = Engine.EnumAppDomains();
                //foreach (AppDomain domain in domains)
                //{
                //    if (domain == AppDomain.CurrentDomain)
                //    {
                        var assemblies = domain.GetAssemblies();
            if (assemblies.Contains(assembly))
            {
                try
                {
                    obj = Activator.CreateInstance(t, args);
                }
                catch (Exception ex)
                {
                    Engine.Out($"Problem constructing {t.GetFriendlyName()}.\n{ex.ToString()}");
                    obj = FormatterServices.GetUninitializedObject(t);
                }
            }
            return obj;
                //    }
                //    else
                //    {
                //        var assemblies = domain.GetAssemblies().Where(x => !currentAssemblies.Contains(x));
                //        if (!assemblies.Contains(assembly))
                //            continue;

                //        MarshalSponsor sponsor = new MarshalSponsor();
                //        MarshalByRefObject obj3 = domain.CreateInstanceAndUnwrap(t.Assembly.FullName, t.FullName) as MarshalByRefObject;
                //        var lease = obj3.InitializeLifetimeService() as ILease;
                //        lease.Register(sponsor);
                //        return obj3;
                //    }
                //}
             
            //}
            //return obj;
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
                case nameof(SByte):
                case nameof(Byte):
                case nameof(Char):
                case nameof(Int16):
                case nameof(UInt16):
                case nameof(Int32):
                case nameof(UInt32):
                case nameof(Int64):
                case nameof(UInt64):
                case nameof(Single):
                case nameof(Double):
                case nameof(Decimal):
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
            if (name is null)
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
        public static TypeProxy DetermineType(string filePath) => DetermineType(filePath, out EFileFormat format);
        public static unsafe TypeProxy DetermineType(string filePath, out EFileFormat format)
        {
            format = TFileObject.GetFormat(filePath, out string ext);
            TypeProxy fileType = null;
            try
            {
                switch (format)
                {
                    default:
                    case EFileFormat.ThirdParty:

                        List<Type> types = TFileObject.DetermineThirdPartyTypes(ext, false).ToList();

                        if (types.Count > 1)
                        {
                            //AppDomain primaryDomain = AppDomainHelper.GetPrimaryAppDomain();
                            types.Sort((x, y) =>
                            {
                                //if (x.Domain != y.Domain)
                                //    return x.Domain == primaryDomain ? -1 : 1;
                                if (!x.IsAssignableTo(y))
                                    return 1;
                                if (y.IsAssignableTo(x))
                                    return -1;
                                return 0;
                            });
                            for (int i = 0; i < types.Count; ++i)
                            {
                                Type thisOne = types[i];
                                for (int x = i + 1; x < types.Count; x++)
                                {
                                    Type thatOne = types[x];
                                    //if (thatOne.IsAssignableTo(thisOne))
                                    //{
                                    //    if (thatOne.Domain == thisOne.Domain || thatOne.Domain != primaryDomain)
                                    //        types.RemoveAt(x--);
                                    //}
                                    //else 
                                    if (thisOne.IsAssignableTo(thatOne))
                                    {
                                        //if (thisOne.Domain == thatOne.Domain || thisOne.Domain != primaryDomain)
                                            types.RemoveAt(i--);
                                        break;
                                    }
                                }
                            }
                        }

                        if (types.Count > 1)
                            Engine.Out($"Multiple possible file types found for 3rd party extension '{ext}': {types.ToStringList(", ")}. Assuming the first.");
                        
                        fileType = types.Count > 0 ? types[0] : null;

                        break;
                    case EFileFormat.XML:
                        using (FileMap map = FileMap.FromFile(filePath, FileMapProtect.Read, 0, 0x100))
                        {
                            XMLReader reader = new XMLReader(map.Address, map.Length, true);
                            if (reader.BeginElement() && reader.ReadAttribute() && reader.Name.Equals(TypeIdent, true))
                            {
                                var proxy = Engine.DomainProxy;
                                fileType = proxy?.GetTypeFor(reader.Value); //Type.GetType(reader.Value);
                            }
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
                Engine.Out(e.ToString());
            }
            //if (fileType is null)
            //    Engine.LogWarning("Cannot determine the type of file at " + filePath + ".");
            return fileType;
        }
    }
}
