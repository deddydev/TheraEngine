﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    public sealed class MemberTreeNode
    {
        private TBaseSerializer.TBaseAbstractReaderWriter _owner;
        private object _object;
        private Type _desiredDerivedObjectType;
        private TSerializeMemberInfo _memberInfo;

        public List<SerializeAttribute> ChildAttributeMembers { get; set; } = new List<SerializeAttribute>();
        public EventList<MemberTreeNode> ChildElementMembers { get; set; }
        private SerializeAttribute _elementContent = new SerializeAttribute();

        public bool IsElementContentNonStringObject => _elementContent.IsNonStringObject;
        public bool IsElementContentUnparsedString => _elementContent.IsUnparsedString;

        //public bool ParseElementContentStringToObject(Type type)
        //    => _elementContent.ParseStringToObject(type);
        public bool GetElementContent(Type expectedType, out object value)
            => _elementContent.GetObject(expectedType, out value);
        public bool GetElementContentAs<T>(out T value)
            => _elementContent.GetObjectAs(out value);
        public bool GetElementContentAsString(out string value)
            => _elementContent.GetString(out value);
        public void SetElementContent(object obj)
            => _elementContent.SetValueAsObject(obj);
        internal void SetElementContentAsString(string str)
            => _elementContent.SetValueAsString(str);
        internal bool SetElementContentAsString(string str, Type type)
            => _elementContent.SetValueAsString(str, type);


        public MemberTreeNode Parent { get; internal set; }
        public TBaseSerializer.TBaseAbstractReaderWriter Owner
        {
            get => _owner;
            internal set
            {
                if (_owner != value)
                {
                    _owner = value;
                    ObjectTypeChanged();
                }
                foreach (var child in ChildElementMembers)
                    child.Owner = _owner;
            }
        }
        public TSerializeMemberInfo MemberInfo
        {
            get => _memberInfo;
            set
            {
                Type t = ObjectType;
                if (_memberInfo != null)
                    _memberInfo.MemberTypeChanged -= _memberInfo_MemberTypeChanged;
                _memberInfo = value;
                if (_memberInfo != null)
                    _memberInfo.MemberTypeChanged += _memberInfo_MemberTypeChanged;
                if (ObjectType != t)
                    ObjectTypeChanged();
            }
        }

        private void _memberInfo_MemberTypeChanged()
        {
            ObjectTypeChanged();
        }

        internal int CalculatedSize { get; set; }
        internal int ManuallyCalculatedSize { get; set; }
        internal byte[] ParsableBytes { get; set; } = null;
        internal string ParsableString { get; set; } = null;
        internal int ParsablePointerSize { get; set; } = 0;

        public object DefaultObject { get; private set; }
        public Type DesiredDerivedObjectType
        {
            get => _desiredDerivedObjectType;
            private set
            {
                Type t = ObjectType;
                _desiredDerivedObjectType = value;
                if (ObjectType != t)
                    ObjectTypeChanged();
            }
        }

        /// <summary>
        /// The value assigned to this member.
        /// </summary>
        public object Object
        {
            get => _object;
            set
            {
                Type t = ObjectType;
                _object = value;
                if (ObjectType != t)
                    ObjectTypeChanged();
            }
        }

        /// <summary>
        /// True if this node is just for categorization of the child nodes.
        /// </summary>
        public bool IsGroupingNode => MemberInfo == null;
        /// <summary>
        /// How many checkpoints need to be hit for this node and all child nodes to advance the progression handler.
        /// </summary>
        public int ProgressionCount => 1 + ChildAttributeMembers.Count + ChildElementMembers.Count + (_elementContent.IsNotNull ? 1 : 0);
        /// <summary>
        /// The type of the object assigned to this member.
        /// </summary>
        public Type ObjectType => _object?.GetType() ?? DesiredDerivedObjectType ?? MemberInfo?.MemberType;
        /// <summary>
        /// <see langword="true"/> if the object's type inherits from the member's type instead of matching it exactly.
        /// </summary>
        public bool IsDerivedType => Parent == null || (ObjectType?.IsSubclassOf(MemberInfo.MemberType) ?? false);
        
        /// <summary>
        /// The class handling how to read and write all members of any given object.
        /// Most classes will use <see cref="CommonObjectSerializer"/>.
        /// </summary>
        public BaseObjectSerializer ObjectSerializer { get; set; }
        /// <summary>
        /// Methods for serializing data in a specific manner.
        /// </summary>
        public List<MethodInfo> CustomSerializeMethods { get; private set; }
        public List<MethodInfo> CustomDeserializeMethods { get; private set; }
        public List<MethodInfo> PreSerializeMethods { get; private set; }
        public List<MethodInfo> PostSerializeMethods { get; private set; }
        public List<MethodInfo> PreDeserializeMethods { get; private set; }
        public List<MethodInfo> PostDeserializeMethods { get; private set; }
        public string Name
        {
            get => MemberInfo?.Name;
            set
            {
                if (MemberInfo != null)
                    MemberInfo.Name = value;
            }
        }

        public MemberTreeNode()
        {
            Parent = null;
            MemberInfo = null;
            Object = null;
            DefaultObject = null;
            ChildElementMembers = new EventList<MemberTreeNode>();
            ChildElementMembers.PostAnythingAdded += ChildElements_PostAnythingAdded;
            ChildElementMembers.PostAnythingRemoved += ChildElements_PostAnythingRemoved;
        }

        private void ChildElements_PostAnythingRemoved(MemberTreeNode item)
        {
            if (item.Parent == this)
                item.Parent = null;
        }
        private void ChildElements_PostAnythingAdded(MemberTreeNode item)
        {
            if (item.Parent != null)
                item.Parent.ChildElementMembers.Remove(item);
            item.Parent = this;
            item.Owner = Owner;
        }
        
        public MemberTreeNode(object obj, TSerializeMemberInfo memberInfo)
        {
            Object = obj;
            Parent = null;
            MemberInfo = memberInfo;
            DefaultObject = ObjectType == null ? null : SerializationCommon.CreateObject(ObjectType);
            ChildElementMembers = new EventList<MemberTreeNode>();
            ChildElementMembers.PostAnythingAdded += ChildElements_PostAnythingAdded;
            ChildElementMembers.PostAnythingRemoved += ChildElements_PostAnythingRemoved;
        }
        /// <summary>
        /// Parent deserializes this node
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryInvokeManualParentDeserializeAsync()
        {
            if (MemberInfo == null || Parent?.CustomDeserializeMethods == null)
                return false;

            var customMethods = Parent.CustomDeserializeMethods.Where(
                x => string.Equals(MemberInfo.Name, x.GetCustomAttribute<CustomDeserializeMethod>().Name));

            foreach (var customMethod in customMethods)
            {
                if (customMethod == null)
                    continue;

                var parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(GetType()))
                {
                    if (customMethod.ReturnType == typeof(Task))
                        await (Task)customMethod.Invoke(Parent.Object, new object[] { this });
                    else
                        customMethod.Invoke(Parent.Object, new object[] { this });
                    //This method will set a member of the parent object
                    //Retrieve it here
                    RetrieveObjectFromParent();
                    return true;
                }
                else
                {
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a {nameof(CustomDeserializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {nameof(MemberTreeNode)}.");
                }
            }

            return false;
        }
        /// <summary>
        /// This node deserializes itself
        /// </summary>
        /// <returns></returns>
        public bool TryInvokeManualDeserializeAsync()
        {
            if (ObjectType.IsSubclassOf(typeof(TFileObject)))
            {
                FileExt ext = TFileObject.GetFileExtension(ObjectType);
                if (ext == null)
                    return false;

                bool serConfig = ext.ManualXmlConfigSerialize;
                bool serState = ext.ManualXmlStateSerialize;

                if (serConfig || serState)
                {
                    Object = SerializationCommon.CreateObject(ObjectType);
                    ((TFileObject)Object).ManualRead(this);
                    ApplyObjectToParent();
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Parent serializes this node
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryInvokeManualParentSerializeAsync()
        {
            if (MemberInfo == null || Parent?.CustomSerializeMethods == null)
                return false;

            var customMethods = Parent.CustomSerializeMethods.Where(
                x => string.Equals(MemberInfo.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));

            foreach (var customMethod in customMethods)
            {
                if (customMethod == null)
                    continue;

                var parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(GetType()))
                {
                    if (customMethod.ReturnType == typeof(Task))
                        await (Task)customMethod.Invoke(Parent.Object, new object[] { this });
                    else
                        customMethod.Invoke(Parent.Object, new object[] { this });
                    return true;
                }
                else
                {
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a {nameof(CustomSerializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {nameof(MemberTreeNode)}.");
                }
            }

            return false;
        }
        /// <summary>
        /// This node serializes itself
        /// </summary>
        /// <returns></returns>
        public bool TryInvokeManualSerializeAsync()
        {
            if (Object is TFileObject tobj)
            {
                FileExt ext = TFileObject.GetFileExtension(ObjectType);
                if (ext == null)
                    return false;

                bool serConfig = ext.ManualXmlConfigSerialize;
                bool serState = ext.ManualXmlStateSerialize;

                if (serConfig || serState)
                {
                    tobj.ManualWrite(this);
                    return true;
                }
            }

            return false;
        }
        public async void TreeToObject()
        {
            if (GetAttributeValue(SerializationCommon.TypeIdent, out string typeDeclaration))
                DesiredDerivedObjectType = SerializationCommon.CreateType(typeDeclaration);
            else
                DesiredDerivedObjectType = null;

            bool custom = await TryInvokeManualParentDeserializeAsync();
            if (!custom)
                custom = TryInvokeManualDeserializeAsync();
            if (!custom)
            {
                ObjectSerializer?.TreeToObject();
                ApplyObjectToParent();
            }
        }
        public async void TreeFromObject()
        {
            await FileObjectCheckAsync();

            if (ObjectType.IsSubclassOf(MemberInfo.MemberType))
                AddAttribute(SerializationCommon.TypeIdent, ObjectType.AssemblyQualifiedName);

            bool custom = await TryInvokeManualParentSerializeAsync();
            if (!custom)
                custom = TryInvokeManualSerializeAsync();
            if (!custom)
                ObjectSerializer?.TreeFromObject();
        }
        private void ObjectTypeChanged()
        {
            PreSerializeMethods = new List<MethodInfo>();
            PostSerializeMethods = new List<MethodInfo>();
            PreDeserializeMethods = new List<MethodInfo>();
            PostDeserializeMethods = new List<MethodInfo>();
            CustomSerializeMethods = new List<MethodInfo>();
            CustomDeserializeMethods = new List<MethodInfo>();

            if (ObjectType != null)
            {
                ObjectSerializer = SerializationCommon.DetermineObjectSerializer(ObjectType, this);

                IEnumerable<MethodInfo> methods = ObjectType?.GetMethods(
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.FlattenHierarchy);
                foreach (MethodInfo m in methods)
                {
                    var attribs = m.GetCustomAttributes();
                    foreach (var attrib in attribs)
                    {
                        if (attrib is SerializationAttribute serAttrib &&
                            (Owner != null ? serAttrib.RunForFormats.HasFlag(Owner.Format) : true))
                        {
                            switch (attrib.GetType().Name)
                            {
                                case nameof(PreDeserialize):
                                    PreDeserializeMethods.Add(m);
                                    break;
                                case nameof(PostDeserialize):
                                    PostDeserializeMethods.Add(m);
                                    break;
                                case nameof(PreSerialize):
                                    PreSerializeMethods.Add(m);
                                    break;
                                case nameof(PostSerialize):
                                    PostSerializeMethods.Add(m);
                                    break;
                                case nameof(CustomSerializeMethod):
                                    CustomSerializeMethods.Add(m);
                                    break;
                                case nameof(CustomDeserializeMethod):
                                    CustomDeserializeMethods.Add(m);
                                    break;
                            }
                        }
                    }
                }
            }
            else
                ObjectSerializer = null;
        }
        public void ApplyObjectToParent()
        {
            if (Parent != null)
                MemberInfo.SetObject(Parent.Object, _object);
        }
        public void RetrieveObjectFromParent()
        {
            DefaultObject = null;
            if (Parent?.Object is null || MemberInfo?.Member is null)
            {
                Object = null;
                return;
            }
            
            if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Field))
            {
                FieldInfo info = (FieldInfo)MemberInfo.Member;
                Object = info.GetValue(Parent.Object);
                if (!(Parent.DefaultObject is null))
                    DefaultObject = info.GetValue(Parent.DefaultObject);
            }
            else if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo info = (PropertyInfo)MemberInfo.Member;
                if (info.CanRead)
                {
                    Object = info.GetValue(Parent.Object);
                    if (!(Parent.DefaultObject is null))
                        DefaultObject = info.GetValue(Parent.DefaultObject);
                }
                else
                {
                    Engine.LogWarning($"Can't read property {info.Name} in {info.DeclaringType.GetFriendlyName()}.");
                }
            }
            else
            {
                Engine.LogWarning($"Member {MemberInfo.Name} is not a field or property.");
            }
        }
        /// <summary>
        /// Performs special processing for classes that implement <see cref="IFileObject"/> and <see cref="IFileRef"/>.
        /// </summary>
        private async Task FileObjectCheckAsync()
        {
            //Update the object's file path
            if (Object is IFileObject fobj)
            {
                fobj.FilePath = Owner.FilePath;
                if (fobj is IFileRef fref && !fref.StoredInternally)
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

                        string root2 = Path.GetPathRoot(Owner.FileDirectory);
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
                            if (fref is IGlobalFileRef && !Owner.Flags.HasFlag(ESerializeFlags.ExportGlobalRefs))
                                return;
                            if (fref is ILocalFileRef && !Owner.Flags.HasFlag(ESerializeFlags.ExportLocalRefs))
                                return;

                            string absPath;
                            if (fref.PathType == EPathType.FileRelative)
                            {
                                string rel = fref.ReferencePathAbsolute.MakePathRelativeTo(Owner.FileDirectory);
                                absPath = Path.GetFullPath(Path.Combine(Owner.FileDirectory, rel));
                                //fref.ReferencePathRelative = absPath.MakePathRelativeTo(_fileDir);
                            }
                            else
                                absPath = fref.ReferencePathAbsolute;

                            string dir = absPath.Contains(".") ? Path.GetDirectoryName(absPath) : absPath;

                            IFileObject file = fref.File;
                            if (file.FileExtension != null)
                            {
                                string fileName = SerializationCommon.ResolveFileName(
                                    Owner.FileDirectory, file.Name, file.FileExtension.GetProperExtension(EProprietaryFileFormat.XML));
                                await file.ExportAsync(dir, fileName, EFileFormat.XML, null, Owner.Flags, null, CancellationToken.None);
                            }
                            else
                            {
                                var f = file.File3rdPartyExtensions;
                                if (f != null && f.ExportableExtensions != null && f.ExportableExtensions.Length > 0)
                                {
                                    string ext = f.ExportableExtensions[0];
                                    string fileName = SerializationCommon.ResolveFileName(Owner.FileDirectory, file.Name, ext);
                                    await file.ExportAsync(dir, fileName, EFileFormat.ThirdParty, ext, Owner.Flags, null, CancellationToken.None);
                                }
                                else
                                    Engine.LogWarning("Cannot export " + file.GetType().GetFriendlyName());
                            }
                        }
                    }
                }
            }
        }

        public void AddChildElementObject(string elementName, object elementObject)
        {
            MemberTreeNode node = new MemberTreeNode(null, new TSerializeMemberInfo(typeof(string), elementName));
            node.SetElementContent(elementObject);
            ChildElementMembers.Add(node);
        }
        public void AddAttribute(string name, object value)
            => ChildAttributeMembers.Add(new SerializeAttribute(name, value));
        public SerializeAttribute GetAttribute(string name) 
            => ChildAttributeMembers.FirstOrDefault(x => string.Equals(x.Name, name));
        /// <summary>
        /// Retrieves the attribute at the given index.
        /// </summary>
        /// <param name="index">The index of the attribute to retrieve.</param>
        /// <returns>The attribute at the specified index. If the index is out of range, returns null.</returns>
        public SerializeAttribute GetAttribute(int index)
            => ChildAttributeMembers.IndexInRange(index) ? ChildAttributeMembers[index] : null;
        /// <summary>
        /// Retrieves the value of an attribute for this node.
        /// </summary>
        /// <typeparam name="T">The value's expected type.</typeparam>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The returned value of the attribute, if succeeded.</param>
        /// <returns>True if the attribute was found and could be converted to the expected type.</returns>
        public bool GetAttributeValue<T>(string name, out T value)
        {
            SerializeAttribute attrib = GetAttribute(name);
            if (attrib != null)
                return attrib.GetObjectAs(out value);
            value = default;
            return false;
        }
        public bool GetAttributeValue<T>(int index, out T value)
        {
            SerializeAttribute attrib = GetAttribute(index);
            if (attrib != null)
                return attrib.GetObjectAs(out value);
            value = default;
            return false;
        }
        public MemberTreeNode GetChildElement(string name)
            => ChildElementMembers.FirstOrDefault(x => string.Equals(x.MemberInfo.Name, name));
        public override string ToString() => MemberInfo.Name;
    }
    public class SerializeAttribute
    {
        public SerializeAttribute() { }
        public SerializeAttribute(string name, object value) { Name = name; _value = value; }

        private object _value;
        private string _stringValue;
        private Type _valueType;

        public string Name { get; set; }

        public bool IsNotNull => _value != null || _stringValue != null;
        public void SetValueAsObject(object o)
        {
            _value = o;
            _valueType = _value?.GetType();
            IsNonStringObject = _value != null ? !SerializationCommon.GetString(_value, _valueType, out _stringValue) : false;
        }
        public void SetValueAsString(string o)
        {
            _stringValue = o;
            _valueType = null;
            IsNonStringObject = false;
        }
        public bool SetValueAsString(string o, Type type)
        {
            _stringValue = o;
            _valueType = null;
            IsNonStringObject = false;
            return ParseStringToObject(type);
        }

        public bool IsNonStringObject { get; private set; } = false;
        public bool IsUnparsedString => _valueType == null;

        private bool ParseStringToObject(Type type)
        {
            bool canParse = SerializationCommon.CanParseAsString(type);
            if (canParse)
            {
                _value = SerializationCommon.ParseString(_stringValue, type);
                _valueType = type;
            }
            else
                _valueType = null;
            IsNonStringObject = false;
            return canParse;
        }
        public bool GetObject(Type expectedType, out object value)
        {
            if (expectedType == null)
            {
                value = _value;
                return _value != null && !IsUnparsedString;
            }

            bool success = IsUnparsedString ?
                ParseStringToObject(expectedType) : 
                (_value == null ? true : expectedType.IsAssignableFrom(_value.GetType()));

            if (success)
                value = _value;
            else
                value = default;

            return success;
        }
        public bool GetObjectAs<T>(out T value)
        {
            bool success = IsUnparsedString ? ParseStringToObject(typeof(T)) : _value is T;
            if (success)
                value = (T)_value;
            else
                value = default;
            return success;
        }
        public bool GetString(out string value)
        {
            if (IsNonStringObject)
            {
                value = null;
                return false;
            }
            value = _stringValue;
            return true;
        }

        public static SerializeAttribute FromString(string name, string value)
        {
            SerializeAttribute attrib = new SerializeAttribute(name, null);
            attrib.SetValueAsString(value);
            return attrib;
        }
        public static SerializeAttribute FromString(string name, string value, Type objectType, out bool parseSucceeded)
        {
            SerializeAttribute attrib = new SerializeAttribute(name, null);
            parseSucceeded = attrib.SetValueAsString(value, objectType);
            return attrib;
        }
    }
}
