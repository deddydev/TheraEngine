using CustomEngine;
using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor
{
    public partial class Editor : Form
    {
        public Editor()
        {
            InitializeComponent();
            panel3.Controls.Add(new RenderPanel());
        }

        private static Editor _instance;
        public static Editor Instance { get { return _instance ?? new Editor(); } }
    }
}
