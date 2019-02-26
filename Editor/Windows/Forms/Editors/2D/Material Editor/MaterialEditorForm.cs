using System;
using TheraEngine;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    //[EditorFor(typeof(TMaterial))]
    public partial class MaterialEditorForm : TheraForm, IDockPanelOwner
    {
        DockPanel IDockPanelOwner.DockPanelRef => dockPanel1;

        public MaterialEditorForm()
        {
            InitializeComponent();
            dockPanel1.Theme = new TheraEditorTheme();
            FormTitle2.MouseDown += TitleBar_MouseDown;
            FormTitle2.MouseUp += (s, e) => 
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right && FormTitle.ClientRectangle.Contains(e.Location))
                    ShowSystemMenu(MouseButtons);
            };
        }
        public MaterialEditorForm(TMaterial m) : this()
        {
            Material = m;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MaterialGraph.Focus();
            MaterialFunctions.Focus();
            MaterialFunctionProperties.Focus();
            MaterialProperties.Focus();
            MaterialGraph.RenderPanel.UI.SelectedFunctionChanged += UI_SelectedFunctionChanged;
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

        private DockableTexRefControl _texRefForm;
        public bool TexRefFormActive => _texRefForm != null;
        public DockableTexRefControl TexRefForm
        {
            get
            {
                if (_texRefForm == null || _texRefForm.IsDisposed)
                {
                    Engine.PrintLine("Created texture reference editor");
                    _texRefForm = new DockableTexRefControl();
                    _texRefForm.Show(dockPanel1, DockState.DockRight);
                }
                return _texRefForm;
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
        private DockableMatFuncProps _matFuncProps = null;
        public DockableMatFuncProps MaterialFunctionProperties
        {
            get
            {
                if (_matFuncProps == null || _matFuncProps.IsDisposed)
                {
                    Engine.PrintLine("Created material function property grid");
                    _matFuncProps = new DockableMatFuncProps();
                    _matFuncProps.Show(dockPanel1, DockState.DockLeft);
                }
                return _matFuncProps;
            }
        }
        private DockableMatProps _matProps = null;
        public DockableMatProps MaterialProperties
        {
            get
            {
                if (_matProps == null || _matProps.IsDisposed)
                {
                    Engine.PrintLine("Created material property grid");
                    _matProps = new DockableMatProps();
                    _matProps.Show(MaterialFunctionProperties.Pane, DockAlignment.Top, 0.5);
                }
                return _matProps;
            }
        }
        public TMaterial Material
        {
            get => MaterialGraph.RenderPanel.UI.TargetMaterial;
            set
            {
                MaterialGraph.RenderPanel.UI.TargetMaterial = value;
                MaterialProperties.TargetMaterial = value;
                FormTitle2.Text = value != null ? value.Name + " [" + value.FilePath + "]" : string.Empty;
            }
        }

        private void UI_SelectedFunctionChanged(MaterialFunction func)
        {
            //MaterialFunctionProperties.TargetFunc = func;
        }
    }
}
