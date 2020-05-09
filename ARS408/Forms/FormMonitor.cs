﻿using ARS408.Core;
using ARS408.Model;
using CommonLib.Clients;
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
        ///// <summary>
        ///// 各方向距离的键值对集合
        ///// </summary>
        //public Dictionary<string, double> DictDistances { get; private set; }

        /// <summary>
        /// 对应装船机对象
        /// </summary>
        private Shiploader Shiploader { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public DataTable DataSource { get; private set; }

        ///// <summary>
        ///// 数据列表
        ///// </summary>
        //public List<Radar> RadarList { get; private set; }

        ///// <summary>
        ///// 包含所有显示窗体的键值对
        ///// </summary>
        //public Dictionary<Radar, FormDisplay> DictForms { get; private set; }

        /// <summary>
        /// 是否正在加载
        /// </summary>
        public bool Loading { get; private set; }

        /// <summary>
        /// OPC工具
        /// </summary>
        public OpcUtilHelper OpcHelper { get; private set; }

        /// <summary>
        /// OPC组
        /// </summary>
        public OPCGroup OpcGroup { get; set; }

        /// <summary>
        /// 所有待添加OPC标签名称
        /// </summary>
        public string[] OpcItemNames { get; set; }

        /// <summary>
        /// OPC标签的服务端句柄
        /// </summary>
        public Array ServerHandles;
        #endregion

        public FormMonitor(int shiploader_id)
        {
            InitializeComponent();
            this.shiploader_id = shiploader_id;
            this.column_width = this.tableLayoutPanel_Main.ColumnStyles[0].Width;
            this.Loading = true;
            this.DataSource = null;
            //BaseConst.DictForms = new Dictionary<Radar, FormDisplay>();
            //this.DictDistances = new Dictionary<string, double>() { { "DistLand", 0 }, { "DistSea", 0 }, { "DistNorth", 0 }, { "DistSouth", 0 }, { "DistLandMax", 0 }, { "DistSeaMax", 0 }, { "DistNorthMax", 0 }, { "DistSouthMax", 0 }, { "ShoreNorth", 0 }, { "ShoreSouth", 0 } };
            //this.DictDistances = new Dictionary<string, double>() { { "DistWheelLeft", 0 }, { "DistWheelRight", 0 } };
            this.UpdateShiploader();
            this.InitOpcHelper();
            this.DataSourceRefresh();

            this.threadUpdateItems = new Thread(new ThreadStart(this.UpdateItemsLoop)) { IsBackground = true };
            this.threadWriteItems = new Thread(new ThreadStart(this.WriteItemValuesLoop)) { IsBackground = true };
            this.AddGroupItemsAsync();
            this.threadUpdateItems.Start();
            //this.threadWriteitems.Start();
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
            DataTable table = (new DataService_Shiploader()).GetShiploader(this.shiploader_id);
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
            this.Shiploader = loader;
            //return loader;
        }

        /// <summary>
        /// OPC初始化
        /// </summary>
        private void InitOpcHelper()
        {
            this.OpcHelper = new OpcUtilHelper(this.Shiploader);
            new Thread(new ThreadStart(() =>
            {
                this.OpcHelper.Init();
                //if (!string.IsNullOrWhiteSpace(this.OpcHelper.LastErrorMessage))
                //    MessageBox.Show(this.OpcHelper.LastErrorMessage);
                this.label_opc.Text = this.OpcHelper.LastErrorMessage;
            }))
            { IsBackground = true }.Start();
        }

        private void DataSourceRefresh()
        {
            try
            {
                //this.DataSource = this.dataService.GetLevels(this.shiploader_id);
                //DataTable radars = (new DataService_Radar()).GetRadars(this.shiploader_id, "radar_name");
                //DataTable radars = (new DataService_Radar()).GetRadars(this.shiploader_id, "radar_id");
                //BaseConst.RadarList = radars == null ? null : radars.Rows.Cast<DataRow>().Select(row => BaseFunc.GetRadarFromDataRow(row)).ToList();
                this.DataSource = this.dataService.GetAllLevels();
                //BaseFunc.RadarListUpdate();
            }
            catch (Exception e)
            {
                string errorMessage = "查询时出现问题：" + e.Message;
                MessageBox.Show(errorMessage, "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //this.InitForms(); //初始化窗体对象
            this.treeView_Main.BindTreeViewDataSource(this.DataSource, this.parent_field, this.key_field, this.display_field);
            this.Loading = false;
        }

        //private Radar GetRadarFromDataRow(DataRow row)
        //{
        //    if (row == null)
        //        return null;

        //    Radar radar = new Radar
        //    {
        //        //OpcHelper = this.OpcHelper,
        //        Id = int.Parse(row["radar_id"].ToString()),
        //        Name = row["radar_name"].ToString(),
        //        IpAddress = row["ip_address"].ToString(),
        //        Port = ushort.Parse(row["port"].ToString()),
        //        ConnectionMode = (ConnectionMode)int.Parse(row["conn_mode_id"].ToString()),
        //        UsingLocal = row["using_local"].ToString().Equals("1"),
        //        IpAddressLocal = row["ip_address_local"].ToString(),
        //        PortLocal = int.Parse(row["port_local"].ToString()),
        //        OwnerShiploaderId = int.Parse(row["shiploader_id"].ToString()),
        //        TopicName = row["topic_name"].ToString(),
        //        OwnerGroupId = int.Parse(row["owner_group_id"].ToString()),
        //        GroupType = (RadarGroupType)int.Parse(row["group_type"].ToString()),
        //        DegreeYoz = double.Parse(row["degree_yoz"].ToString()),
        //        DegreeXoy = double.Parse(row["degree_xoy"].ToString()),
        //        DegreeXoz = double.Parse(row["degree_xoz"].ToString()),
        //        DegreeGeneral = double.Parse(row["degree_general"].ToString()),
        //        Direction = (Directions)int.Parse(row["direction_id"].ToString()),
        //        DefenseMode = int.Parse(row["defense_mode_id"].ToString()),
        //        Offset = double.Parse(row["offset"].ToString()),
        //        Remark = row["remark"].ToString(),
        //        ItemNameRadarState = row["item_name_radar_state"].ToString(),
        //        ItemNameCollisionState = row["item_name_collision_state"].ToString(),
        //        ItemNameCollisionState2 = row["item_name_collision_state_2"].ToString(),
        //        RcsMinimum = int.Parse(row["rcs_min"].ToString()),
        //        RcsMaximum = int.Parse(row["rcs_max"].ToString()),
        //        RadarHeight = double.Parse(row["radar_height"].ToString()),
        //        RadarCoorsLimited = row["radar_coors_limited"].ToString().Equals("1"),
        //        RadarxMin = double.Parse(row["radar_x_min"].ToString()),
        //        RadarxMax = double.Parse(row["radar_x_max"].ToString()),
        //        RadaryMin = double.Parse(row["radar_y_min"].ToString()),
        //        RadaryMax = double.Parse(row["radar_y_max"].ToString()),
        //        ClaimerCoorsLimited = row["claimer_coors_limited"].ToString().Equals("1"),
        //        ClaimerxMin = double.Parse(row["claimer_x_min"].ToString()),
        //        ClaimerxMax = double.Parse(row["claimer_x_max"].ToString()),
        //        ClaimeryMin = double.Parse(row["claimer_y_min"].ToString()),
        //        ClaimeryMax = double.Parse(row["claimer_y_max"].ToString()),
        //        ClaimerzMin = double.Parse(row["claimer_z_min"].ToString()),
        //        ClaimerzMax = double.Parse(row["claimer_z_max"].ToString())
        //    };
        //    return radar;
        //}

        ///// <summary>
        ///// 键值对初始化
        ///// </summary>
        //private void InitForms()
        //{
        //    //if (this.DataSource == null || this.DataSource.Rows.Count == 0)
        //    //{
        //    //    this.Loading = false;
        //    //    return;
        //    //}

        //    ////排除根节点
        //    //foreach (Radar radar in BaseConst.RadarList)
        //    //{
        //    //    if (BaseConst.DictForms.ContainsKey(radar))
        //    //        continue;

        //    //    FormDisplay form = new FormDisplay(radar);
        //    //    BaseConst.DictForms.Add(radar, form);
        //    //}
        //    this.Loading = false;
        //}


        /// <summary>
        /// 根据雷达信息查询窗体，找到则在TabPage页中加载窗体对象
        /// </summary>
        /// <param name="radar">雷达信息</param>
        private void ShowForm(Radar radar)
        {
            this.ShowForm(radar, null);
        }

        /// <summary>
        /// 显示指定窗体
        /// </summary>
        /// <param name="form">待显示的窗体</param>
        private void ShowForm(Form form)
        {
            this.ShowForm(null, form);
        }

        /// <summary>
        /// 根据雷达信息查询窗体，找到则在TabPage页中加载窗体对象，没有雷达信息则显示默认窗体
        /// </summary>
        /// <param name="radar">雷达信息</param>
        /// <param name="form">没有雷达信息为空时显示的窗体</param>
        private void ShowForm(Radar radar, Form form)
        {
            if (this.tabControl_Main.InvokeRequired)
            {
                ShowFormHandler handler = new ShowFormHandler(this.ShowForm);
                this.Invoke(handler, radar);
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
            foreach (TabPage tabPage in this.tabControl_Main.TabPages)
            {
                if (tabPage.Name.Equals(name))
                {
                    this.tabControl_Main.SelectedTab = tabPage;
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

            this.tabControl_Main.TabPages.Add(page);
            this.tabControl_Main.SelectedTab = page;
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
            foreach (TabPage page in this.tabControl_Main.TabPages) this.DisposeTabPage(page);
        }

        public string GetInfoString()
        {
            string main = string.Format(@"[
  ""walkpos"": {0},
  ""armpitch"": {1},
  ""armstretch"": {2},
  ""bucketyaw"": {3},
  ""bucketpitch"": {4},
  ""beltspeed"": {5},
  ""stream"": {6},
  斗轮左：{7}
  斗轮右：{8}
]", this.OpcHelper.WalkingPosition, this.OpcHelper.PitchAngle, this.OpcHelper.StretchLength, this.OpcHelper.BucketYaw, this.OpcHelper.BucketPitch, this.OpcHelper.BeltSpeed, this.OpcHelper.Stream, BaseConst.RadarInfo.DistWheelLeft/*this.DictDistances["DistWheelLeft"]*/, BaseConst.RadarInfo.DistWheelRight/*this.DictDistances["DistWheelRight"]*//*, this.DictDistances["DistLand"] + this.DictDistances["DistSea"] + 2.623, this.DictDistances["DistNorth"], this.DictDistances["DistSouth"], this.DictDistances["DistNorth"] + this.DictDistances["DistSouth"] + 4.831*/).Replace('[', '{').Replace(']', '}');
            return main;
        }

        ///// <summary>
        ///// 获取包含斗轮两侧雷达距离的字符串
        ///// </summary>
        ///// <returns></returns>
        //public static string GetWheelRadarDistances()
        //{
        //    BaseFunc.UpdateUniversalInfo();
        //    //string result = string.Format("radar/dist_wheel_left:{0};dist_wheel_right:{1}", Math.Round(this.DictDistances["DistWheelLeft"], 4), Math.Round(this.DictDistances["DistWheelRight"], 4));
        //    string result = string.Format("radar/dist_wheel_left:{0};dist_wheel_right:{1}", BaseConst.UniversalInfo.DistWheelLeft, BaseConst.UniversalInfo.DistWheelRight);
        //    return result;
        //}

        ///// <summary>
        ///// 更新各方向距离
        ///// </summary>
        //public static void UpdateUniversalInfo()
        //{
        //    //IEnumerable<Radar> radars = BaseConst.RadarList;
        //    BaseConst.UniversalInfo.DistWheelLeft = BaseFunc.GetMinValueExceptZero(BaseConst.RadarList.Where(r => r.GroupType == RadarGroupType.Wheel && r.Name.Contains("左")).Select(r => r.CurrentDistance)); //斗轮左距离
        //    BaseConst.UniversalInfo.DistWheelRight = BaseFunc.GetMinValueExceptZero(BaseConst.RadarList.Where(r => r.GroupType == RadarGroupType.Wheel && r.Name.Contains("右")).Select(r => r.CurrentDistance)); //斗轮右距离
        //}

        ///// <summary>
        ///// 根据Radar对象获取雷达最近距离
        ///// </summary>
        ///// <param name="radar">Radar实体类对象</param>
        ///// <returns></returns>
        //public static double GetRadarDistance(Radar radar)
        //{
        //    double dist = 0;
        //    try { dist = BaseConst.DictForms[radar].Infos.CurrentDistance; } catch (Exception) { }
        //    //if (radar != null && BaseConst.DictForms[radar] != null && BaseConst.DictForms[radar].Infos != null)
        //    //    dist = BaseConst.DictForms[radar].Infos.CurrentDistance;
        //    return dist;
        //}

  //      /// <summary>
  //      /// 根据Radar对象获取雷达信息字符串
  //      /// </summary>
  //      /// <param name="radar">Radar实体类对象</param>
  //      /// <returns></returns>
  //      public static string GetRadarString(Radar radar, out double distance)
  //      {
  //          string result = string.Empty;
  //          FormDisplay display;
  //          DataFrameMessages infos;
  //          distance = 0;
  //          if (radar != null && (display = BaseConst.DictForms[radar]) != null && (infos = display.Infos) != null)
  //          {
  //              dynamic obj_other = infos.CurrentSensorMode == SensorMode.Object ? (dynamic)infos.ObjectHighest : (dynamic)infos.ClusterHighest;
  //              double obj_height = obj_other == null ? 0 : 0 - BaseConst.BucketHeight - obj_other.ModiCoors.Z;
  //              distance = infos.CurrentDistance;
  //              result = string.Format(@"  ""radar_{0}"": [
  //""effective"": {1},
  //""distance"": {2},
  //""below"": {3}
  //],", radar.PortLocal + "_" + radar.Name, infos.RadarState.Working, distance, obj_height);
  //          }

  //          return result;
  //      }
        #endregion

        #region OPC
        /// <summary>
        /// 异步添加OPC组与标签
        /// </summary>
        public void AddGroupItemsAsync()
        {
            new Thread(new ThreadStart(() =>
            {
                this.AddGroupItems();
                if (this.OpcHelper != null && !string.IsNullOrWhiteSpace(this.OpcHelper.LastErrorMessage))
                    this.label_opc.SafeInvoke(() => { this.label_opc.Text = this.OpcHelper.LastErrorMessage; });
                if (BaseConst.RadarList != null && BaseConst.RadarList.Count > 0 && this.OpcHelper != null)
                    this.threadWriteItems.Start();
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
            if (this.Shiploader == null || BaseConst.RadarList == null || BaseConst.RadarList.Count == 0 || this.OpcHelper == null)
                return result;
            try
            {
                this.OpcGroup = this.OpcHelper.OpcServer.OPCGroups.Add("Group_Radar_All");
                string basic = "[" + this.Shiploader.TopicName + "]" + "{0}";
                List<string> names = new List<string>() { string.Format(basic, "ANTICOLL_SYS.SL_SystoPLC_HMBLeiDaZhuangtai"), string.Format(basic, "ANTICOLL_SYS.SL_SystoPLC_LiuTongFangPeng"), string.Format(basic, "ANTICOLL_SYS.SL_SystoPLC_BiJiaFangPeng"), string.Format(basic, "ANTICOLL_SYS.SL_SystoPLC_MenTuiFangPeng") };
                names.AddRange(BaseConst.RadarList.Select(r => string.Format(basic, string.Format("ANTICOLL_SYS.Spare_Real[{0}]", 10 + r.Id))));
                this.OpcItemNames = names.ToArray();

                int count = this.OpcItemNames.Length;
                string[] itemIds = new string[count + 1];
                int[] clientHandlers = new int[count + 1];

                for (int i = 1; i <= count; i++)
                {
                    clientHandlers[i] = i;
                    itemIds[i] = this.OpcItemNames[i - 1];
                }

                Array errors, strit = itemIds.ToArray(), lci = clientHandlers.ToArray();
                this.OpcGroup.OPCItems.AddItems(count, ref strit, ref lci, out ServerHandles, out errors);
                this.OpcGroup.IsSubscribed = true;
                this.OpcGroup.UpdateRate = 30;
            }
            catch (Exception e)
            {
                this.OpcHelper.LastErrorMessage = "添加OPC组与标签时出现问题. " + e.Message;
                FileClient.WriteExceptionInfo(e, this.OpcHelper.LastErrorMessage, false);
                return result;
            }
            return !result;
        }

        private void UpdateItemsLoop()
        {
            int interval = 50;
            //int interval = BaseConst.RefreshInterval;
            while (true)
            {
                this.UpdateItems();
                Thread.Sleep(interval);
            }
        }

        private void UpdateItems()
        {
            if (BaseConst.RadarList == null)
                return;

            StringBuilder states = new StringBuilder(), buckets = new StringBuilder(), arms = new StringBuilder(), feet = new StringBuilder();
            foreach (Radar radar in BaseConst.RadarList)
            {
                if (radar == null)
                    continue;
                states.Insert(0, radar.RadarState.Working);
                if (radar.GroupType == RadarGroupType.Wheel)
                    buckets.Insert(0, radar.ThreatLevelBinary);
                else if (radar.GroupType == RadarGroupType.Arm)
                    arms.Insert(0, radar.ThreatLevelBinary);
                else
                    feet.Insert(0, radar.ThreatLevelBinary);
            }
            this.radarState = Convert.ToUInt32(states.Length > 0 ? states.ToString() : "0", 2);
            this.bucketAlarms = Convert.ToUInt32(buckets.Length > 0 ? buckets.ToString() : "0", 2);
            this.armAlarms = Convert.ToUInt32(arms.Length > 0 ? arms.ToString() : "0", 2);
            this.feetAlarms = Convert.ToUInt32(feet.Length > 0 ? feet.ToString() : "0", 2);

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
                try { this.WriteItemValues(); }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 向PLC写入信息
        /// </summary>
        public void WriteItemValues()
        {
            if (BaseConst.RadarList == null || BaseConst.RadarList.Count == 0 || this.OpcHelper == null || this.OpcGroup == null)
                return;

            try
            {
                //假如未添加任何OPC项
                if (this.OpcGroup.OPCItems.Count == 0)
                    return;

                List<object> values = new List<object>() { 0, this.radarState, this.bucketAlarms, this.armAlarms, this.feetAlarms };
                values.AddRange(BaseConst.RadarList.Select(r => (object)r.CurrentDistance));
                Array itemValues = values.ToArray(), errors;
                this.OpcGroup.SyncWrite(this.OpcItemNames.Length, ref this.ServerHandles, ref itemValues, out errors);
            }
            catch (Exception ex)
            {
                string info = string.Format("OPC写入时出现问题. {0}. ip_address: {1}", ex.Message, this.OpcHelper.Shiploader.OpcServerIp);
                this.label_opc.SafeInvoke(() => { this.label_opc.Text = info; });
                FileClient.WriteExceptionInfo(ex, info, false);
            }
        }
        #endregion

        #region 事件
        private void FormMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.OpcHelper.Epilogue();
        }

        private void TabControl_DoubleClick(object sender, EventArgs e)
        {
            float current = this.tableLayoutPanel_Main.ColumnStyles[0].Width;
            this.tableLayoutPanel_Main.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, this.column_width - current);
        }

        private void TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string name = e.Node.Text.ToString();
            //Radar radar = this.GetRadarByIpPort(name);
            Radar radar = BaseFunc.GetRadarByName(name);
            //假如正在加载或为根节点
            if (this.Loading || radar == null)
                return;

            try
            {
                this.DisposeTabPages_all();
                this.ShowForm(radar);
            }
            catch (Exception)
            {
                MessageBox.Show("初始化未完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Button_StartOrEnd_Click(object sender, EventArgs e)
        {
            bool flag = this.button_StartOrEnd.Text.Equals("开始");
            foreach (FormDisplay form in BaseConst.DictForms.Values)
            {
                try { form.StartOrEndReceiving(flag); }
                catch (Exception ex) { }
            }
            this.button_StartOrEnd.Text = flag ? "结束" : "开始";
        }

        private void Button_Info_Click(object sender, EventArgs e)
        {
            FormInfo form = new FormInfo(this);
            this.ShowForm(form);
        }
        #endregion
    }
}
