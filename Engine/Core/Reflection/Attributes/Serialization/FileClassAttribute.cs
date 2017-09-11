using System.Collections.Generic;
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
        public string GetFilter(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false)
        {
            string allTypes = "";
            string eachType = "";
            string ext = Extension;
            bool first = true;
            if (proprietary)
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
            if (import3rdParty)
                foreach (string ext3rd in ImportableExtensions)
                {
                    if (first)
                        first = false;
                    else
                    {
                        allTypes += ";";
                        eachType += "|";
                    }
                    string fmt = String.Format("*.{0}", ext3rd);
                    if (ExtensionNames3rdParty.ContainsKey(ext3rd.ToUpper()))
                        eachType += String.Format("{0} ({1})|{1}", ExtensionNames3rdParty[ext3rd.ToUpper()], fmt);
                    else
                        eachType += String.Format("{0} file ({1})|{1}", fmt, ext3rd);
                    allTypes += fmt;
                }
            if (export3rdParty)
                foreach (string ext3rd in ExportableExtensions)
                {
                    if (first)
                        first = false;
                    else
                    {
                        allTypes += ";";
                        eachType += "|";
                    }
                    string fmt = String.Format("*.{0}", ext3rd);
                    if (ExtensionNames3rdParty.ContainsKey(ext3rd.ToUpper()))
                        eachType += String.Format("{0} ({1})|{1}", ExtensionNames3rdParty[ext3rd.ToUpper()], fmt);
                    else
                        eachType += String.Format("{0} file ({1})|{1}", fmt, ext3rd);
                    allTypes += fmt;
                }

            string allTypesFull = String.Format("{0} ({1})|{1}", UserFriendlyName, allTypes);
            return allTypesFull + "|" + eachType;
        }

        private static Dictionary<string, string> ExtensionNames3rdParty = new Dictionary<string, string>()
        {
            { "DAE", "Collada Scene" }
        };

        public static void Register3rdPartyExtension(string extension, string userFriendlyName)
        {
            if (!ExtensionNames3rdParty.ContainsKey(extension))
                ExtensionNames3rdParty.Add(extension, userFriendlyName);
            else
                ExtensionNames3rdParty[extension] = userFriendlyName;
        }
    }
}
