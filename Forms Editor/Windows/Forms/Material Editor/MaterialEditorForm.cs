using System;
using TheraEngine;
using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(TMaterial))]
    public partial class MaterialEditorForm : TheraForm
    {
        public MaterialEditorForm()
        {
            InitializeComponent();
            dockPanel1.Theme = new TheraEditorTheme();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MaterialGraph.Focus();
            MaterialFunctions.Focus();
        }
        private DockableMatFuncList _matFuncs = null;
        public DockableMatFuncList MaterialFunctions
        {
            get
            {
                if (_matFuncs == null || _matFuncs.IsDisposed)
                {
                    Engine.PrintLine("Created material functions form");
                    _matFuncs = new DockableMatFuncList();
                    _matFuncs.Show(dockPanel1, DockState.DockRight);
                }
                return _matFuncs;
            }
        }
        private DockableMatGraph _materialGraph = null;
        public DockableMatGraph MaterialGraph
        {
            get
            {
                if (_materialGraph == null || _materialGraph.IsDisposed)
                {
                    Engine.PrintLine("Created material graph viewport");
                    _materialGraph = new DockableMatGraph();
                    _materialGraph.Show(dockPanel1, DockState.Document);
                }
                return _materialGraph;
            }
        }
        public MaterialEditorForm(TMaterial m) : this()
        {
            Material = m;
        }
        public TMaterial Material
        {
            get => MaterialGraph.RenderPanel.UI.TargetMaterial;
            set => MaterialGraph.RenderPanel.UI.TargetMaterial = value;
        }
    }
}
