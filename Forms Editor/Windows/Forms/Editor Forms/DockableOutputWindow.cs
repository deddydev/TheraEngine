﻿using System;
using TheraEngine;
using TheraEngine.Scripting;
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
                    Invoke(new Action<string>(Engine_DebugOutput), message);
                    return;
                }

                OutputTextBox.Text += message;

                //Don't scroll to end if user is reading something
                if (!OutputTextBox.Focused)
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
}
