using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ARS408.Core;
using OPCAutomation;
using CommonLib.DataUtil;
using CommonLib.Function;
using CommonLib.UIControlUtil;
using ARS408.Model;
using CommonLib.Extensions;
//using ScanOutputDemo.DataUtil;

namespace ARS408.Forms
{
    public partial class FormOpcConfig : Form
    {
        private readonly DataService_OpcItem dataService = new DataService_OpcItem(); //数据库服务类

        /// <summary>
        /// IP Address
        /// </summary>
        public string IpAddress { get; private set; }

        public OPCServer KepServer { get; private set; }

        public OPCGroup KepGroup { get; private set; }

        private Array lServerHandlers;

        public FormOpcConfig()
        {
            InitializeComponent();
            InitControls();
            DataSourceRefresh();
        }

        private void FormOpcServerTest_Load(object sender, EventArgs e) { }

        private void InitControls()
        {
            textBox_OpcServerIp.Text = BaseConst.IniHelper.ReadData("OPC", "ServerIp");
            comboBox_OpcServerList.Text = BaseConst.IniHelper.ReadData("OPC", "ServerName");
            //checkBox_WriteItemValue.Checked = BaseConst.IniHelper.ReadData("OPC", "WriteItemValue").Equals("1");
            dataGridView_Main.AutoGenerateColumns = false;
        }

        /// <summary>
        /// 刷新数据源
        /// </summary>
        private void DataSourceRefresh()
        {
            DataTable table;
            try { table = dataService.GetAllOpcItemsOrderbyId(); }
            catch (Exception e)
            {
                string errorMessage = "查询时出错：" + e.Message;
                MessageBox.Show(errorMessage, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dataGridView_Main.DataSource = table;
        }

        private void Button_ServerEnum_Click(object sender, EventArgs e)
        {
            ServerEnum();
        }

        private void ServerEnum()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox_OpcServerIp.Text))
                {
                    MessageBox.Show("请输入IP地址！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (textBox_OpcServerIp.Text != "localhost" && !RegexMatcher.IsIpAddressValidated(textBox_OpcServerIp.Text))//用正则表达式验证IP地址
                {
                    MessageBox.Show("请输入正确格式的IP地址！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                comboBox_OpcServerList.Items.Clear();//清空已显示的OPC Server列表
                IpAddress = textBox_OpcServerIp.Text;
                if (KepServer == null)
                    KepServer = new OPCServer();
                Array array = (Array)(object)KepServer.GetOPCServers(IpAddress);
                //假如Server列表为空，退出方法，否则为ListBoxControl添加Item
                if (array.Length == 0)
                    return;
                comboBox_OpcServerList.Items.AddRange(array.Cast<string>().ToArray());
                comboBox_OpcServerList.SelectedIndex = 0;
            }
            //假如获取OPC Server过程中引发COMException，即代表无法连接此IP的OPC Server
            catch (Exception ex)
            {
                MessageBox.Show("无法连接此IP地址的OPC Server！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Button_Connect_Click(object sender, EventArgs e)
        {
            if (KepServer == null)
                KepServer = new OPCServer();
            string server = string.IsNullOrWhiteSpace(comboBox_OpcServerList.Text) ? comboBox_OpcServerList.SelectedText : comboBox_OpcServerList.Text;
            string ip = textBox_OpcServerIp.Text;
            try
            {
                KepServer.Connect(server, ip);
                string message = string.Format("位于{0}的OPC服务{1}连接成功", ip, server);
                MessageBox.Show(message, "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("位于{0}的OPC服务{1}连接失败：{2}", ip, server, ex.Message));
                //throw;
            }
        }

        private void TextBox_OpcServerIp_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (e.KeyCode == Keys.Enter)
                button_ServerEnum.PerformClick();
        }

        /// <summary>
        /// OPC服务端连接配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_SaveServerInfo_Click(object sender, EventArgs e)
        {
            try
            {
                BaseConst.IniHelper.WriteData("OPC", "ServerIp", textBox_OpcServerIp.Text);
                BaseConst.IniHelper.WriteData("OPC", "ServerName", string.IsNullOrWhiteSpace(comboBox_OpcServerList.Text) ? comboBox_OpcServerList.SelectedText : comboBox_OpcServerList.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("OPC服务端配置保存出现问题. " + ex.Message, "错误提示");
                return;
            }

            MessageBox.Show("OPC服务端配置保存成功", "提示");
        }

        //private void CheckBox_WriteItemValue_CheckedChanged(object sender, EventArgs e)
        //{
        //    BaseConst.IniHelper.WriteData("OPC", "WriteItemValue", checkBox_WriteItemValue.Checked ? "1" : "0");
        //}

        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //取消事件，完成代码处理后再添加事件（代码中改变单元格的值会导致死循环）
                dataGridView_Main.CellValueChanged -= new DataGridViewCellEventHandler(DataGridView_CellValueChanged);
                dataGridView_Main.Rows[e.RowIndex].Cells["Column_Changed"].Value = 1;
                dataGridView_Main.CellValueChanged += new DataGridViewCellEventHandler(DataGridView_CellValueChanged);
            }
        }

        /// <summary>
        /// 新增按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Add_Click(object sender, EventArgs e)
        {
            //按数据库字段的顺序排列，最后一列为remark
            object[] values = new object[] { 0, string.Empty, 1, string.Empty, 0, 0 };
            ((DataTable)dataGridView_Main.DataSource).Rows.Add(values);
            if (dataGridView_Main.Rows.Count == 0)
                return;
            dataGridView_Main.Rows[dataGridView_Main.Rows.Count - 1].Selected = true;
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Save_Click(object sender, EventArgs e)
        {
            if (dataGridView_Main.Rows.Count == 0)
                return;

            List<OpcItem> list = new List<OpcItem>();
            foreach (DataGridViewRow row in dataGridView_Main.Rows)
            {
                object obj = row.Cells["Column_RecordId"].Value;
                obj = row.Cells["Column_Changed"].Value;
                //找到新增或修改行
                if (row.Cells["Column_RecordId"].Value.ToString().Equals("0") || row.Cells["Column_Changed"].Value.ToString().Equals("1"))
                {
                    OpcItem item = DataGridViewUtil.ConvertDataGridViewRow2Obect<OpcItem>(row, false); //不抛出异常
                    //item.ConnectionMode = (ConnectionMode)int.Parse(row.Cells["Column_ConnectionMode"].Value.ToString()); //连接模式
                    //item.Direction = (Directions)int.Parse(row.Cells["Column_Direction"].Value.ToString()); //单独处理雷达朝向字段
                    if (item.OpcGroupId.Between(1, 2)) list.Add(item);
                    else
                    {
                        MessageBox.Show("所属OPC组不得为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            bool result;
            try { result = dataService.SaveOpcItems(list); }
            catch (Exception ex)
            {
                string errorMessage = "保存时出现问题：" + ex.Message;
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

        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Delete_Click(object sender, EventArgs e)
        {
            if (dataGridView_Main.CurrentRow == null)
            {
                MessageBox.Show("未选中任何行", "提示");
                return;
            }
            int number = int.Parse(dataGridView_Main.CurrentRow.Cells["Column_RecordId"].Value.ToString()); //ID
            //假如为新增未保存的行，直接删除
            if (number == 0)
            {
                dataGridView_Main.Rows.Remove(dataGridView_Main.CurrentRow);
                return;
            }
            try { number = dataService.DeleteOpcItemById(number); }
            catch (Exception ex)
            {
                MessageBox.Show("删除记录时出现问题 " + ex.Message);
                return;
            }
            MessageBox.Show(number > 0 ? "删除成功" : "删除失败");
            if (number > 0)
                DataSourceRefresh();
        }

        private void Button_BrowseFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog_DbFile.ShowDialog() != DialogResult.OK)
                return;
            richTextBox_FolderPath.Text = openFileDialog_DbFile.FileName;
            dataService.SetFilePath(openFileDialog_DbFile.FileName);
            DataSourceRefresh();
        }

        private void DataGridView_Main_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!dataGridView_Main.Columns[e.ColumnIndex].Name.Equals("Column_GetValue"))
                return;
            string itemName = dataGridView_Main.Rows[e.RowIndex].Cells["Column_ItemId"].Value.ToString();

            #region 添加Item
            try
            {
                if (KepServer.ServerState != (int)OPCServerState.OPCRunning)
                {
                    MessageBox.Show("OPC服务未连接");
                    return;
                }

                KepServer.OPCGroups.RemoveAll();
                KepGroup = KepServer.OPCGroups.Add("KepGroup");

                string[] items = new string[] { itemName };
                Array lErrors;
                int count = items.Length;
                string[] strItemIDs = new string[count + 1];
                int[] lClientHandlers = new int[count + 1];

                //strItemIDs[0] = items[0];
                for (int i = 1; i <= count; i++)
                {
                    lClientHandlers[i] = i;
                    strItemIDs[i] = items[i - 1];
                }

                Array strit = strItemIDs.ToArray(), lci = lClientHandlers.ToArray();
                KepGroup.OPCItems.AddItems(items.Length, ref strit, ref lci, out lServerHandlers, out lErrors);
                KepGroup.IsSubscribed = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("为OPC Server{0}添加OPC Item时出错：{1}", KepServer == null ? " " : KepServer.ServerName, KepServer == null ? "OPC Server为空" : ex.Message));
                return;
            }
            #endregion
            #region 获取Item的值
            try
            {
                object quality, timestamp;

                Array itemValues = new string[KepGroup.OPCItems.Count], errors;

                if (KepGroup.OPCItems.Count > 0)
                    KepGroup.SyncRead(1, KepGroup.OPCItems.Count, ref lServerHandlers, out itemValues, out errors, out quality, out timestamp);

                if (itemValues.Length > 0)
                    dataGridView_Main.Rows[e.RowIndex].Cells["Column_ItemValue"].Value = itemValues.GetValue(1).ToString();
                else
                    MessageBox.Show("未找到该OPC Item的值");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("获取OPC Item的值时出现错误：{0}", ex.Message));
                //throw;
            }
            #endregion
        }
    }
}
