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
            public MemberTreeNode RootNode { get; internal set; }
            public MemberTreeNode CurrentNode { get; protected set; }
            public ESerializeFlags Flags { get; internal set; }

            protected AbstractWriter(TSerializer owner, TFileObject rootFileObject, string filePath, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
                : base(rootFileObject, filePath, progress, cancel)
            {
                Owner = owner;
                RootNode = new MemberTreeNode(rootFileObject, this);
                CurrentNode = null;
            }

            public abstract Task WriteStartElementAsync(string name);
            public abstract Task WriteEndElementAsync();
            public abstract Task WriteAttributeStringAsync(string name, string value);
            public abstract Task WriteElementStringAsync(string name, string value);

            protected async Task ManualWriteAsync(TFileObject o) => await o?.WriteAsync(this);
            protected abstract Task WriteAsync(MemberTreeNode node);
        }
    }
}
