using System;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Mesh;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMeshList : DockContent
    {
        public DockableMeshList()
        {
            InitializeComponent();
        }

        public void DisplayMeshes(Actor<StaticMeshComponent> staticActor)
        {
            RigidMeshes.pnlMain.Controls.Clear();
            SoftMeshes.pnlMain.Controls.Clear();
            var rigidMeshes = staticActor.RootComponent.ModelRef.File.RigidChildren;
            for (int i = 0; i < rigidMeshes.Count; ++i)
            {
                MeshControl c = new MeshControl()
                {
                    Dock = DockStyle.Top,
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    //AutoSize = true,
                };
                c.SetMesh(rigidMeshes[i], i);
                RigidMeshes.pnlMain.Controls.Add(c);
            }
            var softMeshes = staticActor.RootComponent.ModelRef.File.SoftChildren;
            for (int i = 0; i < softMeshes.Count; ++i)
            {
                MeshControl c = new MeshControl()
                {
                    Dock = DockStyle.Top,
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    //AutoSize = true,
                };
                c.SetMesh(softMeshes[i], i);
                SoftMeshes.pnlMain.Controls.Add(c);
            }
        }
        public void DisplayMeshes(Actor<SkeletalMeshComponent> skeletalActor)
        {
            RigidMeshes.pnlMain.Controls.Clear();
            SoftMeshes.pnlMain.Controls.Clear();
            var rigidMeshes = skeletalActor.RootComponent.ModelRef.File.RigidChildren;
            for (int i = 0; i < rigidMeshes.Count; ++i)
            {
                MeshControl c = new MeshControl()
                {
                    Dock = DockStyle.Top,
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    AutoSize = true,
                };
                c.SetMesh(rigidMeshes[i], i);
                RigidMeshes.pnlMain.Controls.Add(c);
            }
            var softMeshes = skeletalActor.RootComponent.ModelRef.File.SoftChildren;
            for (int i = 0; i < softMeshes.Count; ++i)
            {
                MeshControl c = new MeshControl()
                {
                    Dock = DockStyle.Top,
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    AutoSize = true,
                };
                c.SetMesh(softMeshes[i], i);
                SoftMeshes.pnlMain.Controls.Add(c);
            }
        }
    }
}
