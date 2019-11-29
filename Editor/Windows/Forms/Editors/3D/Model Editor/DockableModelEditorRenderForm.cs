using System;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableModelEditorRenderForm : DockableWorldRenderPanelBase<ModelEditorRenderHandler>
    {
        public DockableModelEditorRenderForm(ELocalPlayerIndex playerIndex, int formIndex, ModelEditorForm form) 
            : base(playerIndex, formIndex)
        {
            ModelWindow = form;

            InitializeComponent();

            Text = $"Model Viewport {(FormIndex + 1).ToString()}";
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            RenderPanel.LinkToWorldManager(ModelWindow.WorldManagerId);
            ModelWindow.WorldManagerChanged += ModelWindow_WorldManagerChanged;
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            ModelWindow.WorldManagerChanged -= ModelWindow_WorldManagerChanged;
            RenderPanel.UnlinkFromWorldManager();
        }

        private void ModelWindow_WorldManagerChanged()
            => RenderPanel.LinkToWorldManager(ModelWindow.WorldManagerId);
        
        public ModelEditorForm ModelWindow { get; private set; }
    }
}
