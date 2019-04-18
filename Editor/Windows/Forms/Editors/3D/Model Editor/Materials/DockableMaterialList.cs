using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMaterialList : DockContent
    {
        public DockableMaterialList()
        {
            InitializeComponent();
        }

        public void DisplayMaterials(StaticModel staticModel)
        {
            if (staticModel == null)
                return;

            HashSet<int> ids = new HashSet<int>();

            var rigidMeshes = staticModel.RigidChildren;
            for (int i = 0; i < rigidMeshes.Count; ++i)
            {
                for (int x = 0; x < rigidMeshes[i].LODs.Count; ++x)
                {
                    ILOD lod = rigidMeshes[i].LODs[x];

                    if (lod.MaterialRef.File != null && !ids.Contains(lod.MaterialRef.File.UniqueID))
                    {
                        ids.Add(lod.MaterialRef.File.UniqueID);
                        listView1.Items.Add(new ListViewItem(lod.MaterialRef.File.Name) { Tag = lod.MaterialRef.File });
                    }
                }
            }
            var softMeshes = staticModel.SoftChildren;
            for (int i = 0; i < softMeshes.Count; ++i)
            {
                for (int x = 0; x < softMeshes[i].LODs.Count; ++x)
                {
                    ILOD lod = softMeshes[i].LODs[x];

                    if (lod.MaterialRef.File != null && !ids.Contains(lod.MaterialRef.File.UniqueID))
                    {
                        ids.Add(lod.MaterialRef.File.UniqueID);
                        listView1.Items.Add(new ListViewItem(lod.MaterialRef.File.Name) { Tag = lod.MaterialRef.File });
                    }
                }
            }
        }
        public void DisplayMaterials(SkeletalModel skelModel)
        {
            if (skelModel == null)
                return;

            HashSet<int> ids = new HashSet<int>();
            
            var rigidMeshes = skelModel.RigidChildren;
            for (int i = 0; i < rigidMeshes.Count; ++i)
            {
                for (int x = 0; x < rigidMeshes[i].LODs.Count; ++x)
                {
                    ILOD lod = rigidMeshes[i].LODs[x];

                    if (lod.MaterialRef.File != null && !ids.Contains(lod.MaterialRef.File.UniqueID))
                    {
                        ids.Add(lod.MaterialRef.File.UniqueID);
                        listView1.Items.Add(new ListViewItem(lod.MaterialRef.File.Name) { Tag = lod.MaterialRef.File });
                    }
                }
            }
            var softMeshes = skelModel.SoftChildren;
            for (int i = 0; i < softMeshes.Count; ++i)
            {
                for (int x = 0; x < softMeshes[i].LODs.Count; ++x)
                {
                    ILOD lod = softMeshes[i].LODs[x];

                    if (lod.MaterialRef.File != null && !ids.Contains(lod.MaterialRef.File.UniqueID))
                    {
                        ids.Add(lod.MaterialRef.File.UniqueID);
                        listView1.Items.Add(new ListViewItem(lod.MaterialRef.File.Name) { Tag = lod.MaterialRef.File });
                    }
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModelEditorForm f = DockPanel.FindForm() as ModelEditorForm;
            if (f == null)
                return;
            if (listView1.SelectedItems.Count == 0)
            {
                f.MaterialEditor.SetMaterial(null);
            }
            else
            {
                if (listView1.SelectedItems[0].Tag is TMaterial mat)
                    f.MaterialEditor.SetMaterial(mat);
            }
        }
    }
}
