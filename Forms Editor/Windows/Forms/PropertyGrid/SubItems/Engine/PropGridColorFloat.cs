﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Files;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(
        typeof(ColorF4),
        typeof(EventColorF4),
        typeof(ColorF3),
        typeof(EventColorF3))]
    public partial class PropGridFloatColor : PropGridItem
    {
        public PropGridFloatColor() => InitializeComponent();
        
        object _previousColor;
        EventColorF3 _colorF3;
        EventColorF3 _colorF4;

        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            
            if (DataType == typeof(ColorF3))
            {
                ColorF3 color = (ColorF3)value;
                panel1.BackColor = color.Color;
                colorControl1.EditAlpha = false;
                colorControl1.Color = color.Color;
            }
            else if (DataType == typeof(ColorF4))
            {
                ColorF4 color = (ColorF4)value;
                panel1.BackColor = color.Color;
                colorControl1.EditAlpha = true;
                colorControl1.Color = color.Color;
            }
            else if (DataType == typeof(EventColorF3))
            {
                EventColorF3 color = (EventColorF3)value;
                panel1.BackColor = color.Color;
                colorControl1.EditAlpha = false;
                colorControl1.Color = color.Color;
            }
            else if (DataType == typeof(EventColorF4))
            {
                EventColorF4 color = (EventColorF4)value;
                panel1.BackColor = color.Color;
                colorControl1.EditAlpha = true;
                colorControl1.Color = color.Color;
            }
            else
                throw new Exception(DataType.GetFriendlyName() + " is not ColorF3 or ColorF4.");
        }

        private void btnShowSelector_Click(object sender, EventArgs e)
        {
            if (colorControl1.Visible)
            {
                colorControl1.Visible = false;
                btnShowSelector.Text = "▼";
                IsEditing = false;
            }
            else
            {
                _previousColor = GetValue();
                colorControl1.Color = ((IByteColor)_previousColor).Color;
                colorControl1.Visible = true;
                btnShowSelector.Text = "▲";
                IsEditing = true;
            }
        }

        private void colorControl1_Closed(object sender, EventArgs e)
        {
            if (colorControl1.DialogResult == DialogResult.OK)
            {
                if (DataType == typeof(ColorF3))
                {
                    Color color = colorControl1.Color;
                    panel1.BackColor = color;
                    UpdateValue((ColorF3)color);
                }
                else if (DataType == typeof(ColorF4))
                {
                    Color color = colorControl1.Color;
                    panel1.BackColor = color;
                    UpdateValue((ColorF4)color);
                }
            }
            else
            {
                if (DataType == typeof(ColorF3))
                {
                    ColorF3 color = (ColorF3)_previousColor;
                    panel1.BackColor = color.Color;
                    UpdateValue(color);
                }
                else if (DataType == typeof(ColorF4))
                {
                    ColorF4 color = (ColorF4)_previousColor;
                    panel1.BackColor = colorControl1.Color;
                    UpdateValue(color);
                }
            }
            colorControl1.Visible = false;
            btnShowSelector.Text = "▼";
            IsEditing = false;
        }
        
        private void colorControl1_OnColorChanged(Color newColor)
        {
            if (DataType == typeof(ColorF3))
            {
                ColorF3 color = (ColorF3)newColor;
                panel1.BackColor = color.Color;
                UpdateValue(color);
            }
            else if (DataType == typeof(ColorF4))
            {
                ColorF4 color = newColor;
                panel1.BackColor = newColor;
                UpdateValue(color);
            }
            else if (DataType == typeof(EventColorF3))
            {
                EventColorF3 color = newColor;
                panel1.BackColor = color.Color;
                UpdateValue(color);
            }
            else if (DataType == typeof(EventColorF4))
            {
                EventColorF4 color = newColor;
                panel1.BackColor = newColor;
                UpdateValue(color);
            }
        }
    }
}
