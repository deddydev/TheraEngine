using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using TheraEngine.Core.Tools;

namespace TheraEngine.Files.Serialization
{
    public partial class CustomXmlSerializer
    {
        private readonly XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace,
            Async = true,
        };

        private ESerializeFlags _flags;
        private XmlWriter _writer;
        private string _fileDir;
        private Dictionary<Guid, TObject> _objects;

        /// <summary>
        /// Writes the given object to the path as xml.
        /// </summary>
        public async void Serialize(
            TFileObject obj,
            string filePath,
            ESerializeFlags flags = ESerializeFlags.Default)
        {
            _objects = new Dictionary<Guid, TObject>();
            _flags = flags;
            _fileDir = Path.GetDirectoryName(filePath);
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan))
            using (_writer = XmlWriter.Create(stream, _writerSettings))
            {
                _writer.Flush();
                stream.Position = 0;
                await _writer.WriteStartDocumentAsync();
                WriteObject(obj, null, true);
                await _writer.WriteEndDocumentAsync();
            }
        }
        private void WriteObject(
            object obj,
            string name,
            bool writeTypeDefinition)
        {
            if (obj == null)
                return;
            
            List<VarInfo> fields = SerializationCommon.CollectSerializedMembers(obj.GetType());
            WriteObject(obj, fields, name, writeTypeDefinition);
        }
        private void WriteObject(
            object obj,
            List<VarInfo> members,
            string name,
            bool writeTypeDefinition)
        {
            if (obj == null)
                return;

            //Update the object's file path
            if (obj is IFileObject fobj)
            {
                fobj.FilePath = _rootFilePath;
                if (obj is IFileRef fref && !fref.StoredInternally)
                {
                    //Make some last minute adjustments to external file refs
                    //First, update file relative paths using the new file location
                    if (fref.PathType == EPathType.FileRelative)
                    {
                        string root = Path.GetPathRoot(fref.ReferencePathAbsolute);
                        int colonIndex = root.IndexOf(":");
                        if (colonIndex > 0)
                            root = root.Substring(0, colonIndex);
                        else
                            root = string.Empty;

                        string root2 = Path.GetPathRoot(_fileDir);
                        colonIndex = root2.IndexOf(":");
                        if (colonIndex > 0)
                            root2 = root2.Substring(0, colonIndex);
                        else
                            root2 = string.Empty;

                        if (!string.Equals(root, root2))
                        {
                            //Totally different drives, cannot be relative in any way
                            fref.PathType = EPathType.Absolute;
                        }
                    }
                    if (fref.IsLoaded)
                    {
                        string path = fref.ReferencePathAbsolute;
                        bool fileExists =
                            !string.IsNullOrWhiteSpace(path) &&
                            path.IsExistingDirectoryPath() == false &&
                            File.Exists(path);

                        //TODO: export even if the file exists,
                        //however only if the file has changed
                        if (!fileExists)
                        {
                            if (fref is IGlobalFileRef && !_flags.HasFlag(ESerializeFlags.ExportGlobalRefs))
                                return;
                            if (fref is ILocalFileRef && !_flags.HasFlag(ESerializeFlags.ExportLocalRefs))
                                return;

                            string absPath;
                            if (fref.PathType == EPathType.FileRelative)
                            {
                                string rel = fref.ReferencePathAbsolute.MakePathRelativeTo(_fileDir);
                                absPath = Path.GetFullPath(Path.Combine(_fileDir, rel));
                                //fref.ReferencePathRelative = absPath.MakePathRelativeTo(_fileDir);
                            }
                            else
                                absPath = fref.ReferencePathAbsolute;

                            string dir = absPath.Contains(".") ? Path.GetDirectoryName(absPath) : absPath;

                            IFileObject file = fref.File;
                            if (file.FileExtension != null)
                            {
                                string fileName = SerializationCommon.ResolveFileName(
                                    _fileDir, file.Name, file.FileExtension.GetProperExtension(EProprietaryFileFormat.XML));
                                file.Export(dir, fileName, EFileFormat.XML, null, _flags);
                            }
                            else
                            {
                                var f = file.File3rdPartyExtensions;
                                if (f != null && f.ExportableExtensions != null && f.ExportableExtensions.Length > 0)
                                {
                                    string ext = f.ExportableExtensions[0];
                                    string fileName = SerializationCommon.ResolveFileName(_fileDir, file.Name, ext);
                                    file.Export(dir, fileName, EFileFormat.ThirdParty, ext, _flags);
                                }
                                else
                                    Engine.LogWarning("Cannot export " + file.GetType().GetFriendlyName());
                            }
                        }
                    }
                }
            }
            
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
            _writer.WriteStartElement(string.IsNullOrEmpty(name) ? SerializationCommon.GetTypeName(objType) : name);
            {
                if (writeTypeDefinition)
                    _writer.WriteAttributeString(SerializationCommon.TypeIdent, objType.AssemblyQualifiedName);
                
                //Attributes are already sorted to come first, then elements
                WriteMembers(obj, members, categorized.Count, customMethods);

                //Write categorized elements
                foreach (var grouping in categorized)
                {
                    //Write category element
                    _writer.WriteStartElement(grouping.Key);
                    {
                        //Write members for this category
                        WriteMembers(obj, grouping.OrderBy(x => !x.Attrib.IsXmlAttribute).ToList(), 0, customMethods);
                    }
                    _writer.WriteEndElement();
                }
            }
            _writer.WriteEndElement();
        }
        private void WriteMembers(
            object obj,
            List<VarInfo> members,
            int categorizedCount, 
            IEnumerable<MethodInfo> customMethods)
        {
            int nonAttribCount = members.Where(x => !x.Attrib.IsXmlAttribute && x.GetValue(obj) != null).Count() + categorizedCount;
            foreach (VarInfo member in members)
            {
                if (member.Attrib.State && !_flags.HasFlag(ESerializeFlags.SerializeState))
                    continue;
                if (member.Attrib.Config && !_flags.HasFlag(ESerializeFlags.SerializeConfig))
                    continue;
                
                MethodInfo customMethod = customMethods.FirstOrDefault(
                    x => string.Equals(member.Name, x.GetCustomAttribute<CustomXMLSerializeMethod>().Name));
                if (customMethod != null)
                    customMethod.Invoke(obj, new object[] { _writer, _flags });
                else
                    WriteMember(obj, member, nonAttribCount);
            }
        }
        private async void WriteMember(
            object obj, 
            VarInfo member,
            int nonAttributeCount)
        {
            if (!string.IsNullOrWhiteSpace(member.Attrib.Condition) &&
                !ExpressionParser.Evaluate<bool>(member.Attrib.Condition, obj))
                return;

            object value = member.GetValue(obj);
            if (value != null)
            {
                Type valueType = value.GetType();

                //If the type of the value is a derived type from what we expect,
                //the type definition must be written to preserve that derivation
                bool writeTypeDef = valueType != member.VariableType;

                if (member.Attrib.IsXmlElementString)
                {
                    if (SerializationCommon.GetString(value, member.VariableType, out string result))
                    {
                        if (nonAttributeCount == 1)
                            await _writer.WriteStringAsync(result);
                        else
                            await _writer.WriteAttributeStringAsync(null, member.Name, null, result);
                    }
                    else
                        WriteElement(value, member, writeTypeDef);
                }
                else if (member.Attrib.IsXmlAttribute)
                {
                    if (SerializationCommon.GetString(value, member.VariableType, out string result))
                        _writer.WriteAttributeString(member.Name, result);
                    else
                        WriteElement(value, member, writeTypeDef);
                }
                else
                    WriteElement(value, member, writeTypeDef);
            }
        }
        
        private void WriteElement(
            object value,
            VarInfo member,
            bool writeTypeDefinition = false)
        {
            switch (SerializationCommon.GetValueType(member.VariableType))
            {
                case SerializationCommon.ValueType.Manual:
                    ((TFileObject)value).Write(_writer, _flags);
                    break;
                case SerializationCommon.ValueType.Parsable:
                    _writer.WriteElementString(member.Name, ((IParsable)value).WriteToString());
                    break;
                case SerializationCommon.ValueType.Array:
                    WriteArray(value as IList, member.Name, member.VariableType);
                    break;
                case SerializationCommon.ValueType.Dictionary:
                    WriteDictionary(value as IDictionary, member);
                    break;
                case SerializationCommon.ValueType.Enum:
                case SerializationCommon.ValueType.String:
                    _writer.WriteElementString(member.Name, value.ToString());
                    break;
                case SerializationCommon.ValueType.Struct:
                    List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(member.VariableType);
                    if (structFields.Count > 0)
                        WriteObject(value, structFields, member.Name, writeTypeDefinition);
                    else
                    {
                        if (SerializationCommon.IsPrimitiveType(member.VariableType))
                            _writer.WriteElementString(member.Name, value.ToString());
                        else
                        {
                            //Custom struct with no members marked as serializable
                            //Serialize all members
                            //FieldInfo[] members = member.VariableType.GetFields(
                            //    BindingFlags.NonPublic |
                            //    BindingFlags.Instance |
                            //    BindingFlags.Public |
                            //    BindingFlags.FlattenHierarchy);
                            string str = SerializationCommon.GetStructAsBytesString(value);
                            string structTypeName = value.GetType().GetFriendlyName();
                            _writer.WriteElementString(structTypeName, str);
                        }
                    }
                    break;
                case SerializationCommon.ValueType.Pointer:
                    WriteObject(value, member.Name, writeTypeDefinition);
                    break;
            }
        }
        private void WriteDictionary(IDictionary dic, VarInfo member)
        {
            _writer.WriteStartElement(member.Name);
            _writer.WriteAttributeString("Count", dic.Count.ToString());
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
                    _writer.WriteStartElement("KeyValuePair");
                    _writer.WriteAttributeString("Index", i.ToString());
                    WriteElement(dicKeys[i], vKeys);
                    WriteElement(dicVals[i], vVals, true);
                    _writer.WriteEndElement();
                }
            }
            _writer.WriteEndElement();
        }
        private void WriteArray(IList array, string name, Type arrayType)
        {
            _writer.WriteStartElement(name);
            _writer.WriteAttributeString("Count", array.Count.ToString());
            Type type = array.GetType();
            if (type != arrayType)
                _writer.WriteAttributeString(SerializationCommon.TypeIdent, type.AssemblyQualifiedName);
            if (array.Count > 0)
            {
                Type elementType = arrayType.GetElementType() ?? arrayType.GenericTypeArguments[0];
                string elementName = SerializationCommon.GetTypeName(elementType);//elementType.GetFriendlyName("[", "]");
                switch (SerializationCommon.GetValueType(elementType))
                {
                    case SerializationCommon.ValueType.Parsable:
                        WriteStringArray(array, true, false);
                        break;
                    case SerializationCommon.ValueType.Array:
                        for (int i = 0; i < array.Count; ++i)
                            WriteArray(array[i] as IList, "Item", elementType);
                        break;
                    case SerializationCommon.ValueType.Enum:
                        WriteStringArray(array, false, true);
                        break;
                    case SerializationCommon.ValueType.String:
                        WriteStringArray(array, false, false);
                        break;
                    case SerializationCommon.ValueType.Struct:
                        WriteStructArray(array, elementType);
                        break;
                    case SerializationCommon.ValueType.Pointer:
                        foreach (object o in array)
                            WriteObject(o, elementName, o?.GetType() != elementType);
                        break;
                }
            }
            _writer.WriteEndElement();
        }
        private void WriteStructArray(IList array, Type elementType)
        {
            string elementName = elementType.Name;
            List<VarInfo> fields = SerializationCommon.CollectSerializedMembers(elementType);
            //Struct has serialized members within it?
            //Needs a full element
            if (fields.Count > 0)
                foreach (object o in array)
                    WriteObject(o, fields, elementName, false);
            else
            {
                if (SerializationCommon.IsPrimitiveType(elementType))
                    WriteStringArray(array, false, false);
                else
                {
                    //var members = elementType.GetFields(
                    //    BindingFlags.NonPublic | 
                    //    BindingFlags.Instance |
                    //    BindingFlags.Public | 
                    //    BindingFlags.FlattenHierarchy);
                    foreach (object o in array)
                    {
                        string str = SerializationCommon.GetStructAsBytesString(o);
                        string structTypeName = o.GetType().GetFriendlyName();
                        _writer.WriteElementString(structTypeName, str);
                    }
                }
            }
        }
        private void WriteStringArray(IList array, bool parsable, bool enums)
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
            _writer.WriteString(output);
        }
    }
}
