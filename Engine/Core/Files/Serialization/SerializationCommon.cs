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
using TheraEngine.Core.Tools;
using TheraEngine.Core.Files;
using System.Threading.Tasks;
using System.Threading;

namespace TheraEngine.Core.Files.Serialization
{
    public enum InterfaceType
    {
        None,
        IList,
        IDictionary,
    }
    public class MemberTreeNode
    {
        public TSerializer.AbstractWriter Writer;
        public object Object;
        public VarInfo Info;

        public int CalculatedSize;
        public int FlagSize;

        public List<MemberTreeNode> Members;
        public List<IGrouping<string, MemberTreeNode>> CategorizedMembers;

        public InterfaceType InterfaceType;
        public MemberTreeNode[] IListMembers;
        public KeyValuePair<MemberTreeNode, MemberTreeNode>[] IDictionaryPairs;
        
        public MemberTreeNode(object root, TSerializer.AbstractWriter writer)
            : this(root, root == null ? null : new VarInfo(root.GetType(), null), writer) { }
        public MemberTreeNode(object obj, VarInfo info, TSerializer.AbstractWriter writer)
        {
            Object = obj;
            Info = info;
            Writer = writer;
            DetermineInterface();
        }
        private void DetermineInterface()
        {
            if (Object is IList array)
            {
                InterfaceType = InterfaceType.IList;
                IListMembers = new MemberTreeNode[array.Count];
                for (int i = 0; i < array.Count; ++i)
                    IListMembers[i] = new MemberTreeNode(array[i], Writer);
            }
            else if (Object is IDictionary dic)
            {
                InterfaceType = InterfaceType.IDictionary;
                IDictionaryPairs = new KeyValuePair<MemberTreeNode, MemberTreeNode>[dic.Count];

            }
            else
                InterfaceType = InterfaceType.None;
        }
        public async Task GenerateChildTree()
        {
            if (Info == null)
                return;

            await FileObjectCheck();

            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(Info.VariableType);

            Members = members.
                Where(x => (x.Attrib == null || x.Attrib.Condition == null) ? true : ExpressionParser.Evaluate<bool>(x.Attrib.Condition, obj)).
                Select(x => new MemberTreeNode(Object == null ? null : x.GetValue(Object), x, Writer)).
                ToList();

            CategorizedMembers = Members.Where(x => x.Info.Category != null).GroupBy(x => SerializationCommon.FixElementName(x.Info.Category)).ToList();
            foreach (var grouping in CategorizedMembers)
                foreach (MemberTreeNode p in grouping)
                    Members.Remove(p);

        }
        /// <summary>
        /// Performs special processing for classes that implement <see cref="IFileObject"/> and <see cref="IFileRef"/>.
        /// </summary>
        private async Task FileObjectCheck()
        {
            //Update the object's file path
            if (Object is IFileObject fobj)
            {
                fobj.FilePath = Writer.FilePath;
                if (fobj is IFileRef fref && !fref.StoredInternally)
                {
                    //Make some last minute adjustments to external file refs
                    //First, update file relative paths using the new file location
                    if (fref.PathType == EPathType.FileRelative)
                    {
                        string root = Path.GetPathRoot(fref.ReferencePathAbsolute);
                        int colonIndex = root.IndexOf(":");
                        if (colonIndex > 0)
                            root = root.Substring(0, colonIndex);
                        else
                            root = string.Empty;

                        string root2 = Path.GetPathRoot(Writer.FileDirectory);
                        colonIndex = root2.IndexOf(":");
                        if (colonIndex > 0)
                            root2 = root2.Substring(0, colonIndex);
                        else
                            root2 = string.Empty;

                        if (!string.Equals(root, root2))
                        {
                            //Totally different drives, cannot be relative in any way
                            fref.PathType = EPathType.Absolute;
                        }
                    }
                    if (fref.IsLoaded)
                    {
                        string path = fref.ReferencePathAbsolute;
                        bool fileExists =
                            !string.IsNullOrWhiteSpace(path) &&
                            path.IsExistingDirectoryPath() == false &&
                            File.Exists(path);

                        //TODO: export even if the file exists,
                        //however only if the file has changed
                        if (!fileExists)
                        {
                            if (fref is IGlobalFileRef && !Writer.Flags.HasFlag(ESerializeFlags.ExportGlobalRefs))
                                return;
                            if (fref is ILocalFileRef && !Writer.Flags.HasFlag(ESerializeFlags.ExportLocalRefs))
                                return;

                            string absPath;
                            if (fref.PathType == EPathType.FileRelative)
                            {
                                string rel = fref.ReferencePathAbsolute.MakePathRelativeTo(Writer.FileDirectory);
                                absPath = Path.GetFullPath(Path.Combine(Writer.FileDirectory, rel));
                                //fref.ReferencePathRelative = absPath.MakePathRelativeTo(_fileDir);
                            }
                            else
                                absPath = fref.ReferencePathAbsolute;

                            string dir = absPath.Contains(".") ? Path.GetDirectoryName(absPath) : absPath;

                            IFileObject file = fref.File;
                            if (file.FileExtension != null)
                            {
                                string fileName = SerializationCommon.ResolveFileName(
                                    Writer.FileDirectory, file.Name, file.FileExtension.GetProperExtension(EProprietaryFileFormat.XML));
                                await file.ExportAsync(dir, fileName, EFileFormat.XML, null, Writer.Flags, null, CancellationToken.None);
                            }
                            else
                            {
                                var f = file.File3rdPartyExtensions;
                                if (f != null && f.ExportableExtensions != null && f.ExportableExtensions.Length > 0)
                                {
                                    string ext = f.ExportableExtensions[0];
                                    string fileName = SerializationCommon.ResolveFileName(Writer.FileDirectory, file.Name, ext);
                                    await file.ExportAsync(dir, fileName, EFileFormat.ThirdParty, ext, Writer.Flags, null, CancellationToken.None);
                                }
                                else
                                    Engine.LogWarning("Cannot export " + file.GetType().GetFriendlyName());
                            }
                        }
                    }
                }
            }
        }

        public override string ToString() => Info.Name;
    }
    /// <summary>
    /// Stores a field/property's information.
    /// </summary>
    public class VarInfo
    {
        private MemberInfo _info;

        public Type OwningType { get; }
        public Type VariableType { get; }
        public string Name { get; }
        public string Category { get; } = null;
        public TSerialize Attrib { get; }

        public void SetValue(object obj, object value)
        {
            if (_info.MemberType.HasFlag(MemberTypes.Field))
                ((FieldInfo)_info).SetValue(obj, value);
            else if (_info.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo p = (PropertyInfo)_info;
                if (p.CanWrite)
                    p.SetValue(obj, value);
                else
                    Engine.LogWarning("Can't set property '" + p.Name + "' in " + p.DeclaringType.GetFriendlyName());
            }
        }
        public object GetValue(object obj)
        {
            if (obj is null)
                return null;
            if (_info.MemberType.HasFlag(MemberTypes.Field))
                return ((FieldInfo)_info).GetValue(obj);
            if (_info.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo p = (PropertyInfo)_info;
                if (p.CanRead)
                    return p.GetValue(obj);
                else
                    Engine.LogWarning("Can't read property '" + p.Name + "' in " + p.DeclaringType.GetFriendlyName());
            }
            return null;
        }
        public VarInfo(Type type, Type owningType, string name) : this(type, owningType)
        {
            Name = new string(name.Where(x => !char.IsWhiteSpace(x)).ToArray());
        }
        public VarInfo(Type type, Type owningType)
        {
            _info = null;
            Attrib = null;
            VariableType = type;
            OwningType = owningType;
            Name = null;
            Category = null;
        }
        public VarInfo(MemberInfo info)
        {
            _info = info;
            Attrib = _info.GetCustomAttribute<TSerialize>();
            if (_info.MemberType.HasFlag(MemberTypes.Field))
                VariableType = ((FieldInfo)_info).FieldType;
            else if (_info.MemberType.HasFlag(MemberTypes.Property))
                VariableType = ((PropertyInfo)_info).PropertyType;
            OwningType = _info.DeclaringType;
            if (Attrib.NameOverride != null)
                Name = Attrib.NameOverride;
            else
            {
                //Don't want to use display name, usually includes spaces or specialized formatting
                //DisplayNameAttribute nameAttrib = _info.GetCustomAttribute<DisplayNameAttribute>();
                //if (nameAttrib != null)
                //    Name = nameAttrib.DisplayName;
                //else
                    Name = _info.Name;
            }
            Name = new string(Name.Where(x => !char.IsWhiteSpace(x)).ToArray());
            if (Attrib.UseCategory)
            {
                if (Attrib.OverrideXmlCategory != null)
                    Category = Attrib.OverrideXmlCategory;
                else
                {
                    CategoryAttribute categoryAttrib = _info.GetCustomAttribute<CategoryAttribute>();
                    if (categoryAttrib != null)
                        Category = categoryAttrib.Category;
                }
            }
        }
        public override string ToString() => Name;
    }
    public static class SerializationCommon
    {
        public const string TypeIdent = "AssemblyType";
        public static bool CanParseAsString(Type t)
            => t.GetInterface(nameof(IParsable)) != null || IsPrimitiveType(t) || IsEnum(t);
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
            if (t.GetInterface(nameof(IParsable)) != null)
            {
                result = ((IParsable)value).WriteToString();
                return true;
            }
            else if (IsPrimitiveType(t))
            {
                result = value.ToString();
                return true;
            }
            else if (IsEnum(t))
            {
                result = value.ToString().Replace(", ", "|");
                return true;
            }
            else if (t.IsValueType)
            {
                result = GetStructAsBytesString(value);
                return true;
            }
            result = null;
            return false;
            //throw new InvalidOperationException(t.Name + " cannot be written as a string.");
        }
        public static object ParseString(string value, Type t)
        {
            if (t.GetInterface(nameof(IParsable)) != null)
            {
                IParsable o = (IParsable)Activator.CreateInstance(t);
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
                case "SByte": return SByte.Parse(value);
                case "Byte": return Byte.Parse(value);
                case "Char": return Char.Parse(value);
                case "Int16": return Int16.Parse(value);
                case "UInt16": return UInt16.Parse(value);
                case "Int32": return Int32.Parse(value);
                case "UInt32": return UInt32.Parse(value);
                case "Int64": return Int64.Parse(value);
                case "UInt64": return UInt64.Parse(value);
                case "Single": return Single.Parse(value);
                case "Double": return Double.Parse(value);
                case "Decimal": return Decimal.Parse(value);
                case "String": return value;
            }
            if (t.IsValueType)
            {
                return ParseStructBytesString(t, value);
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
        public static List<VarInfo> CollectSerializedMembers(Type t)
        {
            BindingFlags retrieveFlags =
                BindingFlags.Instance |
                BindingFlags.NonPublic | 
                BindingFlags.Public | 
                BindingFlags.FlattenHierarchy;

            var members = t.GetMembersExt(retrieveFlags);
            List<VarInfo> fields = members.
                Where(x => (x is FieldInfo || x is PropertyInfo) && Attribute.IsDefined(x, typeof(TSerialize))).
                Select(x => new VarInfo(x)).
                OrderBy(x => (int)x.Attrib.XmlNodeType).ToList();

            int attribCount = 0, elementCount = 0, elementStringCount = 0;
            foreach (VarInfo info in fields)
            {
                switch (info.Attrib.XmlNodeType)
                {
                    case EXmlNodeType.Attribute:
                        ++attribCount;
                        break;
                    case EXmlNodeType.ChildElement:
                        ++elementCount;
                        break;
                    case EXmlNodeType.ElementString:
                        ++elementStringCount;
                        break;
                }
            }

            for (int i = 0; i < fields.Count; ++i)
            {
                VarInfo info = fields[i];
                TSerialize s = info.Attrib;
                if (s.Order >= 0)
                {
                    int index = s.Order;

                    if (i < attribCount)
                        index = index.Clamp(0, attribCount - 1);
                    else
                        index = index.Clamp(0, elementCount - 1) + attribCount;

                    if (index == i)
                        continue;
                    fields.RemoveAt(i--);
                    if (index == fields.Count)
                        fields.Add(info);
                    else
                        fields.Insert(index, info);
                }
            }
            return fields;
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
        public enum ValueType
        {
            Array,
            Dictionary,
            Parsable,
            Enum,
            String,
            Struct,
            Pointer,
            Manual,
        }
        public static ValueType GetValueType(Type t)
        {
            if (t.IsSubclassOf(typeof(TFileObject)) && (TFileObject.GetFileExtension(t)?.ManualXmlConfigSerialize == true))
            {
                return ValueType.Manual;
            }
            else if (t.GetInterface("IParsable") != null)
            {
                return ValueType.Parsable;
            }
            else if (t.GetInterface("IList") != null)
            {
                return ValueType.Array;
            }
            else if (t.GetInterface("IDictionary") != null)
            {
                return ValueType.Dictionary;
            }
            else if (t.IsEnum)
            {
                return ValueType.Enum;
            }
            else if (t == typeof(string))
            {
                return ValueType.String;
            }
            else if (t.IsValueType)
            {
                return ValueType.Struct;
            }
            else
            {
                return ValueType.Pointer;
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

            name = name.Replace(" ", "");
            if (name.ToLowerInvariant().StartsWith("xml"))
                name = name.Substring(3);

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
            Type t = null;
            try
            {
                switch (fmt)
                {
                    default:
                    case EFileFormat.ThirdParty:
                        throw new InvalidOperationException("This type of file is not a proprietary file format.");
                    case EFileFormat.XML:
                        using (FileMap map = FileMap.FromFile(filePath, FileMapProtect.Read, 0, 0x100))
                        {
                            XMLReader reader = new XMLReader(map.Address, map.Length, true);
                            if (reader.BeginElement() && reader.ReadAttribute() && reader.Name.Equals(TypeIdent, true))
                            {
                                string value = reader.Value.ToString();
                                t = Type.GetType(value,
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
            return t;
        }
    }
}
