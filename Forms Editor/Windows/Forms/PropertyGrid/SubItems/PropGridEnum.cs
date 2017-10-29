﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(Enum))]
    public partial class PropGridEnum : PropGridItem
    {
        public PropGridEnum() => InitializeComponent();
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            if (value is Enum e)
            {
                string[] names = Enum.GetNames(DataType);
                Array values = Enum.GetValues(DataType);
                bool flags = DataType.GetCustomAttributes(false).FirstOrDefault(x => x is FlagsAttribute) != null;
                if (flags)
                {
                    string[] enumStrings = value.ToString().Split(new string[] { ", " }, StringSplitOptions.None);
                    panel1.Visible = true;
                    comboBox1.Visible = false;
                    tableLayoutPanel1.RowStyles.Clear();
                    tableLayoutPanel1.RowCount = 0;
                    for (int i = 0; i < names.Length; ++i)
                    {
                        string name = names[i];

                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        tableLayoutPanel1.RowCount++;

                        CheckBox bitSet = new CheckBox()
                        {
                            AutoSize = true,
                            Checked = enumStrings.Contains(name),
                            Tag = name,
                            Margin = new Padding(0),
                            Padding = new Padding(0),
                            Dock = DockStyle.Left,
                        };
                        bitSet.CheckedChanged += BitSet_CheckedChanged;
                        object number = values.GetValue(i);
                        object number2 = Convert.ChangeType(number, e.GetTypeCode());
                        Label bitValue = new Label()
                        {
                            AutoSize = true,
                            Text = number2.ToString(),
                            TextAlign = ContentAlignment.MiddleLeft,
                            Margin = new Padding(0),
                            Padding = new Padding(0),
                            Dock = DockStyle.Left,
                        };
                        Label bitName = new Label()
                        {
                            AutoSize = true,
                            Text = name,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Margin = new Padding(0),
                            Padding = new Padding(0),
                            Dock = DockStyle.Fill,
                        };

                        tableLayoutPanel1.Controls.Add(bitSet, 0, tableLayoutPanel1.RowCount - 1);
                        tableLayoutPanel1.Controls.Add(bitValue, 2, tableLayoutPanel1.RowCount - 1);
                        tableLayoutPanel1.Controls.Add(bitName, 1, tableLayoutPanel1.RowCount - 1);
                    }
                }
                else
                {
                    panel1.Visible = false;
                    comboBox1.Visible = true;
                    string enumName = value.ToString();

                    int selectedIndex = -1;
                    for (int i = 0; i < names.Length; ++i)
                    {
                        string name = names[i];
                        comboBox1.Items.Add(name);
                        if (string.Equals(name, enumName, StringComparison.InvariantCulture))
                            selectedIndex = i;
                    }
                    comboBox1.SelectedIndex = selectedIndex;
                }
                //textBox1.Text = value?.ToString();
            }
            else if (value is Exception)
            {
                comboBox1.Visible = true;
                tableLayoutPanel1.Visible = false;
                comboBox1.Text = value.ToString();
            }
            else
            {
                throw new Exception(DataType.GetFriendlyName() + " is not an Enum type.");
            }
        }

        private void BitSet_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox)sender;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //if (_updating)
            //    return;
            //UpdatePropertyValue(textBox1.Text);
        }
    }
}
