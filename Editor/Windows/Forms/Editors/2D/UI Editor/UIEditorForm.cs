using System;
using TheraEngine.Actors.Types.Pawns;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    //[EditorFor(typeof(IUserInterface))]
    public partial class UIEditorForm : TheraForm, IDockPanelOwner
    {
        DockPanel IDockPanelOwner.DockPanelRef => dockPanel1;

        public UIEditorForm()
        {
            InitializeComponent();

            dockPanel1.Theme = new TheraEditorTheme();
            FormTitle2.MouseDown += TitleBar_MouseDown;
            FormTitle2.MouseUp += (s, e) => 
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right && 
                    FormTitle.ClientRectangle.Contains(e.Location))
                    ShowSystemMenu(MouseButtons);
            };
            UIGraph = new DockableFormInstance<DockableUserInterfaceEditor>(x => x.Show(dockPanel1, DockState.Document));
            UIProps = new DockableFormInstance<DockablePropertyGrid>(x => x.Show(dockPanel1, DockState.DockRight));
        }

        public UIEditorForm(IUserInterface manager) : this()
            => TargetUI = manager;
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UIGraph.Form.Focus();
            //UIGraph.Form.RenderPanel.UI.UIComponentSelected += UI_SelectedComponentChanged;
        }
        public DockableFormInstance<DockableUserInterfaceEditor> UIGraph { get; }
        public DockableFormInstance<DockablePropertyGrid> UIProps { get; }
        public IUserInterface TargetUI
        {
            get => UIGraph.Form.RenderPanel.RenderHandler.UI.TargetUI;
            set
            {
                UIGraph.Form.RenderPanel.RenderHandler.UI.TargetUI = value;
                UIProps.Form.PropertyGrid.TargetObject = value;
                FormTitle2.Text = value != null ? value.Name + " [" + value.FilePath + "]" : string.Empty;
            }
        }

        //private void UI_SelectedComponentChanged(UIComponent comp)
        //{
        //    //MaterialFunctionProperties.TargetFunc = func;
        //}
    }
}
