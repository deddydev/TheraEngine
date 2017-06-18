using TheraEngine;
using System.Threading;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.IO;
using TheraEngine.Worlds;

namespace TheraEngine
{
    public partial class RenderForm : Form
    {
        public RenderForm(Game game)
        {
            InitializeComponent();

            Text = game.Name;
            if (!string.IsNullOrEmpty(game.IconPath) && File.Exists(game.IconPath))
                Icon = new Icon(game.IconPath);

            //TopMost = true;
            //FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            renderPanel1.RegisterTick();
            Engine.Initialize(game);
        }
        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //}
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            renderPanel1.UnregisterTick();
            Engine.ShutDown();
        }
    }
}
