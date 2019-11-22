using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public interface ITheraMenuItem : IObjectSlim 
    {
        void OnOpening();
        void OnClosing();
    }
    public interface ITheraMenuDivider : ITheraMenuItem { }
    public interface ITheraMenuOption : ITheraMenuItem, ITheraMenu
    {
        string Text { get; set; }
        Keys HotKeys { get; set; }
        void ExecuteAction();
    }
    public interface ITheraMenu : IListProxy<ITheraMenuItem>
    {
        void OnOpening();
        void OnClosing();
    }
    public class TMenu : TheraMenuItem, IListProxy<ITheraMenuItem>, ITheraMenu
    {
        private ListProxy<ITheraMenuItem> _items = new ListProxy<ITheraMenuItem>();

        ITheraMenuItem IList<ITheraMenuItem>.this[int index] 
        {
            get => _items[index];
            set => _items[index] = (TheraMenuItem)value;
        }

        int ICollection<ITheraMenuItem>.Count => _items.Count;
        bool ICollection<ITheraMenuItem>.IsReadOnly => _items.IsReadOnly;

        public void Add(ITheraMenuItem item)
            => _items.Add(item);

        void ICollection<ITheraMenuItem>.Add(ITheraMenuItem item)
            => _items.Add((TheraMenuItem)item);
        public void Clear() 
            => _items.Clear();
        bool ICollection<ITheraMenuItem>.Contains(ITheraMenuItem item)
            => _items.Contains((TheraMenuItem)item);
        void ICollection<ITheraMenuItem>.CopyTo(ITheraMenuItem[] array, int arrayIndex)
            => _items.CopyTo(array.Select(x => (TheraMenuItem)x).ToArray(), arrayIndex);
        IEnumerator<ITheraMenuItem> IEnumerable<ITheraMenuItem>.GetEnumerator()
            => _items.GetEnumerator();
        int IList<ITheraMenuItem>.IndexOf(ITheraMenuItem item)
            => _items.IndexOf((TheraMenuItem)item);
        void IList<ITheraMenuItem>.Insert(int index, ITheraMenuItem item)
            => _items.Insert(index, (TheraMenuItem)item);
        bool ICollection<ITheraMenuItem>.Remove(ITheraMenuItem item)
            => _items.Remove((TheraMenuItem)item);
        public void RemoveAt(int index)
            => _items.RemoveAt(index);

        public IEnumerator GetEnumerator()
            => _items.GetEnumerator();

        public override void OnOpening() => _items.ForEach(x => x.OnOpening());
        public override void OnClosing() => _items.ForEach(x => x.OnClosing());

        /// <summary>
        /// Returns a new menu with default menu options.
        /// 0:Rename, 1:Explorer, 2:Divider, 3:Edit, 4:Edit Raw, 5:Divider, 6:Cut, 7:Copy, 8:Paste, 9:Divider, 10:Delete
        /// </summary>
        public static TMenu Default(IBaseFileWrapper wrapper) =>
            new TMenu()
            {
                TMenuOption.Rename(wrapper),
                TMenuOption.Explorer(wrapper),
                TMenuDivider.Instance,
                TMenuOption.Edit(wrapper),
                TMenuOption.EditRaw(wrapper),
                TMenuDivider.Instance,
                TMenuOption.Cut(wrapper),
                TMenuOption.Copy(wrapper),
                TMenuOption.Paste(wrapper),
                TMenuDivider.Instance,
                TMenuOption.Delete(wrapper),
            };
    }
    public class TheraMenuItem : TObjectSlim, ITheraMenuItem
    {
        public bool Visible { get; set; } = true;

        public event Action<TheraMenuItem> Opening;
        public event Action<TheraMenuItem> Closing;

        public virtual void OnClosing() => Closing?.Invoke(this);
        public virtual void OnOpening() => Opening?.Invoke(this);
    }
    public sealed class TMenuDivider : TheraMenuItem, ITheraMenuDivider
    {
        public static TMenuDivider Instance { get; } = new TMenuDivider();
        private TMenuDivider() : base() { }
    }
    public class TMenuOption : TMenu, ITheraMenuOption
    {
        public TMenuOption(string text, Action action, Keys hotKeys)
        {
            Text = text;
            HotKeys = hotKeys;
            Action = action;
        }

        public string Text { get; set; }
        public Keys HotKeys { get; set; }
        public Action Action { get; set; }

        public void ExecuteAction()
        {
            Action?.Invoke();
        }

        public static TMenuOption Edit(IBaseFileWrapper wrapper) => new TMenuOptionEdit(wrapper);
        public static TMenuOption EditRaw(IBaseFileWrapper wrapper) => new TMenuOptionEditRaw(wrapper);
        public static TMenuOption Rename(IBasePathWrapper wrapper) => new TMenuOptionRename(wrapper);
        public static TMenuOption Explorer(IBasePathWrapper wrapper) => new TMenuOptionExplorer(wrapper);
        public static TMenuOption Cut(IBasePathWrapper wrapper) => new TMenuOptionCut(wrapper);
        public static TMenuOption Copy(IBasePathWrapper wrapper) => new TMenuOptionCopy(wrapper);
        public static TMenuOption Paste(IBasePathWrapper wrapper) => new TMenuOptionPaste(wrapper);
        public static TMenuOption Delete(IBasePathWrapper wrapper) => new TMenuOptionDelete(wrapper);

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
