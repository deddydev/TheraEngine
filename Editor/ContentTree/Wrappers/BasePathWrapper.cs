using System;
using TheraEngine;

namespace TheraEditor.Wrappers
{
    public interface IBasePathWrapper : IObjectSlim
    {
        event Action Rename;
        event Action Explorer;
        event Action Cut;
        event Action Copy;
        event Action Paste;
        event Action Delete;

        string FilePath { get; set; }
        ITheraMenu Menu { get; }
    }
    public abstract class BasePathWrapper : TObjectSlim, IBasePathWrapper
    {
        public event Action FilePathChanged;
        public event Action Rename;
        public event Action Explorer;
        public event Action Cut;
        public event Action Copy;
        public event Action Paste;
        public event Action Delete;
        
        protected void OnFilePathChanged() => FilePathChanged?.Invoke();
        protected void OnRename() => Rename?.Invoke();
        protected void OnExplorer() => Explorer?.Invoke();
        protected void OnCut() => Cut?.Invoke();
        protected void OnCopy() => Copy?.Invoke();
        protected void OnPaste() => Paste?.Invoke();
        protected void OnDelete() => Delete?.Invoke();

        public ITheraMenu Menu { get; protected set; }
        public virtual string FilePath { get; set; }
    }
}
