using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(ShaderVec3), typeof(ShaderVec4))]
    public partial class PropGridShaderVecColor : PropGridItem
    {
        public PropGridShaderVecColor()
        {
            InitializeComponent();
        }
        
        object _previousColor;
        ShaderVec3 _vec3;
        ShaderVec4 _vec4;

        protected override bool UpdateDisplayInternal(object value)
        {
            switch (value)
            {
                case ShaderVec3 ecolorF3:
                    _vec3 = ecolorF3;

                    colorControl.Color = pnlColorPreview.BackColor = _vec3.Color;

                    colorControl.EditAlpha = false;
                    btnShowSelector.Enabled = true;
                    pnlColorPreview.Enabled = true;
                    break;
                case ShaderVec4 ecolorF4:
                    _vec4 = ecolorF4;

                    colorControl.Color = pnlColorPreview.BackColor = _vec4.Color;

                    colorControl.EditAlpha = true;
                    btnShowSelector.Enabled = true;
                    pnlColorPreview.Enabled = true;
                    break;
                default:
                    _vec3 = null;
                    _vec4 = null;
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
                if (DataType == typeof(ShaderVec3))
                {
                    Color color = colorControl.Color;
                    SubmitPreManualStateChange(_vec3, nameof(_vec3.Value));
                    _vec3.Color = color;
                    SubmitPostManualStateChange(_vec3, nameof(_vec3.Value));
                    pnlColorPreview.BackColor = color;
                }
                else if (DataType == typeof(ShaderVec4))
                {
                    Color color = colorControl.Color;
                    SubmitPreManualStateChange(_vec4, nameof(_vec4.Value));
                    _vec4.Color = color;
                    SubmitPostManualStateChange(_vec4, nameof(_vec4.Value));
                    pnlColorPreview.BackColor = color;
                }
            }
            else
            {
                //Revert color
                if (DataType == typeof(ShaderVec3))
                {
                    Vec3 color = ((ShaderVec3)_previousColor).Value;
                    _vec3.Value = color;
                    pnlColorPreview.BackColor = color.Color;
                }
                else if (DataType == typeof(ShaderVec4))
                {
                    Vec4 color = ((ShaderVec4)_previousColor).Value;
                    _vec4.Value = color;
                    pnlColorPreview.BackColor = color.Color;
                }
            }
            colorControl.Visible = false;
            btnShowSelector.Text = "▼";
            IsEditing = false;
        }
        
        private void colorControl1_OnColorChanged(Color newColor)
        {
            if (DataType == typeof(ShaderVec3))
            {
                _vec3.Color = newColor;
                pnlColorPreview.BackColor = newColor;
            }
            else if (DataType == typeof(ShaderVec4))
            {
                _vec4.Color = newColor;
                pnlColorPreview.BackColor = newColor;
            }
        }
    }
}
