using Dapper;
using DotNet_Core_API_SwaggerDemo1.IDAL;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
   
namespace DotNet_Core_API_SwaggerDemo1.DAL
{
    /// <summary>
    /// 数据访问
    /// </summary>
    public class DBHelper<T> : IDBHelper<T> where T : class, new()
    {
        #region field
        /// <summary>
        /// 连接字符串
        /// </summary>
        private MySqlConnection _conn= new MySqlConnection("Data Source=127.0.0.1;database=school;User ID=root;Password=831143;");
        /// <summary>
        /// 日志
        /// </summary>
        static Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Find
        /// <summary>
        /// 根据字段名字段值表名获取Model
        /// </summary>
        /// <param name="TBname">表名</param>
        /// <param name="fieldname">字段名</param>
        /// <param name="value">字段值</param>
        /// <returns>Model</returns>
        public virtual async Task<T> GetModel(string TBname, string fieldname, string value)
        {
            _logger.Info("开始查询数据库（数据访问层）");
            string sql = "select * from " + TBname + " where  " + fieldname + " = '" + value + "'";    //构造sql
            T result = default(T);
            try
            {
                result = await Task.Run(() =>
                {
                    _logger.Info("生成的sql语句：" + sql);
                    return _conn.QueryFirstOrDefault<T>(sql);
                });
            }
            catch (Exception error)
            {
                _logger.Error("数据访问出错（数据访问层）：" + error);
            }
            _logger.Info("结束查询（数据访问层）");
            if (result == null)
            {
                _logger.Error("但是未查询到字段值为：" + value + " 相关的数据（数据访问层）");
            }
            return result;
        }
        #endregion

        #region FindList
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="Tbname">表名</param>
        /// <param name="pageNO">需要查询的页</param>
        /// <param name="pagecount">每页显示的页数</param>
        /// <returns>结果集</returns>
        public virtual async Task<List<T>> GetList(string Tbname, int pageNO = 1, int pagecount = 10)
        {
            _logger.Info("开始批量查询数据（数据访问层）");
            //判断页码
            string sqllimit = "";
            if (pageNO == 0)
            {
                sqllimit = " limit 0," + pagecount;
            }
            else
            {
                sqllimit = " limit " + ((pageNO - 1) * pagecount).ToString() + "," + pagecount;
            }
            string sql = "select * from " + Tbname + sqllimit;
            List<T> resultList = new List<T>(); //结果集
            try
            {
                resultList = await Task.Run(() =>
                {
                    _logger.Info("生成的sql语句：(数据访问层)" + sql);
                    var query = _conn.Query<T>(sql);
                    foreach (var q in query)
                    {
                        resultList.Add(q);
                    }
                    if (resultList == null)
                    {
                        _logger.Error("查询完成，但是没有查询到第 " + pageNO + " 页数据（数据访问层）");
                    }
                    else
                    {
                        _logger.Info("查询完成（数据访问层）");
                    }
                    return resultList;
                });
            }
            catch (Exception error)
            {
                _logger.Error("数据访问失败：" + error);
            }
            return resultList;
        }
        #endregion

        #region Update
        /// <summary>
        /// 修改记录
        /// </summary>
        /// <param name="tbname">表名</param>
        /// <param name="model">model</param>
        /// <param name="priname">主键列名</param>
        /// <param name="prikey">主键值</param>
        /// <returns>bool</returns>
        public virtual async Task<bool> EditModel(string tbname, T model, string priname, decimal prikey)
        {
            Type m = model.GetType();
            PropertyInfo[] property = m.GetProperties();    //获取属性

            StringBuilder sql = new StringBuilder();
            sql.Append(" update " + tbname + " set ");          //构造sql语句
            string fields = "";
            return await Task.Run(() =>
            {
                for (int i = 0; i < property.Length; i++)        //循环读取model值
                {
                    if (property[i].PropertyType.FullName == "System.String")       //判断string类型加入引号
                    {
                        fields += property[i].Name + " = '" + property[i].GetValue(model) + "',";
                    }
                    else
                    {
                        fields += property[i].Name + " = " + property[i].GetValue(model) + ",";
                    }

                }
                fields = fields.Substring(0, fields.Length - 1);
                sql.Append(fields + " where " + priname + " = " + prikey);
                _logger.Info("生成的sql语句：" + sql.ToString());
                try
                {
                    if (_conn.Execute(sql.ToString()) != 0)         //执行sql
                    {
                        _logger.Info("修改数据成功（数据访问层）");
                        return true;
                    }
                    else
                    {
                        _logger.Info("修改数据失败（数据访问层）");
                        return false;
                    }
                }
                catch (Exception error)
                {
                    _logger.Error("修改错误：" + error);
                    return false;
                }
            });
        }
        #endregion

        #region Add
        /// <summary>
        /// 添加一条数据（自增ID）
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="Tbname">表名</param>
        /// <returns>bool</returns>
        public async Task<bool> AddModel(T model, string Tbname)
        {
            Type m = model.GetType();
            PropertyInfo[] property = m.GetProperties();    //获取属性

            StringBuilder sql = new StringBuilder();
            sql.Append("insert into " + Tbname);

            string fields = " (";
            //异步操作
            return await Task.Run(() =>
            {
                for (int i = 1; i < property.Length; i++)        //循环列名
                {
                    fields += property[i].Name + ",";
                }
                fields = fields.Substring(0, fields.Length - 1);//删除最后的逗号
                fields += " ) values (";
                for (int i = 1; i < property.Length; i++)        //循环值
                {
                    if (property[i].PropertyType.FullName == "System.String")       //判断string类型加入引号
                    {
                        fields += "'" + property[i].GetValue(model) + "',";
                    }
                    else
                    {
                        fields += property[i].GetValue(model) + ",";
                    }
                }
                fields = fields.Substring(0, fields.Length - 1);//删除最后的逗号
                fields += ")";
                sql.Append(fields);
                try
                {
                    _logger.Info("拼接的SQL：" + sql.ToString());
                    if (_conn.Execute(sql.ToString()) != 0)
                    {
                        _logger.Info("添加数据成功（数据访问层）");
                        return true;
                    }
                    else
                    {
                        _logger.Error("添加数据失败（数据访问层）");
                        return false;
                    }
                }
                catch (Exception error)
                {
                    _logger.Error("添加数据失败（数据访问层）" + error);
                    return false;
                }
            });
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除Model
        /// </summary>
        /// <param name="Tbname">表名</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public async Task<bool> DeleteModel(string Tbname, decimal id)
        {
            string sql = "delete from " + Tbname + " where id=" + id;
            _logger.Info("拼接的Sql语句（数据访问层）：" + sql);
            try
            {
                int result = await Task.Run(() =>
                {
                    _logger.Info("开始执行删除操作（数据访问层）");
                    return _conn.Execute(sql);
                });
                if (result != 0)
                {
                    _logger.Info("执行删除操作成功（数据访问层）");
                    return true;
                }
                else
                {
                    _logger.Error("执行删除操作失败（数据访问层）");
                    return false;
                }
            }
            catch (Exception error)
            {
                _logger.Error("执行操作失败（数据访问层）：" + error);
                return false;
            }

        }
        #endregion 
    }
}
