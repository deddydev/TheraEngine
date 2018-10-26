using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer : TBaseSerializer
    {
        public IAbstractWriter Writer { get; private set; }

        public interface IAbstractWriter : IBaseAbstractReaderWriter
        {
            /// <summary>
            /// The serializer that is using this writer.
            /// </summary>
            TSerializer Owner { get; }
            
            Task WriteObjectAsync();
        }
        public abstract class AbstractWriter<T> : TBaseAbstractReaderWriter<T>, IAbstractWriter where T : class, IMemberTreeNode
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
                RootNode = CreateNode(rootFileObject);
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
