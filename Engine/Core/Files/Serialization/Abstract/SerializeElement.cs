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
    public sealed class SerializeElement
    {
        private TBaseSerializer.TBaseAbstractReaderWriter _owner;
        private object _object;
        private Type _desiredDerivedObjectType;
        private TSerializeMemberInfo _memberInfo;

        public List<SerializeAttribute> Attributes { get; } = new List<SerializeAttribute>();
        public EventList<SerializeElement> ChildElements { get; }
        internal SerializeElementContent _elementContent = new SerializeElementContent();

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
        /// <summary>
        /// Attempts to set the element content as an object.
        /// If unable to serialize the object as a string, returns false.
        /// </summary>
        public bool SetElementContent(object obj)
            => _elementContent.SetValueAsObject(obj);
        internal void SetElementContentAsString(string str)
            => _elementContent.SetValueAsString(str);
        internal bool SetElementContentAsString(string str, Type type)
            => _elementContent.SetValueAsString(str, type);

        private SerializeElement _parent;
        public SerializeElement Parent
        {
            get => _parent;
            internal set
            {
                _parent = value;
                DetermineDefaultObject();
            }
        }
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
                foreach (var child in ChildElements)
                    child.Owner = _owner;
            }
        }
        /// <summary>
        /// Information for this object as a member of the parent.
        /// </summary>
        public TSerializeMemberInfo MemberInfo
        {
            get => _memberInfo;
            set
            {
                Type t = ObjectType;
                if (_memberInfo != null)
                    _memberInfo.MemberTypeChanged -= _memberInfo_MemberTypeChanged;
                _memberInfo = value ?? new TSerializeMemberInfo(null, "null");
                //if (_memberInfo != null)
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
        public object DefaultMemberObject { get; private set; }

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
                Type oldObjType = ObjectType;

                if (_object is TObject tobj && tobj.Guid != Guid.Empty)
                {
                    if (Owner != null && Owner.SharedObjectIndices.ContainsKey(tobj.Guid))
                    {
                        --Owner.SharedObjectIndices[tobj.Guid];
                        if (Owner.SharedObjectIndices[tobj.Guid] <= 0)
                        {
                            Owner.SharedObjectIndices.Remove(tobj.Guid);
                            if (Owner.SharedObjects.ContainsKey(tobj.Guid))
                                Owner.SharedObjects.Remove(tobj.Guid);
                        }
                    }
                }

                _object = value;

                if (ObjectType != oldObjType)
                    ObjectTypeChanged();

                if (Owner != null && _object is TObject tobj2 && tobj2.Guid != Guid.Empty)
                {
                    if (Owner.SharedObjectIndices.ContainsKey(tobj2.Guid))
                        ++Owner.SharedObjectIndices[tobj2.Guid];
                    else
                    {
                        Owner.SharedObjectIndices.Add(tobj2.Guid, 1);
                        if (!Owner.SharedObjects.ContainsKey(tobj2.Guid))
                            Owner.SharedObjects.Add(tobj2.Guid, this);
                    }
                }
            }
        }

        /// <summary>
        /// True if this node is just for categorization of the child nodes.
        /// </summary>
        public bool IsGroupingNode => MemberInfo == null;
        /// <summary>
        /// How many checkpoints need to be hit for this node and all child nodes to advance the progression handler.
        /// </summary>
        public int ProgressionCount => 1 + Attributes.Count + ChildElements.Count + (_elementContent.IsNotNull ? 1 : 0);
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
                else
                    MemberInfo = new TSerializeMemberInfo(null, value);
            }
        }

        /// <summary>
        /// Signifies that this element represents the object containing everything else.
        /// </summary>
        public bool IsRoot
        {
            get => _isRoot;
            internal set
            {
                _isRoot = value;
                DetermineDefaultObject();
            }
        }
        private bool _isRoot = false;
        
        public bool IsObjectDefault()
            => CommonObjectSerializer.IsObjectDefault(Object, DefaultMemberObject);
        
        public SerializeElement()
        {
            Parent = null;
            MemberInfo = null;
            Object = null;
            ChildElements = new EventList<SerializeElement>();
            ChildElements.PostAnythingAdded += ChildElements_PostAnythingAdded;
            ChildElements.PostAnythingRemoved += ChildElements_PostAnythingRemoved;
        }
        public SerializeElement(string name) : this(null, new TSerializeMemberInfo(null, name)) { }
        public SerializeElement(object obj, TSerializeMemberInfo memberInfo)
        {
            Parent = null;
            MemberInfo = memberInfo;
            Object = obj;
            ChildElements = new EventList<SerializeElement>();
            ChildElements.PostAnythingAdded += ChildElements_PostAnythingAdded;
            ChildElements.PostAnythingRemoved += ChildElements_PostAnythingRemoved;
        }
        private void ChildElements_PostAnythingRemoved(SerializeElement item)
        {
            if (item.Parent == this)
                item.Parent = null;
        }
        private void ChildElements_PostAnythingAdded(SerializeElement item)
        {
            item.Parent?.ChildElements?.Remove(item);
            item.Owner = Owner;
            item.Parent = this;
        }
        /// <summary>
        /// Parent deserializes this node
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryInvokeManualParentDeserializeAsync()
        {
            if (MemberInfo == null || Parent?.CustomDeserializeMethods == null)
                return false;

            IEnumerable<MethodInfo> customMethods = Parent.CustomDeserializeMethods.Where(
                x => string.Equals(MemberInfo.Name, x.GetCustomAttribute<CustomMemberDeserializeMethod>().Name));

            foreach (MethodInfo customMethod in customMethods)
            {
                ParameterInfo[] parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(GetType()))
                {
                    //Engine.PrintLine($"Deserializing {ObjectType.GetFriendlyName()} {Name} manually via parent.");

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
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a {nameof(CustomMemberDeserializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {nameof(SerializeElement)}.");
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
                TFileExt ext = TFileObject.GetFileExtension(ObjectType);
                if (ext == null)
                    return false;

                bool serConfig = ext.ManualXmlConfigSerialize;
                bool serState = ext.ManualXmlStateSerialize;

                if (serConfig || serState)
                {
                    //Engine.PrintLine($"Deserializing {ObjectType.GetFriendlyName()} {Name} manually via self.");
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

            IEnumerable<MethodInfo> customMethods = Parent.CustomSerializeMethods.Where(
                x => string.Equals(MemberInfo.Name, x.GetCustomAttribute<CustomMemberSerializeMethod>().Name));

            foreach (MethodInfo customMethod in customMethods)
            {
                ParameterInfo[] parameters = customMethod.GetParameters();
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
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a {nameof(CustomMemberSerializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {nameof(SerializeElement)}.");
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
            if (!(Object is TFileObject tobj))
                return false;
            
            TFileExt ext = TFileObject.GetFileExtension(ObjectType);
            if (ext == null)
                return false;

            bool serConfig = ext.ManualXmlConfigSerialize;
            bool serState = ext.ManualXmlStateSerialize;

            if (!serConfig && !serState)
                return false;

            tobj.ManualWrite(this);
            return true;

        }
        /// <summary>
        /// Creates this object for this element 
        /// using this node's element content, attributes and child elements.
        /// If this node's MemberInfo is set, 
        /// this method also applies the deserialized object to the parent.
        /// </summary>
        public async void DeserializeTreeToObject()
        {
            DesiredDerivedObjectType = 
                GetAttributeValue(SerializationCommon.TypeIdent, out string typeDeclaration) ? 
                    SerializationCommon.CreateType(typeDeclaration) : 
                    null;
            
            bool custom = await TryInvokeManualParentDeserializeAsync();
            if (!custom)
                custom = TryInvokeManualDeserializeAsync();
            if (custom)
                return;
            //Engine.PrintLine($"Deserializing {ObjectType.GetFriendlyName()} {Name} normally.");
            ObjectSerializer?.DeserializeTreeToObject();
            ApplyObjectToParent();
        }
        public async void SerializeTreeFromObject()
        {
            await FileObjectCheckAsync();

            if (ObjectType != MemberInfo.MemberType || IsRoot)
                AddAttribute(SerializationCommon.TypeIdent, ObjectType.AssemblyQualifiedName);

            if (Object == null || IsObjectDefault())
                return;

            if (ObjectType.IsAbstract || ObjectType.IsInterface)
                throw new Exception();

            if (Owner != null && 
                Object is TObject tobj && 
                tobj.Guid != Guid.Empty &&
                Owner.SharedObjectIndices.ContainsKey(tobj.Guid) &&
                Owner.SharedObjectIndices[tobj.Guid] > 1)
                return;

            bool custom = await TryInvokeManualParentSerializeAsync();
            if (!custom)
                custom = TryInvokeManualSerializeAsync();
            if (!custom)
                ObjectSerializer?.SerializeTreeFromObject();
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
                ObjectSerializer = BaseObjectSerializer.DetermineObjectSerializer(ObjectType, false, false);
                ObjectSerializer.TreeNode = this;

                IEnumerable<MethodInfo> methods = ObjectType?.GetMethods(
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.FlattenHierarchy);
                foreach (MethodInfo m in methods)
                {
                    IEnumerable<Attribute> attribs = m.GetCustomAttributes();
                    foreach (Attribute attrib in attribs)
                    {
                        if (!(attrib is SerializationAttribute serAttrib) || (Owner != null && !serAttrib.RunForFormats.HasFlag(Owner.Format)))
                            continue;
                        switch (attrib)
                        {
                            case TPreDeserialize _:
                                PreDeserializeMethods.Add(m);
                                break;
                            case TPostDeserialize _:
                                PostDeserializeMethods.Add(m);
                                break;
                            case TPreSerialize _:
                                PreSerializeMethods.Add(m);
                                break;
                            case TPostSerialize _:
                                PostSerializeMethods.Add(m);
                                break;
                            case CustomMemberSerializeMethod _:
                                CustomSerializeMethods.Add(m);
                                break;
                            case CustomMemberDeserializeMethod _:
                                CustomDeserializeMethods.Add(m);
                                break;
                        }
                    }
                }
            }
            else
                ObjectSerializer = null;

            DetermineDefaultObject();
        }
        public void ApplyObjectToParent()
        {
            if (Parent?.Object != null)
                MemberInfo.SetObject(Parent.Object, _object);
        }
        public void DetermineDefaultObject()
        {
            //if (IsRoot)
            //{
                if (ObjectType != null && !(ObjectType.IsAbstract || ObjectType.IsInterface) && ObjectType.GetConstructors().Any(x => x.GetParameters().Length == 0))
                    DefaultObject = SerializationCommon.CreateObject(ObjectType);
                else
                    DefaultObject = null;
            //    return;
            //}

            //DefaultObject = ObjectType?.GetDefaultValue() ?? null;
            if (Parent is null || MemberInfo?.Member is null)
            {
                DefaultMemberObject = ObjectType?.GetDefaultValue() ?? null;
                return;
            }

            if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Field))
            {
                FieldInfo info = (FieldInfo)MemberInfo.Member;
                if (!(Parent.DefaultObject is null))
                    DefaultMemberObject = info.GetValue(Parent.DefaultObject);
            }
            else if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo info = (PropertyInfo)MemberInfo.Member;
                if (info.CanRead && !(Parent.DefaultObject is null))
                    DefaultMemberObject = info.GetValue(Parent.DefaultObject);
            }
            else
                DefaultMemberObject = ObjectType?.GetDefaultValue() ?? null;
        }
        public void RetrieveObjectFromParent()
        {
            if (Parent?.Object is null || MemberInfo?.Member is null)
            {
                Object = null;
                DetermineDefaultObject();
                return;
            }
            
            if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Field))
            {
                FieldInfo info = (FieldInfo)MemberInfo.Member;
                Object = info.GetValue(Parent.Object);
            }
            else if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo info = (PropertyInfo)MemberInfo.Member;
                if (info.CanRead)
                    Object = info.GetValue(Parent.Object);
                else
                    Engine.LogWarning($"Can't read property {info.Name} in {info.DeclaringType.GetFriendlyName()}.");
            }
            else
                Engine.LogWarning($"Member {MemberInfo.Name} is not a field or property.");

            ObjectTypeChanged();
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
                    //if (fref.Path.Type == EPathType.FileRelative)
                    //{
                    //    string root = Path.GetPathRoot(fref.Path.Path);
                    //    int colonIndex = root.IndexOf(":");
                    //    if (colonIndex > 0)
                    //        root = root.Substring(0, colonIndex);
                    //    else
                    //        root = string.Empty;

                    //    string root2 = Path.GetPathRoot(Owner.FileDirectory);
                    //    colonIndex = root2.IndexOf(":");
                    //    if (colonIndex > 0)
                    //        root2 = root2.Substring(0, colonIndex);
                    //    else
                    //        root2 = string.Empty;

                    //    if (!string.Equals(root, root2))
                    //    {
                    //        //Totally different drives, cannot be relative in any way
                    //        fref.Path.Type = EPathType.Absolute;
                    //    }
                    //}
                    if (fref.IsLoaded)
                    {
                        string path = fref.Path.Path;

                        //TODO: export even if the file exists,
                        //however only if the file has changed
                        if (path.IsValidPath() && !File.Exists(path))
                        {
                            if (fref is IGlobalFileRef && !Owner.Flags.HasFlag(ESerializeFlags.ExportGlobalRefs))
                                return;
                            if (fref is ILocalFileRef && !Owner.Flags.HasFlag(ESerializeFlags.ExportLocalRefs))
                                return;

                            string dir = path.Contains(".") ? Path.GetDirectoryName(path) : path;

                            IFileObject file = fref.File;
                            if (file.FileExtension != null)
                            {
                                string fileName = SerializationCommon.ResolveFileName(
                                    Owner.FileDirectory, file.Name, file.FileExtension.GetFullExtension(EProprietaryFileFormat.XML));

                                await file.ExportAsync(dir, fileName, EFileFormat.XML, null, Owner.Flags, null, CancellationToken.None);
                            }
                            else
                            {
                                TFileExt f = file.FileExtension;
                                if (f?.ExportableExtensions != null && f.ExportableExtensions.Length > 0)
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
            SerializeElement node = new SerializeElement(null, new TSerializeMemberInfo(null, elementName));
            node.SetElementContent(elementObject);
            ChildElements.Add(node);
        }
        public void AddAttribute(string name, object value)
            => Attributes.Add(new SerializeAttribute(name, value));
        public SerializeAttribute GetAttribute(string name) 
            => Attributes.FirstOrDefault(x => string.Equals(x.Name, name));
        /// <summary>
        /// Retrieves the attribute at the given index.
        /// </summary>
        /// <param name="index">The index of the attribute to retrieve.</param>
        /// <returns>The attribute at the specified index. If the index is out of range, returns null.</returns>
        public SerializeAttribute GetAttribute(int index)
            => Attributes.IndexInRange(index) ? Attributes[index] : null;
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
        public SerializeElement GetChildElement(string name)
            => ChildElements.FirstOrDefault(x => string.Equals(x.MemberInfo.Name, name));
        public override string ToString() => MemberInfo.Name;
        public bool GetChildElementObject<T>(string elementName, out T value)
        {
            SerializeElement node = GetChildElement(elementName);
            if (node == null)
            {
                value = default;
                return false;
            }
            return node.GetElementContentAs(out value);
        }
    }
}
