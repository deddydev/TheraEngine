using System;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Rendering.UI;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(IUserInterface))]
    public partial class HudEditorForm : TheraForm
    {
        public HudEditorForm()
        {
            InitializeComponent();
            dockPanel1.Theme = new TheraEditorTheme();
            FormTitle2.MouseDown += TitleBar_MouseDown;
            FormTitle2.MouseUp += (s, e) => { if (e.Button == System.Windows.Forms.MouseButtons.Right && FormTitle.ClientRectangle.Contains(e.Location)) ShowSystemMenu(MouseButtons); };
        }
        public HudEditorForm(IUserInterface manager) : this()
        {
            TargetHUD = manager;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            HudGraph.Focus();
            HudGraph.RenderPanel.UI.UIComponentSelected += UI_SelectedComponentChanged;
        }
        private DockableHudGraph _materialGraph = null;
        public DockableHudGraph HudGraph
        {
            get
            {
                if (_materialGraph == null || _materialGraph.IsDisposed)
                {
                    Engine.PrintLine("Created hud graph viewport");
                    _materialGraph = new DockableHudGraph();
                    _materialGraph.Show(dockPanel1, DockState.Document);
                }
                return _materialGraph;
            }
        }
        public IUserInterface TargetHUD
        {
            get => HudGraph.RenderPanel.UI.TargetHUD;
            set
            {
                HudGraph.RenderPanel.UI.TargetHUD = value;
                FormTitle2.Text = value != null ? value.Name + " [" + value.FilePath + "]" : string.Empty;
            }
        }

        private void UI_SelectedComponentChanged(UIComponent comp)
        {
            //MaterialFunctionProperties.TargetFunc = func;
        }
    }
}
