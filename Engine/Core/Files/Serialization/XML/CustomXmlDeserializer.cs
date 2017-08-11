using System;
using System.Collections;
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

            //Run pre-deserialization methods
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

            //Get custom deserialize methods
            var customMethods = objType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(x => x.GetCustomAttribute<CustomXMLDeserializeMethod>() != null);

            //Read the element for this object
            ReadElement(obj, reader, members, categorized, customMethods);

            //Run post-deserialization methods
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

                Engine.DebugPrint("Attribute \"{0}\" = \"{1}\"", -1, attribName, attribValue);

                //Look for matching attribute member with the same name
                VarInfo attrib = attribs.FirstOrDefault(x => string.Equals(attribName, x.Name, StringComparison.InvariantCultureIgnoreCase));
                if (attrib != null)
                {
                    attribs.Remove(attrib);

                    MethodInfo customMethod = customMethods.FirstOrDefault(
                        x => string.Equals(attrib.Name, x.GetCustomAttribute<CustomXMLDeserializeMethod>().Name));

                    if (customMethod != null)
                        customMethod.Invoke(obj, new object[] { reader });
                    else
                        attrib.SetValue(obj, ParseString(attrib.VariableType, attribValue));
                }
            }

            //For all unwritten remaining attributes, set them to default (null for non-primitive types)
            foreach (VarInfo attrib in attribs)
                attrib.SetValue(obj, attrib.VariableType.GetDefaultValue());

            //Now read elements
            while (reader.BeginElement())
            {
                string elemName = reader.Name;

                Engine.DebugPrint("Reading element \"{0}\"", -1, elemName);

                var category = categorized?.Where(x => string.Equals(elemName, x.Key, StringComparison.InvariantCultureIgnoreCase)).SelectMany(x => x);
                if (category != null)
                    ReadElement(obj, reader, category.ToList(), null, customMethods);
                else
                {
                    VarInfo element = elements.FirstOrDefault(x => string.Equals(elemName, x.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (element != null)
                    {
                        elements.Remove(element);

                        MethodInfo customMethod = customMethods.FirstOrDefault(
                            x => string.Equals(element.Name, x.GetCustomAttribute<CustomXMLDeserializeMethod>().Name));

                        if (customMethod != null)
                            customMethod.Invoke(obj, new object[] { reader });
                        else
                        {
                            object value = null;
                            Type t = element.VariableType;
                            if (t.GetInterface("IList") != null)
                            {
                                if (reader.BeginElement())
                                {
                                    reader.ReadAttribute();
                                    int num = int.Parse(reader.Value);
                                    if (num > 0)
                                    {
                                        IList list = Activator.CreateInstance(t, num) as IList;
                                        Type elementType = t.GetElementType();
                                        if (elementType.IsEnum || elementType == typeof(string))
                                            ReadStringArray(list, reader);
                                        else if (elementType.IsValueType)
                                            ReadStructArray(list, reader);
                                        else
                                            for (int i = 0; i < num; ++i)
                                            {
                                                if (reader.BeginElement())
                                                {
                                                    list[i] = ReadObject(elementType, reader);
                                                    reader.EndElement();
                                                }
                                                else
                                                    throw new Exception();
                                            }
                                    }
                                    reader.EndElement();
                                }
                            }
                            else
                            {
                                //if (t.GetInterface("IParsable") != null)
                                //    writer.WriteElementString(info.Name, ((IParsable)value).WriteToString());
                                //else if (t.IsEnum || value is string)
                                //    writer.WriteElementString(info.Name, value.ToString());
                                //else if (t.IsValueType)
                                //{
                                //    List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(info.VariableType);
                                //    if (structFields.Count > 0)
                                //        WriteObject(value, structFields, info.Name, writer, false);
                                //    else
                                //        writer.WriteElementString(info.Name, value.ToString());
                                //    return;
                                //}
                                //else
                                    value = ReadObject(element.VariableType, reader);
                            }
                            element.SetValue(obj, value);
                        }
                    }
                }
                reader.EndElement();
            }

            //For all unwritten remaining elements, set them to default (null for non-primitive types)
            foreach (VarInfo element in elements)
                element.SetValue(obj, element.VariableType.GetDefaultValue());
        }
        private static void ReadStructArray(IList array, XMLReader reader)
        {
            //List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(array[0].GetType());
            ////Struct has serialized members within it?
            ////Needs a full element
            //if (structFields.Count > 0)
            //    foreach (object o in array)
            //        ReadObject(o, structFields, "Item", writer, false);
            //else
            //{
            //    //Write each struct as a string
            //    string output = array[0].ToString();
            //    for (int i = 1; i < array.Count; ++i)
            //        output += " " + array[i].ToString();
            //    writer.WriteString(output);
            //}
        }

        private static void ReadStringArray(IList array, XMLReader reader)
        {
            //string output = array[0].ToString();
            //if (output.Contains(" "))
            //    output = "\"" + output + "\"";
            //for (int i = 1; i < array.Count; ++i)
            //{
            //    string s = array[i].ToString();
            //    if (s.Contains(" "))
            //        s = "\"" + s + "\"";
            //    output += " " + s;
            //}
            //writer.WriteString(output);
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
