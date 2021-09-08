using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Model
{
    /// <summary>
    /// OPC项对象
    /// </summary>
    public class OpcItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public int RecordId { get; set; }

        /// <summary>
        /// OPC项名称
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// OPC组的ID
        /// </summary>
        public int OpcGroupId { get; set; }

        /// <summary>
        /// 对应数据源类的字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public OpcItem() { }

        /// <summary>
        /// 构造器，从公共变量获取属性值，再用给定的DataRow对象覆盖各属性的值
        /// </summary>
        /// <param name="row"></param>
        public OpcItem(DataRow row) : this()
        {
            if (row == null)
                return;

            RecordId = int.Parse(row["record_id"].ToString());
            ItemId = row["item_id"].ToString();
            OpcGroupId = int.Parse(row["opcgroup_id"].ToString());
            FieldName = row["field_name"].ToString();
            Enabled = row["enabled"].ToString().Equals("1");
        }
    }
}
