using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class Deserializer : BaseSerializationIO
    {
        public AbstractReader Reader { get; private set; }
        public abstract class AbstractReader : BaseAbstractReaderWriter
        {
            /// <summary>
            /// The deserializer that is using this reader.
            /// </summary>
            public Deserializer Owner { get; }
            public TypeProxy RootFileType { get; }

            protected AbstractReader(
                Deserializer owner,
                string filePath,
                TypeProxy fileType,
                Stream stream,
                IProgress<float> progress,
                CancellationToken cancel)
                : base(filePath, stream, progress, cancel)
            {
                Owner = owner;
                RootFileType = fileType ?? SerializationCommon.DetermineType(FilePath, out EFileFormat _);
            }
            
            protected abstract Task ReadTreeAsync();
            public async Task<object> CreateObjectAsync()
            {
                Log($"Deserializing object from {FilePath}");

                await ReadTreeAsync();

                SerializeElement sharedObjectsElem = RootNode?.GetChildElement("SharedObjects");
                if (sharedObjectsElem != null)
                    foreach (SerializeElement sharedObjectElem in sharedObjectsElem.Children)
                    {
                        await sharedObjectElem.DeserializeTreeToObjectAsync();

                        ReadingSharedObjectsList.Add(sharedObjectElem);
                        int index = ReadingSharedObjectsList.Count - 1;
                        if (!ReadingSharedObjectsSetQueue.ContainsKey(index))
                            continue;

                        foreach (SerializeElement elem in ReadingSharedObjectsSetQueue[index])
                        {
                            elem.Object = sharedObjectElem.Object;
                            elem.ApplyObjectToParent();
                        }
                        ReadingSharedObjectsSetQueue.Remove(index);
                    }
                
                if (RootNode != null)
                    await RootNode.DeserializeTreeToObjectAsync();

                object obj = RootNode?.Object;
                if (obj is IFileObject tobj)
                    tobj.FilePath = FilePath;

                if (PendingAsyncTasks.Count > 0)
                    MonitorAsyncCompletion();

                return obj;
            }

            public event Action FileAsyncIOCompleted;

            private async void MonitorAsyncCompletion()
            {
                Log($"Waiting for {PendingAsyncTasks.Count} async file IO tasks to complete.");
                await Task.WhenAll(PendingAsyncTasks);
                Log($"Finished all async file IO tasks.");
                FileAsyncIOCompleted?.Invoke();
            }
        }
    }
}
