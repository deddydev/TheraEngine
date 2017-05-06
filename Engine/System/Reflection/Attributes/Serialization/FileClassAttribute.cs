using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class FileClass : Attribute
    {
        public FileClass(
            string extension,
            string userFriendlyName,
            bool manualBinSerialize = false,
            bool manualXmlSerialize = false,
            SerializeFormat preferredFormat = SerializeFormat.XML)
        {
            _extension = extension;
            _manualBinSerialize = manualBinSerialize;
            _manualXmlSerialize = manualXmlSerialize;
            _userFriendlyName = userFriendlyName;
            _preferredFormat = preferredFormat;
        }

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
    }
}
