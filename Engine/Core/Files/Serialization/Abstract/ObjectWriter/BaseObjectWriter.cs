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
        public object Object { get; set; }
        public TSerializer.AbstractWriter Writer { get; set; }
        public VarInfo Info { get; set; }

        public abstract void Initialize();
        public abstract Task GenerateChildTree();
        public abstract int GetSize(MethodInfo[] customMethods, ref int flagCount, BinaryStringTable table);
        public abstract int GetSizeMember(MemberTreeNode node, MethodInfo[] customMethods, ref int flagCount, BinaryStringTable table);
        public abstract bool Write(ref VoidPtr address, BinaryStringTable table);
    }
}
