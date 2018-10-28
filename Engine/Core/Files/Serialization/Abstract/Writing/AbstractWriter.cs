using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer : TBaseSerializer
    {
        public AbstractWriter Writer { get; private set; }
        public abstract class AbstractWriter : TBaseAbstractReaderWriter
        {
            /// <summary>
            /// The serializer that is using this writer.
            /// </summary>
            public TSerializer Owner { get; }
            
            protected AbstractWriter(TSerializer owner, object rootFileObject, string filePath, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
                : base(filePath, progress, cancel)
            {
                Flags = flags;
                Owner = owner;

                Type t = rootFileObject.GetType();
                RootNode = new MemberTreeNode(rootFileObject, new TSerializeMemberInfo(t, SerializationCommon.FixElementName(t.Name)));
            }
            
            /// <summary>
            /// Writes the root node's member tree to the file path.
            /// </summary>
            protected abstract Task WriteTreeAsync();
            public async Task WriteObjectAsync()
            {
                await RootNode.CreateTreeFromObjectAsync();
                await WriteTreeAsync();
            }
        }
    }
}
