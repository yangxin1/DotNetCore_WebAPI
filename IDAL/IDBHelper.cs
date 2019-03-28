using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet_Core_API_SwaggerDemo1.IDAL
{
    public interface IDBHelper<T>
    {
        /// <summary>
        /// 根据表名 字段名 获取Model(异步)
        /// </summary>
        /// <param name="TBname">表名</param>
        /// <param name="fieldname">字段名</param>
        /// <param name="value">字段值</param>
        /// <returns>model</returns>
        Task<T> GetModel(string TBname, string fieldname, string value);

        /// <summary>
        /// 修改Model（仅支持主键id为）
        /// </summary>
        /// <param name="tbname">表名</param>
        /// <param name="model">修改的model</param>
        /// <param name="priname">主键列名</param>
        /// <param name="prikey">主键值</param>
        /// <returns>bool</returns>
        Task<bool> EditModel(string tbname, T model, string priname, decimal prikey);

        /// <summary>
        /// 添加一条记录
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="Tbname">表名</param>
        /// <returns>bool</returns>
        Task<bool> AddModel(T model, string Tbname);

        /// <summary>
        /// 删除Model
        /// </summary>
        /// <param name="Tbname">表名</param>
        /// <param name="id">主键ID</param>
        /// <returns>bool</returns>
        Task<bool> DeleteModel(string Tbname, decimal id);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="Tbname">表名</param>
        /// <param name="pagesize">需要查询的页</param>
        /// <param name="pagecount">每页显示的页数</param>
        /// <returns>结果集</returns>
        Task<List<T>> GetList(string Tbname, int pagesize = 1, int pagecount = 10);
    }
}
