using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Reflection;
using TheraEngine.Editor;

namespace TheraEditor
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //LifetimeServices.LeaseTime = TimeSpan.FromSeconds(1);
            //LifetimeServices.RenewOnCallTime = TimeSpan.FromSeconds(1);
            //LifetimeServices.LeaseManagerPollTime = TimeSpan.FromSeconds(1);
            ServicePointManager.SecurityProtocol |=
                SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

            Application.Run(Editor.Instance);
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (args.LoadedAssembly.GetName().FullName.Contains("Puyo"))
                throw new Exception();
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            List<EditorState> dirty = EditorState.DirtyStates;
            Exception ex = e.Exception;
            using (IssueDialog d = new IssueDialog(ex, dirty))
                d.Show();
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!(e.ExceptionObject is Exception))
                return;

            List<EditorState> dirty = EditorState.DirtyStates;
            Exception ex = e.ExceptionObject as Exception;
            using (IssueDialog d = new IssueDialog(ex, dirty))
                d.Show();
        }
        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Exception is null)
                return;

            List<EditorState> dirty = EditorState.DirtyStates;
            Exception ex = e.Exception;
            using (IssueDialog d = new IssueDialog(ex, dirty))
                d.Show();
        }

        /// <summary>
        /// Populates a toolstrip button with all matching types based on the predicate method, arranged by namespace.
        /// </summary>
        /// <param name="button">The button to populate.</param>
        /// <param name="onClick">The method to trigger when a leaf button is pressed.
        /// The sender object, a ToolStripDropDownButton, has the corresponding Type assigned to its Tag property.</param>
        /// <param name="match">The predicate method used to find specific types.</param>
        public static TypeProxy[] PopulateTreeView(TreeView tree, EventHandler onClick, Predicate<TypeProxy> match)
        {
            TypeProxy[] fileObjecTypes = AppDomainHelper.FindTypes(match).ToArray();
            
            Dictionary<string, NamespaceNode> nodeCache = new Dictionary<string, NamespaceNode>();
            foreach (TypeProxy type in fileObjecTypes)
            {
                string path = type.Namespace;
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                NamespaceNode node;
                if (nodeCache.ContainsKey(name))
                    node = nodeCache[name];
                else
                {
                    node = new NamespaceNode(name);
                    node.CreateButton(true);
                    nodeCache.Add(name, node);
                    tree.Nodes.Add(node.TreeNode);
                }
                node.Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, type, null, true);
            }
            tree.AfterSelect += (s, e) => onClick(e.Node, e);

            return fileObjecTypes;
        }

        /// <summary>
        /// Populates a toolstrip button with all matching types based on the predicate method, arranged by namespace.
        /// </summary>
        /// <param name="button">The button to populate.</param>
        /// <param name="onClick">The method to trigger when a leaf button is pressed.
        /// The sender object, a ToolStripDropDownButton, has the corresponding Type assigned to its Tag property.</param>
        /// <param name="match">The predicate method used to find specific types.</param>
        public static List<TypeProxy> PopulateMenuDropDown(ToolStripDropDownItem button, EventHandler onClick, Predicate<TypeProxy> match)
        {
            List<TypeProxy> results = AppDomainHelper.FindTypes(match).ToList();
            //ProxyList<NamespaceNode> nodes2 = RemoteFunc.Invoke(AppDomainHelper.GetPrimaryAppDomain(), results, onClick, button, (types, onClick2, button2) =>
            //{
                Dictionary<string, NamespaceNode> nodeCache = new Dictionary<string, NamespaceNode>();
                foreach (TypeProxy type in results)
                {
                    string path = type.Namespace;
                    int dotIndex = path.IndexOf(".");
                    string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                    NamespaceNode node;
                    if (nodeCache.ContainsKey(name))
                        node = nodeCache[name];
                    else
                    {
                        node = new NamespaceNode(name);
                        node.CreateButton(false);
                        nodeCache.Add(name, node);
                    }
                    node.Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, type, onClick, false);
                }
                foreach (NamespaceNode node in nodeCache.Values)
                {
                    if (Editor.Instance.InvokeRequired)
                        Editor.Instance.BeginInvoke((Action)(() => button.DropDownItems.Add(node.Button)));
                    else
                        button.DropDownItems.Add(node.Button);
                }
                //return new ProxyList<NamespaceNode>(nodeCache.Values);
            //});
            return results;
        }
        private class NamespaceNode : MarshalByRefObject
        {
            public override object InitializeLifetimeService() => null;
            public NamespaceNode(string name)
            {
                Name = name;
                Children = new Dictionary<string, NamespaceNode>();
            }

            public string Name { get; set; }
            private Dictionary<string, NamespaceNode> Children { get; set; }
            public ToolStripMenuItem Button { get; set; }
            public TreeNode TreeNode { get; set; }

            public void CreateButton(bool treeView)
            {
                if (Editor.Instance.InvokeRequired)
                {
                    Editor.Instance.BeginInvoke((Action<bool>)CreateButton, treeView);
                    return;
                }

                if (treeView)
                    TreeNode = new TreeNode(Name);
                else
                    Button = new ToolStripMenuItem(Name)
                    {
                        AutoSize = true,
                        //ShowDropDownArrow = true,
                        TextAlign = ContentAlignment.MiddleLeft,
                    };
            }
            public void AddChild(NamespaceNode node, bool treeView)
            {
                if (Editor.Instance.InvokeRequired)
                {
                    Editor.Instance.BeginInvoke((Action<NamespaceNode, bool>)AddChild, node, treeView);
                    return;
                }

                if (treeView)
                    TreeNode.Nodes.Add(node.TreeNode);
                else
                    Button.DropDownItems.Add(node.Button);
            }
            private void AddUIButton(TypeProxy type, string displayText, EventHandler onClick, bool treeView)
            {
                if (Editor.Instance.InvokeRequired)
                {
                    Editor.Instance.BeginInvoke((Action<TypeProxy, string, EventHandler, bool>)AddUIButton, type, displayText, onClick, treeView);
                    return;
                }

                if (treeView)
                {
                    if (TreeNode is null)
                        CreateButton(true);

                    TreeNode treeNode = new TreeNode(displayText)
                    {
                        Tag = type,
                    };

                    TreeNode.Nodes.Add(treeNode);
                }
                else
                {
                    if (Button is null)
                        CreateButton(false);

                    ToolStripMenuItem btn = new ToolStripMenuItem(displayText)
                    {
                        AutoSize = true,
                        //ShowDropDownArrow = false,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Tag = type,
                    };
                    //Size s = TextRenderer.MeasureText(displayText, btn.Font);
                    //btn.Width = s.Width;
                    //btn.Height = s.Height + 10;
                    btn.Click += onClick;
                    Button.DropDownItems.Add(btn);
                }
            }
            public void Add(string path, TypeProxy type, EventHandler onClick, bool treeView)
            {
                if (string.IsNullOrEmpty(path))
                {
                    string typeName = type.GetFriendlyName();
                    //FileDef def = t.GetCustomAttributeExt<FileDef>();
                    string displayText = /*def?.UserFriendlyName ?? */typeName;
                    AddUIButton(type, displayText, onClick, treeView);
                    return;
                }
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                NamespaceNode node;
                if (Children.ContainsKey(name))
                    node = Children[name];
                else
                {
                    node = new NamespaceNode(name);
                    node.CreateButton(treeView);
                    Children.Add(name, node);
                    AddChild(node, treeView);
                }
                node.Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, type, onClick, treeView);
            }
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