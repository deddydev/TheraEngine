using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Files.Serialization
{
    public class TSerializeMemberInfo
    {
        public MemberInfo Member { get; set; }
        public Type MemberType { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool State { get; set; }
        public bool Config { get; set; }
        public ENodeType NodeType { get; set; }
        public int Order { get; set; }
        public string Condition { get; set; }

        public TSerializeMemberInfo(string name, string category, Type memberType, bool state, bool config, ENodeType nodeType, int order, string condition)
        {
            Member = null;
            MemberType = memberType;
            State = state;
            Config = config;
            NodeType = nodeType;
            Order = order;
            Condition = condition;

            Name = name ?? "null";
            Name = new string(Name.Where(x => !char.IsWhiteSpace(x)).ToArray());

            Category = category;
            if (Category != null)
                Category = SerializationCommon.FixElementName(Category);
        }
        public TSerializeMemberInfo(MemberInfo member)
        {
            Member = member;
            TSerialize attrib = member?.GetCustomAttribute<TSerialize>();
            Config = attrib.Config;
            State = attrib.State;
            NodeType = attrib.NodeType;
            Order = attrib.Order;
            Condition = attrib.Condition;

            MemberType =
                Member is PropertyInfo propMember ? propMember.PropertyType :
                Member is FieldInfo fieldMember ? fieldMember.FieldType :
                null;

            Name = attrib?.NameOverride ?? Member?.Name ?? "null";
            Name = new string(Name.Where(x => !char.IsWhiteSpace(x)).ToArray());

            if (attrib.UseCategory)
            {
                if (attrib.OverrideCategory != null)
                    Category = SerializationCommon.FixElementName(attrib.OverrideCategory);
                else
                {
                    CategoryAttribute categoryAttrib = Member.GetCustomAttribute<CategoryAttribute>();
                    if (categoryAttrib != null)
                        Category = SerializationCommon.FixElementName(categoryAttrib.Category);
                }
            }
        }
        public bool AllowSerialize(object obj)
            => Condition == null ? true : ExpressionParser.Evaluate<bool>(Condition, obj);
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
        public static bool CanParseAsString(Type t)
            => t.GetInterface(nameof(ISerializableString)) != null || IsPrimitiveType(t) || IsEnum(t);
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
        public static bool GetString(object value, Type t, out string result)
        {
            if (t.GetInterface(nameof(ISerializableString)) != null)
            {
                result = ((ISerializableString)value).WriteToString();
                return true;
            }
            else if (IsPrimitiveType(t))
            {
                result = value.ToString();
                return true;
            }
            else if (IsEnum(t))
            {
                result = value.ToString().Replace(",", "|").ReplaceWhitespace("");
                return true;
            }
            else if (t.IsValueType)
            {
                result = GetStructAsBytesString(value);
                return true;
            }
            result = null;
            return false;
        }
        public static object ParseString(string value, Type t)
        {
            if (t.GetInterface(nameof(ISerializableString)) != null)
            {
                ISerializableString o = (ISerializableString)Activator.CreateInstance(t);
                o.ReadFromString(value);
                return o;
            }
            if (string.Equals(t.BaseType.Name, "Enum", StringComparison.InvariantCulture))
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
        public static object ParseStructBytesString(Type t, string structBytes)
        {
            string[] strBytes = structBytes.Split(' ');
            byte[] bytes = strBytes.Select(x => byte.Parse(x, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier)).ToArray();
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            object result = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), t);
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
        public static List<TSerializeMemberInfo> CollectSerializedMembers(Type type)
        {
            BindingFlags retrieveFlags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy;

            MemberInfo[] members = type?.GetMembersExt(retrieveFlags) ?? new MemberInfo[0];
            List<TSerializeMemberInfo> serMembers = new List<TSerializeMemberInfo>(members.Length);

            foreach (MemberInfo info in members)
                if ((info is FieldInfo || info is PropertyInfo) && Attribute.IsDefined(info, typeof(TSerialize)))
                    serMembers.Add(new TSerializeMemberInfo(info));

            return serMembers;
        }
        /// <summary>
        /// Creates an object of the given type.
        /// </summary>
        public static object CreateObject(Type t)
        {
            object o = null;
            try
            {
                o = Activator.CreateInstance(t);
            }
            catch (Exception ex)
            {
                Engine.PrintLine($"Problem constructing {t.GetFriendlyName()}.\n{ex.ToString()}");
                o = FormatterServices.GetUninitializedObject(t);
            }
            finally
            {
                if (o is TObject tobj)
                    tobj.ConstructedProgrammatically = false;
            }
            return o;
        }
        public static ESerializeType GetSerializeType(Type t)
        {
            if (t.IsSubclassOf(typeof(TFileObject)) && (TFileObject.GetFileExtension(t)?.ManualXmlConfigSerialize == true))
            {
                return ESerializeType.Manual;
            }
            else if (t.GetInterface(nameof(ISerializableString)) != null)
            {
                return ESerializeType.Parsable;
            }
            //else if (t.GetInterface(nameof(IList)) != null)
            //{
            //    return ESerializeType.Array;
            //}
            //else if (t.GetInterface(nameof(IDictionary)) != null)
            //{
            //    return ESerializeType.Dictionary;
            //}
            else if (t.IsEnum)
            {
                return ESerializeType.Enum;
            }
            else if (t == typeof(string))
            {
                return ESerializeType.String;
            }
            else if (t.IsValueType)
            {
                return ESerializeType.Struct;
            }
            else
            {
                return ESerializeType.Class;
            }
        }
        public static bool IsEnum(Type t)
            => string.Equals(t.BaseType.Name, "Enum", StringComparison.InvariantCulture);
        public static bool IsPrimitiveType(Type t)
        {
            switch (t.Name)
            {
                case "Boolean":
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
                case "String":
                    return true;
            }
            return false;
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
        public static string ResolveFileName(string fileDir, string name, string ext)
        {
            if (!Directory.Exists(fileDir))
                return name;
            if (!ext.StartsWith("."))
                ext = "." + ext;
            if (string.IsNullOrWhiteSpace(name))
                name = "UnnamedFile";
            string[] files = Directory.GetFiles(fileDir);
            string number = "";
            int i = 0;
            while (files.Any(x => string.Equals(Path.GetFileName(x), name + number + ext)))
                number = (i++).ToString();
            return name + number;
        }

        public unsafe static Type DetermineType(string filePath)
        {
            EFileFormat fmt = TFileObject.GetFormat(filePath, out string ext);
            Type fileType = null;
            try
            {
                switch (fmt)
                {
                    default:
                    case EFileFormat.ThirdParty:
                        Type[] types = TFileObject.DetermineThirdPartyTypes(ext);
                        fileType = types.Length > 0 ? types[0] : null;
                        break;
                    case EFileFormat.XML:
                        using (FileMap map = FileMap.FromFile(filePath, FileMapProtect.Read, 0, 0x100))
                        {
                            XMLReader reader = new XMLReader(map.Address, map.Length, true);
                            if (reader.BeginElement() && reader.ReadAttribute() && reader.Name.Equals(TypeIdent, true))
                            {
                                string value = reader.Value.ToString();
                                fileType = Type.GetType(value,
                                    (name) =>
                                    {
                                        var assemblies =
                                        //AppDomain.CurrentDomain.GetAssemblies();
                                        Engine.EnumAppDomains().SelectMany(x => x.GetAssemblies());

                                        return assemblies.FirstOrDefault(z => z.FullName == name.FullName);
                                    },
                                    null,
                                    false);
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
                Engine.PrintLine(e.ToString());
            }
            if (fileType == null)
                Engine.LogWarning("Cannot determine the type of file at " + filePath + ".");
            return fileType;
        }

        /// <summary>
        /// Finds the class to use to read and write the given type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static BaseObjectSerializer DetermineObjectSerializer(Type objectType, IMemberTreeNode node)
        {
            BaseObjectSerializer serializer = null;
            Type[] types = null;

            if (objectType != null)
            {
                Type baseObjSerType = typeof(BaseObjectSerializer);
                types = Engine.FindTypes(type =>
                    baseObjSerType.IsAssignableFrom(type) &&
                    (type.GetCustomAttributeExt<ObjectWriterKind>()?.ObjectType?.IsAssignableFrom(objectType) ?? false),
                true, null).ToArray();
            }

            if (types != null && types.Length > 0)
                serializer = (BaseObjectSerializer)Activator.CreateInstance(types[0]);
            else
                serializer = new CommonSerializer();

            serializer.TreeNode = node;
            return serializer;
        }
    }
}
