using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class Deserializer : BaseSerializationIO
    {
        public async Task<object> DeserializeAsync(string filePath)
            => await DeserializeAsync(filePath, null, CancellationToken.None);

        public async Task<object> DeserializeAsync(
            string filePath,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            TypeProxy fileType = TFileObject.DetermineType(filePath, out EFileFormat format);
            if (format == EFileFormat.ThirdParty || fileType is null)
                return null;

            return await DeserializeAsync(filePath, fileType, (EProprietaryFileFormat)format, progress, cancel);
        }
        /// <summary>
        /// Reads the file at <paramref name="filePath"/> as a binary file.
        /// </summary>
        /// <param name="filePath">The path of the file to write.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        public async Task<object> DeserializeAsync(
            string filePath,
            TypeProxy fileType,
            EProprietaryFileFormat format,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return await DeserializeAsync(filePath, fileType, stream, format, progress, cancel);
            }
        }
        /// <summary>
        /// Reads the file at <paramref name="filePath"/> as a binary file.
        /// </summary>
        /// <param name="filePath">The path of the file to write.</param>
        /// <param name="progress">Handler for progress updates.</param>
        /// <param name="cancel">Handler for the caller to cancel the operation.</param>
        public async Task<object> DeserializeAsync(
            string filePath,
            TypeProxy fileType,
            Stream stream,
            EProprietaryFileFormat format,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            Format = format;
            Reader = GetReader(format, filePath, fileType, stream, progress, cancel);
            object file = await Reader.CreateObjectAsync();
            Engine.PrintLine($"Deserialized {format} file at {filePath}");
            return file;
        }
        private AbstractReader GetReader(
            EProprietaryFileFormat format,
            string filePath,
            TypeProxy fileType,
            Stream stream,
            IProgress<float> progress,
            CancellationToken cancel)
            => format switch
            {
                EProprietaryFileFormat.Binary => new ReaderBinary(this, filePath, fileType, stream, progress, cancel, null),
                EProprietaryFileFormat.XML => new ReaderXML(this, filePath, fileType, stream, progress, cancel, null),
                _ => throw new Exception(),
            };
    }
}
