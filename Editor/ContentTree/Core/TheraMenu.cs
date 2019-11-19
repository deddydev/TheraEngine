using System;
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
    public interface ITheraMenuOption : ITheraMenuItem
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
    public class TMenu : ListProxy<TheraMenuItem>, ITheraMenu
    {
        ITheraMenuItem IList<ITheraMenuItem>.this[int index] 
        {
            get => this[index];
            set => this[index] = (TheraMenuItem)value;
        }

        int ICollection<ITheraMenuItem>.Count => Count;
        bool ICollection<ITheraMenuItem>.IsReadOnly => IsReadOnly;

        void ICollection<ITheraMenuItem>.Add(ITheraMenuItem item)
            => Add((TheraMenuItem)item);
        void ICollection<ITheraMenuItem>.Clear() 
            => Clear();
        bool ICollection<ITheraMenuItem>.Contains(ITheraMenuItem item)
            => Contains((TheraMenuItem)item);
        void ICollection<ITheraMenuItem>.CopyTo(ITheraMenuItem[] array, int arrayIndex)
            => CopyTo(array.Select(x => (TheraMenuItem)x).ToArray(), arrayIndex);
        IEnumerator<ITheraMenuItem> IEnumerable<ITheraMenuItem>.GetEnumerator()
            => GetEnumerator();
        int IList<ITheraMenuItem>.IndexOf(ITheraMenuItem item)
            => IndexOf((TheraMenuItem)item);
        void IList<ITheraMenuItem>.Insert(int index, ITheraMenuItem item)
            => Insert(index, (TheraMenuItem)item);
        bool ICollection<ITheraMenuItem>.Remove(ITheraMenuItem item)
            => Remove((TheraMenuItem)item);
        void IList<ITheraMenuItem>.RemoveAt(int index)
            => RemoveAt(index);

        public void OnOpening() => ForEach(x => x.OnOpening());
        public void OnClosing() => ForEach(x => x.OnClosing());
    }
    public class TheraMenuItem : TObjectSlim, ITheraMenuItem
    {
        public bool Visible { get; set; } = true;

        public event Action<TheraMenuItem> Opening;
        public event Action<TheraMenuItem> Closing;

        public void OnClosing() => Closing?.Invoke(this);
        public void OnOpening() => Opening?.Invoke(this);
    }
    public sealed class TMenuDivider : TheraMenuItem, ITheraMenuDivider
    {
        public static TMenuDivider Instance { get; } = new TMenuDivider();
        private TMenuDivider() : base() { }
    }
    public class TMenuOption : TheraMenuItem, ITheraMenuOption
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
