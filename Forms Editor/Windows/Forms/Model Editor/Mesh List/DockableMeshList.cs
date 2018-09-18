using System;
using System.Windows.Forms;
using TheraEngine.Actors;
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

            if (comp?.Model == null)
                return;

            var rigidMeshes = comp.Model.RigidChildren;
            if (!(splitContainer1.Panel1Collapsed = rigidMeshes.Count == 0))
                for (int i = 0; i < rigidMeshes.Count; ++i)
                    lstRigid.Items.Add(new ListViewItem(rigidMeshes[i].Name) { Tag = (rigidMeshes[i], comp.Meshes[i]) });

            var softMeshes = comp.Model.SoftChildren;
            if (!(splitContainer1.Panel2Collapsed = softMeshes.Count == 0))
                for (int i = 0; i < softMeshes.Count; ++i)
                    lstSoft.Items.Add(new ListViewItem(softMeshes[i].Name) { Tag = (softMeshes[i], comp.Meshes[i + rigidMeshes.Count]) });
        }
        public void DisplayMeshes(SkeletalMeshComponent comp)
        {
            lstRigid.Items.Clear();
            lstSoft.Items.Clear();

            if (comp?.Model == null)
                return;

            var rigidMeshes = comp.Model.RigidChildren;
            if (!(splitContainer1.Panel1Collapsed = rigidMeshes.Count == 0))
                for (int i = 0; i < rigidMeshes.Count; ++i)
                    lstRigid.Items.Add(new ListViewItem(rigidMeshes[i].Name) { Tag = rigidMeshes[i] });

            var softMeshes = comp.Model.SoftChildren;
            if (!(splitContainer1.Panel2Collapsed = softMeshes.Count == 0))
                for (int i = 0; i < softMeshes.Count; ++i)
                    lstSoft.Items.Add(new ListViewItem(softMeshes[i].Name) { Tag = softMeshes[i] });
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
                if (mesh is StaticRigidSubMesh staticMesh)
                    f.MeshEditor.SetMesh(staticMesh);
                else if (mesh is SkeletalRigidSubMesh skelMesh)
                    f.MeshEditor.SetMesh(skelMesh);
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
                if (mesh is StaticSoftSubMesh staticMesh)
                    f.MeshEditor.SetMesh(staticMesh);
                else if (mesh is SkeletalSoftSubMesh skelMesh)
                    f.MeshEditor.SetMesh(skelMesh);
            }
        }
    }
}
