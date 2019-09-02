using System;
using System.Windows.Forms;
using TheraEngine;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public abstract class DockableUIEditor<T> : DockContent 
        where T : class, IUIRenderHandler
    {
        public RenderPanel<T> RenderPanel { get; private set; }
        public abstract bool ShouldHideCursor { get; }

        public DockableUIEditor()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RenderPanel.RenderHandler.FormShown();
        }
        protected override void OnClosed(EventArgs e)
        {
            RenderPanel.RenderHandler.FormClosed();
            base.OnClosed(e);
        }
        private void RenderPanel_MouseEnter(object sender, EventArgs e)
        {
            if (ShouldHideCursor)
                Cursor.Hide();
        }
        private void RenderPanel_MouseLeave(object sender, EventArgs e)
        {
            if (ShouldHideCursor)
                Cursor.Show();
        }

        private void InitializeComponent()
        {
            RenderPanel = new RenderPanel<T>();

            SuspendLayout();

            RenderPanel.AllowDrop = true;
            RenderPanel.Dock = DockStyle.Fill;
            RenderPanel.Location = new System.Drawing.Point(0, 0);
            RenderPanel.Margin = new Padding(0);
            RenderPanel.Name = "RenderPanel";
            RenderPanel.Size = new System.Drawing.Size(378, 332);
            RenderPanel.TabIndex = 1;
            RenderPanel.MouseEnter += RenderPanel_MouseEnter;
            RenderPanel.MouseLeave += RenderPanel_MouseLeave;

            ResumeLayout(false);
        }
    }
}
