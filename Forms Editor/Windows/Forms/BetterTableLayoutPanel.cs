using Core.Win32.Native;
using System;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public class BetterTableLayoutPanel : TableLayoutPanel
    {
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.CacheText, true);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= NativeConstants.WS_EX_COMPOSITED;
                return cp;
            }
        }

        public void BeginUpdate()
        {
            NativeMethods.SendMessage(Handle, NativeConstants.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }

        public void EndUpdate()
        {
            NativeMethods.SendMessage(Handle, NativeConstants.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            Parent.Invalidate(true);
        }
    }
}
