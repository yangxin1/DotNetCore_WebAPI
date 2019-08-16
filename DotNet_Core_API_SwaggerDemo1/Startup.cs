using Autofac;
using Autofac.Extensions.DependencyInjection;
using DotNet_Core_API_SwaggerDemo1.DAL;
using DotNet_Core_API_SwaggerDemo1.IDAL;
using DotNet_Core_API_SwaggerDemo1.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DotNet_Core_API_SwaggerDemo1
{
    public class Startup
    {
        //public void ConfigureServices(IServiceCollection services)
        public IServiceProvider ConfigureServices(IServiceCollection services)  //使用Autofac需要返回IServiceProvider类型
        {
            #region MVC和跨域访问
            //添加mvc
            services.AddMvc();
            //添加跨域访问
            services.AddCors(options =>
            {
                options.AddPolicy("Cors", builder => builder//添加跨域访问规则
                 .AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials());
            });
            #endregion
            
            #region Swagger和Token
            //添加swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Sparkle",
                    Version = "这是显示在名称上面的版本号 ",
                    Description = "这是描述语句",
                    Contact = new Contact { Name = "Sparkle：", Url = "/views/test.html", Email = "a4200322@live.com" }
                });
                //添加说明文档
                var basepath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlpath = Path.Combine(basepath, "DotNet_Core_API_SwaggerDemo1.xml");
                c.IncludeXmlComments(xmlpath,true);         //显示控制器注释

                #region 绑定Token
                var security = new Dictionary<string, IEnumerable<string>> { { "DotNet_Core_API_SwaggerDemo1", new string[] { } }, };
                c.AddSecurityRequirement(security);
                c.AddSecurityDefinition("DotNet_Core_API_SwaggerDemo1", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
                #endregion
            });
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("AdminOrClient", policy => policy.RequireRole("Admin", "Client").Build());
            });
            #endregion

            #region 依赖注入
            var IOCbuilder = new ContainerBuilder();//建立容器
            List<Assembly> programlist = new List<Assembly> { Assembly.Load("DAL")/*,Assembly.Load("IDAL"),Assembly.Load("MODEL"),Assembly.Load("DotNet_Core_API_SwaggerDemo1") */};//批量反射程序集（临时）
            foreach(var q in programlist)
            {
                IOCbuilder.RegisterAssemblyTypes(q).AsImplementedInterfaces(); //注册容器
            }
            IOCbuilder.Populate(services);//将service注入到容器
            var ApplicationContainner = IOCbuilder.Build();//登记创建容器

            return new AutofacServiceProvider(ApplicationContainner);   //IOC接管
            #endregion
        }

        /// <summary>
        /// 配置请求管道（中间件）
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory">日志</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory)
        {
            //
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //NLog
            loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");
            //Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "这是显示在右上角的文字");
            });            
            //调用中间件
            //app.UseMiddleware<JwtToken>();
            //设置可访问静态文件
            app.UseStaticFiles();
            //默认MVC 路径
            app.UseMvcWithDefaultRoute();
        }
        /// <summary>
        /// 依赖注入(默认)
        /// </summary>
        /// <param name="services"></param>
        public void AddTranTest( IServiceCollection services)
        {
            services.AddScoped<IDBHelper<User>, DBHelper<User>>();
        }
    }
}
