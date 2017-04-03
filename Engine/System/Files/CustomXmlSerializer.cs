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
            using (XMLReader reader = new XMLReader(map.Address, map.Length))
            {
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
            List<Pair> fields = CollectFields(t);
            //FileObject obj = (FileObject)FormatterServices.GetUninitializedObject(t);
            object obj = Activator.CreateInstance(t);

            var categorized = fields.
                Where(x => x._category != null).
                GroupBy(x => x._category).ToList();

            foreach (var grouping in categorized)
                foreach (Pair p in grouping)
                    fields.Remove(p);

            List<Pair> attribs = fields.Where(x => x._attrib.IsXmlAttribute).ToList();
            while (reader.ReadAttribute())
            {
                Pair attrib = attribs.FirstOrDefault(x => reader.Name.Equals(x._name, true));
                if (attrib != null)
                {
                    Type fieldType = attrib._info.FieldType;
                    object value = ParseString(reader.Value, fieldType);
                    attrib._info.SetValue(obj, value);
                    attribs.Remove(attrib);
                }
            }

            foreach (Pair attrib in attribs)
                attrib._info.SetValue(obj, attrib._attrib.DefaultValue);

            List<Pair> elements = fields.Where(x => !x._attrib.IsXmlAttribute).ToList();
            while (reader.BeginElement())
            {
                var category = categorized.FirstOrDefault(x => reader.Name.Equals(x.Key, true));
                if (category != null)
                {
                    while (reader.ReadAttribute())
                    {
                        Pair p = category.FirstOrDefault(x => x._attrib.IsXmlAttribute && reader.Name.Equals(x._name, true));
                        
                    }
                    while (reader.BeginElement())
                    {

                    }
                }
                else
                {
                    Pair element = elements.FirstOrDefault(x => reader.Name.Equals(x._name, true));
                    if (element != null)
                    {
                        Type fieldType = element._info.FieldType;

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
            List<Pair> fields = CollectFields(obj.GetType());
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
        private static void WriteObjectElement(object obj, List<Pair> fields, XmlWriter writer)
        {
            if (obj == null)
                return;

            Type t = obj.GetType();

            var categorized = fields.
                Where(x => x._category != null).
                GroupBy(x => x._category).ToList();

            foreach (var grouping in categorized)
                foreach (Pair p in grouping)
                    fields.Remove(p);

            //Write start tag for this object
            string name = GetTypeName(t);
            writer.WriteStartElement(name);
            {
                //Write attributes and then elements
                foreach (Pair p in fields)
                    WriteField(obj, p, writer);

                //Write categorized elements
                foreach (var grouping in categorized)
                {
                    //Write category element
                    writer.WriteStartElement(grouping.Key);
                    {
                        //Write fields on this element
                        foreach (Pair p in grouping.OrderBy(x => !x._attrib.IsXmlAttribute))
                            WriteField(obj, p, writer);
                    }
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        private static void WriteField(object obj, Pair p, XmlWriter writer)
        {
            Type t = p._info.FieldType;
            if (p._attrib.IsXmlAttribute)
            {
                object value = p._info.GetValue(obj);
                if (t.Name == "String" && value == null)
                {
                    //writer.WriteAttributeString(p._name, "");
                }
                else
                    writer.WriteAttributeString(p._name, p._info.GetValue(obj).ToString());
            }
            else
            {
                if (p._attrib.SerializeIf != null && 
                    !BooleanExpressionParser.Evaluate(p._attrib.SerializeIf, obj))
                    return;

                if (t.GetInterface("IList") != null)
                {
                    writer.WriteStartElement(p._name);
                    IList array = (IList)p._info.GetValue(obj);
                    writer.WriteAttributeString("Count", array.Count.ToString());
                    if (array.Count > 0)
                    {
                        Type elementType = array[0].GetType();
                        Top1:
                        switch (GetTypeName(elementType))
                        {
                            //Struct
                            case "ValueType":
                                List<Pair> structFields = CollectFields(p._info.FieldType);
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
                                List<Pair> classFields = CollectFields(p._info.FieldType);
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

                object value = p._info.GetValue(obj);
                if (value == null)
                    return;
                Top:
                switch (GetTypeName(t))
                {
                    //Struct
                    case "ValueType":
                        List<Pair> structFields = CollectFields(p._info.FieldType);
                        if (structFields.Count > 0)
                            WriteObjectElement(value, structFields, writer);
                        else
                            writer.WriteElementString(p._name, value.ToString());
                        break;
                    //Class
                    case "Object":
                        List<Pair> classFields = CollectFields(p._info.FieldType);
                        WriteObjectElement(value, classFields, writer);
                        break;
                    //Primitive class
                    case "String":
                        //if (value != null)
                            writer.WriteElementString(p._name, value.ToString());
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
                        writer.WriteElementString(p._name, value.ToString());
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
        /// Stores a field's information paired with direct access to its serialize attribute.
        /// </summary>
        private class Pair
        {
            public string _name;
            public string _category = null;
            public FieldInfo _info;
            public Serialize _attrib;
            public Pair(FieldInfo info)
            {
                _info = info;
                _attrib = _info.GetCustomAttribute<Serialize>();
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
            public override string ToString() => _name;
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
        private static List<Pair> CollectFields(Type t)
        {
            List<Pair> fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).
                Where(prop => Attribute.IsDefined(prop, typeof(Serialize))).
                Select(x => new Pair(x)).
                //False comes first, so negate the bool so attributes are first
                OrderBy(x => !x._attrib.IsXmlAttribute).ToList();

            int attribCount = fields.Count(x => x._attrib.IsXmlAttribute);
            int elementCount = fields.Count - attribCount;

            for (int i = 0; i < fields.Count; ++i)
            {
                Pair info = fields[i];
                Serialize s = info._attrib;
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
