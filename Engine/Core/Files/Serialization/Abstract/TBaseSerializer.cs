using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Files;

namespace TheraEngine.Core.Files.Serialization
{
    public class TBaseSerializer
    {
        public abstract class TBaseAbstractReaderWriter
        {
            public string FilePath { get; internal set; }
            public string FileDirectory { get; internal set; }
            public IProgress<float> Progress { get; internal set; }
            public CancellationToken Cancel { get; internal set; }
            public TFileObject RootFileObject { get; internal set; }
            public MemberTreeNode RootNode { get; protected set; }
            public Dictionary<Guid, TObject> SharedObjects { get; internal set; }
            internal int CurrentCount { get; set; }
            
            public TBaseAbstractReaderWriter(TFileObject rootFileObject, string filePath, IProgress<float> progress, CancellationToken cancel)
            {
                RootFileObject = rootFileObject;
                FilePath = filePath;
                Progress = progress;
                Cancel = cancel;
                SharedObjects = new Dictionary<Guid, TObject>();
                FileDirectory = Path.GetDirectoryName(FilePath);
            }
            
            /// <summary>
            /// Reports progress back to the deserialization caller 
            /// and returns true if the caller wants to cancel the operation.
            /// </summary>
            /// <returns>True if the caller wants to cancel the operation;
            /// false if the operation should continue.</returns>
            public bool ReportProgress()
            {
                float progress = (float)(++CurrentCount) / RootNode.ProgressionCount;
                Progress?.Report(progress);
                return Cancel.IsCancellationRequested;
            }
        }
    }
}
