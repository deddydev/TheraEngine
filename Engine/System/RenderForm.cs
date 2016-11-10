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
            Initialize(name, iconPath, null);
        }
        public RenderForm(string name, string iconPath, World world)
        {
            Initialize(name, iconPath, world);
        }

        public void Initialize(string name, string iconPath, World startupWorld)
        {
            InitializeComponent();

            Text = name;
            if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
                Icon = new Icon(iconPath);

            Engine.RegisterRenderTick(RenderTick);
            Engine.Initialize(startupWorld);
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
