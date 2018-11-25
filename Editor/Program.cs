using Core.Win32.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Editor;

namespace TheraEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ServicePointManager.SecurityProtocol |= 
                SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.Run(new Editor());
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            List<EditorState> dirty = Editor.Instance.GetDirtyFiles();
            Exception ex = e.Exception;
            using (IssueDialog d = new IssueDialog(ex, dirty))
                d.Show();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                List<EditorState> dirty = Editor.Instance.GetDirtyFiles();
                Exception ex = e.ExceptionObject as Exception;
                using (IssueDialog d = new IssueDialog(ex, dirty))
                    d.Show();
            }
        }

        /// <summary>
        /// Returns true if any editor window has focus.
        /// </summary>
        private static bool CheckFocus()
        {
            var activatedHandle = NativeMethods.GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
                return false; //No window is currently activated

            int procId = Process.GetCurrentProcess().Id;
            NativeMethods.GetWindowThreadProcessId(activatedHandle, out int activeProcId);

            return activeProcId == procId;
        }
        /// <summary>
        /// Populates a toolstrip button with all matching types based on the predicate method, arranged by namespace.
        /// </summary>
        /// <param name="button">The button to populate.</param>
        /// <param name="onClick">The method to trigger when a leaf button is pressed.
        /// The sender object, a ToolStripDropDownButton, has the corresponding Type assigned to its Tag property.</param>
        /// <param name="match">The predicate method used to find specific types.</param>
        public static Type[] PopulateMenuDropDown(ToolStripDropDownItem button, EventHandler onClick, Predicate<Type> match)
        {
            Type[] fileObjectTypes = Engine.FindTypes(match, true, Assembly.GetExecutingAssembly()).ToArray();

            Dictionary<string, NamespaceNode> nodeCache = new Dictionary<string, NamespaceNode>();
            foreach (Type t in fileObjectTypes)
            {
                string path = t.Namespace;
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                NamespaceNode node;
                if (nodeCache.ContainsKey(name))
                    node = nodeCache[name];
                else
                {
                    node = new NamespaceNode(name);
                    nodeCache.Add(name, node);
                    button.DropDownItems.Add(node.Button);
                }
                node.Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, t, onClick);
            }

            return fileObjectTypes;
        }
        private class NamespaceNode
        {
            public NamespaceNode(string name)
            {
                _name = name;
                _children = new Dictionary<string, NamespaceNode>();
                Button = new ToolStripDropDownButton(_name)
                {
                    AutoSize = false,
                    ShowDropDownArrow = true,
                    TextAlign = ContentAlignment.MiddleLeft,
                };
                Size s = TextRenderer.MeasureText(_name, Button.Font);
                Button.Width = s.Width + 10;
                Button.Height = s.Height + 10;
            }

            string _name;
            Dictionary<string, NamespaceNode> _children;
            ToolStripDropDownButton _button;

            public string Name { get => _name; set => _name = value; }
            private Dictionary<string, NamespaceNode> Children { get => _children; set => _children = value; }
            public ToolStripDropDownButton Button { get => _button; set => _button = value; }

            public void Add(string path, Type t, EventHandler onClick)
            {
                if (string.IsNullOrEmpty(path))
                {
                    string typeName = t.GetFriendlyName();
                    //FileDef def = t.GetCustomAttributeExt<FileDef>();
                    string displayText = /*def?.UserFriendlyName ?? */typeName;
                    ToolStripDropDownButton btn = new ToolStripDropDownButton(displayText)
                    {
                        AutoSize = false,
                        ShowDropDownArrow = false,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Tag = t,
                    };
                    Size s = TextRenderer.MeasureText(displayText, btn.Font);
                    btn.Width = s.Width;
                    btn.Height = s.Height + 10;
                    btn.Click += onClick;
                    _button.DropDownItems.Add(btn);
                    return;
                }
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                NamespaceNode node;
                if (_children.ContainsKey(name))
                    node = _children[name];
                else
                {
                    node = new NamespaceNode(name);
                    _children.Add(name, node);
                    Button.DropDownItems.Add(node.Button);
                }
                node.Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, t, onClick);
            }
        }

        public static bool IsFocused { get; private set; } = true;
        public static event Action GotFocus;
        public static event Action LostFocus;
        internal static void FocusChanged()
        {
            //if (CheckFocus())
            //{
            //    if (!IsFocused)
            //    {
            //        IsFocused = true;
            //        Project p = Editor.Instance.Project;
            //        bool? capFPS = p.EngineSettings?.File?.CapFPS;
            //        bool? capUPS = p.EngineSettings?.File?.CapUPS;
            //        Engine.TargetRenderFreq = p != null && capFPS != null && capFPS.Value ? p.EngineSettings.File.TargetFPS : 0.0f;
            //        Engine.TargetUpdateFreq = p != null && capUPS != null && capUPS.Value ? p.EngineSettings.File.TargetUPS : 0.0f;
            //        GotFocus?.Invoke();
            //    }
            //}
            //else
            //{
            //    if (IsFocused)
            //    {
            //        IsFocused = false;
            //        Engine.TargetRenderFreq = 3.0f;
            //        Engine.TargetUpdateFreq = 30.0f;
            //        LostFocus?.Invoke();
            //    }
            //}
        }

        //TODO: loop through all loaded assemblies and delete individually
        //internal static void DeleteSelf()
        //{
        //    string exe = Application.StartupPath;
        //    string batchCode =
        //        "@echo off"         + Environment.NewLine +
        //        ":dele"             + Environment.NewLine +
        //        $"del \"{exe}\""    + Environment.NewLine +
        //        $"if Exist \"{exe}\" GOTO dele" + Environment.NewLine +
        //        "del %0";
            
        //    string tempPath = Environment.GetEnvironmentVariable("TMP");
        //    string batName = SerializationCommon.ResolveFileName(tempPath, "TheraEngineUpdate", "bat");
        //    string batPath = tempPath + batName;

        //    File.WriteAllText(batPath, batchCode);

        //    Process batProcess = new Process();
        //    batProcess.StartInfo.FileName = batPath;
        //    batProcess.StartInfo.CreateNoWindow = true;
        //    batProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //    batProcess.StartInfo.UseShellExecute = true;

        //    batProcess.Start();
        //    batProcess.PriorityClass = ProcessPriorityClass.Normal;
        //}
    }
}