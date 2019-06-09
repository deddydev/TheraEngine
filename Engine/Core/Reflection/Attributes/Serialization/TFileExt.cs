using System.Runtime.Serialization;
using TheraEngine.Core.Files;

namespace System.ComponentModel
{
    /// <summary>
    /// This attribute can be used on <see cref="TFileObject"/> classes to define a proprietary (engine-exclusive) extension for the file.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TFileExt : Attribute, ISerializable
    {
        public TFileExt(string proprietaryExtension, params string[] importExportExtensions)
        {
            Extension = proprietaryExtension;
            ImportableExtensions = importExportExtensions ?? new string[0];
            ExportableExtensions = importExportExtensions ?? new string[0];
        }
        public TFileExt(string proprietaryExtension, string[] importableExtensions, string[] exportableExtensions)
        {
            Extension = proprietaryExtension;
            ImportableExtensions = importableExtensions ?? new string[0];
            ExportableExtensions = exportableExtensions ?? new string[0];
        }
        public TFileExt(SerializationInfo info, StreamingContext context)
        {
            Extension = info.GetString(nameof(Extension));
            ImportableExtensions = info.GetValue(nameof(ImportableExtensions), typeof(string[])) as string[];
            ExportableExtensions = info.GetValue(nameof(ExportableExtensions), typeof(string[])) as string[];
            ManualBinConfigSerialize = info.GetBoolean(nameof(ManualBinConfigSerialize));
            ManualBinStateSerialize = info.GetBoolean(nameof(ManualBinStateSerialize));
            ManualXmlConfigSerialize = info.GetBoolean(nameof(ManualXmlConfigSerialize));
            ManualXmlStateSerialize = info.GetBoolean(nameof(ManualXmlStateSerialize));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Extension), Extension);
            info.AddValue(nameof(ImportableExtensions), ImportableExtensions);
            info.AddValue(nameof(ExportableExtensions), ExportableExtensions);
            info.AddValue(nameof(ManualBinConfigSerialize), ManualBinConfigSerialize);
            info.AddValue(nameof(ManualBinStateSerialize), ManualBinStateSerialize);
            info.AddValue(nameof(ManualXmlConfigSerialize), ManualXmlConfigSerialize);
            info.AddValue(nameof(ManualXmlStateSerialize), ManualXmlStateSerialize);
        }

        private string _extension;

        /// <summary>
        /// The extension for the file. Depending on the <see cref="EProprietaryFileFormat"/>, x, b, etc will be appended to the front of the extension.
        /// </summary>
        public string Extension
        {
            get => _extension;
            set => _extension = value.ToLowerInvariant();
        }

        /// <summary>
        /// If true, the file will be de/serialized manually when im/exported as XML config.
        /// </summary>
        public bool ManualXmlConfigSerialize { get; set; } = false;
        /// <summary>
        /// If true, the file will be de/serialized manually when im/exported as XML state.
        /// </summary>
        public bool ManualXmlStateSerialize { get; set; } = false;
        /// <summary>
        /// If true, the file will be de/serialized manually when im/exported as binary config.
        /// </summary>
        public bool ManualBinConfigSerialize { get; set; } = false;
        /// <summary>
        /// If true, the file will be de/serialized manually when im/exported as binary state.
        /// </summary>
        public bool ManualBinStateSerialize { get; set; } = false;
        
        public EProprietaryFileFormat PreferredFormat { get; set; }
#if DEBUG
            = EProprietaryFileFormat.XML;
#else
            = EProprietaryFileFormat.Binary;
#endif

        /// <summary>
        /// Retrieves the proper extension depending on the desired proprietary file format.
        /// </summary>
        public string GetFullExtension(EProprietaryFileFormat format)
            => format.ToString().ToLowerInvariant()[0] + Extension;
        
        public string[] ImportableExtensions { get; private set; }
        public string[] ExportableExtensions { get; private set; }
        
        public bool HasAnyImportableExportableExtensions => ImportableExtensions.Length + ExportableExtensions.Length > 0;
        public bool HasAnyImportableExtensions => ImportableExtensions.Length > 0;
        public bool HasAnyExportableExtensions => ExportableExtensions.Length > 0;
        public bool HasImportableExtension(string ext)
            => ImportableExtensions.Contains(ext, StringComparison.InvariantCultureIgnoreCase);
        public bool HasExportableExtension(string ext)
            => ExportableExtensions.Contains(ext, StringComparison.InvariantCultureIgnoreCase);
    }
}
