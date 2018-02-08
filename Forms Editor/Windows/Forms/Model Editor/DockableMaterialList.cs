using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Rendering.Models;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMaterialList : DockContent
    {
        public DockableMaterialList()
        {
            InitializeComponent();
        }

        public void DisplayMaterials(Actor<StaticMeshComponent> staticActor)
        {
            if (staticActor.RootComponent.ModelRef?.File == null)
                return;

            HashSet<int> ids = new HashSet<int>();

            Controls.Clear();
            List<Control> controls = new List<Control>();
            
            var rigidMeshes = staticActor.RootComponent.Model.RigidChildren;
            for (int i = 0; i < rigidMeshes.Count; ++i)
            {
                for (int x = 0; x < rigidMeshes[i].LODs.Count; ++x)
                {
                    LOD lod = rigidMeshes[i].LODs[x];

                    if (lod.MaterialRef.File != null && !ids.Contains(lod.MaterialRef.File.UniqueID))
                    {
                        ids.Add(lod.MaterialRef.File.UniqueID);
                        MaterialControl c = new MaterialControl()
                        {
                            Dock = DockStyle.Top,
                            Margin = new Padding(0),
                            Padding = new Padding(0),
                            AutoSize = true,
                        };
                        c.Material = lod.MaterialRef.File;
                        controls.Add(c);
                    }
                }
            }
            var softMeshes = staticActor.RootComponent.Model.SoftChildren;
            for (int i = 0; i < softMeshes.Count; ++i)
            {
                for (int x = 0; x < softMeshes[i].LODs.Count; ++x)
                {
                    LOD lod = softMeshes[i].LODs[x];

                    if (lod.MaterialRef.File != null && !ids.Contains(lod.MaterialRef.File.UniqueID))
                    {
                        ids.Add(lod.MaterialRef.File.UniqueID);
                        MaterialControl c = new MaterialControl()
                        {
                            Dock = DockStyle.Top,
                            Margin = new Padding(0),
                            Padding = new Padding(0),
                            AutoSize = true,
                        };
                        c.Material = lod.MaterialRef.File;
                        controls.Add(c);
                    }
                }
            }
            
            Controls.AddRange(controls.OrderBy(x => x.Name).ToArray());
        }
        public void DisplayMaterials(Actor<SkeletalMeshComponent> skeletalActor)
        {
            if (skeletalActor.RootComponent.ModelRef?.File == null)
                return;

            HashSet<int> ids = new HashSet<int>();

            Controls.Clear();
            List<Control> controls = new List<Control>();

            var rigidMeshes = skeletalActor.RootComponent.Model.RigidChildren;
            for (int i = 0; i < rigidMeshes.Count; ++i)
            {
                for (int x = 0; x < rigidMeshes[i].LODs.Count; ++x)
                {
                    LOD lod = rigidMeshes[i].LODs[x];

                    if (lod.MaterialRef.File != null && !ids.Contains(lod.MaterialRef.File.UniqueID))
                    {
                        ids.Add(lod.MaterialRef.File.UniqueID);
                        MaterialControl c = new MaterialControl()
                        {
                            Dock = DockStyle.Top,
                            Margin = new Padding(0),
                            Padding = new Padding(0),
                            AutoSize = true,
                        };
                        c.Material = lod.MaterialRef.File;
                        controls.Add(c);
                    }
                }
            }
            var softMeshes = skeletalActor.RootComponent.Model.SoftChildren;
            for (int i = 0; i < softMeshes.Count; ++i)
            {
                for (int x = 0; x < softMeshes[i].LODs.Count; ++x)
                {
                    LOD lod = softMeshes[i].LODs[x];

                    if (lod.MaterialRef.File != null && !ids.Contains(lod.MaterialRef.File.UniqueID))
                    {
                        ids.Add(lod.MaterialRef.File.UniqueID);
                        MaterialControl c = new MaterialControl()
                        {
                            Dock = DockStyle.Top,
                            Margin = new Padding(0),
                            Padding = new Padding(0),
                            AutoSize = true,
                        };
                        c.Material = lod.MaterialRef.File;
                        controls.Add(c);
                    }
                }
            }

            Controls.AddRange(controls.OrderBy(x => x.Name).ToArray());
        }
    }
}
