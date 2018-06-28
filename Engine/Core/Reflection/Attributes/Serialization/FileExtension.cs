using TheraEngine.Files;

namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class FileExt : Attribute
    {
        public FileExt(string extension)
        {
            Extension = extension;
        }

        private string _extension;
        public string Extension
        {
            get => _extension;
            set => _extension = value.ToLowerInvariant();
        }

        public bool ManualXmlConfigSerialize { get; set; } = false;
        public bool ManualXmlStateSerialize { get; set; } = false;

        public bool ManualBinConfigSerialize { get; set; } = false;
        public bool ManualBinStateSerialize { get; set; } = false;
        
        public SerializeFormat PreferredFormat { get; set; }
#if DEBUG
            = SerializeFormat.XML;
#else
            = SerializeFormat.Binary;
#endif

        /// <summary>
        /// Converts the desired format into the actual extension for this file in that format.
        /// </summary>
        public string GetProperExtension(EProprietaryFileFormat format)
            => format.ToString().ToLowerInvariant()[0] + Extension;
    }
}
