using System;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public partial class FloatingForm : Form
    {
        public event EventHandler DockingRequested;

        private const int WM_NCLBUTTONDBLCLK = 0xA3;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCLBUTTONDBLCLK:
                    Fire_DockingRequested();
                    break;
            }
            base.WndProc(ref m);
        }

        private void Fire_DockingRequested() => DockingRequested?.Invoke(this, new EventArgs());

        public FloatingForm() => InitializeComponent();
    }
}