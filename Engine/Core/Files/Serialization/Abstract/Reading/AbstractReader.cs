using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class Deserializer : BaseSerializer
    {
        public AbstractReader Reader { get; private set; }
        public abstract class AbstractReader : BaseAbstractReaderWriter
        {
            /// <summary>
            /// The deserializer that is using this reader.
            /// </summary>
            public Deserializer Owner { get; }
            public TypeProxy RootFileType { get; }

            protected AbstractReader(Deserializer owner, string filePath, IProgress<float> progress, CancellationToken cancel)
                : base(filePath, progress, cancel)
            {
                Owner = owner;
                RootFileType = SerializationCommon.DetermineType(FilePath, out EFileFormat _);
            }
            
            protected abstract Task ReadTreeAsync();
            public async Task<object> CreateObjectAsync()
            {
                Engine.PrintLine("Deserializing object...");
                //if (!isGame)
                //{
                    
                //}

                await ReadTreeAsync();

                SerializeElement sharedObjectsElem = RootNode.GetChildElement("SharedObjects");
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
                
                await RootNode.DeserializeTreeToObjectAsync();

                object obj = RootNode.Object;
                if (obj is IFileObject tobj)
                    tobj.FilePath = FilePath;

                return obj;
            }
        }
    }
}
