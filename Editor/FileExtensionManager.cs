using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Files
{
    public static class FileExtensionManager
    {
        public static readonly SupportedFileInfo[] Files =
        {
            new SupportedFileInfo(true, "World", "cworld"),
            new SupportedFileInfo(true, "Map", "cmap"),
            new SupportedFileInfo(true, "Actor", "cactor"),
            new SupportedFileInfo(true, "Component", "ccomp"),
            new SupportedFileInfo(true, "Camera", "ccam"),
            new SupportedFileInfo(true, "Property Animation Group", "cpag"),
            new SupportedFileInfo(true, "Property Animation", "cpa"),

            new SupportedFileInfo(false, "Portable Network Graphics", "png"),
            new SupportedFileInfo(false, "Truevision TARGA", "tga"),
            new SupportedFileInfo(false, "Tagged Image File Format", "tif", "tiff"),
            new SupportedFileInfo(false, "Bitmap", "bmp"),
            new SupportedFileInfo(false, "JPEG Image", "jpg", "jpeg"),
            new SupportedFileInfo(false, "Graphics Interchange Format", "gif"),
            
            new SupportedFileInfo(false, "Text File", "txt"),
            new SupportedFileInfo(false, "Uncompressed PCM", "wav"),
            new SupportedFileInfo(false, "3D Object Mesh", "obj"),
            new SupportedFileInfo(false, "Raw Data", "*"),
        };

        private static string _allSupportedFilter = null;
        private static string _filterList = null;

        private static string _allSupportedFilterEditable = null;
        private static string _filterListEditable = null;

        public static SupportedFileInfo[] GetInfo(params string[] extensions)
        {
            SupportedFileInfo[] infoArray = new SupportedFileInfo[extensions.Length];
            foreach (SupportedFileInfo fileInfo in Files)
                foreach (string ext in fileInfo._extensions)
                {
                    int index = extensions.IndexOf(ext);
                    if (index >= 0 && !infoArray.Contains(fileInfo))
                    {
                        infoArray[index] = fileInfo;
                        extensions[index] = null;
                    }
                }

            //Add remaining extensions not included in the supported formats array
            string s;
            for (int i = 0; i < extensions.Length; i++)
                if (!String.IsNullOrEmpty(s = extensions[i]))
                    infoArray[i] = new SupportedFileInfo(false, String.Format("{0} File", s.ToUpper()), s);

            return infoArray;
        }

        public static string CompleteFilterEditableOnly { get { return GetAllSupportedFilter(true) + "|" + GetListFilter(true); } }

        public static string CompleteFilter { get { return GetAllSupportedFilter(false) + "|" + GetListFilter(false); } }

        public static string GetCompleteFilter(params string[] extensions)
        {
            return GetCompleteFilter(GetInfo(extensions));
        }

        public static string GetCompleteFilter(SupportedFileInfo[] files)
        {
            if (files.Length == 0)
                return "All Files (*.*)|*.*";

            //No need for the all filter if there's only one filter
            if (files.Length == 1)
                return GetListFilter(files);

            return GetAllSupportedFilter(files) + "|" + GetListFilter(files);
        }

        public static string GetAllSupportedFilter(bool editableOnly)
        {
            if (editableOnly && _allSupportedFilterEditable != null)
                return _allSupportedFilterEditable;
            else if (!editableOnly && _allSupportedFilter != null)
                return _allSupportedFilter;

            if (editableOnly)
                return _allSupportedFilterEditable = GetAllSupportedFilter(Files, true);
            else
                return _allSupportedFilter = GetAllSupportedFilter(Files, false);
        }

        const int MaxExtensionsInAllFilter = 5;

        public static string GetAllSupportedFilter(SupportedFileInfo[] files, bool editableOnly = false)
        {
            string filter = "All Supported Formats (";
            string filter2 = "|";

            //The "all supported formats" string needs (*.*) in it
            //or else the window will display EVERY SINGLE FILTER
            bool doNotAdd = files.Length > MaxExtensionsInAllFilter;
            if (doNotAdd)
                filter += "*.*";

            IEnumerable<SupportedFileInfo> e;
            if (editableOnly)
                e = files.Where(x => x._forEditing);
            else
                e = files;

            string[] fileTypeExtensions = e.Select(x => x.ExtensionsFilter).ToArray();
            for (int i = 0; i < fileTypeExtensions.Length; i++)
            {
                string[] extensions = fileTypeExtensions[i].Split(';');
                string n = "";
                for (int x = 0; x < extensions.Length; x++)
                {
                    string ext = extensions[x];
                    string rawExtName = ext.Substring(ext.IndexOf('.') + 1);
                    //if (!rawExtName.Contains("*"))
                    n += (x != 0 ? ";" : "") + ext;
                }
                filter2 += (i != 0 ? ";" : "") + n;
                if (!doNotAdd)
                    filter += (i != 0 ? ", " : "") + n;
            }
            return filter + ")" + filter2;
        }

        public static string GetListFilter(params string[] extensions)
        {
            return GetListFilter(GetInfo(extensions));
        }

        public static string GetListFilter(bool editableOnly)
        {
            if (editableOnly && _filterListEditable != null)
                return _filterListEditable;
            else if (!editableOnly && _filterList != null)
                return _filterList;

            if (editableOnly)
                return _filterListEditable = GetListFilter(Files, true);
            else
                return _filterList = GetListFilter(Files, false);
        }

        public static string GetListFilter(SupportedFileInfo[] files, bool editableOnly = false)
        {
            string filter = "";

            IEnumerable<SupportedFileInfo> e;
            if (editableOnly)
                e = files.Where(x => x._forEditing);
            else
                e = files;

            string[] fileTypeExtensions = e.Select(x => x.Filter).ToArray();
            for (int i = 0; i < fileTypeExtensions.Length; i++)
                filter += fileTypeExtensions[i] + (i == fileTypeExtensions.Length - 1 ? "" : "|");
            return filter;
        }
    }

    public class SupportedFileInfo
    {
        public string _name;
        public string[] _extensions;
        public bool _forEditing;

        public SupportedFileInfo(bool forEditing, string name, params string[] extensions)
        {
            _forEditing = forEditing;
            _name = name;
            if (extensions == null || extensions.Length == 0)
                throw new Exception("No extensions for file type \"" + _name + "\".");
            _extensions = extensions;
        }

        public string Filter { get { string s = ExtensionsFilter; return _name + " (" + s.Replace(";", ", ") + ")|" + s; } }
        public string ExtensionsFilter
        {
            get
            {
                string filter = "";
                bool first = true;
                foreach (string ext in _extensions)
                {
                    if (!first)
                        filter += ";";

                    //In case of a specific file name
                    if (!ext.Contains('.'))
                        filter += "*.";

                    filter += ext;

                    first = false;
                }
                return filter;
            }
        }
    }
}
