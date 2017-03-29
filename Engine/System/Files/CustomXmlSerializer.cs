using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Files
{
    public class CustomXmlSerializer
    {
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

        object _object;
        List<Pair> _sortedFields;

        private void CollectFields(Type t)
        {
            _sortedFields = t.GetFields().Where(
                prop => Attribute.IsDefined(prop, typeof(Serialize))).Select(x => new Pair(x)).ToList();
            for (int i = 0; i < _sortedFields.Count; ++i)
            {
                Pair info = _sortedFields[i];
                Serialize s = info._attrib;
                if (s.Order >= 0)
                {
                    int index = s.Order.Clamp(0, _sortedFields.Count);
                    if (index == i)
                        return;
                    _sortedFields.RemoveAt(i--);
                    if (index == _sortedFields.Count)
                        _sortedFields.Add(info);
                    else
                        _sortedFields.Insert(index, info);
                }
            }
        }

        /// <summary>
        /// Reads a file from the stream as xml.
        /// </summary>
        public unsafe FileObject Deserialize(string filePath, Type t)
        {
            CollectFields(t);
            FileObject obj = (FileObject)Activator.CreateInstance(t);
            obj._filePath = filePath;
            using (FileMap map = FileMap.FromFile(filePath))
            using (XMLReader reader = new XMLReader(map.Address, map.Length))
                ReadFields(obj, _sortedFields, reader);
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
        public string GetString(object obj)
        {
            return null;
        }
        /// <summary>
        /// Writes the given file to the stream as xml.
        /// </summary>
        public void Serialize(FileObject obj, string filePath)
        {
            CollectFields(obj.GetType());
            _object = obj;
            
        }
    }
}
