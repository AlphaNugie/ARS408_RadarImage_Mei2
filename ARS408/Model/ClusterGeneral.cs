using ARS408.Core;
using CommonLib.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Model
{
    /// <summary>
    /// 集群基本信息实体类
    /// </summary>
    public class ClusterGeneral : SensorGeneral
    {
        private FalseAlarmProbability pdh = new FalseAlarmProbability();
        private AmbigState ambig_state = new AmbigState();
        private InvalidState invalid_state = new InvalidState();

        #region 属性
        /// <summary>
        /// 集群的虚警概率（错误报警）
        /// </summary>
        public FalseAlarmProbability Pdh0
        {
            get { return this.pdh; }
            set
            {
                this.pdh = value;
                this.PdhString = this.pdh.GetDescription();
            }
        }

        /// <summary>
        /// 错报概率字符串
        /// </summary>
        public string PdhString { get; set; }

        /// <summary>
        /// 多普勒（径向速度）不确定的状态
        /// </summary>
        public AmbigState AmbigState
        {
            get { return this.ambig_state; }
            set
            {
                this.ambig_state = value;
                this.AmbigStateString = this.ambig_state.GetDescription();
            }
        }

        /// <summary>
        /// 径向速度不确定的状态字符串
        /// </summary>
        public string AmbigStateString { get; set; }

        /// <summary>
        /// 集群的有效状态
        /// </summary>
        public InvalidState InvalidState
        {
            get { return this.invalid_state; }
            set
            {
                this.invalid_state = value;
                this.InvalidStateString = this.invalid_state.GetDescription();
            }
        }

        /// <summary>
        /// 有效状态字符串
        /// </summary>
        public string InvalidStateString { get; set; }
        #endregion

        /// <summary>
        /// 基础信息初始化
        /// </summary>
        /// <param name="message">基础信息</param>
        /// <param name="radar">雷达信息</param>
        public ClusterGeneral(BaseMessage message, Radar radar) : base(message, radar) { }

        /// <summary>
        /// 基础信息初始化
        /// </summary>
        /// <param name="message">基础信息</param>
        public ClusterGeneral(BaseMessage message) : base(message, null) { }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public ClusterGeneral() : base(null, null) { }

        /// <summary>
        /// 转换2进制数据
        /// </summary>
        /// <param name="binary"></param>
        protected override void DataConvert(string binary)
        {
            try
            {
                this.Id = Convert.ToByte(binary.Substring(0, 8), 2);
                this.DistLong = Math.Round(0.2 * Convert.ToUInt16(binary.Substring(8, 13), 2) - 500, 1);
                this.DistLat = Math.Round(0.2 * Convert.ToUInt16(binary.Substring(22, 10), 2) - 102.3, 1);
                this.VrelLong = Math.Round(0.25 * Convert.ToUInt16(binary.Substring(32, 10), 2) - 128, 2);
                this.VrelLat = Math.Round(0.25 * Convert.ToUInt16(binary.Substring(42, 9), 2) - 64, 2);
                this.DynProp = (DynProp)Convert.ToByte(binary.Substring(53, 3), 2);
                this.RCS = 0.5 * Convert.ToUInt16(binary.Substring(56, 8), 2) - 64;
            }
            catch (Exception) { }
        }
    }
}
