using CustomEngine.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CustomEngine.Files.Serialization
{
    public static unsafe partial class CustomBinarySerializer
    {
        /// <summary>
        /// Writes the given object to the path as xml.
        /// </summary>
        public static void Serialize(object obj, string filePath)
        {
            Type t = obj.GetType();
            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(t);
            using (FileStream stream = new FileStream(filePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.ReadWrite,
                    8,
                    FileOptions.RandomAccess))
            {
                StringTable table = new StringTable();
                int dataSize = CalculateSize(obj, members, table).Align(4);
                int stringSize = table.GetTotalSize();
                int totalSize = dataSize + stringSize;
                stream.SetLength(totalSize);
                using (FileMap map = FileMap.FromStream(stream))
                {
                    FileCommonHeader* hdr = (FileCommonHeader*)map.Address;
                    table.WriteTable(hdr);
                    hdr->_fileLength = totalSize;
                    hdr->_stringTableLength = stringSize;
                    Write(obj, members, null, hdr->FileHeader, table);
                }
            }
        }
        private static int CalculateSize(object obj, List<VarInfo> members, StringTable table)
        {
            int size = 0;

            return size;
        }
        private static void Write(
            object obj,
            List<VarInfo> fields,
            string name,
            VoidPtr address,
            StringTable table)
        {
            if (obj == null)
                return;

            Type t = obj.GetType();

            var categorized = fields.
                Where(x => x.Category != null).
                GroupBy(x => x.Category).ToList();

            foreach (var grouping in categorized)
                foreach (VarInfo p in grouping)
                    fields.Remove(p);

            //Write start tag for this object
            if (string.IsNullOrEmpty(name))
                name = SerializationCommon.GetTypeName(t);

            MethodInfo[] customMethods = t.GetMethods(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(x => x.GetCustomAttribute<CustomSerializeMethod>() != null).ToArray();

            writer.WriteStartElement(name);
            {
                //Write attributes and then elements
                foreach (VarInfo p in fields)
                {
                    MethodInfo customMethod = customMethods.FirstOrDefault(x => string.Equals(p.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));
                    if (customMethod != null)
                        customMethod.Invoke(obj, new object[] { writer });
                    else
                        WriteField(obj, p, writer);
                }
                //Write categorized elements
                foreach (var grouping in categorized)
                {
                    //Write category element
                    writer.WriteStartElement(grouping.Key);
                    {
                        //Write fields on this element
                        foreach (VarInfo p in grouping.OrderBy(x => !x.Attrib.IsXmlAttribute))
                            WriteField(obj, p, writer);
                    }
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        private static void WriteField(object obj, VarInfo info, XmlWriter writer)
        {
            Type t = info.VariableType;
            if (info.Attrib.IsXmlAttribute)
            {
                object value = info.GetValue(obj);
                if (t.Name == "String" && value == null)
                {
                    //writer.WriteAttributeString(p._name, "");
                }
                else
                    writer.WriteAttributeString(info.Name, info.GetValue(obj).ToString());
            }
            else
            {
                if (info.Attrib.SerializeIf != null && 
                    !ExpressionParser.Evaluate<bool>(info.Attrib.SerializeIf, obj))
                    return;

                if (t.GetInterface("IList") != null && 
                    info.GetValue(obj) is IList array)
                {
                    writer.WriteStartElement(info.Name);
                    writer.WriteAttributeString("Count", array.Count.ToString());
                    if (array.Count > 0)
                    {
                        Type elementType = array[0].GetType();
                        if (elementType.IsEnum || array[0] is string)
                        {
                            string output = array[0].ToString();
                            if (output.Contains(" "))
                                output = "\"" + output + "\"";
                            for (int i = 1; i < array.Count; ++i)
                            {
                                string s = array[i].ToString();
                                if (s.Contains(" "))
                                    s = "\"" + s + "\"";
                                output += " " + s;
                            }
                            writer.WriteString(output);
                        }
                        else if (elementType.IsValueType)
                        {
                            List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(array[0].GetType());
                            //Struct has serialized members within it?
                            //Needs a full element
                            if (structFields.Count > 0)
                                foreach (object o in array)
                                    Write(o, structFields, "Item", writer);
                            else
                            {
                                //Write each struct as a string
                                string output = array[0].ToString();
                                for (int i = 1; i < array.Count; ++i)
                                    output += " " + array[i].ToString();
                                writer.WriteString(output);
                            }
                        }
                        else
                        {
                            foreach (object o in array)
                                Write(o, "Item", writer);
                        }
                    }
                    writer.WriteEndElement();
                    return;
                }

                object value = info.GetValue(obj);
                if (value == null)
                    return;
                if (value is IParsable)
                    writer.WriteElementString(info.Name, ((IParsable)value).WriteToString());
                else if (t.IsEnum || value is string)
                    writer.WriteElementString(info.Name, value.ToString());
                else if (t.IsValueType)
                {
                    List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(info.VariableType);
                    if (structFields.Count > 0)
                        Write(value, structFields, info.Name, writer);
                    else
                        writer.WriteElementString(info.Name, value.ToString());
                    return;
                }
                else
                    Write(value, info.Name, writer);
            }
        }
    }
}
