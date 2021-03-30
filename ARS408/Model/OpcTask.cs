using ARS408.Core;
using CommonLib.Clients.Tasks;
using CommonLib.Function;
using OpcLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ARS408.Model
{
    public class OpcTask : Task
    {
        private readonly OpcUtilHelper opcHelper = new OpcUtilHelper(1000, true);

        /// <summary>
        /// OPC操作对象
        /// </summary>
        public OpcUtilHelper OpcHelper { get { return opcHelper; } }

        /// <summary>
        /// 装船机信息
        /// </summary>
        public Shiploader Shiploader { get; set; }

        ///// <summary>
        ///// 最新错误信息
        ///// </summary>
        //public string LastErrorMessage { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        public OpcTask(Shiploader shiploader) : base()
        {
            Shiploader = shiploader;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            Interval = int.Parse(BaseConst.IniHelper.ReadData("OPC", "ReadInterval"));
            if (this.Shiploader == null)
                _errorMessage = "装船机信息为空，OPC服务无法初始化";
            else if (string.IsNullOrWhiteSpace(this.Shiploader.OpcServerName))
                _errorMessage = "OPC服务端的名称为空";

            OpcInit();
            SetOpcGroupsDataSource();
        }

        /// <summary>
        /// 循环体内容
        /// </summary>
        public override void LoopContent()
        {
            Interval = int.Parse(BaseConst.IniHelper.ReadData("OPC", "ReadInterval"));
            OpcReadValues();
            OpcWriteValues();
        }

        /// <summary>
        /// OPC初始化
        /// </summary>
        private void OpcInit()
        {
            BaseConst.Log.WriteLogsToFile(string.Format("开始连接IP地址为{0}的OPC SERVER {1}...", Shiploader.OpcServerIp, Shiploader.OpcServerName));
            DataService_Sqlite dataService_Opc = new DataService_Sqlite();
            //opcHelper = new OpcUtilHelper(1000, true);
            string[] servers = opcHelper.ServerEnum(Shiploader.OpcServerIp, out _errorMessage);
            if (!string.IsNullOrWhiteSpace(_errorMessage))
            {
                BaseConst.Log.WriteLogsToFile(string.Format("枚举过程中出现问题：{0}", _errorMessage));
                goto END_OF_OPC;
            }
            if (servers == null || !servers.Contains(Shiploader.OpcServerName))
            {
                BaseConst.Log.WriteLogsToFile(string.Format("无法找到指定OPC SERVER：{0}", Shiploader.OpcServerName));
                goto END_OF_OPC;
            }
            DataTable table = dataService_Opc.GetOpcInfo();
            if (table == null || table.Rows.Count == 0)
            {
                BaseConst.Log.WriteLogsToFile(string.Format("在表中未找到任何OPC记录，将不进行读取或写入", Shiploader.OpcServerName));
                goto END_OF_OPC;
            }
            List<OpcGroupInfo> groups = new List<OpcGroupInfo>();
            List<DataRow> dataRows = table.Rows.Cast<DataRow>().ToList();
            List<OpcItemInfo> items = null;
            int id = 0;
            foreach (var row in dataRows)
            {
                string itemId = row["item_id"].ConvertType<string>();
                if (string.IsNullOrWhiteSpace(itemId))
                    continue;
                int groupId = row["group_id"].ConvertType<int>(), clientHandle = row["record_id"].ConvertType<int>();
                string groupName = row["group_name"].ConvertType<string>(), fieldName = row["field_name"].ConvertType<string>();
                GroupType type = (GroupType)row["group_type"].ConvertType<int>();
                if (groupId != id)
                {
                    id = groupId;
                    groups.Add(new OpcGroupInfo(null, groupName/*, OpcDatasource*/) { GroupType = type, ListItemInfo = new List<OpcItemInfo>() });
                    OpcGroupInfo groupInfo = groups.Last();
                    items = groupInfo.ListItemInfo;
                }
                items.Add(new OpcItemInfo(itemId, clientHandle, fieldName));
            }
            opcHelper.ListGroupInfo = groups;
            opcHelper.ConnectRemoteServer(Shiploader.OpcServerIp, Shiploader.OpcServerName, out _errorMessage);
            BaseConst.Log.WriteLogsToFile(string.Format("OPC连接状态：{0}", opcHelper.OpcConnected));
            if (!string.IsNullOrWhiteSpace(_errorMessage))
                BaseConst.Log.WriteLogsToFile(string.Format("连接过程中出现问题：{0}", _errorMessage));
            END_OF_OPC:;
        }

        /// <summary>
        /// 设置数据源
        /// </summary>
        private void SetOpcGroupsDataSource()
        {
            if (opcHelper != null && opcHelper.ListGroupInfo != null)
                opcHelper.ListGroupInfo.ForEach(group => group.DataSource = BaseConst.OpcDataSource);
        }

        /// <summary>
        /// 读取值
        /// </summary>
        private void OpcReadValues()
        {
            opcHelper.ListGroupInfo.ForEach(group =>
            {
                if (group.GroupType != GroupType.READ)
                    return;

                if (!group.ReadValues(out _errorMessage))
                    BaseConst.Log.WriteLogsToFile(string.Format("读取PLC失败，读取过程中出现问题：{0}", _errorMessage));
            });
        }

        /// <summary>
        /// 写入值
        /// </summary>
        private void OpcWriteValues()
        {
            if (!BaseConst.WriteItemValue)
                return;

            opcHelper.ListGroupInfo.ForEach(group =>
            {
                if (group.GroupType != GroupType.WRITE)
                    return;

                if (!group.WriteValues(out _errorMessage))
                    BaseConst.Log.WriteLogsToFile(string.Format("写入PLC失败，写入过程中出现问题：{0}", _errorMessage));
            });
        }
    }
}
