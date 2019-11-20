using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
        Action Action { get; set; }
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
        void ICollection<ITheraMenuItem>.Clear() 
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
        void IList<ITheraMenuItem>.RemoveAt(int index)
            => _items.RemoveAt(index);

        public IEnumerator GetEnumerator()
            => _items.GetEnumerator();

        public override void OnOpening() => _items.ForEach(x => x.OnOpening());
        public override void OnClosing() => _items.ForEach(x => x.OnClosing());

        /// <summary>
        /// Returns a new menu with default menu options.
        /// 0:Rename, 1:Explorer, 2:Edit, 3:Edit Raw, 4:Divider, 5:Cut, 6:Copy, 7:Paste, 8:Delete
        /// </summary>
        public static TMenu Default() =>
            new TMenu()
            {
                TMenuOption.Rename,
                TMenuOption.Explorer,
                TMenuOption.Edit,
                TMenuOption.EditRaw,
                TMenuDivider.Instance,
                TMenuOption.Cut,
                TMenuOption.Copy,
                TMenuOption.Paste,
                TMenuOption.Delete,
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
        
        public static TMenuOption Edit { get; } = new TMenuOptionEdit();
        public static TMenuOption EditRaw { get; } = new TMenuOptionEditRaw();
        public static TMenuOption Rename { get; } = new TMenuOptionRename();
        public static TMenuOption Explorer { get; } = new TMenuOptionExplorer();
        public static TMenuOption Cut { get; } = new TMenuOptionCut();
        public static TMenuOption Copy { get; } = new TMenuOptionCopy();
        public static TMenuOption Paste { get; } = new TMenuOptionPaste();
        public static TMenuOption Delete { get; } = new TMenuOptionDelete();

        private sealed class TMenuOptionRename : TMenuOption
        {
            public static TMenuOptionRename Instance { get; } = new TMenuOptionRename();
            internal TMenuOptionRename() : base("Rename", null, Keys.F2) { }
        }
        public sealed class TMenuOptionExplorer : TMenuOption
        {
            public static TMenuOptionExplorer Instance { get; } = new TMenuOptionExplorer();
            internal TMenuOptionExplorer() : base("View In Explorer", null, Keys.Control | Keys.E) { }
        }
        public sealed class TMenuOptionEdit : TMenuOption
        {
            public static TMenuOptionEdit Instance { get; } = new TMenuOptionEdit();
            internal TMenuOptionEdit() : base("Edit", null, Keys.F4) { }
        }
        public sealed class TMenuOptionEditRaw : TMenuOption
        {
            public static TMenuOptionEditRaw Instance { get; } = new TMenuOptionEditRaw();
            internal TMenuOptionEditRaw() : base("Edit Raw", null, Keys.F3) { }
        }
        public sealed class TMenuOptionCut : TMenuOption
        {
            public static TMenuOptionCut Instance { get; } = new TMenuOptionCut();
            internal TMenuOptionCut() : base("Cut", null, Keys.Control | Keys.X) { }
        }
        public sealed class TMenuOptionCopy : TMenuOption
        {
            public static TMenuOptionCopy Instance { get; } = new TMenuOptionCopy();
            internal TMenuOptionCopy() : base("Copy", null, Keys.Control | Keys.C) { }
        }
        public sealed class TMenuOptionPaste : TMenuOption
        {
            public static TMenuOptionPaste Instance { get; } = new TMenuOptionPaste();
            internal TMenuOptionPaste() : base("Paste", null, Keys.Control | Keys.V) { }
        }
        public sealed class TMenuOptionDelete : TMenuOption
        {
            public static TMenuOptionDelete Instance { get; } = new TMenuOptionDelete();
            internal TMenuOptionDelete() : base("Delete", null, Keys.Delete) { }
        }
    }
}
