using SevenZip;
using System;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer : TBaseSerializer
    {
        public BaseAbstractWriter Writer { get; private set; }
        public MemberTreeNode RootNode { get; internal set; }

        public async Task SerializeXMLAsync(
            TFileObject obj,
            string dirPath,
            string name,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            string filePath = TFileObject.GetFilePath(dirPath, name, EProprietaryFileFormat.XML, obj.GetType());
            Writer = new WriterXML(this, obj, filePath, flags, progress, cancel, null);
            await Serialize();
        }
        public async Task SerializeBinaryAsync(
            TFileObject obj,
            string dirPath,
            string name,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel,
            Endian.EOrder order,
            bool encrypted,
            bool compressed,
            string encryptionPassword,
            ICodeProgress compressionProgress)
        {
            string filePath = TFileObject.GetFilePath(dirPath, name, EProprietaryFileFormat.Binary, obj.GetType());
            Writer = new WriterBinary(this, obj, filePath, flags, progress, cancel, order, encrypted, compressed, encryptionPassword, compressionProgress);
            await Serialize();
        }
        private async Task Serialize()
        {
            RootNode = Writer.CreateNode(Writer.RootFileObject);
            await RootNode.CollectSerializedMembers();
            await Writer.WriteTree(RootNode);
        }
    }
}
