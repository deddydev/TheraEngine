using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public partial class ColorDialog : Form
    {
        public event ColorControl.ColorChangedEvent OnColorChanged;

        public Color Color
        {
            get { return goodColorControl21.Color; }
            set { goodColorControl21.Color = value; }
        }
        public bool EditAlpha
        {
            get { return goodColorControl21.EditAlpha; }
            set
            {
                if (goodColorControl21.EditAlpha = value)
                    this.Height = 287;
                else
                    this.Height = 267;
            }
        }
        public bool ShowOldColor
        {
            get { return goodColorControl21.ShowOldColor; }
            set { goodColorControl21.ShowOldColor = value; }
        }

        public ColorDialog()
        {
            InitializeComponent();
            goodColorControl21.Closed += goodColorControl21_Closed;
            goodColorControl21.OnColorChanged += goodColorControl21_ColorChanged;
        }

        void goodColorControl21_ColorChanged(Color c)
        {
            if (OnColorChanged != null)
                OnColorChanged(c);
        }

        void goodColorControl21_Closed(object sender, EventArgs e)
        {
            DialogResult = goodColorControl21.DialogResult;
            Close();
        }
    }
}
