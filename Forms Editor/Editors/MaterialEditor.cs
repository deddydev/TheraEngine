﻿using System.Drawing;
using System.Windows.Forms;

namespace TheraEditor.Editors
{
    public partial class MaterialEditor : UserControl
    {
        public MaterialEditor()
        {
            InitializeComponent();
            renderPanel1.GlobalHud = new MaterialEditorHud(renderPanel1);
            renderPanel1.BackColor = Color.Black;
        }
    }
}
