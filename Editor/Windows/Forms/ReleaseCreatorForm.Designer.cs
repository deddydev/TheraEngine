namespace TheraEditor.Windows.Forms
{
    partial class ReleaseCreatorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtEngineNotes = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboConfiguration = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdx64 = new System.Windows.Forms.RadioButton();
            this.rdx86 = new System.Windows.Forms.RadioButton();
            this.chkEngine = new System.Windows.Forms.CheckBox();
            this.chkEditor = new System.Windows.Forms.CheckBox();
            this.spltcReleaseNotes = new System.Windows.Forms.SplitContainer();
            this.btnSaveDefaultEngine = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEditorNotes = new System.Windows.Forms.RichTextBox();
            this.btnSaveDefaultEditor = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltcReleaseNotes)).BeginInit();
            this.spltcReleaseNotes.Panel1.SuspendLayout();
            this.spltcReleaseNotes.Panel2.SuspendLayout();
            this.spltcReleaseNotes.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.BodyPanel.Controls.Add(this.splitContainer1);
            this.BodyPanel.Controls.Add(this.panel1);
            this.BodyPanel.Size = new System.Drawing.Size(586, 431);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(586, 471);
            // 
            // TitlePanel
            // 
            this.TitlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.TitlePanel.Size = new System.Drawing.Size(586, 40);
            // 
            // FormTitle
            // 
            this.FormTitle.Size = new System.Drawing.Size(415, 40);
            this.FormTitle.Text = "Release Creator";
            this.FormTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(586, 479);
            // 
            // txtEngineNotes
            // 
            this.txtEngineNotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtEngineNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEngineNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEngineNotes.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtEngineNotes.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtEngineNotes.Location = new System.Drawing.Point(0, 29);
            this.txtEngineNotes.Name = "txtEngineNotes";
            this.txtEngineNotes.Size = new System.Drawing.Size(268, 234);
            this.txtEngineNotes.TabIndex = 0;
            this.txtEngineNotes.Text = "";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.chkEngine);
            this.splitContainer1.Panel1.Controls.Add(this.chkEditor);
            this.splitContainer1.Panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(200)))));
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.spltcReleaseNotes);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.splitContainer1.Size = new System.Drawing.Size(586, 387);
            this.splitContainer1.SplitterDistance = 82;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.groupBox2.Controls.Add(this.cboConfiguration);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(200)))));
            this.groupBox2.Location = new System.Drawing.Point(230, 16);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(343, 51);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Configuration";
            // 
            // cboConfiguration
            // 
            this.cboConfiguration.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cboConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboConfiguration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConfiguration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboConfiguration.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboConfiguration.ForeColor = System.Drawing.Color.Gainsboro;
            this.cboConfiguration.FormattingEnabled = true;
            this.cboConfiguration.IntegralHeight = false;
            this.cboConfiguration.Items.AddRange(new object[] {
            "goodbye",
            "hello"});
            this.cboConfiguration.Location = new System.Drawing.Point(3, 18);
            this.cboConfiguration.MaxDropDownItems = 100;
            this.cboConfiguration.Name = "cboConfiguration";
            this.cboConfiguration.Size = new System.Drawing.Size(337, 28);
            this.cboConfiguration.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.groupBox1.Controls.Add(this.rdx64);
            this.groupBox1.Controls.Add(this.rdx86);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(200)))));
            this.groupBox1.Location = new System.Drawing.Point(95, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(129, 51);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Platform";
            // 
            // rdx64
            // 
            this.rdx64.AutoSize = true;
            this.rdx64.Checked = true;
            this.rdx64.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdx64.Location = new System.Drawing.Point(72, 21);
            this.rdx64.Name = "rdx64";
            this.rdx64.Size = new System.Drawing.Size(53, 24);
            this.rdx64.TabIndex = 1;
            this.rdx64.TabStop = true;
            this.rdx64.Text = "x64";
            this.rdx64.UseVisualStyleBackColor = true;
            // 
            // rdx86
            // 
            this.rdx86.AutoSize = true;
            this.rdx86.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdx86.Location = new System.Drawing.Point(6, 21);
            this.rdx86.Name = "rdx86";
            this.rdx86.Size = new System.Drawing.Size(53, 24);
            this.rdx86.TabIndex = 0;
            this.rdx86.Text = "x86";
            this.rdx86.UseVisualStyleBackColor = true;
            // 
            // chkEngine
            // 
            this.chkEngine.AutoSize = true;
            this.chkEngine.Checked = true;
            this.chkEngine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEngine.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkEngine.ForeColor = System.Drawing.Color.Gainsboro;
            this.chkEngine.Location = new System.Drawing.Point(13, 43);
            this.chkEngine.Name = "chkEngine";
            this.chkEngine.Size = new System.Drawing.Size(76, 24);
            this.chkEngine.TabIndex = 1;
            this.chkEngine.Text = "Engine";
            this.chkEngine.UseVisualStyleBackColor = true;
            this.chkEngine.CheckedChanged += new System.EventHandler(this.ChkEngine_CheckedChanged);
            // 
            // chkEditor
            // 
            this.chkEditor.AutoSize = true;
            this.chkEditor.Checked = true;
            this.chkEditor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEditor.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkEditor.ForeColor = System.Drawing.Color.Gainsboro;
            this.chkEditor.Location = new System.Drawing.Point(13, 16);
            this.chkEditor.Name = "chkEditor";
            this.chkEditor.Size = new System.Drawing.Size(71, 24);
            this.chkEditor.TabIndex = 0;
            this.chkEditor.Text = "Editor";
            this.chkEditor.UseVisualStyleBackColor = true;
            this.chkEditor.CheckedChanged += new System.EventHandler(this.ChkEditor_CheckedChanged);
            // 
            // spltcReleaseNotes
            // 
            this.spltcReleaseNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltcReleaseNotes.Location = new System.Drawing.Point(10, 0);
            this.spltcReleaseNotes.Name = "spltcReleaseNotes";
            // 
            // spltcReleaseNotes.Panel1
            // 
            this.spltcReleaseNotes.Panel1.Controls.Add(this.txtEngineNotes);
            this.spltcReleaseNotes.Panel1.Controls.Add(this.btnSaveDefaultEngine);
            this.spltcReleaseNotes.Panel1.Controls.Add(this.label1);
            // 
            // spltcReleaseNotes.Panel2
            // 
            this.spltcReleaseNotes.Panel2.Controls.Add(this.txtEditorNotes);
            this.spltcReleaseNotes.Panel2.Controls.Add(this.btnSaveDefaultEditor);
            this.spltcReleaseNotes.Panel2.Controls.Add(this.label2);
            this.spltcReleaseNotes.Size = new System.Drawing.Size(566, 291);
            this.spltcReleaseNotes.SplitterDistance = 268;
            this.spltcReleaseNotes.TabIndex = 2;
            // 
            // btnSaveDefaultEngine
            // 
            this.btnSaveDefaultEngine.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSaveDefaultEngine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSaveDefaultEngine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveDefaultEngine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(200)))));
            this.btnSaveDefaultEngine.Location = new System.Drawing.Point(0, 263);
            this.btnSaveDefaultEngine.Name = "btnSaveDefaultEngine";
            this.btnSaveDefaultEngine.Size = new System.Drawing.Size(268, 28);
            this.btnSaveDefaultEngine.TabIndex = 2;
            this.btnSaveDefaultEngine.Text = "Save as default message";
            this.btnSaveDefaultEngine.UseVisualStyleBackColor = false;
            this.btnSaveDefaultEngine.Click += new System.EventHandler(this.BtnSaveDefaultEngine_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.Gainsboro;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "Engine Release Notes";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtEditorNotes
            // 
            this.txtEditorNotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtEditorNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEditorNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEditorNotes.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtEditorNotes.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtEditorNotes.Location = new System.Drawing.Point(0, 29);
            this.txtEditorNotes.Name = "txtEditorNotes";
            this.txtEditorNotes.Size = new System.Drawing.Size(294, 234);
            this.txtEditorNotes.TabIndex = 3;
            this.txtEditorNotes.Text = "";
            // 
            // btnSaveDefaultEditor
            // 
            this.btnSaveDefaultEditor.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSaveDefaultEditor.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSaveDefaultEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveDefaultEditor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(200)))));
            this.btnSaveDefaultEditor.Location = new System.Drawing.Point(0, 263);
            this.btnSaveDefaultEditor.Name = "btnSaveDefaultEditor";
            this.btnSaveDefaultEditor.Size = new System.Drawing.Size(294, 28);
            this.btnSaveDefaultEditor.TabIndex = 4;
            this.btnSaveDefaultEditor.Text = "Save as default message";
            this.btnSaveDefaultEditor.UseVisualStyleBackColor = false;
            this.btnSaveDefaultEditor.Click += new System.EventHandler(this.BtnSaveDefaultEditor_Click);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.ForeColor = System.Drawing.Color.Gainsboro;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(294, 29);
            this.label2.TabIndex = 2;
            this.label2.Text = "Editor Release Notes";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPost);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 387);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(586, 44);
            this.panel1.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(200)))));
            this.btnCancel.Location = new System.Drawing.Point(431, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 34);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += BtnCancel_Click;
            // 
            // btnPost
            // 
            this.btnPost.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnPost.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnPost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(200)))));
            this.btnPost.Location = new System.Drawing.Point(506, 5);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 34);
            this.btnPost.TabIndex = 0;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.BtnPost_Click);
            // 
            // ReleaseCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 479);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "ReleaseCreatorForm";
            this.Text = "Release Creator";
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.spltcReleaseNotes.Panel1.ResumeLayout(false);
            this.spltcReleaseNotes.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltcReleaseNotes)).EndInit();
            this.spltcReleaseNotes.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox txtEngineNotes;
        private System.Windows.Forms.CheckBox chkEngine;
        private System.Windows.Forms.CheckBox chkEditor;
        private System.Windows.Forms.SplitContainer spltcReleaseNotes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdx64;
        private System.Windows.Forms.RadioButton rdx86;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboConfiguration;
        private System.Windows.Forms.RichTextBox txtEditorNotes;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnSaveDefaultEngine;
        private System.Windows.Forms.Button btnSaveDefaultEditor;
    }
}