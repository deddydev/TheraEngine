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

namespace CustomEngine.Files
{
    public static partial class CustomXmlSerializer
    {
        /// <summary>
        /// Writes the given object to the path as xml.
        /// </summary>
        public static void Serialize(FileObject obj, string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan))
            using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
            {
                writer.Flush();
                stream.Position = 0;
                writer.WriteStartDocument();
                WriteObjectElement(obj, null, writer);
                writer.WriteEndDocument();
            }
        }
        private static readonly XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };
        private static void WriteObjectElement(object obj, string name, XmlWriter writer)
        {
            if (obj == null)
                return;

            Type t = obj.GetType();
            List<VarInfo> fields = CollectSerializedMembers(t);
            WriteObjectElement(obj, fields, name, writer);
        }
        private static void WriteObjectElement(object obj, List<VarInfo> fields, string name, XmlWriter writer)
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
                name = GetTypeName(t);

            MethodInfo[] customMethods = t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
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
                if (info.Attrib.SerializeIf != null && !BooleanExpressionParser.Evaluate(info.Attrib.SerializeIf, obj))
                    return;

                if (t.GetInterface("IList") != null && info.GetValue(obj) is IList array)
                {
                    writer.WriteStartElement(info.Name);
                    writer.WriteAttributeString("Count", array.Count.ToString());
                    if (array.Count > 0)
                    {
                        Type elementType = array[0].GetType();
                        if (elementType.IsEnum)
                        {
                            foreach (object o in array)
                                writer.WriteElementString("Item", o.ToString());
                        }
                        else if (array[0] is string)
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
                            List<VarInfo> structFields = CollectSerializedMembers(array[0].GetType());
                            if (structFields.Count > 0)
                                foreach (object o in array)
                                    WriteObjectElement(o, structFields, "Item", writer);
                            else
                            {
                                string output = array[0].ToString();
                                for (int i = 1; i < array.Count; ++i)
                                    output += " " + array[i].ToString();
                                writer.WriteString(output);
                            }
                        }
                        Top1:
                        switch (GetTypeName(elementType))
                        {
                            //Struct
                            case "ValueType":
                                List<VarInfo> structFields = CollectSerializedMembers(array[0].GetType());
                                if (structFields.Count > 0)
                                    foreach (object o in array)
                                        WriteObjectElement(o, structFields, "Item", writer);
                                else
                                    foreach (object o in array)
                                        writer.WriteElementString("Item", o.ToString());
                                break;
                            //Class
                            case "Object":
                                foreach (object o in array)
                                    WriteObjectElement(o, "Item", writer);
                                break;
                            //Primitive class
                            case "String":
                                string output1 = "";
                                if (array.Count > 0)
                                {
                                    output1 = array[0].ToString();
                                    if (output1.Contains(" "))
                                        output1 = "\"" + output1 + "\"";
                                    for (int i = 1; i < array.Count; ++i)
                                    {
                                        string s = array[i].ToString();
                                        if (s.Contains(" "))
                                            s = "\"" + s + "\"";
                                        output1 += " " + s;
                                    }
                                }
                                writer.WriteString(output1);
                                break;
                            //Primitive struct
                            case "Enum":
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
                                string output2 = "";
                                if (array.Count > 0)
                                {
                                    output2 = array[0].ToString();
                                    for (int i = 1; i < array.Count; ++i)
                                        output2 += " " + array[i].ToString();
                                }
                                writer.WriteString(output2);
                                break;
                            default:
                                elementType = elementType.BaseType;
                                goto Top1;
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
                    List<VarInfo> structFields = CollectSerializedMembers(info.VariableType);
                    if (structFields.Count > 0)
                        WriteObjectElement(value, structFields, info.Name, writer);
                    else
                        writer.WriteElementString(info.Name, value.ToString());
                    return;
                }
                else
                    WriteObjectElement(value, info.Name, writer);
            }
        }
    }
}
