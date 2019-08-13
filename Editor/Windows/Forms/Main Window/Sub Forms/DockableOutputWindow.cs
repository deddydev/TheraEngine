using System;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Reflection;
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

            OutputTextBox.LostFocus += OutputTextBox_LostFocus;
            Engine.Instance.DebugOutput += QueueMessage;
            Application.Idle += Application_Idle;

            OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
            OutputTextBox.ScrollToCaret();
        }

        private void OutputTextBox_LostFocus(object sender, EventArgs e)
        {
            OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
            OutputTextBox.ScrollToCaret();
        }

        private string _queuedMessage = string.Empty;
        private void Application_Idle(object sender, EventArgs e)
        {
            if (_queuedMessage.Length > 0)
            {
                DisplayMessage(_queuedMessage);
                _queuedMessage = string.Empty;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Engine.Instance.DebugOutput -= QueueMessage;
            OutputTextBox.LostFocus -= OutputTextBox_LostFocus;
            Application.Idle -= Application_Idle;

            base.OnHandleDestroyed(e);
        }
        private void QueueMessage(string message)
        {
            _queuedMessage += message;
        }
        private void DisplayMessage(string message)
        {
            try
            {
                if (Disposing || IsDisposed)
                    return;

                if (InvokeRequired)
                {
                    BeginInvoke(new Action<string>(DisplayMessage), message);
                    return;
                }

                bool shouldScroll = OutputTextBox.SelectionStart == OutputTextBox.Text.Length;

                OutputTextBox.Text += message;

                Form activeForm = ActiveForm ?? Application.OpenForms[Application.OpenForms.Count - 1];
                if (activeForm is DockContent c && c.DockPanel != null && c.DockPanel != DockPanel)
                {
                    Show(c.DockPanel);
                    shouldScroll = true;
                }
                else if (activeForm is IDockPanelOwner dockable && dockable.DockPanelRef != DockPanel)
                {
                    Show(dockable.DockPanelRef);
                    shouldScroll = true;
                }

                if (shouldScroll)
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
