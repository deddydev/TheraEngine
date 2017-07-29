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
            _extension = extension.ToLower();
            _manualBinSerialize = manualBinSerialize;
            _manualXmlSerialize = manualXmlSerialize;
            _userFriendlyName = userFriendlyName;
            _preferredFormat = preferredFormat;
            _isSpecialDeserialize = isSpecialDeserialize;
        }
        
        //TODO: add importable and exportable 3rd party extensions

        private string _userFriendlyName;
        private string _extension;
        private bool _manualXmlSerialize = false;
        private bool _manualBinSerialize = false;
        private SerializeFormat _preferredFormat =
#if DEBUG
            SerializeFormat.XML;
#else
            SerializeFormat.Binary;
#endif
        private bool _isSpecialDeserialize = false;

        public SerializeFormat PreferredFormat
        {
            get => _preferredFormat;
            set => _preferredFormat = value;
        }
        public bool ManualXmlSerialize
        {
            get => _manualXmlSerialize;
            set => _manualXmlSerialize = value;
        }
        public bool ManualBinSerialize
        {
            get => _manualBinSerialize;
            set => _manualBinSerialize = value;
        }
        public string UserFriendlyName
        {
            get => _userFriendlyName;
            set => _userFriendlyName = value;
        }
        public string Extension
        {
            get => _extension;
            set => _extension = value;
        }
        public bool IsSpecialDeserialize
        {
            get => _isSpecialDeserialize;
            set => _isSpecialDeserialize = value;
        }

        public string GetProperExtension(FileFormat format)
        {
            return format.ToString().ToLower()[0] + _extension;
        }
        public string GetFilter()
        {
            string allTypes = "";
            string eachType = "";
            string ext = _extension;
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
