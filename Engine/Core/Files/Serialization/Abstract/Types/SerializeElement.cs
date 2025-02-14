﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    public delegate void DelObjectChange(SerializeElement element, object previousObject);
    public sealed class SerializeElement : TObjectSlim
    {
        public event DelObjectChange ObjectChanged;

        public EventList<SerializeAttribute> Attributes { get; }
        public EventList<SerializeElement> Children { get; }
        public SerializeElementContent Content { get; }
        
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
        private BaseSerializationIO.BaseAbstractReaderWriter _manager;
        public BaseSerializationIO.BaseAbstractReaderWriter Manager
        {
            get => _manager;
            internal set
            {
                if (_manager != value)
                {
                    UnlinkObjectGuidFromOwner();
                    _manager = value;
                    ObjectTypeChanged();
                    LinkObjectGuidToOwner();
                }
                
                foreach (var child in Children)
                    child.Manager = _manager;
            }
        }
        private TSerializeMemberInfo _memberInfo;
        /// <summary>
        /// Information for this object as a member of the parent.
        /// </summary>
        public TSerializeMemberInfo MemberInfo
        {
            get => _memberInfo;
            set
            {
                TypeProxy t = ObjectType;
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
        //public bool IsSharedObject { get; internal set; }

        private TypeProxy _desiredDerivedObjectType;
        public TypeProxy DesiredDerivedObjectType
        {
            get => _desiredDerivedObjectType;
            private set
            {
                TypeProxy oldObjType = ObjectType;
                _desiredDerivedObjectType = value;
                if (ObjectType != oldObjType)
                    ObjectTypeChanged();
            }
        }

        private void UnlinkObjectGuidFromOwner()
        {
            if (!(_object is IObject tobj))
                return;

            Guid guid = tobj.Guid;
            if (guid == Guid.Empty || Manager is null || !Manager.WritingSharedObjectIndices.ContainsKey(guid))
                return;

            --Manager.WritingSharedObjectIndices[guid];
            if (Manager.WritingSharedObjectIndices[guid] > 0)
                return;

            Manager.WritingSharedObjectIndices.Remove(guid);
            if (Manager.WritingSharedObjects.ContainsKey(guid))
            {
                Manager.WritingSharedObjects.Remove(guid);
                //IsSharedObject = false;
            }
        }
        private void LinkObjectGuidToOwner()
        {
            if (!(_object is IObject tobj))
                return;

            Guid guid = tobj.Guid;
            if (guid == Guid.Empty || Manager is null)
                return;

            if (Manager.WritingSharedObjectIndices.ContainsKey(guid))
                ++Manager.WritingSharedObjectIndices[guid];
            else
            {
                Manager.WritingSharedObjectIndices.Add(guid, 1);
                if (!Manager.WritingSharedObjects.ContainsKey(guid))
                {
                    Manager.WritingSharedObjects.Add(guid, this);
                    //IsSharedObject = true;
                }
            }
        }

        private object _object;
        /// <summary>
        /// The value assigned to this member.
        /// </summary>
        public object Object
        {
            get => _object;
            set
            {
                TypeProxy oldObjType = ObjectType;

                object oldValue = _object;
                UnlinkObjectGuidFromOwner();

                _object = value;

                //TODO: tell the object it is being serialized or deserialized currently
                if (_object is IObject iobj)
                    iobj.ConstructedProgrammatically = false;

                if (Manager != null && Parent is null && _object != null)
                {
                    Manager.RootFileObject = _object;
                    if (_object is IFileObject fobj)
                    {
                        if (fobj.RootFile != Manager.RootFileObject)
                            fobj.RootFile = Manager.RootFileObject as IFileObject;
                        if (IsRoot)
                            fobj.FilePath = Manager.FilePath;
                    }
                }

                if (ObjectType != oldObjType)
                    ObjectTypeChanged();

                LinkObjectGuidToOwner();

                ObjectChanged?.Invoke(this, oldValue);
            }
        }

        /// <summary>
        /// True if this node is just for categorization of the child nodes.
        /// </summary>
        public bool IsGroupingNode => MemberInfo is null;
        /// <summary>
        /// How many checkpoints need to be hit for this node and all child nodes to advance the progression handler.
        /// </summary>
        public int ProgressionCount => 1 + Attributes.Count + Children.Count + (Content.IsNotNull ? 1 : 0);
        /// <summary>
        /// The type of the object assigned to this member.
        /// </summary>
        public TypeProxy ObjectType => _object?.GetTypeProxy() ?? DesiredDerivedObjectType ?? MemberInfo?.MemberType;
        /// <summary>
        /// <see langword="true"/> if the object's type inherits from the member's type instead of matching it exactly.
        /// </summary>
        public bool IsDerivedType => Parent is null || (ObjectType?.IsSubclassOf(MemberInfo.MemberType) ?? false);
        
        /// <summary>
        /// The class handling how to read and write all members of any given object.
        /// Most classes will use <see cref="CommonObjectSerializer"/>.
        /// </summary>
        public BaseObjectSerializer ObjectSerializer { get; set; }
        /// <summary>
        /// Methods for serializing data in a specific manner.
        /// </summary>
        public ConcurrentQueue<MethodInfoProxy> CustomSerializeMethods { get; private set; }
        public ConcurrentQueue<MethodInfoProxy> CustomDeserializeMethods { get; private set; }
        public ConcurrentQueue<MethodInfoProxy> PreSerializeMethods { get; private set; }
        public ConcurrentQueue<MethodInfoProxy> PostSerializeMethods { get; private set; }
        public ConcurrentQueue<MethodInfoProxy> PreDeserializeMethods { get; private set; }
        public ConcurrentQueue<MethodInfoProxy> PostDeserializeMethods { get; private set; }
        public enum ESerializeMethodType
        {
            CustomSerialize,
            CustomDeserialize,

            PreSerialize,
            PostSerialize,

            PreDeserialize,
            PostDeserialize,
        }
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
        
        public SerializeElement() : this(null, null) { }
        public SerializeElement(string name) : this(null, new TSerializeMemberInfo(null, name)) { }
        public SerializeElement(object obj, TSerializeMemberInfo memberInfo)
        {
            Parent = null;
            MemberInfo = memberInfo;
            Object = obj;

            Children = new EventList<SerializeElement>();
            Children.PostAnythingAdded += ChildElements_PostAnythingAdded;
            Children.PostAnythingRemoved += ChildElements_PostAnythingRemoved;

            Attributes = new EventList<SerializeAttribute>();
            Attributes.PostAnythingAdded += Attributes_PostAnythingAdded;
            Attributes.PostAnythingRemoved += Attributes_PostAnythingRemoved;

            Content = new SerializeElementContent { Parent = this };
            GenerateMethodEnqueueCache();
        }

        private void Attributes_PostAnythingRemoved(SerializeAttribute item)
        {
            if (item?.Parent == this)
                item.Parent = null;
        }
        private void Attributes_PostAnythingAdded(SerializeAttribute item)
        {
            if (item != null)
                item.Parent = this;
        }
        private void ChildElements_PostAnythingRemoved(SerializeElement item)
        {
            if (item?.Parent == this)
                item.Parent = null;
        }
        private void ChildElements_PostAnythingAdded(SerializeElement item)
        {
            if (item is null)
                return;
            item.Parent?.Children?.Remove(item);
            item.Manager = Manager;
            item.Parent = this;
        }

        /// <summary>
        /// Parent deserializes this node
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryInvokeManualParentDeserializeAsync()
        {
            if (MemberInfo is null || Parent?.CustomDeserializeMethods is null)
                return false;

            IEnumerable<MethodInfoProxy> customMethods = Parent.CustomDeserializeMethods.Where(
                x => string.Equals(MemberInfo.Name, x.GetCustomAttribute<CustomMemberDeserializeMethod>().Name));

            foreach (MethodInfoProxy customMethod in customMethods)
            {
                ParameterInfoProxy[] parameters = customMethod.GetParameters();
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
            if (ObjectType?.IsSubclassOf(typeof(TFileObject)) ?? false)
            {
                TFileExt ext = TFileObject.GetFileExtension(ObjectType);
                if (ext is null)
                    return false;

                bool serConfig = ext.ManualXmlConfigSerialize;
                bool serState = ext.ManualXmlStateSerialize;

                if (serConfig || serState)
                {
                    //Engine.PrintLine($"Deserializing {ObjectType.GetFriendlyName()} {Name} manually via self.");
                    Object = Activator.CreateInstance((Type)ObjectType);
                    ((IFileObject)Object).ManualRead(this);
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
            if (MemberInfo is null || Parent?.CustomSerializeMethods is null)
                return false;

            IEnumerable<MethodInfoProxy> customMethods = Parent.CustomSerializeMethods.Where(
                x => string.Equals(MemberInfo.Name, x.GetCustomAttribute<CustomMemberSerializeMethod>().Name));

            foreach (MethodInfoProxy customMethod in customMethods)
            {
                ParameterInfoProxy[] parameters = customMethod.GetParameters();
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
            if (!(Object is IFileObject tobj))
                return false;
            
            TFileExt ext = TFileObject.GetFileExtension(ObjectType);
            if (ext is null)
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
        public async Task<bool> DeserializeTreeToObjectAsync()
        {
            DesiredDerivedObjectType = 
                GetAttributeValue(SerializationCommon.TypeIdent, out string typeDeclaration) ? 
                    Engine.DomainProxy.GetTypeFor(typeDeclaration) : null;

            if (GetAttributeValue("SharedIndex", out int sharedObjectIndex))
            {
                if (Manager.ReadingSharedObjectsList.IndexInRange(sharedObjectIndex))
                {
                    Object = Manager.ReadingSharedObjectsList[sharedObjectIndex].Object;
                    ApplyObjectToParent();
                }
                else
                {
                    //var sharedObjElems = Owner.SharedObjectsElement.ChildElements;
                    if (Manager.ReadingSharedObjectsSetQueue.ContainsKey(sharedObjectIndex))
                        Manager.ReadingSharedObjectsSetQueue[sharedObjectIndex].Add(this);
                    else
                        Manager.ReadingSharedObjectsSetQueue.Add(sharedObjectIndex, new List<SerializeElement>() { this });
                    return false;
                }
            }
            else
            {
                //Parent overrides deserialization for this member?
                bool success = await TryInvokeManualParentDeserializeAsync();

                //This member manually deserializes itself?
                if (!success)
                    success = TryInvokeManualDeserializeAsync();

                if (!success)
                {
                    //Automatic deserialization
                    var ser = ObjectSerializer;
                    if (ser != null && !ObjectType.IsAbstract)
                        await ser.DeserializeTreeToObjectAsync();
                    ApplyObjectToParent();
                }
            }

            return true;
        }
        public async Task SerializeTreeFromObjectAsync()
        {
            await FileObjectCheckAsync();
            
            if (Object is null || IsObjectDefault())
                return;

            if (ObjectType.IsAbstract || ObjectType.IsInterface)
                throw new Exception();

            if (Manager != null && 
                Object is IObject tobj && 
                tobj.Guid != Guid.Empty &&
                Manager.WritingSharedObjectIndices.ContainsKey(tobj.Guid) &&
                Manager.WritingSharedObjectIndices[tobj.Guid] > 1)
                return;

            bool custom = await TryInvokeManualParentSerializeAsync();

            if (!custom)
                custom = TryInvokeManualSerializeAsync();

            var ser = ObjectSerializer;
            if (ser != null && !custom)
                await ser.SerializeTreeFromObjectAsync();
        }

        private void GenerateMethodEnqueueCache()
        {
            MethodEnqueueActions = new Action<MethodInfoProxy>[]
            {
                m => CustomSerializeMethods.Enqueue(m),
                m => CustomDeserializeMethods.Enqueue(m),

                m => PreSerializeMethods.Enqueue(m),
                m => PostSerializeMethods.Enqueue(m),

                m => PreDeserializeMethods.Enqueue(m),
                m => PostDeserializeMethods.Enqueue(m),
            };
        }

        private Action<MethodInfoProxy>[] MethodEnqueueActions { get; set; }

        private void ObjectTypeChanged()
        {
            CustomSerializeMethods = new ConcurrentQueue<MethodInfoProxy>();
            CustomDeserializeMethods = new ConcurrentQueue<MethodInfoProxy>();

            PreSerializeMethods = new ConcurrentQueue<MethodInfoProxy>();
            PostSerializeMethods = new ConcurrentQueue<MethodInfoProxy>();

            PreDeserializeMethods = new ConcurrentQueue<MethodInfoProxy>();
            PostDeserializeMethods = new ConcurrentQueue<MethodInfoProxy>();

            if (ObjectType != null)
            {
                //Debug.WriteLine($"Determining object serializer in {AppDomain.CurrentDomain.FriendlyName}");
                ObjectSerializer = BaseObjectSerializer.GetSerializerFor(ObjectType, false, false);
                ObjectSerializer.TreeNode = this;

                if (Manager != null)
                {
                    MethodInfoProxy[] methods = ObjectType?.GetMethods(
                        BindingFlags.NonPublic |
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.FlattenHierarchy);

                    Task task = Task.Run(() => methods.ForEachParallelArray(method =>
                    {
                        if (method.CanRunForFormat(Manager.Format, out ESerializeMethodType type))
                            MethodEnqueueActions[(int)type](method);
                    }));
                    
                    task.Wait(5000);
                }
            }
            else
                ObjectSerializer = null;

            DetermineDefaultObject();
        }
        public void ApplyObjectToParent()
        {
            object parent = Parent?.Object;
            if (parent != null)
                MemberInfo.SetObject(parent, _object);
        }
        public void DetermineDefaultObject()
        {
            //if (IsRoot)
            //{
                if (ObjectType != null && !(ObjectType.IsAbstract || ObjectType.IsInterface) && ObjectType.GetConstructors().Any(x => x.GetParameters().Length == 0))
                    DefaultObject = ObjectType.CreateInstance();
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
                FieldInfoProxy info = (FieldInfoProxy)MemberInfo.Member;
                if (!(Parent.DefaultObject is null))
                    DefaultMemberObject = info.GetValue(Parent.DefaultObject);
            }
            else if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfoProxy info = (PropertyInfoProxy)MemberInfo.Member;
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
                FieldInfoProxy info = (FieldInfoProxy)MemberInfo.Member;
                Object = info.GetValue(Parent.Object);
            }
            else if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfoProxy info = (PropertyInfoProxy)MemberInfo.Member;
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
                fobj.FilePath = Manager.FilePath;
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
                            if (fref is IGlobalFileRef && !Manager.Flags.HasFlag(ESerializeFlags.ExportGlobalRefs))
                                return;
                            if (fref is ILocalFileRef && !Manager.Flags.HasFlag(ESerializeFlags.ExportLocalRefs))
                                return;

                            string dir = path.Contains(".") ? Path.GetDirectoryName(path) : path;

                            IFileObject file = fref.File;
                            if (file.FileExtension != null)
                            {
                                string fileName = SerializationCommon.ResolveFileName(
                                    Manager.FileDirectory, file.Name, file.FileExtension.GetFullExtension(EProprietaryFileFormat.XML));

                                await file.ExportAsync(dir, fileName, Manager.Flags, EFileFormat.XML, null,null, CancellationToken.None);
                            }
                            else
                            {
                                TFileExt f = file.FileExtension;
                                if (f?.ExportableExtensions != null && f.ExportableExtensions.Length > 0)
                                {
                                    string ext = f.ExportableExtensions[0];
                                    string fileName = SerializationCommon.ResolveFileName(Manager.FileDirectory, file.Name, ext);
                                    await file.ExportAsync(dir, fileName, Manager.Flags, EFileFormat.ThirdParty, ext, null, CancellationToken.None);
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
            node.Content.SetValueAsObject(elementObject);
            Children.Add(node);
        }

        public void InsertAttribute(int index, string name, object value)
            => Attributes.Insert(index, new SerializeAttribute(name, value));
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
            => Children.FirstOrDefault(x => string.Equals(x.MemberInfo.Name, name));
        public override string ToString() => MemberInfo.Name;
        public bool GetChildElementObject<T>(string elementName, out T value)
        {
            SerializeElement node = GetChildElement(elementName);

            if (node != null)
                return node.Content.GetObjectAs(out value);

            value = default;
            return false;
        }
    }
}
