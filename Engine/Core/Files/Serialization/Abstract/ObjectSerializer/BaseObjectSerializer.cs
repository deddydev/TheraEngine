using System;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ObjectSerializerFor : Attribute
    {
        public bool CanSerializeAsBinary { get; set; } = true;
        public bool CanSerializeAsString { get; set; } = false;
        /// <summary>
        /// The type this writer will be collecting members for.
        /// </summary>
        public Type ObjectType { get; }
        /// <summary>
        /// If this object needs specific handling in binary format.
        /// </summary>
        public bool ManualBinarySerialize { get; }
        public ObjectSerializerFor(Type objectType, bool manualBinarySerialize = false)
        {
            ObjectType = objectType;
            ManualBinarySerialize = manualBinarySerialize;
        }
    }
    /// <summary>
    /// Tool to collect all members of an object into an array of children.
    /// </summary>
    public abstract class BaseObjectSerializer : TObject
    {
        public bool ShouldWriteDefaultMembers
            => TreeNode?.Owner?.Flags.HasFlag(ESerializeFlags.WriteDefaultMembers) ?? false;
        public bool WriteChangedMembersOnly
            => TreeNode?.Owner?.Flags.HasFlag(ESerializeFlags.WriteChangedMembersOnly) ?? false;

        public SerializeElement TreeNode { get; protected internal set; } = null;
        public int TreeSize { get; private set; }
        
        protected abstract int OnGetTreeSize(Serializer.WriterBinary binWriter);
        /// <summary>
        /// Retrieves the size of the serialization tree in bytes.
        /// </summary>
        /// <param name="binWriter"></param>
        /// <returns></returns>
        public int GetTreeSize(Serializer.WriterBinary binWriter)
            => TreeSize = OnGetTreeSize(binWriter);

        /// <summary>
        /// Creates the serialization tree from a binary representation.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="binReader"></param>
        public abstract void DeserializeTreeFromBinary(ref VoidPtr address, Deserializer.ReaderBinary binReader);
        /// <summary>
        /// Creates a binary representation of the serialization tree.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="binWriter"></param>
        public abstract void SerializeTreeToBinary(ref VoidPtr address, Serializer.WriterBinary binWriter);
        
        /// <summary>
        /// Creates an object from a string.
        /// </summary>
        public abstract bool ObjectFromString(Type type, string value, out object result);
        /// <summary>
        /// Creates a string from an object.
        /// </summary>
        /// <returns></returns>
        public abstract bool ObjectToString(object obj, out string str);

        /// <summary>
        /// Creates the TreeNode's object from the serialization tree.
        /// </summary>
        public abstract void DeserializeTreeToObject();
        /// <summary>
        /// Creates the serialization tree from the TreeNode's object.
        /// </summary>
        public abstract void SerializeTreeFromObject();

        public abstract bool CanWriteAsString(Type type);
    }
}
