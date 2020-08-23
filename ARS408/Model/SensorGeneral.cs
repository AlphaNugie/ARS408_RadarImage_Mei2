using ARS408.Core;
using CommonLib.Extensions;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Model
{
    /// <summary>
    /// 传感器消息一般消息基础类
    /// </summary>
    [ProtoContract]
    [ProtoInclude(1, typeof(ClusterGeneral))]
    [ProtoInclude(2, typeof(ObjectGeneral))]
    public abstract class SensorGeneral : SensorMessage
    {
        #region static
        /// <summary>
        /// 根据角度、距检测区的最短距离排序
        /// </summary>
        public static Comparison<SensorGeneral> AngleDistComparison = (a, b) =>
        {
            //bool close_enough = Math.Floor(Math.Abs(a.Angle - b.Angle)) == 0; //是否两个角度足够接近（相差小于1°）
            //int result = a.AngleVert.CompareTo(b.AngleVert); //比较角度与俯仰角的差的绝对值的大小
            ////假如上文中绝对值不同且两个角度并不足够接近，直接返回绝对值比较结果，否则返回距离比较结果
            //return result != 0 && !close_enough ? result : a.DistanceToBorder.CompareTo(b.DistanceToBorder);
            int result = a.AngleLevel.CompareTo(b.AngleLevel); //比较角度与俯仰角的差的绝对值的层次
            //假如上文中绝对值不同且两个角度并不足够接近，直接返回绝对值比较结果，否则返回距离比较结果
            return result != 0 ? result : a.DistanceToBorder.CompareTo(b.DistanceToBorder);
        };

        /// <summary>
        /// 根据距检测区的最短距离排序
        /// </summary>
        public static Comparison<SensorGeneral> DistanceComparison = (a, b) => a.DistanceToBorder.CompareTo(b.DistanceToBorder);

        /// <summary>
        /// 根据转换后Z坐标的大小排序
        /// </summary>
        //public static Comparison<SensorGeneral> HeightComparison = (a, b) => a.ModiCoors.Z.CompareTo(b.ModiCoors.Z);
        public static Comparison<SensorGeneral> HeightComparison = (a, b) => a.Z.CompareTo(b.Z);
        #endregion

        #region 属性
        /// <summary>
        /// 所在帧累积未清除的结算(push finalization)次数
        /// </summary>
        public int PushfCounter { get; set; }

        /// <summary>
        /// 消息ID（编号）
        /// </summary>
        public int Id { get; set; }

        private double _dist_long;
        /// <summary>
        /// 纵向（x）坐标，米
        /// </summary>
        [ProtoMember(3)]
        public double DistLong
        {
            get { return this._dist_long; }
            set
            {
                this._dist_long = value;
                this.CalculateConvertedCoors();
                this.CalculateAngle();
                this.CheckIfWithinLimits();
            }
        }

        private double _dist_lat;
        /// <summary>
        /// 横向（y）坐标，米
        /// </summary>
        [ProtoMember(4)]
        public double DistLat
        {
            get { return this._dist_lat; }
            set
            {
                this._dist_lat = value;
                this.CalculateConvertedCoors();
                this.CalculateAngle();
                this.CheckIfWithinLimits();
            }
        }

        /// <summary>
        /// 角度
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// 当前点在单机YOZ平面内的角度，自Y轴正向转向Z轴正向为正，反之为负
        /// </summary>
        public double AngleYoz { get; set; }

        /// <summary>
        /// 与竖直向下的角度的差值
        /// </summary>
        public double AngleVert { get { return Math.Abs(this.Angle - OpcConst.PitchAngle); } }

        /// <summary>
        /// 与竖直向下的角度的差值的层次（越靠近竖直角度层次越小，每个层次差1°）
        /// </summary>
        public double AngleLevel { get { return Math.Floor(Math.Abs(this.Angle - OpcConst.PitchAngle) / 1); } }

        /// <summary>
        /// 纵向的相对速度（x），米/秒
        /// </summary>
        public double VrelLong { get; set; }

        /// <summary>
        /// 横向的相对速度（y），米/秒
        /// </summary>
        public double VrelLat { get; set; }

        /// <summary>
        /// 转换后的最原始坐标
        /// </summary>
        public ModifiedCoordinates ModiCoors { get; set; }

        /// <summary>
        /// 转换后的X坐标（经过偏移处理）
        /// </summary>
        [ProtoMember(6)]
        public double X
        {
            get { return this.ModiCoors.X + this.Radar.XOffset; }
            set { this.ModiCoors.X = value - this.Radar.XOffset; }
        }

        /// <summary>
        /// 转换后的Y坐标（经过偏移处理）
        /// </summary>
        [ProtoMember(7)]
        public double Y
        {
            get { return this.ModiCoors.Y + this.Radar.YOffset; }
            set { this.ModiCoors.Y = value - this.Radar.YOffset; }
        }

        /// <summary>
        /// 转换后的Z坐标（经过偏移处理）
        /// </summary>
        [ProtoMember(8)]
        public double Z
        {
            get { return this.ModiCoors.Z + this.Radar.ZOffset; }
            set { this.ModiCoors.Z = value - this.Radar.ZOffset; }
        }


        /// <summary>
        /// 是否处于雷达坐标限制范围内
        /// </summary>
        public bool WithinRadarLimits { get; set; }

        /// <summary>
        /// 是否处于单机坐标限制范围内
        /// </summary>
        public bool WithinClaimerLimits { get; set; }

        /// <summary>
        /// 是否位于角度限制范围内
        /// </summary>
        public bool WithinAngleLimits { get; set; }

        private DynProp _prop = new DynProp();
        /// <summary>
        /// 动态属性，指示是否在移动或是否已停止等属性
        /// </summary>
        public DynProp DynProp
        {
            get { return this._prop; }
            set
            {
                this._prop = value;
                this.DynPropString = this._prop.GetDescription();
                this.Color = BaseFunc.GetColorByDynProp(this._prop, this.Color);
            }
        }

        /// <summary>
        /// 动态属性的描述
        /// </summary>
        public string DynPropString { get; set; }

        /// <summary>
        /// 动态属性所对应的颜色
        /// </summary>
        public Color Color { get; set; }

        private double _rcs = 0;
        /// <summary>
        /// 雷达散射截面(Radar Crossing Section)，单位 dBm2（分贝，转换为平米：10^(0.1*dB)，例如，-10分贝为0.1平方米）
        /// </summary>
        [ProtoMember(5)]
        public double RCS
        {
            get { return this._rcs; }
            set
            {
                this._rcs = value;
                this.RCS_M2 = Math.Pow(10, 0.1 * this._rcs);
                this.Color = BaseFunc.GetColorByRcs(this._rcs, this.Color);
            }
        }

        /// <summary>
        /// 雷达散射截面(Radar Crossing Section)，单位 m2（平方米）
        /// </summary>
        public double RCS_M2 { get; set; }

        private Radar _radar = null;
        /// <summary>
        /// 对应雷达信息
        /// </summary>
        public Radar Radar
        {
            get { return this._radar; }
            set
            {
                _radar = value;
                if (_radar == null)
                    _radar = new Radar();
            }
        }

        /// <summary>
        /// 距检测边界的距离，与检测方式（点线面）与雷达朝向（海北陆南）有关
        /// </summary>
        public double DistanceToBorder { get; set; }
        #endregion

        /// <summary>
        /// 基础信息初始化
        /// </summary>
        /// <param name="message">基础信息</param>
        /// <param name="radar">雷达信息</param>
        protected SensorGeneral(BaseMessage message, Radar radar)
        {
            this.Color = Color.FromArgb(255, 255, 255);
            this.ModiCoors = new ModifiedCoordinates();
            this.WithinRadarLimits = true;
            this.WithinClaimerLimits = true;
            this.Radar = radar;
            this.Base = message;
        }

        /// <summary>
        /// 将从雷达收到的二进制数据转换为雷达基础信息
        /// </summary>
        /// <param name="binary"></param>
        protected abstract override void DataConvert(string binary);

        /// <summary>
        /// 根据横纵坐标计算角度
        /// </summary>
        public void CalculateAngle()
        {
            this.Angle = this._dist_long == 0 ? Math.Sign(this._dist_lat) * 90 : Math.Atan(this._dist_lat / this._dist_long) * 180 / Math.PI;
            this.AngleYoz = this.Y == 0 ? Math.Sign(this.Z) * 90 : Math.Atan(this.Z / this.Y) * 180 / Math.PI;
        }

        /// <summary>
        /// 计算转换后坐标
        /// </summary>
        public void CalculateConvertedCoors()
        {
            if (this.Radar != null && this.Radar.Id > 0)
            {
                Directions dir = this.Radar.Direction;
                this.ModiCoors.X = this.Radar.XmodifiedRatios.Xratio * this.DistLong + this.Radar.XmodifiedRatios.Yratio * this.DistLat;
                this.ModiCoors.Y = this.Radar.YmodifiedRatios.Xratio * this.DistLong + this.Radar.YmodifiedRatios.Yratio * this.DistLat;
                this.ModiCoors.Z = this.Radar.ZmodifiedRatios.Xratio * this.DistLong + this.Radar.ZmodifiedRatios.Yratio * this.DistLat;
                bool northsouth = dir == Directions.Left || dir == Directions.Right; //是否朝向北或南
                //double x = northsouth ? this.ModiCoors.X : this.ModiCoors.Y, y = northsouth ? this.ModiCoors.Y : this.ModiCoors.X, z = this.ModiCoors.Z; //根据方向调换X/Y的值
                double x = northsouth ? this.X : this.Y, y = northsouth ? this.Y : this.X, z = this.Z; //根据方向调换X/Y的值
                int m = this.Radar.DefenseMode; //防御模式：1 点，2 线，3 面
                //d = (a*x^2+b*z^2+c*y^2)^0.5，其中a, b, c由4-m, 3-m, 2-m的值决定，假如大于0则为1，小于等于0为0（公式形如Math.Sign(4 - m) == 1 ? 1 : 0）
                //含义：面模式，a=1,b=c=0；线模式，a=b=1,c=0；点模式，a=b=c=1
                //假如方向为上下，则只计算竖直方向Z坐标的值
                this.DistanceToBorder = (dir == Directions.Up || dir == Directions.Down) ? z : Math.Sqrt((Math.Sign(4 - m) == 1 ? 1 : 0) * Math.Pow(x, 2) + (Math.Sign(3 - m) == 1 ? 1 : 0) * Math.Pow(z, 2) + (Math.Sign(2 - m) == 1 ? 1 : 0) * Math.Pow(y, 2));
                this.DistanceToBorder = (dir == Directions.Down ? -1 : 1) * this.DistanceToBorder + this.Radar.Offset; //当方向向下时，在距离前乘以一个值为-1的系数（向下指时Z坐标均为负数）
                //假如防御模式为面，再添加处理步骤：乘以x的符号，效果为使边界距离带符号；假如面向北或陆，则再乘以-1（所面向方向坐标均为负数）
                if (m == 3 && dir != Directions.Up && dir != Directions.Down)
                    this.DistanceToBorder *= Math.Sign(x) * (dir == Directions.Left || dir == Directions.Back ? -1 : 1);
            }
        }

        /// <summary>
        /// 判断纵横坐标、转换后的XYZ坐标与角度是否在给定范围内
        /// </summary>
        public void CheckIfWithinLimits()
        {
            if (this.Radar == null || this.Radar.Id < 0)
                return;

            this.WithinRadarLimits = this._dist_long.Between(this.Radar.RadarxMin, this.Radar.RadarxMax) && this._dist_lat.Between(this.Radar.RadaryMin, this.Radar.RadaryMax);
            //this.WithinClaimerLimits = this.ModiCoors.X.Between(this.Radar.ClaimerxMin, this.Radar.ClaimerxMax) && this.ModiCoors.Y.Between(this.Radar.ClaimeryMin, this.Radar.ClaimeryMax) && this.ModiCoors.Z.Between(this.Radar.ClaimerzMin, this.Radar.ClaimerzMax);
            this.WithinClaimerLimits = this.X.Between(this.Radar.ClaimerxMin, this.Radar.ClaimerxMax) && this.Y.Between(this.Radar.ClaimeryMin, this.Radar.ClaimeryMax) && this.Z.Between(this.Radar.ClaimerzMin, this.Radar.ClaimerzMax);
            this.WithinAngleLimits = this.Angle.Between(this.Radar.AngleMin, this.Radar.AngleMax);
        }

        /// <summary>
        /// 获取定制信息
        /// </summary>
        /// <returns></returns>
        public string GetCustomInfo()
        {
            return !BaseConst.AddingCustomInfo ? string.Empty : string.Format(" {0} {1} {2} {3} {4}", this.Id, this.VrelLong, this.VrelLat, (byte)this.DynProp, this.RCS);
        }
    }

    /// <summary>
    /// 修改后坐标
    /// </summary>
    public class ModifiedCoordinates
    {
        public double X, Y, Z;
    }
}
