using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Files.Serialization
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
        private IProgress<float> _progress;
        private CancellationToken _cancel;
        private FileStream _stream;

        private bool ReportProgress()
        {
            _progress.Report((float)_stream.Position / _stream.Length);
            return _cancel.IsCancellationRequested;
        }

        /// <summary>
        /// Writes the given object to the path as xml.
        /// </summary>
        public async Task SerializeAsync(
            TFileObject obj,
            string filePath,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            _objects = new Dictionary<Guid, TObject>();
            _flags = flags;
            _fileDir = Path.GetDirectoryName(filePath);
            _progress = progress;
            _cancel = cancel;

            using (_stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.SequentialScan))
            using (_writer = XmlWriter.Create(_stream, _writerSettings))
            {
                _writer.Flush();
                _stream.Position = 0;

                await _writer.WriteStartDocumentAsync();
                await WriteObjectAsync(obj, null, true);
                await _writer.WriteEndDocumentAsync();
            }
        }
        private async Task WriteObjectAsync(
            object obj,
            string name,
            bool writeTypeDefinition)
        {
            if (obj == null)
                return;
            
            List<VarInfo> fields = SerializationCommon.CollectSerializedMembers(obj.GetType());
            await WriteObjectAsync(obj, fields, name, writeTypeDefinition);
        }
        private async Task WriteObjectAsync(
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
                                await file.ExportAsync(dir, fileName, EFileFormat.XML, null, _flags, null, CancellationToken.None);
                            }
                            else
                            {
                                var f = file.File3rdPartyExtensions;
                                if (f != null && f.ExportableExtensions != null && f.ExportableExtensions.Length > 0)
                                {
                                    string ext = f.ExportableExtensions[0];
                                    string fileName = SerializationCommon.ResolveFileName(_fileDir, file.Name, ext);
                                    await file.ExportAsync(dir, fileName, EFileFormat.ThirdParty, ext, _flags, null, CancellationToken.None);
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
                Where(x => x.GetCustomAttribute<CustomSerializeMethod>() != null);
            
            //Get members categorized together
            var categorized = members.
                Where(x => x.Category != null).
                GroupBy(x => SerializationCommon.FixElementName(x.Category)).ToList();
            //Remove categorized members from original list
            foreach (var grouping in categorized)
                foreach (VarInfo p in grouping)
                    members.Remove(p);

            if (string.IsNullOrEmpty(name))
                name = SerializationCommon.GetTypeName(objType);

            //Write the element for this object
            await _writer.WriteStartElementAsync(null, name, null);
            {
                if (writeTypeDefinition)
                    await _writer.WriteAttributeStringAsync(null, SerializationCommon.TypeIdent, null, objType.AssemblyQualifiedName);
                
                //Attributes are already sorted to come first, then elements
                await WriteMembers(obj, members, categorized.Count, customMethods);

                //Write categorized elements
                foreach (var grouping in categorized)
                {
                    //Write category element
                    await _writer.WriteStartElementAsync(null, grouping.Key, null);
                    {
                        //Write members for this category
                        WriteMembers(obj, grouping.OrderBy(x => !x.Attrib.IsXmlAttribute).ToList(), 0, customMethods);
                    }
                    await _writer.WriteEndElementAsync();
                }
            }
            await _writer.WriteEndElementAsync();
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
                    x => string.Equals(member.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));
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
            switch (SerializationCommon.GetSerializeType(member.VariableType))
            {
                case SerializationCommon.SerializeType.Manual:
                    ((TFileObject)value).WriteAsync(_writer, _flags);
                    break;
                case SerializationCommon.SerializeType.Parsable:
                    _writer.WriteElementString(member.Name, ((IParsable)value).WriteToString());
                    break;
                case SerializationCommon.SerializeType.Array:
                    WriteArray(value as IList, member.Name, member.VariableType);
                    break;
                case SerializationCommon.SerializeType.Dictionary:
                    WriteDictionary(value as IDictionary, member);
                    break;
                case SerializationCommon.SerializeType.Enum:
                case SerializationCommon.SerializeType.String:
                    _writer.WriteElementString(member.Name, value.ToString());
                    break;
                case SerializationCommon.SerializeType.Struct:
                    List<VarInfo> structFields = SerializationCommon.CollectSerializedMembers(member.VariableType);
                    if (structFields.Count > 0)
                        await WriteObjectAsync(value, structFields, member.Name, writeTypeDefinition);
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
                case SerializationCommon.SerializeType.Pointer:
                    WriteObjectAsync(value, member.Name, writeTypeDefinition);
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
                switch (SerializationCommon.GetSerializeType(elementType))
                {
                    case SerializationCommon.SerializeType.Parsable:
                        WriteStringArray(array, true, false);
                        break;
                    case SerializationCommon.SerializeType.Array:
                        for (int i = 0; i < array.Count; ++i)
                            WriteArray(array[i] as IList, "Item", elementType);
                        break;
                    case SerializationCommon.SerializeType.Enum:
                        WriteStringArray(array, false, true);
                        break;
                    case SerializationCommon.SerializeType.String:
                        WriteStringArray(array, false, false);
                        break;
                    case SerializationCommon.SerializeType.Struct:
                        WriteStructArray(array, elementType);
                        break;
                    case SerializationCommon.SerializeType.Pointer:
                        foreach (object o in array)
                            WriteObjectAsync(o, elementName, o?.GetType() != elementType);
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
                    WriteObjectAsync(o, fields, elementName, false);
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
