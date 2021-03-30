using ARS408.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Model
{
    /// <summary>
    /// 防碰雷达坐标网格单元
    /// </summary>
    public class BlockUnit
    {
        /// <summary>
        /// 根据根据网格单元中心的X坐标排序
        /// </summary>
        public static Comparison<BlockUnit> CenterXAbsComparison = (a, b) => Math.Abs(a.CenterX).CompareTo(Math.Abs(b.CenterX));

        private double _width = 0;
        /// <summary>
        /// 宽（X方向）
        /// </summary>
        public double Width
        {
            get { return _width; }
            private set
            {
                _width = value;
                CenterX = _up_left_x + _width / 2;
            }
        }

        private double _height = 0;
        /// <summary>
        /// 高（Y方向）
        /// </summary>
        public double Height
        {
            get { return _height; }
            private set
            {
                _height = value;
                CenterY = _up_left_y - _height / 2;
            }
        }

        /// <summary>
        /// 网格单元面积
        /// </summary>
        public double Area { get { return _width * _height; } }

        private double _up_left_x = 0;
        /// <summary>
        /// 网格左上角X坐标
        /// </summary>
        public double UpLeftCornerX
        {
            get { return _up_left_x; }
            private set
            {
                _up_left_x = value;
                CenterX = _up_left_x + _width / 2;
            }
        }

        private double _up_left_y = 0;
        /// <summary>
        /// 网格左上角Y坐标
        /// </summary>
        public double UpLeftCornerY
        {
            get { return _up_left_y; }
            private set
            {
                _up_left_y = value;
                CenterY = _up_left_y - _height / 2;
            }
        }

        /// <summary>
        /// 网格中心X坐标
        /// </summary>
        public double CenterX { get; private set; }

        /// <summary>
        /// 网格中心Y坐标
        /// </summary>
        public double CenterY { get; private set; }

        private int _column_index = 0;
        /// <summary>
        /// 网格在整个网格矩阵中的列索引（X方向排序）
        /// </summary>
        public int ColumnIndex
        {
            get { return _column_index; }
            private set
            {
                _column_index = value;
                IndexPrint = string.Format("{0},{1}", _column_index, _row_index);
            }
        }

        private int _row_index = 0;
        /// <summary>
        /// 网格在整个网格矩阵中的行索引（Y方向排序）
        /// </summary>
        public int RowIndex
        {
            get { return _row_index; }
            private set
            {
                _row_index = value;
                IndexPrint = string.Format("{0},{1}", _column_index, _row_index);
            }
        }

        /// <summary>
        /// 根据行索引与列索引得到的唯一标志
        /// </summary>
        public string IndexPrint { get; private set; }

        /// <summary>
        /// 网格重量，含义为网格内点的数量
        /// </summary>
        public int Weight { get { return ListGeneral.Count; } }

        private BlockType _type = BlockType.Invalid;
        /// <summary>
        /// 网格类型
        /// </summary>
        public BlockType Type
        {
            get { return _type; }
            private set
            {
                TypeChanged = _type != value;
                _type = value;
            }
        }

        /// <summary>
        /// 网格类型是否改变
        /// </summary>
        public bool TypeChanged { get; private set; }

        /// <summary>
        /// 距离除自身之外最近的若干个网格单元的距离的平均值
        /// </summary>
        public double AverageDistance { get; set; }

        /// <summary>
        /// 网格单元在整片网格区域中的位置
        /// </summary>
        public BlockPosition Position { get; set; }

        /// <summary>
        /// 获取网格单元内所有一般消息的RCS和
        /// </summary>
        public double RcsSum { get { return ListGeneral.Select(g => g.RCS).Sum(); } }
        
        /// <summary>
        /// 网格单元所属的核心网格，仅当当前网格为普通网格时有效
        /// </summary>
        public BlockUnit CoreBlock { get; set; }

        /// <summary>
        /// 传感器一般消息的列表
        /// </summary>
        public List<SensorGeneral> ListGeneral { get; private set; }

        private double _dist = 0;
        /// <summary>
        /// 当前网格内点的最近距离，保留2位小数
        /// </summary>
        public double Distance
        {
            get { return _dist; }
            private set { _dist = Math.Round(value, 2); }
        }

        /// <summary>
        /// 以列索引、行索引、宽高作为基础数据初始化网格单元对象
        /// </summary>
        /// <param name="columnIndex">列索引</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="width">网格单元宽度（米）</param>
        /// <param name="height">网格单元高度（米）</param>
        public BlockUnit(int columnIndex, int rowIndex, double width, double height)
        {
            Distance = BlockConst.DefaultDistance;
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            Width = width;
            Height = height;
            UpLeftCornerX = BlockConst.UpLeftCorner[0] + _column_index * _width;
            UpLeftCornerY = BlockConst.UpLeftCorner[1] - _row_index * _height;
            Position = BlockPosition.Invalid;
            ListGeneral = new List<SensorGeneral>();
        }

        /// <summary>
        /// 根据行列索引与另外一个网格单元对象进行比较
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool Equals(BlockUnit block)
        {
            return block == null ? false : IndexPrint.Equals(block.IndexPrint);
        }

        /// <summary>
        /// 计算距离另一个网格单元的距离（以网格单元的中心为标准）
        /// </summary>
        /// <param name="block">另一个网格单元对象</param>
        /// <returns></returns>
        public double DistanceTo(BlockUnit block)
        {
            return block == null ? double.MaxValue : Math.Sqrt(Math.Pow(block.CenterX - CenterX, 2) + Math.Pow(block.CenterY - CenterY, 2));
        }

        /// <summary>
        /// 添加传感器一般消息
        /// </summary>
        /// <param name="general"></param>
        public void AddSensorGeneral(SensorGeneral general)
        {
            if (general == null)
                return;
            ListGeneral.Add(general);
            //刷新最近距离
            if (general.DistanceToBorder < Distance && general.DistanceToBorder > 0)
                Distance = general.DistanceToBorder;
            UpdateBlockType();
        }

        /// <summary>
        /// 根据网格重量更新网格类型
        /// </summary>
        public void UpdateBlockType()
        {
            if (Weight >= BlockConst.ClusteringThresholds[1])
                Type = BlockType.Core;
            else if (Weight >= BlockConst.ClusteringThresholds[0])
                Type = BlockType.Common;
            else
                Type = BlockType.Invalid;
        }
    }
}
