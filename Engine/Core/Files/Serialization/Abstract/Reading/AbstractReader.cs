using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TDeserializer : TBaseSerializer
    {
        public IAbstractReader Reader { get; private set; }
        public EProprietaryFileFormat Format { get; private set; }

        public interface IAbstractReader : IBaseAbstractReaderWriter
        {
            /// <summary>
            /// The deserializer that is using this reader.
            /// </summary>
            TDeserializer Owner { get; }
            Type RootFileType { get; }
            
            Task CreateObjectAsync();
        }
        public abstract class AbstractReader<T> : TBaseAbstractReaderWriter<T>, IAbstractReader where T : class, IMemberTreeNode
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
                await RootNode.GenerateObjectFromTreeAsync(RootFileType);
            }
        }
    }
}
