using ARS408.Model;
using CommonLib.DataUtil;
using CommonLib.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARS408.Core
{
    public class DataService_OpcItem : BaseDataServiceSqlite
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public DataService_OpcItem() : base(BaseConst.SqliteFileDir, BaseConst.SqliteFileName) { }

        protected override void SetTableName()
        {
            TableName = "t_plc_opcitem";
        }

        protected override void AddMustHaveColumns()
        {
            ColumnsMustHave.Add(new SqliteColumnMapping("coeff", SqliteSqlType.DOUBLE, true, ConflictClause.FAIL, 0));
            ColumnsMustHave.Add(new SqliteColumnMapping("offset", SqliteSqlType.DOUBLE, true, ConflictClause.FAIL, 0));
        }

        #region 查询
        /// <summary>
        /// 获取所有可用的OPC项信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetOpcInfo()
        {
            string sql = "select * from t_plc_opcgroup g left join t_plc_opcitem i on g.group_id = i.opcgroup_id where i.enabled = 1 order by g.group_id, i.record_id";
            return Provider.Query(sql);
        }

        /// <summary>
        /// 获取所有t_plc_opcitem记录
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllOpcItemRecords()
        {
            string sql = "select * from t_plc_opcitem";
            return Provider.Query(sql);
        }

        /// <summary>
        /// 获取所有OPC项，按ID排序
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllOpcItemsOrderbyId()
        {
            return GetAllOpcItems("record_id");
        }

        /// <summary>
        /// 获取所有OPC项，并按特定字段排序
        /// </summary>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetAllOpcItems(string orderby)
        {
            return GetOpcItems(0, orderby);
        }

        /// <summary>
        /// 根据所属OPC组的ID获取所有OPC项，并按特定字段排序
        /// </summary>
        /// <param name="opcgroup_id">OPC组的ID，为0则查询所有</param>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetOpcItems(int opcgroup_id, string orderby)
        {
            string sql = string.Format(@"
select i.*, 0 changed from t_plc_opcitem i
  left join t_plc_opcgroup g on g.group_id = i.opcgroup_id
  where {0} = 0 or g.group_id = {0} {1}", opcgroup_id, string.IsNullOrWhiteSpace(orderby) ? string.Empty : "order by i." + orderby);
            return Provider.Query(sql);
        }
        #endregion

        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public int DeleteOpcItemById(int id)
        {
            string sql = string.Format("delete from t_plc_opcitem where record_id = {0}", id);
            return Provider.ExecuteSql(sql);
        }

        ///// <summary>
        ///// 执行多条SQL语句
        ///// </summary>
        ///// <param name="sqls"></param>
        ///// <returns></returns>
        //public bool ExecuteSqls(IEnumerable<string> sqls)
        //{
        //    return Provider.ExecuteSqlTrans(sqls);
        //}

        /// <summary>
        /// 保存OPC项信息
        /// </summary>
        /// <param name="loader">OPC项对象</param>
        /// <returns></returns>
        public int SaveOpcItem(OpcItem item)
        {
            return Provider.ExecuteSql(GetSqlString(item));
        }

        /// <summary>
        /// 批量保存OPC项信息
        /// </summary>
        /// <param name="items">多个OPC项对象</param>
        /// <returns></returns>
        public bool SaveOpcItems(IEnumerable<OpcItem> items)
        {
            string[] sqls = items == null ? null : items.Select(radar => GetSqlString(radar)).ToArray();
            return Provider.ExecuteSqlTrans(sqls);
        }

        /// <summary>
        /// 获取OPC项SQL字符串
        /// </summary>
        /// <param name="item">OPC项对象</param>
        /// <returns></returns>
        private string GetSqlString(OpcItem item)
        {
            string sql = string.Empty;
            if (item != null)
                sql = string.Format(item.RecordId <= 0 ? "insert into t_plc_opcitem (item_id, opcgroup_id, field_name, enabled, coeff, offset) values ('{1}', {2}, '{3}', {4}, {5}, {6})" : "update t_plc_opcitem set item_id = '{1}', opcgroup_id = {2}, field_name = '{3}', enabled = {4}, coeff = {5}, offset = {6} where record_id = {0}", item.RecordId, item.ItemId, item.OpcGroupId, item.FieldName, item.Enabled ? 1 : 0, item.Coeff, item.Offset);
            return sql;
        }
    }
}
