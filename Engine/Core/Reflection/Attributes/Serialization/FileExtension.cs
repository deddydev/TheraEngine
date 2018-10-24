using TheraEngine.Core.Files;

namespace System.ComponentModel
{
    /// <summary>
    /// This attribute can be used on <see cref="TFileObject"/> classes to define a proprietary (engine-exclusive) extension for the file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class FileExt : Attribute
    {
        public FileExt(string extension)
        {
            Extension = extension;
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
            = SerializeFormat.Binary;
#endif

        /// <summary>
        /// Retrieves the proper extension depending on the desired proprietary file format.
        /// </summary>
        public string GetProperExtension(EProprietaryFileFormat format)
            => format.ToString().ToLowerInvariant()[0] + Extension;
    }
}
