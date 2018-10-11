using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Files.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer : TBaseSerializer
    {
        public abstract class AbstractWriter : TBaseAbstractReaderWriter
        {
            public TSerializer Owner { get; }
            public ESerializeFlags Flags { get; internal set; }

            protected AbstractWriter(TSerializer owner, TFileObject rootFileObject, string filePath, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
                : base(rootFileObject, filePath, progress, cancel) => Owner = owner;

            internal protected abstract Task WriteTree(MemberTreeNode root);
            internal protected abstract MemberTreeNode CreateNode(object rootObject);
            internal protected abstract MemberTreeNode CreateNode(object obj, VarInfo memberInfo);
        }
    }
}
