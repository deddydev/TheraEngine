﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using TheraEngine.Core.Tools;

namespace TheraEngine.Files.Serialization
{
    public static partial class CustomXmlSerializer
    {
        private static readonly XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };

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
        private static void WriteObject(object obj, string name, XmlWriter writer, bool writeTypeDefinition)
        {
            if (obj == null)
                return;
            
            List<VarInfo> fields = SerializationCommon.CollectSerializedMembers(obj.GetType());
            WriteObject(obj, fields, name, writer, writeTypeDefinition);
        }
        private static void WriteObject(object obj, List<VarInfo> members, string name, XmlWriter writer, bool writeTypeDefinition)
        {
            if (obj == null)
                return;

            Type objType = obj.GetType();
            
            //Get custom serialize methods
            var customMethods = objType.GetMethods(
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy).
                Where(x => x.GetCustomAttribute<CustomXMLSerializeMethod>() != null);
            
            //Get members categorized together
            var categorized = members.
                Where(x => x.Category != null).
                GroupBy(x => SerializationCommon.FixElementName(x.Category)).ToList();
            //Remove categorized members from original list
            foreach (var grouping in categorized)
                foreach (VarInfo p in grouping)
                    members.Remove(p);
            
            //Write the element for this object
            writer.WriteStartElement(string.IsNullOrEmpty(name) ? SerializationCommon.GetTypeName(objType) : name);
            {
                if (writeTypeDefinition)
                    writer.WriteAttributeString(TypeIdent, objType.AssemblyQualifiedName);
                
                //Attributes are already sorted to come first, then elements
                WriteMembers(obj, members, categorized.Count, customMethods, writer);

                //Write categorized elements
                foreach (var grouping in categorized)
                {
                    //Write category element
                    writer.WriteStartElement(grouping.Key);
                    {
                        //Write members for this category
                        WriteMembers(obj, grouping.OrderBy(x => !x.Attrib.IsXmlAttribute).ToList(), 0, customMethods, writer);
                    }
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        private static void WriteMembers(object obj, List<VarInfo> members, int categorizedCount, IEnumerable<MethodInfo> customMethods, XmlWriter writer)
        {
            int nonAttribCount = members.Where(x => !x.Attrib.IsXmlAttribute && x.GetValue(obj) != null).Count() + categorizedCount;
            foreach (VarInfo member in members)
            {
                MethodInfo customMethod = customMethods.FirstOrDefault(
                    x => string.Equals(member.Name, x.GetCustomAttribute<CustomXMLSerializeMethod>().Name));
                if (customMethod != null)
                    customMethod.Invoke(obj, new object[] { writer });
                else
                    WriteMember(obj, member, writer, nonAttribCount);
            }
        }
        private static void WriteMember(object obj, VarInfo member, XmlWriter writer, int nonAttributeCount)
        {
            if (member.Attrib.Condition != null && !ExpressionParser.Evaluate<bool>(member.Attrib.Condition, obj))
                return;

            object value = member.GetValue(obj);
            if (value != null)
            {
                if (member.Attrib.IsXmlElementString)
                {
                    if (GetString(value, member.VariableType, out string result))
                    {
                        if (nonAttributeCount == 1)
                            writer.WriteString(result);
                        else
                            writer.WriteAttributeString(member.Name, result);
                    }
                    else
                        WriteElement(value, member, writer);
                }
                else if (member.Attrib.IsXmlAttribute)
                {
                    if (GetString(value, member.VariableType, out string result))
                        writer.WriteAttributeString(member.Name, result);
                    else
                        WriteElement(value, member, writer);
                }
                else
                    WriteElement(value, member, writer);
            }
        }
        private static bool GetString(object value, Type t, out string result)
        {
            if (t.GetInterface("IParsable") != null)
            {
                result = ((IParsable)value).WriteToString();
                return true;
            }
            else if (SerializationCommon.IsPrimitiveType(t))
            {
                result = value.ToString();
                return true;
            }
            else if (SerializationCommon.IsEnum(t))
            {
                result = value.ToString().Replace(", ", "|");
                return true;
            }
            result = null;
            return false;
            //throw new InvalidOperationException(t.Name + " cannot be written as a string.");
        }
        private static void WriteElement(object value, VarInfo member, XmlWriter writer, bool writeTypeDefinition = false)
        {
            switch (SerializationCommon.GetValueType(member.VariableType))
            {
                case SerializationCommon.ValueType.Manual:
                    ((FileObject)value).Write(writer);
                    break;
                case SerializationCommon.ValueType.Parsable:
                    writer.WriteElementString(member.Name, ((IParsable)value).WriteToString());
                    break;
                case SerializationCommon.ValueType.Array:
                    WriteArray(value as IList, member.Name, member.VariableType, writer);
                    break;
                case SerializationCommon.ValueType.Dictionary:
                    WriteDictionary(value as IDictionary, member, writer);
                    break;
                case SerializationCommon.ValueType.Enum:
                case SerializationCommon.ValueType.String:
                    writer.WriteElementString(member.Name, value.ToString());
                    break;
                case SerializationCommon.ValueType.Struct:
                    List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(member.VariableType);
                    if (structFields.Count > 0)
                        WriteObject(value, structFields, member.Name, writer, writeTypeDefinition);
                    else
                    {
                        if (SerializationCommon.IsPrimitiveType(member.VariableType))
                            writer.WriteElementString(member.Name, value.ToString());
                        else
                        {
                            //Custom struct with no members marked as serializable
                            //Serialize all members
                            FieldInfo[] members = member.VariableType.GetFields(
                                BindingFlags.NonPublic |
                                BindingFlags.Instance |
                                BindingFlags.Public |
                                BindingFlags.FlattenHierarchy);
                            WriteStruct(value, members, writer);
                        }
                    }
                    break;
                case SerializationCommon.ValueType.Pointer:
                    WriteObject(value, member.Name, writer, writeTypeDefinition);
                    break;
            }
        }
        private static void WriteDictionary(IDictionary dic, VarInfo member, XmlWriter writer)
        {
            writer.WriteStartElement(member.Name);
            writer.WriteAttributeString("Count", dic.Count.ToString());
            if (dic.Count > 0)
            {
                var args = member.VariableType.GetGenericArguments();
                Type keyType = args[0];
                Type valueType = args[1];

                object[] dicKeys = new object[dic.Count];
                object[] dicVals = new object[dic.Count];
                dic.Keys.CopyTo(dicKeys, 0);
                dic.Values.CopyTo(dicVals, 0);

                VarInfo vKeys = new VarInfo(keyType, null, "Key");
                VarInfo vVals = new VarInfo(valueType, null, "Value");
                for (int i = 0; i < dic.Count; ++i)
                {
                    writer.WriteStartElement("KeyValuePair");
                    writer.WriteAttributeString("Index", i.ToString());
                    WriteElement(dicKeys[i], vKeys, writer);
                    WriteElement(dicVals[i], vVals, writer, true);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        private static void WriteArray(IList array, string name, Type arrayType, XmlWriter writer)
        {
            writer.WriteStartElement(name);
            writer.WriteAttributeString("Count", array.Count.ToString());
            Type type = array.GetType();
            if (type != arrayType)
                writer.WriteAttributeString(TypeIdent, type.AssemblyQualifiedName);
            if (array.Count > 0)
            {
                Type elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];
                string elementName = SerializationCommon.GetTypeName(elementType);//elementType.GetFriendlyName("[", "]");
                switch (SerializationCommon.GetValueType(elementType))
                {
                    case SerializationCommon.ValueType.Parsable:
                        WriteStringArray(array, writer, true, false);
                        break;
                    case SerializationCommon.ValueType.Array:
                        for (int i = 0; i < array.Count; ++i)
                            WriteArray(array[i] as IList, "Item", elementType, writer);
                        break;
                    case SerializationCommon.ValueType.Enum:
                        WriteStringArray(array, writer, false, true);
                        break;
                    case SerializationCommon.ValueType.String:
                        WriteStringArray(array, writer, false, false);
                        break;
                    case SerializationCommon.ValueType.Struct:
                        WriteStructArray(array, elementType, writer);
                        break;
                    case SerializationCommon.ValueType.Pointer:
                        foreach (object o in array)
                            WriteObject(o, elementName, writer, o?.GetType() != elementType);
                        break;
                }
            }
            writer.WriteEndElement();
        }
        private static void WriteStructArray(IList array, Type elementType, XmlWriter writer)
        {
            string elementName = elementType.Name;
            List<VarInfo> fields = SerializationCommon.CollectSerializedMembers(elementType);
            //Struct has serialized members within it?
            //Needs a full element
            if (fields.Count > 0)
                foreach (object o in array)
                    WriteObject(o, fields, elementName, writer, false);
            else
            {
                if (SerializationCommon.IsPrimitiveType(elementType))
                    WriteStringArray(array, writer, false, false);
                else
                {
                    var members = elementType.GetFields(
                        BindingFlags.NonPublic | 
                        BindingFlags.Instance |
                        BindingFlags.Public | 
                        BindingFlags.FlattenHierarchy);
                    foreach (object o in array)
                        WriteStruct(o, members, writer);
                }
            }
        }
        private static void WriteStruct(object structObj, FieldInfo[] structMembers, XmlWriter writer)
        {
            throw new NotImplementedException();
            //foreach (FieldInfo member in structMembers)
            //{
            //    object value = member.GetValue(structObj);
            //}
        }
        private static void WriteStringArray(IList array, XmlWriter writer, bool parsable, bool enums)
        {
            string output = "";
            for (int i = 0; i < array.Count; ++i)
            {
                string s = parsable ? ((IParsable)(array[i])).WriteToString() : array[i].ToString();

                if (enums)
                    s = s.Replace(", ", "|");

                if (s.Contains("\\"))
                    s = s.Replace("\\", "\\\\");
                if (s.Contains(" "))
                    s = s.Replace(" ", "\\ ");

                if (i == 0)
                    output = s;
                else
                    output += " " + s;
            }
            writer.WriteString(output);
        }
    }
}
