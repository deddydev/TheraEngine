using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TDeserializer : TBaseSerializer
    {
        public AbstractReader Reader { get; private set; }
        public abstract class AbstractReader : TBaseAbstractReaderWriter
        {
            /// <summary>
            /// The deserializer that is using this reader.
            /// </summary>
            public TDeserializer Owner { get; }
            public Type RootFileType { get; }

            protected AbstractReader(TDeserializer owner, string filePath, IProgress<float> progress, CancellationToken cancel)
                : base(filePath, progress, cancel)
            {
                Owner = owner;
                RootFileType = SerializationCommon.DetermineType(FilePath);
            }
                        
            protected abstract Task ReadTreeAsync();
            public async Task CreateObjectAsync()
            {
                await ReadTreeAsync();
                RootNode.GenerateObjectFromTree(RootFileType);
            }
        }
    }
}
