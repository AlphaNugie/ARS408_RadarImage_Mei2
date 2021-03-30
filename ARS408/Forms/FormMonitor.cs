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
        private uint radarState, bucketAlarms, armAlarms, feetAlarms;
        private readonly Thread threadUpdateItems, threadWriteItems;
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
            threadWriteItems = new Thread(new ThreadStart(WriteItemValuesLoop)) { IsBackground = true };
            AddGroupItemsAsync();
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
                    TopicName = row["topic_name"].ToString(),
                    TopicNameWalkingPos = row["topic_name_walking_pos"].ToString(),
                    TopicNamePitchAngle = row["topic_name_pitch_angle"].ToString(),
                    TopicNameStretchLength = row["topic_name_stretch_length"].ToString(),
                    TopicNameBucketPitch = row["topic_name_bucket_pitch"].ToString(),
                    TopicNameBucketYaw = row["topic_name_bucket_yaw"].ToString(),
                    TopicNameBeltSpeed = row["topic_name_belt_speed"].ToString(),
                    TopicNameStream = row["topic_name_stream"].ToString(),
                    ItemNameWalkingPos = row["item_name_walking_pos"].ToString(),
                    ItemNamePitchAngle = row["item_name_pitch_angle"].ToString(),
                    ItemNameStretchLength = row["item_name_stretch_length"].ToString(),
                    ItemNameBucketPitch = row["item_name_bucket_pitch"].ToString(),
                    ItemNameBucketYaw = row["item_name_bucket_yaw"].ToString(),
                    ItemNameBeltSpeed = row["item_name_belt_speed"].ToString(),
                    ItemNameStream = row["item_name_stream"].ToString()
                };
            }
            Shiploader = loader;
        }

        private void DataSourceRefresh()
        {
            try
            {
                DataSource = dataService.GetAllLevels();
            }
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

            Form display = radar != null ? BaseConst.DictForms[radar] : form;

            //在TabControl中显示包含该页面的TabPage
            TabPage page = new TabPage();
            display.TopLevel = false; //不置顶
            display.Dock = radar != null ? DockStyle.Fill : DockStyle.None; //控件停靠方式
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
            OpcTask.Init();
            OpcTask.Run();
            label_opc.SafeInvoke(() => label_opc.Text = OpcTask.ErrorMessage);
            //new Thread(new ThreadStart(() =>
            //{
            //    OpcTask.Init();
            //    label_opc.SafeInvoke(() => label_opc.Text = OpcTask.LastErrorMessage);
            //}))
            //{ IsBackground = true }.Start();
        }

        /// <summary>
        /// 异步添加OPC组与标签
        /// </summary>
        public void AddGroupItemsAsync()
        {
            new Thread(new ThreadStart(() =>
            {
                AddGroupItems();
                if (OpcTask != null && !string.IsNullOrWhiteSpace(OpcTask.ErrorMessage))
                    label_opc.SafeInvoke(() => { label_opc.Text = OpcTask.ErrorMessage; });
                if (BaseConst.RadarList != null && BaseConst.RadarList.Count > 0 && OpcTask != null)
                    threadWriteItems.Start();
            }))
            { IsBackground = true }.Start();
        }

        /// <summary>
        /// 添加OPC组与标签
        /// </summary>
        /// <returns></returns>
        public bool AddGroupItems()
        {
            bool result = false;
            if (Shiploader == null || BaseConst.RadarList == null || BaseConst.RadarList.Count == 0 || OpcTask == null)
                return result;
            try
            {
                OpcGroup = OpcTask.OpcHelper.OpcServer.OPCGroups.Add("Group_Radar_All");
                string basic = "[" + Shiploader.TopicName + "]" + "{0}";
                OpcItemNames = new List<string>() { string.Format(basic, "ANTICOLL_SYS.SL_SystoPLC_HMBLeiDaZhuangtai"), string.Format(basic, "ANTICOLL_SYS.SL_SystoPLC_LiuTongFangPeng"), string.Format(basic, "ANTICOLL_SYS.SL_SystoPLC_BiJiaFangPeng"), string.Format(basic, "ANTICOLL_SYS.SL_SystoPLC_MenTuiFangPeng") };
                OpcItemNames.AddRange(BaseConst.RadarList.Select(r => string.Format(basic, string.Format("ANTICOLL_SYS.Spare_Real[{0}]", 10 + r.Id))));

                int count = OpcItemNames.Count;
                string[] itemIds = new string[count + 1];
                int[] clientHandlers = new int[count + 1];

                for (int i = 1; i <= count; i++)
                {
                    clientHandlers[i] = i;
                    itemIds[i] = OpcItemNames[i - 1];
                }

                Array errors, strit = itemIds.ToArray(), lci = clientHandlers.ToArray();
                OpcGroup.OPCItems.AddItems(count, ref strit, ref lci, out ServerHandles, out errors);
                OpcGroup.IsSubscribed = true;
                OpcGroup.UpdateRate = 30;
            }
            catch (Exception e)
            {
                string errorMessage = "添加OPC组与标签时出现问题. " + e.Message;
                FileClient.WriteExceptionInfo(e, errorMessage, false);
                return result;
            }
            return !result;
        }

        private void UpdateItemsLoop()
        {
            //int interval = 50;
            while (true)
            {
                UpdateItems();
                //Thread.Sleep(interval);
                Thread.Sleep(BlockConst.ProcessInternal);
            }
        }

        private readonly Distances _leftDists = new Distances(), _rightDists = new Distances();
        private void UpdateItems()
        {
            if (BaseConst.RadarList == null)
                return;

            StringBuilder states = new StringBuilder(), buckets = new StringBuilder(), arms = new StringBuilder(), feet = new StringBuilder();
            foreach (Radar radar in BaseConst.RadarList)
            {
                if (radar == null)
                    continue;
                states.Insert(0, radar.State.Working);
                if (radar.GroupType == RadarGroupType.Wheel)
                    buckets.Insert(0, radar.ThreatLevelBinary);
                else if (radar.GroupType == RadarGroupType.Arm)
                    arms.Insert(0, radar.ThreatLevelBinary);
                else
                    feet.Insert(0, radar.ThreatLevelBinary);
            }
            radarState = Convert.ToUInt32(states.Length > 0 ? states.ToString() : "0", 2);
            bucketAlarms = Convert.ToUInt32(buckets.Length > 0 ? buckets.ToString() : "0", 2);
            armAlarms = Convert.ToUInt32(arms.Length > 0 ? arms.ToString() : "0", 2);
            feetAlarms = Convert.ToUInt32(feet.Length > 0 ? feet.ToString() : "0", 2);

            BaseFunc.ProcessBlockUnits();
            #region 网格化处理（注释）
            //BlockConst.Blocks.Clear();
            //BlockConst.CommonBlocks.Clear();
            //BlockConst.BlockClusters.Clear();
            //_leftDists.ResetDistances(BlockConst.DefaultDistance);
            //_rightDists.ResetDistances(BlockConst.DefaultDistance);
            ////遍历雷达并将雷达扫描点填入网格单元中
            //foreach (var radar in BaseConst.RadarList)
            //{
            //    if (radar.GroupType != RadarGroupType.Arm)
            //        continue;
            //    List<SensorGeneral> list = null;
            //    //try { list = radar.Infos.ListToSendAll.ToList(); }
            //    try { list = radar.Infos.ListToSend.ToList(); }
            //    catch (Exception) { }
            //    if (list == null)
            //        continue;
            //    //List<SensorGeneral> list = radar.Infos.ListToSend.ToList();
            //    foreach (var general in list)
            //    {
            //        //假如单点测距在大臂范围内或测距临界值之外，则跳过
            //        if (general == null || general.DistanceToBorder <= BlockConst.MainArmScopeX || (BaseConst.BorderDistThres > 0 && general.DistanceToBorder >= BaseConst.BorderDistThres))
            //            continue;
            //        //根据点坐标找到其应归属的网格单元列索引、行索引；假如新的行列索引超出索引范围（小于0或大于等于网格矩阵尺寸）直接前往下一个循环
            //        int columnIndex = (int)Math.Floor((general.X - BlockConst.UpLeftCorner[0]) / BlockConst.UnitSize[0]), rowIndex = (int)Math.Floor((BlockConst.UpLeftCorner[1] - general.Y) / BlockConst.UnitSize[1]);
            //        if (columnIndex < 0 || columnIndex >= BlockConst.MatrixSize[0] || rowIndex < 0 || rowIndex >= BlockConst.MatrixSize[1])
            //            continue;
            //        BlockUnit block = BlockConst.Blocks[columnIndex, rowIndex];
            //        block.AddSensorGeneral(general);
            //        if (block.TypeChanged && block.Type == BlockType.Common)
            //            BlockConst.CommonBlocks.Add(block);
            //        //if (block.TypeChanged && block.Type == BlockType.Core)
            //        //    BlockConst.BlockClusters.Add(new BlockCluster(block));
            //    }
            //}
            //BlockConst.CommonBlocks = BaseFunc.GetOutlierFilteredBlocks(BlockConst.CommonBlocks); //对符合第一阈值的网格单元进行统计滤波
            ////记录核心网格
            //foreach (BlockUnit block in BlockConst.CommonBlocks)
            //    if (block.Type == BlockType.Core)
            //        BlockConst.BlockClusters.Add(new BlockCluster(block));
            //foreach (var cluster in BlockConst.BlockClusters)
            //{
            //    BlockUnit core = cluster.CoreBlock;
            //    if (core == null)
            //        continue;
            //    //int columnIndex = cluster.CoreBlock.ColumnIndex, rowIndex = cluster.CoreBlock.RowIndex;
            //    int columnIndex = 0, rowIndex = 0;
            //    ////遍历核心网格周围5x5的网格单元（从核心网格向四周延伸2格）
            //    //for (int i = -2; i <= 2; i++)
            //    //{
            //    //    for (int j = -2; j <= 2; j++)
            //    //    {
            //    //        columnIndex = core.ColumnIndex + j;
            //    //        rowIndex = core.RowIndex + i;
            //    //        //假如新的行列索引超出索引范围（小于0或大于等于网格矩阵尺寸），假如i与j均为0（代表循环将回到当前网格），直接前往下一个循环
            //    //        if (columnIndex < 0 || columnIndex >= BlockConst.MatrixSize[0] || rowIndex < 0 ||rowIndex >= BlockConst.MatrixSize[1] || (i == 0 && j == 0))
            //    //            continue;
            //    //        cluster.AddCommonBlock(BlockConst.Blocks[columnIndex, rowIndex]);
            //    //    }
            //    //}
            //    //遍历核心网格周围半径内的网格单元（从核心网格向四周延伸若干格，具体数量见配置文件）
            //    for (int i = 0 - BlockConst.BlockClusterRadius[1]; i <= BlockConst.BlockClusterRadius[1]; i++)
            //    {
            //        for (int j = 0 - BlockConst.BlockClusterRadius[0]; j <= BlockConst.BlockClusterRadius[0]; j++)
            //        {
            //            columnIndex = core.ColumnIndex + j;
            //            rowIndex = core.RowIndex + i;
            //            //假如新的行列索引超出索引范围（小于0或大于等于网格矩阵尺寸），假如i与j均为0（代表循环将回到当前网格），直接前往下一个循环
            //            if (columnIndex < 0 || columnIndex >= BlockConst.MatrixSize[0] || rowIndex < 0 ||rowIndex >= BlockConst.MatrixSize[1] || (i == 0 && j == 0))
            //                continue;
            //            cluster.AddCommonBlock(BlockConst.Blocks[columnIndex, rowIndex]);
            //        }
            //    }
            //    //遍历完四周网格单元后更新网格聚类属性，迭代左右距离值
            //    cluster.RefreshProperties();
            //    if (cluster.Type == BlockClusterType.Normal && cluster.CenterX < 0)
            //        _leftDists.Iterate(cluster.Distances);
            //    else if (cluster.Type == BlockClusterType.Normal && cluster.CenterX > 0)
            //        _rightDists.Iterate(cluster.Distances);
            //    //if (cluster.Type != BlockClusterType.MainArm && cluster.CenterX < 0)
            //    //    _leftDists.Iterate(cluster.Distances);
            //    //else if (cluster.Type != BlockClusterType.MainArm && cluster.CenterX > 0)
            //    //    _rightDists.Iterate(cluster.Distances);
            //}
            //BlockConst.DistancesLeft.Copy(_leftDists);
            //BlockConst.DistancesRight.Copy(_rightDists);
            #endregion
        }

        /// <summary>
        /// 向OPC服务写入OPC项的值
        /// </summary>
        private void WriteItemValuesLoop()
        {
            while (true)
            {
                Thread.Sleep(400);
                if (!BaseConst.WriteItemValue)
                    continue;
                try { WriteItemValues(); }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 向PLC写入信息
        /// </summary>
        public void WriteItemValues()
        {
            if (BaseConst.RadarList == null || BaseConst.RadarList.Count == 0 || OpcTask == null || OpcGroup == null)
                return;

            try
            {
                //假如未添加任何OPC项
                if (OpcGroup.OPCItems.Count == 0)
                    return;

                List<object> values = new List<object>() { 0, radarState, bucketAlarms, armAlarms, feetAlarms };
                values.AddRange(BaseConst.RadarList.Select(r => (object)r.CurrentDistance));
                Array itemValues = values.ToArray(), errors;
                OpcGroup.SyncWrite(OpcItemNames.Count, ref ServerHandles, ref itemValues, out errors);
            }
            catch (Exception ex)
            {
                string info = string.Format("OPC写入时出现问题. {0}. ip_address: {1}", ex.Message, OpcTask.Shiploader.OpcServerIp);
                label_opc.SafeInvoke(() => { label_opc.Text = info; });
                FileClient.WriteExceptionInfo(ex, info, false);
            }
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
