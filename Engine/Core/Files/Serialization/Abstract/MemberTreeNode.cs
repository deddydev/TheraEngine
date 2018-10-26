using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    public class SerializeAttribute
    {
        public SerializeAttribute() { }
        public SerializeAttribute(string name, object value) { Name = name; Value = value; }

        public string Name { get; set; }
        public object Value { get; set; }

        public bool CanWriteValueAsString() => SerializationCommon.CanParseAsString()
    }
    public class BinaryMemberTreeNode : MemberTreeNode<BinaryMemberTreeNode>
    {
        public BinaryMemberTreeNode(object obj)
            : base(obj) { }
        public BinaryMemberTreeNode(BinaryMemberTreeNode parent, TSerializeMemberInfo memberInfo)
            : base(parent, memberInfo) { }
        
        internal int CalculatedSize { get; set; }
        internal int ManuallyCalculatedSize { get; set; }
        internal byte[] ParsableBytes { get; set; } = null;
        internal string ParsableString { get; set; } = null;
        internal int ParsablePointerSize { get; set; } = 0;
    }
    public class XMLMemberTreeNode : MemberTreeNode<XMLMemberTreeNode>
    {
        public XMLMemberTreeNode(object obj)
            : base(obj) { }
        public XMLMemberTreeNode(XMLMemberTreeNode parent, TSerializeMemberInfo memberInfo)
            : base(parent, memberInfo) { }
        public XMLMemberTreeNode(string name, string elementString, SerializeAttribute[] attributes, Type memberType)
            : base(null)
        {
            MemberInfo = new TSerializeMemberInfo(name, null, memberType, new TSerialize() { NodeType = ENodeType.ChildElement });
            ElementObject = elementString;
            Attributes = attributes?.ToList();
        }

        public int NonAttributeCount { get; private set; }

        protected override async Task OnAddChildElementAsync(XMLMemberTreeNode childMember)
        {
            if (childMember != null)
            {
                if (childMember.NodeType == ENodeType.ChildElement)
                {
                    ChildElements.Add(childMember);
                    await childMember.CreateTreeFromObjectAsync();
                }
                else
                {
                    if (SerializationCommon.GetString(childMember.Object, childMember.ObjectType, out string result))
                    {
                        if (childMember.NodeType == ENodeType.ElementString && NonAttributeCount == 1)
                            ElementObject = result;
                        else
                            Attributes.Add(new SerializeAttribute(childMember.Name, result));
                    }
                    else
                        Engine.LogWarning(ObjectType.Name + " cannot be written as a string.");
                }
            }
        }

        protected internal override async Task AddChildNodesAsync(int attribCount, int elementCount, int elementStringCount, List<XMLMemberTreeNode> members)
        {
            NonAttributeCount = elementCount + elementStringCount;
            Attributes = new List<SerializeAttribute>(attribCount);
            ChildElements = new List<XMLMemberTreeNode>(elementCount);
            ElementObject = null;
            ProgressionCount = 1 + attribCount + elementCount + elementStringCount;
            if (IsDerivedType)
                ++ProgressionCount;
            foreach (XMLMemberTreeNode member in members)
            {
                await AddChildElementAsync(member);
                ProgressionCount += member.ProgressionCount;
            }
        }
        protected internal override List<IMemberTreeNode> GetChildNodes(Type objectType)
        {
            int elementCount = 0;
            var members = SerializationCommon.CollectSerializedMembers(objectType);
            TSerializeMemberInfo member;

            List<IMemberTreeNode> nodes = new List<IMemberTreeNode>();
            foreach (var attrib in Attributes)
            {
                string name = attrib.Name;
                string val = attrib.Value;
                member = members.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCulture));
                if (member == null)
                {
                    Engine.PrintLine($"No member in {objectType.GetFriendlyName()} matches attribute named '{name}'.");
                }
                else if (!SerializationCommon.CanParseAsString(member.MemberType))
                {
                    Engine.PrintLine($"The value ({val}) of the attribute named '{name}' cannot be parsed as {member.MemberType.GetFriendlyName()}.");
                }
                else
                {
                    object o = SerializationCommon.ParseString(val, member.MemberType);
                    XMLMemberTreeNode attribNode = new XMLMemberTreeNode(o) { NodeType = ENodeType.Attribute };
                    nodes.Add(attribNode);
                }
            }
            if (ChildElements != null)
            {
                foreach (XMLMemberTreeNode node in ChildElements)
                {
                    ++elementCount;
                    node.NodeType = ENodeType.ChildElement;
                    nodes.Add(node);
                }
            }
            if (ElementObject != null)
            {
                if (elementCount > 0)
                {
                    XMLMemberTreeNode attribNode = new XMLMemberTreeNode(null)
                    {
                        NodeType = ENodeType.Attribute,
                    };
                    ++elementCount;

                }
                else
                {

                }
            }
            return nodes;
        }
        public void AddChildElementString(string elementName, string value, Type objectType)
        {
            XMLMemberTreeNode element = new XMLMemberTreeNode(elementName, value)
            {
                NodeType = ENodeType.ChildElement,
            };
        }
        public SerializeAttribute GetAttribute(string attributeName)
            => Attributes.Find(x => string.Equals(x.Name, attributeName, StringComparison.InvariantCulture));
        public XMLMemberTreeNode GetChildElement(string elementName)
            => ChildElements.Find(x => string.Equals(x.MemberInfo?.Name, elementName, StringComparison.InvariantCulture));
        public void AddAttribute(string name, string value)
            => Attributes.Add(new SerializeAttribute(name, value));
    }
    public interface IMemberTreeNode
    {
        List<SerializeAttribute> Attributes { get; }
        EventList<IMemberTreeNode> ChildElements { get; }
        object ElementObject { get; }

        int ProgressionCount { get; }
        /// <summary>
        /// The member this node references.
        /// </summary>
        TSerializeMemberInfo MemberInfo { get; set; }
        /// <summary>
        /// The default value set by the owning object in its constructor with no arguments.
        /// </summary>
        object DefaultObject { get; }
        /// <summary>
        /// The value currently attributed to this member.
        /// </summary>
        object Object { get; set; }
        /// <summary>
        /// The type of the object.
        /// </summary>
        Type ObjectType { get; }
        /// <summary>
        /// True if the object assigned to this member inherits from the member's specified type.
        /// </summary>
        bool IsDerivedType { get; }
        /// <summary>
        /// The node for the object that owns this object.
        /// </summary>
        IMemberTreeNode Parent { get; set; }
        /// <summary>
        /// The reader or writer that is using this node.
        /// </summary>
        TBaseSerializer.IBaseAbstractReaderWriter Owner { get; }
        /// <summary>
        /// The serializer that will be used to read and write this object.
        /// </summary>
        BaseObjectSerializer ObjectSerializer { get; set; }

        List<MethodInfo> PreSerializeMethods { get; }
        List<MethodInfo> PostSerializeMethods { get; }
        List<MethodInfo> PreDeserializeMethods { get; }
        List<MethodInfo> PostDeserializeMethods { get; }
        List<MethodInfo> CustomSerializeMethods  { get; }
        List<MethodInfo> CustomDeserializeMethods  { get; }

        Task GenerateObjectFromTreeAsync(Type objectType);
        Task CreateTreeFromObjectAsync();
    }
    public abstract class MemberTreeNode<T> : IMemberTreeNode where T : class, IMemberTreeNode
    {
        public bool IsGroupingNode => MemberInfo == null;
        
        public List<SerializeAttribute> Attributes { get; internal set; } = new List<SerializeAttribute>();
        public EventList<IMemberTreeNode> ChildElements { get; internal set; }
        public object ElementObject { get; internal set; }

        public T Parent { get; internal set; }
        IMemberTreeNode IMemberTreeNode.Parent { get => Parent; set => Parent = value as T; }

        /// <summary>
        /// How many checkpoints need to be hit for this node and all child nodes to advance the progression handler.
        /// </summary>
        public int ProgressionCount { get; internal set; }
        public TBaseSerializer.IBaseAbstractReaderWriter Owner { get; internal set; }
        public TSerializeMemberInfo MemberInfo { get; set; }

        public object DefaultObject { get; private set; }

        private object _object;
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
        
        public MemberTreeNode()
        {
            Parent = null;
            MemberInfo = null;
            Object = null;
            DefaultObject = null;

            ChildElements = new EventList<IMemberTreeNode>();
            ChildElements.PostAnythingAdded += ChildElements_PostAnythingAdded;
            ChildElements.PostAnythingRemoved += ChildElements_PostAnythingRemoved;
        }
        public MemberTreeNode(object obj)
        {
            Object = obj;
            Parent = null;

            MemberInfo = new TSerializeMemberInfo(SerializationCommon.GetTypeName(ObjectType),
                null, ObjectType, true, true, ENodeType.ChildElement, 0, null);

            DefaultObject = ObjectType == null ? null : SerializationCommon.CreateObject(ObjectType);
        }
        public MemberTreeNode(T parent, TSerializeMemberInfo memberInfo)
        {
            Parent = parent;
            MemberInfo = memberInfo;
            RetrieveObjectFromParent();
        }
        private void ChildElements_PostAnythingRemoved(IMemberTreeNode item)
        {

        }
        private async void ChildElements_PostAnythingAdded(IMemberTreeNode item)
        {
            item.Parent = this;

            if (item?.Object == null)
                return;

            TSerializeMemberInfo info = item.MemberInfo;
            if (item.Owner is TSerializer.IAbstractWriter writer)
            {
                ESerializeFlags flags = writer.Flags;

                if (info.State && !flags.HasFlag(ESerializeFlags.SerializeState))
                    return;

                if (info.Config && !flags.HasFlag(ESerializeFlags.SerializeConfig))
                    return;
            }

            bool invoked = await TryInvokeCustomMethodAsync(info);
            if (!invoked)
                await OnAddChildElementAsync(item);
        }
        protected abstract Task OnAddChildElementAsync(T member);

        private async Task<bool> TryInvokeCustomMethodAsync(TSerializeMemberInfo info)
        {
            var customMethods = CustomSerializeMethods.Where(
                   x => string.Equals(info.Name, x.GetCustomAttribute<CustomSerializeMethod>().Name));

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
                    Engine.LogWarning($"Method {customMethod.GetFriendlyName()} is marked with a {nameof(CustomSerializeMethod)} attribute, but the arguments are not correct. There must be one argument of type {nameof(IMemberTreeNode)}.");
                }
            }

            return false;
        }
        async Task IMemberTreeNode.GenerateObjectFromTreeAsync(Type objectType)
            => await GenerateObjectFromTreeAsync(objectType);
        public async Task GenerateObjectFromTreeAsync(Type objectType)
        {

        }
        async Task IMemberTreeNode.CreateTreeFromObjectAsync()
            => await CreateTreeFromObjectAsync();
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

                //DefaultConstructedObject = SerializationCommon.CreateObject(ObjectType);
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
        public override string ToString() => MemberInfo.Name;

        //protected internal abstract Task AddChildNodesAsync(int attribCount, int elementCount, int elementStringCount, List<T> members);
        //Task IMemberTreeNode.AddChildNodesAsync(int attribCount, int elementCount, int elementStringCount, List<IMemberTreeNode> members)
        //    => AddChildNodesAsync(attribCount, elementCount, elementStringCount, members.Select(x => (T)x).ToList());

        //protected internal abstract List<IMemberTreeNode> GetChildNodes(Type objectType);
        //List<IMemberTreeNode> IMemberTreeNode.GetChildNodes(Type objectType) => GetChildNodes(objectType);
    }
}
