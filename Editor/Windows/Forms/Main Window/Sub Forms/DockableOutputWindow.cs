using System;
using System.Windows.Forms;
using TheraEngine;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableOutputWindow : DockContent
    {
        public DockableOutputWindow()
        {
            InitializeComponent();
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            OutputTextBox.Text = Engine.OutputString;
            Engine.DebugOutput += Engine_DebugOutput;
            if (!OutputTextBox.Focused)
            {
                OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                OutputTextBox.ScrollToCaret();
            }
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            Engine.DebugOutput -= Engine_DebugOutput;
            base.OnHandleDestroyed(e);
        }
        
        private void Engine_DebugOutput(string message)
        {
            try
            {
                if (Disposing || IsDisposed)
                    return;

                if (InvokeRequired)
                {
                    BeginInvoke(new Action<string>(Engine_DebugOutput), message);
                    return;
                }

                OutputTextBox.Text += message;

                Form activeForm = ActiveForm ?? Application.OpenForms[Application.OpenForms.Count - 1];
                if (activeForm is DockContent c && c.DockPanel != null && c.DockPanel != DockPanel)
                {
                    Show(c.DockPanel);
                    OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                    OutputTextBox.ScrollToCaret();
                }
                else if (activeForm is IDockPanelOwner dockable && dockable.DockPanelRef != DockPanel)
                {
                    Show(dockable.DockPanelRef);
                    OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                    OutputTextBox.ScrollToCaret();
                }
                else if (!OutputTextBox.Focused) //Don't scroll to end if user is reading something
                {
                    OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                    OutputTextBox.ScrollToCaret();
                }
            }
            catch
            {

            }
        }
    }

    public interface IDockPanelOwner
    {
        DockPanel DockPanelRef { get; }
    }
}
