using Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files
{
    public abstract partial class TFileObject : TObject, IFileObject
    {
        public static TFileDef GetFileDefinition(Type classType)
            => classType.GetCustomAttribute<TFileDef>(true);
        public static TFileExt GetFileExtension(Type classType)
            => classType.GetCustomAttribute<TFileExt>(true);
        public static TFile3rdPartyExt GetFile3rdPartyExtensions(Type classType)
            => classType.GetCustomAttribute<TFile3rdPartyExt>(true);

        public static TFileDef GetFileDefinition(TypeProxy classType)
            => classType.GetCustomAttribute<TFileDef>(true);
        public static TFileExt GetFileExtension(TypeProxy classType)
            => classType.GetCustomAttribute<TFileExt>(true);
        public static TFile3rdPartyExt GetFile3rdPartyExtensions(TypeProxy classType)
            => classType.GetCustomAttribute<TFile3rdPartyExt>(true);

        //public static TFileDef GetFileDefinition(Type classType)
        //    => Attribute.GetCustomAttribute(classType, typeof(TFileDef), true) as TFileDef;
        //public static TFileExt GetFileExtension(Type classType)
        //    => Attribute.GetCustomAttribute(classType, typeof(TFileExt), true) as TFileExt;
        //public static TFile3rdPartyExt GetFile3rdPartyExtensions(Type classType)
        //    => Attribute.GetCustomAttribute(classType, typeof(TFile3rdPartyExt), true) as TFile3rdPartyExt;

        public static TFileDef GetFileDefinition<T>()
            => GetFileDefinition(typeof(T));
        public static TFileExt GetFileExtension<T>()
            => GetFileExtension(typeof(T));
        public static TFile3rdPartyExt GetFile3rdPartyExtensions<T>() 
            => GetFile3rdPartyExtensions(typeof(T));

        public static string ExtensionFor<T>(EProprietaryFileFormat format)
            => GetFileExtension<T>()?.GetFullExtension(format);
        public static string[] ExtensionsForImporting<T>()
            => GetFileExtension<T>()?.ImportableExtensions;
        public static string[] ExtensionsForExporting<T>()
            => GetFileExtension<T>()?.ExportableExtensions;
        public static string[] ExtensionsForThirdParty<T>()
            => GetFile3rdPartyExtensions<T>()?.Extensions;
        
        public static TypeProxy DetermineType(string path)
            => DetermineType(path, out _);
        public static TypeProxy DetermineType(string path, out EFileFormat format)
            => SerializationCommon.DetermineType(path, out format);

        private static Type[] _thirdPartyCache;
        public static void ClearThirdPartyTypeCache(bool reloadNow)
        {
            if (reloadNow)
            {
                Type fileType = typeof(TFileObject);
                _thirdPartyCache = AppDomainHelper.FindTypes(t => 
                    t.IsSubclassOf(fileType) && 
                    t.HasCustomAttribute<TFile3rdPartyExt>()
                ).Select(x => (Type)x).ToArray();
            }
            else
                _thirdPartyCache = null;
        }
        public static Type[] DetermineThirdPartyTypes(string ext, bool allowWildcard)
        {
            if (_thirdPartyCache is null)
                ClearThirdPartyTypeCache(true);
            return _thirdPartyCache.Where(t => t.GetCustomAttribute<TFile3rdPartyExt>()?.HasExtension(ext, allowWildcard) ?? false).ToArray();
        }
        public static EFileFormat GetFormat(string path, out string ext)
        {
            int index = path.LastIndexOf('.') + 1;
            ext = index != 0 ? path.Substring(index).ToLowerInvariant() : path.ToLowerInvariant();

            if (TFile3rdPartyExt.Is3rdPartyExtension(ext))
                return EFileFormat.ThirdParty;

            //TODO: return raw format
            if (ext.Length == 0)
                return EFileFormat.ThirdParty;

            switch (ext[0])
            {
                default: return EFileFormat.ThirdParty;
                case 'b': return EFileFormat.Binary;
                case 'x': return EFileFormat.XML;
            }
        }

        /// <summary>
        /// Extracts the directory, file name, and file format out of a file path.
        /// </summary>
        /// <param name="path">The path to the file. Does not need to be absolute.</param>
        /// <param name="dir">The path to the folder that the file resides in.</param>
        /// <param name="name">The name of the file in the path.</param>
        /// <param name="fmt">The format the data is written in.</param>
        /// <param name="ext">The extension of the file.</param>
        public static void GetDirNameFmt(string path, out string dir, out string name, out EFileFormat fmt, out string ext)
        {
            dir = Path.GetDirectoryName(path);
            name = Path.GetFileNameWithoutExtension(path);
            fmt = GetFormat(path, out ext);
        }
        /// <summary>
        /// Creates the full file path to a file given separate parameters.
        /// </summary>
        /// <param name="dir">The path to the folder the file resides in.</param>
        /// <param name="name">The name of the file.</param>
        /// <param name="format">The format the data is written in.</param>
        /// <param name="fileType">The type of file object.</param>
        /// <returns>An absolute path to the file.</returns>
        public static string CreateFilePath(string dir, string name, EProprietaryFileFormat format, TypeProxy fileType)
            => Path.Combine(dir, CreateFileName(name, format, fileType));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="format"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string CreateFileName(string name, EProprietaryFileFormat format, TypeProxy fileType)
            => name + "." + CreateFileExtension(format, fileType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string CreateFileExtension(EProprietaryFileFormat format, TypeProxy fileType)
            => GetFileExtension(fileType)?.GetFullExtension(format) ?? throw new InvalidOperationException();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dir"></param>
        /// <param name="name"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string CreateFilePath<T>(string dir, string name, EProprietaryFileFormat format) where T : class, IFileObject
            => Path.Combine(dir, CreateFileName<T>(name, format));
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string CreateFileName<T>(string name, EProprietaryFileFormat format) where T : class, IFileObject
            => name + "." + CreateFileExtension<T>(format);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string CreateFileExtension<T>(EProprietaryFileFormat format) where T : class, IFileObject
            => GetFileExtension<T>().GetFullExtension(format);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="name"></param>
        /// <param name="thirdPartyExtension"></param>
        /// <returns></returns>
        public static string CreateFilePath(string dir, string name, string thirdPartyExtension)
        {
            if (thirdPartyExtension[0] != '.')
                thirdPartyExtension = "." + thirdPartyExtension;
            return Path.Combine(dir, name + thirdPartyExtension);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proprietary"></param>
        /// <param name="thirdParty"></param>
        /// <param name="import3rdParty"></param>
        /// <param name="export3rdParty"></param>
        /// <returns></returns>
        public static string CreateFilter<T>(
            bool proprietary = true,
            bool thirdParty = true,
            bool import3rdParty = false,
            bool export3rdParty = false) where T : class, IFileObject
            => CreateFilter(typeof(T), proprietary, thirdParty, import3rdParty, export3rdParty);
        /// <summary>
        /// Returns the filter for all extensions related to this format.
        /// </summary>
        /// <param name="classType">The type of the class to get the filter for.</param>
        /// <param name="proprietary">Add the TheraEngine proprietary formats (binary, xml, etc)</param>
        /// <param name="import3rdParty">Add any importable 3rd party file formats</param>
        /// <param name="export3rdParty">Add any exportable 3rd party file formats</param>
        /// <returns>The filter to be used in open file dialog, save file dialog, etc</returns>
        public static string CreateFilter(
            Type classType,
            bool proprietary = true,
            bool thirdParty = true,
            bool import3rdParty = false,
            bool export3rdParty = false)
        {
            if (classType is null)
                return "File (*.*)|*.*";
            
            string allTypes = "";
            string eachType = "";
            bool first = true;
            TFileDef def = GetFileDefinition(classType);
            TFileExt ext = GetFileExtension(classType);
            string name = def?.UserFriendlyName ?? classType.GetFriendlyName();
            if (proprietary && ext != null)
            {
                foreach (string type in Enum.GetNames(typeof(EProprietaryFileFormat)))
                {
                    if (first)
                        first = false;
                    else
                    {
                        allTypes += ";";
                        eachType += "|";
                    }
                    string fmt = $"*.{type.Substring(0, 1).ToLowerInvariant()}{ext.Extension}";
                    eachType += $"{name} [{type}] ({fmt})|{fmt}";
                    allTypes += fmt;
                }
            }
            if (thirdParty)
            {
                TFile3rdPartyExt tpExt = GetFile3rdPartyExtensions(classType);
                if (tpExt != null)
                    foreach (string extStr in tpExt.Extensions)
                    {
                        if (first)
                        {
                            first = false;
                            if (eachType.Length > 0)
                                eachType += "|";
                            if (allTypes.Length > 0)
                                allTypes += ";";
                        }
                        else
                        {
                            allTypes += ";";
                            eachType += "|";
                        }

                        string fmt = $"*.{extStr}";
                        eachType += $"{name} ({fmt})|{fmt}";
                        allTypes += fmt;
                    }
            }
            if (ext != null)
            {
                if (import3rdParty)
                {
                    foreach (string ext3rd in ext.ImportableExtensions)
                    {
                        string extLower = ext3rd.ToLowerInvariant();
                        if (first)
                        {
                            first = false;
                            if (eachType.Length > 0)
                                eachType += "|";
                            if (allTypes.Length > 0)
                                allTypes += ";";
                        }
                        else
                        {
                            allTypes += ";";
                            eachType += "|";
                        }
                        string fmt = $"*.{extLower}";
                        if (TFile3rdPartyExt.ExtensionNames3rdParty.ContainsKey(extLower))
                            eachType += $"{TFile3rdPartyExt.ExtensionNames3rdParty[extLower]} ({fmt})|{fmt}";
                        else
                            eachType += $"{extLower} file ({fmt})|{fmt}";
                        allTypes += fmt;
                    }
                }
                if (export3rdParty)
                {
                    foreach (string ext3rd in ext.ExportableExtensions)
                    {
                        //Don't rewrite extension if it was already written as importable
                        if (import3rdParty &&
                            ext.ImportableExtensions != null &&
                            ext.ImportableExtensions.Contains(ext3rd, StringComparer.InvariantCultureIgnoreCase))
                            continue;

                        string extLower = ext3rd.ToLowerInvariant();

                        if (first)
                        {
                            first = false;
                            if (eachType.Length > 0)
                                eachType += "|";
                            if (allTypes.Length > 0)
                                allTypes += ";";
                        }
                        else
                        {
                            allTypes += ";";
                            eachType += "|";
                        }
                        string fmt = $"*.{extLower}";
                        if (TFile3rdPartyExt.ExtensionNames3rdParty.ContainsKey(extLower))
                            eachType += $"{TFile3rdPartyExt.ExtensionNames3rdParty[extLower]} ({fmt})|{fmt}";
                        else
                            eachType += $"{extLower} file ({fmt})|{fmt}";
                        allTypes += fmt;
                    }
                }
            }
            
            string allTypesFull = $"{name} ({allTypes})|{allTypes}";
            return allTypesFull + "|" + eachType;
        }

        #region Import/Export

        public static async Task<T> LoadAsync<T>(string filePath)
        {
            T file;
            if (Engine.BeginOperation != null)
            {
                int op = Engine.BeginOperation(
                    $"Loading file from {filePath}...",
                    $"{filePath} loaded successfully.", 
                    out Progress<float> progress,
                    out CancellationTokenSource cancel);

                file = (T)await LoadAsync(typeof(T), filePath, progress, cancel.Token);

                if (Engine.EndOperation != null)
                    Engine.EndOperation(op);
                else
                    ((IProgress<float>)progress).Report(1.0f);

                cancel.Dispose();
            }
            else
                file = (T)(await LoadAsync(typeof(T), filePath, null, CancellationToken.None));
            
            return file;
        }
        /// <summary>
        /// Opens a new instance of the given file at the given file path.
        /// </summary>
        /// <typeparam name="T">The type of the file object to load.</typeparam>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A new instance of the file.</returns>
        public static async Task<T> LoadAsync<T>(
            string filePath, IProgress<float> progress, CancellationToken cancel)
            => (T)(await LoadAsync(typeof(T), filePath, progress, cancel));
        /// <summary>
        /// Opens a new instance of the file object at the given file path.
        /// </summary>
        /// <param name="fileType">The type of the file object to load.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A new instance of the file.</returns>
        public static async Task<object> LoadAsync(
            Type fileType, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            object result = await TryLoadAsync(fileType, filePath, progress, cancel);
            if (result != null)
                return result;

            Engine.LogWarning($"{filePath} cannot be loaded as {fileType.GetFriendlyName()}.");
            return null;
        }

        public static async Task<object> TryLoadAsync(
            TypeProxy fileType,
            string filePath,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            object file = null;
            try
            {
                TFileExt extAttrib = GetFileExtension(fileType);
                TFile3rdPartyExt tpAttrib = GetFile3rdPartyExtensions(fileType);
                EFileFormat fmt = GetFormat(filePath, out string ext);
                if (extAttrib != null)
                {
                    if (fmt == EFileFormat.ThirdParty)
                    {
                        bool hasWildcard = extAttrib.HasImportableExtension("*");
                        bool hasExt = extAttrib.HasImportableExtension(ext);
                        if (hasWildcard || hasExt)
                            file = await Read3rdPartyAsync(fileType, filePath, progress, cancel);
                    }
                    else
                    {
                        EProprietaryFileFormat pfmt = (EProprietaryFileFormat)(int)fmt;

                        //string fileExt = extAttrib.GetFullExtension(pfmt);
                        //if (string.Equals(ext, fileExt))
                        //{

                        file = await DeserializeProprietaryFormat(filePath, fileType, pfmt, progress, cancel);

                        //if (fileType != null && file != null)
                        //{
                        //    TypeProxy fileType = file?.GetTypeProxy();
                        //    if (!fileType.IsAssignableFrom(fileType))
                        //    {
                        //        Engine.LogWarning($"{fileType.GetFriendlyName()} is not assignable to {fileType.GetFriendlyName()}.");
                        //        file = null;
                        //    }
                        //}

                        //}
                    }
                }
                else if (tpAttrib != null)
                {
                    bool hasWildcard = tpAttrib.HasExtension("*", true);
                    bool hasExt = tpAttrib.HasExtension(ext, false);
                    if (hasWildcard || hasExt)
                        file = await Read3rdPartyAsync(fileType, filePath, progress, cancel);
                }

                //if (file is null)
                //{
                //    var subClassTypes = AppDomainHelper.FindTypes(x => x.IsSubclassOf(fileType));
                //    foreach (var subClassType in subClassTypes)
                //    {
                //        object result = await TryLoadAsync(subClassType, filePath, progress, cancel);
                //        if (result != null)
                //        {
                //            file = result;
                //            break;
                //        }
                //    }
                //}

                if (file is IFileObject fileObj)
                {
                    fileObj.FilePath = filePath;
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Engine.Out(e.ToString());
            }
            return file;
        }

        #endregion

        #region Deserialize Proprietary Format

        //private static async Task<T> DeserializeProperietaryFormat<T>(
        //    string filePath,
        //    TypeProxy fileType,
        //    EProprietaryFileFormat format,
        //    IProgress<float> progress, 
        //    CancellationToken cancel)
        //    where T : class, IFileObject
        //    => await DeserializeProperietaryFormat(filePath, fileType, format, progress, cancel) as T;

        private static async Task<IFileObject> DeserializeProprietaryFormat(
            string filePath,
            TypeProxy fileType,
            EProprietaryFileFormat format,
            IProgress<float> progress, 
            CancellationToken cancel)
        {
            try
            {
                Deserializer deser = new Deserializer();
                IFileObject file = await deser.DeserializeAsync(filePath, fileType, format, progress, cancel) as IFileObject;
                return file;
            }
            catch (Exception ex)
            {
                Engine.LogWarning($"Unable to deserialize file at {filePath}.\n{ex.ToString()}");
            }
            return null;
        }

        #endregion

        #region 3rd Party
        public static async Task<T> Read3rdPartyAsync<T>(string filePath)
            => (T)(await Read3rdPartyAsync(typeof(T), filePath, null, CancellationToken.None));
        public static async Task<object> Read3rdPartyAsync(TypeProxy classType, string filePath)
            => await Read3rdPartyAsync(classType, filePath, null, CancellationToken.None);
        public static async Task<T> Read3rdPartyAsync<T>(string filePath, IProgress<float> progress, CancellationToken cancel)
            => (T)await Read3rdPartyAsync(typeof(T), filePath, progress, cancel);
        public static async Task<object> Read3rdPartyAsync(TypeProxy classType, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            string ext = Path.GetExtension(filePath);
            if (ext != null && ext.StartsWith("."))
                ext = ext.Substring(1);

            Delegate loader = Engine.DomainProxy.Get3rdPartyLoader(classType, ext);
            if (loader != null)
            {
                Type genericDef = loader.GetType().GetGenericTypeDefinition();
                if (genericDef == typeof(Del3rdPartyImportFileMethod<>))
                    return loader.DynamicInvoke(filePath);
                else if (genericDef == typeof(Del3rdPartyImportFileMethodAsync<>))
                {
                    var task = (Task)loader.DynamicInvoke(filePath, progress, cancel);
                    await task.ConfigureAwait(false);
                    var resultProperty = task.GetType().GetProperty("Result");
                    return resultProperty.GetValue(task);
                }
            }

            //No third party loader defined, create instance directly and call method to do it
            IFileObject obj = classType.CreateInstance() as IFileObject;
            if (obj != null)
            {
                TFile3rdPartyExt attrib = GetFile3rdPartyExtensions(classType);
                if (attrib != null)
                {
                    if (attrib.AsyncManualRead)
                        await obj.ManualRead3rdPartyAsync(filePath);
                    else
                        obj.ManualRead3rdParty(filePath);
                }
            }
            return obj;
        }

        #endregion
    }
}
