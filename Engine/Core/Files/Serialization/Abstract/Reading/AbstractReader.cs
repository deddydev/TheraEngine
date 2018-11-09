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
                RootFileType = SerializationCommon.DetermineType(FilePath, out EFileFormat format);
            }
                        
            protected abstract Task ReadTreeAsync();
            public async Task<object> CreateObjectAsync()
            {
                await ReadTreeAsync();
                RootNode.TreeToObject();
                object obj = RootNode.Object;
                if (obj is TFileObject tobj)
                    tobj.FilePath = FilePath;
                return obj;
            }
        }
    }
}
