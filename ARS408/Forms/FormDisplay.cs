using ARS408.Core;
using ARS408.Model;
using CommonLib.Clients;
using CommonLib.Events;
using CommonLib.Extensions;
using CommonLib.UIControlUtil;
using OPCAutomation;
using SocketHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace ARS408.Forms
{
    public partial class FormDisplay : Form
    {
        #region 私有变量
        private bool finalized = false;
        //private readonly Regex pattern = new Regex(BaseConst.Pattern_WrappedStatus, RegexOptions.Compiled);
        private readonly DataService_Sqlite dataService = new DataService_Sqlite();
        private readonly DataService_Radar dataService_Radar = new DataService_Radar();
        private List<SensorGeneral> list_general = null;
        private Bitmap bitmap = null;
        private Graphics graphic = null;
        private string received = string.Empty, wrapped = string.Empty;
        private Thread thread = null/*, thread_writeitems = null*/;
        private float scale = 1, column_width = 0;
        private readonly float scale_original = 1;
        private int time = 0; //重连次数
        #endregion
        #region 属性
        /// <summary>
        /// 标题栏原始标题
        /// </summary>
        public string Title { get { return Radar.Name; } }

        /// <summary>
        /// 帧消息处理类
        /// </summary>
        public DataFrameMessages Infos { get { return Radar.Infos; } }
        //public DataFrameMessages Infos { get; private set; }

        /// <summary>
        /// 雷达信息对象，假如为null，则代表为单雷达显示模式
        /// </summary>
        public Radar Radar { get; set; }

        #region 连接
        /// <summary>
        /// UDP客户端
        /// </summary>
        public DerivedUdpClient UdpClient { get; set; }

        /// <summary>
        /// 远端IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 远端端口
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// 连接模式
        /// </summary>
        public ConnectionMode ConnectionMode { get; set; }

        /// <summary>
        /// 是否制定本地IP与端口
        /// </summary>
        public bool UsingLocal { get; set; }

        /// <summary>
        /// 本地IP地址
        /// </summary>
        public string IpAddress_Local { get; set; }

        /// <summary>
        /// 本地端口
        /// </summary>
        public int Port_Local { get; set; }
        #endregion

        /// <summary>
        /// 当前的画面比例
        /// </summary>
        public float S
        {
            get { return scale; }
            set
            {
                scale = value;
                PaintAll();
            }
        }

        /// <summary>
        /// 是否在拖拽图片
        /// </summary>
        public bool PictureMoving { get; set; }

        /// <summary>
        /// 是否在显示
        /// </summary>
        public bool IsShown { get; set; }

        //private int _rcsMinimum = -64;
        //private int _rcsMaximum = 64;

        /// <summary>
        /// RCS最小值
        /// </summary>
        public int RcsMinimum
        {
            //是否使用公共RCS值范围
            get { return BaseConst.UsePublicRcsRange ? BaseConst.RcsMinimum : Infos.RcsMinimum; }
            set
            {
                if (BaseConst.UsePublicRcsRange)
                    BaseConst.RcsMinimum = value;
                else
                {
                    Infos.RcsMinimum = value;
                    dataService_Radar.UpdateRadarRcsMinById(Infos.RcsMinimum, Radar.Id); //向数据库保存RCS值最小值
                }
            }
        }

        /// <summary>
        /// RCS最大值
        /// </summary>
        public int RcsMaximum
        {
            get { return BaseConst.UsePublicRcsRange ? BaseConst.RcsMaximum : Infos.RcsMaximum; }
            set
            {
                if (BaseConst.UsePublicRcsRange)
                    BaseConst.RcsMaximum = value;
                else
                {
                    Infos.RcsMaximum = value;
                    dataService_Radar.UpdateRadarRcsMaxById(Infos.RcsMaximum, Radar.Id); //向数据库保存RCS值最小值
                }
            }
        }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="radar">雷达信息对象</param>
        public FormDisplay(Radar radar)
        {
            InitializeComponent();
            Radar = radar == null ? new Radar() : radar;

            Init();
            InitControls();
        }

        /// <summary>
        /// 窗体加载后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormDisplay_Load(object sender, EventArgs e) { }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            Finalizing();
        }

        #region 功能
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            IpAddress = Radar.IpAddress;
            Port = Radar.Port;
            ConnectionMode = Radar.ConnectionMode;
            UsingLocal = Radar.UsingLocal;
            IpAddress_Local = BaseConst.IpAddress_Local;
            Port_Local = Radar.PortLocal;
            //Infos.RcsMinimum = Radar.RcsMinimum;
            //Infos.RcsMaximum = Radar.RcsMaximum;

            column_width = tableLayoutPanel_Main.ColumnStyles[0].Width;
            //Infos = new DataFrameMessages(/*this, */Radar);
            list_general = Infos.ListTrigger;
            Name = Title;
            Text = Title;
            S = scale_original;
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitControls()
        {
            dataGridView_Output.AutoGenerateColumns = false;
            dataGridView_Output.SetDoubleBuffered(true);
            tabControl_Main.SelectedTab = tabPage_vertex; //选中图像页
            textBox_IpAddress.Text = IpAddress;
            numeric_Port.Value = Port;

            comboBox_ConnMode.DataSource = dataService.GetConnModes();
            comboBox_ConnMode.SelectedIndexChanged += new System.EventHandler(ComboBox_ConnMode_SelectedIndexChanged);
            comboBox_ConnMode.SelectedValue = (int)ConnectionMode;

            checkBox_UsingLocal.Checked = UsingLocal;
            textBox_IpAddress_Local.Text = IpAddress_Local;
            numeric_Port_Local.Value = Port_Local;
            trackBar_RcsMin.Value = RcsMinimum;
            trackBar_RcsMax.Value = RcsMaximum;
            timer_UIUpdate.Start();

            pictureBox_Dots.MouseWheel += new MouseEventHandler(PictureBox_Dots_MouseWheel);
        }

        public void Finalizing()
        {
            if (finalized)
                return;
            SocketTcpClient.StopConnection();
            SocketTcpServer.Stop();
            ThreadControl(false);
            Infos.ThreadCheck.Abort();
            //thread_writeitems.Abort();
            finalized = true;
            BaseConst.IniHelper.WriteData("Detection", "RcsMinimum", RcsMinimum.ToString());
            BaseConst.IniHelper.WriteData("Detection", "RcsMaximum", RcsMaximum.ToString());
            if (Radar.Id > 0)
                return;

            BaseConst.IniHelper.WriteData("Connection", "IpAddress", IpAddress);
            BaseConst.IniHelper.WriteData("Connection", "Port", Port.ToString());
            BaseConst.IniHelper.WriteData("Connection", "ConnectionMode", ((int)ConnectionMode).ToString());
            BaseConst.IniHelper.WriteData("Connection", "UsingLocal", checkBox_UsingLocal.Checked ? "1" : "0");
            BaseConst.IniHelper.WriteData("Connection", "IpAddressLocal", textBox_IpAddress_Local.Text);
            BaseConst.IniHelper.WriteData("Connection", "PortLocal", numeric_Port_Local.Value.ToString());
        }

        /// <summary>
        /// 线程控制
        /// </summary>
        /// <param name="flag"></param>
        private void ThreadControl(bool flag)
        {
            if (flag)
            {
                if (thread == null)
                    thread = new Thread(new ThreadStart(ProcessReceivedData)) { IsBackground = true };
                thread.Start();
            }
            else
            {
                if (thread != null)
                    thread.Abort();
                thread = null;
            }
        }

        /// <summary>
        /// 开始或结束接收
        /// </summary>
        /// <param name="flag">假如为true，则开始接收，否则结束接收</param>
        public void StartOrEndReceiving(bool flag)
        {
            int result;
            switch (ConnectionMode)
            {
                case ConnectionMode.TCP_CLIENT:
                    result = flag ? Connect() : Disconnect();
                    break;
                case ConnectionMode.UDP:
                    UdpInitOrClose();
                    break;
                case ConnectionMode.TCP_SERVER:
                    TcpServerInitOrClose(flag);
                    break;
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        private int Connect()
        {
            try
            {
                switch (ConnectionMode)
                {
                    case ConnectionMode.TCP_CLIENT:
                        SocketTcpClient.ServerIp = IpAddress;
                        SocketTcpClient.ServerPort = Port;
                        SocketTcpClient.AssignLocalAddress = checkBox_UsingLocal.Checked;
                        SocketTcpClient.LocalIp = IpAddress_Local;
                        SocketTcpClient.LocalPort = Port_Local;
                        SocketTcpClient.StartConnection();
                        break;
                    case ConnectionMode.UDP:
                        if (checkBox_UsingLocal.Checked)
                            UdpClient.Connect(IpAddress, Port, IpAddress_Local, Port_Local);
                        else
                            UdpClient.Connect(IpAddress, Port);
                        break;
                    case ConnectionMode.TCP_SERVER:
                        break;
                }
            }
            catch (Exception)
            {
                Text = Title + " - 连接失败";
                return 0;
            }
            comboBox_ConnMode.Enabled = false;
            button_Send.Enabled = true;
            button_Disconnect.Enabled = true;
            button_Connect.Enabled = false;
            Text = Title + " - 连接成功";
            if (ConnectionMode == ConnectionMode.TCP_CLIENT)
                ThreadControl(true);
            timer_GraphicRefresh.Enabled = true;
            timer_GridRefresh.Enabled = true;
            return 1;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        private int Disconnect()
        {
            try
            {
                switch (ConnectionMode)
                {
                    case ConnectionMode.TCP_CLIENT:
                        SocketTcpClient.StopConnection();
                        break;
                    case ConnectionMode.UDP:
                        UdpClient.Close();
                        break;
                    case ConnectionMode.TCP_SERVER:
                        break;
                }
            }
            catch (Exception)
            {
                Text = Title + " - 断开失败";
                return 0;
            }
            comboBox_ConnMode.Enabled = true;
            button_Send.Enabled = false;
            button_Connect.Enabled = true;
            button_Disconnect.Enabled = false;
            Text = Title + " - 断开成功";
            if (ConnectionMode == ConnectionMode.TCP_CLIENT)
                ThreadControl(false);
            timer_GraphicRefresh.Enabled = false;
            timer_GridRefresh.Enabled = false;
            return 1;
        }

        /// <summary>
        /// UDP初始化或关闭
        /// </summary>
        private void UdpInitOrClose()
        {
            bool init = button_ServerInit.Text.Equals("初始化"); //初始化或结束
            if (init)
            {
                try { UdpClient = checkBox_UsingLocal.Checked ? new DerivedUdpClient(IpAddress_Local, Port_Local) : new DerivedUdpClient(); }
                catch (Exception) { return; }
                UdpClient.DataReceived += new DataReceivedEventHandler(Client_DataReceived);
                UdpClient.ReconnTimerChanged += new ReconnTimerChangedEventHandler(ReconnTimerChanged);
            }
            else
            {
                UdpClient.DataReceived -= new DataReceivedEventHandler(Client_DataReceived);
                UdpClient.ReconnTimerChanged -= new ReconnTimerChangedEventHandler(ReconnTimerChanged);
                UdpClient.Close();
                UdpClient = null;
            }
            button_ServerInit.Text = init ? "结束" : "初始化";
            button_Connect.Enabled = init;
            ThreadControl(init);
            timer_GraphicRefresh.Enabled = init;
            timer_GridRefresh.Enabled = init;
        }

        /// <summary>
        /// TCP监听启动或关闭
        /// </summary>
        /// <param name="init">是否开始监听，为true开始，为false结束</param>
        private void TcpServerInitOrClose(bool init)
        {
            if (init)
            {
                SocketTcpServer.ServerIp = IpAddress_Local;
                SocketTcpServer.ServerPort = Port_Local;
                SocketTcpServer.Start();
            }
            else
                SocketTcpServer.Stop();
            button_ServerInit.Text = init ? "结束" : "初始化";
            ThreadControl(init);
            timer_GraphicRefresh.Enabled = init;
            timer_GridRefresh.Enabled = init;
        }

        /// <summary>
        /// 处理单次输入
        /// </summary>
        /// <param name="input"></param>
        private void ProcessUnit(string input)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Infos.Filter(input);
            if (IsShown)
            {
                textBox_Input.SafeInvoke(() => textBox_Input.Text = input);
                textBox_Info.SafeInvoke(() =>
                {
                    textBox_Info.Text = string.Format(@"{0} 至 {1:yyyy-MM-dd HH:mm:ss.fff} ==>
数据列表长度：{2}
集群或目标数量：{3}
实际数量：{4}", time, DateTime.Now, Infos.ListTriggerCount, Infos.BufferSize, Infos.ActualSize);
                });
            }
        }

        /// <summary>
        /// 循环处理数据
        /// </summary>
        private void ProcessReceivedData()
        {
            while (true)
            {
                try
                {
                    bool connected = false;
                    string name = string.Empty;
                    switch (ConnectionMode)
                    {
                        case ConnectionMode.TCP_CLIENT:
                            connected = SocketTcpClient.IsStart;
                            name = SocketTcpClient.Name;
                            break;
                        case ConnectionMode.UDP:
                            connected = UdpClient.IsConnected_Socket || UdpClient.IsStartListening;
                            name = UdpClient.Name;
                            break;
                        case ConnectionMode.TCP_SERVER:
                            connected = SocketTcpServer.IsStartListening;
                            name = SocketTcpServer.Name;
                            break;
                    }
                    this.SafeInvoke(() => { Text = Title + " - " + (connected ? name : "连接断开"); });

                    #region 被动接收
                    wrapped = received;
                    if (BaseFunc.GetWrappedMessage(ref wrapped))
                    {
                        received = string.Empty;
                        ProcessUnit(wrapped);
                    }
                    #endregion
                }
                catch (Exception) { }
                Thread.Sleep(BaseConst.UsePublicInterval ? BaseConst.RefreshInterval : Radar.RefreshInterval);
            }
        }
        #endregion

        #region 表格与图像
        /// <summary>
        /// 刷新GridView数据源
        /// </summary>
        private void DataGridViewRefresh()
        {
            if (!IsShown)
                return;

            try
            {
                dynamic list_new, binding;
                if (Infos.CurrentSensorMode == SensorMode.Cluster)
                {
                    list_new = list_general.Cast<ClusterGeneral>().ToList();
                    binding = new BindingList<ClusterGeneral>(list_new);
                }
                else
                {
                    list_new = list_general.Cast<ObjectGeneral>().ToList();
                    binding = new BindingList<ObjectGeneral>(list_new);
                }
                //var binding = new BindingList<SensorGeneral>(list_general.ToList());
                dataGridView_Output.DataSource = null;
                dataGridView_Output.DataSource = binding;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 放一张背景图
        /// </summary>
        private void PaintInit()
        {
            bitmap = new Bitmap(BaseConst.OriginalImage, (int)(BaseConst.OriginalImage.Width * S), (int)(BaseConst.OriginalImage.Height * S));
            graphic = Graphics.FromImage(bitmap);
            pictureBox_Dots.SafeInvoke(() => { pictureBox_Dots.Image = bitmap; });
        }

        /// <summary>
        /// 画出所有集群
        /// </summary>
        private void PaintVertexes()
        {
            PaintVertexes(list_general);
        }

        /// <summary>
        /// 画出指定的集群
        /// </summary>
        /// <param name="list">集群列表</param>
        private void PaintVertexes<T>(IEnumerable<T> list) where T : SensorGeneral
        {
            if (list == null || list.Count() == 0)
                return;

            List<T> list_new = list.ToList();
            foreach (var dot in list_new)
                if (dot != null)
                    if (dot is ClusterGeneral)
                        graphic.FillEllipse(new SolidBrush(dot.Color), (float)(448 - dot.DistLat * BaseConst.R) * S, (float)(839 - dot.DistLong * BaseConst.R) * S, BaseConst.T, BaseConst.T); //画实心椭圆
                    else if (dot is ObjectGeneral)
                        graphic.FillRectangle(new SolidBrush(dot.Color), (float)(448 - dot.DistLat * BaseConst.R) * S, (float)(839 - dot.DistLong * BaseConst.R) * S, BaseConst.T * 2, BaseConst.T * 2); //画实心矩形
        }

        /// <summary>
        /// 绘制所有图像
        /// </summary>
        private void PaintAll()
        {
            if (!IsShown)
                return;

            PaintInit();
            PaintVertexes();
        }

        /// <summary>
        /// 放大或缩小
        /// </summary>
        /// <param name="mode">-1 缩小，1 放大</param>
        /// <param name="layers">操作次数（层数）</param>
        private void Zoom(int mode, uint layers)
        {
            double ratio;
            if (mode == -1)
                ratio = BaseConst.ScrollRatio;
            else if (mode == 1)
                ratio = 1 / BaseConst.ScrollRatio;
            else
                ratio = scale_original;
            S *= (float)Math.Pow(ratio, layers);
        }

        /// <summary>
        /// 放大或缩小一次
        /// </summary>
        /// <param name="mode">-1 缩小，1 放大</param>
        private void Zoom(int mode)
        {
            Zoom(mode, 1);
        }
        #endregion

        #region 事件
        /// <summary>
        /// IP地址改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_IpAddress_TextChanged(object sender, EventArgs e)
        {
            IpAddress = textBox_IpAddress.Text;
        }

        /// <summary>
        /// 端口号改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_Port_ValueChanged(object sender, EventArgs e)
        {
            Port = (ushort)numeric_Port.Value;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Connect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Disconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        /// <summary>
        /// 初始化按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ServerInit_Click(object sender, EventArgs e)
        {
            switch (ConnectionMode)
            {
                case ConnectionMode.UDP:
                    UdpInitOrClose();
                    break;
                case ConnectionMode.TCP_SERVER:
                    TcpServerInitOrClose(button_ServerInit.Text.Equals("初始化"));
                    break;
            }
        }

        /// <summary>
        /// TCP客户端连接次数改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="time"></param>
        private void ReconnTimerChanged(object sender, int time) { this.time = time; }

        /// <summary>
        /// 数据接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Client_DataReceived(object sender, DataReceivedEventArgs eventArgs)
        {
            received += " " + eventArgs.ReceivedInfo_HexString;
            Infos.Timer = 0;
        }

        private void SocketTcpClient_OnRecevice(object sender, ReceivedEventArgs e)
        {
            received += " " + e.ReceivedHexString;
            Infos.Timer = 0;
        }

        private void SocketTcpServer_TcpServerReceived(object sender, ReceivedEventArgs e)
        {
            received += " " + e.ReceivedHexString;
            Infos.Timer = 0;
        }

        /// <summary>
        /// 刷新控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_GraphicRefresh_Tick(object sender, EventArgs e)
        {
            //DataGridViewRefresh();
            PaintAll();
        }

        /// <summary>
        /// 刷新控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_GridRefresh_Tick(object sender, EventArgs e)
        {
            DataGridViewRefresh();
            //PaintAll();
        }

        /// <summary>
        /// 放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Zoomin_Click(object sender, EventArgs e)
        {
            //S *= 1 / BaseConst.ScrollRatio;
            Zoom(1);
        }

        /// <summary>
        /// 缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Zoomout_Click(object sender, EventArgs e)
        {
            //S *= BaseConst.ScrollRatio;
            Zoom(-1);
        }

        /// <summary>
        /// 恢复原尺寸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Fullsize_Click(object sender, EventArgs e)
        {
            S = scale_original;
            //Zoom(0);
        }

        int mousex = 0, mousey = 0, hscrollv = 0, vscrollv = 0;
        bool occupied = false;

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_Dots_MouseDown(object sender, MouseEventArgs e)
        {
            //记录鼠标按下时的坐标与滚动轴位置
            mousex = e.X;
            mousey = e.Y;
            hscrollv = panel1.HorizontalScroll.Value;
            vscrollv = panel1.VerticalScroll.Value;
            PictureMoving = true;
        }

        /// <summary>
        /// 鼠标松开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_Dots_MouseUp(object sender, MouseEventArgs e)
        {
            PictureMoving = false;
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_Dots_MouseMove(object sender, MouseEventArgs e)
        {
            if (!PictureMoving || occupied)
                return;

            occupied = true;
            int deltax = mousex - e.X, deltay = mousey - e.Y; //本次移动距离
            //移动后的位置，假如在范围内，更新滚动条位置
            int resultx = hscrollv + deltax;
            int resulty = vscrollv + deltay;
            if (resultx.Between(panel1.HorizontalScroll.Minimum, panel1.HorizontalScroll.Maximum))
                panel1.HorizontalScroll.Value = resultx;
            if (resulty.Between(panel1.VerticalScroll.Minimum, panel1.VerticalScroll.Maximum))
                panel1.VerticalScroll.Value = resulty;
            occupied = false;
        }

        private void TextBox_IpAddress_Local_TextChanged(object sender, EventArgs e)
        {
            IpAddress_Local = textBox_IpAddress_Local.Text;
        }

        private void Numeric_Port_Local_ValueChanged(object sender, EventArgs e)
        {
            Port_Local = (ushort)numeric_Port_Local.Value;
        }

        private void Button_Send_Click(object sender, EventArgs e)
        {
            if (ConnectionMode == ConnectionMode.TCP_CLIENT && SocketTcpClient.IsConnected_Socket)
                SocketTcpClient.SendData(textBox_SendContent.Text);
            else if (ConnectionMode == ConnectionMode.UDP && UdpClient.IsConnected)
                UdpClient.SendString(textBox_SendContent.Text);
            else if (ConnectionMode == ConnectionMode.TCP_SERVER)
                SocketTcpServer.ClientSocketList.ForEach(client => SocketTcpServer.SendData(client, textBox_SendContent.Text));
        }

        private void ComboBox_ConnMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectionMode = (ConnectionMode)int.Parse(comboBox_ConnMode.SelectedValue.ToString());
            button_ServerInit.Enabled = ConnectionMode != ConnectionMode.TCP_CLIENT; //UDP, TCP Server模式下可使用初始化按钮
            button_Connect.Enabled = ConnectionMode != ConnectionMode.TCP_SERVER && !button_ServerInit.Enabled; //非Tcp Server模式连接按钮是否可用取决于初始化按钮
        }

        private void Timer_WriteItems_Tick(object sender, EventArgs e)
        {
            //WriteItemValues();
        }

        private void Timer_UIUpdate_Tick(object sender, EventArgs e)
        {
            label_ReconnCounter.Text = SocketTcpClient.ReConnectedCount.ToString();
            trackBar_RcsMax.Value = RcsMaximum;
            trackBar_RcsMin.Value = RcsMinimum;
        }

        private void CheckBox_UsingLocal_CheckedChanged(object sender, EventArgs e)
        {
            UsingLocal = checkBox_UsingLocal.Checked;
        }

        /// <summary>
        /// 滚轮滑动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_Dots_MouseWheel(object sender, MouseEventArgs e)
        {
            //DELTA为负数代表向下搓，用来放大，反之则代表向上搓，用来缩小
            int times = e.Delta / -120;
            int mode = Math.Sign(times); //放大或缩小
            uint layer = (uint)(times / mode); //操作层数（连续操作若干次）
            Zoom(mode, layer);
        }

        /// <summary>
        /// 双击分页栏掩藏左边栏，假如已隐藏则显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_Main_DoubleClick(object sender, EventArgs e)
        {
            float current = tableLayoutPanel_Main.ColumnStyles[0].Width;
            tableLayoutPanel_Main.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, column_width - current);
        }

        private void FormDisplay_Shown(object sender, EventArgs e)
        {
            IsShown = true;
        }

        private void TrackBar_RcsMin_ValueChanged(object sender, EventArgs e)
        {
            label_RcsMin.Text = (RcsMinimum = trackBar_RcsMin.Value).ToString();
        }

        private void TrackBar_RcsMax_ValueChanged(object sender, EventArgs e)
        {
            label_RcsMax.Text = (RcsMaximum = trackBar_RcsMax.Value).ToString();
        }
        #endregion
    }
}
