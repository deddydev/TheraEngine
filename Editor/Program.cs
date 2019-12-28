using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.ContentTree.Core;
using TheraEditor.Windows.Forms;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Core.Reflection;
using TheraEngine.Editor;

namespace TheraEditor
{
    public static class Program
    {
        public static AppDomain MainDomain { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
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

#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
#endif
            AppDomain.CurrentDomain.AssemblyLoad += AppDomainHelper.CurrentDomain_AssemblyLoad;

            Engine.Instance.SetDomainProxy<EngineDomainProxyEditor>(AppDomain.CurrentDomain, null, null);

            Application.Run(Editor.Instance);
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            List<EditorState> dirty = EditorState.DirtyStates;
            Exception ex = e.Exception;
            using (IssueDialog d = new IssueDialog(ex, dirty))
                d.ShowDialog();
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

        public static List<NamespaceNode> GenerateTypeTree(IEnumerable<TypeProxy> types)
        {
            Dictionary<string, NamespaceNode> children = new Dictionary<string, NamespaceNode>();
            foreach (TypeProxy type in types)
            {
                string path = type.FullName;
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                NamespaceNode node;

                if (children.ContainsKey(name))
                    node = children[name];
                else
                {
                    node = new NamespaceNode(name);
                    children.Add(name, node);
                }

                node.Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, type);
            }
            return children.Values.ToList();
        }

        public static List<NamespaceNode> GenerateTypeTree(Predicate<TypeProxy> match)
            => GenerateTypeTree(AppDomainHelper.FindTypes(match));

        public static void GenerateTMenu(TMenuOption parentOption, IEnumerable<NamespaceNode> childList, Action<TypeProxy> action)
        {
            foreach (var child in childList)
            {
                var op = new TMenuNamespaceOption(child) { CreateAction = action };
                GenerateTMenu(op, child.Children.OrderBy(x => x.Key).Select(x => x.Value), action);
                parentOption.Add(op);
            }
        }

        public static void GenerateTreeNodes(
            TreeNodeCollection parentCollection,
            IEnumerable<NamespaceNode> childList)
        {
            foreach (var child in childList)
            {
                var op = new TreeNode(child.Name) { Tag = child.Type };
                GenerateTreeNodes(op.Nodes, child.Children.OrderBy(x => x.Key).Select(x => x.Value));
                parentCollection.Add(op);
            }
        }

        public static void GenerateToolStripItems(
            ToolStripItemCollection parentCollection, 
            IEnumerable<NamespaceNode> childList,
            Func<NamespaceNode, ToolStripMenuItem> createItemFunc)
        {
            foreach (var child in childList)
            {
                var op = createItemFunc(child);
                GenerateToolStripItems(op.DropDownItems, child.Children.OrderBy(x => x.Key).Select(x => x.Value), createItemFunc);
                parentCollection.Add(op);
            }
        }

        private class TMenuNamespaceOption : TMenuOption
        {
            public TMenuNamespaceOption(NamespaceNode node) : base(node.Name, null, Keys.None)
            {
                NamespaceInfo = node;
                Action = SelectedAction;
            }

            public Action<TypeProxy> CreateAction { get; set; }
            public NamespaceNode NamespaceInfo { get; set; }

            private void SelectedAction()
            {
                TypeProxy type = NamespaceInfo?.Type;
                if (type != null)
                    CreateAction?.Invoke(type);
            }
        }

        public class NamespaceNode : MarshalByRefObject
        {
            public override object InitializeLifetimeService() => null;
            public NamespaceNode(string name)
            {
                Name = name;
                Children = new Dictionary<string, NamespaceNode>();
            }

            public string Name { get; set; }
            public NamespaceNode Parent { get; set; }
            public Dictionary<string, NamespaceNode> Children { get; set; }
            public TypeProxy Type { get; set; }
            public bool IsLeaf => Type != null;

            //public void CreateButton(bool treeView)
            //{
            //    if (treeView)
            //        TreeNode = new TreeNode(Name);
            //    else
            //        Button = new ToolStripMenuItem(Name)
            //        {
            //            AutoSize = true,
            //            //ShowDropDownArrow = true,
            //            TextAlign = ContentAlignment.MiddleLeft,
            //        };
            //}
            //public void AddChild(NamespaceNode node, bool treeView)
            //{
            //    if (treeView)
            //        TreeNode.Nodes.Add(node.TreeNode);
            //    else
            //        Button.DropDownItems.Add(node.Button);
            //}
            //public void PopulateTreeNode(TreeNode node, EventHandler onClick)
            //{
            //    if (TreeNode is null)
            //        CreateButton(true);

            //    TreeNode treeNode = new TreeNode(displayText)
            //    {
            //        Tag = type,
            //    };

            //    TreeNode.Nodes.Add(treeNode);
            //}
            //public void PopulateMenuNode(ToolStripMenuItem node, EventHandler onClick)
            //{
            //    //if (Button is null)
            //    //    CreateButton(false);

            //    //ToolStripMenuItem btn = new ToolStripMenuItem(displayText)
            //    //{
            //    //    AutoSize = true,
            //    //    //ShowDropDownArrow = false,
            //    //    TextAlign = ContentAlignment.MiddleLeft,
            //    //    Tag = type,
            //    //};
            //    ////Size s = TextRenderer.MeasureText(displayText, btn.Font);
            //    ////btn.Width = s.Width;
            //    ////btn.Height = s.Height + 10;
            //    //btn.Click += onClick;
            //    //Button.DropDownItems.Add(btn);
            //}
            public void Add(string path, TypeProxy type)
            {
                if (string.IsNullOrEmpty(path))
                {
                    Type = type;
                    Name = Type.GetFriendlyName();
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
                    Children.Add(name, node);
                }
                node.Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, type);
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