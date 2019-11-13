﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public class DefaultFileWrapper : BaseFileWrapper
    {
        public override bool IsLoaded => false;
        public override TypeProxy FileType => null;
        public IFileObject SingleInstance { get => SingleInstanceRef.File; set => SingleInstanceRef.File = value; }
        public IFileRef SingleInstanceRef { get; } = new LocalFileRef<IFileObject>();
        public IFileObject GetNewInstance() => null;
        public Task<IFileObject> GetNewInstanceAsync() => null;
        public override IFileObject GetFile() => SingleInstance;

        //private Process _runningProcess;
        //private string _processName;

        //Let Windows decide how the file should be edited
        public override void Edit()
        {
            try
            {
                Process.Start(FilePath);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            //try
            //{
                //if (_runningProcess != null)
                //{
                //    if (_runningProcess.MainWindowHandle == IntPtr.Zero)
                //        NativeMethods.ShowWindow(_runningProcess.Handle, EShowWindowEnum.Restore);
                //    NativeMethods.SetForegroundWindow(_runningProcess.MainWindowHandle);
                //}
                //else
                //{
                //    _runningProcess = new Process
                //    {
                //        EnableRaisingEvents = true,
                //        StartInfo = new ProcessStartInfo(FilePath)
                //        {
                //            RedirectStandardError = true,
                //            RedirectStandardOutput = true,
                //            UseShellExecute = false,
                //        }
                //    };
                //    _runningProcess.Exited += P_Exited;
                //    _runningProcess.ErrorDataReceived += P_ErrorDataReceived;
                //    _runningProcess.OutputDataReceived += P_OutputDataReceived;
                //    _runningProcess.Start();
                //    _runningProcess.BeginOutputReadLine();
                //    _runningProcess.BeginErrorReadLine();
                //    _processName = _runningProcess.ProcessName;
                //}
            //}
            //catch (Exception ex)
            //{

            //}
        }

        //private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    Engine.PrintLine(e.Data);
        //}
        //private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    Engine.PrintLine(e.Data);
        //}
        //private void P_Exited(object sender, EventArgs e)
        //{
        //    Engine.PrintLine("External program {0} has exited.", _processName);
        //    _runningProcess = null;
        //}
    }
}
