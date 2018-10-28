using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    public class SerializeAttribute
    {
        public SerializeAttribute() { }
        public SerializeAttribute(string name, object value) { Name = name; _value = value; }

        private object _value;
        private string _stringValue;

        public string Name { get; set; }

        public void SetValueAsObject(object o)
        {
            _value = o;
            Type t = _value?.GetType();
            IsNonStringObject = _value != null ? !SerializationCommon.GetString(_value, t, out _stringValue) : false;
            IsUnparsedString = false;
        }
        public void SetValueAsString(string o)
        {
            _stringValue = o;
            IsUnparsedString = true;
            IsNonStringObject = false;
        }
        public bool SetValueAsString(string o, Type type)
        {
            _stringValue = o;
            IsUnparsedString = true;
            IsNonStringObject = false;
            return ParseStringToObject(type);
        }

        public bool IsNonStringObject { get; private set; } = false;
        public bool IsUnparsedString { get; private set; } = false;

        public bool ParseStringToObject(Type type)
        {
            bool canParse = SerializationCommon.CanParseAsString(type);
            if (canParse)
                _value = SerializationCommon.ParseString(_stringValue, type);
            IsUnparsedString = !canParse;
            IsNonStringObject = false;
            return canParse;
        }
        public bool GetObject<T>(out T value)
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
    public sealed class MemberTreeNode
    {
        public List<SerializeAttribute> ChildAttributeMembers { get; set; } = new List<SerializeAttribute>();
        public EventList<MemberTreeNode> ChildElementMembers { get; set; }
        public object ChildElementObjectMember { get; set; }

        public bool GetChildElementObjectMemberAsString(out string value)
        {
            value = null;
            return ChildElementObjectMember == null || SerializationCommon.GetString(ChildElementObjectMember, ChildElementObjectMember.GetType(), out value);
        }

        public bool ElementObjectAsString(out string result)
            => SerializationCommon.GetString(ChildElementObjectMember, ChildElementObjectMember.GetType(), out result);
        
        public MemberTreeNode Parent { get; internal set; }
        public TBaseSerializer.TBaseAbstractReaderWriter Owner { get; internal set; }
        public TSerializeMemberInfo MemberInfo { get; set; }

        internal int CalculatedSize { get; set; }
        internal int ManuallyCalculatedSize { get; set; }
        internal byte[] ParsableBytes { get; set; } = null;
        internal string ParsableString { get; set; } = null;
        internal int ParsablePointerSize { get; set; } = 0;

        public object DefaultObject { get; private set; }

        /// <summary>
        /// The value assigned to this member.
        /// </summary>
        public object Object
        {
            get => _object;
            set
            {
                _object = value;
                ObjectChanged();
            }
        }
        private object _object;

        /// <summary>
        /// True if this node is just for categorization of the child nodes.
        /// </summary>
        public bool IsGroupingNode => MemberInfo == null;
        /// <summary>
        /// How many checkpoints need to be hit for this node and all child nodes to advance the progression handler.
        /// </summary>
        public int ProgressionCount => 1 + ChildAttributeMembers.Count + ChildElementMembers.Count + (ChildElementObjectMember != null ? 1 : 0) + (IsDerivedType ? 1 : 0);
        /// <summary>
        /// The type of the object assigned to this member.
        /// </summary>
        public Type ObjectType => _object?.GetType() ?? MemberInfo?.MemberType;
        /// <summary>
        /// <see langword="true"/> if the object's type inherits from the member's type instead of matching it exactly.
        /// </summary>
        public bool IsDerivedType => Parent == null || (ObjectType?.IsSubclassOf(MemberInfo.MemberType) ?? false);
        
        /// <summary>
        /// The class handling how to read and write all members of any given object.
        /// Most classes will use <see cref="CommonSerializer"/>.
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
        }
        
        public MemberTreeNode(object obj, TSerializeMemberInfo memberInfo)
        {
            Object = obj;
            Parent = null;
            MemberInfo = memberInfo;
            DefaultObject = ObjectType == null ? null : SerializationCommon.CreateObject(ObjectType);
        }

        public async Task<bool> TryInvokeCustomDeserializeAsync()
        {
            if (MemberInfo == null)
                return false;

            var customMethods = CustomDeserializeMethods.Where(
                x => string.Equals(MemberInfo.Name, x.GetCustomAttribute<CustomDeserializeMethod>().Name));

            foreach (var customMethod in customMethods)
            {
                if (customMethod == null)
                    continue;

                var parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(GetType()))
                {
                    if (customMethod.ReturnType == typeof(Task))
                        await (Task)customMethod.Invoke(Object, new object[] { this });
                    else
                        customMethod.Invoke(Object, new object[] { this });
                    return true;
                }
                else
                {
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a {nameof(CustomDeserializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {nameof(MemberTreeNode)}.");
                }
            }
            return false;
        }
        public async Task<bool> TryInvokeCustomSerializeAsync()
        {
            if (MemberInfo == null)
                return false;

            var customMethods = CustomSerializeMethods.Where(
                   x => string.Equals(MemberInfo.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));

            foreach (var customMethod in customMethods)
            {
                if (customMethod == null)
                    continue;

                var parameters = customMethod.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(GetType()))
                {
                    if (customMethod.ReturnType == typeof(Task))
                        await (Task)customMethod.Invoke(Object, new object[] { this });
                    else
                        customMethod.Invoke(Object, new object[] { this });
                    return true;
                }
                else
                {
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a {nameof(CustomSerializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {nameof(MemberTreeNode)}.");
                }
            }
            return false;
        }
        public async Task GenerateObjectFromTreeAsync(Type objectType)
        {
            Object = SerializationCommon.CreateObject(objectType);
            await ObjectSerializer.ReadObjectMembersFromTreeAsync();
        }
        public async Task CreateTreeFromObjectAsync()
        {
            await FileObjectCheckAsync();
            if (ObjectSerializer != null)
                await ObjectSerializer.GenerateTreeFromObject();
        }
        private void ObjectChanged()
        {
            PreSerializeMethods = new List<MethodInfo>();
            PostSerializeMethods = new List<MethodInfo>();
            PreDeserializeMethods = new List<MethodInfo>();
            PostDeserializeMethods = new List<MethodInfo>();
            CustomSerializeMethods = new List<MethodInfo>();
            CustomDeserializeMethods = new List<MethodInfo>();

            if (ObjectType != null)
            {
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
                            serAttrib.RunForFormats.HasFlag(Owner.Format))
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
                
                ObjectSerializer = SerializationCommon.DetermineObjectSerializer(ObjectType, this);
            }
            else
            {
                ObjectSerializer = null;
            }
        }
        public void ApplyObjectToParent()
        {
            if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Field))
                ((FieldInfo)MemberInfo.Member).SetValue(Parent.Object, _object);
            else if (MemberInfo.Member.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo p = (PropertyInfo)MemberInfo.Member;
                if (p.CanWrite)
                    p.SetValue(Parent.Object, _object);
                else
                    Engine.LogWarning($"Can't set property {p.Name} in {p.DeclaringType.GetFriendlyName()}.");
            }
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
            => ChildElementMembers.Add(new MemberTreeNode(null, new TSerializeMemberInfo(typeof(string), elementName)) { ChildElementObjectMember = elementObject });
        public void AddAttribute(string name, object value)
            => ChildAttributeMembers.Add(new SerializeAttribute(name, value));
        public SerializeAttribute GetAttribute(string name) 
            => ChildAttributeMembers.FirstOrDefault(x => string.Equals(x.Name, name));
        public SerializeAttribute GetAttribute(int index)
            => ChildAttributeMembers.IndexInRange(index) ? ChildAttributeMembers[index] : null;
        public bool GetAttributeValue<T>(string name, out T value)
        {
            SerializeAttribute attrib = GetAttribute(name);
            if (attrib != null)
                return attrib.GetObject(out value);
            value = default;
            return false;
        }
        public bool GetAttributeValue<T>(int index, out T value)
        {
            SerializeAttribute attrib = GetAttribute(index);
            if (attrib != null)
                return attrib.GetObject(out value);
            value = default;
            return false;
        }
        public MemberTreeNode GetChildElement(string name)
            => ChildElementMembers.FirstOrDefault(x => string.Equals(x.MemberInfo.Name, name));
        public override string ToString() => MemberInfo.Name;
    }
}
