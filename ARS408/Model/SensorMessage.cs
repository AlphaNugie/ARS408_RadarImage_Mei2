using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Model
{
    /// <summary>
    /// 传感器消息基础类
    /// </summary>
    public abstract class SensorMessage
    {
        private BaseMessage _base = new BaseMessage();

        /// <summary>
        /// 基础信息
        /// </summary>
        public BaseMessage Base
        {
            get { return this._base; }
            set
            {
                this._base = value;
                if (this._base != null)
                    this.DataConvert(this._base.DataString_Binary);
            }
        }

        //public abstract SensorMessage Copy();

        /// <summary>
        /// 将从雷达收到的二进制数据转换为实际信息
        /// </summary>
        /// <param name="binary"></param>
        protected abstract void DataConvert(string binary);
    }
}
