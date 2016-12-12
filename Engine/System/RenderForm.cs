using CustomEngine;
using System.Threading;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.IO;
using CustomEngine.Worlds;

namespace CustomEngine
{
    public partial class RenderForm : Form
    {
        public RenderForm(string name, string iconPath)
        {
            InitializeComponent();

            Text = name;
            if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
                Icon = new Icon(iconPath);

            renderPanel1._context.Initialize();

            Engine.RegisterRenderTick(RenderTick);
            Engine.Initialize();
        }
        public void RenderTick(object sender, FrameEventArgs e)
        {
            renderPanel1.Redraw();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Engine.ShutDown();
        }
    }
}
