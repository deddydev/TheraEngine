﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace BorderlessForm
{
    /// <summary>
    /// Based on http://customerborderform.codeplex.com/
    /// </summary>
    public class BorderlessFormBase : Form
    {
        public void DecorationMouseDown(HitTestValues hit, Point p)
        {
            NativeMethods.ReleaseCapture();
            var pt = new POINTS { X = (short)p.X, Y = (short)p.Y };
            NativeMethods.SendMessage(FindForm().Handle, (int)WindowMessages.WM_NCLBUTTONDOWN, (int)hit, pt);
        }

        public void DecorationMouseDown(HitTestValues hit)
        {
            DecorationMouseDown(hit, MousePosition);
        }

        public void DecorationMouseUp(HitTestValues hit, Point p)
        {
            NativeMethods.ReleaseCapture();
            var pt = new POINTS { X = (short)p.X, Y = (short)p.Y };
            NativeMethods.SendMessage(FindForm().Handle, (int)WindowMessages.WM_NCLBUTTONUP, (int)hit, pt);
        }

        public void DecorationMouseUp(HitTestValues hit)
        {
            DecorationMouseUp(hit, MousePosition);
        }
        
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!DesignMode)
                SetWindowRegion(Handle, 0, 0, Width, Height);
        }

        protected void ShowSystemMenu(MouseButtons buttons)
        {
            ShowSystemMenu(buttons, MousePosition);
        }

        protected static int MakeLong(short lowPart, short highPart)
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }
        
        protected void ShowSystemMenu(MouseButtons buttons, Point pos)
        {
            NativeMethods.SendMessage(Handle, (int)WindowMessages.WM_SYSMENU, 0, MakeLong((short)pos.X, (short)pos.Y));
        }
        
        protected override void WndProc(ref Message m)
        {
            if (DesignMode)
            {
                base.WndProc(ref m);
                return;
            }

            switch (m.Msg)
            {
                case (int)WindowMessages.WM_NCCALCSIZE:
                    {
                        // Provides new coordinates for the window client area.
                        WmNCCalcSize(ref m);
                        break;
                    }
                case (int)WindowMessages.WM_NCPAINT:
                    {
                        // Here should all our painting occur, but...
                        WmNCPaint(ref m);
                        break;
                    }
                case (int)WindowMessages.WM_NCACTIVATE:
                    {
                        // ... WM_NCACTIVATE does some painting directly 
                        // without bothering with WM_NCPAINT ...
                        WmNCActivate(ref m);
                        break;
                    }
                case (int)WindowMessages.WM_SETTEXT:
                    {
                        // ... and some painting is required in here as well
                        WmSetText(ref m);
                        break;
                    }
                case (int)WindowMessages.WM_WINDOWPOSCHANGED:
                    {
                        WmWindowPosChanged(ref m);
                        break;
                    }
                case 174: // ignore magic message number
                    {
                        break;
                    }
                default:
                    {
                        base.WndProc(ref m);
                        break;
                    }
            }
        }

        private void SetWindowRegion(IntPtr hwnd, int left, int top, int right, int bottom)
        {
            var hrg = new HandleRef(this, NativeMethods.CreateRectRgn(0, 0, 0, 0));
            NativeMethods.GetWindowRgn(hwnd, hrg.Handle);
            NativeMethods.GetRgnBox(hrg.Handle, out RECT box);
            if (box.left != left || box.top != top || box.right != right || box.bottom != bottom)
            {
                var hr = new HandleRef(FindForm(), NativeMethods.CreateRectRgn(left, top, right, bottom));
                NativeMethods.SetWindowRgn(hwnd, hr.Handle, NativeMethods.IsWindowVisible(hwnd));
            }
        }

        [Browsable(false)]
        public FormWindowState MinMaxState
        {
            get
            {
                var s = NativeMethods.GetWindowLong(Handle, NativeConstants.GWL_STYLE);
                var max = (s & (int)WindowStyle.WS_MAXIMIZE) > 0;
                if (max) return FormWindowState.Maximized;
                var min = (s & (int)WindowStyle.WS_MINIMIZE) > 0;
                if (min) return FormWindowState.Minimized;
                return FormWindowState.Normal;
            }
        }

        private void WmWindowPosChanged(ref Message m)
        {
            DefWndProc(ref m);
            UpdateBounds();
            var pos = (WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(WINDOWPOS));
            SetWindowRegion(FindForm().Handle, 0, 0, pos.cx, pos.cy);
            m.Result = NativeConstants.TRUE;
        }

        //
        // Summary:
        //     Gets or sets a value that indicates whether form is minimized, maximized, or
        //     normal.
        //
        // Returns:
        //     A System.Windows.Forms.FormWindowState that represents whether form is minimized,
        //     maximized, or normal. The default is FormWindowState.Normal.
        //
        // Exceptions:
        //   T:System.ComponentModel.InvalidEnumArgumentException:
        //     The value specified is outside the range of valid values.
        [DefaultValue(FormWindowState.Normal)]
        public new FormWindowState WindowState
        {
            get => base.WindowState;
            set
            {
                if (value == WindowState)
                    return;
                if (value != FormWindowState.Normal)
                {
                    IntPtr handle = FindForm().Handle;
                    AnimateWindowFlags slideYFlags = AnimateWindowFlags.AW_SLIDE | AnimateWindowFlags.AW_VER_POSITIVE;
                    AnimateWindowFlags slideXFlags = AnimateWindowFlags.AW_SLIDE | AnimateWindowFlags.AW_HOR_POSITIVE;
                    AnimateWindowFlags scaleFlags = AnimateWindowFlags.AW_CENTER;
                    AnimateWindowFlags opacityFlags = AnimateWindowFlags.AW_BLEND;
                    switch (value)
                    {
                        case FormWindowState.Minimized:
                            break;
                        case FormWindowState.Maximized:
                            slideYFlags |= AnimateWindowFlags.AW_HIDE;
                            slideXFlags |= AnimateWindowFlags.AW_HIDE;
                            scaleFlags |= AnimateWindowFlags.AW_HIDE;
                            opacityFlags |= AnimateWindowFlags.AW_HIDE;
                            MaximizedBounds = Screen.FromHandle(Handle).WorkingArea;
                            break;
                    }
                    //NativeMethods.AnimateWindow(handle, 100, slideXFlags);
                    //NativeMethods.AnimateWindow(handle, 100, slideYFlags);
                    //NativeMethods.AnimateWindow(handle, 100, scaleFlags);
                    NativeMethods.AnimateWindow(handle, 100, opacityFlags);
                }
                base.WindowState = value;
            }
        }

        private void WmNCCalcSize(ref Message m)
        {
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowmessages/wm_nccalcsize.asp
            // http://groups.google.pl/groups?selm=OnRNaGfDEHA.1600%40tk2msftngp13.phx.gbl

            var max = MinMaxState == FormWindowState.Maximized;

            if (max)
            {
                //var x = NativeMethods.GetSystemMetrics(NativeConstants.SM_CXSIZEFRAME);
                //var y = NativeMethods.GetSystemMetrics(NativeConstants.SM_CYSIZEFRAME);
                //var p = NativeMethods.GetSystemMetrics(NativeConstants.SM_CXPADDEDBORDER);
                //var w = x + p;
                //var h = y + p;

                //r.left += w;
                //r.top += h;
                //r.right -= w;
                //r.bottom -= h;
                
                //NativeMethods.SystemParametersInfo(NativeConstants.SPI_GETWORKAREA, 0, m.LParam, 0);
                
                //var r = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));

                //var appBarData = new APPBARDATA()
                //{
                //    cbSize = Marshal.SizeOf(typeof(APPBARDATA))
                //};
                //var autohide = (NativeMethods.SHAppBarMessage(NativeConstants.ABM_GETSTATE, ref appBarData) & NativeConstants.ABS_AUTOHIDE) != 0;
                //if (autohide)
                //{

                //}

                //Marshal.StructureToPtr(r, m.LParam, true);
            }

            m.Result = IntPtr.Zero;
        }

        private void WmNCPaint(ref Message msg)
        {
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdi/pantdraw_8gdw.asp
            // example in q. 2.9 on http://www.syncfusion.com/FAQ/WindowsForms/FAQ_c41c.aspx#q1026q

            // The WParam contains handle to clipRegion or 1 if entire window should be repainted
            //PaintNonClientArea(msg.HWnd, (IntPtr)msg.WParam);

            // we handled everything
            msg.Result = NativeConstants.TRUE;
        }

        private void WmSetText(ref Message msg)
        {
            // allow the system to receive the new window title
            DefWndProc(ref msg);

            // repaint title bar
            //PaintNonClientArea(msg.HWnd, (IntPtr)1);
        }

        private void WmNCActivate(ref Message msg)
        {
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowmessages/wm_ncactivate.asp

            bool active = (msg.WParam == NativeConstants.TRUE);

            if (MinMaxState == FormWindowState.Minimized)
                DefWndProc(ref msg);
            else
            {
                // repaint title bar
                //PaintNonClientArea(msg.HWnd, (IntPtr)1);

                // allow to deactivate window
                msg.Result = NativeConstants.TRUE;
            }
        }
    }
}
