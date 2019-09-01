using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MODEL.Web
{
    /// <summary>
    /// 返回结果测试类
    /// </summary>
    public class WebResult : JsonResult 
    {
        /// <summary>
        /// 请求码：200：成功，400：失败
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// 3参数构造函数
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="msg">返回消息</param>
        /// <param name="data">返回数据</param>
        public WebResult(int code, string msg, dynamic data = null) : base(null)
        {
            Code = code;
            Message = msg;
            Data = data;
        }
        /// <summary>
        /// 2参数构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public WebResult(int code,dynamic data = null):base(null)
        {
            Code = code;
            if (Code == 200)
            {
                Message = "操作成功";
            }
            else
            {
                Message = "操作失败";
            }
            Data=data;
        }
        /// <summary>
        /// 必须要重写该方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ExecuteResultAsync(ActionContext context)
        {
            Value = new
            {
                code = Code,
                message = Message,
                data = Data
            };
            return base.ExecuteResultAsync(context);
        }
    }
}
