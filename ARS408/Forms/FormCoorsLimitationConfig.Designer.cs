﻿namespace ARS408.Forms
{
    partial class FormCoorsLimitationConfig
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.Column_Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_RadarCoorsLimited = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_WithinRadarLimit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_RadarxMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_RadarxMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_RadaryMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_RadaryMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_ClaimerCoorsLimited = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_WithinClaimerLimit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_ClaimerxMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_ClaimerxMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_ClaimeryMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_ClaimeryMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_ClaimerzMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_ClaimerzMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_AngleLimited = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_WithinAngleLimit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_AngleMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_AngleMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Changed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button_Save = new System.Windows.Forms.Button();
            this.button_Refresh = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dataGridView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1187, 664);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_Id,
            this.Column_Name,
            this.Column_Address,
            this.Column_RadarCoorsLimited,
            this.Column_WithinRadarLimit,
            this.Column_RadarxMin,
            this.Column_RadarxMax,
            this.Column_RadaryMin,
            this.Column_RadaryMax,
            this.Column_ClaimerCoorsLimited,
            this.Column_WithinClaimerLimit,
            this.Column_ClaimerxMin,
            this.Column_ClaimerxMax,
            this.Column_ClaimeryMin,
            this.Column_ClaimeryMax,
            this.Column_ClaimerzMin,
            this.Column_ClaimerzMax,
            this.Column_AngleLimited,
            this.Column_WithinAngleLimit,
            this.Column_AngleMin,
            this.Column_AngleMax,
            this.Column_Changed});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 53);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidth = 51;
            this.dataGridView.RowTemplate.Height = 27;
            this.dataGridView.Size = new System.Drawing.Size(1181, 608);
            this.dataGridView.TabIndex = 26;
            this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellValueChanged);
            // 
            // Column_Id
            // 
            this.Column_Id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Id.DataPropertyName = "RADAR_ID";
            this.Column_Id.HeaderText = "ID";
            this.Column_Id.MinimumWidth = 6;
            this.Column_Id.Name = "Column_Id";
            this.Column_Id.ReadOnly = true;
            this.Column_Id.Width = 53;
            // 
            // Column_Name
            // 
            this.Column_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Name.DataPropertyName = "RADAR_NAME";
            this.Column_Name.HeaderText = "雷达名称";
            this.Column_Name.MinimumWidth = 6;
            this.Column_Name.Name = "Column_Name";
            this.Column_Name.ReadOnly = true;
            this.Column_Name.Width = 98;
            // 
            // Column_Address
            // 
            this.Column_Address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Address.DataPropertyName = "ADDRESS";
            this.Column_Address.HeaderText = "地址";
            this.Column_Address.MinimumWidth = 6;
            this.Column_Address.Name = "Column_Address";
            this.Column_Address.ReadOnly = true;
            this.Column_Address.Width = 68;
            // 
            // Column_RadarCoorsLimited
            // 
            this.Column_RadarCoorsLimited.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_RadarCoorsLimited.DataPropertyName = "radar_coors_limited";
            this.Column_RadarCoorsLimited.FalseValue = "0";
            this.Column_RadarCoorsLimited.HeaderText = "限制雷达";
            this.Column_RadarCoorsLimited.MinimumWidth = 6;
            this.Column_RadarCoorsLimited.Name = "Column_RadarCoorsLimited";
            this.Column_RadarCoorsLimited.TrueValue = "1";
            this.Column_RadarCoorsLimited.Width = 75;
            // 
            // Column_WithinRadarLimit
            // 
            this.Column_WithinRadarLimit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_WithinRadarLimit.DataPropertyName = "within_radar_limit";
            this.Column_WithinRadarLimit.FalseValue = "0";
            this.Column_WithinRadarLimit.HeaderText = "范围内";
            this.Column_WithinRadarLimit.MinimumWidth = 6;
            this.Column_WithinRadarLimit.Name = "Column_WithinRadarLimit";
            this.Column_WithinRadarLimit.TrueValue = "1";
            this.Column_WithinRadarLimit.Width = 60;
            // 
            // Column_RadarxMin
            // 
            this.Column_RadarxMin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_RadarxMin.DataPropertyName = "radar_x_min";
            this.Column_RadarxMin.HeaderText = "X-";
            this.Column_RadarxMin.MinimumWidth = 6;
            this.Column_RadarxMin.Name = "Column_RadarxMin";
            this.Column_RadarxMin.Width = 54;
            // 
            // Column_RadarxMax
            // 
            this.Column_RadarxMax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_RadarxMax.DataPropertyName = "radar_x_max";
            this.Column_RadarxMax.HeaderText = "X+";
            this.Column_RadarxMax.MinimumWidth = 6;
            this.Column_RadarxMax.Name = "Column_RadarxMax";
            this.Column_RadarxMax.Width = 59;
            // 
            // Column_RadaryMin
            // 
            this.Column_RadaryMin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_RadaryMin.DataPropertyName = "radar_y_min";
            this.Column_RadaryMin.HeaderText = "Y-";
            this.Column_RadaryMin.MinimumWidth = 6;
            this.Column_RadaryMin.Name = "Column_RadaryMin";
            this.Column_RadaryMin.Width = 53;
            // 
            // Column_RadaryMax
            // 
            this.Column_RadaryMax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_RadaryMax.DataPropertyName = "radar_y_max";
            this.Column_RadaryMax.HeaderText = "Y+";
            this.Column_RadaryMax.MinimumWidth = 6;
            this.Column_RadaryMax.Name = "Column_RadaryMax";
            this.Column_RadaryMax.Width = 58;
            // 
            // Column_ClaimerCoorsLimited
            // 
            this.Column_ClaimerCoorsLimited.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ClaimerCoorsLimited.DataPropertyName = "claimer_coors_limited";
            this.Column_ClaimerCoorsLimited.FalseValue = "0";
            this.Column_ClaimerCoorsLimited.HeaderText = "限制单机";
            this.Column_ClaimerCoorsLimited.MinimumWidth = 6;
            this.Column_ClaimerCoorsLimited.Name = "Column_ClaimerCoorsLimited";
            this.Column_ClaimerCoorsLimited.TrueValue = "1";
            this.Column_ClaimerCoorsLimited.Width = 75;
            // 
            // Column_WithinClaimerLimit
            // 
            this.Column_WithinClaimerLimit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_WithinClaimerLimit.DataPropertyName = "within_claimer_limit";
            this.Column_WithinClaimerLimit.FalseValue = "0";
            this.Column_WithinClaimerLimit.HeaderText = "范围内";
            this.Column_WithinClaimerLimit.MinimumWidth = 6;
            this.Column_WithinClaimerLimit.Name = "Column_WithinClaimerLimit";
            this.Column_WithinClaimerLimit.TrueValue = "1";
            this.Column_WithinClaimerLimit.Width = 60;
            // 
            // Column_ClaimerxMin
            // 
            this.Column_ClaimerxMin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ClaimerxMin.DataPropertyName = "claimer_x_min";
            this.Column_ClaimerxMin.HeaderText = "X-";
            this.Column_ClaimerxMin.MinimumWidth = 6;
            this.Column_ClaimerxMin.Name = "Column_ClaimerxMin";
            this.Column_ClaimerxMin.Width = 54;
            // 
            // Column_ClaimerxMax
            // 
            this.Column_ClaimerxMax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ClaimerxMax.DataPropertyName = "claimer_x_max";
            this.Column_ClaimerxMax.HeaderText = "X+";
            this.Column_ClaimerxMax.MinimumWidth = 6;
            this.Column_ClaimerxMax.Name = "Column_ClaimerxMax";
            this.Column_ClaimerxMax.Width = 59;
            // 
            // Column_ClaimeryMin
            // 
            this.Column_ClaimeryMin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ClaimeryMin.DataPropertyName = "claimer_y_min";
            this.Column_ClaimeryMin.HeaderText = "Y-";
            this.Column_ClaimeryMin.MinimumWidth = 6;
            this.Column_ClaimeryMin.Name = "Column_ClaimeryMin";
            this.Column_ClaimeryMin.Width = 53;
            // 
            // Column_ClaimeryMax
            // 
            this.Column_ClaimeryMax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ClaimeryMax.DataPropertyName = "claimer_y_max";
            this.Column_ClaimeryMax.HeaderText = "Y+";
            this.Column_ClaimeryMax.MinimumWidth = 6;
            this.Column_ClaimeryMax.Name = "Column_ClaimeryMax";
            this.Column_ClaimeryMax.Width = 58;
            // 
            // Column_ClaimerzMin
            // 
            this.Column_ClaimerzMin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ClaimerzMin.DataPropertyName = "claimer_z_min";
            this.Column_ClaimerzMin.HeaderText = "Z-";
            this.Column_ClaimerzMin.MinimumWidth = 6;
            this.Column_ClaimerzMin.Name = "Column_ClaimerzMin";
            this.Column_ClaimerzMin.Width = 53;
            // 
            // Column_ClaimerzMax
            // 
            this.Column_ClaimerzMax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ClaimerzMax.DataPropertyName = "claimer_z_max";
            this.Column_ClaimerzMax.HeaderText = "Z+";
            this.Column_ClaimerzMax.MinimumWidth = 6;
            this.Column_ClaimerzMax.Name = "Column_ClaimerzMax";
            this.Column_ClaimerzMax.Width = 58;
            // 
            // Column_AngleLimited
            // 
            this.Column_AngleLimited.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_AngleLimited.DataPropertyName = "angle_limited";
            this.Column_AngleLimited.FalseValue = "0";
            this.Column_AngleLimited.HeaderText = "限制角度";
            this.Column_AngleLimited.MinimumWidth = 6;
            this.Column_AngleLimited.Name = "Column_AngleLimited";
            this.Column_AngleLimited.TrueValue = "1";
            this.Column_AngleLimited.Width = 75;
            // 
            // Column_WithinAngleLimit
            // 
            this.Column_WithinAngleLimit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_WithinAngleLimit.DataPropertyName = "within_angle_limit";
            this.Column_WithinAngleLimit.FalseValue = "0";
            this.Column_WithinAngleLimit.HeaderText = "范围内";
            this.Column_WithinAngleLimit.MinimumWidth = 6;
            this.Column_WithinAngleLimit.Name = "Column_WithinAngleLimit";
            this.Column_WithinAngleLimit.TrueValue = "1";
            this.Column_WithinAngleLimit.Width = 60;
            // 
            // Column_AngleMin
            // 
            this.Column_AngleMin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_AngleMin.DataPropertyName = "angle_min";
            this.Column_AngleMin.HeaderText = "A-";
            this.Column_AngleMin.MinimumWidth = 6;
            this.Column_AngleMin.Name = "Column_AngleMin";
            this.Column_AngleMin.Width = 55;
            // 
            // Column_AngleMax
            // 
            this.Column_AngleMax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_AngleMax.DataPropertyName = "angle_max";
            this.Column_AngleMax.HeaderText = "A+";
            this.Column_AngleMax.MinimumWidth = 6;
            this.Column_AngleMax.Name = "Column_AngleMax";
            this.Column_AngleMax.Width = 60;
            // 
            // Column_Changed
            // 
            this.Column_Changed.DataPropertyName = "CHANGED";
            this.Column_Changed.HeaderText = "是否改变";
            this.Column_Changed.MinimumWidth = 6;
            this.Column_Changed.Name = "Column_Changed";
            this.Column_Changed.Visible = false;
            this.Column_Changed.Width = 125;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.button_Refresh);
            this.flowLayoutPanel1.Controls.Add(this.button_Save);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 4);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1181, 42);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // button_Save
            // 
            this.button_Save.Dock = System.Windows.Forms.DockStyle.Top;
            this.button_Save.Location = new System.Drawing.Point(108, 4);
            this.button_Save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(99, 38);
            this.button_Save.TabIndex = 0;
            this.button_Save.Text = "保存";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.Button_Save_Click);
            // 
            // button_Refresh
            // 
            this.button_Refresh.Dock = System.Windows.Forms.DockStyle.Top;
            this.button_Refresh.Location = new System.Drawing.Point(3, 4);
            this.button_Refresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(99, 38);
            this.button_Refresh.TabIndex = 2;
            this.button_Refresh.Text = "刷新";
            this.button_Refresh.UseVisualStyleBackColor = true;
            this.button_Refresh.Click += new System.EventHandler(this.Button_Refresh_Click);
            // 
            // FormCoorsLimitationConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1187, 664);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormCoorsLimitationConfig";
            this.Text = "坐标限定范围配置";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Address;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_RadarCoorsLimited;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_WithinRadarLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_RadarxMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_RadarxMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_RadaryMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_RadaryMax;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_ClaimerCoorsLimited;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_WithinClaimerLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_ClaimerxMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_ClaimerxMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_ClaimeryMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_ClaimeryMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_ClaimerzMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_ClaimerzMax;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_AngleLimited;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_WithinAngleLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_AngleMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_AngleMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Changed;
        private System.Windows.Forms.Button button_Refresh;
    }
}