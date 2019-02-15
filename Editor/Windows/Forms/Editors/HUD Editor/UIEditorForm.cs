using System;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Rendering.UI;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(IUserInterface))]
    public partial class HudEditorForm : TheraForm, IDockPanelOwner
    {
        DockPanel IDockPanelOwner.DockPanelRef => dockPanel1;

        public HudEditorForm()
        {
            InitializeComponent();
            dockPanel1.Theme = new TheraEditorTheme();
            FormTitle2.MouseDown += TitleBar_MouseDown;
            FormTitle2.MouseUp += (s, e) => { if (e.Button == System.Windows.Forms.MouseButtons.Right && FormTitle.ClientRectangle.Contains(e.Location)) ShowSystemMenu(MouseButtons); };
            HUDGraph = new DockableFormInstance<DockableHudGraph>(x => x.Show(dockPanel1, DockState.Document));
            HUDProps = new DockableFormInstance<DockablePropertyGrid>(x => x.Show(dockPanel1, DockState.DockRight));
        }
        public HudEditorForm(IUserInterface manager) : this()
        {
            TargetHUD = manager;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            HUDGraph.Form.Focus();
            HUDGraph.Form.RenderPanel.UI.UIComponentSelected += UI_SelectedComponentChanged;
        }
        public DockableFormInstance<DockableHudGraph> HUDGraph { get; }
        public DockableFormInstance<DockablePropertyGrid> HUDProps { get; }
        public IUserInterface TargetHUD
        {
            get => HUDGraph.Form.RenderPanel.UI.TargetHUD;
            set
            {
                HUDGraph.Form.RenderPanel.UI.TargetHUD = value;
                HUDProps.Form.PropertyGrid.TargetObject = value;
                FormTitle2.Text = value != null ? value.Name + " [" + value.FilePath + "]" : string.Empty;
            }
        }

        private void UI_SelectedComponentChanged(UIComponent comp)
        {
            //MaterialFunctionProperties.TargetFunc = func;
        }
    }
}
