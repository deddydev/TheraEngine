using System.Collections.Generic;
using System.Linq;
using TheraEngine.Files;

namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class FileClass : Attribute
    {
        public FileClass(string extension, string userFriendlyName)
        {
            UserFriendlyName = userFriendlyName;
            Extension = extension;
        }

        private string _extension;

        public string UserFriendlyName { get; set; }
        public string Extension { get => _extension; set => _extension = value.ToLowerInvariant(); }

        public bool ManualXmlConfigSerialize { get; set; } = false;
        public bool ManualXmlStateSerialize { get; set; } = false;

        public bool ManualBinConfigSerialize { get; set; } = false;
        public bool ManualBinStateSerialize { get; set; } = false;

        /// <summary>
        /// Determines if the file needs special deserialization handling.
        /// If so, the class needs a constructor that takes the file's absolute path (string) as its only argument.
        /// </summary>
        public bool IsSpecialDeserialize { get; set; } = false;
        public string[] ImportableExtensions { get; set; } = null;
        public string[] ExportableExtensions { get; set; } = null;

        public SerializeFormat PreferredFormat { get; set; }
#if DEBUG
            = SerializeFormat.XML;
#else
            = SerializeFormat.Binary;
#endif

        /// <summary>
        /// Converts the desired format into the actual extension for this file in that format.
        /// </summary>
        public string GetProperExtension(ProprietaryFileFormat format)
        {
            return format.ToString().ToLowerInvariant()[0] + Extension;
        }
        /// <summary>
        /// Returns the filter for all extensions related to this format.
        /// </summary>
        /// <param name="proprietary">Add the TheraEngine proprietary formats (binary, xml, etc)</param>
        /// <param name="import3rdParty">Add any importable 3rd party file formats</param>
        /// <param name="export3rdParty">Add any exportable 3rd party file formats</param>
        /// <returns>The filter to be used in open file dialog, save file dialog, etc</returns>
        public string GetFilter(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false)
        {
            string allTypes = "";
            string eachType = "";
            string ext = Extension;
            bool first = true;
            if (proprietary)
                foreach (string type in Enum.GetNames(typeof(ProprietaryFileFormat)))
                {
                    if (first)
                        first = false;
                    else
                    {
                        allTypes += ";";
                        eachType += "|";
                    }
                    string fmt = String.Format("*.{0}{1}", type.Substring(0, 1).ToLowerInvariant(), ext);
                    eachType += String.Format("{0} [{2}] ({1})|{1}", UserFriendlyName, fmt, type);
                    allTypes += fmt;
                }
            if (import3rdParty)
                foreach (string ext3rd in ImportableExtensions)
                {
                    string extLower = ext3rd.ToLowerInvariant();
                    if (first)
                        first = false;
                    else
                    {
                        allTypes += ";";
                        eachType += "|";
                    }
                    string fmt = String.Format("*.{0}", extLower);
                    if (ExtensionNames3rdParty.ContainsKey(extLower))
                        eachType += String.Format("{0} ({1})|{1}", ExtensionNames3rdParty[extLower], fmt);
                    else
                        eachType += String.Format("{0} file ({1})|{1}", extLower, fmt);
                    allTypes += fmt;
                }
            if (export3rdParty)
                foreach (string ext3rd in ExportableExtensions)
                {
                    if (import3rdParty && ImportableExtensions != null && ImportableExtensions.Contains(ext3rd, StringComparer.InvariantCultureIgnoreCase))
                        continue;

                    string extLower = ext3rd.ToLowerInvariant();

                    if (first)
                        first = false;
                    else
                    {
                        allTypes += ";";
                        eachType += "|";
                    }
                    string fmt = String.Format("*.{0}", extLower);
                    if (ExtensionNames3rdParty.ContainsKey(extLower))
                        eachType += String.Format("{0} ({1})|{1}", ExtensionNames3rdParty[extLower], fmt);
                    else
                        eachType += String.Format("{0} file ({1})|{1}", extLower, fmt);
                    allTypes += fmt;
                }

            string allTypesFull = String.Format("{0} ({1})|{1}", UserFriendlyName, allTypes);
            return allTypesFull + "|" + eachType;
        }

        public static bool Has3rdPartyExtension(string ext)
            => ExtensionNames3rdParty.ContainsKey(ext.ToLowerInvariant());       

        private static Dictionary<string, string> ExtensionNames3rdParty = new Dictionary<string, string>()
        {
            { "dae", "Collada Scene" }
        };

        public static void Register3rdPartyExtension(string extension, string userFriendlyName)
        {
            extension = extension.ToLowerInvariant();
            if (!ExtensionNames3rdParty.ContainsKey(extension))
                ExtensionNames3rdParty.Add(extension, userFriendlyName);
            else
                ExtensionNames3rdParty[extension] = userFriendlyName;
        }
    }
}
