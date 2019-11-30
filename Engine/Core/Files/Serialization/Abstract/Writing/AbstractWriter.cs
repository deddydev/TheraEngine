using Extensions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class Serializer : BaseSerializationIO
    {
        public AbstractWriter Writer { get; private set; }
        public abstract class AbstractWriter : BaseAbstractReaderWriter
        {
            /// <summary>
            /// The serializer that is using this writer.
            /// </summary>
            public Serializer Owner { get; }
            
            protected AbstractWriter(
                Serializer owner, 
                object rootFileObject,
                string filePath,
                Stream stream,
                ESerializeFlags flags, 
                IProgress<float> progress, 
                CancellationToken cancel)
                : base(filePath, stream, progress, cancel)
            {
                Flags = flags;
                Owner = owner;

                TypeProxy t = rootFileObject.GetTypeProxy();
                RootNode = new SerializeElement(rootFileObject, new TSerializeMemberInfo(t, SerializationCommon.FixElementName(t.Name)));
            }
            
            /// <summary>
            /// Writes the root node's member tree to the file path.
            /// </summary>
            protected abstract Task WriteTreeAsync();
            public async Task WriteObjectAsync()
            {
                RootNode.SerializeTreeFromObject();

                if (WritingSharedObjectIndices.Count > 0)
                {
                    foreach (var kv in WritingSharedObjectIndices)
                        if (kv.Value <= 1)
                        {
                            //WritingSharedObjects[kv.Key].IsSharedObject = false;
                            WritingSharedObjects.Remove(kv.Key);
                        }

                    WritingSharedObjectIndices.Clear();
                    int index = 0;
                    foreach (var shared in WritingSharedObjects)
                        WritingSharedObjectIndices.Add(shared.Key, index++);
                }

                await WriteTreeAsync();
            }
        }
    }
}
