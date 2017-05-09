using CustomEngine.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace CustomEngine.Files.Serialization
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
            List<VarInfo> fields = SerializationCommon.CollectSerializedMembers(t);
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
                name = SerializationCommon.GetTypeName(t);

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
                if (info.Attrib.SerializeIf != null && 
                    !ExpressionParser.Evaluate<bool>(info.Attrib.SerializeIf, obj))
                    return;

                object value = info.GetValue(obj);
                if (value == null)
                    return;

                if ()
                if (t.GetInterface("IList") != null && 
                    info.GetValue(obj) is IList array)
                {
                    writer.WriteStartElement(info.Name);
                    writer.WriteAttributeString("Count", array.Count.ToString());
                    if (array.Count > 0)
                    {
                        Type elementType = array[0].GetType();
                        if (elementType.IsEnum || array[0] is string)
                            WriteStringArray(array, writer);
                        else if (elementType.IsValueType)
                            WriteStructArray(array, writer);
                        else
                            foreach (object o in array)
                                WriteObjectElement(o, "Item", writer);
                    }
                    writer.WriteEndElement();
                    return;
                }

                if (value is IParsable)
                    writer.WriteElementString(info.Name, ((IParsable)value).WriteToString());
                else if (t.IsEnum || value is string)
                    writer.WriteElementString(info.Name, value.ToString());
                else if (t.IsValueType)
                {
                    List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(info.VariableType);
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

        private static void WriteStructArray(IList array, XmlWriter writer)
        {
            List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(array[0].GetType());
            //Struct has serialized members within it?
            //Needs a full element
            if (structFields.Count > 0)
                foreach (object o in array)
                    WriteObjectElement(o, structFields, "Item", writer);
            else
            {
                //Write each struct as a string
                string output = array[0].ToString();
                for (int i = 1; i < array.Count; ++i)
                    output += " " + array[i].ToString();
                writer.WriteString(output);
            }
        }

        private static void WriteStringArray(IList array, XmlWriter writer)
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
    }
}
