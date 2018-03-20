using ComponentOwl.BetterListView;

namespace TheraEditor.Windows.Forms
{
    partial class DockableMatFuncList
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblFunctions = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.betterListView1 = new ComponentOwl.BetterListView.BetterListView();
            this.betterListViewColumnHeader1 = new ComponentOwl.BetterListView.BetterListViewColumnHeader();
            this.betterListViewColumnHeader2 = new ComponentOwl.BetterListView.BetterListViewColumnHeader();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.betterListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(60)))));
            this.panel1.Controls.Add(this.lblFunctions);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(501, 39);
            this.panel1.TabIndex = 2;
            // 
            // lblFunctions
            // 
            this.lblFunctions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(60)))));
            this.lblFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFunctions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblFunctions.Location = new System.Drawing.Point(0, 0);
            this.lblFunctions.Name = "lblFunctions";
            this.lblFunctions.Size = new System.Drawing.Size(302, 39);
            this.lblFunctions.TabIndex = 0;
            this.lblFunctions.Text = "Functions";
            this.lblFunctions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label1.Location = new System.Drawing.Point(302, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 39);
            this.label1.TabIndex = 1;
            this.label1.Text = "Find:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(361, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 9, 0, 0);
            this.panel2.Size = new System.Drawing.Size(140, 39);
            this.panel2.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(50)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.textBox1.Location = new System.Drawing.Point(0, 9);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(140, 22);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // betterListView1
            // 
            this.betterListView1.AllowDrag = true;
            this.betterListView1.AllowDrop = true;
            this.betterListView1.AllowedDragEffects = System.Windows.Forms.DragDropEffects.Move;
            this.betterListView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(60)))));
            this.betterListView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.betterListView1.Columns.Add(this.betterListViewColumnHeader1);
            this.betterListView1.Columns.Add(this.betterListViewColumnHeader2);
            this.betterListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.betterListView1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.betterListView1.ForeColorGroups = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.betterListView1.HeaderStyle = ComponentOwl.BetterListView.BetterListViewHeaderStyle.Sortable;
            this.betterListView1.HideSelectionMode = ComponentOwl.BetterListView.BetterListViewHideSelectionMode.Disable;
            this.betterListView1.Location = new System.Drawing.Point(0, 39);
            this.betterListView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.betterListView1.MultiSelect = false;
            this.betterListView1.Name = "betterListView1";
            this.betterListView1.ShowGroups = true;
            this.betterListView1.Size = new System.Drawing.Size(501, 328);
            this.betterListView1.TabIndex = 3;
            this.betterListView1.BeforeDrag += new ComponentOwl.BetterListView.BetterListViewBeforeDragEventHandler(this.BetterListView1_BeforeDrag);
            this.betterListView1.ItemDrag += new ComponentOwl.BetterListView.BetterListViewItemDragEventHandler(this.betterListView1_ItemDrag);
            // 
            // betterListViewColumnHeader1
            // 
            this.betterListViewColumnHeader1.Name = "betterListViewColumnHeader1";
            this.betterListViewColumnHeader1.SortMethod = ComponentOwl.BetterListView.BetterListViewSortMethod.Text;
            this.betterListViewColumnHeader1.Style = ComponentOwl.BetterListView.BetterListViewColumnHeaderStyle.Sortable;
            this.betterListViewColumnHeader1.Text = "Name";
            this.betterListViewColumnHeader1.Width = 208;
            // 
            // betterListViewColumnHeader2
            // 
            this.betterListViewColumnHeader2.Name = "betterListViewColumnHeader2";
            this.betterListViewColumnHeader2.Style = ComponentOwl.BetterListView.BetterListViewColumnHeaderStyle.Nonclickable;
            this.betterListViewColumnHeader2.Text = "Description";
            this.betterListViewColumnHeader2.Width = 232;
            // 
            // DockableMatFuncList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 367);
            this.Controls.Add(this.betterListView1);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "DockableMatFuncList";
            this.Text = "Function List";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.betterListView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblFunctions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
        private BetterListView betterListView1;
        private BetterListViewColumnHeader betterListViewColumnHeader1;
        private BetterListViewColumnHeader betterListViewColumnHeader2;
    }
}