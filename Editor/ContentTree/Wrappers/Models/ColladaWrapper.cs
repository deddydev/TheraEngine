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
        public ColladaWrapper() 
        {
            Menu.RemoveAt(3); //Remove edit option
            Menu.Insert(3, new TMenuOption("Import As...", null, Keys.None)
            {
                new TMenuOption("Skeletal Mesh", ImportAsSkeletalMesh, Keys.None),
                new TMenuOption("Static Mesh", ImportAsStaticMesh, Keys.None),
                new TMenuOption("Skeleton", ImportAsSkeleton, Keys.None),
                new TMenuOption("Actor", ImportAsActor, Keys.None),
            });
        }

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