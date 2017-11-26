using System.Windows.Forms;
using System;
using System.Drawing;
using System.IO;

namespace TheraEngine
{
    public partial class RenderForm : Form
    {
        public RenderForm(Game game)
        {
            Engine.SetGame(game);
            InitializeComponent();
            Engine.SetGamePanel(renderPanel1);
            Engine.Initialize();

            Text = game.Name;
            if (!string.IsNullOrEmpty(game.IconPath) && File.Exists(game.IconPath))
                Icon = new Icon(game.IconPath);
            
            switch (game.UserSettings.File.WindowBorderStyle)
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

            Cursor.Clip = renderPanel1.RectangleToScreen(renderPanel1.ClientRectangle);

            if (game.UserSettings.File.FullScreen)
                WindowState = FormWindowState.Maximized;

            Cursor.Hide();

            Application.ApplicationExit += Application_ApplicationExit;
        }
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Engine.ShutDown();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            Cursor.Clip = renderPanel1.RectangleToScreen(renderPanel1.ClientRectangle);
            base.OnSizeChanged(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Engine.Run();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            renderPanel1.UnregisterTick();
        }
    }
}
