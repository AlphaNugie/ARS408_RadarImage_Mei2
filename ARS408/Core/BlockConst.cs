using ARS408.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Core
{
    /// <summary>
    /// 网格相关变量
    /// </summary>
    public static class BlockConst
    {
        #region 网格
        /// <summary>
        /// 左上角XY坐标，单位米
        /// </summary>
        public static double[] UpLeftCorner { get; set; }

        /// <summary>
        /// 整块区域的尺寸：宽(X)与高(Y)，单位米
        /// </summary>
        public static double[] AreaSize { get; set; }

        /// <summary>
        /// 网格矩阵尺寸（数量），X坐标方向(宽)划分数量 * Y坐标方向(高)划分数量
        /// </summary>
        public static int[] MatrixSize { get; set; }

        /// <summary>
        /// 网格单元的尺寸：宽(X)与高(Y)，单位米
        /// </summary>
        public static double[] UnitSize { get; set; }

        private static int _proc_intern = 50;
        /// <summary>
        /// 循环处理的循环间隔（毫秒）
        /// </summary>
        public static int ProcessInternal
        {
            get { return _proc_intern; }
            set
            {
                if (value > 0)
                    _proc_intern = value;
            }
        }

        /// <summary>
        /// 网格测距默认初始值
        /// </summary>
        public static double DefaultDistance { get; set; }

        /// <summary>
        /// 网格聚类在XY方向的半径长度（向四周扩展的网格单元数量，不包括核心网格，默认为2x2）
        /// </summary>
        public static int[] BlockClusterRadius { get; set; }

        /// <summary>
        /// 网格聚类的低阈值、高阈值，大于等于高阈值为核心网格，大于等于低阈值为向核心网格聚类的普通网格，小于低阈值视为无效网格
        /// </summary>
        public static double[] ClusteringThresholds { get; set; }

        /// <summary>
        /// 判断是否为地面杂波、小煤堆的网格聚类阈值（面积、RCS），面积与RCS均小于阈值时为地面杂波、小煤堆
        /// </summary>
        public static double[] ClutterThresholds { get; set; }

        /// <summary>
        /// 根据网格聚类形心角度判断是否为坝基时，形心角度与回转角度间允许相差的最大值（绝对值）
        /// </summary>
        public static double FoundAngleMarg { get; set; }

        /// <summary>
        /// 针对网格聚类形心坐标，大臂在X方向的最大值（绝对值）
        /// </summary>
        public static double MainArmScopeX { get; set; }

        /// <summary>
        /// 划分网格防碰区域前中后部分的Y轴坐标值分界点（向下取整，假定回转轴Y坐标为0，从斗轮中心往后分别是5米，10米，N-15米（假如大臂长N米））
        /// </summary>
        public static double[] FieldBorders { get; set; }

        /// <summary>
        /// 网格单元组成的二维数组
        /// </summary>
        public static BlockUnit[,] Blocks { get; set; }

        /// <summary>
        /// 一般网格单元列表，符合第一阈值
        /// </summary>
        public static List<BlockUnit> CommonBlocks { get; set; }

        /// <summary>
        /// 网格聚类对象列表
        /// </summary>
        public static List<BlockCluster> BlockClusters { get; set; }

        #region 距离
        //private static readonly Distances _distLeft = new Distances(Directions.Left, true);
        /// <summary>
        /// 左侧前中后距离
        /// </summary>
        public static Distances DistancesLeft { get; set; }

        //private static readonly Distances _distRight = new Distances(Directions.Right, true);
        /// <summary>
        /// 右侧前中后距离
        /// </summary>
        public static Distances DistancesRight { get; set; }

        /// <summary>
        /// 左侧距离校正值，分别为前，中，后
        /// </summary>
        public static double[] DistCorrLeft { get; set; }

        /// <summary>
        /// 右侧距离校正值，分别为前，中，后
        /// </summary>
        public static double[] DistCorrRight { get; set; }

        /// <summary>
        /// 左前距离
        /// </summary>
        public static double DistLeftFront { get { return DistancesLeft == null ? DefaultDistance : DistancesLeft.Front.Value; } }

        /// <summary>
        /// 左中距离
        /// </summary>
        public static double DistLeftMiddle { get { return DistancesLeft == null ? DefaultDistance : DistancesLeft.Middle.Value; } }

        /// <summary>
        /// 左后距离
        /// </summary>
        public static double DistLeftBack { get { return DistancesLeft == null ? DefaultDistance : DistancesLeft.Back.Value; } }

        /// <summary>
        /// 右前距离
        /// </summary>
        public static double DistRightFront { get { return DistancesRight == null ? DefaultDistance : DistancesRight.Front.Value; } }

        /// <summary>
        /// 右中距离
        /// </summary>
        public static double DistRightMiddle { get { return DistancesRight == null ? DefaultDistance : DistancesRight.Middle.Value; } }

        /// <summary>
        /// 右后距离
        /// </summary>
        public static double DistRightBack { get { return DistancesRight == null ? DefaultDistance : DistancesRight.Back.Value; } }
        #endregion

        #region 级别
        /// <summary>
        /// 左前距离级别
        /// </summary>
        public static double LevelLeftFront { get { return DistancesLeft == null ? BaseFunc.GetThreatLevelByValue(DefaultDistance) : DistancesLeft.Front.Level; } }

        /// <summary>
        /// 左中距离级别
        /// </summary>
        public static double LevelLeftMiddle { get { return DistancesLeft == null ? BaseFunc.GetThreatLevelByValue(DefaultDistance) : DistancesLeft.Middle.Level; } }

        /// <summary>
        /// 左后距离级别
        /// </summary>
        public static double LevelLeftBack { get { return DistancesLeft == null ? BaseFunc.GetThreatLevelByValue(DefaultDistance) : DistancesLeft.Back.Level; } }

        /// <summary>
        /// 右前距离级别
        /// </summary>
        public static double LevelRightFront { get { return DistancesRight == null ? BaseFunc.GetThreatLevelByValue(DefaultDistance) : DistancesRight.Front.Level; } }

        /// <summary>
        /// 右中距离级别
        /// </summary>
        public static double LevelRightMiddle { get { return DistancesRight == null ? BaseFunc.GetThreatLevelByValue(DefaultDistance) : DistancesRight.Middle.Level; } }

        /// <summary>
        /// 右后距离级别
        /// </summary>
        public static double LevelRightBack { get { return DistancesRight == null ? BaseFunc.GetThreatLevelByValue(DefaultDistance) : DistancesRight.Back.Level; } }
        #endregion

        #region 滤波
        /// <summary>
        /// 是否移除距离值毛刺（数据突变）
        /// </summary>
        public static bool GlitchRemovalEnabled { get; set; }

        /// <summary>
        /// 测距默认值检定次数限定值（超过此值则以默认值替代当前值）
        /// </summary>
        public static int LongDistCountLimit { get; set; }

        /// <summary>
        /// 达到固定差距检定次数限定值（超过此次数则以初步处理值替代当前值）
        /// </summary>
        public static int FixedGapCountLimit { get; set; }

        /// <summary>
        /// （针对距离值）卡尔曼滤波是否可用
        /// </summary>
        public static bool KalmanFilterEnabled { get; set; }

        /// <summary>
        /// 卡尔曼滤波的预测值偏差度
        /// </summary>
        public static double PredictionDeviation { get; set; }

        /// <summary>
        /// 卡尔曼滤波的观测值偏差度
        /// </summary>
        public static double ObservationDeviation { get; set; }
        #endregion
        #endregion
    }
}
