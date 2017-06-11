using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TheraEngine.Files.Serialization
{
    public static partial class CustomXmlSerializer
    {
        /// <summary>
        /// Reads a file from the stream as xml.
        /// </summary>
        public static unsafe object Deserialize(string filePath, Type t)
        {
            object obj;
            using (FileMap map = FileMap.FromFile(filePath))
            using (XMLReader reader = new XMLReader(map.Address, map.Length))
            {
                if (reader.BeginElement())
                {
                    obj = (FileObject)ReadObjectElement(t, reader);
                    reader.EndElement();
                }
                else
                    obj = SerializationCommon.CreateObject(t);
                if (obj is FileObject o)
                    o.FilePath = filePath;
            }
            return obj;
        }
        private static object ReadObjectElement(Type t, XMLReader reader)
        {
            //Collect the members of this object's type that are serialized
            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(t);

            //Create the object
            object obj = SerializationCommon.CreateObject(t);

            //Get pre and post deserialize methods
            MethodInfo[] methods = t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
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

            //Run pre-deserialize method
            foreach (MethodInfo m in preMethods.OrderBy(x => x.GetCustomAttribute<PreDeserialize>().Order))
                m.Invoke(obj, m.GetCustomAttribute<PreDeserialize>().Arguments);

            //Get members categorized together
            List<IGrouping<string, VarInfo>> categorized = members.
                Where(x => x.Category != null).
                GroupBy(x => x.Category).ToList();

            //Remove categorized members from original list
            foreach (var grouping in categorized)
                foreach (VarInfo p in grouping)
                    members.Remove(p);

            List<VarInfo> attribs = members.Where(x => x.Attrib.IsXmlAttribute).ToList();
            List<VarInfo> elements = members.Where(x => !x.Attrib.IsXmlAttribute).ToList();

            //Read attributes first
            while (reader.ReadAttribute())
            {
                //Look for matching attribute member with the same name
                VarInfo attrib = attribs.FirstOrDefault(x => reader.Name.Equals(x.Name, true));
                if (attrib != null)
                {
                    Type fieldType = attrib.VariableType;
                    object value = ParseString(reader.Value, fieldType);
                    attrib.SetValue(obj, value);
                    attribs.Remove(attrib);
                }
            }
            //Now read elements
            while (reader.BeginElement())
            {
                var category = categorized.Where(x => reader.Name.Equals(x.Key, true)).ToArray();
                if (category != null)
                {
                    while (reader.ReadAttribute())
                    {
                        VarInfo attrib = attribs.FirstOrDefault(x => reader.Name.Equals(x.Name, true));
                        if (attrib != null)
                        {
                            Type fieldType = attrib.VariableType;
                            object value = ParseString(reader.Value, fieldType);
                            attrib.SetValue(obj, value);
                            attribs.Remove(attrib);
                        }
                    }
                    while (reader.BeginElement())
                    {
                        reader.EndElement();
                    }
                }
                else
                {
                    VarInfo element = elements.FirstOrDefault(x => reader.Name.Equals(x.Name, true));
                    if (element != null)
                    {
                        Type fieldType = element.VariableType;

                    }
                }
                reader.EndElement();
            }

            foreach (MethodInfo m in postMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
                m.Invoke(obj, m.GetCustomAttribute<PostDeserialize>().Arguments);

            return obj;
        }
        private static void ReadAttribute(XMLReader reader)
        {

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
                case "String":
                    return value;
            }
            throw new Exception(t.ToString() + " is not parsable");
        }
    }
}
