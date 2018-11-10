﻿using System;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    public class ObjectWriterKind : Attribute
    {
        /// <summary>
        /// The type this writer will be collecting members for.
        /// </summary>
        public Type ObjectType { get; }
        /// <summary>
        /// If this object needs specific handling in binary format.
        /// </summary>
        public bool ManualBinarySerialize { get; }
        public ObjectWriterKind(Type objectType, bool manualBinarySerialize = false)
        {
            ObjectType = objectType;
            ManualBinarySerialize = manualBinarySerialize;
        }
    }
    /// <summary>
    /// Tool to collect all members of an object into an array of children.
    /// </summary>
    public abstract class BaseObjectSerializer
    {
        public SerializeElement TreeNode { get; internal protected set; } = null;
        public int TreeSize { get; private set; }

        //protected TSerializer.WriterBinary      GetBinaryWriter()   => TreeNode.Owner as TSerializer.WriterBinary;
        //protected TSerializer.WriterXML         GetXMLWriter()      => TreeNode.Owner as TSerializer.WriterXML;
        //protected TDeserializer.ReaderBinary    GetBinaryReader()   => TreeNode.Owner as TDeserializer.ReaderBinary;
        //protected TDeserializer.ReaderXML       GetXMLReader()      => TreeNode.Owner as TDeserializer.ReaderXML;

        public int GetTreeSize(TSerializer.WriterBinary binWriter) => TreeSize = OnGetTreeSize(binWriter);
        public abstract int OnGetTreeSize(TSerializer.WriterBinary binWriter);
        public abstract void TreeFromBinary(ref VoidPtr address, TDeserializer.ReaderBinary binReader);
        public abstract void TreeToBinary(ref VoidPtr address, TSerializer.WriterBinary binWriter);
        public abstract void DeserializeTreeToObject();
        public abstract void SerializeTreeFromObject();
    }
}
