using System;
using System.Collections.Generic;
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
            public string FilePath { get; private set; }
            public IProgress<float> Progress { get; private set; }
            public CancellationToken Cancel { get; private set; }
            public TFileObject RootFileObject { get; private set; }
            public Dictionary<Guid, TObject> SharedObjects { get; private set; }

            public abstract Task Start();
            public abstract Task Finish();

            /// <summary>
            /// Reports progress back to the deserialization caller 
            /// and returns true if the caller wants to cancel the operation.
            /// </summary>
            /// <returns>True if the caller wants to cancel the operation;
            /// false if the operation should continue.</returns>
            public void ReportProgress()
            {
                OnReportProgress();
                return Cancel.IsCancellationRequested;
            }
            protected abstract void OnReportProgress();
        }
    }
}
