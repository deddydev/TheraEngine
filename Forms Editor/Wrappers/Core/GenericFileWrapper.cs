using Core.Win32.Native;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;

namespace TheraEditor.Wrappers
{
    public class GenericFileWrapper : BaseFileWrapper
    {
        public GenericFileWrapper() : base() { }
        public GenericFileWrapper(ContextMenuStrip menu) : base(menu) { }
        public GenericFileWrapper(string path) : base()
        {
            Text = Path.GetFileName(path);
            FilePath = Name = path;
        }
        public override IFileObject SingleInstance { get => null; set { } }
        public override bool IsLoaded => false;
        public override Type FileType => null;
        public override IGlobalFileRef SingleInstanceRef => throw new NotImplementedException();
        public override IFileObject GetNewInstance() => null;
        public override Task<IFileObject> GetNewInstanceAsync() => null;
        protected internal override void FixPath(string parentFolderPath) { }

        //private Process _runningProcess;
        //private string _processName;

        //Let Windows decide how the file should be edited
        public override void EditResource()
        {
            Process.Start(FilePath);
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
