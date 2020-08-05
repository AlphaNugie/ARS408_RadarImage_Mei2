namespace ARS408.Forms
{
    partial class FormRadarBehavior
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button_Save = new System.Windows.Forms.Button();
            this.Column_Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_ApplyFilter = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_UsePublicFilters = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_ApplyIteration = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_PushfMaxCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_FalseAlarmFilterString = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_AmbigStateFilterString = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_InvalidStateFilterString = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_MeasStateFilterString = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_ProbOfExistFilterString = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Changed = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.Column_ApplyFilter,
            this.Column_UsePublicFilters,
            this.Column_ApplyIteration,
            this.Column_PushfMaxCount,
            this.Column_FalseAlarmFilterString,
            this.Column_AmbigStateFilterString,
            this.Column_InvalidStateFilterString,
            this.Column_MeasStateFilterString,
            this.Column_ProbOfExistFilterString,
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
            // flowLayoutPanel1
            // 
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
            this.button_Save.Location = new System.Drawing.Point(3, 4);
            this.button_Save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(99, 38);
            this.button_Save.TabIndex = 0;
            this.button_Save.Text = "保存";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.Button_Save_Click);
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
            this.Column_Name.DataPropertyName = "radar_name";
            this.Column_Name.HeaderText = "雷达名称";
            this.Column_Name.MinimumWidth = 6;
            this.Column_Name.Name = "Column_Name";
            this.Column_Name.ReadOnly = true;
            this.Column_Name.Width = 98;
            // 
            // Column_Address
            // 
            this.Column_Address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Address.DataPropertyName = "address";
            this.Column_Address.HeaderText = "地址";
            this.Column_Address.MinimumWidth = 6;
            this.Column_Address.Name = "Column_Address";
            this.Column_Address.ReadOnly = true;
            this.Column_Address.Width = 68;
            // 
            // Column_ApplyFilter
            // 
            this.Column_ApplyFilter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ApplyFilter.DataPropertyName = "apply_filter";
            this.Column_ApplyFilter.FalseValue = "0";
            this.Column_ApplyFilter.HeaderText = "过滤";
            this.Column_ApplyFilter.MinimumWidth = 6;
            this.Column_ApplyFilter.Name = "Column_ApplyFilter";
            this.Column_ApplyFilter.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column_ApplyFilter.TrueValue = "1";
            this.Column_ApplyFilter.Width = 68;
            // 
            // Column_UsePublicFilters
            // 
            this.Column_UsePublicFilters.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_UsePublicFilters.DataPropertyName = "use_public_filters";
            this.Column_UsePublicFilters.FalseValue = "0";
            this.Column_UsePublicFilters.HeaderText = "公共过滤";
            this.Column_UsePublicFilters.MinimumWidth = 6;
            this.Column_UsePublicFilters.Name = "Column_UsePublicFilters";
            this.Column_UsePublicFilters.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column_UsePublicFilters.TrueValue = "1";
            this.Column_UsePublicFilters.Width = 98;
            // 
            // Column_ApplyIteration
            // 
            this.Column_ApplyIteration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ApplyIteration.DataPropertyName = "apply_iteration";
            this.Column_ApplyIteration.FalseValue = "0";
            this.Column_ApplyIteration.HeaderText = "迭代";
            this.Column_ApplyIteration.MinimumWidth = 6;
            this.Column_ApplyIteration.Name = "Column_ApplyIteration";
            this.Column_ApplyIteration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column_ApplyIteration.TrueValue = "1";
            this.Column_ApplyIteration.Width = 68;
            // 
            // Column_PushfMaxCount
            // 
            this.Column_PushfMaxCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_PushfMaxCount.DataPropertyName = "pushf_max_count";
            this.Column_PushfMaxCount.HeaderText = "累积周期";
            this.Column_PushfMaxCount.MinimumWidth = 6;
            this.Column_PushfMaxCount.Name = "Column_PushfMaxCount";
            this.Column_PushfMaxCount.Width = 98;
            // 
            // Column_FalseAlarmFilterString
            // 
            this.Column_FalseAlarmFilterString.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_FalseAlarmFilterString.DataPropertyName = "false_alarm_filter";
            this.Column_FalseAlarmFilterString.HeaderText = "虚警";
            this.Column_FalseAlarmFilterString.MinimumWidth = 6;
            this.Column_FalseAlarmFilterString.Name = "Column_FalseAlarmFilterString";
            this.Column_FalseAlarmFilterString.Width = 68;
            // 
            // Column_AmbigStateFilterString
            // 
            this.Column_AmbigStateFilterString.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_AmbigStateFilterString.DataPropertyName = "ambig_state_filter";
            this.Column_AmbigStateFilterString.HeaderText = "径向速度不确定性";
            this.Column_AmbigStateFilterString.MinimumWidth = 6;
            this.Column_AmbigStateFilterString.Name = "Column_AmbigStateFilterString";
            this.Column_AmbigStateFilterString.Width = 104;
            // 
            // Column_InvalidStateFilterString
            // 
            this.Column_InvalidStateFilterString.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_InvalidStateFilterString.DataPropertyName = "invalid_state_filter";
            this.Column_InvalidStateFilterString.HeaderText = "不可用性";
            this.Column_InvalidStateFilterString.MinimumWidth = 6;
            this.Column_InvalidStateFilterString.Name = "Column_InvalidStateFilterString";
            this.Column_InvalidStateFilterString.Width = 77;
            // 
            // Column_MeasStateFilterString
            // 
            this.Column_MeasStateFilterString.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_MeasStateFilterString.DataPropertyName = "meas_state_filter";
            this.Column_MeasStateFilterString.HeaderText = "测量状态";
            this.Column_MeasStateFilterString.MinimumWidth = 6;
            this.Column_MeasStateFilterString.Name = "Column_MeasStateFilterString";
            this.Column_MeasStateFilterString.Width = 77;
            // 
            // Column_ProbOfExistFilterString
            // 
            this.Column_ProbOfExistFilterString.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_ProbOfExistFilterString.DataPropertyName = "prob_exist_filter";
            this.Column_ProbOfExistFilterString.HeaderText = "存在概率";
            this.Column_ProbOfExistFilterString.MinimumWidth = 6;
            this.Column_ProbOfExistFilterString.Name = "Column_ProbOfExistFilterString";
            this.Column_ProbOfExistFilterString.Width = 77;
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
            // FormRadarBehavior
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1187, 664);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormRadarBehavior";
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
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_ApplyFilter;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_UsePublicFilters;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_ApplyIteration;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_PushfMaxCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_FalseAlarmFilterString;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_AmbigStateFilterString;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_InvalidStateFilterString;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_MeasStateFilterString;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_ProbOfExistFilterString;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Changed;
    }
}