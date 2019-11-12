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

        void EditResource();
    }
    public abstract class BaseFileWrapper : TObjectSlim, IBaseFileWrapper
    {
        public abstract TypeProxy FileType { get; }
        public virtual string FilePath { get; set; }
        public IListProxy<ITheraMenuItem> MenuItems { get; set; }
        public abstract bool IsLoaded { get; }

        public abstract void EditResource();
    }
}
