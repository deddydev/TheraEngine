using System.Drawing;

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
                    Height = 287;
                else
                    Height = 267;
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
            goodColorControl21.ColorChanged += goodColorControl21_ColorChanged;
        }

        void goodColorControl21_ColorChanged(Color c)
        {
            OnColorChanged?.Invoke(c);
        }

        void goodColorControl21_Closed(object sender, EventArgs e)
        {
            DialogResult = goodColorControl21.DialogResult;
            Close();
        }
    }
}
