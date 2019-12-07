using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Reflection;

namespace TheraEditor.ContentTree.Core
{
    public interface ITMenuItem : IObjectSlim 
    {
        bool Visible { get; set; }
        bool Enabled { get; set; }
        
        void OnOpening();
        void OnClosing();
    }
    public interface ITMenuDivider : ITMenuItem { }
    public interface ITMenuOption : ITMenuItem, ITMenu
    {
        string Text { get; set; }
        Keys HotKeys { get; set; }
        void ExecuteAction();
    }
    public interface ITMenu : IListProxy<ITMenuItem>
    {
        event Action ChildrenCleared;
        event Action<ITMenuItem> ChildAdded;

        void OnOpening();
        void OnClosing();
    }
    public class TMenu : TMenuItem, IListProxy<ITMenuItem>, ITMenu
    {
        public event Action ChildrenCleared;
        public event Action<ITMenuItem> ChildAdded;

        private readonly ListProxy<ITMenuItem> _items = new ListProxy<ITMenuItem>();

        ITMenuItem IList<ITMenuItem>.this[int index] 
        {
            get => _items[index];
            set => _items[index] = (TMenuItem)value;
        }

        int ICollection<ITMenuItem>.Count => _items.Count;
        bool ICollection<ITMenuItem>.IsReadOnly => _items.IsReadOnly;

        public void Add(ITMenuItem item)
        {
            _items.Add(item);
            ChildAdded?.Invoke(item);
        }
        public void Clear()
        {
            _items.Clear();
            ChildrenCleared?.Invoke();
        }

        void ICollection<ITMenuItem>.Add(ITMenuItem item)
            => _items.Add((TMenuItem)item);
        bool ICollection<ITMenuItem>.Contains(ITMenuItem item)
            => _items.Contains((TMenuItem)item);
        void ICollection<ITMenuItem>.CopyTo(ITMenuItem[] array, int arrayIndex)
            => _items.CopyTo(array.Select(x => (TMenuItem)x).ToArray(), arrayIndex);
        IEnumerator<ITMenuItem> IEnumerable<ITMenuItem>.GetEnumerator()
            => _items.GetEnumerator();
        int IList<ITMenuItem>.IndexOf(ITMenuItem item)
            => _items.IndexOf((TMenuItem)item);
        void IList<ITMenuItem>.Insert(int index, ITMenuItem item)
            => _items.Insert(index, (TMenuItem)item);
        bool ICollection<ITMenuItem>.Remove(ITMenuItem item)
            => _items.Remove((TMenuItem)item);
        public void RemoveAt(int index)
            => _items.RemoveAt(index);

        public IEnumerator GetEnumerator()
            => _items.GetEnumerator();

        public override void OnOpening() => _items.ForEach(x => x.OnOpening());
        public override void OnClosing() => _items.ForEach(x => x.OnClosing());
    }
    public class TMenuItem : TObjectSlim, ITMenuItem
    {
        private bool _enabled = true;
        private bool _visible = true;

        public bool Visible 
        {
            get => _visible;
            set
            {
                if (_visible == value)
                    return;

                _visible = value;
                VisibilityChanged?.Invoke(this);
            }
        }
        public bool Enabled 
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;

                _enabled = value;
                EnabledChanged?.Invoke(this);
            }
        }

        public event Action<ITMenuItem> VisibilityChanged;
        public event Action<ITMenuItem> EnabledChanged;
        public event Action<ITMenuItem> Opening;
        public event Action<ITMenuItem> Closing;

        public virtual void OnClosing() => Closing?.Invoke(this);
        public virtual void OnOpening() => Opening?.Invoke(this);
    }
    public sealed class TMenuDivider : TMenuItem, ITMenuDivider
    {
        public static TMenuDivider Instance { get; } = new TMenuDivider();
        private TMenuDivider() : base() { }
    }
    public class TMenuOption : TMenu, ITMenuOption
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
    }
}
