using CustomEngine;
using System.Threading;
using System.Windows.Forms;

namespace CustomEngine
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
