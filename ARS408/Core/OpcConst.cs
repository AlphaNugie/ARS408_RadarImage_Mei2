using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Core
{
    /// <summary>
    /// OPC数据
    /// </summary>
    public static class OpcConst
    {
        /// <summary>
        /// 行走位置
        /// </summary>
        public static double WalkingPosition { get; set; }

        /// <summary>
        /// 俯仰角度
        /// </summary>
        public static double PitchAngle { get; set; }

        /// <summary>
        /// 回转角度
        /// </summary>
        public static double YawAngle { get; set; }

        /// <summary>
        /// 行走位置左（PLC）
        /// </summary>
        public static double WalkingLeft_Plc { get; set; }

        /// <summary>
        /// 行走位置右（PLC）
        /// </summary>
        public static double WalkingRight_Plc { get; set; }

        /// <summary>
        /// 俯仰角度（PLC）
        /// </summary>
        public static double Pitch_Plc { get; set; }

        /// <summary>
        /// 回转角度（PLC）
        /// </summary>
        public static double Yaw_Plc { get; set; }
    }
}
