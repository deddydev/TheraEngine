using System;
using System.Windows.Forms;
using TheraEditor.ContentTree.Core;
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
        ITMenu Menu { get; }
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

        public ITMenu Menu { get; protected set; }
        public virtual string FilePath { get; set; }

        /// <summary>
        /// Returns a new menu with default menu options.
        /// 0:Rename, 1:Explorer, 2:Divider, 3:Edit, 4:Edit Raw, 5:Divider, 6:Cut, 7:Copy, 8:Paste, 9:Divider, 10:Delete
        /// </summary>
        public static ITMenu DefaultMenu(IBaseFileWrapper wrapper) =>
            new TMenu()
            {
                RenameMenuOption(wrapper),
                ExplorerMenuOption(wrapper),
                TMenuDivider.Instance,
                EditMenuOption(wrapper),
                EditRawMenuOption(wrapper),
                TMenuDivider.Instance,
                CutMenuOption(wrapper),
                CopyMenuOption(wrapper),
                PasteMenuOption(wrapper),
                TMenuDivider.Instance,
                DeleteMenuOption(wrapper),
            };

        public static TMenuOption EditMenuOption(IBaseFileWrapper wrapper) => new TMenuOptionEdit(wrapper);
        public static TMenuOption EditRawMenuOption(IBaseFileWrapper wrapper) => new TMenuOptionEditRaw(wrapper);
        public static TMenuOption RenameMenuOption(IBasePathWrapper wrapper) => new TMenuOptionRename(wrapper);
        public static TMenuOption ExplorerMenuOption(IBasePathWrapper wrapper) => new TMenuOptionExplorer(wrapper);
        public static TMenuOption CutMenuOption(IBasePathWrapper wrapper) => new TMenuOptionCut(wrapper);
        public static TMenuOption CopyMenuOption(IBasePathWrapper wrapper) => new TMenuOptionCopy(wrapper);
        public static TMenuOption PasteMenuOption(IBasePathWrapper wrapper) => new TMenuOptionPaste(wrapper);
        public static TMenuOption DeleteMenuOption(IBasePathWrapper wrapper) => new TMenuOptionDelete(wrapper);

        private sealed class TMenuOptionRename : TMenuOption
        {
            internal TMenuOptionRename(IBasePathWrapper wrapper) : base("Rename", wrapper.Rename, Keys.F2) { }
        }
        public sealed class TMenuOptionExplorer : TMenuOption
        {
            internal TMenuOptionExplorer(IBasePathWrapper wrapper) : base("View In Explorer", wrapper.Explorer, Keys.Control | Keys.E) { }
        }
        public sealed class TMenuOptionEdit : TMenuOption
        {
            internal TMenuOptionEdit(IBaseFileWrapper wrapper) : base("Edit", wrapper.Edit, Keys.F4) { }
        }
        public sealed class TMenuOptionEditRaw : TMenuOption
        {
            internal TMenuOptionEditRaw(IBaseFileWrapper wrapper) : base("Edit Raw", wrapper.EditRaw, Keys.F3) { }
        }
        public sealed class TMenuOptionCut : TMenuOption
        {
            internal TMenuOptionCut(IBasePathWrapper wrapper) : base("Cut", wrapper.Cut, Keys.Control | Keys.X) { }
        }
        public sealed class TMenuOptionCopy : TMenuOption
        {
            internal TMenuOptionCopy(IBasePathWrapper wrapper) : base("Copy", wrapper.Copy, Keys.Control | Keys.C) { }
        }
        public sealed class TMenuOptionPaste : TMenuOption
        {
            internal TMenuOptionPaste(IBasePathWrapper wrapper) : base("Paste", wrapper.Paste, Keys.Control | Keys.V) { }
        }
        public sealed class TMenuOptionDelete : TMenuOption
        {
            internal TMenuOptionDelete(IBasePathWrapper wrapper) : base("Delete", wrapper.Delete, Keys.Delete) { }
        }
    }
}
