﻿using ARS408.Model;
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
    /// 雷达SQLITE服务类
    /// </summary>
    public class DataService_Radar
    {
        //private readonly SqliteProvider provider = new SqliteProvider();
        private readonly SqliteProvider provider = new SqliteProvider(string.Empty, "base.db");

        #region 查询
        /// <summary>
        /// 获取所有雷达，按ID排序
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllRadarsOrderbyId()
        {
            return this.GetAllRadars("radar_id");
        }

        /// <summary>
        /// 根据装船机ID获取所有雷达，按ID排序
        /// </summary>
        /// <returns></returns>
        public DataTable GetRadarsOrderbyId(int shiploader_id)
        {
            return this.GetRadars(shiploader_id, "radar_id");
        }

        /// <summary>
        /// 获取所有雷达，按名称排序
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllRadarsOrderbyName()
        {
            return this.GetAllRadars("radar_name");
        }

        /// <summary>
        /// 获取所有雷达，并按特定字段排序
        /// </summary>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetAllRadars(string orderby)
        {
            return this.GetRadars(0, orderby);
        }

        /// <summary>
        /// 根据所属装船机ID获取所有雷达，并按特定字段排序
        /// </summary>
        /// <param name="shiploader_id">装船机ID，为0则查询所有</param>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetRadars(int shiploader_id, string orderby)
        {
            string sql = string.Format(@"
select t.*, g.group_type, s.shiploader_id, s.topic_name, 0 changed from t_base_radar_info t
  left join t_base_radargroup_info g on t.owner_group_id = g.group_id
  left join t_base_shiploader_info s on g.owner_shiploader_id = s.shiploader_id
  where {0} = 0 or s.shiploader_id = {0} {1}", shiploader_id, string.IsNullOrWhiteSpace(orderby) ? string.Empty : "order by t." + orderby);
            return this.provider.Query(sql);
        }

        /// <summary>
        /// 获取所有雷达的状态标签与碰撞标签名称，按ID排序
        /// </summary>
        /// <returns></returns>
        public DataTable GetRadarItemNamesOrderbyId()
        {
            return this.GetRadarItemNames("radar_id");
        }

        /// <summary>
        /// 获取所有雷达的状态标签与碰撞标签名称，并按特定字段排序
        /// </summary>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetRadarItemNames(string orderby)
        {
            string sql = "select t.radar_id, t.radar_name, t.ip_address||':'||t.port address, t.item_name_radar_state, t.item_name_collision_state, t.item_name_collision_state_2, 0 changed from t_base_radar_info t " + (string.IsNullOrWhiteSpace(orderby) ? string.Empty : "order by t." + orderby);
            return this.provider.Query(sql);
        }

        /// <summary>
        /// 获取所有雷达的坐标限制情况，并按ID排序
        /// </summary>
        /// <returns></returns>
        public DataTable GetRadarCoorsLimitations()
        {
            return this.GetRadarCoorsLimitations("radar_id");
        }

        /// <summary>
        /// 获取所有雷达的坐标限制情况，并按特定字段排序
        /// </summary>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetRadarCoorsLimitations(string orderby)
        {
            string sql = "select t.radar_id, t.radar_name, t.ip_address||':'||t.port address, t.radar_coors_limited, t.radar_x_min, t.radar_x_max, t.radar_y_min, t.radar_y_max, t.claimer_coors_limited, t.claimer_x_min, t.claimer_x_max, t.claimer_y_min, t.claimer_y_max, t.claimer_z_min, t.claimer_z_max, t.angle_limited, t.angle_min, t.angle_max, 0 changed from t_base_radar_info t " + (string.IsNullOrWhiteSpace(orderby) ? string.Empty : "order by t." + orderby);
            return this.provider.Query(sql);
        }

        /// <summary>
        /// 获取所有雷达的坐标限制情况，并按ID排序
        /// </summary>
        /// <returns></returns>
        public DataTable GetRadarBehaviors()
        {
            return this.GetRadarBehaviors("radar_id");
        }

        /// <summary>
        /// 获取所有雷达的坐标限制情况，并按特定字段排序
        /// </summary>
        /// <param name="orderby">排序字段，假如为空则不排序</param>
        /// <returns></returns>
        public DataTable GetRadarBehaviors(string orderby)
        {
            string sql = "select t.radar_id, t.radar_name, t.ip_address||':'||t.port address, t.apply_filter, t.use_public_filters, t.apply_iteration, t.pushf_max_count, t.false_alarm_filter, t.ambig_state_filter, t.invalid_state_filter, t.meas_state_filter, t.prob_exist_filter, 0 changed from t_base_radar_info t " + (string.IsNullOrWhiteSpace(orderby) ? string.Empty : "order by t." + orderby);
            return this.provider.Query(sql);
        }
        #endregion

        /// <summary>
        /// 获取雷达SQL字符串
        /// </summary>
        /// <param name="radar">雷达对象</param>
        /// <returns></returns>
        private string GetRadarSqlString(Radar radar)
        {
            string sql = string.Empty;
            if (radar != null)
                sql = string.Format(radar.Id <= 0 ? "insert into t_base_radar_info (radar_name, ip_address, port, owner_group_id, conn_mode_id, using_local, ip_address_local, port_local, degree_xoy, degree_yoz, degree_xoz, degree_general, direction_id, defense_mode_id, offset, x_offset, y_offset, z_offset, remark, rcs_min, rcs_max, radar_height, refresh_interval, apply_filter, apply_iteration) values ('{1}', '{2}', {3}, {4}, {5}, {6}, '{7}', {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, '{19}', {20}, {21}, {22}, {23}, {24}, {25})" : "update t_base_radar_info set radar_name = '{1}', ip_address = '{2}', port = {3}, owner_group_id = {4}, conn_mode_id = {5}, using_local = {6}, ip_address_local = '{7}', port_local = {8}, degree_xoy = {9}, degree_yoz = {10}, degree_xoz = {11}, degree_general = {12}, direction_id = {13}, defense_mode_id = {14}, offset = {15}, x_offset = {16}, y_offset = {17}, z_offset = {18}, remark = '{19}', rcs_min = {20}, rcs_max = {21}, radar_height = {22}, refresh_interval = {23}, apply_filter = {24}, apply_iteration = {25} where radar_id = {0}", radar.Id, radar.Name, radar.IpAddress, radar.Port, radar.OwnerGroupId, (int)radar.ConnectionMode, radar.UsingLocal ? 1 : 0, radar.IpAddressLocal, radar.PortLocal, radar.DegreeXoy, radar.DegreeYoz, radar.DegreeXoz, radar.DegreeGeneral, (int)radar.Direction, radar.DefenseMode, radar.Offset, radar.XOffset, radar.YOffset, radar.ZOffset, radar.Remark, radar.RcsMinimum, radar.RcsMaximum, radar.RadarHeight, radar.RefreshInterval, radar.ApplyFilter ? 1 : 0, radar.ApplyIteration ? 1 : 0);
            return sql;
        }

        /// <summary>
        /// 获取雷达SQL字符串
        /// </summary>
        /// <param name="radar">雷达对象</param>
        /// <returns></returns>
        private string GetRadarSqlString_ItemName(Radar radar)
        {
            string sql = string.Empty;
            if (radar != null)
                sql = string.Format("update t_base_radar_info set item_name_radar_state = '{0}', item_name_collision_state = '{1}', item_name_collision_state_2 = '{2}' where radar_id = {3}", radar.ItemNameRadarState, radar.ItemNameCollisionState, radar.ItemNameCollisionState2, radar.Id);
            return sql;
        }

        /// <summary>
        /// 获取雷达SQL字符串
        /// </summary>
        /// <param name="radar">雷达对象</param>
        /// <returns></returns>
        private string GetRadarSqlString_CoorsLimitations(Radar radar)
        {
            string sql = string.Empty;
            if (radar != null)
                sql = string.Format("update t_base_radar_info set radar_coors_limited = {1}, radar_x_min = {2}, radar_x_max = {3}, radar_y_min = {4}, radar_y_max = {5}, claimer_coors_limited = {6}, claimer_x_min = {7}, claimer_x_max = {8}, claimer_y_min = {9}, claimer_y_max = {10}, claimer_z_min = {11}, claimer_z_max = {12}, angle_limited = {13}, angle_min = {14}, angle_max = {15} where radar_id = {0}", radar.Id, radar.RadarCoorsLimited ? 1 : 0, radar.RadarxMin, radar.RadarxMax, radar.RadaryMin, radar.RadaryMax, radar.ClaimerCoorsLimited ? 1 : 0, radar.ClaimerxMin, radar.ClaimerxMax, radar.ClaimeryMin, radar.ClaimeryMax, radar.ClaimerzMin, radar.ClaimerzMax, radar.AngleLimited ? 1 : 0, radar.AngleMin, radar.AngleMax);
            return sql;
        }

        /// <summary>
        /// 获取雷达SQL字符串
        /// </summary>
        /// <param name="radar">雷达对象</param>
        /// <returns></returns>
        private string GetRadarSqlString_RadarBehaviors(Radar radar)
        {
            string sql = string.Empty;
            if (radar != null)
                sql = string.Format("update t_base_radar_info set apply_filter = {1}, use_public_filters = {2}, apply_iteration = {3}, pushf_max_count = {4}, false_alarm_filter = '{5}', ambig_state_filter = '{6}', invalid_state_filter = '{7}', meas_state_filter = '{8}', prob_exist_filter = '{9}' where radar_id = {0}", radar.Id, radar.ApplyFilter ? 1 : 0, radar.UsePublicFilters ? 1 : 0, radar.ApplyIteration ? 1 : 0, radar.PushfMaxCount, radar.FalseAlarmFilterString, radar.AmbigStateFilterString, radar.InvalidStateFilterString, radar.MeasStateFilterString, radar.ProbOfExistFilterString);
            return sql;
        }

        #region 增删改
        /// <summary>
        /// 根据ID删除雷达
        /// </summary>
        /// <param name="id">雷达ID</param>
        /// <returns></returns>
        public int DeleteRadarById(int id)
        {
            string sql = string.Format("delete from t_base_radar_info where radar_id = {0}", id);
            return this.provider.ExecuteSql(sql);
        }

        /// <summary>
        /// 根据ID更新雷达RCS值范围
        /// </summary>
        /// <param name="rcsMin">RCS最小值</param>
        /// <param name="rcsMax">RCS最大值</param>
        /// <param name="id">雷达ID</param>
        /// <returns></returns>
        public int UpdateRadarRcsRangeById(int rcsMin, int rcsMax, int id)
        {
            string sqlString = string.Format("update t_base_radar_info set rcs_min = {0}, rcs_max = {1} where radar_id = {2}", rcsMin, rcsMax, id);
            return this.provider.ExecuteSql(sqlString);
        }

        /// <summary>
        /// 根据ID更新雷达RCS值最小值
        /// </summary>
        /// <param name="rcsMin">RCS最小值</param>
        /// <param name="id">雷达ID</param>
        /// <returns></returns>
        public int UpdateRadarRcsMinById(int rcsMin, int id)
        {
            string sqlString = string.Format("update t_base_radar_info set rcs_min = {0} where radar_id = {1}", rcsMin, id);
            return this.provider.ExecuteSql(sqlString);
        }

        /// <summary>
        /// 根据ID更新雷达RCS值最大值
        /// </summary>
        /// <param name="rcsMax">RCS最大值</param>
        /// <param name="id">雷达ID</param>
        /// <returns></returns>
        public int UpdateRadarRcsMaxById(int rcsMax, int id)
        {
            string sqlString = string.Format("update t_base_radar_info set rcs_max = {0} where radar_id = {1}", rcsMax, id);
            return this.provider.ExecuteSql(sqlString);
        }

        /// <summary>
        /// 保存雷达信息
        /// </summary>
        /// <param name="loader">雷达对象</param>
        /// <returns></returns>
        public int SaveRadar(Radar radar)
        {
            return this.provider.ExecuteSql(this.GetRadarSqlString(radar));
        }

        /// <summary>
        /// 批量保存雷达信息
        /// </summary>
        /// <param name="radars">多个雷达对象</param>
        /// <returns></returns>
        public bool SaveRadars(IEnumerable<Radar> radars)
        {
            string[] sqls = radars == null ? null : radars.Select(radar => this.GetRadarSqlString(radar)).ToArray();
            return this.provider.ExecuteSqlTrans(sqls);
        }

        /// <summary>
        /// 批量保存雷达信息
        /// </summary>
        /// <param name="radars">多个雷达对象</param>
        /// <returns></returns>
        public bool SaveRadarItemNames(IEnumerable<Radar> radars)
        {
            string[] sqls = radars == null ? null : radars.Select(radar => this.GetRadarSqlString_ItemName(radar)).ToArray();
            return this.provider.ExecuteSqlTrans(sqls);
        }

        /// <summary>
        /// 批量保存雷达坐标点限制信息
        /// </summary>
        /// <param name="radars">多个雷达对象</param>
        /// <returns></returns>
        public bool SaveRadarCoorsLimitations(IEnumerable<Radar> radars)
        {
            string[] sqls = radars == null ? null : radars.Select(radar => this.GetRadarSqlString_CoorsLimitations(radar)).ToArray();
            return this.provider.ExecuteSqlTrans(sqls);
        }

        /// <summary>
        /// 批量保存雷达坐标点限制信息
        /// </summary>
        /// <param name="radars">多个雷达对象</param>
        /// <returns></returns>
        public bool SaveRadarBehaviors(IEnumerable<Radar> radars)
        {
            string[] sqls = radars == null ? null : radars.Select(radar => this.GetRadarSqlString_RadarBehaviors(radar)).ToArray();
            return this.provider.ExecuteSqlTrans(sqls);
        }

        /// <summary>
        /// 插入雷达测距值
        /// </summary>
        /// <param name="radar_id">雷达ID</param>
        /// <param name="dist">距离</param>
        /// <returns></returns>
        public int InsertRadarDistance(int radar_id, string radar_name, double dist)
        {
            string sqlString = string.Format("insert into t_radar_distances_his (radar_id, radar_name, distance) values ({0}, {1}, {2})", radar_id, radar_name, dist);
            return this.provider.ExecuteSql(sqlString);
        }
        #endregion
    }
}
