using System;
using System.Windows.Forms;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Rendering.Models;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMeshList : DockContent
    {
        public DockableMeshList()
        {
            InitializeComponent();
        }

        public void DisplayMeshes(StaticMeshComponent comp)
        {
            lstRigid.Items.Clear();
            lstSoft.Items.Clear();

            if (comp?.Model is null)
                return;

            Tuple<StaticRigidSubMesh, StaticRenderableMesh> rigidMeshTag;
            Tuple<StaticSoftSubMesh, StaticRenderableMesh> softMeshTag;

            var rigidMeshes = comp.Model.RigidChildren;
            int rMeshCount = rigidMeshes.Count;
            if (!(splitContainer1.Panel1Collapsed = rMeshCount == 0))
                for (int i = 0; i < rigidMeshes.Count; ++i)
                {
                    var mesh = rigidMeshes[i];
                    var rmesh = comp.Meshes[i];
                    rigidMeshTag = new Tuple<StaticRigidSubMesh, StaticRenderableMesh>(mesh, rmesh);
                    lstRigid.Items.Add(new ListViewItem(mesh.Name) { Tag = rigidMeshTag });
                }

            var softMeshes = comp.Model.SoftChildren;
            int sMeshCount = softMeshes.Count;
            if (!(splitContainer1.Panel2Collapsed = sMeshCount == 0))
                for (int i = 0; i < softMeshes.Count; ++i)
                {
                    var mesh = softMeshes[i];
                    var rmesh = comp.Meshes[i + rMeshCount];
                    softMeshTag = new Tuple<StaticSoftSubMesh, StaticRenderableMesh>(mesh, rmesh);
                    lstSoft.Items.Add(new ListViewItem(mesh.Name) { Tag = softMeshTag });
                }
        }
        public void DisplayMeshes(SkeletalMeshComponent comp)
        {
            lstRigid.Items.Clear();
            lstSoft.Items.Clear();

            if (comp?.Model is null)
                return;

            Tuple<SkeletalRigidSubMesh, SkeletalRenderableMesh> rigidMeshTag;
            Tuple<SkeletalSoftSubMesh, SkeletalRenderableMesh> softMeshTag;

            var rigidMeshes = comp.Model.RigidChildren;
            if (!(splitContainer1.Panel1Collapsed = rigidMeshes.Count == 0))
                for (int i = 0; i < rigidMeshes.Count; ++i)
                {
                    rigidMeshTag = new Tuple<SkeletalRigidSubMesh, SkeletalRenderableMesh>(rigidMeshes[i], comp.Meshes[i]);
                    lstRigid.Items.Add(new ListViewItem(rigidMeshes[i].Name) { Tag = rigidMeshTag });
                }

            var softMeshes = comp.Model.SoftChildren;
            if (!(splitContainer1.Panel2Collapsed = softMeshes.Count == 0))
                for (int i = 0; i < softMeshes.Count; ++i)
                {
                    softMeshTag = new Tuple<SkeletalSoftSubMesh, SkeletalRenderableMesh>(softMeshes[i], comp.Meshes[i + rigidMeshes.Count]);
                    lstSoft.Items.Add(new ListViewItem(softMeshes[i].Name) { Tag = softMeshTag });
                }
        }

        private void lstRigid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(DockPanel.FindForm() is ModelEditorForm f))
                return;
            if (lstRigid.SelectedItems.Count == 0)
                f.MeshEditor.ClearMesh();
            else
            {
                object mesh = lstRigid.SelectedItems[0].Tag;
                if (mesh is Tuple<StaticRigidSubMesh, StaticRenderableMesh> staticMesh)
                    f.MeshEditor.SetMesh(staticMesh.Item1, staticMesh.Item2);
                else if (mesh is Tuple<SkeletalRigidSubMesh, SkeletalRenderableMesh> skelMesh)
                    f.MeshEditor.SetMesh(skelMesh.Item1, skelMesh.Item2);
            }
        }

        private void lstSoft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(DockPanel.FindForm() is ModelEditorForm f))
                return;
            if (lstSoft.SelectedItems.Count == 0)
                f.MeshEditor.ClearMesh();
            else
            {
                object mesh = lstRigid.SelectedItems[0].Tag;
                if (mesh is Tuple<StaticSoftSubMesh, StaticRenderableMesh> staticMesh)
                    f.MeshEditor.SetMesh(staticMesh.Item1, staticMesh.Item2);
                else if (mesh is Tuple<SkeletalSoftSubMesh, SkeletalRenderableMesh> skelMesh)
                    f.MeshEditor.SetMesh(skelMesh.Item1, skelMesh.Item2);
            }
        }
    }
}
