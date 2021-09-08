using ARS408.Core;
using CommonLib.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Model
{
    /// <summary>
    /// 网格聚类
    /// </summary>
    public class BlockCluster
    {
        /// <summary>
        /// 核心网格
        /// </summary>
        public BlockUnit CoreBlock { get; private set; }

        /// <summary>
        /// 普通网格列表
        /// </summary>
        public List<BlockUnit> CommonBlocks { get; set; }

        /// <summary>
        /// 前部网格列表
        /// </summary>
        public List<BlockUnit> BlocksInFront { get; set; }

        /// <summary>
        /// 中部网格列表
        /// </summary>
        public List<BlockUnit> BlocksInMiddle { get; set; }

        /// <summary>
        /// 后部网格列表
        /// </summary>
        public List<BlockUnit> BlocksInBack { get; set; }

        /// <summary>
        /// 网格聚类对象的面积，为聚类内所有网格的面积和
        /// </summary>
        public double Area { get; private set; }

        private double _centerx = 0;
        /// <summary>
        /// 目标形心X坐标
        /// </summary>
        public double CenterX
        {
            get { return _centerx; }
            private set
            {
                _centerx = value;
                Angle = _centery == 0 ? 0 : Math.Atan(_centerx / _centery);
            }
        }

        private double _centery = 0;
        /// <summary>
        /// 目标形心Y坐标
        /// </summary>
        public double CenterY
        {
            get { return _centery; }
            private set
            {
                _centery = value;
                Angle = _centery == 0 ? 0 : Math.Atan(_centerx / _centery);
            }
        }

        /// <summary>
        /// 网格聚类形心与坐标原点（回转轴）连线相对于坐标系Y轴（大臂）的角度
        /// </summary>
        public double Angle { get; private set; }

        /// <summary>
        /// RCS平均值
        /// </summary>
        public double RcsAver { get; private set; }

        /// <summary>
        /// 网格聚类类型
        /// </summary>
        public BlockClusterType Type { get; set; }

        /// <summary>
        /// 距离数据
        /// </summary>
        public Distances Distances { get; set; }

        /// <summary>
        /// 用指定的核心网格初始化网格聚类对象
        /// </summary>
        /// <param name="core"></param>
        public BlockCluster(BlockUnit core)
        {
            CoreBlock = core;
            CommonBlocks = new List<BlockUnit>();
            BlocksInFront = new List<BlockUnit>();
            BlocksInMiddle = new List<BlockUnit>();
            BlocksInBack = new List<BlockUnit>();
            Distances = new Distances();
            if (core != null)
            {
                core.Position = (BlockPosition)BlockConst.FieldBorders.Where(b => b < core.CenterY).Count(); //更新网格单元位置
                switch (core.Position)
                {
                    case BlockPosition.Front:
                        BlocksInFront.Add(core);
                        break;
                    case BlockPosition.Middle:
                        BlocksInMiddle.Add(core);
                        break;
                    case BlockPosition.Back:
                        BlocksInBack.Add(core);
                        break;
                }
            }
        }

        /// <summary>
        /// 将普通网格添加进入网格聚类中
        /// </summary>
        /// <param name="block">待添加的网格</param>
        public void AddCommonBlock(BlockUnit block)
        {
            //假如网格类型不是普通网格，或所属的核心网格不为空（已属于某个核心网格），放弃添加
            if (CoreBlock == null || block == null || block.Type != BlockType.Common || block.CoreBlock != null)
                return;
            //假如网格单元中心X坐标小于等于大臂的范围，则排除
            if (Math.Abs(block.CenterX) <= BlockConst.MainArmScopeX)
                return;
            block.CoreBlock = CoreBlock;
            block.Position = (BlockPosition)BlockConst.FieldBorders.Where(b => b < block.CenterY).Count(); //更新网格单元位置
            CommonBlocks.Add(block);
            switch (block.Position)
            {
                case BlockPosition.Front:
                    BlocksInFront.Add(block);
                    break;
                case BlockPosition.Middle:
                    BlocksInMiddle.Add(block);
                    break;
                case BlockPosition.Back:
                    BlocksInBack.Add(block);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 更新网格集群属性，包括面积、形心坐标以及RCS均值
        /// </summary>
        public void RefreshProperties()
        {
            CommonBlocks = BaseFunc.GetOutlierFilteredBlocks(CommonBlocks); //对一般网格单元进行统计滤波
            //计算网格聚类面积和
            Area = CoreBlock == null ? 0 : CoreBlock.Area + CommonBlocks.Select(block => block.Area).Sum();
            //计算网格聚类形心XY坐标
            CenterX = CoreBlock == null ? 0 : (CoreBlock.CenterX + CommonBlocks.Select(block => block.CenterX).Sum()) / (CommonBlocks.Count + 1);
            CenterY = CoreBlock == null ? 0 : (CoreBlock.CenterY + CommonBlocks.Select(block => block.CenterY).Sum()) / (CommonBlocks.Count + 1);
            //计算平均RCS值
            RcsAver = CoreBlock == null ? -64 : (CoreBlock.RcsSum + CommonBlocks.Select(block => block.RcsSum).Sum()) / (CoreBlock.Weight + CommonBlocks.Select(block => block.Weight).Sum());
            //判断是否为大臂的一部分
            if (Math.Abs(_centerx) <= BlockConst.MainArmScopeX)
                Type = BlockClusterType.MainArm;
            //判断是否为地面杂波或小煤堆
            else if (Area < BlockConst.ClutterThresholds[0] && RcsAver < BlockConst.ClutterThresholds[1])
                Type = BlockClusterType.Clutter;
            ////回转角度与网格聚类形心角度应刚好相反
            //else if (Math.Round(Angle + BaseConst.OpcDataSource.YawAngle) <= BlockConst.FoundAngleMarg)
            //    Type = BlockClusterType.Foundation;
            else
                Type = BlockClusterType.Normal;
            BlocksInFront.Sort(BlockUnit.CenterXAbsComparison);
            BlocksInMiddle.Sort(BlockUnit.CenterXAbsComparison);
            BlocksInBack.Sort(BlockUnit.CenterXAbsComparison);
            BlockUnit blockFront = BlocksInFront.FirstOrDefault(), blockMiddle = BlocksInMiddle.FirstOrDefault(), blockBack = BlocksInBack.FirstOrDefault();
            Distances.Update(blockFront, blockMiddle, blockBack);
        }
    }

    /// <summary>
    /// 前中后距离数据结构体
    /// </summary>
    public class Distances
    {
        private const double LENGTH_FRONT = 48, LENGTH_MIDDLE = 42, LENGTH_BACK = 22; //前部、中部、后部中心距离回转中心的距离
        private readonly double _def = BlockConst.DefaultDistance; //默认距离值
        private readonly bool _glitchRemoving = false; //是否移除毛刺（数据突变）

        /// <summary>
        /// 朝向方向
        /// </summary>
        public Directions Direction { get; private set; }

        ///// <summary>
        ///// 获取前中后部测距中最近的距离
        ///// </summary>
        //public double Nearest { get { return new double[] { Front.Value, Middle.Value, Back.Value }.Min(); } }

        /// <summary>
        /// 前部距离
        /// </summary>
        public Distance Front { get; set; }

        /// <summary>
        /// 中部距离
        /// </summary>
        public Distance Middle { get; set; }

        /// <summary>
        /// 后部距离
        /// </summary>
        public Distance Back { get; set; }

        /// <summary>
        /// 默认构造器，距离设为网格测距默认距离，默认不移除毛刺、无明确朝向
        /// </summary>
        public Distances() : this(Directions.None, BlockConst.DefaultDistance, false) { }

        /// <summary>
        /// 根据给定的朝向、是否移除毛刺初始化，距离设为网格测距默认距离
        /// </summary>
        /// <param name="dir">所朝方向</param>
        /// <param name="glitchRemoving">是否移除突变毛刺</param>
        public Distances(Directions dir, bool glitchRemoving) : this(dir, BlockConst.DefaultDistance, glitchRemoving) { }

        /// <summary>
        /// 根据给定的朝向、距离值、是否移除毛刺初始化
        /// </summary>
        /// <param name="dir">所朝方向</param>
        /// <param name="def">给定距离值</param>
        /// <param name="glitchRemoving">是否移除突变毛刺</param>
        public Distances(Directions dir, double def, bool glitchRemoving)
        {
            Direction = dir;
            _def = def;
            _glitchRemoving = glitchRemoving;
            Front = new Distance(_glitchRemoving, _def, LENGTH_FRONT);
            Middle = new Distance(_glitchRemoving, _def, LENGTH_MIDDLE);
            Back = new Distance(_glitchRemoving, _def, LENGTH_BACK);
            ResetDistances(_def);
        }

        /// <summary>
        /// 设置前部、中部、后部距离校正值
        /// </summary>
        /// <param name="corrs">提供校正值的数组，假如为空或长度小于3则跳过</param>
        public void SetDistCorrs(double[] corrs)
        {
            if (corrs == null || corrs.Length < 3)
                return;
            Front.DistCorr = corrs[0];
            Middle.DistCorr = corrs[1];
            Back.DistCorr = corrs[2];
        }

        /// <summary>
        /// 根据给定的距离设置前中后部距离
        /// </summary>
        /// <param name="dist">距离值</param>
        public void ResetDistances(double dist)
        {
            Front.SetValue(dist);
            Middle.SetValue(dist);
            Back.SetValue(dist);
        }

        /// <summary>
        /// 根据前部、中部、后部网格单元包含的一般雷达信息更新距离
        /// </summary>
        /// <param name="front"></param>
        /// <param name="middle"></param>
        /// <param name="back"></param>
        public void Update(BlockUnit front, BlockUnit middle, BlockUnit back)
        {
            Front.SetValue(front == null ? _def : front.Distance);
            Middle.SetValue(middle == null ? _def : middle.Distance);
            Back.SetValue(back == null ? _def : back.Distance);
        }

        /// <summary>
        /// 用新的前中后距离值代替当前距离值
        /// </summary>
        /// <param name="other"></param>
        public void Copy(Distances other)
        {
            if (other == null)
                return;
            Front.SetValue(other.Front.Value);
            Middle.SetValue(other.Middle.Value);
            Back.SetValue(other.Back.Value);
        }

        /// <summary>
        /// 用新的前中后距离值迭代当前距离值，假如更小则替换
        /// </summary>
        /// <param name="other"></param>
        public void Iterate(Distances other)
        {
            if (other == null)
                return;
            Front.Iterate(other.Front);
            Middle.Iterate(other.Middle);
            Back.Iterate(other.Back);
        }

        /// <summary>
        /// 根据给定数字标识获取前中后距离数值，以逗号分隔
        /// </summary>
        /// <param name="type">数字标识：1 原始数值，2 远距过滤数值，3 固定距离过滤数值，4 卡尔曼过滤数值</param>
        /// <returns></returns>
        public string GetValues(int type)
        {
            return string.Format("{0}{1}{2}", Front.GetValueString(type), Middle.GetValueString(type), Back.GetValueString(type));
        }
    }

    public class Distance
    {
        private readonly double _def = BlockConst.DefaultDistance; //默认距离值
        private readonly double _length; //距离回转中心的距离
        private readonly bool _glitchRemoving = false; //是否移除毛刺（数据突变）
        private readonly GlitchFilter _glitchFilter; //去除尖刺工具
        private readonly KalmanFilter _kalmanFilter; //卡尔曼滤波工具
        private double _value_ori, _value_ref, _value_cur, _value_kal;

        /// <summary>
        /// 距离校正值
        /// </summary>
        public double DistCorr { get; set; }

        private double _value, _value_prev;
        /// <summary>
        /// 距离值
        /// </summary>
        public double Value
        {
            get { return _value; }
            private set
            {
                _value = value;
                Level = BaseFunc.GetThreatLevelByValue(_value);
            }
        }

        private int _level/*, _level_prev*/;
        /// <summary>
        /// 距离级别
        /// </summary>
        public int Level
        {
            get { return _level; }
            private set
            {
                //_level_prev = _level;
                ////TODO 当上一个报警级别为3时，只在最新报警级别为0时更新，否则维持3
                //if (_level_prev == 3 && value != 0)
                //    return;
                //_level = value;
                _level = _level == 3 && value != 0 ? 3 : value;
            }
        }

        /// <summary>
        /// 数据是否已做好准备
        /// </summary>
        public bool SlidedIntoRunway { get; private set; }

        /// <summary>
        /// 初始化距离实体
        /// </summary>
        /// <param name="glitchRemoving">是否去除尖刺</param>
        /// <param name="def">距离默认值</param>
        /// <param name="length">所在位置距离回转中心距离</param>
        public Distance(bool glitchRemoving, double def, double length)
        {
            _glitchRemoving = glitchRemoving;
            _length = length;
            _def = def;
            Value = _def;
            _glitchFilter = new GlitchFilter(_glitchRemoving, 6, 5) { LongDistAvoidEnabled = true, LongDist2Avoid = BlockConst.DefaultDistance, LongDistCountLimit = BlockConst.LongDistCountLimit, AverageGapEnabled = false, FixedGapEnabled = true, FixedGapThres = 0.7, FixedGapCountLimit = 3 };
            _kalmanFilter = new KalmanFilter(BlockConst.PredictionDeviation, BlockConst.ObservationDeviation);
            SlideIntoRunway();
        }

        private bool _locked;
        public void SetValue(double value)
        {
            _value_prev = Value;
            if (_glitchRemoving)
            {
                //假如单机无动作，解冻
                if (!BaseConst.OpcDataSource.StayingStill)
                    _locked = false;
                if (_locked)
                    return;

                _value_ori = value;
                PushValueToStorage(ref value, ref _value_ref);
                _value_cur = value;
                if (BlockConst.KalmanFilterEnabled)
                    _kalmanFilter.SetValue(ref value, BaseConst.OpcDataSource.YawSpeed_Plc * Math.PI * _length / 180);
                _value_kal = value;
                value += DistCorr; //距离校正
            }
            Value = value;
            //TODO（仅针对去尖刺的距离计算）计算新计算距离的威胁等级，假如在报警范围内而单机无动作，则不应用新的距离值（锁定，单机有动作后解冻）
            if (_glitchRemoving && SlidedIntoRunway && Level > 0 && BaseConst.OpcDataSource.StayingStill)
                _locked = true;
        }

        /// <summary>
        /// 为数据做好基础准备，将距离值拉入无威胁区
        /// </summary>
        public void SlideIntoRunway()
        {
            if (_glitchFilter == null || _kalmanFilter == null)
                return;
            int maxCount = 50, index = 0;
            while (++index <= maxCount)
            {
                SetValue(_def);
                if (Level == 0)
                    break;
            }
            SlidedIntoRunway = true;
        }

        /// <summary>
        /// 用新的距离值迭代当前距离值，假如更小则替换
        /// </summary>
        /// <param name="other"></param>
        public void Iterate(Distance other)
        {
            if (other == null)
                return;
            if (other.Value < Value)
                SetValue(other.Value);
        }

        /// <summary>
        /// 将新值与旧值的差压入队列中，假如差值绝对值大于5倍的过去差值平均值，则用平均值更新新值
        /// </summary>
        /// <param name="storage">储存差值的队列</param>
        /// <param name="prev">上一次的值</param>
        /// <param name="value">新的值</param>
        private void PushValueToStorage(ref double value, ref double value_ref)
        {
            if (_glitchRemoving && BlockConst.GlitchRemovalEnabled)
            {
                _glitchFilter.PushValue(value);
                value = _glitchFilter.CurrVal;
                value_ref = _glitchFilter.RefVal;
            }
        }

        /// <summary>
        /// 根据给定数字标识获取距离数值，以逗号分隔
        /// </summary>
        /// <param name="type">数字标识：1 原始数值，2 远距过滤数值，3 固定距离过滤数值，4 卡尔曼过滤数值</param>
        /// <returns></returns>
        public string GetValueString(int type)
        {
            switch (type)
            {
                case 1:
                    return string.Format("{0:f2}", _value_ori);
                case 2:
                    return string.Format("{0:f2}", _value_ref);
                case 3:
                    return string.Format("{0:f2}", _value_cur);
                case 4:
                    return string.Format("{0:f2}", _value_kal);
                default:
                    return "-1";
            }
        }
    }
}
