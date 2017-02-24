using CustomEngine;
using CustomEngine.Cutscenes;
using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering.Cameras;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Textures;
using CustomEngine.Audio;
using CustomEngine.Worlds;
using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomEngine.Worlds.Actors;
using CustomEngine.Worlds.Maps;

namespace CustomEngine.Files
{
    public static class FileManager
    {
        public static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
        }

        public static string GetTag(Type type)
        {
            return Filters[type]._tag;
        }
        public static Type GetType(string tag)
        {
            return Filters.FirstOrDefault(x => x.Value._tag == tag).Key;
        }
        public static string GetExtension(Type type)
        {
            return Filters[type]._extensions[0];
        }

        public static Type GetTypeWithExtension(string ext)
        {
            if (ext.StartsWith("."))
                ext = ext.Substring(1);
            ext = ext.ToLower();
            return Filters.FirstOrDefault(x => x.Value._extensions[0] == ext).Key;
        }
        public static ResourceType? GetResourceTypeWithExtension(string ext)
        {
            var kv = Filters.FirstOrDefault(x => x.Value._extensions[0] == ext);
            if (kv.Value == null)
                return null;
            return kv.Value._resourceType;
        }

        const int MaxExtensionsInAllFilter = 5;
        private static string _allSupportedFilter = null;
        private static string _filterList = null;
        public static readonly Dictionary<Type, FilterInfo> Filters = new Dictionary<Type, FilterInfo>()
        {   
            { typeof(UserSettings), new FilterInfo("User Settings", "SETT", ResourceType.UserSettings, "csettings") },
            { typeof(EngineSettings), new FilterInfo("Engine Settings", "SETT", ResourceType.EngineSettings, "csettings") },

            { typeof(IMultiFileRef), new FilterInfo("Multi File Reference", "FREF", ResourceType.MultiFileRef, "cmref") },
            { typeof(ISingleFileRef), new FilterInfo("Single File Reference", "FREF", ResourceType.SingleFileRef, "csref") },

            { typeof(World), new FilterInfo("World", "CWRL", ResourceType.World, "cworld") },
            { typeof(WorldSettings), new FilterInfo("World Settings", "SETT", ResourceType.WorldSettings, "csettings") },
            { typeof(WorldState), new FilterInfo("World State", "STAT", ResourceType.WorldState, "cstate") },

            { typeof(Map), new FilterInfo("Map", "CMAP", ResourceType.Map, "cmap") },
            { typeof(MapSettings), new FilterInfo("Map Settings", "SETT", ResourceType.MapSettings, "csettings") },
            { typeof(MapState), new FilterInfo("Map State", "STAT", ResourceType.MapState, "cstate") },

            { typeof(Actor), new FilterInfo("Actor", "CACT", ResourceType.Actor, "cactor") },
            //{ typeof(ActorSettings), new FilterInfo("Actor Settings", "SETT", ResourceType.ActorSettings, "csettings") },
            //{ typeof(ActorState), new FilterInfo("Actor State", "STAT", ResourceType.ActorState, "cstate") },
            
            { typeof(Component), new FilterInfo("Component", "COMP", ResourceType.Component, "ccomp") },

            { typeof(SkeletalMesh), new FilterInfo("Skeletal Model", "SKMD", ResourceType.SkeletalMesh, "cmdl") },
            { typeof(StaticMesh), new FilterInfo("Static Model", "STMD", ResourceType.SkeletalMesh, "cmdl") },
            { typeof(Skeleton), new FilterInfo("Model Skeleton", "SKEL", ResourceType.Skeleton, "cskl") },

            { typeof(Camera), new FilterInfo("Camera", "CCAM", ResourceType.Camera, "ccam") },
            { typeof(Cutscene), new FilterInfo("Cutscene", "CCUT", ResourceType.Cutscene, "ccut") },
            { typeof(AnimationContainer), new FilterInfo("Animation Archive", "ANMA",ResourceType.AnimationContainer,  "cpac") },
            { typeof(AnimationScalar), new FilterInfo("Numeric Property Animation", "PANM", ResourceType.AnimationScalar, "cpa") },
            { typeof(AnimationString), new FilterInfo("String Property Animation", "PANM", ResourceType.AnimationString, "cpa") },
            { typeof(AnimationBool), new FilterInfo("Boolean Animation", "PANM", ResourceType.AnimationBool, "cpa") },
            { typeof(Texture), new FilterInfo("Texture", "CTEX", ResourceType.Texture, "ctex") },
        };
        public static List<FilterInfo> GenericInfo = new List<FilterInfo>()
        {
            new FilterInfo("Portable Network Graphics", "png", ResourceType.Texture)
            { _fileTypes = new Type[] { typeof(Texture) },
                _canExport = true, _canImport = true },

            new FilterInfo("Truevision TARGA", "tga", ResourceType.Texture)
            { _fileTypes = new Type[] { typeof(Texture) },
                _canExport = true, _canImport = true },

            new FilterInfo("Tagged Image File Format", "tif", ResourceType.Texture, "tiff")
            { _fileTypes = new Type[] { typeof(Texture) },
                _canExport = true, _canImport = true },

            new FilterInfo("Bitmap", "bmp", ResourceType.Texture)
            { _fileTypes = new Type[] { typeof(Texture) },
                _canExport = true, _canImport = true },

            new FilterInfo("JPEG Image", "jpg", ResourceType.Texture, "jpeg")
            { _fileTypes = new Type[] { typeof(Texture) },
                _canExport = true, _canImport = true },

            new FilterInfo("Graphics Interchange Format", "gif", ResourceType.Texture)
            { _fileTypes = new Type[] { typeof(Texture) },
                _canExport = true, _canImport = true },

            new FilterInfo("Text File", "txt")
            { _fileTypes = new Type[] {
                typeof(AnimationScalar),
                typeof(AnimationBool),
                typeof(AnimationString),
                typeof(AnimationContainer) },
                _canExport = true, _canImport = true },

            new FilterInfo("Uncompressed PCM", "wav", ResourceType.Sound)
            { _fileTypes = new Type[] { typeof(SoundDataBase) },
                _canExport = true, _canImport = true },

            new FilterInfo("3D Mesh", "obj")
            { _fileTypes = new Type[] { typeof(StaticMesh) },
                _canExport = true, _canImport = true },

            new FilterInfo("Autodesk Scene", "fbx")
            { _fileTypes = new Type[] {
                typeof(SkeletalMesh),
                typeof(StaticMesh),
                typeof(AnimationContainer) },
                _canExport = true, _canImport = true },

            new FilterInfo("Collada Scene", "dae")
            { _fileTypes = new Type[] {
                typeof(SkeletalMesh),
                typeof(StaticMesh),
                typeof(AnimationContainer) },
                _canExport = true, _canImport = true },

            new FilterInfo("Raw Data", "*"),
        };

        private static FilterInfo[] GetInfo(params Type[] types)
        {
            if (types == null || types.Length == 0)
                return Filters.Values.ToArray();
            return Filters.Where(x => types.Contains(x.Key)).Select(x => x.Value).ToArray();
        }
        public static string CompleteImportFilter { get { return GetAllSupportedFilter() + "|" + GetListFilter(); } }

        public static bool IsSpecial(string ext)
        {
            return GenericInfo.Select(x => x._tag).Contains(ext.ToLower());
        }

        public static string CompleteExportFilter { get { return GetAllSupportedFilter() + "|" + GetListFilter(); } }
        public static string GetCompleteFilter(params Type[] types)
        {
            return GetCompleteFilter(GetInfo(types));
        }
        public static string GetAllSupportedFilter(params Type[] types)
        {
            return GetAllSupportedFilter(GetInfo(types));
        }
        public static string GetListFilter(params Type[] types)
        {
            return GetListFilter(GetInfo(types));
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
        public static string GetAllSupportedFilter(FilterInfo[] files)
        {
            string filter = "All Supported Formats (";
            string filter2 = "|";

            //The "all supported formats" string needs (*.*) in it
            //or else the window will display EVERY SINGLE FILTER
            bool doNotAdd = files.Length > MaxExtensionsInAllFilter;
            if (doNotAdd)
                filter += "*.*";

            string[] fileTypeExtensions = files.Select(x => x.ExtensionsFilter).ToArray();
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
        public static string GetListFilter(FilterInfo[] files)
        {
            string filter = "";
            string[] fileTypeExtensions = files.Select(x => x.Filter).ToArray();
            for (int i = 0; i < fileTypeExtensions.Length; i++)
                filter += fileTypeExtensions[i] + (i == fileTypeExtensions.Length - 1 ? "" : "|");
            return filter;
        }
        //public static FilterInfo[] GetInfo(params string[] extensions)
        //{
        //    FilterInfo[] infoArray = new FilterInfo[extensions.Length];
        //    foreach (FilterInfo fileInfo in Files)
        //        foreach (string ext in fileInfo._extensions)
        //        {
        //            int index = extensions.IndexOf(ext);
        //            if (index >= 0 && !infoArray.Contains(fileInfo))
        //            {
        //                infoArray[index] = fileInfo;
        //                extensions[index] = null;
        //            }
        //        }

        //    //Add remaining extensions not included in the supported formats array
        //    string s;
        //    for (int i = 0; i < extensions.Length; i++)
        //        if (!String.IsNullOrEmpty(s = extensions[i]))
        //            infoArray[i] = new FilterInfo(String.Format("{0} File", s.ToUpper()), s);

        //    return infoArray;
        //}
        //public static string GetCompleteFilter(params string[] extensions)
        //{
        //    return GetCompleteFilter(GetInfo(extensions));
        //}
        //public static string GetListFilter(params string[] extensions)
        //{
        //    return GetListFilter(GetInfo(extensions));
        //}
        //public static string GetListFilter(bool editableOnly)
        //{
        //    if (editableOnly && _filterListEditable != null)
        //        return _filterListEditable;
        //    else if (!editableOnly && _filterList != null)
        //        return _filterList;

        //    if (editableOnly)
        //        return _filterListEditable = GetListFilter(Files, true);
        //    else
        //        return _filterList = GetListFilter(Files, false);
        //}
    }

    public class FilterInfo
    {
        public string _name, _tag;
        public string[] _extensions;
        public ResourceType _resourceType;

        public Type[] _fileTypes;
        public bool _canExport, _canImport;
        
        public string _exportExt;
        public string _importExt;

        public FilterInfo(string name, string tag, ResourceType type, params string[] extensions)
        {
            _name = name;
            _tag = tag;
            _extensions = extensions;
            _resourceType = type;
        }
        public FilterInfo(string name, string tag, params string[] extensions)
        {
            _name = name;
            _tag = tag;
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
                    if (first)
                    {
                        filter += ";";
                        first = false;
                    }
                    if (!ext.Contains('.'))
                        filter += "*.";
                    filter += ext;
                }
                return filter;
            }
        }
    }
}
