using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Wrappers
{
    [TreeFileType("dae")]
    public class ColladaWrapper : FileWrapper
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static ColladaWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            _menu.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.O));   //1
            ToolStripMenuItem importItem = new ToolStripMenuItem("Import As...", null);
            ToolStripMenuItem skeletalMeshImportItem = new ToolStripMenuItem("Skeletal Model", null, ImportAsSkeletalMeshAction);
            ToolStripMenuItem staticMeshImportItem = new ToolStripMenuItem("Static Model", null, ImportAsStaticMeshAction);
            ToolStripMenuItem skeletonImportItem = new ToolStripMenuItem("Skeleton", null, ImportAsSkeletonAction);
            ToolStripMenuItem actorImportItem = new ToolStripMenuItem("Actor", null, ImportAsActorAction);
            importItem.DropDownItems.Add(skeletalMeshImportItem);
            importItem.DropDownItems.Add(staticMeshImportItem);
            importItem.DropDownItems.Add(skeletonImportItem);
            importItem.DropDownItems.Add(actorImportItem);
            _menu.Items.Add(importItem);                                                                                //2
            _menu.Items.Add(new ToolStripMenuItem("Edit Raw", null, EditRawAction, Keys.F3));                           //3
            _menu.Items.Add(new ToolStripSeparator());                                                                  //4
            _menu.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //5
            _menu.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //6
            _menu.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //7
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //8
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }

        private static void ImportAsActorAction(object sender, EventArgs e)
            => GetInstance<ColladaWrapper>().ImportAsActor();
        private static void ImportAsSkeletonAction(object sender, EventArgs e)
            => GetInstance<ColladaWrapper>().ImportAsSkeleton();
        private static void ImportAsStaticMeshAction(object sender, EventArgs e)
            => GetInstance<ColladaWrapper>().ImportAsStaticMesh();
        private static void ImportAsSkeletalMeshAction(object sender, EventArgs e)
            => GetInstance<ColladaWrapper>().ImportAsSkeletalMesh();

        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {

        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            ColladaWrapper w = GetInstance<ColladaWrapper>();
        }
        #endregion

        public ColladaWrapper() { }

        private void ImportAsActor()
        {
            //int op = Editor.Instance.BeginOperation($"Importing {Path.GetFileName(FilePath)} as skeleton...", out Progress<float> progress, out CancellationTokenSource cancel);
            //TFileObject actor = await Actor.LoadDAEAsync(FilePath, progress, cancel.Token);
            //Editor.Instance.EndOperation(op);

            //string dir = Path.GetDirectoryName(FilePath);
            //string name = Path.GetFileNameWithoutExtension(FilePath);

            //op = Editor.Instance.BeginOperation("Saving model...", out progress, out cancel);
            //await actor.ExportAsync(dir, name, EFileFormat.XML, null, ESerializeFlags.Default, progress, cancel.Token);
            //Editor.Instance.EndOperation(op);
        }
        private async void ImportAsSkeleton()
        {
            ColladaImportOptions o = new ColladaImportOptions()
            {
                IgnoreFlags =
                    Collada.EIgnoreFlags.Extra |
                    Collada.EIgnoreFlags.Controllers |
                    Collada.EIgnoreFlags.Cameras |
                    Collada.EIgnoreFlags.Lights
            };

            var data = await Editor.RunOperationAsync(
                $"Importing {Path.GetFileName(FilePath)} as skeleton...", "Skeleton imported.",
                async (p, c) => await Collada.ImportAsync(FilePath, o, p, c.Token));

            if (data?.Models is null || data.Models.Count == 0)
                return;

            TFileObject skeleton = data.Models[0].Skeleton;
            if (skeleton is null)
                return;

            string dir = Path.GetDirectoryName(FilePath);
            string name = Path.GetFileNameWithoutExtension(FilePath);

            await Editor.RunOperationAsync(
                "Saving skeleton...", "Skeleton saved.", 
                async (p, c) => await skeleton.ExportAsync(dir, name, ESerializeFlags.Default, EFileFormat.XML, null, p, c.Token));
        }
        private async void ImportAsStaticMesh()
        {
            ColladaImportOptions o = new ColladaImportOptions()
            {
                IgnoreFlags =
                    Collada.EIgnoreFlags.Extra |
                    Collada.EIgnoreFlags.Controllers |
                    Collada.EIgnoreFlags.Cameras |
                    Collada.EIgnoreFlags.Lights
            };

            var data = await Editor.RunOperationAsync(
                $"Importing {Path.GetFileName(FilePath)} as static model...", "Model imported.", 
                async (p, c) => await Collada.ImportAsync(FilePath, o, p, c.Token));

            if (data is null || data.Models.Count == 0)
                return;

            TFileObject staticModel = data.Models[0].StaticModel;
            if (staticModel is null)
                return;
            
            string dir = Path.GetDirectoryName(FilePath);
            string name = Path.GetFileNameWithoutExtension(FilePath);

            await Editor.RunOperationAsync(
                "Saving model...", "Model saved.",
                async (p, c) => await staticModel.ExportAsync(dir, name, ESerializeFlags.Default, EFileFormat.XML, null, p, c.Token));

        }
        private async void ImportAsSkeletalMesh()
        {
            ColladaImportOptions o = new ColladaImportOptions()
            {
                IgnoreFlags =
                    Collada.EIgnoreFlags.Extra |
                    Collada.EIgnoreFlags.Cameras |
                    Collada.EIgnoreFlags.Lights
            };

            var data = await Editor.RunOperationAsync(
                 $"Importing {Path.GetFileName(FilePath)} as static model...", "Model imported.",
                 async (p, c) => await Collada.ImportAsync(FilePath, o, p, c.Token));

            if (data is null || data.Models.Count == 0)
                return;

            TFileObject skeletalModel = data.Models[0].SkeletalModel;
            if (skeletalModel is null)
                return;
            
            string dir = Path.GetDirectoryName(FilePath);
            string name = Path.GetFileNameWithoutExtension(FilePath);

            await Editor.RunOperationAsync(
                "Saving model...", "Model saved.",
                async (p, c) => await skeletalModel.ExportAsync(dir, name, ESerializeFlags.Default, EFileFormat.XML, null, p, c.Token));
        }
    }
}