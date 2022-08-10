using CommonLib.DataUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Core
{
    /// <summary>
    /// 雷达组类型SQLITE操作类
    /// </summary>
    public class DataService_GroupType : BaseDataServiceSqlite
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public DataService_GroupType() : base(BaseConst.SqliteFileDir, BaseConst.SqliteFileName) { }

        protected override void SetTableName()
        {
            TableName = "t_base_grouptype";
        }

        protected override void AddMustHaveColumns() { }

        /// <summary>
        /// 获取所有组类型，按ID排序
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllGroupTypesOrderbyId()
        {
            return GetAllGroupTypes("group_code");
        }

        /// <summary>
        /// 获取所有组类型，按名称排序
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllGroupTypesOrderbyName()
        {
            return GetAllGroupTypes("group_name");
        }

        /// <summary>
        /// 获取所有组类型，并按特定字段排序
        /// </summary>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetAllGroupTypes(string orderby)
        {
            string sql = "select t.*, 0 changed from t_base_grouptype t " + (string.IsNullOrWhiteSpace(orderby) ? string.Empty : "order by t." + orderby);
            return Provider.Query(sql);
        }
    }
}
