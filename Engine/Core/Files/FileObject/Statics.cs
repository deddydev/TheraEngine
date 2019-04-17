﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files
{
    public abstract partial class TFileObject : TObject, IFileObject
    {
        private static void ReadLoaders(IDictionary<string, Dictionary<TypeProxy, Delegate>> loaders, TypeProxy type, IEnumerable<string> extensions)
        {
            foreach (string ext3rd in extensions)
            {
                string extLower = ext3rd.ToLowerInvariant();
                Dictionary<TypeProxy, Delegate> extensionLoaders;
                if (loaders.ContainsKey(extLower))
                    extensionLoaders = loaders[extLower];
                else
                    loaders.Add(extLower, extensionLoaders = new Dictionary<TypeProxy, Delegate>());

                if (extensionLoaders.ContainsKey(type))
                    throw new Exception(type.GetFriendlyName() + " has already been added to the third party loader list for " + extLower);

                MethodInfoProxy[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                    .Where(x => string.Equals(x.GetCustomAttribute<ThirdPartyLoader>()?.Extension, extLower, StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();

                if (methods.Length <= 0)
                    continue;

                MethodInfoProxy m = methods[0];
                ThirdPartyLoader loader = m.GetCustomAttribute<ThirdPartyLoader>();
                bool async = loader.Async;
                TypeProxy delGenType = async ? typeof(Del3rdPartyImportFileMethodAsync<>) : typeof(Del3rdPartyImportFileMethod<>);

                try
                {
                    TypeProxy delType = delGenType.MakeGenericType(m.DeclaringType);
                    Delegate d = delType.CreateDelegate(m);
                    extensionLoaders.Add(type, d);
                }
                catch
                {
                    Engine.LogWarning($"Cannot use {m.GetFriendlyName()} as a third party loader for {m.DeclaringType.GetFriendlyName()}.");
                }
            }
        }

        public static void Reset3rdPartyImportExportMethods(bool reloadNow = true)
        {
            if (reloadNow)
            {
                _3rdPartyLoaders = new Dictionary<string, Dictionary<TypeProxy, Delegate>>();
                _3rdPartyExporters = new Dictionary<string, Dictionary<TypeProxy, Delegate>>();
                try
                {
                    TypeProxy[] types = PrimaryAppDomainManager.FindTypes(t => t.IsSubclassOf(typeof(TFileObject)) && !t.IsAbstract).ToArray();
                    foreach (TypeProxy type in types)
                    {
                        TFileExt attrib = GetFileExtension(type);
                        if (attrib == null)
                            continue;

                        ReadLoaders(_3rdPartyLoaders, type, attrib.ImportableExtensions);
                        ReadLoaders(_3rdPartyExporters, type, attrib.ExportableExtensions);
                    }
                }
                catch { }
            }
            else
            {
                _3rdPartyLoaders = null;
                _3rdPartyExporters = null;
            }
        }

        public static TFileDef GetFileDefinition(TypeProxy classType)
            => classType.GetCustomAttribute<TFileDef>(true);
        public static TFileExt GetFileExtension(TypeProxy classType)
            => classType.GetCustomAttribute<TFileExt>(true);
        public static TFile3rdPartyExt GetFile3rdPartyExtensions(TypeProxy classType)
            => classType.GetCustomAttribute<TFile3rdPartyExt>(true);

        public static TFileDef GetFileDefinition(Type classType)
            => Attribute.GetCustomAttribute(classType, typeof(TFileDef), true) as TFileDef;
        public static TFileExt GetFileExtension(Type classType)
            => Attribute.GetCustomAttribute(classType, typeof(TFileExt), true) as TFileExt;
        public static TFile3rdPartyExt GetFile3rdPartyExtensions(Type classType)
            => Attribute.GetCustomAttribute(classType, typeof(TFile3rdPartyExt), true) as TFile3rdPartyExt;

        public static TFileDef GetFileDefinition<T>()
            => GetFileDefinition(TypeProxy.TypeOf<T>());
        public static TFileExt GetFileExtension<T>()
            => GetFileExtension(TypeProxy.TypeOf<T>());
        public static TFile3rdPartyExt GetFile3rdPartyExtensions<T>() 
            => GetFile3rdPartyExtensions(typeof(T));

        public static TypeProxy DetermineType(string path)
            => DetermineType(path, out _);
        public static TypeProxy DetermineType(string path, out EFileFormat format)
            => SerializationCommon.DetermineType(path, out format);

        private static TypeProxy[] _thirdPartyCache;
        public static void ClearThirdPartyTypeCache(bool reloadNow)
        {
            if (reloadNow)
            {
                Type fileType = typeof(TFileObject);
                _thirdPartyCache = PrimaryAppDomainManager.FindTypes(t => t.IsSubclassOf(fileType) && t.HasCustomAttribute<TFile3rdPartyExt>()).ToArray();
            }
            else
                _thirdPartyCache = null;
        }
        public static TypeProxy[] DetermineThirdPartyTypes(string ext)
        {
            if (_thirdPartyCache == null)
                ClearThirdPartyTypeCache(true);
            return _thirdPartyCache.Where(t => t.GetCustomAttribute<TFile3rdPartyExt>()?.HasExtension(ext) ?? false).ToArray();
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
        public static string GetFilePath(string dir, string name, EProprietaryFileFormat format, Type fileType)
            => Path.Combine(dir, GetFileName(name, format, fileType));
        public static string GetFileName(string name, EProprietaryFileFormat format, Type fileType)
            => name + "." + GetFileExtension(format, fileType);
        public static string GetFileExtension(EProprietaryFileFormat format, Type fileType)
            => GetFileExtension(fileType)?.GetFullExtension(format) ?? throw new InvalidOperationException();

        public static string GetFilePath<T>(string dir, string name, EProprietaryFileFormat format) where T : class, IFileObject
            => Path.Combine(dir, GetFileName<T>(name, format));
        public static string GetFileName<T>(string name, EProprietaryFileFormat format) where T : class, IFileObject
            => name + "." + GetFileExtension<T>(format);
        public static string GetFileExtension<T>(EProprietaryFileFormat format) where T : class, IFileObject
            => GetFileExtension<T>().GetFullExtension(format);

        public static string GetFilePath(string dir, string name, string thirdPartyExtension)
        {
            if (thirdPartyExtension[0] != '.')
                thirdPartyExtension = "." + thirdPartyExtension;
            return Path.Combine(dir, name + thirdPartyExtension);
        }
        public static string GetFilter<T>(
            bool proprietary = true,
            bool thirdParty = true,
            bool import3rdParty = false,
            bool export3rdParty = false) where T : class, IFileObject
            => GetFilter(typeof(T), proprietary, thirdParty, import3rdParty, export3rdParty);
        /// <summary>
        /// Returns the filter for all extensions related to this format.
        /// </summary>
        /// <param name="classType">The type of the class to get the filter for.</param>
        /// <param name="proprietary">Add the TheraEngine proprietary formats (binary, xml, etc)</param>
        /// <param name="import3rdParty">Add any importable 3rd party file formats</param>
        /// <param name="export3rdParty">Add any exportable 3rd party file formats</param>
        /// <returns>The filter to be used in open file dialog, save file dialog, etc</returns>
        public static string GetFilter(
            TypeProxy classType,
            bool proprietary = true,
            bool thirdParty = true,
            bool import3rdParty = false,
            bool export3rdParty = false)
        {
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
                int op = Engine.BeginOperation($"Loading file from {filePath}...", $"{filePath} loaded successfully.", out Progress<float> progress, out CancellationTokenSource cancel);
                file = (T)(await LoadAsync(typeof(T), filePath, progress, cancel.Token));
                if (Engine.EndOperation != null)
                    Engine.EndOperation(op);
                else
                    ((IProgress<float>)progress).Report(1.0f);
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
        /// <param name="expectedType">The type of the file object to load.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A new instance of the file.</returns>
        public static async Task<object> LoadAsync(
            TypeProxy expectedType, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            TFileExt extAttrib = GetFileExtension(expectedType);
            TFile3rdPartyExt tpAttrib = GetFile3rdPartyExtensions(expectedType);
            EFileFormat fmt = GetFormat(filePath, out string ext);

            if (extAttrib != null)
            {
                if (fmt == EFileFormat.ThirdParty)
                {
                    bool hasWildcard = extAttrib.HasImportableExtension("*");
                    bool hasExt = extAttrib.HasImportableExtension(ext);
                    if (hasWildcard || hasExt)
                        return await Read3rdPartyAsync(expectedType, filePath, progress, cancel);
                }
                else
                {
                    EProprietaryFileFormat pfmt = (EProprietaryFileFormat)(int)fmt;

                    string fileExt = extAttrib.GetFullExtension(pfmt);
                    if (string.Equals(ext, fileExt))
                    {
                        var file = fmt == EFileFormat.XML ?
                            await FromXMLAsync(filePath, progress, cancel) :
                            await FromBinaryAsync(filePath, progress, cancel);

                        if (file != null && expectedType != null)
                        {
                            Type fileType = file?.GetType();
                            if (!expectedType.IsAssignableFrom(fileType))
                            {
                                Engine.LogWarning($"{fileType.GetFriendlyName()} is not assignable to {expectedType.GetFriendlyName()}.");
                                return null;
                            }
                        }

                        return file;
                    }
                }
            }
            else if (tpAttrib != null)
            {
                bool hasWildcard = tpAttrib.HasExtension("*");
                bool hasExt = tpAttrib.HasExtension(ext);
                if (hasWildcard || hasExt)
                    return await Read3rdPartyAsync(expectedType, filePath, progress, cancel);
            }

            Engine.LogWarning("{0} cannot be loaded as {1}.", filePath, expectedType.GetFriendlyName());
            return default;
        }
        #endregion

        #region XML
        internal static async Task<T> FromXMLAsync<T>(
            string filePath, IProgress<float> progress, CancellationToken cancel)
            where T : class, IFileObject
            => await FromXMLAsync(filePath, progress, cancel) as T;
        internal static async Task<IFileObject> FromXMLAsync(
            string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                TypeProxy fileType = SerializationCommon.DetermineType(filePath, out EFileFormat format);
                if (fileType == null)
                    return null;

                Deserializer deser = new Deserializer();
                IFileObject file = await deser.DeserializeXMLAsync(filePath, progress, cancel) as IFileObject;
                return file;
            }
            catch (Exception ex)
            {
                Engine.LogWarning($"Unable to deserialize XML file at {filePath}.\n{ex.ToString()}");
            }
            return null;
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

        internal static async Task<T> FromBinaryAsync<T>(string filePath, IProgress<float> progress, CancellationToken cancel) where T : class, IFileObject
            => await FromBinaryAsync(filePath, progress, cancel) as T;
        internal static async Task<IFileObject> FromBinaryAsync(
            string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                TypeProxy fileType = SerializationCommon.DetermineType(filePath, out EFileFormat format);
                if (format != EFileFormat.Binary)
                {
                    Engine.LogWarning($"Specified to read in {(EFileFormat.Binary).ToString()} format, but file is {format.ToString()}. Reading anyway.");
                }

                Deserializer deser = new Deserializer();
                IFileObject file = await deser.DeserializeBinaryAsync(filePath, progress, cancel,  null) as IFileObject;
                return file;
            }
            catch (Exception ex)
            {
                Engine.LogWarning($"Unable to deserialize binary file at {filePath}.\n{ex.ToString()}");
            }
            return null;
        }
        #endregion

        #region 3rd Party
        public static async Task<T> Read3rdPartyAsync<T>(string filePath)
            => (T)(await Read3rdPartyAsync(TypeProxy.TypeOf<T>(), filePath, null, CancellationToken.None));
        public static async Task<object> Read3rdPartyAsync(TypeProxy classType, string filePath)
            => await Read3rdPartyAsync(classType, filePath, null, CancellationToken.None);
        public static async Task<T> Read3rdPartyAsync<T>(string filePath, IProgress<float> progress, CancellationToken cancel)
            => (T)await Read3rdPartyAsync(TypeProxy.TypeOf<T>(), filePath, progress, cancel);
        public static async Task<object> Read3rdPartyAsync(TypeProxy classType, string filePath, IProgress<float> progress, CancellationToken cancel)
        {
            string ext = Path.GetExtension(filePath);
            if (ext != null && ext.StartsWith("."))
                ext = ext.Substring(1);

            Delegate loader = Get3rdPartyLoader(classType, ext);
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

        private static Dictionary<string, Dictionary<TypeProxy, Delegate>> _3rdPartyLoaders;
        private static Dictionary<string, Dictionary<TypeProxy, Delegate>> _3rdPartyExporters;

        public static Delegate Get3rdPartyLoader(TypeProxy fileType, string extension)
        {
            if (_3rdPartyLoaders == null)
                Reset3rdPartyImportExportMethods();
            return Get3rdPartyMethod(_3rdPartyLoaders, fileType, extension);
        }
        public static Delegate Get3rdPartyExporter(TypeProxy fileType, string extension)
        {
            if (_3rdPartyExporters == null)
                Reset3rdPartyImportExportMethods();
            return Get3rdPartyMethod(_3rdPartyExporters, fileType, extension);
        }
        private static Delegate Get3rdPartyMethod(Dictionary<string, Dictionary<TypeProxy, Delegate>> methodDic, TypeProxy fileType, string extension)
        {
            extension = extension.ToLowerInvariant();
            if (methodDic != null && methodDic.ContainsKey(extension))
            {
                var t = methodDic[extension];
                if (t.ContainsKey(fileType))
                    return t[fileType];
            }
            return null;
        }

        public static void Register3rdPartyLoader<T>(string extension, Del3rdPartyImportFileMethod<T> loadMethod) where T : class, IFileObject
        {
            if (_3rdPartyLoaders == null)
                Reset3rdPartyImportExportMethods();
            Register3rdParty<T>(_3rdPartyLoaders, extension, loadMethod);
        }
        public static void Register3rdPartyExporter<T>(string extension, Del3rdPartyImportFileMethod<T> exportMethod) where T : class, IFileObject
        {
            if (_3rdPartyExporters == null)
                Reset3rdPartyImportExportMethods();
            Register3rdParty<T>(_3rdPartyExporters, extension, exportMethod);
        }
        public static void Register3rdPartyLoader<T>(string extension, Del3rdPartyImportFileMethodAsync<T> loadMethod) where T : class, IFileObject
        {
            if (_3rdPartyLoaders == null)
                Reset3rdPartyImportExportMethods();
            Register3rdParty<T>(_3rdPartyLoaders, extension, loadMethod);
        }
        public static void Register3rdPartyExporter<T>(string extension, Del3rdPartyImportFileMethodAsync<T> exportMethod) where T : class, IFileObject
        {
            if (_3rdPartyExporters == null)
                Reset3rdPartyImportExportMethods();
            Register3rdParty<T>(_3rdPartyExporters, extension, exportMethod);
        }
        
        private static void Register3rdParty<T>(
            Dictionary<string, Dictionary<TypeProxy, Delegate>> methodDic,
            string extension,
            Delegate method)
            where T : class, IFileObject
        {
            extension = extension.ToLowerInvariant();

            if (methodDic == null)
                methodDic = new Dictionary<string, Dictionary<TypeProxy, Delegate>>();

            Dictionary<TypeProxy, Delegate> typesforExt;
            if (!methodDic.ContainsKey(extension))
                methodDic.Add(extension, typesforExt = new Dictionary<TypeProxy, Delegate>());
            else
                typesforExt = methodDic[extension];

            TypeProxy fileType = TypeProxy.TypeOf<T>();
            if (!typesforExt.ContainsKey(fileType))
                typesforExt.Add(fileType, method);
            else
                throw new Exception("Registered " + extension + " for " + fileType.GetFriendlyName() + " too many times.");
        }
        #endregion
    }
}
