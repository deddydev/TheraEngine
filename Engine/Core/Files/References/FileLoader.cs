using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Files
{
    public interface IFileLoader
    {
        string ReferencePath { get; set; }
        Type ReferencedType { get; }
    }
    /// <summary>
    /// Indicates that this variable references a file that must be loaded.
    /// </summary>
    [FileExt("ldr")]
    [FileDef("File Loader")]
    public class FileLoader<T> : FileObject, IFileLoader where T : FileObject
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
            ReferencePath = filePath;
        }
        public FileLoader(string filePath, Type type)
        {
            if (typeof(T).IsAssignableFrom(type))
                _subType = type;
            else
                throw new Exception(type.ToString() + " is not assignable to " + typeof(T).ToString());
            //if (Path.HasExtension(filePath) && FileManager.GetTypeWithExtension(Path.GetExtension(filePath)) != _subType)
            //    throw new InvalidOperationException("Extension does not match type");
            ReferencePath = filePath;
        }
        public FileLoader(string dir, string name, ProprietaryFileFormat format) : this(GetFilePath(dir, name, format, typeof(T))) { }
        #endregion
        
        protected string _localPath = null;
        protected string _absolutePath = null;
        protected Type _subType = null;

        [Category("File Reference")]
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public virtual string ReferencePath
        {
            get
            {
                //if (string.IsNullOrEmpty(_absolutePath) && Engine.Settings != null && !string.IsNullOrEmpty(Engine.Game.FilePath) && !string.IsNullOrEmpty(_localPath))
                //    _absolutePath = Engine.Game.FilePath + _localPath;
                return _absolutePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _localPath = value.MakePathRelativeTo(Application.StartupPath);
                    _absolutePath = Path.GetFullPath(Application.StartupPath + _localPath);
                }
                else
                {
                    _absolutePath = _localPath = "";
                }
            }
        }

        [Browsable(false)]
        public Type ReferencedType => typeof(T);
        
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
        protected virtual T LoadNewInstance(bool allowConstruct = true, params object[] constructionArgs)
        {
            string absolutePath = ReferencePath;

            if (string.IsNullOrWhiteSpace(absolutePath))
            {
                if (allowConstruct)
                {
                    T file = CreateNewInstance_Internal(false, constructionArgs);
                    if (file != null)
                    {
                        file.FilePath = absolutePath;
                        OnFileLoaded(file);
                        file.OnLoaded();
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
                if (IsThirdPartyFormat())
                {
                    T file = Activator.CreateInstance<T>();
                    file.FilePath = absolutePath;
                    file.Read3rdParty(absolutePath);
                    OnFileLoaded(file);
                    file.OnLoaded();
                    Loaded?.Invoke(file);
                    return file;
                }
                else
                {
                    T file = null;
                    switch (GetFormat())
                    {
                        case FileFormat.XML:
                            file = FromXML(_subType, absolutePath) as T;
                            break;
                        case FileFormat.Binary:
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
        public string Extension() => ReferencePath == null ? null :
            Path.GetExtension(ReferencePath).ToLowerInvariant().Substring(1);

        /// <summary>
        /// Retrieves the proprietary file format type from the extension.
        /// Does not account for extensions that are 3rd party.
        /// </summary>
        public FileFormat GetFormat()
        {
            string ext = Extension();
            if (!string.IsNullOrEmpty(ext))
            {
                switch (ext.ToLowerInvariant()[0])
                {
                    case 'x': return FileFormat.XML;
                    case 'b': return FileFormat.Binary;
                }
            }
            return FileFormat.Binary;
        }

        public void LoadNewInstanceAsync(Action<T> onLoaded, TaskCreationOptions options = TaskCreationOptions.PreferFairness)
            => LoadNewInstanceAsync(options).ContinueWith(task => onLoaded(task.Result));
        public void LoadNewInstanceAsync(Action<T> onLoaded)
            => LoadNewInstanceAsync().ContinueWith(task => onLoaded(task.Result));
        public async Task<T> LoadNewInstanceAsync() => await Task.Run(() => LoadNewInstance());
        public async Task<T> LoadNewInstanceAsync(TaskCreationOptions options) => await Task.Factory.StartNew(() => LoadNewInstance(), options);

        public T CreateNewInstance(params object[] args) => CreateNewInstance_Internal(true, args);
        protected T CreateNewInstance_Internal(bool callLoadedEvent, params object[] args)
        {
            if (_subType.IsAbstract)
            {
                //Engine.LogWarning("Can't automatically instantiate an abstract class: " + _subType.GetFriendlyName());
                return null;
            }
            else if (_subType.IsInterface)
            {
                //Engine.LogWarning("Can't automatically instantiate an interface: " + _subType.GetFriendlyName());
                return null;
            }
            else
            {
                var types = args.Select(x => x.GetType()).ToArray();
                if (_subType.GetConstructor(types) == null)
                {
                    Engine.LogWarning("Can't automatically instantiate '" + _subType.GetFriendlyName() + "' with " + (types.Length == 0 ?
                        "no parameters." :
                        "these parameters: " + string.Join(", ", types.Select(x => x.GetFriendlyName()))));
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
        private bool IsThirdPartyFormat()
        {
            File3rdParty header = GetFile3rdPartyExtensions(_subType);
            return header?.HasAnyExtensions ?? false;
        }
        public override string ToString() => ReferencePath;
        
        public static implicit operator T(FileLoader<T> fileRef) => fileRef?.CreateNewInstance();
        public static implicit operator FileLoader<T>(string filePath) => new FileLoader<T>(filePath);
    }
}
