namespace ARS408.Forms
{
    partial class FormPreferences
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
            this.tabPage_Main = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.checkBox_ShowDeserted = new System.Windows.Forms.CheckBox();
            this.tabPage_Main.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage_Main
            // 
            this.tabPage_Main.Controls.Add(this.checkBox_ShowDeserted);
            this.tabPage_Main.Location = new System.Drawing.Point(4, 29);
            this.tabPage_Main.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage_Main.Name = "tabPage_Main";
            this.tabPage_Main.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage_Main.Size = new System.Drawing.Size(851, 440);
            this.tabPage_Main.TabIndex = 0;
            this.tabPage_Main.Text = "Main";
            this.tabPage_Main.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_Main);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(859, 473);
            this.tabControl1.TabIndex = 0;
            // 
            // checkBox_ShowDeserted
            // 
            this.checkBox_ShowDeserted.AutoSize = true;
            this.checkBox_ShowDeserted.Location = new System.Drawing.Point(32, 30);
            this.checkBox_ShowDeserted.Name = "checkBox_ShowDeserted";
            this.checkBox_ShowDeserted.Size = new System.Drawing.Size(181, 24);
            this.checkBox_ShowDeserted.TabIndex = 0;
            this.checkBox_ShowDeserted.Text = "是否显示被过滤掉的点";
            this.checkBox_ShowDeserted.UseVisualStyleBackColor = true;
            this.checkBox_ShowDeserted.CheckedChanged += new System.EventHandler(this.CheckBox_ShowDeserted_CheckedChanged);
            // 
            // FormPreferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 473);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormPreferences";
            this.Text = "首选项";
            this.tabPage_Main.ResumeLayout(false);
            this.tabPage_Main.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage_Main;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.CheckBox checkBox_ShowDeserted;
    }
}