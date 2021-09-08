using ARS408.Core;
using ARS408.Forms;
using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARS408.Model
{
    /// <summary>
    /// 帧消息处理类
    /// </summary>
    public class DataFrameMessages
    {
        #region 私有成员
        private readonly Regex _pattern = new Regex(BaseConst.Pattern_SensorMessage, RegexOptions.Compiled);
        private SensorGeneral _general_most_threat;

        private int _count = 0;
        private readonly int limit_factor = 1;
        private double _new, _assumed, _diff, _diff1;
        //private readonly int _pushf_max_count = 1; //push finalization的最大次数
        private int _pushf_counter = 0; //计算push finalization的次数
        private readonly int _id_step = 500; //累积不同帧的点时为防止ID重复所添加的ID步长（与_pushf_counter结合使用）

        /// <summary>
        /// push finalization的最大次数
        /// </summary>
        private int PushfMaxCount { get { return Radar.PushfMaxCount; } }
        #endregion

        #region 公共属性
        /// <summary>
        /// 雷达信息对象
        /// </summary>
        public Radar Radar { get; set; }

        /// <summary>
        /// 是否是皮带雷达
        /// </summary>
        public bool IsBelt { get; private set; }

        /// <summary>
        /// 雷达目标点的过滤条件
        /// </summary>
        private List<bool> Flags { get; set; }

        private int _rcsMinimum = -64;
        /// <summary>
        /// RCS最小值
        /// </summary>
        public int RcsMinimum
        {
            get { return _rcsMinimum; }
            set { _rcsMinimum = value; }
        }

        private int _rcsMaximum = 64;
        /// <summary>
        /// RCS最大值
        /// </summary>
        public int RcsMaximum
        {
            get { return _rcsMaximum; }
            set { _rcsMaximum = value; }
        }

        /// <summary>
        /// 雷达状态信息
        /// </summary>
        public RadarState RadarState
        {
            get { return Radar != null ? Radar.State : null; }
            set
            {
                if (Radar != null)
                    Radar.State = value;
            }
        }

        /// <summary>
        /// 当前检测模式
        /// </summary>
        public SensorMode CurrentSensorMode { get; set; }

        /// <summary>
        /// 接收缓冲区大小（达到此大小则放入正式数据）
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        /// 接收目标实际数量
        /// </summary>
        public int ActualSize { get; set; }

        /// <summary>
        /// 接收缓冲区（存放临时数据，直到接收完一组数据再放入正式数据）
        /// </summary>
        public List<SensorGeneral> ListBuffer { get; set; }

        ///// <summary>
        ///// 接收缓冲区（除合格数据外的数据，不包括质量信息不合格、RCS不合格数据）（存放临时数据，直到接收完一组数据再放入正式数据）
        ///// </summary>
        //public List<SensorGeneral> ListBuffer_Other { get; set; }

        /// <summary>
        /// 接收缓冲区（除合格数据外其它所有数据）（存放临时数据，直到接收完一组数据再放入正式数据）
        /// </summary>
        public List<SensorGeneral> ListBuffer_AllOther { get; set; }

        /// <summary>
        /// 正式数据
        /// </summary>
        public List<SensorGeneral> ListTrigger { get; set; }

        /// <summary>
        /// 待发送列表
        /// </summary>
        public List<SensorGeneral> ListToSend { get; set; }

        /// <summary>
        /// 待发送列表（所有点）
        /// </summary>
        public List<SensorGeneral> ListToSendAll { get; set; }

        /// <summary>
        /// 历史数据记录
        /// </summary>
        public List<SensorGeneral> ListHistory { get; set; }

        /// <summary>
        /// 最具有威胁的集群或目标点
        /// </summary>
        public SensorGeneral GeneralMostThreat
        {
            get { return _general_most_threat; }
            set
            {
                _general_most_threat = value;
                CurrentDistance = _general_most_threat != null ? Math.Round(_general_most_threat.DistanceToBorder, 4) : 0;
                CurrentAngle = _general_most_threat != null ? Math.Round(_general_most_threat.Angle, 2) : 0;
            }
        }

        ///// <summary>
        ///// 下方离溜桶最近的集群或目标点
        ///// </summary>
        //public SensorGeneral GeneralHighest { get; set; }

        /// <summary>
        /// 当前障碍物距离，保留4位小数
        /// </summary>
        public double CurrentDistance
        {
            get { return Radar._current_dist; }
            set
            {
                IterateDistance(value);
                ThreatLevel = BaseFunc.GetThreatLevelByValue(Radar._current_dist, Radar != null ? Radar.GroupType : RadarGroupType.None);
            }
        }

        /// <summary>
        /// 当前障碍物所处角度
        /// </summary>
        public double CurrentAngle
        {
            get { return Radar._current_angle; }
            private set { Radar._current_angle = value; }
        }

        /// <summary>
        /// 报警级数
        /// </summary>
        public int ThreatLevel
        {
            get { return Radar._threat_level; }
            set
            {
                Radar._threat_level = value;
                ThreatLevelBinary = Convert.ToString(Radar._threat_level, 2).PadLeft(2, '0').Reverse();
            }
        }

        /// <summary>
        /// 报警级数2进制字符串（2位）
        /// </summary>
        public string ThreatLevelBinary
        {
            get { return Radar._threat_level_binary; }
            set { Radar._threat_level_binary = value; }
        }

        private int timer = 0;
        public int Timer
        {
            get { return timer; }
            set
            {
                timer = value;
                if (RadarState != null)
                    RadarState.Working = timer < 5 ? 1 : 0;
            }
        }

        /// <summary>
        /// 检查雷达工作状态的线程
        /// </summary>
        public Thread ThreadCheck { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="radar">雷达信息对象</param>
        public DataFrameMessages(Radar radar)
        {
            Radar = radar == null ? new Radar() : radar;
            Flags = new List<bool>() { false, false, false, false, false, false, false, true, true, true };
            limit_factor = Radar.GroupType == RadarGroupType.Feet ? 10 : 1;
            CurrentSensorMode = SensorMode.Cluster;
            ListBuffer = new List<SensorGeneral>();
            //ListBuffer_Other = new List<SensorGeneral>();
            ListBuffer_AllOther = new List<SensorGeneral>();
            ListTrigger = new List<SensorGeneral>();
            ListToSend = new List<SensorGeneral>();
            ListToSendAll = new List<SensorGeneral>();
            ThreadCheck = new Thread(new ThreadStart(CheckIfRadarsWorking)) { IsBackground = true };

            if (Radar != null)
            {
                IsBelt = Radar.GroupType == RadarGroupType.Belt; //是否为皮带雷达
                //Flags[4] = Radar.GroupType == RadarGroupType.Arm && Radar.Name.Contains("陆"); //是否为大臂陆侧雷达
                //Flags[6] = Radar.GroupType == RadarGroupType.Feet; //是否为门腿雷达
            }
            ThreadCheck.Start();
        }

        #region 功能
        public void CheckIfRadarsWorking()
        {
            int interval = 1;
            while (true)
            {
                Thread.Sleep(interval * 1000);
                Timer += interval;
            }
        }

        /// <summary>
        /// 处理输入信息
        /// </summary>
        /// <param name="input">输入的信息</param>
        public void Filter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            #region 方法3 新正则提取
            MatchCollection col = _pattern.Matches(input);
            if (col == null || col.Count == 0)
                return;
            foreach (Match match in col)
            {
                BaseMessage message = new BaseMessage(match.Value);
                dynamic obj;
                switch (message.MessageId)
                {
                    case SensorMessageId_0.RadarState_Out:
                        RadarState.Base = message;
                        break;
                    case SensorMessageId_0.Cluster_0_Status_Out:
                        obj = new ClusterStatus(message);
                        DataPushFinalize();
                        CurrentSensorMode = SensorMode.Cluster;
                        BufferSize = obj.NofClustersNear + obj.NofClustersFar;
                        break;
                    case SensorMessageId_0.Cluster_1_General_Out:
                        obj = new ClusterGeneral(message, Radar);
                        DataPush<ClusterGeneral>(obj);
                        ActualSize++;
                        break;
                    case SensorMessageId_0.Cluster_2_Quality_Out:
                        obj = new ClusterQuality(message);
                        DataQualityUpdate<ClusterQuality>(obj);
                        break;
                    case SensorMessageId_0.Obj_0_Status_Out:
                        obj = new ObjectStatus(message);
                        DataPushFinalize();
                        CurrentSensorMode = SensorMode.Object;
                        BufferSize = obj.NofObjects;
                        break;
                    case SensorMessageId_0.Obj_1_General_Out:
                        obj = new ObjectGeneral(message, Radar);
                        DataPush<ObjectGeneral>(obj);
                        ActualSize++;
                        break;
                    case SensorMessageId_0.Obj_2_Quality_Out:
                        obj = new ObjectQuality(message);
                        DataQualityUpdate<ObjectQuality>(obj);
                        break;
                    default:
                        continue;
                }
            }
            #endregion
        }
        #endregion

        ///// <summary>
        ///// 缓冲区数据长度
        ///// </summary>
        //public int ListBufferCount { get { return ListBuffer.Count + ListBuffer_AllOther.Count; } }

        /// <summary>
        /// 正式数据长度
        /// </summary>
        public int ListTriggerCount { get { return ListTrigger.Count; } }

        /// <summary>
        /// 将一般信息压入缓冲区
        /// </summary>
        /// <typeparam name="T">集群或目标一般信息类</typeparam>
        /// <param name="general">一般信息对象</param>
        public void DataPush<T>(T general) where T : SensorGeneral
        {
            #region 目标点的过滤
            Flags[2] = !general.RCS.Between(RcsMinimum, RcsMaximum); //RCS值是否不在范围内
            if (Radar != null)
            {
                Flags[1] = BaseConst.BorderDistThres > 0 && general.DistanceToBorder > BaseConst.BorderDistThres; //距边界距离是否小于0或超出阈值
                Flags[7] = !Radar.RadarCoorsLimited || general.WithinRadarLimits; //雷达坐标系坐标的限制
                Flags[8] = !Radar.ClaimerCoorsLimited || general.WithinClaimerLimits; //单机坐标系坐标的限制
                Flags[9] = !Radar.AngleLimited || general.WithinAngleLimits; //角度的限制
            }
            //TODO (所有雷达)过滤条件Lv1：RCS值、坐标、角度在限定范围内，距边界范围在阈值内
            bool save2list = !Flags[2] && Flags[7] && Flags[8] && Flags[9] && !Flags[1];
            ////YOZ角度加上俯仰角记为相对于水平方向的角度，取向下30°范围内的点
            //if (save2list && Radar.GroupType == RadarGroupType.Wheel && Radar.Name.Contains("斗轮"))
            //    save2list = (general.AngleYoz + BaseConst.OpcDataSource.PitchAngle_Plc).Between(-30, 0); //水平以下30°
            //TODO (其余数据)过滤条件Lv2：RCS值在范围内
            //bool save2other = !save2list && !Flags[2];
            //bool save2allother = !save2list;
            #endregion

            general.PushfCounter = _pushf_counter;
            general.Id += _pushf_counter * _id_step;
            if (save2list)
                ListBuffer.Add(general);
            if (!save2list)
                ListBuffer_AllOther.Add(general);
            //if (save2other)
            //    ListBuffer_Other.Add(general);
        }

        public void DataQualityUpdate<T>(T q) where T : SensorQuality
        {
            try
            {
                List<SensorGeneral> list = ListBuffer;
                q.Id += _pushf_counter * _id_step;
                SensorGeneral g = list.Find(c => c.Id == q.Id);
                if (g == null)
                {
                    list = ListBuffer_AllOther;
                    g = list.Find(c => c.Id == q.Id);
                }
                if (g == null)
                    return;

                if (q is ClusterQuality)
                {
                    ClusterQuality quality = q as ClusterQuality;
                    ClusterGeneral general = g as ClusterGeneral;
                    general.Pdh0 = quality.Pdh0;
                    general.InvalidState = quality.InvalidState;
                    general.AmbigState = quality.AmbigState;
                    List<FalseAlarmProbability> listFalseAlarm = Radar.UsePublicFilters ? ClusterQuality.FalseAlarmFilter : Radar.FalseAlarmFilter;
                    List<AmbigState> listAmbigState = Radar.UsePublicFilters ? ClusterQuality.AmbigStateFilter : Radar.AmbigStateFilter;
                    List<InvalidState> listInvalidState = Radar.UsePublicFilters ? ClusterQuality.InvalidStateFilter : Radar.InvalidStateFilter;
                    //TODO 集群模式输出结果过滤条件2：（过滤器启用、过滤器不为空）不在集群/不确定性/有效性过滤器内
                    if (BaseConst.ClusterFilterEnabled && Radar.ApplyFilter && (
                        (listFalseAlarm.Count > 0 && !listFalseAlarm.Contains(general.Pdh0)) ||
                        (listAmbigState.Count > 0 && !listAmbigState.Contains(general.AmbigState)) ||
                        (listInvalidState.Count > 0 && !listInvalidState.Contains(general.InvalidState))))
                    {
                        list.Remove(general);
                        ListBuffer_AllOther.Add(general);
                    }
                }
                else
                {
                    ObjectQuality quality = q as ObjectQuality;
                    ObjectGeneral general = g as ObjectGeneral;
                    general.MeasState = quality.MeasState;
                    general.ProbOfExist = quality.ProbOfExist;
                    List<MeasState> listMeasState = Radar.UsePublicFilters ? ObjectQuality.MeasStateFilter : Radar.MeasStateFilter;
                    List<ProbOfExist> listProbExist = Radar.UsePublicFilters ? ObjectQuality.ProbOfExistFilter : Radar.ProbOfExistFilter;
                    //TODO 目标模式输出结果过滤条件2：（假如过滤器启用）判断存在概率的可能最小值是否小于允许的最低值
                    //if (BaseConst.ObjectFilterEnabled && Radar.ApplyFilter && ((ObjectQuality.MeasStateFilter.Count > 0 && !ObjectQuality.MeasStateFilter.Contains(general.MeasState)) ||
                    //    (ObjectQuality.ProbOfExistFilter.Count > 0 && !ObjectQuality.ProbOfExistFilter.Contains(general.ProbOfExist))))
                    if (BaseConst.ObjectFilterEnabled && Radar.ApplyFilter && (
                        (listMeasState.Count > 0 && !listMeasState.Contains(general.MeasState)) ||
                        (listProbExist.Count > 0 && !listProbExist.Contains(general.ProbOfExist))))
                    {
                        list.Remove(general);
                        ListBuffer_AllOther.Add(general);
                    }
                }
            }
            catch (Exception) { }
        }

        private readonly Queue<double> surfaceAnglesQueue = new Queue<double>();
        //int counter = 0;
        //private int max_count = 3;
        /// <summary>
        /// 结束一个阶段的数据压入，将缓冲区数据汇入正式数据
        /// </summary>
        public void DataPushFinalize()
        {
            //假如应获取的集群/目标数量不为0但实际未收到，则退出（收到了空的帧）
            if (BufferSize != 0 && ActualSize == 0)
                return;

            if (++_pushf_counter >= PushfMaxCount)
            {
                //不要添加ListBuffer_Cluster与ListBuffer_Cluster_Other数量是否均为0的判断，否则当不存在目标时无法及时反映在数据上
                ListBuffer.Sort(SensorGeneral.DistanceComparison); //根据距检测区的最短距离排序
                //对于料流雷达，计算所有点的X坐标平均值（计算到下方皮带的平均距离）
                if (Radar.GroupType == RadarGroupType.Belt)
                    CurrentDistance = ListBuffer.Count > 0 ? Math.Round(ListBuffer.Select(g => g.DistLong).Average(), 3) : 0;
                //对于非料流雷达：找出距离最小的点
                else
                    GeneralMostThreat = ListBuffer.Count() > 0 ? ListBuffer.First() : null;
                ListTrigger.Clear();
                ListToSend.Clear();
                ListToSendAll.Clear();
                ListTrigger.AddRange(ListBuffer);
                ListToSend.AddRange(ListBuffer);
                ListToSendAll.AddRange(ListBuffer);
                if (BaseConst.ShowDesertedPoints)
                    ListTrigger.AddRange(ListBuffer_AllOther);
                //ListToSend.AddRange(ListBuffer_Other);
                ListToSendAll.AddRange(ListBuffer_AllOther);
                Radar.ProcFlag = true; //设置雷达处理标志，表示未处理过
                #region 斗轮雷达处理
                ////计算斗轮雷达点的1次拟合斜率，纵向坐标1~15，横向坐标-10~10，剔除10个距离其它点最远的点
                //if (Radar.GroupType == RadarGroupType.Wheel && Radar.Name.Contains("斗轮"))
                //{
                //    string message;
                //    bool onLeft = Radar.Name.Contains("左");
                //    //double[] modelArray;
                //    //根据距其它点的距离和来排除的点比例
                //    double dist_ex_count = 0.2;
                //    //xy坐标取值范围，xy坐标偏移量
                //    double[] xybias = new double[] { Radar.YOffset, Radar.ZOffset };
                //    double rangle = Radar.DegreeXoz;
                //    //最后处理角度为90°-返回结果（后两个角度均带正负号）
                //    //double angle = 90 - (BaseFunc.GetSurfaceAngle(ListToSend, xlimit, ylimit, xybias, radar_angle, dist_ex_count, false, out Radar._radius_average, out message)/* + radar_angle + BaseConst.OpcDataSource.PitchAngle_Plc*/);
                //    //double angle = 90 - BaseFunc.GetSurfaceAngle(ListToSend, 1, 11.652 - Radar.YOffset, -10, 10, 0.2, false, out message) - Radar.DegreeXoz - BaseConst.OpcDataSource.PitchAngle_Plc;
                //    //double angle = 90 - BaseFunc.GetSurfaceAngleV1(ListToSend, 1, 11.652 - Radar.YOffset, -10, 10, 0.2, false, out Radar._radius_average, out message) - Radar.DegreeXoz - BaseConst.OpcDataSource.PitchAngle_Plc;
                //    double angle = 90 - BaseFunc.GetSurfaceAngleV2(ListToSend, xybias, rangle, BaseConst.OpcDataSource.PitchAngle_Plc, dist_ex_count, false, out Radar._radius_average, out message);
                //    //angle = double.IsNaN(angle) ? -1 : angle;
                //    if (!double.IsNaN(angle))
                //        surfaceAnglesQueue.Enqueue(angle);
                //    while(surfaceAnglesQueue.Count > BaseConst.SurfaceAngleSampleLength)
                //        surfaceAnglesQueue.Dequeue();
                //    Radar._surface_angle = surfaceAnglesQueue.Count == 0 ? 0 : surfaceAnglesQueue.Average();
                //    //Radar._out_of_stack = MatlabFunctions.IsOutOfStack(modelArray);
                //}
                #endregion
                ListBuffer.Clear();
                //ListBuffer_Other.Clear();
                ListBuffer_AllOther.Clear();
                _pushf_counter = 0;
            }
            ActualSize = 0;
        }

        /// <summary>
        /// 用新值来迭代距障碍物的距离
        /// </summary>
        /// <param name="value">新的距离值</param>
        public void IterateDistance(double value)
        {
            _new = value; //新值
            _diff = Math.Abs(_new - Radar._current_dist); //新值与当前值的差
            _diff1 = Math.Abs(_new - _assumed); //新值与假定值的差
            //假如未启用迭代 / 当前值为0 / 新值与当前值的差不超过距离限定值：计数置0，用新值取代现有值
            if (!(BaseConst.IterationEnabled && Radar.ApplyIteration) || _diff <= BaseConst.IteDistLimit * limit_factor)
            {
                _count = 0;
                Radar._current_dist = _new;
            }
            //假如新值与当前值的差超过距离限定值，计数刷新，用新值取代假定值
            else
            {
                //假如新值与假定值的差未超过距离限定值，计数+1（否则置0）
                _count = _diff1 <= BaseConst.IteDistLimit * limit_factor ? _count + 1 : 0;
                _assumed = _new;
                //假如计数超过计数限定值，则用新值取代现有值
                if (_count > BaseConst.IteCountLimit)
                {
                    Radar._current_dist = _new;
                    _count = 0;
                }
            }
        }
    }
}
