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
        public EProprietaryFileFormat Format { get; private set; }

        /// <summary>
        /// Writes <paramref name="fileObject"/> as an XML file.
        /// </summary>
        /// <param name="fileObject">The object to serialize into XML.</param>
        /// <param name="targetDirectoryPath">The path to a directory to write the file in.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="flags">Flags to determine what information to serialize.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        public async Task SerializeXMLAsync(
            TFileObject fileObject,
            string targetDirectoryPath,
            string fileName,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            string filePath = TFileObject.GetFilePath(targetDirectoryPath, fileName, Format = EProprietaryFileFormat.XML, fileObject.GetType());
            Writer = new WriterXML(this, fileObject, filePath, flags, progress, cancel, null);
            await Serialize();
        }
        /// <summary>
        /// Writes <paramref name="fileObject"/> as an XML file.
        /// </summary>
        /// <param name="fileObject">The object to serialize into binary.</param>
        /// <param name="targetDirectoryPath">The path to a directory to write the file in.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="flags">Flags to determine what information to serialize.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        /// <param name="endian">The direction to write multi-byte values in.</param>
        /// <param name="encrypted">If true, encrypts the file. The data cannot be decrypted without the password.</param>
        /// <param name="compressed">If true, compresses the file. This will make the file size as small as possible.</param>
        /// <param name="encryptionPassword">If encrypted, this is the password to use to encrypt and decrypt.</param>
        /// <param name="compressionProgress">Handler for compression updates.</param>
        public async Task SerializeBinaryAsync(
            TFileObject fileObject,
            string targetDirectoryPath,
            string fileName,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel,
            Endian.EOrder endian,
            bool encrypted,
            bool compressed,
            string encryptionPassword,
            ICodeProgress compressionProgress)
        {
            string filePath = TFileObject.GetFilePath(targetDirectoryPath, fileName, Format = EProprietaryFileFormat.Binary, fileObject.GetType());
            Writer = new WriterBinary(this, fileObject, filePath, flags, progress, cancel, endian, encrypted, compressed, encryptionPassword, compressionProgress);
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
