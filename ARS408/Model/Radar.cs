﻿using ARS408.Core;
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
        internal double _current, _curve_slope, _surface_angle, _radius_average; //当前距离，当前斜率
        internal bool _out_of_stack;
        internal string _threat_level_binary = "00";

        /// <summary>
        /// 处理标志，为true表示未处理，否则已处理
        /// </summary>
        public bool ProcFlag { get; set; }

        #region 属性
        /// <summary>
        /// 帧消息处理类
        /// </summary>
        public DataFrameMessages Infos { get; private set; }

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
            get { return State.Working; }
            set { State.Working = value; }
        }

        /// <summary>
        /// 雷达状态信息
        /// </summary>
        public RadarState State { get; set; }

        /// <summary>
        /// 刷新时间间隔
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// 当前障碍物距离
        /// </summary>
        [ProtoMember(4)]
        public double CurrentDistance
        {
            get { return _current; }
            set { _current = value; }
        }

        /// <summary>
        /// 当前一次拟合斜率
        /// </summary>
        public double CurveSlope
        {
            get { return _curve_slope; }
            set { _curve_slope = value; }
        }

        /// <summary>
        /// 平面拟合后与水平面的夹角
        /// </summary>
        public double SurfaceAngle
        {
            get { return _surface_angle; }
            set { _surface_angle = value; }
        }

        /// <summary>
        /// 雷达特定范围内点的平均距离（平均极坐标半径）
        /// </summary>
        public double RadiusAverage
        {
            get { return _radius_average; }
            set { _radius_average = value; }
        }

        /// <summary>
        /// 是否出垛边判断（由matlab模型决定）
        /// </summary>
        public bool OutOfStack
        {
            get { return _out_of_stack; }
            set { _out_of_stack = value; }
        }

        internal int _threat_level = 0;
        /// <summary>
        /// 报警级数
        /// </summary>
        [ProtoMember(5)]
        public int ThreatLevel
        {
            get { return _threat_level; }
            set { _threat_level = value; }
        }

        /// <summary>
        /// 报警级数2进制字符串（2位）
        /// </summary>
        [ProtoMember(6)]
        public string ThreatLevelBinary
        {
            get { return _threat_level_binary; }
            set { _threat_level_binary = value; }
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
            get { return degree_xoy; }
            set
            {
                degree_xoy = value;
                sinphi = Math.Sin(degree_xoy * Math.PI / 180);
                cosphi = Math.Cos(degree_xoy * Math.PI / 180);
                UpdateRatios();
            }
        }

        /// <summary>
        /// YOZ平面内旋转角度
        /// </summary>
        public double DegreeYoz
        {
            get { return degree_yoz; }
            set
            {
                degree_yoz = value;
                sintheta = Math.Sin(degree_yoz * Math.PI / 180);
                costheta = Math.Cos(degree_yoz * Math.PI / 180);
                UpdateRatios();
            }
        }

        /// <summary>
        /// XOZ平面内旋转角度
        /// </summary>
        public double DegreeXoz
        {
            get { return degree_xoz; }
            set
            {
                degree_xoz = value;
                sinlamda = Math.Sin(degree_xoz * Math.PI / 180);
                coslamda = Math.Cos(degree_xoz * Math.PI / 180);
                UpdateRatios();
            }
        }

        /// <summary>
        /// 垂直于地面的轴的整体旋转角度，面向正南为0，面向海（东）为90，面向北为180，面向陆地（西）为270（或-90）
        /// </summary>
        public double DegreeGeneral
        {
            get { return degree_general; }
            set
            {
                degree_general = value;
                sing = Math.Sin(degree_general * Math.PI / 180);
                cosg = Math.Cos(degree_general * Math.PI / 180);
                UpdateRatios();
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
        public double Offset { get; private set; }

        /// <summary>
        /// X坐标校正值
        /// </summary>
        public double XOffset { get; private set; }

        /// <summary>
        /// Y坐标校正值
        /// </summary>
        public double YOffset { get; private set; }

        /// <summary>
        /// Z坐标校正值
        /// </summary>
        public double ZOffset { get; private set; }

        private int _rcsMinimum;
        /// <summary>
        /// RCS最小值
        /// </summary>
        public int RcsMinimum
        {
            get { return _rcsMinimum; }
            set
            {
                _rcsMinimum = value;
                Infos.RcsMinimum = _rcsMinimum;
            }
        }

        private int _rcsMaximum;
        /// <summary>
        /// RCS最大值
        /// </summary>
        public int RcsMaximum
        {
            get { return _rcsMaximum; }
            set
            {
                _rcsMaximum = value;
                Infos.RcsMinimum = _rcsMinimum;
            }
        }

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

        #region 角度限制
        /// <summary>
        /// 是否限制角度
        /// </summary>
        public bool AngleLimited { get; set; }

        /// <summary>
        /// 角度最小值
        /// </summary>
        public double AngleMin { get; set; }

        /// <summary>
        /// 角度最大值
        /// </summary>
        public double AngleMax { get; set; }
        #endregion

        #region 检测特性
        /// <summary>
        /// 是否应用集群或目标过滤器
        /// </summary>
        public bool ApplyFilter { get; set; }

        /// <summary>
        /// 是否应用迭代
        /// </summary>
        public bool ApplyIteration { get; set; }

        /// <summary>
        /// push finalization的最大次数，雷达帧的累积周期
        /// </summary>
        public int PushfMaxCount { get; set; }

        /// <summary>
        /// 是否使用公共过滤条件
        /// </summary>
        public bool UsePublicFilters { get; set; }

        /// <summary>
        /// 错误警报概率过滤器
        /// </summary>
        public List<FalseAlarmProbability> FalseAlarmFilter { get; set; }

        /// <summary>
        /// 径向速度不确定性过滤器
        /// </summary>
        public List<AmbigState> AmbigStateFilter { get; set; }

        /// <summary>
        /// 有效性(有效/高概率多目标)
        /// </summary>
        public List<InvalidState> InvalidStateFilter { get; set; }

        /// <summary>
        /// 测量状态过滤器
        /// </summary>
        public List<MeasState> MeasStateFilter { get; set; }

        /// <summary>
        /// 存在概率过滤器
        /// </summary>
        public List<ProbOfExist> ProbOfExistFilter { get; set; }

        private string false_alarm_string;
        /// <summary>
        /// 错误警报概率过滤器
        /// </summary>
        public string FalseAlarmFilterString
        {
            get { return false_alarm_string; }
            set
            {
                false_alarm_string = value == null ? string.Empty : value;
                FalseAlarmFilter = false_alarm_string.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (FalseAlarmProbability)int.Parse(p)).ToList();
            }
        }

        private string ambig_state_string;
        /// <summary>
        /// 径向速度不确定性过滤器
        /// </summary>
        public string AmbigStateFilterString
        {
            get { return ambig_state_string; }
            set
            {
                ambig_state_string = value == null ? string.Empty : value;
                AmbigStateFilter = ambig_state_string.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (AmbigState)int.Parse(p)).ToList();
            }
        }

        private string invalid_state_string;
        /// <summary>
        /// 有效性(有效/高概率多目标)
        /// </summary>
        public string InvalidStateFilterString
        {
            get { return invalid_state_string; }
            set
            {
                invalid_state_string = value == null ? string.Empty : value;
                InvalidStateFilter = invalid_state_string.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (InvalidState)int.Parse(p)).ToList();
            }
        }

        private string meas_state_string;
        /// <summary>
        /// 测量状态过滤器
        /// </summary>
        public string MeasStateFilterString
        {
            get { return meas_state_string; }
            set
            {
                meas_state_string = value == null ? string.Empty : value;
                MeasStateFilter = meas_state_string.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (MeasState)int.Parse(p)).ToList();
            }
        }

        private string prob_exist_string;
        /// <summary>
        /// 存在概率过滤器
        /// </summary>
        public string ProbOfExistFilterString
        {
            get { return prob_exist_string; }
            set
            {
                prob_exist_string = value == null ? string.Empty : value;
                ProbOfExistFilter = prob_exist_string.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (ProbOfExist)int.Parse(p)).ToList();
            }
        }
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
            ProcFlag = true;
            RadarHeight = 0;
            Infos = new DataFrameMessages(this);
            State = new RadarState();
            Id = -1;
            Name = "ARS408-21";
            RefreshInterval = BaseConst.RefreshInterval;
            GroupType = RadarGroupType.None;
            IpAddress = BaseConst.IpAddress;
            Port = BaseConst.Port;
            ConnectionMode = BaseConst.ConnectionMode;
            UsingLocal = BaseConst.UsingLocal;
            PortLocal = BaseConst.Port_Local;
            RcsMinimum = BaseConst.RcsMinimum;
            RcsMinimum = BaseConst.RcsMaximum;
            ApplyFilter = true;
            ApplyIteration = true;
            PushfMaxCount = 1;
            UsePublicFilters = true;
            FalseAlarmFilterString = AmbigStateFilterString = InvalidStateFilterString = MeasStateFilterString = ProbOfExistFilterString = string.Empty;
        }

        /// <summary>
        /// 构造器，从公共变量获取属性值，再用给定的DataRow对象覆盖各属性的值
        /// </summary>
        /// <param name="row"></param>
        public Radar(DataRow row) : this()
        {
            if (row == null)
                return;

            Id = int.Parse(row["radar_id"].ToString());
            Name = row["radar_name"].ToString();
            RefreshInterval = int.Parse(row["refresh_interval"].ToString());
            IpAddress = row["ip_address"].ToString();
            Port = ushort.Parse(row["port"].ToString());
            ConnectionMode = (ConnectionMode)int.Parse(row["conn_mode_id"].ToString());
            UsingLocal = row["using_local"].ToString().Equals("1");
            IpAddressLocal = row["ip_address_local"].ToString();
            PortLocal = int.Parse(row["port_local"].ToString());
            OwnerShiploaderId = int.Parse(row["shiploader_id"].ToString());
            TopicName = row["topic_name"].ToString();
            OwnerGroupId = int.Parse(row["owner_group_id"].ToString());
            GroupType = (RadarGroupType)int.Parse(row["group_type"].ToString());
            DegreeYoz = double.Parse(row["degree_yoz"].ToString());
            DegreeXoy = double.Parse(row["degree_xoy"].ToString());
            DegreeXoz = double.Parse(row["degree_xoz"].ToString());
            DegreeGeneral = double.Parse(row["degree_general"].ToString());
            Direction = (Directions)int.Parse(row["direction_id"].ToString());
            DefenseMode = int.Parse(row["defense_mode_id"].ToString());
            Offset = double.Parse(row["offset"].ToString());
            XOffset = double.Parse(row["x_offset"].ToString());
            YOffset = double.Parse(row["y_offset"].ToString());
            ZOffset = double.Parse(row["z_offset"].ToString());
            Remark = row["remark"].ToString();
            ItemNameRadarState = row["item_name_radar_state"].ToString();
            ItemNameCollisionState = row["item_name_collision_state"].ToString();
            ItemNameCollisionState2 = row["item_name_collision_state_2"].ToString();
            RcsMinimum = int.Parse(row["rcs_min"].ToString());
            RcsMaximum = int.Parse(row["rcs_max"].ToString());
            RadarHeight = double.Parse(row["radar_height"].ToString());
            RadarCoorsLimited = row["radar_coors_limited"].ToString().Equals("1");
            RadarxMin = double.Parse(row["radar_x_min"].ToString());
            RadarxMax = double.Parse(row["radar_x_max"].ToString());
            RadaryMin = double.Parse(row["radar_y_min"].ToString());
            RadaryMax = double.Parse(row["radar_y_max"].ToString());
            ClaimerCoorsLimited = row["claimer_coors_limited"].ToString().Equals("1");
            ClaimerxMin = double.Parse(row["claimer_x_min"].ToString());
            ClaimerxMax = double.Parse(row["claimer_x_max"].ToString());
            ClaimeryMin = double.Parse(row["claimer_y_min"].ToString());
            ClaimeryMax = double.Parse(row["claimer_y_max"].ToString());
            ClaimerzMin = double.Parse(row["claimer_z_min"].ToString());
            ClaimerzMax = double.Parse(row["claimer_z_max"].ToString());
            AngleLimited = row["angle_limited"].ToString().Equals("1");
            AngleMin = double.Parse(row["angle_min"].ToString());
            AngleMax = double.Parse(row["angle_max"].ToString());
            #region 检测特性
            ApplyFilter = row["apply_filter"].ToString().Equals("1");
            ApplyIteration = row["apply_iteration"].ToString().Equals("1");
            PushfMaxCount = int.Parse(row["pushf_max_count"].ToString());
            UsePublicFilters = row["use_public_filters"].ToString().Equals("1");
            FalseAlarmFilterString = row["false_alarm_filter"].ToString();
            AmbigStateFilterString = row["ambig_state_filter"].ToString();
            InvalidStateFilterString = row["invalid_state_filter"].ToString();
            MeasStateFilterString = row["meas_state_filter"].ToString();
            ProbOfExistFilterString = row["prob_exist_filter"].ToString();
            //FalseAlarmFilter = row["false_alarm_filter"].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (FalseAlarmProbability)int.Parse(p)).ToList();
            //AmbigStateFilter = row["ambig_state_filter"].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (AmbigState)int.Parse(p)).ToList();
            //InvalidStateFilter = row["invalid_state_filter"].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (InvalidState)int.Parse(p)).ToList();
            //MeasStateFilter = row["meas_state_filter"].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (MeasState)int.Parse(p)).ToList();
            //ProbOfExistFilter = row["prob_exist_filter"].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (ProbOfExist)int.Parse(p)).ToList();
            #endregion
        }
        #endregion

        /// <summary>
        /// 更新修改后XYZ坐标的系数
        /// </summary>
        public void UpdateRatios()
        {
            XmodifiedRatios = new CoordinateRatios() { Xratio = cosphi * coslamda * cosg - sinphi * sing, Yratio = 0 - sintheta * sinlamda * cosg - costheta * sinphi * coslamda * cosg - costheta * cosphi * sing };
            YmodifiedRatios = new CoordinateRatios() { Xratio = cosphi * coslamda * sing + sinphi * cosg, Yratio = costheta * cosphi * cosg - costheta * sinphi * coslamda * sing - sintheta * sinlamda * sing };
            ZmodifiedRatios = new CoordinateRatios() { Xratio = cosphi * sinlamda, Yratio = sintheta * coslamda - costheta * sinphi * sinlamda };

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
