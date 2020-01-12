using System;
using TheraEngine;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    //[EditorFor(typeof(TMaterial))]
    public partial class MaterialEditorForm : FileEditorTheraForm<TMaterial>, IDockPanelOwner
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
        public MaterialEditorForm(TMaterial file) : this() => File = file;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            MaterialGraph.Focus();
            MaterialFunctions.Focus();
            MaterialFunctionProperties.Focus();
            MaterialProperties.Focus();
            MaterialGraph.RenderPanel.RenderHandler.UI.SelectedFunctionChanged += UI_SelectedFunctionChanged;
        }
        private DockableMatFuncList _matFuncs = null;
        public DockableMatFuncList MaterialFunctions
        {
            get
            {
                if (_matFuncs is null || _matFuncs.IsDisposed)
                {
                    Engine.Out("Created material functions form");
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
                if (_texRefForm is null || _texRefForm.IsDisposed)
                {
                    Engine.Out("Created texture reference editor");
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
                if (_materialGraph is null || _materialGraph.IsDisposed)
                {
                    Engine.Out("Created material graph viewport");
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
                if (_matFuncProps is null || _matFuncProps.IsDisposed)
                {
                    Engine.Out("Created material function property grid");
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
                if (_matProps is null || _matProps.IsDisposed)
                {
                    Engine.Out("Created material property grid");
                    _matProps = new DockableMatProps();
                    _matProps.Show(MaterialFunctionProperties.Pane, DockAlignment.Top, 0.5);
                }
                return _matProps;
            }
        }
        protected override bool TrySetFile(TMaterial file)
        {
            if (!base.TrySetFile(file))
                return false;

            MaterialGraph.File = file;
            MaterialProperties.TargetMaterial = file;
            FormTitle2.Text = file != null ? file.Name + " [" + file.FilePath + "]" : string.Empty;

            return true;
        }
        private void UI_SelectedFunctionChanged(MaterialFunction func)
        {
            //MaterialFunctionProperties.TargetFunc = func;
        }
    }
}
