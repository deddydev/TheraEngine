using Extensions;
using SevenZip;
using System;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class Serializer : BaseSerializationIO
    {
        public class Options
        {
            public Endian.EOrder Endian { get; set; } = Memory.Endian.EOrder.Little;
            public bool Encrypted { get; set; } = false;
            public bool Compressed { get; set; } = false;
            public string EncryptionPassword { get; set; } = null;
            public ICodeProgress CompressionProgress { get; set; } = null;
            public XmlWriterSettings XMLSettings { get; set; } = null;
        }

        /// <summary>
        /// Writes <paramref name="fileObject"/> as an XML file.
        /// </summary>
        /// <param name="fileObject">The object to serialize into XML.</param>
        /// <param name="filePath">The path of the file to write.</param>
        /// <param name="flags">Flags to determine what information to serialize.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        public async Task SerializeAsync(
            object fileObject,
            string filePath,
            Stream stream,
            EProprietaryFileFormat format,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            if (PreExport(fileObject, filePath, format, null))
            {
                Writer = GetWriter(format, fileObject, filePath, stream, flags, progress, cancel);
                await Writer.WriteObjectAsync();
                //Engine.PrintLine("Serialized XML file to {0}", filePath);
            }
        }
        /// <summary>
        /// Writes <paramref name="fileObject"/> as an XML file.
        /// </summary>
        /// <param name="fileObject">The object to serialize into XML.</param>
        /// <param name="targetDirectoryPath">The path to a directory to write the file in.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="flags">Flags to determine what information to serialize.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        public async Task SerializeAsync(
            object fileObject,
            string targetDirectoryPath,
            string fileName,
            EProprietaryFileFormat format,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            if (!PreExport(fileObject, targetDirectoryPath, fileName, format, null, out string filePath))
                return;
            
            using Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            Writer = GetWriter(format, fileObject, filePath, stream, flags, progress, cancel);
            await Writer.WriteObjectAsync();
        }

        private AbstractWriter GetWriter(
            EProprietaryFileFormat format,
            object fileObject,
            string filePath,
            Stream stream,
            ESerializeFlags flags,
            IProgress<float> progress,
            CancellationToken cancel)
            => format switch
            {
                EProprietaryFileFormat.Binary => new WriterBinary(this, fileObject, filePath, stream, flags, progress, cancel, Endian.EOrder.Big, false, true, null, null),
                EProprietaryFileFormat.XML => new WriterXML(this, fileObject, filePath, stream, flags, progress, cancel, null),
                _ => throw new Exception(),
            };

        public static bool PreExport(object fileObject, string filePath, EProprietaryFileFormat format, TFileExt extOverride)
        {
            filePath = null;

            if (RemotingServices.IsTransparentProxy(fileObject))
            {
                Engine.LogWarning("Cannot properly serialize files from other AppDomains. Errors will be thrown on serialized private properties.");
                return false;
            }

            TypeProxy fileType = fileObject.GetTypeProxy();
            TFileExt extAttrib = extOverride ?? TFileObject.GetFileExtension(fileType);
            if (extAttrib is null)
            {
                Engine.LogWarning($"No {nameof(TFileExt)} attribute specified for {fileType.GetFriendlyName()}.");
                return false;
            }

            string ext = extAttrib.GetFullExtension(format);
            if (!filePath.EndsWith(ext))
            {
                Engine.LogWarning($"Path '{filePath}' needs to end with extension {ext}.");
                return false;
            }

            if (fileObject is IFileObject fobj)
                fobj.FilePath = filePath;

            return true;
        }
        public static bool PreExport(object fileObject, string directory, string fileName, EProprietaryFileFormat format, TFileExt extOverride, out string filePath)
        {
            filePath = null;

            if (RemotingServices.IsTransparentProxy(fileObject))
            {
                Engine.LogWarning("Cannot properly serialize files from other AppDomains! Errors will be thrown on serialized private properties.");
                return false;
            }

            TypeProxy fileType = fileObject.GetTypeProxy();
            TFileExt extAttrib = extOverride ?? TFileObject.GetFileExtension(fileType);
            if (extAttrib is null)
            {
                Engine.LogWarning($"No {nameof(TFileExt)} attribute specified for {fileType.GetFriendlyName()}.");
                return false;
            }

            string ext = extAttrib.GetFullExtension(format);
            if (string.IsNullOrWhiteSpace(directory))
            {
                Engine.LogWarning($"Cannot export {fileName}.{ext}; no valid specified directory.");
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
                return false;
            }

            fileName = string.IsNullOrEmpty(fileName) ? "NewFile" : fileName;

            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
                directory += Path.DirectorySeparatorChar;

            filePath = directory + fileName + "." + ext;
            if (fileObject is IFileObject fobj)
                fobj.FilePath = filePath;

            return true;
        }
    }
}
