using Core.Win32.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace TheraEngine.Core.Extensions
{
    public static class ControlExtension
    {
        public static bool IsOnScreen(this Control control)
            => Screen.AllScreens.Any(x => x.WorkingArea.Contains(control.Bounds));
        
        public static bool IsOverlapped(this IWin32Window window)
        {
            if (window == null)
                return false;
            if (window.Handle == IntPtr.Zero)
                return false;
            if (!NativeMethods.IsWindowVisible(window.Handle))
                return false;

            IntPtr hWnd = window.Handle;
            HashSet<IntPtr> visited = new HashSet<IntPtr> { hWnd };

            // The set is used to make calling GetWindow in a loop stable by checking if we have already
            //  visited the window returned by GetWindow. This avoids the possibility of an infinate loop.

            NativeMethods.GetWindowRect(hWnd, out RECT thisRect);

            while ((hWnd = NativeMethods.GetWindow(hWnd, NativeConstants.GW_HWNDPREV)) != IntPtr.Zero && !visited.Contains(hWnd))
            {
                visited.Add(hWnd);
                if (NativeMethods.IsWindowVisible(hWnd) &&
                    NativeMethods.GetWindowRect(hWnd, out RECT testRect) &&
                    NativeMethods.IntersectRect(out RECT intersection, ref thisRect, ref testRect))
                    return true;
            }

            return false;
        }

        [ReflectionPermission(SecurityAction.Demand, MemberAccess = true)]
        public static void Reset(this Control c)
        {
            typeof(Control).InvokeMember(
                "SetState",
                BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance,
                null, c,
                new object[] { 0x400000, false });
        }
        public static bool ThreadSafeBlockingInvoke<T>(this Control c, Delegate method, out T result, params object[] args)
        {
            if (c != null && c.InvokeRequired && !(c.IsDisposed || c.Disposing))
            {
                IAsyncResult async = c.BeginInvoke(method, args);

                //if (!async.CompletedSynchronously)
                //{
                //    while (!async.IsCompleted)
                //    {
                //        Application.DoEvents();
                //        Thread.Sleep(0);
                //    }
                //}
                result = (T)c.EndInvoke(async);
                return true;
            }
            result = default;
            return false;
        }
        /// <summary>
        /// Returns true if the render panel needs to be invoked from the calling thread.
        /// If it does, then it calls the method.
        /// </summary>
        public static bool ThreadSafeBlockingInvoke(this Control c, Delegate method, params object[] args)
        {
            if (c != null && c.InvokeRequired && !(c.IsDisposed || c.Disposing))
            {
                IAsyncResult async = c.BeginInvoke(method, args);

                //if (!async.CompletedSynchronously)
                //{
                //    while (!async.IsCompleted)
                //    {
                //        Application.DoEvents();
                //        Thread.Sleep(0);
                //        break;
                //    }
                //    //c.EndInvoke(async);
                //}
                return true;
            }
            return false;
        }
    }
}
