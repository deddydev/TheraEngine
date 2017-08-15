using TheraEngine.Files;

namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class FileClass : Attribute
    {
        public FileClass(
            string extension,
            string userFriendlyName,
            bool isSpecialDeserialize = false,
            bool manualBinSerialize = false,
            bool manualXmlSerialize = false,
            SerializeFormat preferredFormat =
#if DEBUG
            SerializeFormat.XML
#else
            SerializeFormat.Binary
#endif
            )
        {
            UserFriendlyName = userFriendlyName;
            Extension = extension.ToLower();
            ManualBinSerialize = manualBinSerialize;
            ManualXmlSerialize = manualXmlSerialize;
            PreferredFormat = preferredFormat;
            IsSpecialDeserialize = isSpecialDeserialize;
        }
        
        public string UserFriendlyName { get; set; }
        public string Extension { get; set; }
        public bool ManualXmlSerialize { get; set; } = false;
        public bool ManualBinSerialize { get; set; } = false;
        public bool IsSpecialDeserialize { get; set; } = false;
        public string[] ImportableExtensions { get; set; } = null;
        public string[] ExportableExtensions { get; set; } = null;
#if DEBUG
        public SerializeFormat PreferredFormat { get; set; } = SerializeFormat.XML;
#else
        public SerializeFormat PreferredFormat { get; set; } = SerializeFormat.Binary;
#endif

        public string GetProperExtension(FileFormat format)
        {
            return format.ToString().ToLower()[0] + Extension;
        }
        public string GetFilter()
        {
            string allTypes = "";
            string eachType = "";
            string ext = Extension;
            bool first = true;
            foreach (string type in Enum.GetNames(typeof(FileFormat)))
            {
                if (first)
                    first = false;
                else
                {
                    allTypes += ";";
                    eachType += "|";
                }
                string fmt = String.Format("*.{0}{1}", type.Substring(0, 1).ToLower(), ext);
                eachType += String.Format("{0} [{2}] ({1})|{1}", UserFriendlyName, fmt, type);
                allTypes += fmt;
            }
            string allTypesFull = String.Format("{0} ({1})|{1}", UserFriendlyName, allTypes);
            return allTypesFull + "|" + eachType;
        }
    }
}
