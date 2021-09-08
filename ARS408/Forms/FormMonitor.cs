using ARS408.Core;
using ARS408.Model;
using CommonLib.Clients;
using CommonLib.Extensions;
using CommonLib.Function;
using CommonLib.UIControlUtil;
using OPCAutomation;
using ProtobufNetLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARS408.Forms
{
    public partial class FormMonitor : Form
    {
        public delegate void ShowFormHandler(Radar radar, Form form);

        #region 私有成员
        private readonly DataService_Sqlite dataService = new DataService_Sqlite();
        private readonly string parent_field = "parent_id"; //上级关联字段
        private readonly string key_field = "id"; //本级关联字段
        private readonly string display_field = "name"; //显示字段
        private readonly float column_width = 0;
        private readonly int shiploader_id = 0;
        //private uint radarState, bucketAlarms, armAlarms, feetAlarms;
        private readonly Thread threadUpdateItems/*, threadWriteItems*/;
        #endregion

        #region 属性
        /// <summary>
        /// 对应装船机对象
        /// </summary>
        private Shiploader Shiploader { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public DataTable DataSource { get; private set; }

        /// <summary>
        /// 是否正在加载
        /// </summary>
        public bool Loading { get; private set; }

        /// <summary>
        /// OPC工具
        /// </summary>
        public OpcTask OpcTask { get; private set; }

        /// <summary>
        /// OPC组
        /// </summary>
        public OPCGroup OpcGroup { get; set; }

        /// <summary>
        /// 所有待添加OPC标签名称
        /// </summary>
        public List<string> OpcItemNames { get; set; }

        /// <summary>
        /// OPC标签的服务端句柄
        /// </summary>
        public Array ServerHandles;
        #endregion

        public FormMonitor(int shiploader_id)
        {
            InitializeComponent();
            BaseConst.Log.WriteLogsToFile("监视页面初始化中...shiploader_id: " + shiploader_id);
            this.shiploader_id = shiploader_id;
            column_width = tableLayoutPanel_Main.ColumnStyles[0].Width;
            Loading = true;
            DataSource = null;
            UpdateShiploader();
            InitOpcTask();
            DataSourceRefresh();

            threadUpdateItems = new Thread(new ThreadStart(UpdateItemsLoop)) { IsBackground = true };
            //threadWriteItems = new Thread(new ThreadStart(WriteItemValuesLoop)) { IsBackground = true };
            //AddGroupItemsAsync();
            threadUpdateItems.Start();
            //threadWriteitems.Start();

            if (BaseConst.AutoConnect)
                StartOrEnd(true);

            BaseConst.Log.WriteLogsToFile("监视页面初始化完成, shiploader_id: " + shiploader_id);
        }

        public FormMonitor() : this(0) { }

        private void FormMonitor_Load(object sender, EventArgs e) { }

        #region 功能
        /// <summary>
        /// 获取当前装船机对象
        /// </summary>
        /// <returns></returns>
        private void UpdateShiploader()
        {
            DataTable table = (new DataService_Shiploader()).GetShiploader(shiploader_id);
            Shiploader loader = null;
            if (table != null && table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                loader = new Shiploader()
                {
                    Id = int.Parse(row["shiploader_id"].ToString()),
                    Name = row["shiploader_name"].ToString(),
                    OpcServerIp = row["opcserver_ip"].ToString(),
                    OpcServerName = row["opcserver_name"].ToString(),
                };
            }
            Shiploader = loader;
        }

        private void DataSourceRefresh()
        {
            try { DataSource = dataService.GetAllLevels(); }
            catch (Exception e)
            {
                string errorMessage = "查询时出现问题：" + e.Message;
                MessageBox.Show(errorMessage, "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            treeView_Main.BindTreeViewDataSource(DataSource, parent_field, key_field, display_field);
            Loading = false;
        }

        /// <summary>
        /// 全部开始或全部结束
        /// </summary>
        /// <param name="flag">假如为true则开始，否则结束</param>
        private void StartOrEnd(bool flag)
        {
            //bool flag = button_StartOrEnd.Text.Equals("开始");
            BaseConst.Log.WriteLogsToFile("通讯操作开始，全部" + (flag ? "开始" : "结束"));
            foreach (FormDisplay form in BaseConst.DictForms.Values)
            {
                try { form.StartOrEndReceiving(flag); }
                catch (Exception) { }
            }
            button_StartOrEnd.Text = flag ? "结束" : "开始";
        }

        /// <summary>
        /// 根据雷达信息查询窗体，找到则在TabPage页中加载窗体对象
        /// </summary>
        /// <param name="radar">雷达信息</param>
        private void ShowForm(Radar radar)
        {
            ShowForm(radar, null);
        }

        /// <summary>
        /// 显示指定窗体
        /// </summary>
        /// <param name="form">待显示的窗体</param>
        private void ShowForm(Form form)
        {
            ShowForm(null, form);
        }

        /// <summary>
        /// 根据雷达信息查询窗体，找到则在TabPage页中加载窗体对象，没有雷达信息则显示默认窗体
        /// </summary>
        /// <param name="radar">雷达信息</param>
        /// <param name="form">没有雷达信息为空时显示的窗体</param>
        private void ShowForm(Radar radar, Form form)
        {
            if (tabControl_Main.InvokeRequired)
            {
                ShowFormHandler handler = new ShowFormHandler(ShowForm);
                Invoke(handler, radar);
                return;
            }

            //假如雷达信息与窗体对象均为空，不作处理
            if (radar == null && form == null)
            {
                MessageBox.Show("雷达信息无效，无法显示窗体", "提示消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string name = radar != null ? radar.Name : form.Name;
            //假如Tab页已存在，选中该页面
            foreach (TabPage tabPage in tabControl_Main.TabPages)
            {
                if (tabPage.Name.Equals(name))
                {
                    tabControl_Main.SelectedTab = tabPage;
                    return;
                }
            }

            Form display = radar != null ? BaseConst.DictForms[radar.Id] : form;

            //在TabControl中显示包含该页面的TabPage
            TabPage page = new TabPage();
            display.TopLevel = false; //不置顶
            //display.Dock = radar != null ? DockStyle.Fill : DockStyle.None; //控件停靠方式
            display.Dock = DockStyle.Fill; //控件停靠方式
            display.FormBorderStyle = FormBorderStyle.None; //页面无边框
            page.Controls.Add(display);
            page.Text = display.Text;
            page.Name = name;
            page.AutoScroll = true;

            tabControl_Main.TabPages.Add(page);
            tabControl_Main.SelectedTab = page;
            display.Show();
            if (display is FormDisplay)
                ((FormDisplay)display).IsShown = true;
        }

        /// <summary>
        /// 移除所有控件后释放TabPage（防止释放控件）
        /// </summary>
        /// <param name="page">待释放的TabPage对象</param>
        private void DisposeTabPage(TabPage page)
        {
            Control control = page.Controls[0];
            //雷达信息分页不关闭
            if (control is Form && control.Name.Equals("FormInfo"))
                return;
            if (control is FormDisplay)
                ((FormDisplay)control).IsShown = false;
            else
                ((Form)control).Close();
            page.Controls.Clear();
            page.Dispose();
        }

        /// <summary>
        /// 释放所有TabPage对象
        /// </summary>
        private void DisposeTabPages_all()
        {
            foreach (TabPage page in tabControl_Main.TabPages) DisposeTabPage(page);
        }
        #endregion

        #region OPC
        /// <summary>
        /// OPC初始化
        /// </summary>
        private void InitOpcTask()
        {
            OpcTask = new OpcTask(Shiploader);
            if (!BaseConst.OpcEnabled)
                return;

            OpcTask.Init();
            OpcTask.Run();
            label_opc.SafeInvoke(() => label_opc.Text = OpcTask.ErrorMessage);
        }

        private void UpdateItemsLoop()
        {
            while (true)
            {
                UpdateItems();
                Thread.Sleep(BlockConst.ProcessInternal);
            }
        }

        private readonly Distances _leftDists = new Distances(), _rightDists = new Distances();
        private void UpdateItems()
        {
            if (BaseConst.RadarList == null)
                return;

            BaseFunc.ProcessBlockUnits();
        }
        #endregion

        #region 事件
        private void FormMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeTabPages_all();
            StartOrEnd(false);
            OpcTask.Stop();
            //OpcTask.Epilogue();
        }

        private void TabControl_DoubleClick(object sender, EventArgs e)
        {
            float current = tableLayoutPanel_Main.ColumnStyles[0].Width;
            tableLayoutPanel_Main.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, column_width - current);
        }

        private void TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string name = e.Node.Text.ToString();
            //Radar radar = GetRadarByIpPort(name);
            Radar radar = BaseFunc.GetRadarByName(name);
            //假如正在加载或为根节点
            if (Loading || radar == null)
                return;

            try
            {
                DisposeTabPages_all();
                ShowForm(radar);
            }
            catch (Exception)
            {
                MessageBox.Show("初始化未完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Button_StartOrEnd_Click(object sender, EventArgs e)
        {
            StartOrEnd(button_StartOrEnd.Text.Equals("开始"));
            //bool flag = button_StartOrEnd.Text.Equals("开始");
            //foreach (FormDisplay form in BaseConst.DictForms.Values)
            //{
            //    try { form.StartOrEndReceiving(flag); }
            //    catch (Exception ex) { }
            //}
            //button_StartOrEnd.Text = flag ? "结束" : "开始";
        }

        private void Button_Info_Click(object sender, EventArgs e)
        {
            FormInfo form = new FormInfo(this);
            ShowForm(form);
        }
        #endregion
    }
}
