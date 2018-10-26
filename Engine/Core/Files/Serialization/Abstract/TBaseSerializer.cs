using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace TheraEngine.Core.Files.Serialization
{
    public class TBaseSerializer
    {
        public EProprietaryFileFormat Format { get; protected set; }
        public interface IBaseAbstractReaderWriter
        {
            string FilePath { get; }
            string FileDirectory { get; }
            IProgress<float> Progress { get; }
            CancellationToken Cancel { get; }
            object RootFileObject { get; }
            IMemberTreeNode RootNode { get; }
            ESerializeFlags Flags { get; }
            EProprietaryFileFormatFlag Format { get; }

            IMemberTreeNode CreateNode(object obj);
            IMemberTreeNode CreateNode(IMemberTreeNode parent, TSerializeMemberInfo memberInfo);
            bool ReportProgress();
        }
        public abstract class TBaseAbstractReaderWriter<T> : IBaseAbstractReaderWriter where T : class, IMemberTreeNode
        {
            public string FilePath { get; internal set; }
            public string FileDirectory { get; internal set; }
            public IProgress<float> Progress { get; internal set; }
            public CancellationToken Cancel { get; internal set; }
            public object RootFileObject => RootNode?.Object;
            public T RootNode { get; protected set; }
            IMemberTreeNode IBaseAbstractReaderWriter.RootNode => RootNode;
            public Dictionary<Guid, T> SharedObjects { get; internal set; }
            public Dictionary<Guid, int> SharedObjectIndices { get; set; }
            internal int CurrentCount { get; set; }
            public ESerializeFlags Flags { get; internal set; }
            public abstract EProprietaryFileFormatFlag Format { get; }

            public TBaseAbstractReaderWriter(string filePath, IProgress<float> progress, CancellationToken cancel)
            {
                FilePath = filePath;
                Progress = progress;
                Cancel = cancel;
                SharedObjects = new Dictionary<Guid, T>();
                FileDirectory = Path.GetDirectoryName(FilePath);
            }

            IMemberTreeNode IBaseAbstractReaderWriter.CreateNode(object obj)
                => CreateNode(obj);
            IMemberTreeNode IBaseAbstractReaderWriter.CreateNode(IMemberTreeNode parent, TSerializeMemberInfo memberInfo)
                => CreateNode(parent as T, memberInfo);

            public abstract T CreateNode(T parent, TSerializeMemberInfo memberInfo);
            public abstract T CreateNode(object obj);

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
