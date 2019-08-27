using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Core;

namespace TheraEngine
{
    public partial class RenderForm : Form
    {
        public RenderForm(TGame game)
        {
            Engine.Instance.SetDomainProxy<EngineDomainProxy>(AppDomain.CurrentDomain, game.FilePath);

            InitializeComponent();
            Engine.SetWorldPanel(renderPanel);

            Text = game.Name;
            Icon icon = game.GetIcon();
            if (icon != null)
                Icon = icon;
            
            switch (game.UserSettingsRef.File.WindowBorderStyle)
            {
                case WindowBorderStyle.None:
                    FormBorderStyle = FormBorderStyle.None;
                    break;
                case WindowBorderStyle.Fixed:
                    FormBorderStyle = FormBorderStyle.FixedSingle;
                    break;
                case WindowBorderStyle.Sizable:
                    FormBorderStyle = FormBorderStyle.Sizable;
                    break;
            }

            if (game.UserSettingsRef.File.FullScreen)
                WindowState = FormWindowState.Maximized;

#if !DEBUG
            Cursor.Clip = renderPanel.RectangleToScreen(renderPanel.ClientRectangle);
            Cursor.Hide();
#endif

            Application.ApplicationExit += Application_ApplicationExit;
        }
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Engine.Stop();
            Engine.ShutDown();
        }
#if !DEBUG
        protected override void OnSizeChanged(EventArgs e)
        {
            Cursor.Clip = renderPanel.RectangleToScreen(renderPanel.ClientRectangle);
            base.OnSizeChanged(e);
        }
#endif
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Engine.Run();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            renderPanel.UnregisterTick();
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Engine.FocusChanged();
        }
        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            Engine.FocusChanged();
        }
    }
}
