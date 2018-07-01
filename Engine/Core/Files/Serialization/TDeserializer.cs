using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Files;
using TheraEngine.Files.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    public class TDeserializer
    {
        private TFileReader _reader;
        private string _rootFilePath;
        private TFileObject _rootFileObject;

        private abstract class TFileReader
        {
            public virtual string Name { get; }
            public virtual string Value { get; }

            public abstract bool BeginElement();
            public abstract bool ReadAttribute();
            public abstract void EndElement();
            public abstract string ReadElementString();
            public abstract void MoveBackToElementClose();
            public abstract void ManualRead(TFileObject o);
        }
        private class TFileReaderXML : TFileReader
        {
            XMLReader Reader { get; set; } = new XMLReader();

            public override string Name => Reader.Name;
            public override string Value => Reader.Value;
            public override bool BeginElement() => Reader.BeginElement();
            public override bool ReadAttribute() => Reader.ReadAttribute();
            public override void EndElement() => Reader.EndElement();
            public override string ReadElementString() => Reader.ReadElementString();
            public override void MoveBackToElementClose() => Reader.MoveBackToElementClose();
            public override void ManualRead(TFileObject o) => o.Read(Reader);
        }
        private class TFileReaderBinary : TFileReader
        {
            public override string Name => null;//Reader.Name;
            public override string Value => null;//Reader.Value;
            public override bool BeginElement()
            {
                throw new NotImplementedException();
            }
            public override bool ReadAttribute()
            {
                throw new NotImplementedException();
            }
            public override void EndElement()
            {
                throw new NotImplementedException();
            }
            public override string ReadElementString()
            {
                throw new NotImplementedException();
            }
            public override void MoveBackToElementClose()
            {
                throw new NotImplementedException();
            }
            public override void ManualRead(TFileObject o)
            {
                throw new NotImplementedException();
            }
        }
        public TFileObject Deserialize(string filePath)
        {
            EFileFormat fmt = TFileObject.GetFormat(filePath, out string ext);
            switch (fmt)
            {
                default:
                case EFileFormat.ThirdParty:
                    //throw new InvalidOperationException("This type of file is not a proprietary file format.");
                    var task = TFileObject.Read3rdPartyAsync(null, filePath, null, CancellationToken.None);
                    task.Wait();
                    return task.Result;
                case EFileFormat.XML:
                    _reader = new TFileReaderXML();
                    break;
                case EFileFormat.Binary:
                    _reader = new TFileReaderBinary();
                    break;
            }
            return Deserialize2(filePath);
        }
        /// <summary>
        /// Reads a file from the stream as xml.
        /// </summary>
        public unsafe TFileObject Deserialize2(string filePath)
        {
            _rootFileObject = null;
            _rootFilePath = filePath;
            TFileObject obj = null;
            using (FileMap map = FileMap.FromFile(filePath))
            {
                if (_reader.BeginElement() &&
                    _reader.ReadAttribute() &&
                    _reader.Name.Equals(SerializationCommon.TypeIdent, StringComparison.InvariantCulture))
                {
                    string value = _reader.Value.ToString();
                    Type t = Type.GetType(value,
                        (name) =>
                        {
                            return AppDomain.CurrentDomain.GetAssemblies().
                            Where(z => z.FullName == name.FullName).FirstOrDefault();
                        },
                        null,
                        false);
                    //Type t = Type.GetType(_reader.Value.ToString(), false, false);
                    obj = ReadObject(t) as TFileObject;
                    _reader.EndElement();

                    if (obj is TFileObject o)
                        o.FilePath = filePath;
                }
            }
            return obj;
        }
        private object ReadObject(Type objType)
        {
            if (_reader.ReadAttribute())
            {
                if (string.Equals(_reader.Name.ToString(), SerializationCommon.TypeIdent, StringComparison.InvariantCultureIgnoreCase))
                {
                    string value = _reader.Value.ToString();
                    Type subType = Type.GetType(value,
                        (name) =>
                        {
                            return AppDomain.CurrentDomain.GetAssemblies().
                            Where(z => z.FullName == name.FullName).FirstOrDefault();
                        },
                        null,
                        false);

                    //if (subType != null)
                    //{
                    if (objType.IsAssignableFrom(subType))
                        objType = subType;
                    else
                        Engine.LogWarning(string.Format("Type mismatch: {0} and {1}", objType.GetFriendlyName(), subType.GetFriendlyName()));
                    //}
                }
            }

            //Collect the members of this object's type that are serialized
            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(objType);
            return ReadObject(objType, members);
        }
        private object ReadObject(Type objType, List<VarInfo> members)
        {
            //Create the object
            object obj = SerializationCommon.CreateObject(objType);
            if (obj is TFileObject tobj)
            {
                if (_rootFileObject == null)
                    _rootFileObject = tobj;
                else
                    tobj.Root = _rootFileObject;
                tobj.FilePath = _rootFilePath;
            }

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
                if (pre != null && pre.RunForFormats.HasFlag(ESerializeFormatFlag.XML))
                    preMethods.Add(m);

                PostDeserialize post = m.GetCustomAttribute<PostDeserialize>();
                if (post != null && post.RunForFormats.HasFlag(ESerializeFormatFlag.XML))
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
            ReadObjectElement(obj, members, categorized, customMethods);

            //Run post-deserialization methods
            foreach (MethodInfo m in postMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
                m.Invoke(obj, m.GetCustomAttribute<PostDeserialize>().Arguments);

            return obj;
        }
        //Separate method for handling category elements
        //This method interprets an element as a new class.
        private void ReadObjectElement(
            object obj,
            IEnumerable<VarInfo> members,
            IEnumerable<IGrouping<string, VarInfo>> categorized,
            IEnumerable<MethodInfo> customMethods)
        {
            List<VarInfo> attribs = new List<VarInfo>();
            List<VarInfo> elements = new List<VarInfo>();
            List<VarInfo> elementStrings = new List<VarInfo>();

            foreach (VarInfo info in members)
            {
                switch (info.Attrib.XmlNodeType)
                {
                    case EXmlNodeType.Attribute:
                        attribs.Add(info);
                        break;
                    case EXmlNodeType.ChildElement:
                        elements.Add(info);
                        break;
                    case EXmlNodeType.ElementString:
                        elementStrings.Add(info);
                        break;
                }
            }

            //Engine.PrintLine("Attribs: " + string.Join(", ", attribs));
            //Engine.PrintLine("Elems: " + string.Join(", ", elements));
            //Engine.PrintLine("Strings: " + string.Join(", ", elementStrings));

            //Read attributes first.
            //First attribute has already been read when checking for the assembly type name
            do
            {
                string attribName = _reader.Name.ToString();
                string attribValue = _reader.Value.ToString();

                //Look for matching attribute member with the same name
                int index = attribs.FindIndex(x => string.Equals(attribName, x.Name, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    VarInfo attrib = attribs[index];
                    attribs.RemoveAt(index);
                    ReadMember(obj, attrib, customMethods, true);
                }
            } while (_reader.ReadAttribute());

            //For all unwritten remaining attributes, set them to default (null for non-primitive types)
            //if (attribs.Count > 0)
            //{
            //    Engine.PrintLine("Unread attributes: " + string.Join(", ", attribs));
            //    //foreach (VarInfo attrib in attribs)
            //    //    attrib.SetValue(obj, attrib.VariableType.GetDefaultValue());
            //}

            //Now read elements
            bool allElementsNull = true;
            while (_reader.BeginElement())
            {
                allElementsNull = false;
                string elemName = _reader.Name.ToString();

                //Categorized key is the name of the category
                //So match element name to the key
                var categoryMembers = categorized?.Where(x => string.Equals(elemName, x.Key, StringComparison.InvariantCultureIgnoreCase))?.SelectMany(x => x).ToArray();
                if (categoryMembers != null && categoryMembers.Length > 0)
                    ReadObjectElement(obj, categoryMembers, null, customMethods);
                else
                {
                    VarInfo element = null;
                    int elementIndex = elements.FindIndex(elemMem => string.Equals(elemName, elemMem.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (elementIndex < 0)
                    {
                        elementIndex = elementStrings.FindIndex(elemMem => string.Equals(elemName, elemMem.Name, StringComparison.InvariantCultureIgnoreCase));
                        if (elementIndex >= 0)
                        {
                            element = elementStrings[elementIndex];
                            elementStrings.RemoveAt(elementIndex);
                        }
                    }
                    else
                    {
                        element = elements[elementIndex];
                        elements.RemoveAt(elementIndex);
                    }

                    if (element != null)
                        ReadMember(obj, element, customMethods, false);
                }
                _reader.EndElement();
            }

            //For all unwritten remaining elements, set them to default (null for non-primitive types)
            //foreach (VarInfo element in elements)
            //    element.SetValue(obj, element.VariableType.GetDefaultValue());

            if (allElementsNull && elementStrings.Count == 1)
            {
                //We've moved to the end element tag trying to find child elements, so move back to start
                _reader.MoveBackToElementClose();

                VarInfo elemStr = elementStrings[0];
                //Engine.PrintLine("Reading element string for {0}", elemStr.Name);
                if (SerializationCommon.CanParseAsString(elemStr.VariableType))
                {
                    MethodInfo customMethod = customMethods.FirstOrDefault(
                        x => string.Equals(elemStr.Name, x.GetCustomAttribute<CustomXMLDeserializeMethod>().Name));

                    if (customMethod != null)
                    {
                        //Engine.PrintLine("Invoking custom deserialization method for {0}: {1}", elemStr.Name, customMethod.GetFriendlyName());
                        customMethod.Invoke(obj, new object[] { _reader });
                    }
                    else
                    {
                        string s = _reader.ReadElementString();
                        elemStr.SetValue(obj, SerializationCommon.ParseString(s, elemStr.VariableType));
                    }
                }
                else
                    ReadMember(obj, elemStr, customMethods, false);
            }
        }
        private void ReadMember(object obj, VarInfo member, IEnumerable<MethodInfo> customMethods, bool isAttribute)
        {
            MethodInfo customMethod = customMethods.FirstOrDefault(
                x => string.Equals(member.Name, x.GetCustomAttribute<CustomXMLDeserializeMethod>().Name));

            //Engine.PrintLine("Reading {0} [{1}]", member.Name, member.VariableType.GetFriendlyName());

            if (customMethod != null)
            {
                //Engine.PrintLine("Invoking custom deserialization method for {0}: {1}", member.Name, customMethod.GetFriendlyName());
                customMethod.Invoke(obj, new object[] { _reader });
            }
            else
            {
                object value = isAttribute ?
                    SerializationCommon.ParseString(_reader.Value.ToString(), member.VariableType) :
                    ReadMemberElement(member.VariableType);
                member.SetValue(obj, value);
            }
        }
        private object ReadMemberElement(Type memberType)
        {
            switch (SerializationCommon.GetValueType(memberType))
            {
                case SerializationCommon.ValueType.Manual:
                    TFileObject o = (TFileObject)Activator.CreateInstance(memberType);
                    _reader.ManualRead(o);
                    return o;
                case SerializationCommon.ValueType.Array:
                    return ReadArray(memberType);
                case SerializationCommon.ValueType.Dictionary:
                    return ReadDictionary(memberType);
                case SerializationCommon.ValueType.Enum:
                case SerializationCommon.ValueType.String:
                case SerializationCommon.ValueType.Parsable:
                    return SerializationCommon.ParseString(_reader.ReadElementString(), memberType);
                case SerializationCommon.ValueType.Struct:
                    List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(memberType);
                    if (structFields.Count > 0)
                        return ReadObject(memberType, structFields);
                    else
                    {
                        //Enums and parsables have already been handled above
                        //This leaves only primitive types to check
                        if (SerializationCommon.IsPrimitiveType(memberType))
                            return SerializationCommon.ParseString(_reader.ReadElementString(), memberType);
                        else
                        {
                            //Custom struct with no members marked as serializable
                            //Serialize all members
                            var members = memberType.GetFields(
                                BindingFlags.NonPublic |
                                BindingFlags.Instance |
                                BindingFlags.Public |
                                BindingFlags.FlattenHierarchy);
                            return ReadStruct(memberType, members);
                        }
                    }
                case SerializationCommon.ValueType.Pointer:
                    return ReadObject(memberType);
            }
            return null;
        }

        private IDictionary ReadDictionary(Type dicType)
        {
            _reader.ReadAttribute();
            int num = int.Parse(_reader.Value.ToString());

            IDictionary dic = Activator.CreateInstance(dicType) as IDictionary;
            if (num > 0)
            {
                var args = dicType.GetGenericArguments();
                Type keyType = args[0];
                Type valueType = args[1];
                while (_reader.BeginElement())
                {
                    _reader.BeginElement();
                    object keyObj = ReadMemberElement(keyType);
                    _reader.EndElement();
                    _reader.BeginElement();
                    object valueObj = ReadMemberElement(valueType);
                    _reader.EndElement();
                    dic.Add(keyObj, valueObj);
                    _reader.EndElement();
                }
            }
            return dic;
        }

        private IList ReadArray(Type arrayType)
        {
            _reader.ReadAttribute();
            int num = int.Parse(_reader.Value.ToString());
            if (_reader.ReadAttribute())
            {
                //arrayType = Type.GetType(_reader.Value.ToString());
                string value = _reader.Value.ToString();
                arrayType = Type.GetType(value,
                    (name) =>
                    {
                        return AppDomain.CurrentDomain.GetAssemblies().
                        Where(z => z.FullName == name.FullName).FirstOrDefault();
                    },
                    null,
                    false);
            }

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
                        ReadArrayArray(list, num, elementType);
                        break;
                    case SerializationCommon.ValueType.Enum:
                    case SerializationCommon.ValueType.String:
                        ReadStringArray(list, num, elementType, false);
                        break;
                    case SerializationCommon.ValueType.Parsable:
                        ReadStringArray(list, num, elementType, true);
                        break;
                    case SerializationCommon.ValueType.Struct:
                        ReadStructArray(list, num, elementType);
                        break;
                    case SerializationCommon.ValueType.Pointer:
                        ReadObjectArray(list, num, elementType);
                        break;
                }
            }
            return list;
        }
        private void ReadArrayArray(IList list, int count, Type elementType)
        {
            for (int i = 0; i < count; ++i)
            {
                if (_reader.BeginElement())
                {
                    if (list.IsFixedSize)
                        list[i] = ReadArray(elementType);
                    else
                        list.Add(ReadArray(elementType));

                    _reader.EndElement();
                }
                else
                    throw new Exception();
            }
        }
        private void ReadObjectArray(IList list, int count, Type elementType)
        {
            for (int i = 0; i < count; ++i)
            {
                if (_reader.BeginElement())
                {
                    if (list.IsFixedSize)
                        list[i] = ReadObject(elementType);
                    else
                        list.Add(ReadObject(elementType));
                    _reader.EndElement();
                }
                //else
                //    throw new Exception();
            }
        }
        private void ReadStructArray(IList list, int count, Type elementType)
        {
            List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(elementType);
            //Struct has serialized members within it?
            //Needs a full element
            if (structFields.Count > 0)
                ReadObjectArray(list, count, elementType);
            else
            {
                if (SerializationCommon.IsPrimitiveType(elementType))
                    ReadStringArray(list, count, elementType, false);
                else
                {
                    FieldInfo[] structMembers = elementType.GetFields(
                        BindingFlags.NonPublic |
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.FlattenHierarchy);
                    for (int i = 0; i < count; ++i)
                    {
                        if (_reader.BeginElement())
                        {
                            object structValue = ReadStruct(elementType, structMembers);
                            if (list.IsFixedSize)
                                list[i] = structValue;
                            else
                                list.Add(structValue);
                            _reader.EndElement();
                        }
                        else
                            throw new Exception();
                    }
                }
            }
        }
        private object ReadStruct(Type t, FieldInfo[] members)
        {
            return null;
        }
        private void ReadStringArray(IList list, int count, Type elementType, bool parsable)
        {
            string fullstring = _reader.ReadElementString();
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
                            o = SerializationCommon.ParseString(value, elementType);
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

            o = SerializationCommon.ParseString(value, elementType);
            if (list.IsFixedSize)
                list[x++] = o;
            else
                list.Add(o);

            if (x != count)
                throw new Exception();
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
                        using (XMLReader reader = new XMLReader(map.Address, map.Length, true))
                        {
                            if (reader.BeginElement() &&
                                reader.ReadAttribute() &&
                                reader.Name.Equals(SerializationCommon.TypeIdent, true))
                            {
                                string value = reader.Value.ToString();
                                t = Type.GetType(value,
                                    (name) =>
                                    {
                                        return AppDomain.CurrentDomain.GetAssemblies().
                                        Where(z => z.FullName == name.FullName).FirstOrDefault();
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
