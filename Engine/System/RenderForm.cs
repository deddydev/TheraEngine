using TheraEngine;
using System.Threading;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.IO;
using TheraEngine.Worlds;
using TheraEngine.Input;

namespace TheraEngine
{
    public partial class RenderForm : Form
    {
        public RenderForm(Game game)
        {
            Engine.SetGame(game);
            InitializeComponent();
            Engine.Initialize();

            Text = game.Name;
            if (!string.IsNullOrEmpty(game.IconPath) && File.Exists(game.IconPath))
                Icon = new Icon(game.IconPath);

            //TopMost = true;
            //FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            renderPanel1.RegisterTick();
            Engine.Run();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            renderPanel1.UnregisterTick();
            Engine.ShutDown();
        }
    }
}
