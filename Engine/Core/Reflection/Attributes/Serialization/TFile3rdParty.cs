using System.Collections.Generic;

namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TFile3rdParty : Attribute
    {
        public TFile3rdParty(params string[] extensions)
        {
            ImportableExtensions = extensions ?? new string[0];
            ExportableExtensions = extensions ?? new string[0];
        }
        public TFile3rdParty(string[] importableExtensions, string[] exportableExtensions)
        {
            ImportableExtensions = importableExtensions ?? new string[0];
            ExportableExtensions = exportableExtensions ?? new string[0];
        }
        
        public string[] ImportableExtensions { get; private set; }
        public string[] ExportableExtensions { get; private set; }
        public bool AsyncManualRead { get; set; } = false;
        public bool AsyncManualWrite { get; set; } = false;

        public bool HasAnyExtensions => ImportableExtensions.Length + ExportableExtensions.Length > 0;

        public static bool Has3rdPartyExtension(string ext)
        {
            if (string.IsNullOrWhiteSpace(ext))
                return false;
            ext = ext.ToLowerInvariant();
            if (ext[0] == '.')
                ext = ext.Substring(0);
            return ExtensionNames3rdParty.ContainsKey(ext);
        }

        public static Dictionary<string, string> ExtensionNames3rdParty = new Dictionary<string, string>()
        {
            { "obj", "OBJ Static Mesh" },
            { "mtl", "OBJ Static Mesh Material" },
            { "dae", "Khronos Group Collada Scene" },
            { "png", "Portable Network Graphics Image" },
            { "jpg", "Joint Photographic Group Image" },
            { "jpeg", "Joint Photographic Experts Group Image" },
            { "tiff", "Tagged Image File Format" },
            { "tga", "Truevision Graphics Adapter Image" },
            { "dds", "Microsoft DirectDraw Surface Image" },
            { "gif", "Graphics Interchange Format" },
            { "rtf", "Rich Text File" },
            { "txt", "Text File" },
            { "py", "Python Script" },
            { "pmx", "Miku Miku Dance Extended Model" },
        };

        public static void Register3rdPartyExtension(string extension, string userFriendlyName)
        {
            extension = extension.ToLowerInvariant();
            if (!ExtensionNames3rdParty.ContainsKey(extension))
                ExtensionNames3rdParty.Add(extension, userFriendlyName);
            else
                ExtensionNames3rdParty[extension] = userFriendlyName;
        }

        public bool HasExtension(string ext)
        {
            return 
                ExportableExtensions.Contains(ext, StringComparison.InvariantCultureIgnoreCase) ||
                ImportableExtensions.Contains(ext, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
