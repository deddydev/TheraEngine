using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Tools;
using static TheraEngine.Core.Files.Serialization.TSerializer.WriterBinary;

namespace TheraEngine.Core.Files.Serialization
{
    public class ObjectWriterKind : Attribute
    {
        public Type ObjectType { get; }
        public ObjectWriterKind(Type objectType) => ObjectType = objectType;
    }
    public abstract class BaseObjectWriter
    {
        public MemberTreeNode TreeNode { get; internal set; }

        public List<(string, object)> Attributes { get; internal set; }
        public List<MemberTreeNode> ChildElements { get; internal set; }
        public object SingleSerializableChildData { get; internal set; }
        
        public abstract Task GenerateTree();
    }
}
