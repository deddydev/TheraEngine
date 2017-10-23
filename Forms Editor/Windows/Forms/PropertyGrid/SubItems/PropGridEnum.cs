using System;
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
            object value = GetPropertyValue();
            if (value is Enum)
            {
                string[] names = Enum.GetNames(Property.PropertyType);
                Array values = Enum.GetValues(Property.PropertyType);
                bool flags = Property.GetCustomAttributes(false).FirstOrDefault(x => x is FlagsAttribute) != null;
                if (flags)
                {
                    string[] enumStrings = value.ToString().Split(new string[] { ", " }, StringSplitOptions.None);
                    tableLayoutPanel1.Visible = true;
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
                            Checked = enumStrings.Contains(name),
                            Tag = name,
                        };
                        bitSet.CheckedChanged += BitSet_CheckedChanged;
                        Label bitValue = new Label()
                        {
                            Text = values.GetValue(i).ToString(),
                            TextAlign = ContentAlignment.MiddleLeft
                        };
                        Label bitName = new Label()
                        {
                            Text = name,
                            TextAlign = ContentAlignment.MiddleLeft
                        };

                        tableLayoutPanel1.Controls.Add(bitSet, 0, tableLayoutPanel1.RowCount - 1);
                        tableLayoutPanel1.Controls.Add(bitValue, 1, tableLayoutPanel1.RowCount - 1);
                        tableLayoutPanel1.Controls.Add(bitName, 2, tableLayoutPanel1.RowCount - 1);
                    }
                }
                else
                {
                    tableLayoutPanel1.Visible = false;
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
                throw new Exception(Property.PropertyType.GetFriendlyName() + " is not an Enum type.");
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
