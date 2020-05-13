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
        #endregion

        #region 属性
        /// <summary>
        /// 父窗体
        /// </summary>
        public FormDisplay ParentForm { get; set; }

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

        /// <summary>
        /// 雷达状态信息
        /// </summary>
        public RadarState RadarState
        {
            get { return this.Radar.State; }
            set { this.Radar.State = value; }
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

        /// <summary>
        /// 接收缓冲区（其它）（存放临时数据，直到接收完一组数据再放入正式数据）
        /// </summary>
        public List<SensorGeneral> ListBuffer_Other { get; set; }

        /// <summary>
        /// 正式数据
        /// </summary>
        public List<SensorGeneral> ListTrigger { get; set; }

        /// <summary>
        /// 最具有威胁的集群或目标点
        /// </summary>
        public SensorGeneral GeneralMostThreat
        {
            get { return this._general_most_threat; }
            set
            {
                this._general_most_threat = value;
                this.CurrentDistance = this._general_most_threat != null ? Math.Round(this._general_most_threat.DistanceToBorder, 4) : 0;
            }
        }

        /// <summary>
        /// 下方离溜桶最近的集群或目标点
        /// </summary>
        public SensorGeneral GeneralHighest { get; set; }

        /// <summary>
        /// 当前障碍物距离，保留4位小数
        /// </summary>
        public double CurrentDistance
        {
            get { return this.Radar._current; }
            set
            {
                this.IterateDistance(value);
                this.ThreatLevel = BaseFunc.GetThreatLevelByValue(this.Radar._current, this.Radar != null ? this.Radar.GroupType : RadarGroupType.None);
            }
        }

        /// <summary>
        /// 报警级数
        /// </summary>
        public int ThreatLevel
        {
            get { return this.Radar._threat_level; }
            set
            {
                this.Radar._threat_level = value;
                this.ThreatLevelBinary = Convert.ToString(this.Radar._threat_level, 2).PadLeft(2, '0').Reverse();
            }
        }

        /// <summary>
        /// 报警级数2进制字符串（2位）
        /// </summary>
        public string ThreatLevelBinary
        {
            get { return this.Radar._threat_level_binary; }
            set { this.Radar._threat_level_binary = value; }
        }
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public DataFrameMessages() : this(null, null) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="form">父窗体</param>
        public DataFrameMessages(FormDisplay form) : this(form, null) { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="form">父窗体</param>
        /// <param name="radar">雷达信息对象</param>
        public DataFrameMessages(FormDisplay form, Radar radar)
        {
            this.ParentForm = form;
            //this.Radar = radar;
            this.Radar = radar == null ? new Radar() : radar;
            this.Flags = new List<bool>() { false, false, false, false, false, false, false, true, true };
            this.limit_factor = this.Radar.GroupType == RadarGroupType.Feet ? 10 : 1;
            this.CurrentSensorMode = SensorMode.Cluster;
            this.ListBuffer = new List<SensorGeneral>();
            this.ListBuffer_Other = new List<SensorGeneral>();
            this.ListTrigger = new List<SensorGeneral>();
            this.ThreadCheck = new Thread(new ThreadStart(this.CheckIfRadarsWorking)) { IsBackground = true };

            if (this.Radar != null)
            {
                this.IsBelt = this.Radar.GroupType == RadarGroupType.Belt; //是否为皮带雷达
                this.Flags[4] = this.Radar.GroupType == RadarGroupType.Arm && this.Radar.Name.Contains("陆"); //是否为大臂陆侧雷达
                this.Flags[6] = this.Radar.GroupType == RadarGroupType.Feet; //是否为门腿雷达
            }
            this.ThreadCheck.Start();
        }

        private int timer = 0;
        public int Timer
        {
            get { return this.timer; }
            set
            {
                this.timer = value;
                this.RadarState.Working = this.timer < 5 ? 1 : 0;
            }
        }

        public Thread ThreadCheck;
        public void CheckIfRadarsWorking()
        {
            int interval = 1;
            while (true)
            {
                Thread.Sleep(interval * 1000);
                this.Timer += interval;
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
                        this.RadarState.Base = message;
                        break;
                    case SensorMessageId_0.Cluster_0_Status_Out:
                        obj = new ClusterStatus(message);
                        this.DataPushFinalize();
                        this.CurrentSensorMode = SensorMode.Cluster;
                        this.BufferSize = obj.NofClustersNear + obj.NofClustersFar;
                        break;
                    case SensorMessageId_0.Cluster_1_General_Out:
                        obj = new ClusterGeneral(message, this.Radar);
                        this.DataPush<ClusterGeneral>(obj);
                        this.ActualSize++;
                        break;
                    case SensorMessageId_0.Cluster_2_Quality_Out:
                        obj = new ClusterQuality(message);
                        this.DataQualityUpdate<ClusterQuality>(obj);
                        break;
                    case SensorMessageId_0.Obj_0_Status_Out:
                        obj = new ObjectStatus(message);
                        this.DataPushFinalize();
                        this.CurrentSensorMode = SensorMode.Object;
                        this.BufferSize = obj.NofObjects;
                        break;
                    case SensorMessageId_0.Obj_1_General_Out:
                        obj = new ObjectGeneral(message, this.Radar);
                        this.DataPush<ObjectGeneral>(obj);
                        this.ActualSize++;
                        break;
                    case SensorMessageId_0.Obj_2_Quality_Out:
                        obj = new ObjectQuality(message);
                        this.DataQualityUpdate<ObjectQuality>(obj);
                        break;
                    default:
                        continue;
                }
            }
            #endregion
        }

        /// <summary>
        /// 缓冲区数据长度
        /// </summary>
        public int ListBufferCount { get { return this.ListBuffer.Count + this.ListBuffer_Other.Count; } }

        /// <summary>
        /// 正式数据长度
        /// </summary>
        public int ListTriggerCount { get { return this.ListTrigger.Count; } }

        /// <summary>
        /// 将一般信息压入缓冲区
        /// </summary>
        /// <typeparam name="T">集群或目标一般信息类</typeparam>
        /// <param name="general">一般信息对象</param>
        public void DataPush<T>(T general) where T : SensorGeneral
        {
            #region 目标点的过滤
            Flags[2] = this.ParentForm != null && !general.RCS.Between(this.ParentForm.RcsMinimum, this.ParentForm.RcsMaximum); //RCS值是否不在范围内
            if (this.Radar != null)
            {
                Flags[1] = (BaseConst.BorderDistThres > 0 && general.DistanceToBorder > BaseConst.BorderDistThres); //距边界距离是否小于0（溜桶雷达）或超出阈值
                //Flags[1] = (this.Radar.GroupType == RadarGroupType.Wheel && g.DistanceToBorder < 0) || (BaseConst.BorderDistThres > 0 && g.DistanceToBorder > BaseConst.BorderDistThres); //距边界距离是否小于0（溜桶雷达）或超出阈值
                //Flags[3] = this.Radar.GroupType == RadarGroupType.Arm && !z.Between(0 - BaseConst.BucketHeight, BaseConst.BucketUpLimit); //溜桶雷达Z方向坐标是否低于大铲最低点，或高于检测高度上限
                //Flags[5] = below.Between(0, BaseConst.ObsBelowThres) && g.DistanceToBorder < BaseConst.ObsBelowFrontier; //障碍物在溜桶下方的距离在阈值(ObsBelowThres)内，且距边界距离不超过1米(ObsBelowFrontier)
                Flags[7] = !this.Radar.RadarCoorsLimited || general.WithinRadarLimits; //雷达坐标系坐标的限制
                Flags[8] = !this.Radar.ClaimerCoorsLimited || general.WithinClaimerLimits; //单机坐标系坐标的限制
                //Flags[7] = !this.Radar.RadarCoorsLimited || (lon.Between(this.Radar.RadarxMin, this.Radar.RadarxMax) && lat.Between(this.Radar.RadaryMin, this.Radar.RadaryMax)); //雷达坐标系坐标的限制
                //Flags[8] = !this.Radar.ClaimerCoorsLimited || (x.Between(this.Radar.ClaimerxMin, this.Radar.ClaimerxMax) && y.Between(this.Radar.ClaimeryMin, this.Radar.ClaimeryMax) && z.Between(this.Radar.ClaimerzMin, this.Radar.ClaimerzMax)); //单机坐标系坐标的限制
            }
            //TODO (所有雷达)过滤条件Lv1：RCS值、坐标在限定范围内 / RCS值在范围内
            bool save2list = !Flags[2] && Flags[7] && Flags[8], save2other = false;
            ////非皮带雷达额外判断
            //if (!this.IsBelt)
            //{
            //TODO (非臂架下方)过滤条件Lv2：距边界范围在阈值内，溜桶雷达Z方向坐标不低于大铲最低点
            save2list = save2list && !(Flags[1] || Flags[3]);
            //TODO (臂架下方)过滤条件Lv2：RCS值在范围内，障碍物在溜桶下方的距离在阈值内、且距边界距离不超过1米
            if (!save2list)
                save2other = !Flags[2] && Flags[5];
            //}
            #region 旧判断逻辑
            //if (this.IsShore)
            //    save2list = !Flags[2] && z.Between(-1, 1) && x.Between(-5, 5);
            //else
            //{
            //    //非岸基输出结果过滤条件1：距边界范围在阈值内，RCS值在范围内，溜桶雷达Z方向坐标不低于大铲最低点
            //    if (!(Flags[1] || Flags[2] || Flags[3]))
            //    {
            //        //非大臂陆侧
            //        if (!Flags[4])
            //            //非门腿雷达直接返回true，门腿雷达判断：横向距离在黄标线范围内,高度高于地面0.1米以上
            //            save2list = !Flags[6] ? true : lat.Between(this.outer, this.inner) && z > BaseConst.FeetFilterHeight - this.Radar.RadarHeight;
            //        else
            //            //大臂陆侧过滤条件：横向坐标在5~10，纵向坐标在5~10之间
            //            save2list = lat.Between(0, 5) && lon.Between(5, 10);
            //    }
            //    //溜桶下方障碍物过滤条件：RCS值在范围内，障碍物在溜桶下方的距离在阈值内、且距边界距离不超过1米
            //    else
            //        save2other = !(Flags[2]) && Flags[5];
            //}
            #endregion
            #endregion

            //TODO 溜桶下方物体检测：另外添加2个ListBuffer，分别对应cluster和object，添加点，DataPushFinalize时一起压入ListTrigger
            if (save2list)
                this.ListBuffer.Add(general);
            else if (save2other)
                this.ListBuffer_Other.Add(general);
        }

        public void DataQualityUpdate<T>(T q) where T : SensorQuality
        {
            try
            {
                List<SensorGeneral> list = this.ListBuffer;
                SensorGeneral g = list.Find(c => c.Id == q.Id);
                if (g == null)
                {
                    list = this.ListBuffer_Other;
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
                    //TODO 集群模式输出结果过滤条件2：（过滤器启用、过滤器不为空）不在集群/不确定性/有效性过滤器内
                    if (BaseConst.ClusterFilterEnabled && ((ClusterQuality.FalseAlarmFilter.Count > 0 && !ClusterQuality.FalseAlarmFilter.Contains(general.Pdh0)) ||
                        (ClusterQuality.AmbigStateFilter.Count > 0 && !ClusterQuality.AmbigStateFilter.Contains(general.AmbigState)) ||
                        (ClusterQuality.InvalidStateFilter.Count > 0 && !ClusterQuality.InvalidStateFilter.Contains(general.InvalidState))))
                        list.Remove(general);
                }
                else
                {
                    ObjectQuality quality = q as ObjectQuality;
                    ObjectGeneral general = g as ObjectGeneral;
                    general.MeasState = quality.MeasState;
                    general.ProbOfExist = quality.ProbOfExist;
                    //TODO 目标模式输出结果过滤条件2：（假如过滤器启用）判断存在概率的可能最小值是否小于允许的最低值
                    if (BaseConst.ObjectFilterEnabled && ((ObjectQuality.MeasStateFilter.Count > 0 && !ObjectQuality.MeasStateFilter.Contains(general.MeasState)) ||
                        (ObjectQuality.ProbOfExistFilter.Count > 0 && !ObjectQuality.ProbOfExistFilter.Contains(general.ProbOfExist))))
                        list.Remove(general);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 结束一个阶段的数据压入，将缓冲区数据汇入正式数据
        /// </summary>
        public void DataPushFinalize()
        {
            //假如应获取的集群/目标数量不为0但实际未收到，则退出（收到了空的帧）
            if (this.BufferSize != 0 && this.ActualSize == 0)
                return;

            //不要添加this.ListBuffer_Cluster与ListBuffer_Cluster_Other数量是否均为0的判断，否则当不存在目标时无法及时反映在数据上
            if (this.Radar != null)
            {
                this.ListBuffer.Sort((a, b) => a.DistanceToBorder.CompareTo(b.DistanceToBorder)); //根据距检测区的最短距离排序
                this.ListBuffer_Other.Sort((a, b) => a.ModiCoors.Z.CompareTo(b.ModiCoors.Z)); //根据Z轴坐标排序
                this.GeneralMostThreat = this.ListBuffer.Count() > 0 ? this.ListBuffer.First() : null; //找出距离最小的点
                this.GeneralHighest = this.ListBuffer_Other.Count() > 0 ? this.ListBuffer_Other.Last() : null; //找出Z轴坐标最大的点（最高的点）
            }
            this.ListTrigger.Clear();
            this.ListTrigger.AddRange(this.ListBuffer);
            this.ListTrigger.AddRange(this.ListBuffer_Other);
            this.ListBuffer.Clear();
            this.ListBuffer_Other.Clear();
            this.ActualSize = 0;
        }

        /// <summary>
        /// 用新值来迭代距障碍物的距离
        /// </summary>
        /// <param name="value">新的距离值</param>
        public void IterateDistance(double value)
        {
            _new = value; //新值
            _diff = Math.Abs(_new - this.Radar._current); //新值与当前值的差
            _diff1 = Math.Abs(_new - _assumed); //新值与假定值的差
            //假如未启用迭代 / 当前值为0 / 新值与当前值的差不超过距离限定值：计数置0，用新值取代现有值
            if (!BaseConst.IterationEnabled || _diff <= BaseConst.IteDistLimit * this.limit_factor)
            {
                _count = 0;
                this.Radar._current = _new;
            }
            //假如新值与当前值的差超过距离限定值，计数刷新，用新值取代假定值
            else
            {
                //假如新值与假定值的差未超过距离限定值，计数+1（否则置0）
                _count = _diff1 <= BaseConst.IteDistLimit * this.limit_factor ? _count + 1 : 0;
                _assumed = _new;
                //假如计数超过计数限定值，则用新值取代现有值
                if (_count > BaseConst.IteCountLimit)
                {
                    this.Radar._current = _new;
                    _count = 0;
                }
            }
        }
    }
}
