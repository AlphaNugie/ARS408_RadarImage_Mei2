using ARS408.Core;
using ARS408.Model;
using CommonLib.UIControlUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARS408.Forms
{
    public partial class FormCoorsLimitationConfig : Form
    {
        private readonly DataService_Radar dataService = new DataService_Radar(); //数据库服务类

        public FormCoorsLimitationConfig()
        {
            InitializeComponent();
            DataSourceRefresh();
        }

        /// <summary>
        /// 刷新数据源
        /// </summary>
        private void DataSourceRefresh()
        {
            DataTable table;
            try { table = dataService.GetRadarCoorsLimitations(); }
            catch (Exception e)
            {
                string errorMessage = "查询时出错：" + e.Message;
                MessageBox.Show(errorMessage, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dataGridView.SetDoubleBuffered(true);
            dataGridView.DataSource = table;
        }

        /// <summary>
        /// 刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            DataSourceRefresh();
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Save_Click(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count == 0)
                return;

            List<Radar> list = new List<Radar>();
            foreach (DataGridViewRow row in dataGridView.Rows)
                if (row.Cells["Column_Changed"].Value.ToString().Equals("1"))
                {
                    Radar radar = DataGridViewUtil.ConvertDataGridViewRow2Obect<Radar>(row, false); //不抛出异常
                    radar.RadarCoorsLimited = row.Cells["Column_RadarCoorsLimited"].Value.ToString().Equals("1");
                    radar.ClaimerCoorsLimited = row.Cells["Column_ClaimerCoorsLimited"].Value.ToString().Equals("1");
                    radar.AngleLimited = row.Cells["Column_AngleLimited"].Value.ToString().Equals("1");
                    radar.WithinRadarLimit = row.Cells["Column_WithinRadarLimit"].Value.ToString().Equals("1");
                    radar.WithinClaimerLimit = row.Cells["Column_WithinClaimerLimit"].Value.ToString().Equals("1");
                    radar.WithinAngleLimit = row.Cells["Column_WithinAngleLimit"].Value.ToString().Equals("1");
                    list.Add(radar);
                }

            bool result;
            try { result = dataService.SaveRadarCoorsLimitations(list); }
            catch (Exception ex)
            {
                string errorMessage = "信息保存时出现问题：" + ex.Message;
                MessageBox.Show(errorMessage, "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (result)
            {
                MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DataSourceRefresh();
            }
            else
                MessageBox.Show("保存失败", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //取消事件，完成代码处理后再添加事件（代码中改变单元格的值会导致死循环）
                dataGridView.CellValueChanged -= new DataGridViewCellEventHandler(DataGridView_CellValueChanged);
                dataGridView.Rows[e.RowIndex].Cells["Column_Changed"].Value = 1;
                dataGridView.CellValueChanged += new DataGridViewCellEventHandler(DataGridView_CellValueChanged);
            }
        }
    }
}
