using CustomEngine;
using System.Threading;
using System.Windows.Forms;
using System;

namespace CustomEngine
{
    public partial class RenderForm : Form
    {
        Thread EngineThread;
        public RenderForm()
        {
            InitializeComponent();
            EngineThread = new Thread(Engine.Initialize);
            Engine.RegisterRenderTick(RenderTick);
            EngineThread.Start();
        }

        public void RenderTick(object sender, FrameEventArgs e)
        {
            
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            EngineThread.Abort();
            //Engine.Stop();
        }
    }
}
