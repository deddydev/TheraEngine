using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new Editor());
        }

        /// <summary>Returns true if the current application has focus, false otherwise</summary>
        private static bool CheckFocus()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
                return false; //No window is currently activated

            int procId = Process.GetCurrentProcess().Id;
            GetWindowThreadProcessId(activatedHandle, out int activeProcId);

            return activeProcId == procId;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        public static Type[] FindPublicTypes(Predicate<Type> match)
        {
            return
                (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetExportedTypes()
                where match(assemblyType) && !assemblyType.IsAbstract
                select assemblyType).ToArray();
        }
        public static void PopulateMenuDropDown(ToolStripDropDownItem button, EventHandler onClick, Predicate<Type> match)
        {
            var fileObjectTypes =
                            from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetExportedTypes()
                            where match(assemblyType) && !assemblyType.IsAbstract
                            select assemblyType;
            Dictionary<string, NamespaceNode> nodeCache = new Dictionary<string, NamespaceNode>();
            foreach (Type t in fileObjectTypes)
            {
                string path = t.Namespace;
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                if (nodeCache.ContainsKey(name))
                    nodeCache[name].Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, t, onClick);
                else
                {
                    NamespaceNode node = new NamespaceNode(name);
                    nodeCache.Add(name, node);
                    button.DropDownItems.Add(node.Button);
                }
            }
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
                    ToolStripDropDownButton btn = new ToolStripDropDownButton(t.Name)
                    {
                        AutoSize = false,
                        ShowDropDownArrow = false,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Tag = t,
                    };
                    Size s = TextRenderer.MeasureText(t.Name, btn.Font);
                    btn.Width = s.Width;
                    btn.Height = s.Height + 10;
                    btn.Click += onClick;
                    _button.DropDownItems.Add(btn);
                    return;
                }
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                if (_children.ContainsKey(name))
                    _children[name].Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, t, onClick);
                else
                {
                    NamespaceNode node = new NamespaceNode(name);
                    _children.Add(name, node);
                    Button.DropDownItems.Add(node.Button);
                }
            }
        }

        public static bool IsFocused { get; private set; } = true;
        public static event Action GotFocus;
        public static event Action LostFocus;
        internal static void FocusChanged()
        {
            if (CheckFocus())
            {
                if (!IsFocused)
                {
                    IsFocused = true;
                    Project p = Editor.Instance.Project;
                    Engine.TargetRenderFreq = p != null && p.EngineSettings.File.CapFPS ? p.EngineSettings.File.TargetFPS : 0.0f;
                    Engine.TargetUpdateFreq = p != null && p.EngineSettings.File.CapUPS ? p.EngineSettings.File.TargetUPS : 0.0f;
                    GotFocus?.Invoke();
                }
            }
            else
            {
                if (IsFocused)
                {
                    IsFocused = false;
                    Engine.TargetRenderFreq = 3.0f;
                    Engine.TargetUpdateFreq = 3.0f;
                    LostFocus?.Invoke();
                }
            }
        }
    }
}