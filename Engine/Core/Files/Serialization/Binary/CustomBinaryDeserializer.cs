using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Files.Serialization
{
    public static partial class CustomBinarySerializer
    {
        public unsafe static Type DetermineType(string filePath)
        {
            Type t = null;
            try
            {
                using (FileMap map = FileMap.FromFile(filePath, FileMapProtect.Read, 0, 0x100))
                {
                    FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                    //if (reader.BeginElement() && reader.ReadAttribute() && reader.Name.Equals(SerializationCommon.TypeIdent, true))
                    //    t = Type.GetType(reader.Value, false, false);
                }
            }
            catch (Exception e)
            {
                Engine.PrintLine(e.ToString());
            }
            return t;
        }
        ///// <summary>
        ///// Reads a file from the stream as xml.
        ///// </summary>
        //public unsafe object Deserialize(string filePath)
        //{
        //    object obj = null;
        //    using (FileMap map = FileMap.FromFile(filePath))
        //    using (_reader = new XMLReader(map.Address, map.Length, true))
        //    {
        //        if (_reader.BeginElement() && _reader.ReadAttribute() && _reader.Name.Equals(SerializationCommon.TypeIdent, true))
        //        {
        //            Type t = Type.GetType(_reader.Value.ToString(), false, false);
        //            obj = ReadObject(t);
        //            _reader.EndElement();

        //            if (obj is TFileObject o)
        //                o.FilePath = filePath;
        //        }
        //    }
        //    return obj;
        //}
        //private object ReadObject(Type objType)
        //{
        //    if (_reader.ReadAttribute())
        //    {
        //        if (string.Equals(_reader.Name.ToString(), SerializationCommon.TypeIdent, StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            Type subType = Type.GetType(_reader.Value.ToString(), false, false);
        //            //if (subType != null)
        //            //{
        //            if (objType.IsAssignableFrom(subType))
        //                objType = subType;
        //            else
        //                Engine.LogWarning(string.Format("Type mismatch: {0} and {1}", objType.GetFriendlyName(), subType.GetFriendlyName()));
        //            //}
        //        }
        //    }

        //    //Collect the members of this object's type that are serialized
        //    List<VarInfo> members = SerializationCommon.CollectSerializedMembers(objType);
        //    return ReadObject(objType, members);
        //}
        //private object ReadObject(Type objType, List<VarInfo> members)
        //{
        //    //Create the object
        //    object obj = SerializationCommon.CreateObject(objType);

        //    #region Methods

        //    //Get custom deserialize methods
        //    var customMethods = objType.GetMethods(
        //        BindingFlags.NonPublic |
        //        BindingFlags.Instance |
        //        BindingFlags.Public |
        //        BindingFlags.FlattenHierarchy).
        //        Where(x => x.GetCustomAttribute<CustomXMLDeserializeMethod>() != null);

        //    //Get pre and post deserialize methods
        //    MethodInfo[] methods = objType.GetMethods(
        //        BindingFlags.NonPublic |
        //        BindingFlags.Instance |
        //        BindingFlags.Public |
        //        BindingFlags.FlattenHierarchy);

        //    List<MethodInfo> preMethods = new List<MethodInfo>();
        //    List<MethodInfo> postMethods = new List<MethodInfo>();
        //    foreach (MethodInfo m in methods)
        //    {
        //        PreDeserialize pre = m.GetCustomAttribute<PreDeserialize>();
        //        if (pre != null && pre.RunForFormats.HasFlag(SerializeFormatFlag.XML))
        //            preMethods.Add(m);

        //        PostDeserialize post = m.GetCustomAttribute<PostDeserialize>();
        //        if (post != null && post.RunForFormats.HasFlag(SerializeFormatFlag.XML))
        //            postMethods.Add(m);
        //    }

        //    #endregion

        //    #region Categories

        //    //Get members categorized together
        //    IEnumerable<IGrouping<string, VarInfo>> categorized = members.
        //        Where(x => x.Category != null).
        //        GroupBy(x => SerializationCommon.FixElementName(x.Category));
        //    //Remove categorized members from original list
        //    foreach (var grouping in categorized)
        //        foreach (VarInfo p in grouping)
        //            members.Remove(p);

        //    #endregion

        //    //Run pre-deserialization methods
        //    foreach (MethodInfo m in preMethods.OrderBy(x => x.GetCustomAttribute<PreDeserialize>().Order))
        //        m.Invoke(obj, m.GetCustomAttribute<PreDeserialize>().Arguments);

        //    //Read the element for this object
        //    ReadObjectElement(obj, members, categorized, customMethods);

        //    //Run post-deserialization methods
        //    foreach (MethodInfo m in postMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
        //        m.Invoke(obj, m.GetCustomAttribute<PostDeserialize>().Arguments);

        //    return obj;
        //}
        ////Separate method for handling category elements
        ////This method interprets an element as a new class.
        //private void ReadObjectElement(
        //    object obj,
        //    IEnumerable<VarInfo> members,
        //    IEnumerable<IGrouping<string, VarInfo>> categorized,
        //    IEnumerable<MethodInfo> customMethods)
        //{
        //    List<VarInfo> attribs = new List<VarInfo>();
        //    List<VarInfo> elements = new List<VarInfo>();
        //    List<VarInfo> elementStrings = new List<VarInfo>();

        //    foreach (VarInfo info in members)
        //    {
        //        switch (info.Attrib.XmlNodeType)
        //        {
        //            case EXmlNodeType.Attribute:
        //                attribs.Add(info);
        //                break;
        //            case EXmlNodeType.ChildElement:
        //                elements.Add(info);
        //                break;
        //            case EXmlNodeType.ElementString:
        //                elementStrings.Add(info);
        //                break;
        //        }
        //    }

        //    //Engine.PrintLine("Attribs: " + string.Join(", ", attribs));
        //    //Engine.PrintLine("Elems: " + string.Join(", ", elements));
        //    //Engine.PrintLine("Strings: " + string.Join(", ", elementStrings));

        //    //Read attributes first.
        //    //First attribute has already been read when checking for the assembly type name
        //    do
        //    {
        //        string attribName = _reader.Name.ToString();
        //        string attribValue = _reader.Value.ToString();

        //        //Look for matching attribute member with the same name
        //        int index = attribs.FindIndex(x => string.Equals(attribName, x.Name, StringComparison.InvariantCultureIgnoreCase));
        //        if (index >= 0)
        //        {
        //            VarInfo attrib = attribs[index];
        //            attribs.RemoveAt(index);
        //            ReadMember(obj, attrib, customMethods, true);
        //        }
        //    } while (_reader.ReadAttribute());

        //    //For all unwritten remaining attributes, set them to default (null for non-primitive types)
        //    //if (attribs.Count > 0)
        //    //{
        //    //    Engine.PrintLine("Unread attributes: " + string.Join(", ", attribs));
        //    //    //foreach (VarInfo attrib in attribs)
        //    //    //    attrib.SetValue(obj, attrib.VariableType.GetDefaultValue());
        //    //}

        //    //Now read elements
        //    bool allElementsNull = true;
        //    while (_reader.BeginElement())
        //    {
        //        allElementsNull = false;
        //        string elemName = _reader.Name.ToString();

        //        //Categorized key is the name of the category
        //        //So match element name to the key
        //        var categoryMembers = categorized?.Where(x => string.Equals(elemName, x.Key, StringComparison.InvariantCultureIgnoreCase))?.SelectMany(x => x).ToArray();
        //        if (categoryMembers != null && categoryMembers.Length > 0)
        //            ReadObjectElement(obj, categoryMembers, null, customMethods);
        //        else
        //        {
        //            VarInfo element = null;
        //            int elementIndex = elements.FindIndex(elemMem => string.Equals(elemName, elemMem.Name, StringComparison.InvariantCultureIgnoreCase));
        //            if (elementIndex < 0)
        //            {
        //                elementIndex = elementStrings.FindIndex(elemMem => string.Equals(elemName, elemMem.Name, StringComparison.InvariantCultureIgnoreCase));
        //                if (elementIndex >= 0)
        //                {
        //                    element = elementStrings[elementIndex];
        //                    elementStrings.RemoveAt(elementIndex);
        //                }
        //            }
        //            else
        //            {
        //                element = elements[elementIndex];
        //                elements.RemoveAt(elementIndex);
        //            }

        //            if (element != null)
        //                ReadMember(obj, element, customMethods, false);
        //        }
        //        _reader.EndElement();
        //    }

        //    //For all unwritten remaining elements, set them to default (null for non-primitive types)
        //    //foreach (VarInfo element in elements)
        //    //    element.SetValue(obj, element.VariableType.GetDefaultValue());

        //    if (allElementsNull && elementStrings.Count == 1)
        //    {
        //        //We've moved to the end element tag trying to find child elements, so move back to start
        //        _reader.MoveBackToElementClose();

        //        VarInfo elemStr = elementStrings[0];
        //        //Engine.PrintLine("Reading element string for {0}", elemStr.Name);
        //        if (SerializationCommon.CanParseAsString(elemStr.VariableType))
        //        {
        //            MethodInfo customMethod = customMethods.FirstOrDefault(
        //                x => string.Equals(elemStr.Name, x.GetCustomAttribute<CustomXMLDeserializeMethod>().Name));

        //            if (customMethod != null)
        //            {
        //                //Engine.PrintLine("Invoking custom deserialization method for {0}: {1}", elemStr.Name, customMethod.GetFriendlyName());
        //                customMethod.Invoke(obj, new object[] { _reader });
        //            }
        //            else
        //            {
        //                string s = _reader.ReadElementString();
        //                elemStr.SetValue(obj, SerializationCommon.ParseString(s, elemStr.VariableType));
        //            }
        //        }
        //        else
        //            ReadMember(obj, elemStr, customMethods, false);
        //    }
        //}
        //private void ReadMember(object obj, VarInfo member, IEnumerable<MethodInfo> customMethods, bool isAttribute)
        //{
        //    MethodInfo customMethod = customMethods.FirstOrDefault(
        //        x => string.Equals(member.Name, x.GetCustomAttribute<CustomXMLDeserializeMethod>().Name));

        //    //Engine.PrintLine("Reading {0} [{1}]", member.Name, member.VariableType.GetFriendlyName());

        //    if (customMethod != null)
        //    {
        //        //Engine.PrintLine("Invoking custom deserialization method for {0}: {1}", member.Name, customMethod.GetFriendlyName());
        //        customMethod.Invoke(obj, new object[] { _reader });
        //    }
        //    else
        //    {
        //        object value = isAttribute ?
        //            SerializationCommon.ParseString(_reader.Value.ToString(), member.VariableType) :
        //            ReadMemberElement(member.VariableType);
        //        member.SetValue(obj, value);
        //    }
        //}
        //private object ReadMemberElement(Type memberType)
        //{
        //    switch (SerializationCommon.GetValueType(memberType))
        //    {
        //        case SerializationCommon.ValueType.Manual:
        //            TFileObject o = (TFileObject)Activator.CreateInstance(memberType);
        //            o.Read(_reader);
        //            return o;
        //        case SerializationCommon.ValueType.Array:
        //            return ReadArray(memberType);
        //        case SerializationCommon.ValueType.Dictionary:
        //            return ReadDictionary(memberType);
        //        case SerializationCommon.ValueType.Enum:
        //        case SerializationCommon.ValueType.String:
        //        case SerializationCommon.ValueType.Parsable:
        //            return SerializationCommon.ParseString(_reader.ReadElementString(), memberType);
        //        case SerializationCommon.ValueType.Struct:
        //            List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(memberType);
        //            if (structFields.Count > 0)
        //                return ReadObject(memberType, structFields);
        //            else
        //            {
        //                //Enums and parsables have already been handled above
        //                //This leaves only primitive types to check
        //                if (SerializationCommon.IsPrimitiveType(memberType))
        //                    return SerializationCommon.ParseString(_reader.ReadElementString(), memberType);
        //                else
        //                {
        //                    //Custom struct with no members marked as serializable
        //                    //Serialize all members
        //                    var members = memberType.GetFields(
        //                        BindingFlags.NonPublic |
        //                        BindingFlags.Instance |
        //                        BindingFlags.Public |
        //                        BindingFlags.FlattenHierarchy);
        //                    return ReadStruct(memberType, members);
        //                }
        //            }
        //        case SerializationCommon.ValueType.Pointer:
        //            return ReadObject(memberType);
        //    }
        //    return null;
        //}

        //private IDictionary ReadDictionary(Type dicType)
        //{
        //    _reader.ReadAttribute();
        //    int num = int.Parse(_reader.Value.ToString());

        //    IDictionary dic = Activator.CreateInstance(dicType) as IDictionary;
        //    if (num > 0)
        //    {
        //        var args = dicType.GetGenericArguments();
        //        Type keyType = args[0];
        //        Type valueType = args[1];
        //        while (_reader.BeginElement())
        //        {
        //            _reader.BeginElement();
        //            object keyObj = ReadMemberElement(keyType);
        //            _reader.EndElement();
        //            _reader.BeginElement();
        //            object valueObj = ReadMemberElement(valueType);
        //            _reader.EndElement();
        //            dic.Add(keyObj, valueObj);
        //            _reader.EndElement();
        //        }
        //    }
        //    return dic;
        //}

        //private IList ReadArray(Type arrayType)
        //{
        //    _reader.ReadAttribute();
        //    int num = int.Parse(_reader.Value.ToString());
        //    if (_reader.ReadAttribute())
        //        arrayType = Type.GetType(_reader.Value.ToString());

        //    IList list;
        //    if (string.Equals(arrayType.BaseType.Name, "Array", StringComparison.InvariantCulture))
        //        list = Activator.CreateInstance(arrayType, num) as IList;
        //    else
        //        list = Activator.CreateInstance(arrayType) as IList;

        //    if (num > 0)
        //    {
        //        Type elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];
        //        SerializationCommon.ValueType type = SerializationCommon.GetValueType(elementType);
        //        switch (type)
        //        {
        //            case SerializationCommon.ValueType.Array:
        //                ReadArrayArray(list, num, elementType);
        //                break;
        //            case SerializationCommon.ValueType.Enum:
        //            case SerializationCommon.ValueType.String:
        //                ReadStringArray(list, num, elementType, false);
        //                break;
        //            case SerializationCommon.ValueType.Parsable:
        //                ReadStringArray(list, num, elementType, true);
        //                break;
        //            case SerializationCommon.ValueType.Struct:
        //                ReadStructArray(list, num, elementType);
        //                break;
        //            case SerializationCommon.ValueType.Pointer:
        //                ReadObjectArray(list, num, elementType);
        //                break;
        //        }
        //    }
        //    return list;
        //}
        //private void ReadArrayArray(IList list, int count, Type elementType)
        //{
        //    for (int i = 0; i < count; ++i)
        //    {
        //        if (_reader.BeginElement())
        //        {
        //            if (list.IsFixedSize)
        //                list[i] = ReadArray(elementType);
        //            else
        //                list.Add(ReadArray(elementType));

        //            _reader.EndElement();
        //        }
        //        else
        //            throw new Exception();
        //    }
        //}
        //private void ReadObjectArray(IList list, int count, Type elementType)
        //{
        //    for (int i = 0; i < count; ++i)
        //    {
        //        if (_reader.BeginElement())
        //        {
        //            if (list.IsFixedSize)
        //                list[i] = ReadObject(elementType);
        //            else
        //                list.Add(ReadObject(elementType));
        //            _reader.EndElement();
        //        }
        //        //else
        //        //    throw new Exception();
        //    }
        //}
        //private void ReadStructArray(IList list, int count, Type elementType)
        //{
        //    List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(elementType);
        //    //Struct has serialized members within it?
        //    //Needs a full element
        //    if (structFields.Count > 0)
        //        ReadObjectArray(list, count, elementType);
        //    else
        //    {
        //        if (SerializationCommon.IsPrimitiveType(elementType))
        //            ReadStringArray(list, count, elementType, false);
        //        else
        //        {
        //            FieldInfo[] structMembers = elementType.GetFields(
        //                BindingFlags.NonPublic |
        //                BindingFlags.Instance |
        //                BindingFlags.Public |
        //                BindingFlags.FlattenHierarchy);
        //            for (int i = 0; i < count; ++i)
        //            {
        //                if (_reader.BeginElement())
        //                {
        //                    object structValue = ReadStruct(elementType, structMembers);
        //                    if (list.IsFixedSize)
        //                        list[i] = structValue;
        //                    else
        //                        list.Add(structValue);
        //                    _reader.EndElement();
        //                }
        //                else
        //                    throw new Exception();
        //            }
        //        }
        //    }
        //}
        //private object ReadStruct(Type t, FieldInfo[] members)
        //{
        //    return null;
        //}
        //private void ReadStringArray(IList list, int count, Type elementType, bool parsable)
        //{
        //    string fullstring = _reader.ReadElementString();
        //    int x = 0;
        //    bool ignore = false;
        //    string value = "";
        //    object o;
        //    for (int i = 0; i < fullstring.Length; ++i)
        //    {
        //        char c = fullstring[i];
        //        if (ignore)
        //        {
        //            ignore = false;
        //            value += c;
        //        }
        //        else
        //            switch (c)
        //            {
        //                case '\\':
        //                    ignore = true;
        //                    break;
        //                case ' ':
        //                    o = SerializationCommon.ParseString(value, elementType);
        //                    if (list.IsFixedSize)
        //                        list[x++] = o;
        //                    else
        //                        list.Add(o);
        //                    value = "";
        //                    break;
        //                default:
        //                    value += c;
        //                    break;
        //            }
        //    }

        //    o = SerializationCommon.ParseString(value, elementType);
        //    if (list.IsFixedSize)
        //        list[x++] = o;
        //    else
        //        list.Add(o);

        //    if (x != count)
        //        throw new Exception();
        //}
        /// <summary>
        /// Reads a file from the stream as xml.
        /// </summary>
        public static unsafe object Deserialize(string filePath, Type t, string decryptPass = null)
        {
            object obj;
            using (FileMap map = FileMap.FromFile(filePath))
            {
                ushort parts = map.Address.UShort;
                bool encrypted = (parts & 0xFF00) != 0;
                bool compressed = (parts & 0x00FF) != 0;
                if (encrypted)
                {
                    //Use decryptPass to decrypt data
                }
                if (compressed)
                {
                    //Decompress to temp map before reading
                }
                FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                VoidPtr addr = hdr->Data;
                obj = (TFileObject)ReadObject(t, ref addr, hdr->Strings);
                if (obj is TFileObject o)
                    o.FilePath = filePath;
            }
            return obj;
        }
        private static object ReadObject(Type t, ref VoidPtr address, VoidPtr strings)
        {
            //Collect the members of this object's type that are serialized
            List<VarInfo> serializedMembers = SerializationCommon.CollectSerializedMembers(t);

            //Create the object
            object obj = SerializationCommon.CreateObject(t);
            //if (obj is TFileObject tobj)
            //    tobj.FilePath = _filePath;

            //Get pre and post deserialize methods
            MethodInfo[] methods = t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<MethodInfo> preMethods = new List<MethodInfo>();
            List<MethodInfo> postMethods = new List<MethodInfo>();
            foreach (MethodInfo m in methods)
            {
                PreDeserialize pre = m.GetCustomAttribute<PreDeserialize>();
                if (pre != null && pre.RunForFormats.HasFlag(ESerializeFormatFlag.Binary))
                    preMethods.Add(m);
                PostDeserialize post = m.GetCustomAttribute<PostDeserialize>();
                if (post != null && post.RunForFormats.HasFlag(ESerializeFormatFlag.Binary))
                    postMethods.Add(m);
            }

            //Run pre-deserialize method
            foreach (MethodInfo m in preMethods.OrderBy(x => x.GetCustomAttribute<PreDeserialize>().Order))
                m.Invoke(obj, m.GetCustomAttribute<PreDeserialize>().Arguments);

            //Get members categorized together
            List<IGrouping<string, VarInfo>> categorized = serializedMembers.
                Where(x => x.Category != null).
                GroupBy(x => SerializationCommon.FixElementName(x.Category)).ToList();

            //Remove categorized members from original list
            foreach (IGrouping<string, VarInfo> grouping in categorized)
                foreach (VarInfo p in grouping)
                    serializedMembers.Remove(p);

            //Read attributes first
            List<VarInfo> attribs = serializedMembers.Where(x => x.Attrib.IsXmlAttribute).ToList();
            //while (reader.ReadAttribute())
            //{
            //    //Look for matching attribute member with the same name
            //    VarInfo attrib = attribs.FirstOrDefault(x => reader.Name.Equals(x.Name, true));
            //    if (attrib != null)
            //    {
            //        Type fieldType = attrib.VariableType;
            //        object value = ParseString(reader.Value, fieldType);
            //        attrib.SetValue(obj, value);
            //        attribs.Remove(attrib);
            //    }
            //}

            //foreach (VarInfo attrib in attribs)
            //    attrib.SetValue(obj, attrib.Attrib.DefaultValue);

            //List<VarInfo> elements = serializedMembers.Where(x => !x.Attrib.IsXmlAttribute).ToList();
            //while (reader.BeginElement())
            //{
            //    var category = categorized.FirstOrDefault(x => reader.Name.Equals(x.Key, true));
            //    if (category != null)
            //    {
            //        while (reader.ReadAttribute())
            //        {
            //            VarInfo p = category.FirstOrDefault(x => x.Attrib.IsXmlAttribute && reader.Name.Equals(x.Name, true));
                        
            //        }
            //        while (reader.BeginElement())
            //        {

            //        }
            //    }
            //    else
            //    {
            //        VarInfo element = elements.FirstOrDefault(x => reader.Name.Equals(x.Name, true));
            //        if (element != null)
            //        {
            //            Type fieldType = element.VariableType;

            //        }
            //    }
            //    reader.EndElement();
            //}

            foreach (MethodInfo m in postMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
                m.Invoke(obj, m.GetCustomAttribute<PostDeserialize>().Arguments);

            return obj;
        }
        private static object ParseString(string value, Type t)
        {
            if (t.GetInterface("IParsable") != null)
            {
                IParsable o = (IParsable)Activator.CreateInstance(t);
                o.ReadFromString(value);
                return o;
            }
            switch (t.Name)
            {
                case "Boolean":
                    return Boolean.Parse(value);
                case "SByte":
                    return SByte.Parse(value);
                case "Byte":
                    return Byte.Parse(value);
                case "Char":
                    return Char.Parse(value);
                case "Int16":
                    return Int16.Parse(value);
                case "UInt16":
                    return UInt16.Parse(value);
                case "Int32":
                    return Int32.Parse(value);
                case "UInt32":
                    return UInt32.Parse(value);
                case "Int64":
                    return Int64.Parse(value);
                case "UInt64":
                    return UInt64.Parse(value);
                case "Single":
                    return Single.Parse(value);
                case "Double":
                    return Double.Parse(value);
                case "Decimal":
                    return Decimal.Parse(value);
            }
            throw new Exception(t.ToString() + " is not parsable");
        }
    }
}
