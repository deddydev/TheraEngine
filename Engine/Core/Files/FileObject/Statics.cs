using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Files.Serialization;

namespace TheraEngine.Core.Files
{
    public abstract partial class TFileObject : TObject, IFileObject
    {
        static TFileObject()
        {
            _3rdPartyLoaders = new Dictionary<string, Dictionary<Type, Delegate>>();
            _3rdPartyExporters = new Dictionary<string, Dictionary<Type, Delegate>>();
            try
            {
                var types = Engine.FindTypes(t => t.IsSubclassOf(typeof(TFileObject)) && !t.IsAbstract, true, Assembly.GetEntryAssembly()).ToArray();
                foreach (Type type in types)
                {
                    File3rdParty attrib = GetFile3rdPartyExtensions(type);
                    if (attrib == null)
                        continue;

                    foreach (string ext3rd in attrib.ImportableExtensions)
                    {
                        string extLower = ext3rd.ToLowerInvariant();
                        Dictionary<Type, Delegate> extensionLoaders;
                        if (_3rdPartyLoaders.ContainsKey(extLower))
                            extensionLoaders = _3rdPartyLoaders[extLower];
                        else
                            _3rdPartyLoaders.Add(extLower, extensionLoaders = new Dictionary<Type, Delegate>());
                        if (!extensionLoaders.ContainsKey(type))
                        {
                            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                .Where(x => string.Equals(x.GetCustomAttribute<ThirdPartyLoader>()?.Extension, extLower, StringComparison.InvariantCultureIgnoreCase))
                                .ToArray();
                            if (methods.Length > 0)
                            {
                                MethodInfo m = methods[0];
                                bool async = m.GetCustomAttribute<ThirdPartyLoader>().Async;
                                if (async)
                                {
                                    if (Delegate.CreateDelegate(typeof(Del3rdPartyImportFileMethodAsync), m) is Del3rdPartyImportFileMethodAsync result)
                                        extensionLoaders.Add(type, result);
                                }
                                else
                                {
                                    if (Delegate.CreateDelegate(typeof(Del3rdPartyImportFileMethod), m) is Del3rdPartyImportFileMethod result)
                                        extensionLoaders.Add(type, result);
                                }
                            }
                        }
                        else
                            throw new Exception(type.GetFriendlyName() + " has already been added to the third party loader list for " + extLower);
                    }

                    foreach (string ext3rd in attrib.ExportableExtensions)
                    {
                        string extLower = ext3rd.ToLowerInvariant();
                        Dictionary<Type, Delegate> extensionExporters;
                        if (_3rdPartyExporters.ContainsKey(extLower))
                            extensionExporters = _3rdPartyExporters[extLower];
                        else
                            _3rdPartyExporters.Add(extLower, extensionExporters = new Dictionary<Type, Delegate>());
                        if (!extensionExporters.ContainsKey(type))
                        {
                            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                .Where(x => string.Equals(x.GetCustomAttribute<ThirdPartyExporter>()?.Extension, extLower, StringComparison.InvariantCultureIgnoreCase))
                                .ToArray();
                            if (methods.Length > 0)
                            {
                                MethodInfo m = methods[0];
                                bool async = m.GetCustomAttribute<ThirdPartyExporter>().Async;
                                if (async)
                                {
                                    if (Delegate.CreateDelegate(typeof(Del3rdPartyExportFileMethodAsync), m) is Del3rdPartyExportFileMethodAsync result)
                                        extensionExporters.Add(type, result);
                                }
                                else
                                {
                                    if (Delegate.CreateDelegate(typeof(Del3rdPartyExportFileMethod), m) is Del3rdPartyExportFileMethod result)
                                        extensionExporters.Add(type, result);
                                }
                            }
                        }
                        else
                            throw new Exception(type.GetFriendlyName() + " has already been added to the third party exporter list for " + extLower);
                    }
                }
            }
            catch { }
        }
        public static FileDef GetFileDefinition(Type classType)
            => Attribute.GetCustomAttribute(classType, typeof(FileDef), true) as FileDef;
        public static FileExt GetFileExtension(Type classType)
            => Attribute.GetCustomAttribute(classType, typeof(FileExt), true) as FileExt;
        public static File3rdParty GetFile3rdPartyExtensions(Type classType)
            => Attribute.GetCustomAttribute(classType, typeof(File3rdParty), true) as File3rdParty;
        public static Type DetermineType(string path)
            => SerializationCommon.DetermineType(path);
        public static Type[] DetermineThirdPartyTypes(string ext)
            => Engine.FindTypes(t => typeof(TFileObject).IsAssignableFrom(t) && (t.GetCustomAttribute<File3rdParty>()?.HasExtension(ext) ?? false), true, Assembly.GetEntryAssembly()).ToArray();
        public static EFileFormat GetFormat(string path, out string ext)
        {
            int index = path.LastIndexOf('.') + 1;
            if (index != 0)
                ext = path.Substring(index).ToLowerInvariant();
            else
                ext = path.ToLowerInvariant();

            if (File3rdParty.Has3rdPartyExtension(ext))
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
        public static string GetFilePath(string dir, string name, EProprietaryFileFormat format, Type fileType)
            => Path.Combine(dir, name + "." + GetFileExtension(fileType).GetProperExtension(format));
        public static string GetFilePath<T>(string dir, string name, EProprietaryFileFormat format) where T : TFileObject
            => Path.Combine(dir, name + "." + GetFileExtension(typeof(T)).GetProperExtension(format));
        public static string GetFilePath(string dir, string name, string thirdPartyExtension)
        {
            if (thirdPartyExtension[0] != '.')
                thirdPartyExtension = "." + thirdPartyExtension;
            return Path.Combine(dir, name + thirdPartyExtension);
        }
        public static string GetFilter<T>(bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false) where T : TFileObject
            => GetFilter(typeof(T), proprietary, import3rdParty, export3rdParty);
        /// <summary>
        /// Returns the filter for all extensions related to this format.
        /// </summary>
        /// <param name="classType">The type of the class to get the filter for.</param>
        /// <param name="proprietary">Add the TheraEngine proprietary formats (binary, xml, etc)</param>
        /// <param name="import3rdParty">Add any importable 3rd party file formats</param>
        /// <param name="export3rdParty">Add any exportable 3rd party file formats</param>
        /// <returns>The filter to be used in open file dialog, save file dialog, etc</returns>
        public static string GetFilter(Type classType, bool proprietary = true, bool import3rdParty = false, bool export3rdParty = false)
        {
            string allTypes = "";
            string eachType = "";
            bool first = true;
            FileDef def = GetFileDefinition(classType);
            if (proprietary)
            {
                FileExt ext = GetFileExtension(classType);
                foreach (string type in Enum.GetNames(typeof(EProprietaryFileFormat)))
                {
                    if (first)
                        first = false;
                    else
                    {
                        allTypes += ";";
                        eachType += "|";
                    }
                    string fmt = String.Format("*.{0}{1}", type.Substring(0, 1).ToLowerInvariant(), ext.Extension);
                    eachType += String.Format("{0} [{2}] ({1})|{1}", def.UserFriendlyName, fmt, type);
                    allTypes += fmt;
                }
            }
            if (import3rdParty || export3rdParty)
            {
                File3rdParty ext = GetFile3rdPartyExtensions(classType);
                if (import3rdParty)
                {
                    foreach (string ext3rd in ext.ImportableExtensions)
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
                        if (File3rdParty.ExtensionNames3rdParty.ContainsKey(extLower))
                            eachType += String.Format("{0} ({1})|{1}", File3rdParty.ExtensionNames3rdParty[extLower], fmt);
                        else
                            eachType += String.Format("{0} file ({1})|{1}", extLower, fmt);
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
                            first = false;
                        else
                        {
                            allTypes += ";";
                            eachType += "|";
                        }
                        string fmt = String.Format("*.{0}", extLower);
                        if (File3rdParty.ExtensionNames3rdParty.ContainsKey(extLower))
                            eachType += String.Format("{0} ({1})|{1}", File3rdParty.ExtensionNames3rdParty[extLower], fmt);
                        else
                            eachType += String.Format("{0} file ({1})|{1}", extLower, fmt);
                        allTypes += fmt;
                    }
                }
            }

            string allTypesFull = String.Format("{0} ({1})|{1}", def.UserFriendlyName, allTypes);
            return allTypesFull + "|" + eachType;
        }

        #region Import/Export
        public static async Task<T> LoadAsync<T>(string filePath) where T : TFileObject
            => await LoadAsync(typeof(T), filePath, null, CancellationToken.None) as T;
        /// <summary>
        /// Opens a new instance of the given file at the given file path.
        /// </summary>
        /// <typeparam name="T">The type of the file object to load.</typeparam>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A new instance of the file.</returns>
        public static async Task<T> LoadAsync<T>(
            string filePath, IProgress<float> progress, CancellationToken cancel) where T : TFileObject
            => await LoadAsync(typeof(T), filePath, progress, cancel) as T;
        /// <summary>
        /// Opens a new instance of the file object at the given file path.
        /// </summary>
        /// <param name="type">The type of the file object to load.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A new instance of the file.</returns>
        public static async Task<TFileObject> LoadAsync(
            Type type, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            FileExt extAttrib = GetFileExtension(type);
            File3rdParty tpAttrib = GetFile3rdPartyExtensions(type);
            EFileFormat fmt = GetFormat(filePath, out string ext);
            if (extAttrib != null && fmt != EFileFormat.ThirdParty)
            {
                EProprietaryFileFormat pfmt = fmt == EFileFormat.Binary ?
                    EProprietaryFileFormat.Binary :
                    EProprietaryFileFormat.XML;

                string fileExt = extAttrib.GetProperExtension(pfmt);
                if (string.Equals(ext, fileExt))
                    return fmt == EFileFormat.XML ?
                        await FromXMLAsync(type, filePath, progress, cancel) :
                        await FromBinaryAsync(type, filePath, progress, cancel);
            }
            else if (tpAttrib != null)
            {
                bool hasWildcard = tpAttrib.ImportableExtensions.Contains("*");
                bool hasExt = tpAttrib.ImportableExtensions.Contains(ext.ToLowerInvariant());
                if (hasWildcard || hasExt)
                    return await Read3rdPartyAsync(type, filePath, progress, cancel);
            }

            Engine.LogWarning("{0} cannot be loaded as {1}.", filePath, type.GetFriendlyName());
            return default;
        }
        #endregion

        #region XML
        internal static async Task<T> FromXMLAsync<T>(
            string filePath, IProgress<float> progress, CancellationToken cancel)
            where T : TFileObject
            => await FromXMLAsync(typeof(T), filePath, progress, cancel) as T;
        internal static async Task<TFileObject> FromXMLAsync(
            Type type, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(filePath))
                    return null;

                TFileObject file = null;
                try
                {
                    Type fileType = CustomXmlSerializer.DetermineType(filePath);
                    FileExt ext = GetFileExtension(fileType);
                    if (ext?.ManualXmlConfigSerialize ?? false)
                    {
                        unsafe
                        {
                            using (FileMap map = FileMap.FromFile(filePath))
                            {
                                XMLReader reader = new XMLReader(map.Address, map.Length, true);
                                file = SerializationCommon.CreateObject(fileType) as TFileObject;
                                if (file != null && reader.BeginElement())
                                {
                                    file.FilePath = filePath;
                                    //if (reader.Name.Equals(t.ToString(), true))
                                    file.ReadAsync(reader);
                                    //else
                                    //    throw new Exception("File was not of expected type.");
                                    reader.EndElement();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (type.IsAssignableFrom(fileType))
                            file = new CustomXmlSerializer().Deserialize(filePath);
                        else
                            Engine.LogWarning($"{fileType.GetFriendlyName()} is not assignable to {type.GetFriendlyName()}.");
                    }
                }
                catch (Exception ex)
                {
                    Engine.LogWarning($"Unable to load XML file at {filePath}.\n{ex.ToString()}");
                }
                if (file != null)
                    file.FilePath = filePath;
                return file;
            });
        }
        private static readonly XmlWriterSettings DefaultWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = Environment.NewLine,
            NewLineHandling = NewLineHandling.Replace
        };
        #endregion

        #region Binary

        internal static async Task<T> FromBinaryAsync<T>(string filePath, IProgress<float> progress, CancellationToken cancel) where T : TFileObject
            => await FromBinaryAsync(typeof(T), filePath, progress, cancel) as T;
        internal static async Task<TFileObject> FromBinaryAsync(
            Type type, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            Type fileType = TDeserializer.DetermineType(filePath);
            if (type.IsAssignableFrom(fileType))
            {
                TDeserializer deserializer = new TDeserializer();
                TFileObject file = await deserializer.Deserialize(filePath, type) as TFileObject;
                return file;
            }
            else
            {
                Engine.LogWarning($"{fileType.GetFriendlyName()} is not assignable to {type.GetFriendlyName()}.");
                return null;
            }
            
        }
        #endregion

        #region 3rd Party
        public static async Task<T> Read3rdPartyAsync<T>(string filePath) where T : TFileObject
            => await Read3rdPartyAsync(typeof(T), filePath, null, CancellationToken.None) as T;
        public static async Task<T> Read3rdPartyAsync<T>(string filePath, IProgress<float> progress, CancellationToken cancel) where T : TFileObject
            => await Read3rdPartyAsync(typeof(T), filePath, progress, cancel) as T;
        public static async Task<TFileObject> Read3rdPartyAsync(Type classType, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            string ext = Path.GetExtension(filePath).Substring(1);
            Delegate loader = Get3rdPartyLoader(classType, ext);
            if (loader != null)
            {
                if (loader is Del3rdPartyImportFileMethod method)
                    return method.Invoke(filePath);
                else if (loader is Del3rdPartyImportFileMethodAsync methodAsync)
                    return await methodAsync.Invoke(filePath, progress, cancel);
            }

            //No third party loader defined, create instance directly and call method to do it
            TFileObject obj = SerializationCommon.CreateObject(classType) as TFileObject;
            if (obj != null)
            {
                File3rdParty f = GetFile3rdPartyExtensions(classType);
                if (f != null)
                {
                    if (f.AsyncManualRead)
                        await obj.ManualRead3rdPartyAsync(filePath);
                    else
                        obj.ManualRead3rdParty(filePath);
                }
            }
            return obj;
        }
        private static Dictionary<string, Dictionary<Type, Delegate>> _3rdPartyLoaders;
        private static Dictionary<string, Dictionary<Type, Delegate>> _3rdPartyExporters;
        public static Delegate Get3rdPartyLoader(Type fileType, string extension)
        {
            extension = extension.ToLowerInvariant();
            if (_3rdPartyLoaders != null && _3rdPartyLoaders.ContainsKey(extension))
            {
                var t = _3rdPartyLoaders[extension];
                if (t.ContainsKey(fileType))
                    return t[fileType];
            }
            return null;
        }
        public static Delegate Get3rdPartyExporter(Type fileType, string extension)
        {
            extension = extension.ToLowerInvariant();
            if (_3rdPartyExporters != null && _3rdPartyExporters.ContainsKey(extension))
            {
                var t = _3rdPartyExporters[extension];
                if (t.ContainsKey(fileType))
                    return t[fileType];
            }
            return null;
        }
        public static void Register3rdPartyLoader<T>(string extension, Del3rdPartyImportFileMethod loadMethod) where T : TFileObject
        {
            Register3rdParty<T>(_3rdPartyLoaders, extension, loadMethod);
        }
        public static void Register3rdPartyExporter<T>(string extension, Del3rdPartyImportFileMethod exportMethod) where T : TFileObject
        {
            Register3rdParty<T>(_3rdPartyExporters, extension, exportMethod);
        }
        private static void Register3rdParty<T>(
            Dictionary<string, Dictionary<Type, Delegate>> methodDic, 
            string extension, 
            Del3rdPartyImportFileMethod method)
            where T : TFileObject
        {
            extension = extension.ToLowerInvariant();

            if (methodDic == null)
                methodDic = new Dictionary<string, Dictionary<Type, Delegate>>();

            Dictionary<Type, Delegate> typesforExt;
            if (!methodDic.ContainsKey(extension))
                methodDic.Add(extension, typesforExt = new Dictionary<Type, Delegate>());
            else
                typesforExt = methodDic[extension];
            
            Type fileType = typeof(T);
            if (!typesforExt.ContainsKey(fileType))
                typesforExt.Add(fileType, method);
            else
                throw new Exception("Registered " + extension + " for " + fileType.GetFriendlyName() + " too many times.");
        }
        #endregion
    }
}
