using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CustomEngine.Files
{
    public class CustomXmlSerializer
    {
        private static XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };
        private class Pair
        {
            public Pair(FieldInfo info)
            {
                _info = info;
                _attrib = _info.GetCustomAttribute<Serialize>();
                _name = _attrib.NameOverride ?? _info.Name;
            }

            public string _name;
            public FieldInfo _info;
            public Serialize _attrib;
        }
        
        private List<Pair> CollectFields(Type t)
        {
            List<Pair> fields = t.GetFields().
                Where(prop => Attribute.IsDefined(prop, typeof(Serialize))).
                Select(x => new Pair(x)).
                //False comes first, so negate the bool so attributes are first
                OrderBy(x => !x._attrib.IsXmlAttribute).ToList();
            int attribCount = fields.Count(x => x._attrib.IsXmlAttribute);

            for (int i = 0; i < fields.Count; ++i)
            {
                Pair info = fields[i];
                Serialize s = info._attrib;
                if (s.Order >= 0)
                {
                    int index = s.Order.Clamp(0, fields.Count);
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

        /// <summary>
        /// Reads a file from the stream as xml.
        /// </summary>
        public unsafe FileObject Deserialize(string filePath, Type t)
        {
            List<Pair> fields = CollectFields(t);
            FileObject obj = (FileObject)Activator.CreateInstance(t);
            obj._filePath = filePath;
            using (FileMap map = FileMap.FromFile(filePath))
            using (XMLReader reader = new XMLReader(map.Address, map.Length))
                ReadFields(obj, fields, reader);
            return obj;
        }
        private void ReadFields(object obj, List<Pair> fields, XMLReader reader)
        {
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
                Pair element = elements.FirstOrDefault(x => reader.Name.Equals(x._name, true));
                if (element != null)
                {
                    Type fieldType = element._info.FieldType;

                }
                reader.EndElement();
            }
        }
        private object ParseString(string value, Type t)
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
        /// <summary>
        /// Writes the given file to the stream as xml.
        /// </summary>
        public void Serialize(FileObject obj, string filePath)
        {
            List<Pair> fields = CollectFields(obj.GetType());
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan))
            using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
            {
                writer.Flush();
                stream.Position = 0;

                writer.WriteStartDocument();
                WriteFields(obj, fields, writer);
                writer.WriteEndDocument();
            }
        }
        private void WriteFields(object obj, List<Pair> fields, XmlWriter writer)
        {
            if (obj == null)
                return;

            Type t = obj.GetType();

            var categorized = fields.
                Where(x => x._attrib.XmlCategoryGrouping != null).
                GroupBy(x => x._attrib.XmlCategoryGrouping).ToList();

            foreach (var grouping in categorized)
                foreach (Pair p in grouping)
                    fields.Remove(p);

            //Write start tag for this object
            string name = obj.GetType().Name;
            writer.WriteStartElement(name);
            {
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
        private void WriteField(object obj, Pair p, XmlWriter writer)
        {
            if (p._attrib.IsXmlAttribute)
                writer.WriteAttributeString(p._name, p._info.GetValue(obj).ToString());
            else
            {
                Type t = p._info.FieldType;
                Top:
                switch (t.Name)
                {
                    case "Object":
                        object obj2 = p._info.GetValue(obj);
                        List<Pair> fields2 = CollectFields(p._info.FieldType);
                        WriteFields(obj2, fields2, writer);
                        break;
                    case "ValueType":
                        throw new Exception();
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
                        writer.WriteElementString(p._name, p._info.GetValue(obj).ToString());
                        break;
                    default:
                        t = t.BaseType;
                        goto Top;
                }
            }
        }
    }
}
