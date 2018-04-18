using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMaterialEditor : DockContent
    {
        public DockableMaterialEditor()
        {
            InitializeComponent();
        }
        public void SetMaterial(TMaterial mat)
        {
            materialControl1.Material = mat;
        }
    }
}
