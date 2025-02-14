﻿namespace System.Windows.Forms
{
    public delegate void ValueChange(float value);
    public partial class MinMaxControl : UserControl
    {
        public const float Precision = 0.001f;
        public event ValueChange ValueChanged;

        public MinMaxControl(float min, float max, float current)
        {
            InitializeComponent();

            float invPrecision = 1.0f / Precision;
            trackBar1.Minimum = (int)(min * invPrecision);
            trackBar1.Maximum = (int)(max * invPrecision);
            trackBar1.SmallChange = (int)invPrecision;
            trackBar1.LargeChange = (int)invPrecision;
            trackBar1.Value = (int)(current * invPrecision);
            trackBar1.ValueChanged += TrackBar1_ValueChanged;
            lblMinvalue.Text = min.ToString();
            lblMaxValue.Text = max.ToString();
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(Value);
        }

        public float Value => trackBar1.Value * Precision;

        public DialogResult DialogResult = DialogResult.OK;
        public event EventHandler Closed;

        private void button1_Click(object sender, EventArgs e)
        {
            Closed?.Invoke(this, null);
        }
    }
}
