using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Reflection.Attributes;
using Extensions;

namespace TheraEngine.Core.Files
{
    public interface IFileLoader : IFileObject
    {
        PathReference Path { get; set; }
        TypeProxy ReferencedType { get; }
    }

    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    [Serializable]
    [TFileExt("ldr")]
    [TFileDef("File Loader")]
    public class FileLoader<T> : TFileObject, IFileLoader where T : class, IFileObject
    {
        public event DelPathChange AbsoluteRefPathChanged;
        public event DelPathChange RelativeRefPathChanged;

        public delegate void DelPathChange(string oldPath, string newPath);

        #region Constructors
        public FileLoader() : this(typeof(T)) { }
        public FileLoader(TypeProxy type)
        {
            if (type.IsAssignableTo(typeof(T)))
                SubType = type;
            else
                throw new Exception(type.GetFriendlyName() + " is not assignable to " + typeof(T).GetFriendlyName());

            Path = new PathReference();
        }
        public FileLoader(string filePath)
        {
            SubType = typeof(T);
            //if (Path.HasExtension(filePath) && FileManager.GetTypeWithExtension(Path.GetExtension(filePath)) != _subType)
            //    throw new InvalidOperationException("Extension does not match type");
            Path = new PathReference
            {
                Path = filePath
            };
        }
        public FileLoader(string filePath, TypeProxy type)
        {
            if (type != null && !type.IsAssignableTo(typeof(T)))
                throw new Exception(type.GetFriendlyName() + " is not assignable to " + typeof(T).GetFriendlyName());

            SubType = type;

            //if (Path.HasExtension(filePath) && FileManager.GetTypeWithExtension(Path.GetExtension(filePath)) != _subType)
            //    throw new InvalidOperationException("Extension does not match type");
            Path = new PathReference
            {
                Path = string.IsNullOrWhiteSpace(filePath) ? null : filePath
            };
        }
        public FileLoader(string dir, string name, EProprietaryFileFormat format) 
            : this(GetFilePath(dir, name, format, typeof(T))) { }
        #endregion
        
        [Browsable(false)]
        public TypeProxy SubType
        {
            get => _subType ?? (_subType = Engine.DomainProxy.GetTypeFor<T>());
            set
            {
                if (!(value is null) && value.IsAssignableTo(typeof(T)))
                    _subType = value;
            }
        }
        private TypeProxy _subType = null;

        protected bool _updating;
        private PathReference _path;

        [Category("Construction")]
        [TSerialize]
        public (Type Type, object Value)[] DefaultConstructionArguments { get; set; } = null;

        [Category("Construction")]
        [TSerialize]
        public bool CreateFileIfNonExistent { get; set; } = false;
        [Category("Construction")]
        [TSerialize]
        public bool AllowDynamicConstruction { get; set; } = false;

        [Category("Loading")]
        public EPathType PathType
        {
            get => _path.Type;
            set => _path.Type = value;
        }
        [TString(false, true)]
        [Category("Loading")]
        public string AbsolutePath
        {
            get => _path.Path;
            set => _path.Path = value;
        }

        [Browsable(false)]
        [Category("File Reference")]
        [DisplayName("Reference Path")]
        [TSerialize]
        public PathReference Path
        {
            get => _path;
            set
            {
                if (_path != null)
                    _path.AbsolutePathChanged -= OnAbsoluteRefPathChanged;
                _path = value ?? new PathReference();
                _path.AbsolutePathChanged += OnAbsoluteRefPathChanged;
            }
        }

        //[Browsable(false)]
        //[TString(false, true, false)]
        //[Category("Object")]
        //public override string FilePath
        //{
        //    get => base.FilePath;
        //    set
        //    {
        //        base.FilePath = value;
        //        if (Path.Type == EPathType.FileRelative)
        //        {
        //            if (!string.IsNullOrWhiteSpace(RootFile.DirectoryPath) && 
        //                !string.IsNullOrWhiteSpace(Path.Path) &&
        //                Path.Path.IsValidExistingPath())
        //            {
        //                Path.Relative = Path.Path.MakeAbsolutePathRelativeTo(RootFile.DirectoryPath);
        //            }
        //        }
        //    }
        //}
        
        /// <summary>
        /// Returns true if a file exists at the path that this reference points to.
        /// </summary>
        [Browsable(false)]
        public virtual bool FileExists
        {
            get
            {
                if (!Path.FileExists)
                    return false;

                TypeProxy fileType = DetermineType(Path.Path);
                return fileType != null && SubType.IsAssignableFrom(fileType);
            }
        }

        [Browsable(false)]
        public TypeProxy ReferencedType => SubType;

        private event Action<T> Loaded_Internal;
        public virtual event Action<T> Loaded
        {
            add
            {
                if (value != null)
                    Loaded_Internal += value;
            }
            remove
            {
                if (value != null)
                    Loaded_Internal -= value;
            }
        }

        protected void OnLoaded(T file) => Loaded_Internal?.Invoke(file);

        protected virtual void OnAbsoluteRefPathChanged(string oldPath, string newPath)
        {
            if (!_updating)
                AbsoluteRefPathChanged?.Invoke(oldPath, newPath);
        }
        protected virtual void OnRelativeRefPathChanged(string oldPath, string newPath)
        {
            if (!_updating)
                RelativeRefPathChanged?.Invoke(oldPath, newPath);
        }

        ///// <summary>
        ///// Loads a new instance synchronously and allows dynamic construction when the ReferencePathAbsolute is invalid, assuming there is a constructor with no arguments.
        ///// </summary>
        //public T LoadNewInstance()
        //    => LoadNewInstance(null);

        /// <summary>
        /// Loads a new instance synchronously.
        /// </summary>
        public T LoadNewInstance()
        {
            Func<Task<T>> func = async () => await LoadNewInstanceAsync();
            return func.RunSync();
        }
        
        public void LoadNewInstanceAsync(IProgress<float> progress, CancellationToken cancel, Action<T> onLoaded)
            => LoadNewInstanceAsync(progress, cancel).ContinueWith(t => onLoaded?.Invoke(t.Result));
        
        /// <summary>
        /// Loads a new instance asynchronously.
        /// </summary>
        /// <param name="onLoaded"></param>
        public void LoadNewInstanceAsync(Action<T> onLoaded)
            => LoadNewInstanceAsync().ContinueWith(t => onLoaded?.Invoke(t.Result));

        public async Task<T> LoadNewInstanceAsync()
            => (await LoadNewInstanceAsync(null, CancellationToken.None));

        public async Task<T> LoadNewInstanceAsync(IProgress<float> progress, CancellationToken cancel)
        {
            var (Instance, _, _) = await LoadNewInstanceAsync(progress, cancel, true);
            return Instance;
        }

        public async Task<(T Instance, bool LoadedFromFile, bool LoadAttempted)> LoadNewInstanceAsync(
            IProgress<float> progress,
            CancellationToken cancel,
            bool allowLoadingFromFile = true)
        {
            T file = null;

            string absolutePath = Path.Path;
            bool loadedFromFile = false;
            bool loadAttempted = false;

            if (!allowLoadingFromFile || !absolutePath.IsAbsolutePath() || !File.Exists(absolutePath))
            {
                if (AllowDynamicConstruction)
                {
                    file = DynamicConstruct(DefaultConstructionArguments, absolutePath);
                    if (CreateFileIfNonExistent && !(file is null))
                        await file.ExportAsync(absolutePath, ESerializeFlags.Default, progress, cancel);
                }
                else
                {
                    //Engine.LogWarning($"No file exists at \"{absolutePath}\" and the '{nameof(AllowDynamicConstruction)}' property is not enabled.");
                }
            }
            else
            {
                loadAttempted = true;
                try
                {
                    string ext = System.IO.Path.GetExtension(absolutePath);
                    if (IsThirdPartyImportableExt(ext?.Substring(1)))
                    {
                        file = SubType.CreateInstance() as T;
                        file.FilePath = absolutePath;
                        file.ManualRead3rdParty(absolutePath);
                        loadedFromFile = true;
                    }
                    else
                    {
                        switch (GetFormat())
                        {
                            case EFileFormat.XML:
                                file = await FromXMLAsync(absolutePath, progress, cancel) as T;
                                loadedFromFile = true;
                                break;
                            case EFileFormat.Binary:
                                file = await FromBinaryAsync(absolutePath, progress, cancel) as T;
                                loadedFromFile = true;
                                break;
                            default:
                                Engine.LogWarning($"Could not load file at \"{absolutePath}\". Invalid file format.");
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Engine.LogWarning($"Could not load file at \"{absolutePath}\".\nException:\n\n{e}");
                }
            }

            if (file != null && SubType != null)
            {
                TypeProxy fileType = file?.GetTypeProxy();
                if (!SubType.IsAssignableFrom(fileType))
                {
                    Engine.LogWarning($"{fileType.GetFriendlyName()} is not assignable to {SubType.GetFriendlyName()}.");
                    return (null, loadedFromFile, loadAttempted);
                }

                OnFileLoaded(file);
                OnLoaded(file);
            }

            return (file, loadedFromFile, loadAttempted);
        }

        protected virtual void OnFileLoaded(T file)
        {
            
        }

        /// <summary>
        /// Retrieves the extension from the reference path.
        /// Returns lowercase WITHOUT a period as the first char.
        /// </summary>
        public string Extension() => System.IO.Path.GetExtension(Path.Path)?.ToLowerInvariant().Substring(1);

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

        private T DynamicConstruct(
            (Type Type, object Value)[] constructionArgs,
            string absolutePath)
        {
            T file = ConstructNewInstance_Internal(false, constructionArgs);
            if (file != null)
                file.FilePath = absolutePath;
            return file;
        }

        /// <summary>
        /// Constructs a new instance of the referenced type using the constructor with the given parameters, if it exists.
        /// If it does not exist, or the type is abstract or an interface, returns null.
        /// Does NOT load from the reference path.
        /// </summary>
        public T ConstructNewInstance(params (Type Type, object Value)[] args) => ConstructNewInstance_Internal(true, args);
        protected T ConstructNewInstance_Internal(bool callLoadedEvent, (Type Type, object Value)[] args)
        {
            if (SubType.IsAbstract)
            {
                Engine.LogWarning("Can't automatically instantiate an abstract class: " + SubType.GetFriendlyName());
                return null;
            }
            else if (SubType.IsInterface)
            {
                Engine.LogWarning("Can't automatically instantiate an interface: " + SubType.GetFriendlyName());
                return null;
            }
            else
            {
                if (args is null)
                    args = new (Type, object)[0];
                if (SubType.GetConstructor(args.Select(x => x.Type).ToArray()) is null)
                {
                    Engine.LogWarning("Can't automatically instantiate '" + SubType.GetFriendlyName() + "' with " + (args.Length == 0 ?
                        "no parameters." : "these parameters: " + string.Join(", ", args.Select(x => x.Type.GetFriendlyName()))));
                    return null;
                }
            }

            T file;
            if (args.Length == 0)
                file = SubType.CreateInstance() as T;
            else
                file = SubType.CreateInstance(args.Select(x => x.Value)) as T;

            if (callLoadedEvent)
                OnLoaded(file);

            return file;
        }

        /// <summary>
        /// Returns true if the file needs special deserialization handling.
        /// If so, the class needs a constructor that takes the file's absolute path (string) as its only argument.
        /// </summary>
        private bool IsThirdPartyImportableExt(string ext)
        {
            var header = GetFile3rdPartyExtensions(SubType);
            return header?.HasExtension(ext) ?? false;
        }
        //private bool IsThirdPartyExportableExt(string ext)
        //{
        //    var header = GetFileExtension(_subType);
        //    return header?.ExportableExtensions?.Contains(ext, StringComparison.InvariantCultureIgnoreCase) ?? false;
        //}
        public override string ToString() => Path.Path;

        public static implicit operator Task<T>(FileLoader<T> fileRef) => fileRef?.LoadNewInstanceAsync();
        public static implicit operator FileLoader<T>(string filePath) => new FileLoader<T>(filePath);
    }
}