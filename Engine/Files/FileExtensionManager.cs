using CustomEngine.Cutscenes;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering.Cameras;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Files
{
    public static class FileExtensionManager
    {
        public static readonly Dictionary<Type, FilterInfo> ImportInfo = new Dictionary<Type, FilterInfo>()
        {
            { typeof(World), new FilterInfo("World", "cworld") },
            { typeof(Map), new FilterInfo("Map", "cmap") },
            { typeof(Actor), new FilterInfo("Actor", "cactor") },
            { typeof(Component), new FilterInfo("Component", "ccomp") },
            { typeof(Model), new FilterInfo("Model", "cmdl") },
            { typeof(Camera), new FilterInfo("Camera", "ccam") },
            { typeof(Cutscene), new FilterInfo("Cutscene", "ccut") },
            { typeof(Component), new FilterInfo("Component", "ccomp") },
            { typeof(AnimationContainer), new FilterInfo("Animation Archive", "cpac") },
            { typeof(PropertyAnimation), new FilterInfo("Animation Archive", "cpac") },


            //new SupportedFileInfo(true, "Map", "cmap"),
            //new SupportedFileInfo(true, "Actor", "cactor"),W
            //new SupportedFileInfo(true, "Component", "ccomp"),
            //new SupportedFileInfo(true, "Camera", "ccam"),
            //new SupportedFileInfo(true, "Shape", "cshape"),
            //new SupportedFileInfo(true, "Property Animation Group", "cpag"),
            //new SupportedFileInfo(true, "Property Animation", "cpa"),

            //new SupportedFileInfo(false, "Portable Network Graphics", "png"),
            //new SupportedFileInfo(false, "Truevision TARGA", "tga"),
            //new SupportedFileInfo(false, "Tagged Image File Format", "tif", "tiff"),
            //new SupportedFileInfo(false, "Bitmap", "bmp"),
            //new SupportedFileInfo(false, "JPEG Image", "jpg", "jpeg"),
            //new SupportedFileInfo(false, "Graphics Interchange Format", "gif"),
            
            //new SupportedFileInfo(false, "Text File", "txt"),
            //new SupportedFileInfo(false, "Uncompressed PCM", "wav"),
            //new SupportedFileInfo(false, "3D Object Mesh", "obj"),
            //new SupportedFileInfo(false, "Raw Data", "*"),
        };

        private static string _allSupportedFilter = null;
        private static string _filterList = null;

        private static string _allSupportedFilterEditable = null;
        private static string _filterListEditable = null;

        public static FilterInfo[] GetInfo(params string[] extensions)
        {
            FilterInfo[] infoArray = new FilterInfo[extensions.Length];
            foreach (FilterInfo fileInfo in Files)
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
                    infoArray[i] = new FilterInfo(false, String.Format("{0} File", s.ToUpper()), s);

            return infoArray;
        }

        public static string CompleteFilterEditableOnly { get { return GetAllSupportedFilter(true) + "|" + GetListFilter(true); } }

        public static string CompleteFilter { get { return GetAllSupportedFilter(false) + "|" + GetListFilter(false); } }

        public static string GetCompleteFilter(params string[] extensions)
        {
            return GetCompleteFilter(GetInfo(extensions));
        }

        public static string GetCompleteFilter(params Type[] types)
        {
            return GetCompleteFilter(GetInfo(extensions));
        }

        public static string GetCompleteFilter(FilterInfo[] files)
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

        public static string GetAllSupportedFilter(FilterInfo[] files, bool editableOnly = false)
        {
            string filter = "All Supported Formats (";
            string filter2 = "|";

            //The "all supported formats" string needs (*.*) in it
            //or else the window will display EVERY SINGLE FILTER
            bool doNotAdd = files.Length > MaxExtensionsInAllFilter;
            if (doNotAdd)
                filter += "*.*";

            IEnumerable<FilterInfo> e;
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

        public static string GetListFilter(FilterInfo[] files, bool editableOnly = false)
        {
            string filter = "";

            IEnumerable<FilterInfo> e;
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

    public class FilterInfo
    {
        public string _name;
        public string _primaryExtension;
        public string[] _otherExtensions;

        /// <summary>
        /// Constructs a new instance containing filter info for a file type.
        /// </summary>
        /// <param name="name">The name of the file type.</param>
        /// <param name="primaryExtension">The file type's main file format extension.</param>
        /// <param name="otherExtensions">Various other extensions that this file type can be stored in.</param>
        public FilterInfo(string name, string primaryExtension, params string[] otherExtensions)
        {
            _name = name;
            _primaryExtension = primaryExtension;
            _otherExtensions = otherExtensions;
        }

        public string Filter { get { string s = ExtensionsFilter; return _name + " (" + s.Replace(";", ", ") + ")|" + s; } }
        public string ExtensionsFilter
        {
            get
            {
                string filter = "";
                if (!_primaryExtension.Contains("."))
                    filter += "*.";
                filter += _primaryExtension;
                foreach (string ext in _otherExtensions)
                {
                    filter += ";";
                    if (!ext.Contains('.'))
                        filter += "*.";
                    filter += ext;
                }
                return filter;
            }
        }
    }
}
