using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Editors
{
    public partial class MaterialEditor : UserControl
    {
        public MaterialEditor()
        {
            InitializeComponent();
            renderPanel1.GlobalHud = new MaterialEditorHud(renderPanel1);
        }
    }
}
