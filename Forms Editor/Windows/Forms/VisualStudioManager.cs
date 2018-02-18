using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading;
using TheraEngine;

namespace TheraEditor.Windows.Forms
{
    public static class VisualStudioManager
    {
        private static string _devEnvPath = null;
        public static string DevEnvPath
        {
            get
            {
                if (_devEnvPath == null)
                {
                    _devEnvPath = FindDevEnv()[0];
                }
                return _devEnvPath;
            }
        }
        public static EnvDTE80.DTE2 CreateVSInstance()
        {
            EnvDTE80.DTE2 dte = null;

            string devEnvPath = DevEnvPath;
            if (devEnvPath != null)
            {
                try
                {
                    // Register the IOleMessageFilter to handle any threading errors.
                    if (Editor.Instance.InvokeRequired)
                        Editor.Instance.Invoke((Action)(() => MessageFilter.Register()));
                    else
                        MessageFilter.Register();

                    // start devenv
                    ProcessStartInfo procStartInfo = new ProcessStartInfo
                    {
                        Arguments = "-Embedding",
                        CreateNoWindow = true,
                        FileName = devEnvPath,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        WorkingDirectory = Path.GetDirectoryName(devEnvPath)
                    };

                    Process proc = Process.Start(procStartInfo);
                    
                    if (proc != null)
                        dte = GetDTE(proc.Id, 120);

                    if (dte != null)
                    {
                        //++_activeInstances;

                    }
                }
                catch (Exception ex)
                {
                    Engine.PrintLine(ex.ToString());
                }
            }
            return dte;
        }
        public static void VSInstanceClosed()
        {
            if (Editor.Instance.InvokeRequired)
                Editor.Instance.Invoke((Action)(() => MessageFilter.Revoke()));
            else
                MessageFilter.Revoke();
        }
        public static string[] FindDevEnv()
        {
            return new string[] { @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe" };
        }
        /// <summary>
        /// Gets the DTE object from any devenv process.
        /// </summary>
        /// <remarks>
        /// After starting devenv.exe, the DTE object is not ready. We need to try repeatedly and fail after the
        /// timeout.
        /// </remarks>
        /// <param name="processId">
        /// <param name="timeout">Timeout in seconds.
        /// <returns>
        /// Retrieved DTE object or <see langword="null"> if not found.
        /// </see></returns>
        private static EnvDTE80.DTE2 GetDTE(int processId, int timeout)
        {
            EnvDTE80.DTE2 res = null;
            DateTime startTime = DateTime.Now;

            while (res == null && DateTime.Now.Subtract(startTime).Seconds < timeout)
            {
                Thread.Sleep(1000);
                res = GetDTE(processId);
            }
            return res;
        }

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        /// <summary>
        /// Gets the DTE object from any devenv process.
        /// </summary>
        /// <param name="processId">
        /// <returns>
        /// Retrieved DTE object or <see langword="null"> if not found.
        /// </see></returns>
        private static EnvDTE80.DTE2 GetDTE(int processId)
        {
            object runningObject = null;

            IBindCtx bindCtx = null;
            IRunningObjectTable rot = null;
            IEnumMoniker enumMonikers = null;

            try
            {
                Marshal.ThrowExceptionForHR(CreateBindCtx(reserved: 0, ppbc: out bindCtx));
                bindCtx.GetRunningObjectTable(out rot);
                rot.EnumRunning(out enumMonikers);

                IMoniker[] moniker = new IMoniker[1];
                IntPtr numberFetched = IntPtr.Zero;
                while (enumMonikers.Next(1, moniker, numberFetched) == 0)
                {
                    IMoniker runningObjectMoniker = moniker[0];

                    string name = null;

                    try
                    {
                        if (runningObjectMoniker != null)
                            runningObjectMoniker.GetDisplayName(bindCtx, null, out name);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Do nothing, there is something in the ROT that we do not have access to.
                    }

                    Regex monikerRegex = new Regex(@"!VisualStudio.DTE\.\d+\.\d+\:" + processId, RegexOptions.IgnoreCase);
                    if (!string.IsNullOrEmpty(name) && monikerRegex.IsMatch(name))
                    {
                        Marshal.ThrowExceptionForHR(rot.GetObject(runningObjectMoniker, out runningObject));
                        break;
                    }
                }
            }
            finally
            {
                if (enumMonikers != null)
                    Marshal.ReleaseComObject(enumMonikers);
                if (rot != null)
                    Marshal.ReleaseComObject(rot);
                if (bindCtx != null)
                    Marshal.ReleaseComObject(bindCtx);
            }

            return runningObject as EnvDTE80.DTE2;
        }
        public class MessageFilter : IOleMessageFilter
        {
            // Class containing the IOleMessageFilter
            // thread error-handling functions.

            // Start the filter.
            public static void Register()
            {
                IOleMessageFilter newFilter = new MessageFilter();
                int hr = CoRegisterMessageFilter(newFilter, out IOleMessageFilter oldFilter);
                if (hr != 0)
                    Marshal.ThrowExceptionForHR(hr);
            }

            // Done with the filter, close it.
            public static void Revoke()
            {
                CoRegisterMessageFilter(null, out IOleMessageFilter oldFilter);
            }
            
            // IOleMessageFilter functions.
            // Handle incoming thread requests.
            int IOleMessageFilter.HandleInComingCall(int dwCallType,
                IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo)
            {
                //Return the flag SERVERCALL_ISHANDLED.
                return 0;
            }

            // Thread call was rejected, so try again.
            int IOleMessageFilter.RetryRejectedCall(
                IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
            {
                if (dwRejectType == 2)
                // flag = SERVERCALL_RETRYLATER.
                {
                    // Retry the thread call immediately if return >=0 & 
                    // <100.
                    return 99;
                }
                // Too busy; cancel call.
                return -1;
            }

            int IOleMessageFilter.MessagePending(
                IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
            {
                //Return the flag PENDINGMSG_WAITDEFPROCESS.
                return 2;
            }

            // Implement the IOleMessageFilter interface.
            [DllImport("Ole32.dll")]
            private static extern int
                CoRegisterMessageFilter(IOleMessageFilter newFilter, out
                IOleMessageFilter oldFilter);
        }

        [ComImport(), Guid("00000016-0000-0000-C000-000000000046"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IOleMessageFilter
        {
            [PreserveSig]
            int HandleInComingCall(
                int dwCallType,
                IntPtr hTaskCaller,
                int dwTickCount,
                IntPtr lpInterfaceInfo);

            [PreserveSig]
            int RetryRejectedCall(
                IntPtr hTaskCallee,
                int dwTickCount,
                int dwRejectType);

            [PreserveSig]
            int MessagePending(
                IntPtr hTaskCallee,
                int dwTickCount,
                int dwPendingType);
        }
    }
}