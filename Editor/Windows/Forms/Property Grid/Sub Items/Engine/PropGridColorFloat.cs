using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(
        typeof(ColorF4),
        typeof(EventColorF4),
        typeof(ColorF3),
        typeof(EventColorF3))]
    public partial class PropGridFloatColor : PropGridItem
    {
        public PropGridFloatColor()
        {
            InitializeComponent();
        }
        
        object _previousColor;
        EventColorF3 _colorF3;
        EventColorF4 _colorF4;

        protected override bool UpdateDisplayInternal(object value)
        {
            switch (value)
            {
                case ColorF3 colorF3:
                    pnlColorPreview.BackColor = colorControl.Color = colorF3.Color;

                    colorControl.EditAlpha = false;
                    btnShowSelector.Enabled = true;
                    pnlColorPreview.Enabled = true;
                    break;
                case ColorF4 colorF4:
                    pnlColorPreview.BackColor = colorControl.Color = colorF4.Color;

                    colorControl.EditAlpha = true;
                    btnShowSelector.Enabled = true;
                    pnlColorPreview.Enabled = true;
                    break;
                case EventColorF3 ecolorF3:
                    _colorF3 = ecolorF3;

                    colorControl.Color = pnlColorPreview.BackColor = _colorF3.Color;

                    colorControl.EditAlpha = false;
                    btnShowSelector.Enabled = true;
                    pnlColorPreview.Enabled = true;
                    break;
                case EventColorF4 ecolorF4:
                    _colorF4 = ecolorF4;

                    colorControl.Color = pnlColorPreview.BackColor = _colorF4.Color;

                    colorControl.EditAlpha = true;
                    btnShowSelector.Enabled = true;
                    pnlColorPreview.Enabled = true;
                    break;
                default:
                    _colorF3 = null;
                    _colorF4 = null;
                    colorControl.Color = pnlColorPreview.BackColor = Color.Black;
                    colorControl.EditAlpha = false;
                    btnShowSelector.Enabled = false;
                    pnlColorPreview.Enabled = false;
                    break;
            }

            return false;
        }

        private void btnShowSelector_Click(object sender, EventArgs e)
        {
            if (colorControl.Visible)
            {
                colorControl.Visible = false;
                btnShowSelector.Text = "▼";
                IsEditing = false;
            }
            else
            {
                _previousColor = GetValue();
                colorControl.Color = ((IByteColor)_previousColor).Color;
                colorControl.Visible = true;
                btnShowSelector.Text = "▲";
                IsEditing = true;
            }
        }

        private void colorControl1_Closed(object sender, EventArgs e)
        {
            if (colorControl.DialogResult == DialogResult.OK)
            {
                if (DataType == typeof(ColorF3))
                {
                    Color color = colorControl.Color;
                    pnlColorPreview.BackColor = color;
                    UpdateValue((ColorF3)color, true);
                }
                else if (DataType == typeof(ColorF4))
                {
                    Color color = colorControl.Color;
                    pnlColorPreview.BackColor = color;
                    UpdateValue((ColorF4)color, true);
                }
                else if (DataType == typeof(EventColorF3))
                {
                    Color color = colorControl.Color;
                    SubmitPreManualStateChange(_colorF3, nameof(_colorF3.Raw));
                    _colorF3.Color = color;
                    SubmitPostManualStateChange(_colorF3, nameof(_colorF3.Raw));
                    pnlColorPreview.BackColor = color;
                }
                else if (DataType == typeof(EventColorF4))
                {
                    Color color = colorControl.Color;
                    SubmitPreManualStateChange(_colorF4, nameof(_colorF4.Raw));
                    _colorF4.Color = color;
                    SubmitPostManualStateChange(_colorF4, nameof(_colorF4.Raw));
                    pnlColorPreview.BackColor = color;
                }
            }
            else
            {
                //Revert color
                if (DataType == typeof(ColorF3))
                {
                    ColorF3 color = (ColorF3)_previousColor;
                    pnlColorPreview.BackColor = color.Color;
                    UpdateValue(color, false);
                }
                else if (DataType == typeof(ColorF4))
                {
                    ColorF4 color = (ColorF4)_previousColor;
                    pnlColorPreview.BackColor = colorControl.Color;
                    UpdateValue(color, false);
                }
                else if (DataType == typeof(EventColorF3))
                {
                    ColorF3 color = ((EventColorF3)_previousColor).Raw;
                    _colorF3.Raw = color;
                    pnlColorPreview.BackColor = color.Color;
                }
                else if (DataType == typeof(EventColorF4))
                {
                    ColorF4 color = ((EventColorF4)_previousColor).Raw;
                    _colorF4.Raw = color;
                    pnlColorPreview.BackColor = color.Color;
                }
            }
            colorControl.Visible = false;
            btnShowSelector.Text = "▼";
            IsEditing = false;
        }
        
        private void colorControl1_OnColorChanged(Color newColor)
        {
            if (DataType == typeof(ColorF3))
            {
                ColorF3 color = (ColorF3)newColor;
                pnlColorPreview.BackColor = color.Color;
                UpdateValue(color, false);
            }
            else if (DataType == typeof(ColorF4))
            {
                ColorF4 color = newColor;
                pnlColorPreview.BackColor = newColor;
                UpdateValue(color, false);
            }
            else if (DataType == typeof(EventColorF3))
            {
                if (_colorF3 != null)
                    _colorF3.Color = newColor;
                pnlColorPreview.BackColor = newColor;
            }
            else if (DataType == typeof(EventColorF4))
            {
                if (_colorF4 != null)
                    _colorF4.Color = newColor;
                pnlColorPreview.BackColor = newColor;
            }
        }
    }
}
