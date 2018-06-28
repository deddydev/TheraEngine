﻿using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Files
{
    public interface IFileLoader : IFileObject
    {
        EPathType PathType { get; set; }
        string ReferencePath { get; set; }
        string ReferencePathAbsolute { get; set; }
        Type ReferencedType { get; }
    }
    public enum EPathType
    {
        Absolute,
        EngineRelative,
        FileRelative,
    }
    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    [FileExt("ldr")]
    [FileDef("File Loader")]
    public class FileLoader<T> : TFileObject, IFileLoader where T : class, IFileObject
    {
        #region Constructors
        public FileLoader() : this(typeof(T)) { }
        public FileLoader(Type type)
        {
            if (typeof(T).IsAssignableFrom(type))
                _subType = type;
            else
                throw new Exception(type.ToString() + " is not assignable to " + typeof(T).ToString());
        }
        public FileLoader(string filePath)
        {
            _subType = typeof(T);
            //if (Path.HasExtension(filePath) && FileManager.GetTypeWithExtension(Path.GetExtension(filePath)) != _subType)
            //    throw new InvalidOperationException("Extension does not match type");
            ReferencePathAbsolute = filePath;
        }
        public FileLoader(string filePath, Type type)
        {
            if (typeof(T).IsAssignableFrom(type))
                _subType = type;
            else
                throw new Exception(type.ToString() + " is not assignable to " + typeof(T).ToString());
            //if (Path.HasExtension(filePath) && FileManager.GetTypeWithExtension(Path.GetExtension(filePath)) != _subType)
            //    throw new InvalidOperationException("Extension does not match type");
            ReferencePathAbsolute = filePath;
        }
        public FileLoader(string dir, string name, EProprietaryFileFormat format) : this(GetFilePath(dir, name, format, typeof(T))) { }
        #endregion
        
        protected string _localRefPath;
        protected string _absoluteRefPath;
        protected Type _subType = null;
        protected EPathType _pathType = EPathType.FileRelative;

        [Category("File Reference")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public EPathType PathType
        {
            get => _pathType;
            set
            {
                _pathType = value;

                if (_pathType == EPathType.Absolute)
                    _localRefPath = _absoluteRefPath;
                else if (!string.IsNullOrWhiteSpace(_absoluteRefPath) && _absoluteRefPath.IsValidPath())
                {
                    if (PathType == EPathType.EngineRelative)
                        _localRefPath = _absoluteRefPath.MakePathRelativeTo(Application.StartupPath);
                    else
                    {
                        if (!string.IsNullOrEmpty(DirectoryPath))
                            _localRefPath = _absoluteRefPath.MakePathRelativeTo(DirectoryPath);
                        else
                            _localRefPath = null;
                    }
                }
                else
                    _localRefPath = null;
            }
        }

        [TString(false, true, false)]
        [Category("File Reference")]
        public virtual string ReferencePathAbsolute
        {
            get => _absoluteRefPath;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _absoluteRefPath = Path.GetFullPath(value);
                    if (PathType == EPathType.Absolute)
                        _localRefPath = _absoluteRefPath;
                    else
                    {
                        string root = Path.GetPathRoot(value);
                        int colonIndex = root.IndexOf(":");
                        if (colonIndex > 0)
                            root = root.Substring(0, colonIndex);
                        else
                            root = string.Empty;

                        if (PathType == EPathType.EngineRelative)
                        {
                            string root2 = Path.GetPathRoot(Application.StartupPath);
                            colonIndex = root2.IndexOf(":");
                            if (colonIndex > 0)
                                root2 = root2.Substring(0, colonIndex);
                            else
                                root2 = string.Empty;
                            if (!string.Equals(root, root2))
                            {
                                PathType = EPathType.Absolute;
                                _localRefPath = _absoluteRefPath;
                            }
                            else
                                _localRefPath = _absoluteRefPath.MakePathRelativeTo(Application.StartupPath);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(DirectoryPath))
                            {
                                string root2 = Path.GetPathRoot(DirectoryPath);
                                colonIndex = root2.IndexOf(":");
                                if (colonIndex > 0)
                                    root2 = root2.Substring(0, colonIndex);
                                else
                                    root2 = string.Empty;
                                if (!string.Equals(root, root2))
                                {
                                    PathType = EPathType.Absolute;
                                    _localRefPath = _absoluteRefPath;
                                }
                                else
                                    _localRefPath = _absoluteRefPath.MakePathRelativeTo(DirectoryPath);
                            }
                            else
                                _localRefPath = null;
                        }
                    }
                }
                else
                {
                    _localRefPath = null;
                    _absoluteRefPath = null;
                }
            }
        }
        [TString(false, true, false)]
        [Category("File Reference")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public virtual string ReferencePath
        {
            get => _localRefPath;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _localRefPath = value;
                    if (PathType == EPathType.Absolute)
                    {
                        _absoluteRefPath = _localRefPath;
                    }
                    else
                    {
                        bool engineRelative = PathType == EPathType.EngineRelative || string.IsNullOrEmpty(DirectoryPath);
                        string combinedPath = (engineRelative ? Application.StartupPath : DirectoryPath) + _localRefPath;
                        _absoluteRefPath = Path.GetFullPath(combinedPath);
                    }
                }
                else
                {
                    _localRefPath = null;
                    _absoluteRefPath = null;
                }
            }
        }
        /// <summary>
        /// Returns true if a file exists at the path that this reference points to.
        /// </summary>
        [Browsable(false)]
        public virtual bool FileExists
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ReferencePathAbsolute))
                    return false;
                if (!File.Exists(ReferencePathAbsolute))
                    return false;
                Type fileType = DetermineType(ReferencePathAbsolute);
                if (fileType == null)
                    return false;
                return _subType.IsAssignableFrom(fileType);
            }
        }

        [Browsable(false)]
        public Type ReferencedType => _subType;
        
        private event Action<T> Loaded;
        /// <summary>
        /// Method to be called when a new instance is loaded.
        /// </summary>
        public virtual void RegisterLoadEvent(Action<T> onLoaded)
        {
            if (onLoaded == null)
                return;

            Loaded += onLoaded;
        }
        public virtual void UnregisterLoadEvent(Action<T> onLoaded)
        {
            if (onLoaded == null)
                return;
            Loaded -= onLoaded;
        }
        public virtual T LoadNewInstance(bool allowConstruct, Type[] constructorTypes, object[] constructionArgs)
        {
            string absolutePath = ReferencePathAbsolute;

            if (string.IsNullOrWhiteSpace(absolutePath))
            {
                if (allowConstruct)
                {
                    T file = DynamicConstructNewInstance_Internal(false, constructorTypes, constructionArgs);
                    if (file != null)
                    {
                        file.FilePath = absolutePath;
                        OnFileLoaded(file);
                        Loaded?.Invoke(file);
                    }
                    return file;
                }
                return null;
            }

            if (!File.Exists(absolutePath))
            {
                Engine.LogWarning(string.Format("No file exists at \"{0}\".", absolutePath));
                return null;
            }

            try
            {
                if (IsThirdPartyImportableExt(Path.GetExtension(absolutePath).Substring(1)))
                {
                    T file = Activator.CreateInstance<T>();
                    file.FilePath = absolutePath;
                    file.Read3rdParty(absolutePath);
                    OnFileLoaded(file);
                    Loaded?.Invoke(file);
                    return file;
                }
                else
                {
                    T file = null;
                    switch (GetFormat())
                    {
                        case EFileFormat.XML:
                            file = FromXML(_subType, absolutePath) as T;
                            break;
                        case EFileFormat.Binary:
                            file = FromBinary(_subType, absolutePath) as T;
                            break;
                        default:
                            Engine.LogWarning(string.Format("Could not load file at \"{0}\". Invalid file format.", absolutePath));
                            break;
                    }
                    if (file != null)
                    {
                        OnFileLoaded(file);
                        Loaded?.Invoke(file);
                    }
                    return file;
                }
            }
            catch (Exception e)
            {
                Engine.LogWarning(string.Format("Could not load file at \"{0}\".\nException:\n\n{1}", absolutePath, e.ToString()));
            }

            return null;
        }

        protected virtual void OnFileLoaded(T file)
        {
            
        }

        /// <summary>
        /// Retrieves the extension from the reference path.
        /// Returns lowercase WITHOUT a period as the first char.
        /// </summary>
        public string Extension() => ReferencePathAbsolute == null ? null :
            Path.GetExtension(ReferencePathAbsolute).ToLowerInvariant().Substring(1);

        /// <summary>
        /// Retrieves the proprietary file format type from the extension.
        /// Does not account for extensions that are 3rd party.
        /// </summary>
        public EFileFormat GetFormat()
        {
            string ext = Extension();
            if (!string.IsNullOrEmpty(ext))
            {
                switch (ext.ToLowerInvariant()[0])
                {
                    case 'x': return EFileFormat.XML;
                    case 'b': return EFileFormat.Binary;
                }
            }
            return EFileFormat.Binary;
        }

        public void LoadNewInstanceAsync(Action<T> onLoaded, TaskCreationOptions options = TaskCreationOptions.PreferFairness)
            => LoadNewInstanceAsync(options).ContinueWith(task => onLoaded(task.Result));
        public void LoadNewInstanceAsync(Action<T> onLoaded)
            => LoadNewInstanceAsync().ContinueWith(task => onLoaded(task.Result));
        public async Task<T> LoadNewInstanceAsync() => await Task.Run(() => LoadNewInstance(true, null, null));
        public async Task<T> LoadNewInstanceAsync(TaskCreationOptions options) => await Task.Factory.StartNew(() => LoadNewInstance(true, null, null), options);

        /// <summary>
        /// Constructs a new instance of the referenced type using the constructor with the given parameters, if it exists.
        /// If it does not exist, or the type is abstract or an interface, returns null.
        /// Does NOT load from the reference path.
        /// </summary>
        public T DynamicConstructNewInstance(Type[] types, object[] args) => DynamicConstructNewInstance_Internal(true, types, args);
        public T DynamicConstructNewInstance(params object[] args) => DynamicConstructNewInstance_Internal(true, args?.Select(x => x.GetType())?.ToArray(), args);
        protected T DynamicConstructNewInstance_Internal(bool callLoadedEvent, Type[] types, object[] args)
        {
            if (_subType.IsAbstract)
            {
                Engine.LogWarning("Can't automatically instantiate an abstract class: " + _subType.GetFriendlyName());
                return null;
            }
            else if (_subType.IsInterface)
            {
                Engine.LogWarning("Can't automatically instantiate an interface: " + _subType.GetFriendlyName());
                return null;
            }
            else
            {
                if (args == null)
                    args = new object[0];
                if (types == null)
                    types = args?.Select(x => x.GetType())?.ToArray();
                if (_subType.GetConstructor(types) == null)
                {
                    Engine.LogWarning("Can't automatically instantiate '" + _subType.GetFriendlyName() + "' with " + (types.Length == 0 ?
                        "no parameters." : "these parameters: " + string.Join(", ", types.Select(x => x.GetFriendlyName()))));
                    return null;
                }
            }
            T file =  Activator.CreateInstance(_subType, args) as T;
            if (callLoadedEvent)
                Loaded?.Invoke(file);
            return file;
        }

        /// <summary>
        /// Returns true if the file needs special deserialization handling.
        /// If so, the class needs a constructor that takes the file's absolute path (string) as its only argument.
        /// </summary>
        private bool IsThirdPartyImportableExt(string ext)
        {
            File3rdParty header = GetFile3rdPartyExtensions(_subType);
            return header?.ImportableExtensions?.Contains(ext, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }
        private bool IsThirdPartyExportableExt(string ext)
        {
            File3rdParty header = GetFile3rdPartyExtensions(_subType);
            return header?.ExportableExtensions?.Contains(ext, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }
        public override string ToString() => ReferencePathAbsolute;
        
        public static implicit operator T(FileLoader<T> fileRef) => fileRef?.LoadNewInstance(true, null, null);
        public static implicit operator FileLoader<T>(string filePath) => new FileLoader<T>(filePath);
    }
}