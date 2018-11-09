using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TheraEngine.Core.Files.Serialization
{
    public class TBaseSerializer
    {
        public EProprietaryFileFormat Format { get; protected set; }
        public abstract class TBaseAbstractReaderWriter
        {
            public string FilePath { get; internal set; }
            public string FileDirectory { get; internal set; }
            public IProgress<float> Progress { get; internal set; }
            public CancellationToken Cancel { get; internal set; }
            public object RootFileObject => RootNode?.Object;
            private SerializeElement _rootNode;
            public SerializeElement RootNode
            {
                get => _rootNode;
                protected set
                {
                    _rootNode = value;
                    if (_rootNode != null)
                        _rootNode.Owner = this;
                }
            }
            public Dictionary<Guid, SerializeElement> SharedObjects { get; internal set; }
            public Dictionary<Guid, int> SharedObjectIndices { get; set; }
            internal int CurrentCount { get; set; }
            public ESerializeFlags Flags { get; internal set; }
            public abstract EProprietaryFileFormatFlag Format { get; }

            public TBaseAbstractReaderWriter(string filePath, IProgress<float> progress, CancellationToken cancel)
            {
                FilePath = filePath;
                Progress = progress;
                Cancel = cancel;
                SharedObjects = new Dictionary<Guid, SerializeElement>();
                FileDirectory = Path.GetDirectoryName(FilePath);
            }
            
            /// <summary>
            /// Reports progress back to the deserialization caller 
            /// and returns true if the caller wants to cancel the operation.
            /// </summary>
            /// <returns>True if the caller wants to cancel the operation;
            /// false if the operation should continue.</returns>
            public virtual bool ReportProgress()
            {
                float progress = (float)(++CurrentCount) / RootNode.ProgressionCount;
                Progress?.Report(progress);
                return Cancel.IsCancellationRequested;
            }
        }
    }
}
