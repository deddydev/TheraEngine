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

            //TopMost = true;
            //FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            renderPanel1.RegisterTick();
            Engine.Initialize();
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
