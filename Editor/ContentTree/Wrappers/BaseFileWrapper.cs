using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public interface IBaseFileWrapper : IObjectSlim
    {
        TypeProxy FileType { get; }
        string FilePath { get; set; }
        IListProxy<ITheraMenuItem> MenuItems { get; set; }
        bool IsLoaded { get; }
        IFileObject File { get; }

        void Edit();
    }
    public abstract class BaseFileWrapper : TObjectSlim, IBaseFileWrapper
    {
        public event Action FilePathChanged;
        protected void OnFilePathChanged() => FilePathChanged?.Invoke();

        public TheraMenu Menu { get; set; }
        public abstract TypeProxy FileType { get; }
        public virtual string FilePath { get; set; }
        public IListProxy<ITheraMenuItem> MenuItems { get; set; }
        public abstract bool IsLoaded { get; }
        public bool AlwaysReload { get; set; } = false;
        public bool ExternallyModified { get; set; } = false;
        IFileObject IBaseFileWrapper.File => GetFile();

        public abstract IFileObject GetFile();
        public abstract void Edit();
    }
}
