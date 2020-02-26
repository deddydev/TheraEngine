using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Files.Serialization
{
    public class BaseSerializationIO : MarshalByRefObject
    {
        protected static void Log(string message)
            => Trace.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] " + message);

        public EProprietaryFileFormat Format { get; protected set; }
        public abstract class BaseAbstractReaderWriter : MarshalByRefObject
        {
            protected ProgressStream _stream;

            public string FilePath { get; internal set; }
            public string FileDirectory { get; internal set; }
            public IProgress<float> Progress { get; internal set; }
            public CancellationToken Cancel { get; internal set; }
            public object RootFileObject { get; internal set; }
            public Stream Stream { get; internal set; }

            private SerializeElement _rootNode;
            public SerializeElement RootNode
            {
                get => _rootNode;
                protected set
                {
                    if (_rootNode != null)
                    {
                        _rootNode.Manager = null;
                        _rootNode.IsRoot = false;
                        if (_rootNode.Object is IFileObject tobj && !string.IsNullOrEmpty(tobj.FilePath))
                            tobj.FilePath = null;
                    }
                    _rootNode = value;
                    if (_rootNode != null)
                    {
                        _rootNode.Manager = this;
                        _rootNode.IsRoot = true;
                        if (_rootNode.Object is IFileObject tobj && !string.IsNullOrEmpty(FilePath))
                            tobj.FilePath = FilePath;
                    }
                }
            }

            internal int CurrentCount { get; set; }
            public ESerializeFlags Flags { get; internal set; }
            public abstract EProprietaryFileFormatFlag Format { get; }
            public bool IsBinary => Format == EProprietaryFileFormatFlag.Binary;

            protected BaseAbstractReaderWriter(
                string filePath,
                Stream stream,
                IProgress<float> progress,
                CancellationToken cancel)
            {
                FilePath = filePath;
                Stream = stream;
                Progress = progress;
                Cancel = cancel;
                FileDirectory = Path.GetDirectoryName(FilePath);
            }
            
            /// <summary>
            /// Reports progress back to the deserialization caller 
            /// and returns true if the caller wants to cancel the operation.
            /// </summary>
            /// <returns>True if the caller wants to cancel the operation;
            /// false if the operation should continue.</returns>
            public bool CancelRequested => Cancel.IsCancellationRequested;

            public Dictionary<Guid, SerializeElement> WritingSharedObjects { get; } = new Dictionary<Guid, SerializeElement>();
            public Dictionary<Guid, int> WritingSharedObjectIndices { get; } = new Dictionary<Guid, int>();

            public SerializeElement SharedObjectsElement { get; set; }
            public List<SerializeElement> ReadingSharedObjectsList { get; } = new List<SerializeElement>();
            public Dictionary<int, List<SerializeElement>> ReadingSharedObjectsSetQueue { get; } = new Dictionary<int, List<SerializeElement>>();

            public List<Task> PendingAsyncTasks { get; } = new List<Task>();
        }
    }
}
