namespace TheraEditor.Windows.Forms
{
    partial class DockableWelcomeWindow
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
            this.recentProjects = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.pnlCreateProj = new TheraEditor.Windows.Forms.TransparentPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtProjectDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBrowseProjectDir = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.transparentPanel1 = new TheraEditor.Windows.Forms.TransparentPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlCreateProj.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // recentProjects
            // 
            this.recentProjects.AutoScroll = true;
            this.recentProjects.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(42)))));
            this.recentProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentProjects.Location = new System.Drawing.Point(0, 32);
            this.recentProjects.Margin = new System.Windows.Forms.Padding(0);
            this.recentProjects.MinimumSize = new System.Drawing.Size(0, 160);
            this.recentProjects.Name = "recentProjects";
            this.recentProjects.Padding = new System.Windows.Forms.Padding(5);
            this.recentProjects.Size = new System.Drawing.Size(896, 160);
            this.recentProjects.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(896, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Recent Projects";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.splitContainer1.Panel2.Controls.Add(this.recentProjects);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2MinSize = 192;
            this.splitContainer1.Size = new System.Drawing.Size(896, 517);
            this.splitContainer1.SplitterDistance = 321;
            this.splitContainer1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(86)))), ((int)(((byte)(94)))));
            this.panel2.Controls.Add(this.btnOpen);
            this.panel2.Controls.Add(this.btnNew);
            this.panel2.Controls.Add(this.pnlCreateProj);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.transparentPanel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(896, 321);
            this.panel2.TabIndex = 8;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            this.btnOpen.Location = new System.Drawing.Point(468, 169);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(148, 56);
            this.btnOpen.TabIndex = 4;
            this.btnOpen.Text = "Open Project";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.Location = new System.Drawing.Point(296, 169);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(148, 56);
            this.btnNew.TabIndex = 2;
            this.btnNew.Text = "New Project";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // pnlCreateProj
            // 
            this.pnlCreateProj.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnlCreateProj.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlCreateProj.BackColor = System.Drawing.Color.Transparent;
            this.pnlCreateProj.Controls.Add(this.panel5);
            this.pnlCreateProj.Controls.Add(this.panel4);
            this.pnlCreateProj.Controls.Add(this.panel6);
            this.pnlCreateProj.Location = new System.Drawing.Point(296, 168);
            this.pnlCreateProj.Name = "pnlCreateProj";
            this.pnlCreateProj.Size = new System.Drawing.Size(320, 116);
            this.pnlCreateProj.TabIndex = 7;
            this.pnlCreateProj.Visible = false;
            // 
            // panel5
            // 
            this.panel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel5.BackColor = System.Drawing.Color.Transparent;
            this.panel5.Controls.Add(this.button1);
            this.panel5.Controls.Add(this.button2);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 84);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(320, 32);
            this.panel5.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 32);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(30)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            this.button2.Location = new System.Drawing.Point(172, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(148, 32);
            this.button2.TabIndex = 4;
            this.button2.Text = "Create";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // panel4
            // 
            this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.txtProjectDir);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.btnBrowseProjectDir);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 40);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(320, 32);
            this.panel4.TabIndex = 2;
            // 
            // txtProjectDir
            // 
            this.txtProjectDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtProjectDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProjectDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProjectDir.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProjectDir.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtProjectDir.Location = new System.Drawing.Point(73, 0);
            this.txtProjectDir.Name = "txtProjectDir";
            this.txtProjectDir.ReadOnly = true;
            this.txtProjectDir.Size = new System.Drawing.Size(208, 27);
            this.txtProjectDir.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Directory:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label3.UseMnemonic = false;
            // 
            // btnBrowseProjectDir
            // 
            this.btnBrowseProjectDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnBrowseProjectDir.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnBrowseProjectDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseProjectDir.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseProjectDir.Location = new System.Drawing.Point(281, 0);
            this.btnBrowseProjectDir.Name = "btnBrowseProjectDir";
            this.btnBrowseProjectDir.Size = new System.Drawing.Size(39, 32);
            this.btnBrowseProjectDir.TabIndex = 1;
            this.btnBrowseProjectDir.Text = "...";
            this.btnBrowseProjectDir.UseVisualStyleBackColor = false;
            this.btnBrowseProjectDir.Click += new System.EventHandler(this.btnBrowseProjectDir_Click);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Transparent;
            this.panel6.Controls.Add(this.txtName);
            this.panel6.Controls.Add(this.label4);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(320, 40);
            this.panel6.TabIndex = 3;
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtName.Location = new System.Drawing.Point(52, 0);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(268, 27);
            this.txtName.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Dock = System.Windows.Forms.DockStyle.Left;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 20);
            this.label4.TabIndex = 2;
            this.label4.Text = "Name:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label4.UseMnemonic = false;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(315, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(209, 37);
            this.label2.TabIndex = 6;
            this.label2.Text = "Thera Engine";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label2_MouseDown);
            this.label2.MouseEnter += new System.EventHandler(this.label2_MouseEnter);
            this.label2.MouseLeave += new System.EventHandler(this.label2_MouseLeave);
            this.label2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label2_MouseMove);
            this.label2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label2_MouseUp);
            // 
            // transparentPanel1
            // 
            this.transparentPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.transparentPanel1.BackColor = System.Drawing.Color.Transparent;
            this.transparentPanel1.Location = new System.Drawing.Point(296, 64);
            this.transparentPanel1.Name = "transparentPanel1";
            this.transparentPanel1.Size = new System.Drawing.Size(320, 97);
            this.transparentPanel1.TabIndex = 8;
            this.transparentPanel1.MouseEnter += new System.EventHandler(this.transparentPanel1_MouseEnter);
            this.transparentPanel1.MouseLeave += new System.EventHandler(this.transparentPanel1_MouseLeave);
            // 
            // DockableWelcomeWindow
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.ClientSize = new System.Drawing.Size(896, 517);
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Name = "DockableWelcomeWindow";
            this.Text = "Welcome";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.pnlCreateProj.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.FlowLayoutPanel recentProjects;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel2;
        private TransparentPanel pnlCreateProj;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox txtProjectDir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowseProjectDir;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label4;
        private TransparentPanel transparentPanel1;
    }
}