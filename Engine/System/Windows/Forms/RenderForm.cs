using CustomEngine;
using System.Threading;

namespace System.Windows.Forms
{
    public partial class RenderForm : Form
    {
        Thread EngineThread;
        public RenderForm()
        {
            InitializeComponent();
            EngineThread = new Thread(Engine.Initialize);
            EngineThread.Start();
        }
    }
}
