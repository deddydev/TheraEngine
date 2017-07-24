using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TheraEngine.Files.Serialization
{
    public static partial class CustomXmlSerializer
    {
        public static unsafe Type DetermineType(string filePath)
        {
            Type t = null;
            using (FileMap map = FileMap.FromFile(filePath, FileMapProtect.Read, 0, 0x100))
            using (XMLReader reader = new XMLReader(map.Address, map.Length))
            {
                if (reader.BeginElement() && reader.ReadAttribute() && reader.Name.Equals("Type", true))
                    t = Type.GetType(reader.Value, false, false);
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
            using (XMLReader reader = new XMLReader(map.Address, map.Length))
            {
                if (reader.BeginElement() && reader.ReadAttribute() && reader.Name.Equals("Type", true))
                {
                    Type t = Type.GetType(reader.Value, false, false);
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

            //Create the object
            object obj = SerializationCommon.CreateObject(objType);

            //Get pre and post deserialize methods
            MethodInfo[] methods = objType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
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
            IEnumerable<IGrouping<string, VarInfo>> categorized = members.
                Where(x => x.Category != null).
                GroupBy(x => x.Category);

            //Remove categorized members from original list
            foreach (var grouping in categorized)
                foreach (VarInfo p in grouping)
                    members.Remove(p);

            var customMethods = objType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(x => x.GetCustomAttribute<CustomXMLDeserializeMethod>() != null);

            ReadElement(obj, reader, members, categorized, customMethods);

            foreach (MethodInfo m in postMethods.OrderBy(x => x.GetCustomAttribute<PostDeserialize>().Order))
                m.Invoke(obj, m.GetCustomAttribute<PostDeserialize>().Arguments);

            return obj;
        }
        private static void ReadElement(
            object obj,
            XMLReader reader, 
            List<VarInfo> members,
            IEnumerable<IGrouping<string, VarInfo>> categorized,
            IEnumerable<MethodInfo> customMethods)
        {
            List<VarInfo> attribs = members.Where(x => x.Attrib.IsXmlAttribute).ToList();
            List<VarInfo> elements = members.Where(x => !x.Attrib.IsXmlAttribute).ToList();

            //Read attributes first
            while (reader.ReadAttribute())
            {
                string attribName = reader.Name;
                string attribValue = reader.Value;

                Debug.WriteLine("Attribute \"{0}\" = \"{1}\"", attribName, attribValue);

                //Look for matching attribute member with the same name
                VarInfo attrib = attribs.FirstOrDefault(x => string.Equals(attribName, x.Name, StringComparison.InvariantCultureIgnoreCase));
                if (attrib != null)
                {
                    attribs.Remove(attrib);
                    attrib.SetValue(obj, ParseString(attrib.VariableType, attribValue));
                }
            }
            //Now read elements
            while (reader.BeginElement())
            {
                string elemName = reader.Name;

                Debug.WriteLine("Reading element \"{0}\"", elemName);

                var category = categorized?.Where(x => string.Equals(elemName, x.Key, StringComparison.InvariantCultureIgnoreCase)).SelectMany(x => x);
                if (category != null)
                    ReadElement(obj, reader, category.ToList(), null, customMethods);
                else
                {
                    VarInfo element = elements.FirstOrDefault(x => string.Equals(elemName, x.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (element != null)
                    {
                        elements.Remove(element);
                        element.SetValue(obj, ReadObject(element.VariableType, reader));
                    }
                }
                reader.EndElement();
            }
        }
        private static object ParseString(Type t, string value)
        {
            if (t.GetInterface("IParsable") != null)
            {
                IParsable o = (IParsable)Activator.CreateInstance(t);
                o.ReadFromString(value);
                return o;
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
            throw new Exception(t.ToString() + " is not parsable");
        }
    }
}
