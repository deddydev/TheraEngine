using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Core.Files
{
    public interface IFileLoader : IFileObject
    {
        PathReference Path { get; set; }
        Type ReferencedType { get; }
    }
    
    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    [TFileExt("ldr")]
    [TFileDef("File Loader")]
    public class FileLoader<T> : TFileObject, IFileLoader where T : class, IFileObject
    {
        public event DelPathChange AbsoluteRefPathChanged;
        public event DelPathChange RelativeRefPathChanged;

        public delegate void DelPathChange(string oldPath, string newPath);

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
            Path.Absolute = filePath;
        }
        public FileLoader(string filePath, Type type)
        {
            if (typeof(T).IsAssignableFrom(type))
                _subType = type;
            else
                throw new Exception(type.ToString() + " is not assignable to " + typeof(T).ToString());
            //if (Path.HasExtension(filePath) && FileManager.GetTypeWithExtension(Path.GetExtension(filePath)) != _subType)
            //    throw new InvalidOperationException("Extension does not match type");
            Path.Absolute = filePath;
        }
        public FileLoader(string dir, string name, EProprietaryFileFormat format) : this(GetFilePath(dir, name, format, typeof(T))) { }
        #endregion
        
        protected Type _subType = null;
        protected bool _updating;
        private PathReference _path = new PathReference();

        [Category("File Reference")]
        [DisplayName("Reference Path")]
        [TSerialize]
        public PathReference Path
        {
            get => _path;
            set
            {
                if (_path != null)
                {
                    _path.RelativePathChanged -= OnRelativeRefPathChanged;
                    _path.AbsolutePathChanged -= OnAbsoluteRefPathChanged;
                }

                _path = value ?? new PathReference();

                _path.RelativePathChanged += OnRelativeRefPathChanged;
                _path.AbsolutePathChanged += OnAbsoluteRefPathChanged;
            }
        }

        [Browsable(false)]
        [TString(false, true, false)]
        [Category("Object")]
        public override string FilePath
        {
            get => base.FilePath;
            set
            {
                base.FilePath = value;
                if (Path.Type == EPathType.FileRelative)
                {
                    if (!string.IsNullOrWhiteSpace(RootFile.DirectoryPath) && 
                        !string.IsNullOrWhiteSpace(Path.Absolute) &&
                        Path.Absolute.IsValidExistingPath())
                    {
                        Path.Relative = Path.Absolute.MakeAbsolutePathRelativeTo(RootFile.DirectoryPath);
                    }
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
                if (!Path.FileExists)
                    return false;

                Type fileType = DetermineType(Path.Absolute);
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

        /// <summary>
        /// Loads a new instance synchronously and allows dynamic construction when the ReferencePathAbsolute is invalid, assuming there is a constructor with no arguments.
        /// </summary>
        public T LoadNewInstance()
            => LoadNewInstance(true, null);

        /// <summary>
        /// Loads a new instance synchronously.
        /// </summary>
        /// <param name="allowConstruct"></param>
        /// <param name="constructionArgs"></param>
        public T LoadNewInstance(bool allowConstruct, (Type Type, object Value)[] constructionArgs)
        {
            Func<Task<T>> func = async () => { return await LoadNewInstanceAsync(allowConstruct, constructionArgs); };
            return func.RunSync();
        }

        public void LoadNewInstanceAsync(Action<T> onLoaded)
            => LoadNewInstanceAsync(true, null).ContinueWith(t => onLoaded?.Invoke(t.Result));

        public void LoadNewInstanceAsync(IProgress<float> progress, CancellationToken cancel, Action<T> onLoaded)
            => LoadNewInstanceAsync(true, null, progress, cancel).ContinueWith(t => onLoaded?.Invoke(t.Result));

        public void LoadNewInstanceAsync(bool allowConstruct, (Type Type, object Value)[] constructionArgs, IProgress<float> progress, CancellationToken cancel, Action<T> onLoaded)
            => LoadNewInstanceAsync(allowConstruct, constructionArgs, progress, cancel).ContinueWith(t => onLoaded?.Invoke(t.Result));

        /// <summary>
        /// Loads a new instance asynchronously.
        /// </summary>
        /// <param name="allowConstruct"></param>
        /// <param name="constructionArgs"></param>
        /// <param name="onLoaded"></param>
        public void LoadNewInstanceAsync(bool allowConstruct, (Type Type, object Value)[] constructionArgs, Action<T> onLoaded)
            => LoadNewInstanceAsync(allowConstruct, constructionArgs).ContinueWith(t => onLoaded?.Invoke(t.Result));

        public async Task<T> LoadNewInstanceAsync()
            => await LoadNewInstanceAsync(true, null, null, CancellationToken.None);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowConstruct"></param>
        /// <param name="constructionArgs"></param>
        /// <returns></returns>
        public async Task<T> LoadNewInstanceAsync(bool allowConstruct, (Type Type, object Value)[] constructionArgs)
            => await LoadNewInstanceAsync(allowConstruct, constructionArgs, null, CancellationToken.None);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public async Task<T> LoadNewInstanceAsync(IProgress<float> progress, CancellationToken cancel)
            => await LoadNewInstanceAsync(true, null, progress, cancel);

        public virtual async Task<T> LoadNewInstanceAsync(
            bool allowConstruct,
            (Type Type, object Value)[] constructionArgs,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            string absolutePath = Path.Absolute;

            if (string.IsNullOrWhiteSpace(absolutePath))
            {
                if (allowConstruct)
                {
                    T file = DynamicConstructNewInstance_Internal(false, constructionArgs);
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
                if (IsThirdPartyImportableExt(System.IO.Path.GetExtension(absolutePath).Substring(1)))
                {
                    T file = Activator.CreateInstance<T>();
                    file.FilePath = absolutePath;
                    file.ManualRead3rdParty(absolutePath);
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
                            file = await FromXMLAsync(_subType, absolutePath, progress, cancel) as T;
                            break;
                        case EFileFormat.Binary:
                            file = await FromBinaryAsync(_subType, absolutePath, progress, cancel) as T;
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
        public string Extension() => Path.Absolute == null ? null :
            System.IO.Path.GetExtension(Path.Absolute).ToLowerInvariant().Substring(1);

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

        /// <summary>
        /// Constructs a new instance of the referenced type using the constructor with the given parameters, if it exists.
        /// If it does not exist, or the type is abstract or an interface, returns null.
        /// Does NOT load from the reference path.
        /// </summary>
        public T DynamicConstructNewInstance(params (Type Type, object Value)[] args) => DynamicConstructNewInstance_Internal(true, args);
        protected T DynamicConstructNewInstance_Internal(bool callLoadedEvent, (Type Type, object Value)[] args)
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
                    args = new (Type, object)[0];
                if (_subType.GetConstructor(args.Select(x => x.Type).ToArray()) == null)
                {
                    Engine.LogWarning("Can't automatically instantiate '" + _subType.GetFriendlyName() + "' with " + (args.Length == 0 ?
                        "no parameters." : "these parameters: " + string.Join(", ", args.Select(x => x.Type.GetFriendlyName()))));
                    return null;
                }
            }

            T file;
            if (args.Length == 0)
                file = Activator.CreateInstance(_subType) as T;
            else
                file = Activator.CreateInstance(_subType, args.Select(x => x.Value)) as T;

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
            TFile3rdParty header = GetFile3rdPartyExtensions(_subType);
            return header?.ImportableExtensions?.Contains(ext, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }
        private bool IsThirdPartyExportableExt(string ext)
        {
            TFile3rdParty header = GetFile3rdPartyExtensions(_subType);
            return header?.ExportableExtensions?.Contains(ext, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }
        public override string ToString() => Path.Absolute;

        public static implicit operator Task<T>(FileLoader<T> fileRef) => fileRef?.LoadNewInstanceAsync();
        public static implicit operator FileLoader<T>(string filePath) => new FileLoader<T>(filePath);
    }
}