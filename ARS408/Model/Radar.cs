using ARS408.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Model
{
    /// <summary>
    /// 雷达实体类
    /// </summary>
    [ProtoContract]
    public class Radar
    {
        private double degree_xoy, degree_yoz, degree_xoz, degree_general;
        private double sinphi, cosphi, sintheta, costheta, sinlamda, coslamda, sing, cosg;
        internal double _current;
        internal int _threat_level = 0;
        internal string _threat_level_binary = "00";

        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// 雷达名称
        /// </summary>
        [ProtoMember(2)]
        public string Name { get; set; }

        /// <summary>
        /// 工作状态，1 收到数据，0 未收到数据
        /// </summary>
        [ProtoMember(3)]
        public int Working
        {
            get { return this.State.Working; }
            set { this.State.Working = value; }
        }

        /// <summary>
        /// 雷达状态信息
        /// </summary>
        public RadarState State { get; set; }

        /// <summary>
        /// 当前障碍物距离
        /// </summary>
        [ProtoMember(4)]
        public double CurrentDistance
        {
            get { return this._current; }
            set { this._current = value; }
        }

        /// <summary>
        /// 报警级数
        /// </summary>
        [ProtoMember(5)]
        public int ThreatLevel
        {
            get { return this._threat_level; }
            set { this._threat_level = value; }
        }

        /// <summary>
        /// 报警级数2进制字符串（2位）
        /// </summary>
        [ProtoMember(6)]
        public string ThreatLevelBinary
        {
            get { return this._threat_level_binary; }
            set { this._threat_level_binary = value; }
        }

        #region 通讯与地址
        /// <summary>
        /// IP地址
        /// </summary>
        [ProtoMember(7)]
        public string IpAddress { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        [ProtoMember(8)]
        public ushort Port { get; set; }

        /// <summary>
        /// IP地址+端口
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 连接模式
        /// </summary>
        public ConnectionMode ConnectionMode { get; set; }

        /// <summary>
        /// 是否使用本地IP与端口
        /// </summary>
        public bool UsingLocal { get; set; }

        /// <summary>
        /// 本地IP
        /// </summary>
        public string IpAddressLocal { get; set; }

        /// <summary>
        /// 本地端口
        /// </summary>
        public int PortLocal { get; set; }
        #endregion

        /// <summary>
        /// 所属装船机ID
        /// </summary>
        public int OwnerShiploaderId { get; set; }

        /// <summary>
        /// Topic名称，从装船机获取
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// 所属雷达组
        /// </summary>
        public int OwnerGroupId { get; set; }

        /// <summary>
        /// 雷达组类型，1 臂架，2 溜桶，3 门腿
        /// </summary>
        public RadarGroupType GroupType { get; set; }

        #region 角度与转换
        /// <summary>
        /// XOY平面内旋转角度
        /// </summary>
        public double DegreeXoy
        {
            get { return this.degree_xoy; }
            set
            {
                this.degree_xoy = value;
                this.sinphi = Math.Sin(this.degree_xoy * Math.PI / 180);
                this.cosphi = Math.Cos(this.degree_xoy * Math.PI / 180);
                this.UpdateRatios();
            }
        }

        /// <summary>
        /// YOZ平面内旋转角度
        /// </summary>
        public double DegreeYoz
        {
            get { return this.degree_yoz; }
            set
            {
                this.degree_yoz = value;
                this.sintheta = Math.Sin(this.degree_yoz * Math.PI / 180);
                this.costheta = Math.Cos(this.degree_yoz * Math.PI / 180);
                this.UpdateRatios();
            }
        }

        /// <summary>
        /// XOZ平面内旋转角度
        /// </summary>
        public double DegreeXoz
        {
            get { return this.degree_xoz; }
            set
            {
                this.degree_xoz = value;
                this.sinlamda = Math.Sin(this.degree_xoz * Math.PI / 180);
                this.coslamda = Math.Cos(this.degree_xoz * Math.PI / 180);
                this.UpdateRatios();
            }
        }

        /// <summary>
        /// 垂直于地面的轴的整体旋转角度，面向正南为0，面向海（东）为90，面向北为180，面向陆地（西）为270（或-90）
        /// </summary>
        public double DegreeGeneral
        {
            get { return this.degree_general; }
            set
            {
                this.degree_general = value;
                this.sing = Math.Sin(this.degree_general * Math.PI / 180);
                this.cosg = Math.Cos(this.degree_general * Math.PI / 180);
                this.UpdateRatios();
            }
        }

        /// <summary>
        /// 修改后的X坐标的原XY坐标参数
        /// </summary>
        public CoordinateRatios XmodifiedRatios { get; set; }

        /// <summary>
        /// 修改后的Y坐标的原XY坐标参数
        /// </summary>
        public CoordinateRatios YmodifiedRatios { get; set; }

        /// <summary>
        /// 修改后的Z坐标的原XY坐标参数
        /// </summary>
        public CoordinateRatios ZmodifiedRatios { get; set; }
        #endregion

        #region 测距与检测
        /// <summary>
        /// 方向：123456，海北陆南上下
        /// </summary>
        public Directions Direction { get; set; }

        /// <summary>
        /// 防御模式：1 点，2 线，3 面
        /// </summary>
        public int DefenseMode { get; set; }

        /// <summary>
        /// 距离校正值，以此值校正距防御边界的距离
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// RCS最小值
        /// </summary>
        public int RcsMinimum { get; set; }

        /// <summary>
        /// RCS最大值
        /// </summary>
        public int RcsMaximum { get; set; }

        /// <summary>
        /// 雷达所在高度，一般只有门腿雷达此项有意义
        /// </summary>
        public double RadarHeight { get; set; }
        #endregion

        #region 坐标系坐标限制
        /// <summary>
        /// 是否限制雷达坐标系坐标
        /// </summary>
        public bool RadarCoorsLimited { get; set; }

        /// <summary>
        /// 雷达坐标系X轴最小值
        /// </summary>
        public double RadarxMin { get; set; }

        /// <summary>
        /// 雷达坐标系x轴最大值
        /// </summary>
        public double RadarxMax { get; set; }

        /// <summary>
        /// 雷达坐标系y轴最小值
        /// </summary>
        public double RadaryMin { get; set; }

        /// <summary>
        /// 雷达坐标系y轴最大值
        /// </summary>
        public double RadaryMax { get; set; }

        /// <summary>
        /// 是否限制单机坐标系坐标
        /// </summary>
        public bool ClaimerCoorsLimited { get; set; }

        /// <summary>
        /// 单机坐标系X轴最小值
        /// </summary>
        public double ClaimerxMin { get; set; }

        /// <summary>
        /// 单机坐标系X轴最大值
        /// </summary>
        public double ClaimerxMax { get; set; }

        /// <summary>
        /// 单机坐标系y轴最小值
        /// </summary>
        public double ClaimeryMin { get; set; }

        /// <summary>
        /// 单机坐标系y轴最大值
        /// </summary>
        public double ClaimeryMax { get; set; }

        /// <summary>
        /// 单机坐标系z轴最小值
        /// </summary>
        public double ClaimerzMin { get; set; }

        /// <summary>
        /// 单机坐标系z轴最小值
        /// </summary>
        public double ClaimerzMax { get; set; }
        #endregion

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 雷达状态标签
        /// </summary>
        public string ItemNameRadarState { get; set; }

        /// <summary>
        /// 雷达碰撞状态标签1
        /// </summary>
        public string ItemNameCollisionState { get; set; }

        /// <summary>
        /// 雷达碰撞状态标签2
        /// </summary>
        public string ItemNameCollisionState2 { get; set; }

        ///// <summary>
        ///// 存在概率最低值
        ///// </summary>
        //public double ProbOfExistMinimum { get; set; }
        #endregion

        #region 构造器
        /// <summary>
        /// 默认构造器，从公共变量获取属性值
        /// </summary>
        public Radar()
        {
            this.RadarHeight = 0;
            this.State = new RadarState();
            this.Id = -1;
            this.Name = "ARS408-21";
            this.GroupType = RadarGroupType.None;
            this.IpAddress = BaseConst.IpAddress;
            this.Port = BaseConst.Port;
            this.ConnectionMode = BaseConst.ConnectionMode;
            this.UsingLocal = BaseConst.UsingLocal;
            this.PortLocal = BaseConst.Port_Local;
            this.RcsMinimum = BaseConst.RcsMinimum;
            this.RcsMinimum = BaseConst.RcsMaximum;
        }

        /// <summary>
        /// 构造器，从公共变量获取属性值，再用给定的DataRow对象覆盖各属性的值
        /// </summary>
        /// <param name="row"></param>
        public Radar(DataRow row) : this()
        {
            if (row == null)
                return;

            this.Id = int.Parse(row["radar_id"].ToString());
            this.Name = row["radar_name"].ToString();
            this.IpAddress = row["ip_address"].ToString();
            this.Port = ushort.Parse(row["port"].ToString());
            this.ConnectionMode = (ConnectionMode)int.Parse(row["conn_mode_id"].ToString());
            this.UsingLocal = row["using_local"].ToString().Equals("1");
            this.IpAddressLocal = row["ip_address_local"].ToString();
            this.PortLocal = int.Parse(row["port_local"].ToString());
            this.OwnerShiploaderId = int.Parse(row["shiploader_id"].ToString());
            this.TopicName = row["topic_name"].ToString();
            this.OwnerGroupId = int.Parse(row["owner_group_id"].ToString());
            this.GroupType = (RadarGroupType)int.Parse(row["group_type"].ToString());
            this.DegreeYoz = double.Parse(row["degree_yoz"].ToString());
            this.DegreeXoy = double.Parse(row["degree_xoy"].ToString());
            this.DegreeXoz = double.Parse(row["degree_xoz"].ToString());
            this.DegreeGeneral = double.Parse(row["degree_general"].ToString());
            this.Direction = (Directions)int.Parse(row["direction_id"].ToString());
            this.DefenseMode = int.Parse(row["defense_mode_id"].ToString());
            this.Offset = double.Parse(row["offset"].ToString());
            this.Remark = row["remark"].ToString();
            this.ItemNameRadarState = row["item_name_radar_state"].ToString();
            this.ItemNameCollisionState = row["item_name_collision_state"].ToString();
            this.ItemNameCollisionState2 = row["item_name_collision_state_2"].ToString();
            this.RcsMinimum = int.Parse(row["rcs_min"].ToString());
            this.RcsMaximum = int.Parse(row["rcs_max"].ToString());
            this.RadarHeight = double.Parse(row["radar_height"].ToString());
            this.RadarCoorsLimited = row["radar_coors_limited"].ToString().Equals("1");
            this.RadarxMin = double.Parse(row["radar_x_min"].ToString());
            this.RadarxMax = double.Parse(row["radar_x_max"].ToString());
            this.RadaryMin = double.Parse(row["radar_y_min"].ToString());
            this.RadaryMax = double.Parse(row["radar_y_max"].ToString());
            this.ClaimerCoorsLimited = row["claimer_coors_limited"].ToString().Equals("1");
            this.ClaimerxMin = double.Parse(row["claimer_x_min"].ToString());
            this.ClaimerxMax = double.Parse(row["claimer_x_max"].ToString());
            this.ClaimeryMin = double.Parse(row["claimer_y_min"].ToString());
            this.ClaimeryMax = double.Parse(row["claimer_y_max"].ToString());
            this.ClaimerzMin = double.Parse(row["claimer_z_min"].ToString());
            this.ClaimerzMax = double.Parse(row["claimer_z_max"].ToString());
        }
        #endregion

        /// <summary>
        /// 更新修改后XYZ坐标的系数
        /// </summary>
        public void UpdateRatios()
        {
            this.XmodifiedRatios = new CoordinateRatios() { Xratio = cosphi * coslamda * cosg - sinphi * sing, Yratio = 0 - sintheta * sinlamda * cosg - costheta * sinphi * coslamda * cosg - costheta * cosphi * sing };
            this.YmodifiedRatios = new CoordinateRatios() { Xratio = cosphi * coslamda * sing + sinphi * cosg, Yratio = costheta * cosphi * cosg - costheta * sinphi * coslamda * sing - sintheta * sinlamda * sing };
            this.ZmodifiedRatios = new CoordinateRatios() { Xratio = cosphi * sinlamda, Yratio = sintheta * coslamda - costheta * sinphi * sinlamda };

        }
    }

    /// <summary>
    /// 修改后的坐标中原XY坐标的系数
    /// </summary>
    public class CoordinateRatios
    {
        /// <summary>
        /// 原X坐标的系数
        /// </summary>
        public double Xratio;

        /// <summary>
        /// 原Y坐标的系数
        /// </summary>
        public double Yratio;
    }
}
