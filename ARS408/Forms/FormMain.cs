using ARS408.Core;
using CommonLib.Clients;
using CommonLib.Function;
using CommonLib.UIControlUtil;
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
            Init_Watchdog();
            BaseFunc.UpdateRadarList();

            toolStripMenu_AutoConnect.Checked = BaseConst.AutoConnect;
            toolStripMenu_AutoMonitor.Checked = BaseConst.IniHelper.ReadData("Main", "AutoMonitor").Equals("1");
            toolStrip_ShowDeserted.Checked = BaseConst.ShowDesertedPoints;
            toolStrip_WriteItemValues.Checked = BaseConst.IniHelper.ReadData("OPC", "WriteItemValue").Equals("1");
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
            if (toolStripMenu_AutoMonitor.Checked)
                ShowMonitors();
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tabControl_Main.DisposeAllTabPages();
            tcpServer_Watchdog.Stop();
            timer1.Stop();

            BaseConst.DictForms.Values.Cast<FormDisplay>().ToList().ForEach(form =>
            {
                form.Finalizing();
                form.Close();
                form.Dispose();
            });
        }

        #region 功能
        /// <summary>
        /// Tcp服务端初始化，发送心跳数据
        /// </summary>
        private void Init_Watchdog()
        {
            tcpServer_Watchdog.ServerIp = BaseConst.IniHelper.ReadData("Watchdog", "MainServerIp");
            tcpServer_Watchdog.ServerPort = int.Parse(BaseConst.IniHelper.ReadData("Watchdog", "MainServerPort"));
            tcpServer_Watchdog.IsHeartCheck = BaseConst.IniHelper.ReadData("Watchdog", "SendHeartBeat").Equals("1");
            tcpServer_Watchdog.HeartBeatPacket = BaseConst.IniHelper.ReadData("Watchdog", "HeartBeatString");
            tcpServer_Watchdog.CheckTime = int.Parse(BaseConst.IniHelper.ReadData("Watchdog", "HeartBeatInterval"));
            tcpServer_Watchdog.Start();
            BaseConst.Log.WriteLogsToFile("看门狗兼下发服务启动");
        }

        /// <summary>
        /// 监视页面对象初始化
        /// </summary>
        private void InitializeMonitors()
        {
            BaseConst.Log.WriteLogsToFile("开始刷新监视页面列表...");
            DataTable table = DataService_Shiploader.GetAllShiploadersOrderbyId();
            if (table == null || table.Rows.Count == 0)
                return;

            list_monitors = table.Rows.Cast<DataRow>().Select(row => new FormMonitor(int.Parse(row["shiploader_id"].ToString()))).ToList();
            //table.Rows.Cast<DataRow>().ToList().ForEach(row => ShowForm(first_monitor = new FormMonitor(int.Parse(row["shiploader_id"].ToString())), DockStyle.Fill));
            BaseConst.Log.WriteLogsToFile("监视页面列表刷新完成");
        }

        /// <summary>
        /// 打开监视页面
        /// </summary>
        private void ShowMonitors()
        {
            InitializeMonitors();
            list_monitors.ForEach(monitor => tabControl_Main.ShowForm(monitor, DockStyle.Fill));
            BaseConst.Log.WriteLogsToFile("已显示所有监视页面");
        }

        /// <summary>
        /// 关闭所有监视页面
        /// </summary>
        private void CloseMonitors()
        {
            foreach (TabPage page in tabControl_Main.TabPages)
                if (page.Controls[0] is FormMonitor)
                    tabControl_Main.DisposeTabPage(page);
            BaseConst.Log.WriteLogsToFile("已关闭所有监视页面");
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
            Close();
        }

        /// <summary>
        /// 装船机字典按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_Shiploaders_Click(object sender, EventArgs e)
        {
            tabControl_Main.ShowForm(new FormShiploader());
        }

        /// <summary>
        /// 雷达组字典按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_RadarGroup_Click(object sender, EventArgs e)
        {
            tabControl_Main.ShowForm(new FormRadarGroup());
        }

        /// <summary>
        /// 雷达字典按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_Radar_Click(object sender, EventArgs e)
        {
            tabControl_Main.ShowForm(new FormRadar(), DockStyle.Fill);
        }

        /// <summary>
        /// Tab页双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_Main_DoubleClick(object sender, EventArgs e)
        {
            tabControl_Main.DisposeSelectedTabPage();
        }

        /// <summary>
        /// 监视页面双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenu_Monitor_Click(object sender, EventArgs e)
        {
            CloseMonitors();
            ShowMonitors();
        }

        /// <summary>
        /// 威胁级数按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStrip_ThreatLevels_Click(object sender, EventArgs e)
        {
            tabControl_Main.ShowForm(new FormThreatLevels());
        }

        /// <summary>
        /// OPC配置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStrip_OpcConfig_Click(object sender, EventArgs e)
        {
            //tabControl_Main.ShowForm(new FormOpcConfig());
            FormOpcConfig form = new FormOpcConfig { StartPosition = FormStartPosition.CenterScreen };
            form.ShowDialog();
        }

        /// <summary>
        /// 坐标限制配置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStrip_CoorsLimitConfig_Click(object sender, EventArgs e)
        {
            FormCoorsLimitationConfig form = new FormCoorsLimitationConfig { StartPosition = FormStartPosition.CenterScreen };
            form.ShowDialog();
        }

        /// <summary>
        /// TcpServer出现错误事件
        /// </summary>
        /// <param name="msg">错误信息</param>
        private void TcpServer_Watchdog_OnErrorMsg(string message)
        {
            try { OnTcpInfoCallBack(message, 1); }
            catch (Exception ex) { FileClient.WriteExceptionInfo(ex, "处理TCP服务端错误信息时出现错误", true); }
        }

        private void TcpServer_Watchdog_TcpServerRecevice(object sender, ReceivedEventArgs e)
        {
            try { MessageReceived(e); }
            catch (Exception ex) { FileClient.WriteExceptionInfo(ex, "处理TCP服务端接收的信息时出现错误", true); }
        }

        private void TcpServer_Watchdog_OnStateInfo(object sender, StateInfoEventArgs e)
        {
            try { OnTcpInfoCallBack(e.StateInfo, 2); }
            catch (Exception ex) { FileClient.WriteExceptionInfo(ex, "TCP服务端状态信息出错", true); }
        }

        public void OnTcpInfoCallBack(object obj, int index)
        {
            string message = Functions.AddTimeToMessage(obj);
            switch (index)
            {
                case 1:
                    tcp_info_error = message;
                    break;
                case 2:
                    tcp_info_state = message;
                    break;
            }
            file_client_dog.WriteLineToFile(message);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public void MessageReceived(ReceivedEventArgs e)
        {
            string info, ip = ((IPEndPoint)e.Socket.RemoteEndPoint).Address.ToString();
            int port = ((IPEndPoint)e.Socket.RemoteEndPoint).Port;
            ClientModel clientModel = tcpServer_Watchdog.ResolveSocket(ip, port);
            if (clientModel == null)
            {
                info = Functions.AddTimeToMessage("客户端为空，怎么就收到消息了呢？不管咋说，消息是这个：" + e.ReceivedString);
                tcp_info_receive = info;
                return;
            }
            //if (clientModel.ClientType == ClientType.None)
            //{
            //    ClientType clientType = ClientModel.AnalyzeClientType(e.ReceivedString);
            //    clientModel.ClientType = clientType;
            //}
            info = Functions.AddTimeToMessage(string.Format("{0}:{1} -> 从客户端 {2}:{3} 接收到数据：{4}", tcpServer_Watchdog.ServerIp, tcpServer_Watchdog.ServerPort, ip, port, e.ReceivedString));
            tcp_info_receive = info;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            BaseFunc.UpdateRadarInfo();
            tcpServer_Watchdog.SendData(ProtobufNetWrapper.SerializeToBytes(BaseConst.RadarInfo, (int)ProtoInfoType.RADAR));
            label_Error.Text = tcp_info_error;
            label_State.Text = tcp_info_state;
            label_Receive.Text = tcp_info_receive;
        }

        private void ToolStripMenu_AutoMonitor_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Main", "AutoMonitor", toolStripMenu_AutoMonitor.Checked ? "1" : "0");
        }

        private void ToolStripMenu_AutoConnect_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Main", "AutoConnect", toolStripMenu_AutoConnect.Checked ? "1" : "0");
        }

        private void ToolStrip_RadarBehavior_Click(object sender, EventArgs e)
        {
            new FormRadarBehavior().Show();
        }

        private void ToolStrip_Preferences_Click(object sender, EventArgs e)
        {
            new FormPreferences().Show();
        }

        private void ToolStrip_ShowDeserted_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Main", "ShowDesertedPoints", toolStrip_ShowDeserted.Checked ? "1" : "0");
        }

        private void ToolStrip_WriteItemValues_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("OPC", "WriteItemValue", toolStrip_WriteItemValues.Checked ? "1" : "0");
        }
        #endregion
    }
}
