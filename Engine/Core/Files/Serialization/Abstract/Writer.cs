using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer : TBaseSerializer
    {
        public IAbstractWriter Writer { get; private set; }

        public interface IAbstractWriter : IBaseAbstractReaderWriter
        {
            TSerializer Owner { get; }
            
            Task WriteTreeAsync();
        }
        public abstract class AbstractWriter<T> : TBaseAbstractReaderWriter<T>, IAbstractWriter where T : class, IMemberTreeNode
        {
            public TSerializer Owner { get; }
            
            protected AbstractWriter(TSerializer owner, TFileObject rootFileObject, string filePath, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
                : base(rootFileObject, filePath, progress, cancel)
            {
                Flags = flags;
                Owner = owner;
                RootNode = CreateNode(rootFileObject);
            }
            
            internal protected abstract Task WriteTree();
            Task IAbstractWriter.WriteTreeAsync() => WriteTree();
        }
    }
}
