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

namespace System.Windows.Forms
{
    public partial class RenderForm : Form
    {
        public RenderForm()
        {
            InitializeComponent();
            Engine.Run(60.0f);
        }
    }
}
