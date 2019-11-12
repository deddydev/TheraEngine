using System.Windows.Forms;
using TheraEngine;

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
    public class TheraMenuItem : TObjectSlim, ITheraMenuItem
    {

    }
    public class TheraMenuDivider : TheraMenuItem, ITheraMenuDivider
    {

    }
    public class TheraMenuOption : TheraMenuItem, ITheraMenuOption
    {
        public TheraMenuOption(string text, Keys hotKeys, string actionName)
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
