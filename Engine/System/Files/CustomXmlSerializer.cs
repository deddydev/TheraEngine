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
    public static class CustomXmlSerializer
    {
        #region Deserialization
        /// <summary>
        /// Reads a file from the stream as xml.
        /// </summary>
        public static unsafe FileObject Deserialize(string filePath, Type t)
        {
            FileObject obj;
            using (FileMap map = FileMap.FromFile(filePath))
            {
                XMLReader reader = new XMLReader(map.BaseStream);
                if (reader.BeginElement())
                {
                    obj = (FileObject)ReadObject(t, reader);
                    reader.EndElement();
                }
                else
                    obj = (FileObject)Activator.CreateInstance(t);
                obj._filePath = filePath;
            }
            return obj;
        }
        private static object ReadObject(Type t, XMLReader reader)
        {
            List<VarInfo> fields = CollectSerializedMembers(t);
            //FileObject obj = (FileObject)FormatterServices.GetUninitializedObject(t);
            object obj = Activator.CreateInstance(t);
            MethodInfo[] methods = t.GetMethods();

            var categorized = fields.
                Where(x => x.Category != null).
                GroupBy(x => x.Category).ToList();

            foreach (var grouping in categorized)
                foreach (VarInfo p in grouping)
                    fields.Remove(p);

            List<VarInfo> attribs = fields.Where(x => x.Attrib.IsXmlAttribute).ToList();
            while (reader.ReadAttribute())
            {
                VarInfo attrib = attribs.FirstOrDefault(x => String.Equals(reader.Name, x.Name, StringComparison.InvariantCultureIgnoreCase));
                if (attrib != null)
                {
                    Type fieldType = attrib.VariableType;
                    object value = ParseString(reader.Value, fieldType);
                    attrib.SetValue(obj, value);
                    attribs.Remove(attrib);
                }
            }

            foreach (VarInfo attrib in attribs)
                attrib.SetValue(obj, attrib.Attrib.DefaultValue);

            List<VarInfo> elements = fields.Where(x => !x.Attrib.IsXmlAttribute).ToList();
            while (reader.BeginElement())
            {
                var category = categorized.FirstOrDefault(x => String.Equals(reader.Name, x.Key, StringComparison.InvariantCultureIgnoreCase));
                if (category != null)
                {
                    while (reader.ReadAttribute())
                    {
                        VarInfo p = category.FirstOrDefault(x => x.Attrib.IsXmlAttribute && String.Equals(reader.Name, x.Name, StringComparison.InvariantCultureIgnoreCase));
                        
                    }
                    while (reader.BeginElement())
                    {

                    }
                }
                else
                {
                    VarInfo element = elements.FirstOrDefault(x => reader.Name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (element != null)
                    {
                        Type fieldType = element.VariableType;

                    }
                }
                reader.EndElement();
            }

            return obj;
        }
        private static object ParseString(string value, Type t)
        {
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
            return null;
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Writes the given file to the stream as xml.
        /// </summary>
        public static void Serialize(FileObject obj, string filePath)
        {
            List<VarInfo> fields = CollectSerializedMembers(obj.GetType());
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan))
            using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
            {
                writer.Flush();
                stream.Position = 0;
                writer.WriteStartDocument();
                WriteObjectElement(obj, fields, writer);
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
        private static void WriteObjectElement(object obj, List<VarInfo> fields, XmlWriter writer)
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
            string name = GetTypeName(t);
            writer.WriteStartElement(name);
            {
                //Write attributes and then elements
                foreach (VarInfo p in fields)
                    WriteField(obj, p, writer);

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
                    !BooleanExpressionParser.Evaluate(info.Attrib.SerializeIf, obj))
                    return;

                if (t.GetInterface("IList") != null)
                {
                    writer.WriteStartElement(info.Name);
                    IList array = (IList)info.GetValue(obj);
                    writer.WriteAttributeString("Count", array.Count.ToString());
                    if (array.Count > 0)
                    {
                        Type elementType = array[0].GetType();
                        Top1:
                        switch (GetTypeName(elementType))
                        {
                            //Struct
                            case "ValueType":
                                List<VarInfo> structFields = CollectSerializedMembers(info.VariableType);
                                if (structFields.Count > 0)
                                {
                                    foreach (object o in array)
                                        WriteObjectElement(o, structFields, writer);
                                }
                                else
                                {
                                    foreach (object o in array)
                                        writer.WriteElementString("Item", o.ToString());
                                }
                                break;
                            //Class
                            case "Object":
                                List<VarInfo> classFields = CollectSerializedMembers(info.VariableType);
                                foreach (object o in array)
                                    WriteObjectElement(o, classFields, writer);
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
                Top:
                switch (GetTypeName(t))
                {
                    //Struct
                    case "ValueType":
                        List<VarInfo> structFields = CollectSerializedMembers(info.VariableType);
                        if (structFields.Count > 0)
                            WriteObjectElement(value, structFields, writer);
                        else
                            writer.WriteElementString(info.Name, value.ToString());
                        break;
                    //Class
                    case "Object":
                        List<VarInfo> classFields = CollectSerializedMembers(info.VariableType);
                        WriteObjectElement(value, classFields, writer);
                        break;
                    //Primitive class
                    case "String":
                        //if (value != null)
                            writer.WriteElementString(info.Name, value.ToString());
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
                        writer.WriteElementString(info.Name, value.ToString());
                        break;
                    default:
                        t = t.BaseType;
                        goto Top;
                }
            }
        }
        #endregion

        #region Common
        /// <summary>
        /// Stores a field/property's information.
        /// </summary>
        private class VarInfo
        {
            private string _name;
            private string _category = null;
            private MemberInfo _info;
            private Serialize _attrib;
            private Type _variableType;

            public Type VariableType => _variableType;
            public string Name => _name;
            public string Category => _category; 
            public Serialize Attrib => _attrib;

            public void SetValue(object obj, object value)
            {
                if (_info.MemberType.HasFlag(MemberTypes.Field))
                    ((FieldInfo)_info).SetValue(obj, value);
                else if (_info.MemberType.HasFlag(MemberTypes.Property))
                    ((PropertyInfo)_info).SetValue(obj, value);
            }
            public object GetValue(object obj)
            {
                if (_info.MemberType.HasFlag(MemberTypes.Field))
                    return ((FieldInfo)_info).GetValue(obj);
                if (_info.MemberType.HasFlag(MemberTypes.Property))
                    return ((PropertyInfo)_info).GetValue(obj);
                return null;
            }
            public VarInfo(MemberInfo info)
            {
                _info = info;
                _attrib = _info.GetCustomAttribute<Serialize>();
                if (_info.MemberType.HasFlag(MemberTypes.Field))
                    _variableType = ((FieldInfo)_info).FieldType;
                else if (_info.MemberType.HasFlag(MemberTypes.Property))
                    _variableType = ((PropertyInfo)_info).PropertyType;
                if (_attrib.NameOverride != null)
                    _name = _attrib.NameOverride;
                else
                {
                    DisplayNameAttribute nameAttrib = _info.GetCustomAttribute<DisplayNameAttribute>();
                    if (nameAttrib != null)
                        _name = nameAttrib.DisplayName;
                    else
                        _name = _info.Name;
                }
                if (_attrib.UseCategory)
                {
                    if (_attrib.OverrideXmlCategory != null)
                        _category = _attrib.OverrideXmlCategory;
                    else
                    {
                        CategoryAttribute categoryAttrib = _info.GetCustomAttribute<CategoryAttribute>();
                        if (categoryAttrib != null)
                            _category = categoryAttrib.Category;
                    }
                }
            }
            public override string ToString() => Name;
        }
        private static string GetTypeName(Type t)
        {
            string name = t.Name;
            if (t.IsGenericType)
            {
                name = t.Name.Remove(t.Name.IndexOf('`'))/* + "-"*/;
                //Type[] genericArgs = t.GenericTypeArguments;
                //bool first = true;
                //foreach (Type gt in genericArgs)
                //{
                //    if (first)
                //        first = false;
                //    else
                //        name += ", ";
                //    name += GetTypeName(gt);
                //}
                //name += "-";
            }
            return name;
        }
        private static List<VarInfo> CollectSerializedMembers(Type t)
        {
            List<VarInfo> fields = t.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).
                Where(x => (x is FieldInfo || x is PropertyInfo) && Attribute.IsDefined(x, typeof(Serialize))).
                Select(x => new VarInfo(x)).
                //False comes first, so negate the bool so attributes are first
                OrderBy(x => !x.Attrib.IsXmlAttribute).ToList();

            int attribCount = fields.Count(x => x.Attrib.IsXmlAttribute);
            int elementCount = fields.Count - attribCount;

            for (int i = 0; i < fields.Count; ++i)
            {
                VarInfo info = fields[i];
                Serialize s = info.Attrib;
                if (s.Order >= 0)
                {
                    int index = s.Order;

                    if (i < attribCount)
                        index = index.Clamp(0, attribCount - 1);
                    else
                        index = index.Clamp(0, elementCount - 1) + attribCount;

                    if (index == i)
                        continue;
                    fields.RemoveAt(i--);
                    if (index == fields.Count)
                        fields.Add(info);
                    else
                        fields.Insert(index, info);
                }
            }
            return fields;
        }
        #endregion
    }
}
