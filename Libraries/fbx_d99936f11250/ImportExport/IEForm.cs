using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SceneTreeView
{
    public partial class IEForm : Form
    {
        private int exportIndex;
        private int importIndex;
        private UIOutput output = null;

        private ImportExport ie;

        public IEForm()
        {
            InitializeComponent();
            output = new UIOutput(this.txtOutput);
            ie = new ImportExport(output);
        }

        public class UIOutput
        {

            /// <summary>
            /// output log
            /// </summary>
            private TextBox output;
            private StringBuilder strBuilder;
            internal UIOutput(TextBox output)
            {
                this.output = output;
                strBuilder = new StringBuilder();
            }
            /// <summary>
            /// Add message to new line
            /// </summary>
            /// <param name="msg">Message to add</param>
            public void Print(string msg)
            {
                strBuilder.AppendLine(msg);
                UpdateOutput();
            }
            /// <summary>
            /// Clear output
            /// </summary>
            public void Clear()
            {
                strBuilder.Remove(0, strBuilder.Length);
                UpdateOutput();
            }
            private void UpdateOutput()
            {
                output.Text = strBuilder.ToString();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FrmAbout a = new FrmAbout())
            {
                a.ShowDialog(this);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.AddExtension = true;
                open.CheckFileExists = true;
                open.CheckPathExists = true;
                open.Filter = "FBX {*.fbx}|*.fbx|" +
                              "3D Studio 3DS {*.3ds}|*.3ds|" +
                              "Alias OBJ {*.obj}|*.obj|" +
                              "Collada DAE {*.dae}|*.dae";
                open.FilterIndex = 1;
                open.ShowDialog(this);
                if (!string.IsNullOrEmpty(open.FileName))
                {
                    txtImport.Text = open.FileName;
                    importIndex = open.FilterIndex;
                }
                else
                    importIndex = -1;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog save = new SaveFileDialog())
            {
                save.AddExtension = true;
                save.Filter = "FBX binary {*.fbx}|*.fbx|" +
                              "FBX ascii {*.fbx}|*.fbx|" +
                              "FBX encrypted {*.fbx}|*.fbx|" +
                              "FBX 5.0 binary {*.fbx}|*.fbx|" +
                              "FBX 5.0 ascii {*.fbx}|*.fbx|" +
                              "Autocad DXF {*.dxf}|*.dxf|" +
                              "3D Studio 3DS {*.3ds}|*.3ds|" +
                              "Alias OBJ {*.obj}|*.obj|" +
                              "Collada DAE {*.dae}|*.dae";
                save.FilterIndex = 1;
                save.ShowDialog(this);
                if (!string.IsNullOrEmpty(save.FileName))
                {
                    txtExport.Text = save.FileName;
                    exportIndex = save.FilterIndex;
                }
                else
                    exportIndex = -1;
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtImport.Text))
            {
                MessageBox.Show("Please enter valid import file");
                return;
            }
            if (string.IsNullOrEmpty(txtExport.Text))
            {
                MessageBox.Show("Please enter valid export file");
                return;
            }
            if (!File.Exists(txtImport.Text))
            {
                MessageBox.Show("import file does not exist");
                return;
            }
            output.Clear();
            ImportExportDelegate ieFunc = new ImportExportDelegate(ExecuteImportExport);
            this.Invoke(ieFunc);
        }
        delegate void ImportExportDelegate();

        private void ExecuteImportExport()
        {
            ie.ImportAndExport(txtImport.Text, txtExport.Text, exportIndex - 1, txtPassword.Text);
        }
    }
}
