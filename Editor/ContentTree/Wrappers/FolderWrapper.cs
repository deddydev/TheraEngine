using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;
using TheraEngine.Scripting;

namespace TheraEditor.Wrappers
{
    public interface IFolderWrapper : IBasePathWrapper
    {
        event Action NewFolderEvent;

        void NewFolder();
        void ToArchive();
        void LoadFileTypes(bool now);
    }
    public class FolderWrapper : BasePathWrapper, IFolderWrapper
    {
        private TMenuOption ImportFileOption { get; set; }
        private TMenuOption NewFileOption { get; set; }

        public FolderWrapper()
        {
            //Generate menu in main domain for folders
            //Populate new and import child items from game domain

            ImportFileOption = new TMenuOption("Import File", null, Keys.Control | Keys.I);
            NewFileOption = new TMenuOption("New File", null, Keys.Control | Keys.N);

            Menu = new TMenu()
            {
                TMenuOption.Rename(this),
                TMenuOption.Explorer(this),
                TMenuDivider.Instance,
                ImportFileOption,
                NewFileOption,
                new TMenuOption("New Folder", NewFolder, Keys.Control | Keys.F),
                new TMenuOption("Compile To Archive", ToArchive, Keys.Control | Keys.A),
                TMenuDivider.Instance,
                TMenuOption.Cut(this),
                TMenuOption.Copy(this),
                TMenuOption.Paste(this),
                TMenuDivider.Instance,
                TMenuOption.Delete(this),
            };
        }

        public event Action NewFolderEvent;
        public void NewFolder() => NewFolderEvent?.Invoke();

        private static bool IsFileObject(TypeProxy t) =>
            !t.Assembly.EqualTo(Assembly.GetExecutingAssembly()) &&
            !t.IsAbstract && !t.IsInterface && t.GetConstructors().Any(x => x.IsPublic) &&
            t.IsSubclassOf(typeof(TFileObject)) &&
            t.HasCustomAttribute<TFileExt>();

        private static bool Is3rdPartyImportable(TypeProxy t)
            => IsFileObject(t) && (t?.GetCustomAttribute<TFileExt>().HasAnyImportableExtensions ?? false);

        private Lazy<List<Program.NamespaceNode>> ImportableTree { get; set; } 
            = new Lazy<List<Program.NamespaceNode>>(() => Program.GenerateTypeTree(Is3rdPartyImportable),
                System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        private Lazy<List<Program.NamespaceNode>> NewTree { get; set; }
            = new Lazy<List<Program.NamespaceNode>>(() => Program.GenerateTypeTree(IsFileObject),
                System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        public void LoadFileTypes(bool now)
        {
            ImportFileOption.Clear();
            NewFileOption.Clear();

            //TMenuOption newCodeItem = new TMenuOption("C#", null, Keys.None);
            //newCodeItem.Add(new TMenuOption("Class", null, NewClassAction));
            //newCodeItem.Add(new TMenuOption("Struct", null, NewStructAction));
            //newCodeItem.Add(new TMenuOption("Interface", null, NewInterfaceAction));
            //newCodeItem.Add(new TMenuOption("Enum", null, NewEnumAction));
            //NewFileOption.Add(newCodeItem);

            if (!now)
            {
                ImportableTree = new Lazy<List<Program.NamespaceNode>>(() => 
                    Program.GenerateTypeTree(Is3rdPartyImportable),
                    System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

                NewTree = new Lazy<List<Program.NamespaceNode>>(() => 
                    Program.GenerateTypeTree(IsFileObject),
                    System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
            }
            else
            {
                Engine.PrintLine("Loading importable and creatable file types to folder menu.");

                Task import = Task.Run(() => 
                    RecursiveAdd(ImportFileOption, ImportableTree.Value, OnImportClick));

                Task create = Task.Run(() => 
                    RecursiveAdd(NewFileOption, NewTree.Value, OnNewClick));

                Task.WhenAll(import, create).ContinueWith(t => 
                    Engine.PrintLine("Finished loading importable and creatable file types to folder menu."));
            }
        }

        private void OnImportClick(TypeProxy type)
            => Editor.DomainProxy.ImportFile(type, FilePath);

        private void OnNewClick(TypeProxy type)
        {
            object o = Editor.UserCreateInstanceOf(type, true);
            if (!(o is IFileObject file))
                return;

            Engine.DomainProxy.ExportFile(file, FilePath, EProprietaryFileFormat.XML);

            //if (Serializer.PreExport(file, dir, file.Name, EProprietaryFileFormat.XML, null, out string path))
            //{
            //    int op = Editor.Instance.BeginOperation($"Exporting {path}...", $"Export to {path} completed.", out Progress<float> progress, out CancellationTokenSource cancel);
            //    string name = file.Name;
            //    name = name.Replace("<", "[");
            //    name = name.Replace(">", "]");
            //    await Serializer.ExportXMLAsync(file, dir, name, ESerializeFlags.Default, progress, cancel.Token);
            //    Editor.Instance.EndOperation(op);
            //}
        }

        private class TMenuNamespaceOption : TMenuOption
        {
            public TMenuNamespaceOption(Program.NamespaceNode node) : base(node.Name, null, Keys.None)
            {
                NamespaceInfo = node;
                Action = SelectedAction;
            }

            public Action<TypeProxy> CreateAction { get; set; }
            public Program.NamespaceNode NamespaceInfo { get; set; }

            private void SelectedAction()
            {
                TypeProxy type = NamespaceInfo?.Type;
                if (type != null)
                    CreateAction?.Invoke(type);
            }
        }
        private void RecursiveAdd(TMenuOption parentOption, IEnumerable<Program.NamespaceNode> childList, Action<TypeProxy> action)
        {
            foreach (var child in childList)
            {
                var op = new TMenuNamespaceOption(child) { CreateAction = action };
                RecursiveAdd(op, child.Children.OrderBy(x => x.Key).Select(x => x.Value), action);
                parentOption.Add(op);
            }
        }

        private enum ECodeFileType
        {
            Class,
            Struct,
            Interface,
            Enum,
        }
        private async void NewCodeFile(ECodeFileType type)
        {
            string ns = Editor.Instance.Project.RootNamespace;
            string ClassName = "NewClass";

            TextFile file = Engine.Files.Script(type.ToString() + "_Template.cs");
            string text = string.Format(file.Text, ns, ClassName, ": " + nameof(TObject));
            text = text.Replace("@", "{").Replace("#", "}");
            CSharpScript code = CSharpScript.FromText(text);
            string dir = FilePath;

            string name = "NewCodeFile";
            string path = Path.Combine(dir, name + ".cs");
            await Editor.RunOperationAsync(
                $"Saving script to {path}...", 
                $"Script saved to {path} successfully.",
                async (p, c) => await code.Export3rdPartyAsync(dir, "NewCodeFile", "cs", p, c.Token));
        }

        public async void ToArchive()
            => await ToArchiveAsync();

        public async Task ToArchiveAsync(
            ESerializeFlags flags = ESerializeFlags.Default,
            EProprietaryFileFormat format = EProprietaryFileFormat.Binary)
        {
            string path = FilePath;
            ArchiveFile file = ArchiveFile.FromDirectory(path, true);
            string parentDir = Path.GetDirectoryName(path);
            string dirName = Path.GetFileName(path);
            await file.ExportAsync(parentDir, dirName, flags, format);
        }

        public override void Explorer()
        {
            string path = FilePath;
            if (!string.IsNullOrEmpty(path))
                Process.Start("explorer.exe", path);
        }
    }
}
