using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public interface ITheraMenuItem : IObjectSlim
    {

    }
    public interface ITheraMenuDivider : ITheraMenuItem
    {

    }
    public interface ITheraMenuOption : ITheraMenuItem
    {
        string Text { get; set; }
        Keys HotKeys { get; set; }
        string ActionName { get; set; }
    }
    public interface ITheraMenu : IListProxy<ITheraMenuItem>
    {

    }
    public class TheraMenu : ListProxy<TheraMenuItem>, ITheraMenu
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
    }
    public class TheraMenuItem : TObjectSlim, ITheraMenuItem
    {

    }
    public class TheraMenuDivider : TheraMenuItem, ITheraMenuDivider
    {

    }
    public class TheraMenuOption : TheraMenuItem, ITheraMenuOption
    {
        public TheraMenuOption(string text, string actionName, Keys hotKeys)
        {
            Text = text;
            HotKeys = hotKeys;
            ActionName = actionName;
        }

        public string Text { get; set; }
        public Keys HotKeys { get; set; }
        public string ActionName { get; set; } 
    }
}
