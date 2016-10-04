using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomEngine;
using System.Threading;
using CustomEngine.Worlds;

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
