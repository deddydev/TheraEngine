using System;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;

namespace TheraEngine.Core.Extensions
{
    public static class ControlExtension
    {
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
            if (c != null && c.InvokeRequired)
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
            if (c != null && c.InvokeRequired)
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
