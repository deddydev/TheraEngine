using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Files.Serialization
{
    public static partial class CustomXmlSerializer
    {
        public const string TypeIdent = "AssemblyType";
        public static unsafe Type DetermineType(string filePath)
        {
            Type t = null;
            try
            {
                using (FileMap map = FileMap.FromFile(filePath, FileMapProtect.Read, 0, 0x100))
                using (XMLReader reader = new XMLReader(map.Address, map.Length, true))
                {
                    if (reader.BeginElement() && reader.ReadAttribute() && reader.Name.Equals(TypeIdent, true))
                        t = Type.GetType(reader.Value, false, false);
                }
            }
            catch (Exception e)
            {
                Engine.PrintLine(e.ToString());
            }
            return t;
        }
        /// <summary>
        /// Reads a file from the stream as xml.
        /// </summary>
        public static unsafe object Deserialize(string filePath)
        {
            object obj = null;
            using (FileMap map = FileMap.FromFile(filePath))
            using (XMLReader reader = new XMLReader(map.Address, map.Length, true))
            {
                if (reader.BeginElement() && reader.ReadAttribute() && reader.Name.Equals(TypeIdent, true))
                {
                    Type t = Type.GetType(reader.Value.ToString(), false, false);
                    obj = ReadObject(t, reader);
                    reader.EndElement();

                    if (obj is FileObject o)
                        o.FilePath = filePath;
                }
            }
            return obj;
        }
        private static object ReadObject(Type objType, XMLReader reader)
        {
            //Collect the members of this object's type that are serialized
            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(objType);
            return ReadObject(objType, members, reader);
        }
        private static object ReadObject(Type objType, List<VarInfo> members, XMLReader reader)
        {
            if (reader.ReadAttribute())
            {
                if (string.Equals(reader.Name.ToString(), TypeIdent, StringComparison.InvariantCultureIgnoreCase))
                {
                    Type subType = Type.GetType(reader.Value.ToString(), false, false);
                    //if (subType != null)
                    //{
                        if (objType.IsAssignableFrom(subType))
                            objType = subType;
                        else
                            Engine.LogWarning(string.Format("Type mismatch: {0} and {1}", objType.GetFriendlyName(), subType.GetFriendlyName()));
                    //}
                }
            }

            //Create the object
            object obj = SerializationCommon.CreateObject(objType);

            #region Methods

            //Get custom deserialize methods
            var customMethods = objType.GetMethods(
                BindingFlags.NonPublic |
                BindingFlags.Instance | 
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy).
                Where(x => x.GetCustomAttribute<CustomXMLDeserializeMethod>() != null);

            //Get pre and post deserialize methods
            MethodInfo[] methods = objType.GetMethods(
                BindingFlags.NonPublic |
                BindingFlags.Instance | 
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy);

            List<MethodInfo> preMethods = new List<MethodInfo>();
            List<MethodInfo> postMethods = new List<MethodInfo>();
            foreach (MethodInfo m in methods)
            {
                PreDeserialize pre = m.GetCustomAttribute<PreDeserialize>();
                if (pre != null && pre.RunForFormats.HasFlag(SerializeFormatFlag.XML))
                    preMethods.Add(m);

                PostDeserialize post = m.GetCustomAttribute<PostDeserialize>();
                if (post != null && post.RunForFormats.HasFlag(SerializeFormatFlag.XML))
                    postMethods.Add(m);
            }

            #endregion

            #region Categories

            //Get members categorized together
            IEnumerable<IGrouping<string, VarInfo>> categorized = members.
                Where(x => x.Category != null).
                GroupBy(x => SerializationCommon.FixElementName(x.Category));
            //Remove categorized members from original list
            foreach (var grouping in categorized)
                foreach (VarInfo p in grouping)
                    members.Remove(p);

            #endregion
            
            //Run pre-deserialization methods
            foreach (MethodInfo m in preMethods.OrderBy(x => x.GetCustomAttribute<PreDeserialize>().Order))
                m.Invoke(obj, m.GetCustomAttribute<PreDeserialize>().Arguments);

            //Read the element for this object
            ReadObjectElement(obj, reader, members, categorized, customMethods);

            //Run post-deserialization methods
            foreach (MethodInfo m in postMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
                m.Invoke(obj, m.GetCustomAttribute<PostDeserialize>().Arguments);

            return obj;
        }
        //Separate method for handling category elements
        //This method interprets an element as a new class.
        private static void ReadObjectElement(
            object obj,
            XMLReader reader, 
            IEnumerable<VarInfo> members,
            IEnumerable<IGrouping<string, VarInfo>> categorized,
            IEnumerable<MethodInfo> customMethods)
        {
            List<VarInfo> attribs = members.Where(x => x.Attrib.IsXmlAttribute).ToList();
            List<VarInfo> elements = members.Where(x => !x.Attrib.IsXmlAttribute).ToList();

            //Read attributes first
            while (reader.ReadAttribute())
            {
                string attribName = reader.Name.ToString();
                string attribValue = reader.Value.ToString();
                
                //Look for matching attribute member with the same name
                int index = attribs.FindIndex(x => string.Equals(attribName, x.Name, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    VarInfo attrib = attribs[index];
                    attribs.RemoveAt(index);
                    ReadMember(obj, attrib, reader, customMethods, true);
                }
            }

            //For all unwritten remaining attributes, set them to default (null for non-primitive types)
            foreach (VarInfo attrib in attribs)
                attrib.SetValue(obj, attrib.VariableType.GetDefaultValue());

            //Now read elements
            while (reader.BeginElement())
            {
                string elemName = reader.Name.ToString();
                
                //Categorized key is the name of the category
                //So match element name to the key
                var categoryMembers = categorized?.Where(x => string.Equals(elemName, x.Key, StringComparison.InvariantCultureIgnoreCase))?.SelectMany(x => x).ToArray();
                if (categoryMembers != null && categoryMembers.Length > 0)
                    ReadObjectElement(obj, reader, categoryMembers, null, customMethods);
                else
                {
                    int elementIndex = elements.FindIndex(elemMem => string.Equals(elemName, elemMem.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (elementIndex >= 0)
                    {
                        VarInfo element = elements[elementIndex];
                        elements.RemoveAt(elementIndex);
                        ReadMember(obj, element, reader, customMethods, false);
                    }
                }
                reader.EndElement();
            }

            //For all unwritten remaining elements, set them to default (null for non-primitive types)
            foreach (VarInfo element in elements)
                element.SetValue(obj, element.VariableType.GetDefaultValue());
        }
        private static void ReadMember(object obj, VarInfo member, XMLReader reader, IEnumerable<MethodInfo> customMethods, bool isAttribute)
        {
            MethodInfo customMethod = customMethods.FirstOrDefault(
                x => string.Equals(member.Name, x.GetCustomAttribute<CustomXMLDeserializeMethod>().Name));

            if (customMethod != null)
                customMethod.Invoke(obj, new object[] { reader });
            else
            {
                object value = isAttribute ? 
                    ParseString(reader.Value.ToString(), member.VariableType) : 
                    ReadMemberElement(member.VariableType, reader);
                member.SetValue(obj, value);
            }
        }
        private static object ReadMemberElement(Type memberType, XMLReader reader)
        {
            switch (SerializationCommon.GetValueType(memberType))
            {
                case SerializationCommon.ValueType.Manual:
                    FileObject o = (FileObject)Activator.CreateInstance(memberType);
                    o.Read(reader);
                    return o;
                case SerializationCommon.ValueType.Array:
                    return ReadArray(memberType, reader);
                case SerializationCommon.ValueType.Dictionary:
                    return ReadDictionary(memberType, reader);
                case SerializationCommon.ValueType.Enum:
                case SerializationCommon.ValueType.String:
                case SerializationCommon.ValueType.Parsable:
                    return ParseString(reader.ReadElementString(), memberType);
                case SerializationCommon.ValueType.Struct:
                    List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(memberType);
                    if (structFields.Count > 0)
                        return ReadObject(memberType, structFields, reader);
                    else
                    {
                        //Enums and parsables have already been handled above
                        //This leaves only primitive types to check
                        if (SerializationCommon.IsPrimitiveType(memberType))
                            return ParseString(reader.ReadElementString(), memberType);
                        else
                        {
                            //Custom struct with no members marked as serializable
                            //Serialize all members
                            var members = memberType.GetFields(
                                BindingFlags.NonPublic |
                                BindingFlags.Instance |
                                BindingFlags.Public |
                                BindingFlags.FlattenHierarchy);
                            return ReadStruct(memberType, members, reader);
                        }
                    }
                case SerializationCommon.ValueType.Pointer:
                    return ReadObject(memberType, reader);
            }
            return null;
        }

        private static IDictionary ReadDictionary(Type dicType, XMLReader reader)
        {
            reader.ReadAttribute();
            int num = int.Parse(reader.Value.ToString());

            IDictionary dic = Activator.CreateInstance(dicType) as IDictionary;
            if (num > 0)
            {
                var args = dicType.GetGenericArguments();
                Type keyType = args[0];
                Type valueType = args[1];
                while (reader.BeginElement())
                {
                    reader.BeginElement();
                    object keyObj = ReadMemberElement(keyType, reader);
                    reader.EndElement();
                    reader.BeginElement();
                    object valueObj = ReadMemberElement(valueType, reader);
                    reader.EndElement();
                    dic.Add(keyObj, valueObj);
                    reader.EndElement();
                }
            }
            return dic;
        }

        private static IList ReadArray(Type arrayType, XMLReader reader)
        {
            reader.ReadAttribute();
            int num = int.Parse(reader.Value.ToString());
            if (reader.ReadAttribute())
                arrayType = Type.GetType(reader.Value.ToString());

            IList list;
            if (string.Equals(arrayType.BaseType.Name, "Array", StringComparison.InvariantCulture))
                list = Activator.CreateInstance(arrayType, num) as IList;
            else
                list = Activator.CreateInstance(arrayType) as IList;

            if (num > 0)
            {
                Type elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];
                SerializationCommon.ValueType type = SerializationCommon.GetValueType(elementType);
                switch (type)
                {
                    case SerializationCommon.ValueType.Array:
                        ReadArrayArray(list, num, elementType, reader);
                        break;
                    case SerializationCommon.ValueType.Enum:
                    case SerializationCommon.ValueType.String:
                        ReadStringArray(list, num, elementType, reader, false);
                        break;
                    case SerializationCommon.ValueType.Parsable:
                        ReadStringArray(list, num, elementType, reader, true);
                        break;
                    case SerializationCommon.ValueType.Struct:
                        ReadStructArray(list, num, elementType, reader);
                        break;
                    case SerializationCommon.ValueType.Pointer:
                        ReadObjectArray(list, num, elementType, reader);
                        break;
                }
            }
            return list;
        }
        private static void ReadArrayArray(IList list, int count, Type elementType, XMLReader reader)
        {
            for (int i = 0; i < count; ++i)
            {
                if (reader.BeginElement())
                {
                    if (list.IsFixedSize)
                        list[i] = ReadArray(elementType, reader);
                    else
                        list.Add(ReadArray(elementType, reader));

                    reader.EndElement();
                }
                else
                    throw new Exception();
            }
        }
        private static void ReadObjectArray(IList list, int count, Type elementType, XMLReader reader)
        {
            for (int i = 0; i < count; ++i)
            {
                if (reader.BeginElement())
                {
                    if (list.IsFixedSize)
                        list[i] = ReadObject(elementType, reader);
                    else
                        list.Add(ReadObject(elementType, reader));
                    reader.EndElement();
                }
                else
                    throw new Exception();
            }
        }
        private static void ReadStructArray(IList list, int count, Type elementType, XMLReader reader)
        {
            List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(elementType);
            //Struct has serialized members within it?
            //Needs a full element
            if (structFields.Count > 0)
                ReadObjectArray(list, count, elementType, reader);
            else
            {
                if (SerializationCommon.IsPrimitiveType(elementType))
                    ReadStringArray(list, count, elementType, reader, false);
                else
                {
                    FieldInfo[] structMembers = elementType.GetFields(
                        BindingFlags.NonPublic | 
                        BindingFlags.Instance | 
                        BindingFlags.Public | 
                        BindingFlags.FlattenHierarchy);
                    for (int i = 0; i < count; ++i)
                    {
                        if (reader.BeginElement())
                        {
                            object structValue = ReadStruct(elementType, structMembers, reader);
                            if (list.IsFixedSize)
                                list[i] = structValue;
                            else
                                list.Add(structValue);
                            reader.EndElement();
                        }
                        else
                            throw new Exception();
                    }
                }
            }
        }
        private static object ReadStruct(Type t, FieldInfo[] members, XMLReader reader)
        {
            return null;
        }
        private static void ReadStringArray(IList list, int count, Type elementType, XMLReader reader, bool parsable)
        {
            string fullstring = reader.ReadElementString();
            int x = 0;
            bool ignore = false;
            string value = "";
            object o;
            for (int i = 0; i < fullstring.Length; ++i)
            {
                char c = fullstring[i];
                if (ignore)
                {
                    ignore = false;
                    value += c;
                }
                else
                    switch (c)
                    {
                        case '\\':
                            ignore = true;
                            break;
                        case ' ':
                            o = ParseString(value, elementType);
                            if (list.IsFixedSize)
                                list[x++] = o;
                            else
                                list.Add(o);
                            value = "";
                            break;
                        default:
                            value += c;
                            break;
                    }
            }

            o = ParseString(value, elementType);
            if (list.IsFixedSize)
                list[x++] = o;
            else
                list.Add(o);

            if (x != count)
                throw new Exception();
        }
        private static object ParseString(string value, Type t)
        {
            if (t.GetInterface("IParsable") != null)
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
            throw new InvalidOperationException(t.ToString() + " is not parsable");
        }
    }
}
