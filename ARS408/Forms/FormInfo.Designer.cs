namespace ARS408.Forms
{
    partial class FormInfo
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label_RadarCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_RadarWorkingCount = new System.Windows.Forms.Label();
            this.textBox_Info = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button_Start = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel_TextBoxes = new System.Windows.Forms.FlowLayoutPanel();
            this.textBox_Radar_Sample = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel_Buttons = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel_TextBoxes.SuspendLayout();
            this.flowLayoutPanel_Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "雷达数量";
            // 
            // label_RadarCount
            // 
            this.label_RadarCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_RadarCount.AutoSize = true;
            this.label_RadarCount.Location = new System.Drawing.Point(78, 7);
            this.label_RadarCount.Name = "label_RadarCount";
            this.label_RadarCount.Size = new System.Drawing.Size(18, 20);
            this.label_RadarCount.TabIndex = 0;
            this.label_RadarCount.Text = "0";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(102, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "有效数目";
            // 
            // label_RadarWorkingCount
            // 
            this.label_RadarWorkingCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_RadarWorkingCount.AutoSize = true;
            this.label_RadarWorkingCount.Location = new System.Drawing.Point(177, 7);
            this.label_RadarWorkingCount.Name = "label_RadarWorkingCount";
            this.label_RadarWorkingCount.Size = new System.Drawing.Size(18, 20);
            this.label_RadarWorkingCount.TabIndex = 0;
            this.label_RadarWorkingCount.Text = "0";
            // 
            // textBox_Info
            // 
            this.textBox_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Info.Location = new System.Drawing.Point(757, 45);
            this.textBox_Info.Multiline = true;
            this.textBox_Info.Name = "textBox_Info";
            this.textBox_Info.Size = new System.Drawing.Size(244, 600);
            this.textBox_Info.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // button_Start
            // 
            this.button_Start.Location = new System.Drawing.Point(201, 2);
            this.button_Start.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(75, 30);
            this.button_Start.TabIndex = 2;
            this.button_Start.Text = "启动";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.Button_Start_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.Enabled = false;
            this.button_Stop.Location = new System.Drawing.Point(282, 2);
            this.button_Stop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(75, 30);
            this.button_Stop.TabIndex = 2;
            this.button_Stop.Text = "停止";
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.Button_Stop_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel_TextBoxes, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox_Info, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel_Buttons, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1004, 648);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // flowLayoutPanel_TextBoxes
            // 
            this.flowLayoutPanel_TextBoxes.Controls.Add(this.textBox_Radar_Sample);
            this.flowLayoutPanel_TextBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_TextBoxes.Location = new System.Drawing.Point(3, 45);
            this.flowLayoutPanel_TextBoxes.Name = "flowLayoutPanel_TextBoxes";
            this.flowLayoutPanel_TextBoxes.Size = new System.Drawing.Size(748, 600);
            this.flowLayoutPanel_TextBoxes.TabIndex = 4;
            // 
            // textBox_Radar_Sample
            // 
            this.textBox_Radar_Sample.Location = new System.Drawing.Point(3, 3);
            this.textBox_Radar_Sample.Multiline = true;
            this.textBox_Radar_Sample.Name = "textBox_Radar_Sample";
            this.textBox_Radar_Sample.Size = new System.Drawing.Size(240, 88);
            this.textBox_Radar_Sample.TabIndex = 2;
            this.textBox_Radar_Sample.Text = "  雷达ID: {0}\r\n  是否工作: {1},\r\n  距离: {2}";
            this.textBox_Radar_Sample.Visible = false;
            // 
            // flowLayoutPanel_Buttons
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel_Buttons, 2);
            this.flowLayoutPanel_Buttons.Controls.Add(this.label1);
            this.flowLayoutPanel_Buttons.Controls.Add(this.label_RadarCount);
            this.flowLayoutPanel_Buttons.Controls.Add(this.label3);
            this.flowLayoutPanel_Buttons.Controls.Add(this.label_RadarWorkingCount);
            this.flowLayoutPanel_Buttons.Controls.Add(this.button_Start);
            this.flowLayoutPanel_Buttons.Controls.Add(this.button_Stop);
            this.flowLayoutPanel_Buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_Buttons.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel_Buttons.Name = "flowLayoutPanel_Buttons";
            this.flowLayoutPanel_Buttons.Size = new System.Drawing.Size(998, 36);
            this.flowLayoutPanel_Buttons.TabIndex = 0;
            // 
            // FormInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 648);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormInfo";
            this.Text = "信息";
            this.Load += new System.EventHandler(this.FormInfo_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel_TextBoxes.ResumeLayout(false);
            this.flowLayoutPanel_TextBoxes.PerformLayout();
            this.flowLayoutPanel_Buttons.ResumeLayout(false);
            this.flowLayoutPanel_Buttons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_RadarCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_RadarWorkingCount;
        private System.Windows.Forms.TextBox textBox_Info;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox_Radar_Sample;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_Buttons;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_TextBoxes;
    }
}