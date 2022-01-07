using ARS408.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Core
{
    public class OpcDataSource
    {
        /// <summary>
        /// 
        /// </summary>
        public Radar Radar { get; set; }

        /// <summary>
        /// 行走位置
        /// </summary>
        public double WalkingPosition { get; set; }

        /// <summary>
        /// 俯仰角度
        /// </summary>
        public double PitchAngle { get; set; }

        /// <summary>
        /// 回转角度
        /// </summary>
        public double YawAngle { get; set; }

        /// <summary>
        /// 行走位置左（PLC）
        /// </summary>
        public double WalkingLeft_Plc { get; set; }

        /// <summary>
        /// 行走位置右（PLC）
        /// </summary>
        public double WalkingRight_Plc { get; set; }

        private double _walkSpeedPlc = 0;
        /// <summary>
        /// 回转角速度（PLC）
        /// </summary>
        public double WalkingSpeed_Plc
        {
            get { return _walkSpeedPlc; }
            set
            {
                _walkSpeedPlc = value / 15000 * 0.5; //变频器最大值为15000，对应最大走行速度为0.5米/秒（30米/分钟）
                //假如走行速度为0则停止，或假如回转角绝对值过小，则无所谓向左或向右
                if (_walkSpeedPlc == 0 || Math.Abs(YawAngle_Plc) <= 10)
                    WalkDirection = Directions.None;
                //假如走行速度与回转角度同符号，则在向大臂左侧运动，否则在向大臂右侧运动
                else
                    WalkDirection = _walkSpeedPlc * YawAngle_Plc > 0 ? Directions.Left : Directions.Right;
            }
        }

        /// <summary>
        /// 俯仰角度（PLC）
        /// </summary>
        public double PitchAngle_Plc { get; set; }

        /// <summary>
        /// 回转角度（PLC）
        /// </summary>
        public double YawAngle_Plc { get; set; }

        private double _yawSpeedPlc = 0;
        /// <summary>
        /// 回转角速度（PLC）
        /// </summary>
        public double YawSpeed_Plc
        {
            get { return _yawSpeedPlc; }
            set
            {
                _yawSpeedPlc = value / 14500 * 0.13 * 360 / 60; //变频器最大值为14500，对应最大转速为0.13rpm，转换为°/s
                if (_yawSpeedPlc == 0)
                    YawDirection = Directions.None;
                else
                    YawDirection = _yawSpeedPlc < 0 ? Directions.Left : Directions.Right;
            }
        }

        /// <summary>
        /// PLC内垛高（距地面高度）
        /// </summary>
        public double PileHeight_Plc { get; set; }

        /// <summary>
        /// 是否位于底层
        /// </summary>
        public bool OnBottomLevel { get { return (BaseConst.BottomLevelType == BottomLevelType.PitchAngle && PitchAngle_Plc < BaseConst.BottomLevelPitchAngle) || (BaseConst.BottomLevelType == BottomLevelType.PileHeight && PileHeight_Plc < BaseConst.BottomLevelPileHeight); } }
        //public bool OnBottomLevel { get { return PitchAngle_Plc < -8; } }
        //public static bool OnBottomLevel { get { return PileHeight_Plc < 3; } }

        #region 姿态运动标志
        private int _walkBack, _walkFixed, _walkFor;
        /// <summary>
        /// 走行向后
        /// </summary>
        public int WalkBackward
        {
            get { return _walkBack; }
            set
            {
                _walkBack = value;
                //if (_walkBack != 1)
                //    return;
                //假如既没有向前走也没有向后走，或回转角绝对值过小，则无所谓向左或向右
                if ((_walkBack == 0 && _walkFor == 0) || Math.Abs(YawAngle_Plc) <= 10)
                    WalkDirection = Directions.None;
                //当行走向后时，回转角小于0相当于向左侧靠近，否则相当于向右侧靠近
                else if (_walkBack == 1)
                    WalkDirection = YawAngle_Plc < 0 ? Directions.Left : Directions.Right;
            }
        }

        /// <summary>
        /// 走行停止
        /// </summary>
        public int WalkFixated
        {
            get { return _walkFixed; }
            set
            {
                _walkFixed = value;
                if (_walkFixed == 1)
                    WalkDirection = Directions.None;
            }
        }

        /// <summary>
        /// 走行向前
        /// </summary>
        public int WalkForward
        {
            get { return _walkFor; }
            set
            {
                _walkFor = value;
                //if (_walkFor != 1)
                //    return;
                //假如回转角绝对值过小，则无所谓向左或向右
                if ((_walkBack == 0 && _walkFor == 0) || Math.Abs(YawAngle_Plc) <= 10)
                    WalkDirection = Directions.None;
                //当行走向前时，回转角小于0相当于向左侧靠近，否则相当于向右侧靠近
                else if (_walkFor == 1)
                    WalkDirection = YawAngle_Plc > 0 ? Directions.Left : Directions.Right;
            }
        }

        private int _pitchDown, _pitchFixed, _pitchUp;
        /// <summary>
        /// 俯仰向下
        /// </summary>
        public int PitchDownward
        {
            get { return _pitchDown; }
            set
            {
                _pitchDown = value;
                if (_pitchDown == 1)
                    PitchDirection = Directions.Down;
                else if (_pitchDown == 0 && _pitchUp == 0)
                    PitchDirection = Directions.None;
            }
        }

        /// <summary>
        /// 俯仰停止
        /// </summary>
        public int PitchFixated
        {
            get { return _pitchFixed; }
            set
            {
                _pitchFixed = value;
                if (_pitchFixed == 1)
                    PitchDirection = Directions.None;
            }
        }

        /// <summary>
        /// 俯仰向上
        /// </summary>
        public int PitchUpward
        {
            get { return _pitchUp; }
            set
            {
                _pitchUp = value;
                if (_pitchUp == 1)
                    PitchDirection = Directions.Up;
                else if (_pitchDown == 0 && _pitchUp == 0)
                    PitchDirection = Directions.None;
            }
        }

        private int _yawLeft, _yawFixed, _yawRight;
        /// <summary>
        /// 回转向左
        /// </summary>
        public int YawLeft
        {
            get { return _yawLeft; }
            set
            {
                _yawLeft = value;
                if (_yawLeft == 1)
                    YawDirection = Directions.Left;
                else if (_yawLeft == 0 && _yawRight == 0)
                    YawDirection = Directions.None;
            }
        }

        /// <summary>
        /// 回转停止
        /// </summary>
        public int YawFixated
        {
            get { return _yawFixed; }
            set
            {
                _yawFixed = value;
                if (_yawFixed == 1)
                    YawDirection = Directions.None;
            }
        }

        /// <summary>
        /// 回转向右
        /// </summary>
        public int YawRight
        {
            get { return _yawRight; }
            set
            {
                _yawRight = value;
                if (_yawRight == 1)
                    YawDirection = Directions.Right;
                else if (_yawLeft == 0 && _yawRight == 0)
                    YawDirection = Directions.None;
            }
        }
        #endregion

        #region 姿态运动标志布尔量
        /// <summary>
        /// 走行向后
        /// </summary>
        public bool WalkBackwardBool
        {
            get { return WalkBackward == 1; }
            set { WalkBackward = value ? 1 : 0; }
        }

        /// <summary>
        /// 走行停止
        /// </summary>
        public bool WalkFixatedBool
        {
            get { return WalkFixated == 1; }
            set { WalkFixated = value ? 1 : 0; }
        }

        /// <summary>
        /// 走行向前
        /// </summary>
        public bool WalkForwardBool
        {
            get { return WalkForward == 1; }
            set { WalkForward = value ? 1 : 0; }
        }

        /// <summary>
        /// 俯仰向下
        /// </summary>
        public bool PitchDownwardBool
        {
            get { return PitchDownward == 1; }
            set { PitchDownward = value ? 1 : 0; }
        }

        /// <summary>
        /// 俯仰停止
        /// </summary>
        public bool PitchFixatedBool
        {
            get { return PitchFixated == 1; }
            set { PitchFixated = value ? 1 : 0; }
        }

        /// <summary>
        /// 俯仰向上
        /// </summary>
        public bool PitchUpwardBool
        {
            get { return PitchUpward == 1; }
            set { PitchUpward = value ? 1 : 0; }
        }

        /// <summary>
        /// 回转向左
        /// </summary>
        public bool YawLeftBool
        {
            get { return YawLeft == 1; }
            set { YawLeft = value ? 1 : 0; }
        }

        /// <summary>
        /// 回转停止
        /// </summary>
        public bool YawFixatedBool
        {
            get { return YawFixated == 1; }
            set { YawFixated = value ? 1 : 0; }
        }

        /// <summary>
        /// 回转向右
        /// </summary>
        public bool YawRightBool
        {
            get { return YawRight == 1; }
            set { YawRight = value ? 1 : 0; }
        }
        #endregion

        #region 方向
        /// <summary>
        /// 单机是否保持静止（行走、俯仰、回转均无动作）
        /// </summary>
        public bool StayingStill { get { return WalkDirection == Directions.None && PitchDirection == Directions.None && YawDirection == Directions.None; } }

        /// <summary>
        /// 大臂运动方向
        /// </summary>
        public Directions MovingDirection { get; set; }

        private Directions _walkDir;
        /// <summary>
        /// 行走方向，由行走位置与回转角同时决定
        /// </summary>
        public Directions WalkDirection
        {
            get { return _walkDir; }
            set
            {
                _walkDir = value;
                MovingDirection = BaseFunc.GetMovingDirection(_walkDir, _pitchDir, _yawDir);
            }
        }

        private Directions _pitchDir;
        /// <summary>
        /// 俯仰方向
        /// </summary>
        public Directions PitchDirection
        {
            get { return _pitchDir; }
            set
            {
                _pitchDir = value;
                MovingDirection = BaseFunc.GetMovingDirection(_walkDir, _pitchDir, _yawDir);
            }
        }

        private Directions _yawDir;
        /// <summary>
        /// 回转方向
        /// </summary>
        public Directions YawDirection
        {
            get { return _yawDir; }
            set
            {
                _yawDir = value;
                MovingDirection = BaseFunc.GetMovingDirection(_walkDir, _pitchDir, _yawDir);
            }
        }
        #endregion

        public OpcDataSource()
        {
            PileHeight_Plc = 50;
        }
    }
}
