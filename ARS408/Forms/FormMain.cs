using ARS408.Core;
using CarServer;
using CommonLib.Clients;
using CommonLib.Function;
using ProtobufNetLibrary;
using SerializationFactory;
using SocketHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARS408.Forms
{
    public partial class FormMain : Form
    {
        #region 私有变量
        //private FormMonitor first_monitor;
        private List<FormMonitor> list_monitors = new List<FormMonitor>();
        private readonly DataService_Shiploader DataService_Shiploader = new DataService_Shiploader();
        private string tcp_info_error = string.Empty;
        private string tcp_info_state = string.Empty;
        private string tcp_info_receive = string.Empty;
        private readonly FileClient file_client_dog = new FileClient("logs\\tcp_server_watchdog", "tcp_server_log.txt");
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            this.Init_Watchdog();
            BaseFunc.UpdateRadarList();
            //this.InitializeMonitors();

            this.toolStripMenu_AutoConnect.Checked = BaseConst.AutoConnect;
            this.toolStripMenu_AutoMonitor.Checked = BaseConst.IniHelper.ReadData("Main", "AutoMonitor").Equals("1");
            this.toolStrip_ShowDeserted.Checked = BaseConst.ShowDesertedPoints;
            ////假如勾选自动开始监视，打开监视页面
            //if (this.toolStripMenu_AutoMonitor.Checked = BaseConst.IniHelper.ReadData("Main", "AutoMonitor").Equals("1"))
            //    this.ShowMonitors();
            BaseConst.Log.WriteLogsToFile("程序主体初始化完成");
        }

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            //假如勾选自动开始监视，打开监视页面
            if (this.toolStripMenu_AutoMonitor.Checked/* = BaseConst.IniHelper.ReadData("Main", "AutoMonitor").Equals("1")*/)
                this.ShowMonitors();
        }

        //private void MonitorStartup()
        //{
        //    //假如勾选自动开始监视，打开监视页面
        //    if (this.toolStripMenu_AutoMonitor.Checked = BaseConst.IniHelper.ReadData("Main", "AutoMonitor").Equals("1"))
        //        this.StartMonitor();
        //}

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (TabPage page in this.tabControl_Main.TabPages)
                this.DisposeTabPage(page);
            this.tcpServer_Watchdog.Stop();
            this.timer1.Stop();

            BaseConst.DictForms.Values.Cast<FormDisplay>().ToList().ForEach(form =>
            {
                form.Finalizing();
                form.Close();
                form.Dispose();
            });
            //this.list_monitors.ForEach(form =>
            //{
            //    form.Close();
            //    form.Dispose();
            //});
        }

        #region 功能
        /// <summary>
        /// Tcp服务端初始化，发送心跳数据
        /// </summary>
        private void Init_Watchdog()
        {
            this.tcpServer_Watchdog.ServerIp = BaseConst.IniHelper.ReadData("Watchdog", "MainServerIp");
            this.tcpServer_Watchdog.ServerPort = int.Parse(BaseConst.IniHelper.ReadData("Watchdog", "MainServerPort"));
            this.tcpServer_Watchdog.IsHeartCheck = BaseConst.IniHelper.ReadData("Watchdog", "SendHeartBeat").Equals("1");
            this.tcpServer_Watchdog.HeartBeatPacket = BaseConst.IniHelper.ReadData("Watchdog", "HeartBeatString");
            this.tcpServer_Watchdog.CheckTime = int.Parse(BaseConst.IniHelper.ReadData("Watchdog", "HeartBeatInterval"));
            this.tcpServer_Watchdog.Start();
            BaseConst.Log.WriteLogsToFile("看门狗兼下发服务启动");
        }

        /// <summary>
        /// 监视页面对象初始化
        /// </summary>
        private void InitializeMonitors()
        {
            BaseConst.Log.WriteLogsToFile("开始刷新监视页面列表...");
            DataTable table = this.DataService_Shiploader.GetAllShiploadersOrderbyId();
            if (table == null || table.Rows.Count == 0)
                return;

            this.list_monitors = table.Rows.Cast<DataRow>().Select(row => new FormMonitor(int.Parse(row["shiploader_id"].ToString()))).ToList();
            //table.Rows.Cast<DataRow>().ToList().ForEach(row => this.ShowForm(this.first_monitor = new FormMonitor(int.Parse(row["shiploader_id"].ToString())), DockStyle.Fill));
            BaseConst.Log.WriteLogsToFile("监视页面列表刷新完成");
        }

        /// <summary>
        /// 打开监视页面
        /// </summary>
        private void ShowMonitors()
        {
            this.InitializeMonitors();
            this.list_monitors.ForEach(monitor => this.ShowForm(monitor, DockStyle.Fill));
            BaseConst.Log.WriteLogsToFile("已显示所有监视页面");
        }

        /// <summary>
        /// 关闭所有监视页面
        /// </summary>
        private void CloseMonitors()
        {
            foreach (TabPage page in this.tabControl_Main.TabPages)
                if (page.Controls[0] is FormMonitor)
                    this.DisposeTabPage(page);
            BaseConst.Log.WriteLogsToFile("已关闭所有监视页面");
        }

        /// <summary>
        /// 在TabPage页中加载窗体对象，默认不停靠TabPage
        /// </summary>
        /// <param name="form">需在TabPage页中加载或显示的窗体对象</param>
        private void ShowForm(Form form)
        {
            this.ShowForm(form, null);
        }

        /// <summary>
        /// 在TabPage页中加载窗体对象
        /// </summary>
        /// <param name="form">需在TabPage页中加载或显示的窗体对象</param>
        /// <param name="dock">停靠方式，假如为null则不停靠</param>
        private void ShowForm(Form form, DockStyle? dock)
        {
            if (form == null)
            {
                MessageBox.Show("无法显示空窗体", "提示消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //假如Tab页已存在，选中该页面
            foreach (TabPage tabPage in this.tabControl_Main.TabPages)
                if (tabPage.Name == form.Name)
                {
                    this.tabControl_Main.SelectedTab = tabPage;
                    return;
                }

            //在TabControl中显示包含该页面的TabPage
            form.TopLevel = false; //不置顶
            if (dock != null)
                form.Dock = dock.Value; //控件停靠方式
            form.FormBorderStyle = FormBorderStyle.None; //页面无边框
            TabPage page = new TabPage();
            page.Controls.Add(form);
            page.Text = form.Text;
            page.Name = form.Name;
            page.AutoScroll = true;
            this.Invoke(new MethodInvoker(delegate
            {
                this.tabControl_Main.TabPages.Add(page);
                this.tabControl_Main.SelectedTab = page;
                form.Show();
            }));
        }

        /// <summary>
        /// 释放TabPage的资源
        /// </summary>
        /// <param name="page"></param>
        private void DisposeTabPage(TabPage page)
        {
            if (page == null)
                return;
            if (page.Controls.Count > 0)
            {
                //Control control = page.Controls[0];
                //if (control is FormMonitor)
                //    page.Controls.Remove(control);
                //else
                //{
                //    Form form = (Form)control;
                //    if (form != null)
                //    {
                //        form.Close();
                //        form.Dispose();
                //    }
                //}
                Form form = (Form)page.Controls[0];
                if (form != null)
                {
                    form.Close();
                    form.Dispose();
                }
            }

            page.Dispose();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 退出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 装船机字典按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_Shiploaders_Click(object sender, EventArgs e)
        {
            this.ShowForm(new FormShiploader());
        }

        /// <summary>
        /// 雷达组字典按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_RadarGroup_Click(object sender, EventArgs e)
        {
            this.ShowForm(new FormRadarGroup());
        }

        /// <summary>
        /// 雷达字典按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_Radar_Click(object sender, EventArgs e)
        {
            this.ShowForm(new FormRadar(), DockStyle.Fill);
        }

        /// <summary>
        /// Tab页双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_Main_DoubleClick(object sender, EventArgs e)
        {
            this.DisposeTabPage(this.tabControl_Main.SelectedTab);
        }

        /// <summary>
        /// 监视页面双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_Monitor_Click(object sender, EventArgs e)
        {
            this.CloseMonitors();
            //this.InitializeMonitors();
            this.ShowMonitors();
        }

        /// <summary>
        /// 威胁级数按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStrip_ThreatLevels_Click(object sender, EventArgs e)
        {
            this.ShowForm(new FormThreatLevels());
        }

        /// <summary>
        /// OPC配置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStrip_OpcConfig_Click(object sender, EventArgs e)
        {
            FormOpcConfig form = new FormOpcConfig();
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowDialog();
        }

        /// <summary>
        /// 坐标限制配置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStrip_CoorsLimitConfig_Click(object sender, EventArgs e)
        {
            FormCoorsLimitationConfig form = new FormCoorsLimitationConfig();
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowDialog();
        }

        /// <summary>
        /// TcpServer出现错误事件
        /// </summary>
        /// <param name="msg">错误信息</param>
        private void TcpServer_Watchdog_OnErrorMsg(string message)
        {
            try { this.OnTcpInfoCallBack(message, 1); }
            catch (Exception ex) { FileClient.WriteExceptionInfo(ex, "处理TCP服务端错误信息时出现错误", true); }
        }

        private void TcpServer_Watchdog_TcpServerRecevice(Socket socket, ReceivedEventArgs e)
        {
            try { this.MessageReceived(socket, e); }
            catch (Exception ex) { FileClient.WriteExceptionInfo(ex, "处理TCP服务端接收的信息时出现错误", true); }
        }

        private void TcpServer_Watchdog_OnStateInfo(string message, SocketHelper.SocketState state)
        {
            try { this.OnTcpInfoCallBack(message, 2); }
            catch (Exception ex) { FileClient.WriteExceptionInfo(ex, "TCP服务端状态信息出错", true); }
        }

        public void OnTcpInfoCallBack(object obj, int index)
        {
            string message = Functions.AddTimeToMessage(obj);
            switch (index)
            {
                case 1:
                    this.tcp_info_error = message;
                    break;
                case 2:
                    this.tcp_info_state = message;
                    break;
            }
            this.file_client_dog.WriteLineToFile(message);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public void MessageReceived(Socket socket, ReceivedEventArgs e)
        {
            string info, ip = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            int port = ((IPEndPoint)socket.RemoteEndPoint).Port;
            ClientModel clientModel = this.tcpServer_Watchdog.ResolveSocket(ip, port);
            if (clientModel == null)
            {
                info = Functions.AddTimeToMessage("客户端为空，怎么就收到消息了呢？不管咋说，消息是这个：" + e.ReceivedString);
                this.tcp_info_receive = info;
                return;
            }
            if (clientModel.ClientType == ClientType.None)
            {
                ClientType clientType = ClientModel.AnalyzeClientType(e.ReceivedString);
                clientModel.ClientType = clientType;
            }
            info = Functions.AddTimeToMessage(string.Format("{0}:{1} -> 从类型为{2}的客户端 {3}:{4} 接收到数据：{5}", this.tcpServer_Watchdog.ServerIp, this.tcpServer_Watchdog.ServerPort, clientModel.ClientType.ToString(), ip, port, e.ReceivedString));
            this.tcp_info_receive = info;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            BaseFunc.UpdateRadarInfo();
            //BaseConst.RadarInfo.DistWheelLeft = -18;
            //BaseConst.RadarInfo.DistWheelRight = 17;
            //string s = ProtobufNetWrapper.SerializeToString(BaseConst.RadarInfo, RadarProtoInfo.RADAR_PROTO_PREFIX);
            //byte[] buffer = ProtobufNetWrapper.SerializeToBytes(BaseConst.RadarInfo);
            //RadarProtoInfo info = ProtobufNetWrapper.DeserializeFromBytes<RadarProtoInfo>(buffer);
            //info = ProtobufNetWrapper.DeserializeFromString<RadarProtoInfo>(s, RadarProtoInfo.RADAR_PROTO_PREFIX);
            //byte[] array = ProtobufNetWrapper.SerializeToBytes(BaseConst.RadarInfo, (int)ProtoInfoType.RADAR);
            //RadarProtoInfo info = ProtobufNetWrapper.DeserializeFromBytes<RadarProtoInfo>(array);
            this.tcpServer_Watchdog.SendData(ProtobufNetWrapper.SerializeToBytes(BaseConst.RadarInfo, (int)ProtoInfoType.RADAR));
            this.label_Error.Text = this.tcp_info_error;
            this.label_State.Text = this.tcp_info_state;
            this.label_Receive.Text = this.tcp_info_receive;
        }

        private void ToolStripMenu_AutoMonitor_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Main", "AutoMonitor", this.toolStripMenu_AutoMonitor.Checked ? "1" : "0");
        }

        private void ToolStripMenu_AutoConnect_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Main", "AutoConnect", this.toolStripMenu_AutoConnect.Checked ? "1" : "0");
        }

        private void ToolStrip_Preferences_Click(object sender, EventArgs e)
        {
            new FormPreferences().Show();
        }
        #endregion

        private void ToolStrip_ShowDeserted_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Main", "ShowDesertedPoints", this.toolStrip_ShowDeserted.Checked ? "1" : "0");
        }
    }
}
