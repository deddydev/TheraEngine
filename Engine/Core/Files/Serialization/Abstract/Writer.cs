using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Files.Serialization;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer : TBaseSerializer
    {
        public abstract class AbstractWriter : TBaseAbstractReaderWriter
        {
            public TSerializer Owner { get; }
            public MemberTreeNode RootNode { get; private set; }
            public MemberTreeNode CurrentNode { get; private set; }

            protected AbstractWriter(TSerializer owner, MemberTreeNode rootNode)
            {
                Owner = owner;
                RootNode = rootNode;
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
