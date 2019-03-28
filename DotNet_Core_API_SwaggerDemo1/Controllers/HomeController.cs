using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet_Core_API_SwaggerDemo1.DAL;
using DotNet_Core_API_SwaggerDemo1.IDAL;
using DotNet_Core_API_SwaggerDemo1.Model;
using IDAL;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NLog;

namespace DotNet_Core_API_SwaggerDemo1.Controllers
{
    /// <summary>
    /// 基础CRUD控制器
    /// </summary>
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        #region 字段/属性/构造函数
        /// <summary>
        /// 日志
        /// </summary>
        static Logger _log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 数据库访问类
        /// </summary>
        private IHomeDAL _DBHelper;
        /// <summary>
        /// 构造函数
        /// </summary>
        public HomeController(IHomeDAL db)
        {
            _DBHelper = db;
        }
        #endregion

        #region 控制器
        /// <summary>
        /// 获取数据（异步，读取数据库，分页）
        /// </summary>
        /// <param name="pagesize">当前显示的页</param>
        /// <param name="pagecount">每页显示条数</param>
        /// <returns></returns>
        [EnableCors("Cors")]//跨域
        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<List<User>> GetList(int pagesize=1,int pagecount=2)
        {
            return await _DBHelper.GetList("user", pagesize, pagecount);
            //return await DBHelper<User>.GetList("user", pagesize, pagecount);
        }
        /// <summary>
        /// 根据ID 获取用户（使用异步，读取数据库）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [EnableCors("Cors")]//跨域
        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<User> GetUserbyId(int id)
        {
            _log.Info("开始查询数据（控制器端）");
            return await _DBHelper.GetModel("user", "id", id.ToString());

            //异步方法1
            //var result = Task.Run(() =>
            //{
            //    return _DBHelper.GetModel("user", id);
            //});
            //return await result;

            //异步方法2简写
            //return Task.Run(() => { return _DBHelper.GetModel("user", id); }); 
        }
        /// <summary>
        /// 根据ID获取用户（使用异步，读取数据库，json数据 ）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [EnableCors("Cors")]//跨域
        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<string> GetUserbyIdJson(int id)
        {
            _log.Info("开始查询数据（控制器端）");
            User result = await _DBHelper.GetModel("user", "id", id.ToString());
            return JsonConvert.SerializeObject(result);
        }
        /// <summary>
        /// 根据姓名获取Model
        /// </summary>
        /// <param name="name">姓名</param>
        /// <returns>model</returns>
        [EnableCors("Cors")]//跨域
        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<User> GetUserbyName(string name)
        {
            return await _DBHelper.GetModel("user", "name", name);
        }
        /// <summary>
        /// 根据ID修改信息(使用异步，读取数据库)
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        [EnableCors("Cors")]//跨域
        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> EditUser([FromBody]User newuser)
        {
            _log.Info("开始修改数据（控制器端）");
            bool result = await _DBHelper.EditModel("user", newuser, "id", newuser.Id);
            if (result)
            {
                _log.Info("修改数据成功（控制器端）");
                return Ok();
            }
            else
            {
                _log.Info("修改数据失败（控制器端）");
                return NotFound();
            }
        }
        /// <summary>
        /// 添加用户(使用异步,添加到数据库)
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        [EnableCors("Cors")]//跨域
        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> AddUser([FromBody] User newuser)
        {
            bool result = await _DBHelper.AddModel(newuser, "user");
            if (result)
            {
                _log.Info("数据添加成功（控制器端）");
                return Ok();
            }
            else
            {
                _log.Error("数据添加失败（控制器端）");
                return NotFound();
            }
        }
        /// <summary>
        /// 删除用户（异步，读取数据库）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [EnableCors("Cors")]//跨域
        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            bool result = await _DBHelper.DeleteModel("user", id);
            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        /// <summary>
        /// 添加1000条数据（禁用）
        /// </summary>
        /// <param name="isadd">是否添加</param>
        /// <returns></returns>
        [EnableCors("Cors")]//跨域
        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Add1000data(bool isadd)
        {
            isadd = false;
            if (isadd)
            {
                for(int i = 50; i < 1000; i++)
                {
                    User data = new User { Id = i, Name = "machine_data" + i.ToString(), Age = 50 };
                    bool result = await _DBHelper.AddModel(data, "user");
                }
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        #endregion 
    }
}