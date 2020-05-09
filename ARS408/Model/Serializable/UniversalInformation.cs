using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Model.Serializable
{
    /// <summary>
    /// 用于数据传输的protobuf数据实体类
    /// </summary>
    [ProtoContract]
    public class UniversalInformation
    {
        private double _dist_wheel_left, _dist_wheel_right;

        /// <summary>
        /// 斗轮左侧距离
        /// </summary>
        [ProtoMember(1)]
        public double DistWheelLeft
        {
            get { return this._dist_wheel_left; }
            set
            {
                this._dist_wheel_left = Math.Round(value, 4);
                this.DistWheelAverage = Math.Round((this._dist_wheel_left + this._dist_wheel_right) / 2, 4);
            }
        }

        /// <summary>
        /// 斗轮右侧距离
        /// </summary>
        [ProtoMember(2)]
        public double DistWheelRight
        {
            get { return this._dist_wheel_right; }
            set
            {
                this._dist_wheel_right = Math.Round(value, 3);
                this.DistWheelAverage = Math.Round((this._dist_wheel_left + this._dist_wheel_right) / 2, 4);
            }
        }

        /// <summary>
        /// 斗轮平均距离
        /// </summary>
        [ProtoMember(3)]
        public double DistWheelAverage { get; set; }
    }
}
