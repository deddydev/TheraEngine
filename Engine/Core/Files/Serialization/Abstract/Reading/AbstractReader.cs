using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class Deserializer : TBaseSerializer
    {
        public AbstractReader Reader { get; private set; }
        public abstract class AbstractReader : TBaseAbstractReaderWriter
        {
            /// <summary>
            /// The deserializer that is using this reader.
            /// </summary>
            public Deserializer Owner { get; }
            public Type RootFileType { get; }

            protected AbstractReader(Deserializer owner, string filePath, IProgress<float> progress, CancellationToken cancel)
                : base(filePath, progress, cancel)
            {
                Owner = owner;
                RootFileType = SerializationCommon.DetermineType(FilePath, out EFileFormat format);
            }
                        
            protected abstract Task ReadTreeAsync();
            public async Task<object> CreateObjectAsync()
            {
                await ReadTreeAsync();
                RootNode.DeserializeTreeToObject();
                object obj = RootNode.Object;
                if (obj is TFileObject tobj)
                    tobj.FilePath = FilePath;
                return obj;
            }
        }
    }
}
