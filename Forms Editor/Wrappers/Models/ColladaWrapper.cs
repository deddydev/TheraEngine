using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Files;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Wrappers
{
    [NodeWrapper("dae")]
    public class ColladaWrapper : ThirdPartyFileWrapper
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static ColladaWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            _menu.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.O));   //1
            ToolStripMenuItem importItem = new ToolStripMenuItem("Import As...", null);
            ToolStripMenuItem skeletalMeshImportItem    = new ToolStripMenuItem("Skeletal Model",   null, ImportAsSkeletalMeshAction);
            ToolStripMenuItem staticMeshImportItem      = new ToolStripMenuItem("Static Model",     null, ImportAsStaticMeshAction);
            ToolStripMenuItem skeletonImportItem        = new ToolStripMenuItem("Skeleton",         null, ImportAsSkeletonAction);
            ToolStripMenuItem actorImportItem           = new ToolStripMenuItem("Actor",            null, ImportAsActorAction);
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
        
        public ColladaWrapper() : base(_menu) { }
        
        private async void ImportAsActor()
        {
            TFileObject actor = await Actor.LoadDAEAsync(FilePath);
            string dir = Path.GetDirectoryName(FilePath);
            string name = Path.GetFileNameWithoutExtension(FilePath);
            actor.Export(dir, name, FileFormat.XML);
        }
        private void ImportAsSkeleton()
        {
            //TFileObject actor = Skeleton.LoadDAE(FilePath);
            //string dir = Path.GetDirectoryName(FilePath);
            //string name = Path.GetFileNameWithoutExtension(FilePath);
            //actor.Export(dir, name, FileFormat.XML);
        }
        private async void ImportAsStaticMesh()
        {
            TFileObject staticModel = await StaticModel.LoadDAEAsync(FilePath);
            string dir = Path.GetDirectoryName(FilePath);
            string name = Path.GetFileNameWithoutExtension(FilePath);
            staticModel.Export(dir, name, FileFormat.XML);
        }
        private async void ImportAsSkeletalMesh()
        {
            TFileObject skeletalModel = await SkeletalModel.LoadDAEAsync(FilePath);
            string dir = Path.GetDirectoryName(FilePath);
            string name = Path.GetFileNameWithoutExtension(FilePath);
            skeletalModel.Export(dir, name, FileFormat.XML);
        }
    }
}