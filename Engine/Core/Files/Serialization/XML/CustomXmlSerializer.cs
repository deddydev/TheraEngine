﻿using TheraEngine.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace TheraEngine.Files.Serialization
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
                WriteObject(obj, null, writer, true);
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
        private static void WriteObject(object obj, string name, XmlWriter writer, bool rootElement)
        {
            if (obj == null)
                return;

            Type t = obj.GetType();
            List<VarInfo> fields = SerializationCommon.CollectSerializedMembers(t);
            WriteObject(obj, fields, name, writer, rootElement);
        }
        private static void WriteObject(object obj, List<VarInfo> members, string name, XmlWriter writer, bool rootElement)
        {
            if (obj == null)
                return;

            Type objType = obj.GetType();

            //Get members categorized together
            var categorized = members.
                Where(x => x.Category != null).
                GroupBy(x => x.Category).ToList();
            //Remove categorized members from original list
            foreach (var grouping in categorized)
                foreach (VarInfo p in grouping)
                    members.Remove(p);

            //Write start tag for this object
            if (string.IsNullOrEmpty(name))
                name = SerializationCommon.GetTypeName(objType);

            //Get custom serialize methods
            var customMethods = objType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(x => x.GetCustomAttribute<CustomXMLSerializeMethod>() != null);

            //Write the element for this object
            writer.WriteStartElement(name);
            {
                if (rootElement)
                    writer.WriteAttributeString("Type", objType.AssemblyQualifiedName);

                //Write attributes and then elements
                foreach (VarInfo p in members)
                {
                    MethodInfo customMethod = customMethods.FirstOrDefault(
                        x => string.Equals(p.Name, x.GetCustomAttribute<CustomXMLSerializeMethod>().Name));
                    if (customMethod != null)
                        customMethod.Invoke(obj, new object[] { writer });
                    else
                        WriteMember(obj, p, writer);
                }
                //Write categorized elements
                foreach (var grouping in categorized)
                {
                    //Write category element
                    writer.WriteStartElement(grouping.Key);
                    {
                        //Write fields on this element
                        foreach (VarInfo p in grouping.OrderBy(x => !x.Attrib.IsXmlAttribute))
                        {
                            MethodInfo customMethod = customMethods.FirstOrDefault(
                                x => string.Equals(p.Name, x.GetCustomAttribute<CustomXMLSerializeMethod>().Name));
                            if (customMethod != null)
                                customMethod.Invoke(obj, new object[] { writer });
                            else
                                WriteMember(obj, p, writer);
                        }
                    }
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        private static void WriteMember(object obj, VarInfo info, XmlWriter writer)
        {
            if (info.Attrib.SerializeIf != null &&
                !ExpressionParser.Evaluate<bool>(info.Attrib.SerializeIf, obj))
                return;

            Type t = info.VariableType;
            if (info.Attrib.IsXmlAttribute)
            {
                object value = info.GetValue(obj);
                if (value != null)
                    writer.WriteAttributeString(info.Name, value.ToString());
            }
            else
            {
                object value = info.GetValue(obj);
                if (value == null)
                    return;
                
                if (t.GetInterface("IList") != null && 
                    info.GetValue(obj) is IList array)
                {
                    writer.WriteStartElement(info.Name);
                    writer.WriteAttributeString("Count", array.Count.ToString());
                    if (array.Count > 0)
                    {
                        Type elementType = t.GetElementType();
                        if (elementType.IsEnum || elementType == typeof(string))
                        {
                            WriteStringArray(array, writer);
                        }
                        else if (elementType.IsValueType)
                        {
                            WriteStructArray(array, writer);
                        }
                        else
                        {
                            foreach (object o in array)
                                WriteObject(o, "Item", writer, false);
                        }
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
                        WriteObject(value, structFields, info.Name, writer, false);
                    else
                        writer.WriteElementString(info.Name, value.ToString());
                    return;
                }
                else
                    WriteObject(value, info.Name, writer, false);
            }
        }

        private static void WriteStructArray(IList array, XmlWriter writer)
        {
            List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(array[0].GetType());
            //Struct has serialized members within it?
            //Needs a full element
            if (structFields.Count > 0)
                foreach (object o in array)
                    WriteObject(o, structFields, "Item", writer, false);
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
