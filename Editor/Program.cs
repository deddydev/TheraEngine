using AppDomainToolkit;
using Core.Win32.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Reflection.Proxies;
using TheraEngine.Editor;

namespace TheraEditor
{
    public class Program : MarshalByRefObject
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

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.Run(Editor.Instance);
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
                    node = new NamespaceNode(name, true);
                    nodeCache.Add(name, node);
                    tree.Nodes.Add(node.TreeNode);
                }
                node.Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, type, null);
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
        public static ProxyList<TypeProxy> PopulateMenuDropDown(ToolStripDropDownItem button, EventHandler onClick, Predicate<TypeProxy> match)
        {
            ProxyList<TypeProxy> results = new ProxyList<TypeProxy>(AppDomainHelper.FindTypes(match));
            RemoteAction.Invoke(AppDomainHelper.GetPrimaryAppDomain(), results, (types) =>
            {
                Dictionary<string, NamespaceNode> nodeCache = new Dictionary<string, NamespaceNode>();
                foreach (TypeProxy type in types)
                {
                    string path = type.Namespace;
                    int dotIndex = path.IndexOf(".");
                    string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                    NamespaceNode node;
                    if (nodeCache.ContainsKey(name))
                        node = nodeCache[name];
                    else
                    {
                        node = new NamespaceNode(name, false);
                        nodeCache.Add(name, node);
                        if (Editor.Instance.InvokeRequired)
                            Editor.Instance.BeginInvoke((Action)(() => button.DropDownItems.Add(node.Button)));
                        else
                            button.DropDownItems.Add(node.Button);
                    }
                    node.Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, type, onClick);
                }
            });
            return results;
        }
        private class NamespaceNode
        {
            public NamespaceNode(string name, bool treeView)
            {
                Name = name;
                Children = new Dictionary<string, NamespaceNode>();
                if (Editor.Instance.InvokeRequired)
                {
                    Editor.Instance.BeginInvoke((Action)(() =>
                    {
                        if (treeView)
                        {
                            TreeNode = new TreeNode(Name);
                        }
                        else
                        {
                            Button = new ToolStripMenuItem(Name)
                            {
                                AutoSize = true,
                                //ShowDropDownArrow = true,
                                TextAlign = ContentAlignment.MiddleLeft,
                            };
                        }
                    }));
                }
                else
                {
                    if (treeView)
                    {
                        TreeNode = new TreeNode(Name);
                    }
                    else
                    {
                        Button = new ToolStripMenuItem(Name)
                        {
                            AutoSize = true,
                            //ShowDropDownArrow = true,
                            TextAlign = ContentAlignment.MiddleLeft,
                        };
                    }
                }
            }

            public string Name { get; set; }
            private Dictionary<string, NamespaceNode> Children { get; set; }
            public ToolStripMenuItem Button { get; set; }
            public TreeNode TreeNode { get; set; }

            private void AddUIButton(TypeProxy type, string displayText, EventHandler onClick)
            {
                if (Button != null)
                {
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
                else
                {
                    TreeNode treeNode = new TreeNode(displayText)
                    {
                        Tag = type,
                    };

                    TreeNode.Nodes.Add(treeNode);
                }
            }
            public void Add(string path, TypeProxy type, EventHandler onClick)
            {
                if (string.IsNullOrEmpty(path))
                {
                    string typeName = type.GetFriendlyName();
                    //FileDef def = t.GetCustomAttributeExt<FileDef>();
                    string displayText = /*def?.UserFriendlyName ?? */typeName;
                    if (Editor.Instance.InvokeRequired)
                        Editor.Instance.BeginInvoke((Action<TypeProxy, string, EventHandler>)AddUIButton, type, displayText, onClick);
                    else
                        AddUIButton(type, displayText, onClick);
                    return;
                }
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                NamespaceNode node;
                if (Children.ContainsKey(name))
                    node = Children[name];
                else
                {
                    bool treeView = TreeNode != null;
                    node = new NamespaceNode(name, treeView);
                    Children.Add(name, node);
                    if (Editor.Instance.InvokeRequired)
                    {
                        Editor.Instance.BeginInvoke((Action)(() =>
                        {
                            if (treeView)
                                TreeNode.Nodes.Add(node.TreeNode);
                            else
                                Button.DropDownItems.Add(node.Button);
                        }));
                    }
                    else
                    {
                        if (treeView)
                            TreeNode.Nodes.Add(node.TreeNode);
                        else
                            Button.DropDownItems.Add(node.Button);
                    }
                }
                node.Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, type, onClick);
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