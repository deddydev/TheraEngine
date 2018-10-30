using System;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    public class ObjectWriterKind : Attribute
    {
        /// <summary>
        /// The type this writer will be collecting members for.
        /// </summary>
        public Type ObjectType { get; }
        public ObjectWriterKind(Type objectType) => ObjectType = objectType;
    }
    /// <summary>
    /// Tool to collect all members of an object into an array of children.
    /// </summary>
    public abstract class BaseObjectSerializer
    {
        public MemberTreeNode TreeNode { get; internal protected set; } = null;
        public int TreeSize { get; private set; }

        private TSerializer.WriterBinary GetBinaryWriter() => TreeNode.Owner as TSerializer.WriterBinary;
        private TSerializer.WriterXML GetXMLWriter() => TreeNode.Owner as TSerializer.WriterXML;
        private TDeserializer.ReaderBinary GetBinaryReader() => TreeNode.Owner as TDeserializer.ReaderBinary;
        private TDeserializer.ReaderXML GetXMLReader() => TreeNode.Owner as TDeserializer.ReaderXML;

        public int GetTreeSize() => TreeSize = OnGetTreeSize();
        public abstract int OnGetTreeSize();
        public abstract void TreeFromBinary(ref VoidPtr address);
        public abstract void TreeToBinary(ref VoidPtr address);
        public abstract void TreeToObject();
        public abstract void TreeFromObject();
    }
}
