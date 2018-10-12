using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer : TBaseSerializer
    {
        public abstract class BaseAbstractWriter : TBaseAbstractReaderWriter
        {
            public TSerializer Owner { get; }
            public ESerializeFlags Flags { get; internal set; }
            public MemberTreeNode RootNode { get; protected set; }

            protected BaseAbstractWriter(TSerializer owner, TFileObject rootFileObject, string filePath, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
                : base(rootFileObject, filePath, progress, cancel)
            {
                Owner = owner;
                Flags = flags;
            }

            public abstract MemberTreeNode CreateNode(object root);
            public abstract MemberTreeNode CreateNode(MemberTreeNode parent, MemberInfo memberInfo);

            internal protected abstract Task WriteTree(MemberTreeNode rootNode);
        }
        public abstract class AbstractWriter<T> : BaseAbstractWriter where T : MemberTreeNode
        {
            protected AbstractWriter(TSerializer owner, TFileObject rootFileObject, string filePath, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
                : base(owner, rootFileObject, filePath, flags, progress, cancel) { }

            public override MemberTreeNode CreateNode(MemberTreeNode parent, MemberInfo memberInfo) => CreateNode(parent as T, memberInfo);
            public abstract T CreateNode(T parent, MemberInfo memberInfo);

            internal protected override Task WriteTree(MemberTreeNode root) => WriteTree(root as T);
            internal protected abstract Task WriteTree(T root);
        }
    }
}
