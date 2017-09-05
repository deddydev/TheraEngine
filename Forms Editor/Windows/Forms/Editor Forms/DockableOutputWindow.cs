using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Engine.DebugOutput += Engine_DebugOutput;
        }

        private void Engine_DebugOutput(string message)
        {
            OutputTextBox.Text += message;
        }
    }
}
