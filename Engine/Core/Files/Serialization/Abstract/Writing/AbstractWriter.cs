using SevenZip;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer : TBaseSerializer
    {
        public AbstractWriter Writer { get; private set; }
        public abstract class AbstractWriter : TBaseAbstractReaderWriter
        {
            /// <summary>
            /// The serializer that is using this writer.
            /// </summary>
            public TSerializer Owner { get; }
            
            protected AbstractWriter(TSerializer owner, object rootFileObject, string filePath, ESerializeFlags flags, IProgress<float> progress, CancellationToken cancel)
                : base(filePath, progress, cancel)
            {
                Flags = flags;
                Owner = owner;

                Type t = rootFileObject.GetType();
                RootNode = new SerializeElement(rootFileObject, new TSerializeMemberInfo(t, SerializationCommon.FixElementName(t.Name)));
            }
            
            /// <summary>
            /// Writes the root node's member tree to the file path.
            /// </summary>
            protected abstract Task WriteTreeAsync();
            public async Task WriteObjectAsync()
            {
                RootNode.SerializeTreeFromObject();
                await WriteTreeAsync();
            }
        }

        public static async Task ExportXMLAsync(
            object file,
            string directory,
            string fileName,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            if (PreExport(file, directory, fileName, EProprietaryFileFormat.XML, null, out string filePath))
            {
                TSerializer serializer = new TSerializer();
                await serializer.SerializeXMLAsync(file, filePath, flags, progress, cancel);
            }
        }
        public static async Task ExportBinaryAsync(
            object file,
            string directory,
            string fileName,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel,
            Endian.EOrder endian = Endian.EOrder.Big,
            bool encrypted = false,
            bool compressed = false,
            string encryptionPassword = null,
            ICodeProgress compressionProgress = null)
        {
            if (PreExport(file, directory, fileName, EProprietaryFileFormat.Binary, null, out string filePath))
            {
                TSerializer serializer = new TSerializer();
                await serializer.SerializeBinaryAsync(
                    file, filePath, flags, progress, cancel,
                    endian, encrypted, compressed, encryptionPassword, compressionProgress);
            }
        }
        private static bool PreExport(object file, string directory, string fileName, EProprietaryFileFormat format, FileExt extOverride, out string filePath)
        {
            Type fileType = file.GetType();
            FileExt extAttrib = extOverride ?? TFileObject.GetFileExtension(fileType);
            if (extAttrib == null)
            {
                Engine.LogWarning($"No {nameof(FileExt)} attribute specified for {fileType.GetFriendlyName()}.");
                filePath = null;
                return false;
            }

            string ext = extAttrib.GetProperExtension(format);
            if (string.IsNullOrWhiteSpace(directory))
            {
                Engine.LogWarning($"Cannot export {fileName}.{ext}; no valid specified directory.");
                filePath = null;
                return false;
            }

            try
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
            }
            catch
            {
                Engine.LogWarning($"Cannot export to directory at {directory}.");
                filePath = null;
                return false;
            }

            fileName = string.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                directory += Path.DirectorySeparatorChar;

            filePath = directory + fileName + "." + ext; ;
            if (fileType is IFileObject fobj)
                fobj.FilePath = filePath;

            return true;
        }
    }
}
