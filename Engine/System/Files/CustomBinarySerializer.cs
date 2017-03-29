using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Files
{
    public class CustomBinarySerializer
    {
        object _object;
        StringTable _table;
        List<FieldInfo> _sortedFields;

        public void Serialize(FileObject obj, FileStream stream)
        {
            _object = obj;

            _sortedFields = _object.GetType().GetFields().Where(
                prop => Attribute.IsDefined(prop, typeof(Serialize))).ToList();
            for (int i = 0; i < _sortedFields.Count; ++i)
            {
                FieldInfo info = _sortedFields[i];
                Serialize s = info.GetCustomAttribute<Serialize>();
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

            int size = GetSize();
            
        }
        private int GetSize()
        {
            _table = new StringTable();
            int size = 0;

            foreach (FieldInfo i in _sortedFields)
            {
                Type fieldType = i.GetType();
                switch (i.GetType().Name)
                {
                    case "byte":

                        break;
                    case "sbyte":

                        break;
                    case "ushort":

                        break;
                    case "short":

                        break;
                    case "uint":

                        break;
                    case "int":

                        break;
                    case "float":

                        break;
                    case "double":

                        break;
                    case "string":

                        break;
                    default:

                        break;
                }
            }
            return size + _table.GetTotalSize();
        }
    }
}
