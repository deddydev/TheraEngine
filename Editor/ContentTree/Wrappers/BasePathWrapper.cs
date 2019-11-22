using System;
using TheraEngine;

namespace TheraEditor.Wrappers
{
    public interface IBasePathWrapper : IObjectSlim
    {
        event Action RenameEvent;
        event Action CutEvent;
        event Action CopyEvent;
        event Action PasteEvent;
        event Action DeleteEvent;

        void Rename();
        void Cut();
        void Copy();
        void Paste();
        void Delete();
        void Explorer();

        string FilePath { get; set; }
        ITheraMenu Menu { get; }
    }
    public abstract class BasePathWrapper : TObjectSlim, IBasePathWrapper
    {
        public event Action RenameEvent;
        public event Action CutEvent;
        public event Action CopyEvent;
        public event Action PasteEvent;
        public event Action DeleteEvent;
        
        public void Rename() => RenameEvent?.Invoke();
        public void Cut() => CutEvent?.Invoke();
        public void Copy() => CopyEvent?.Invoke();
        public void Paste() => PasteEvent?.Invoke();
        public void Delete() => DeleteEvent?.Invoke();

        public abstract void Explorer();

        public ITheraMenu Menu { get; protected set; }
        public virtual string FilePath { get; set; }
    }
}
