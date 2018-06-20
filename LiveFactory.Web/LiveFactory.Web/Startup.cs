using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiveFactory.Web.Common;
using LiveFactory.Web.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace LiveFactory.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IServiceProvider service)
        {
            Configuration = configuration;
            _service = service;
        }

        public IConfiguration Configuration { get; }
        private IServiceProvider _service { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LiveDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //                           .AddCookie(options =>
            //                           {
            //                               options.LoginPath = new PathString("/login/index");
            //                               options.AccessDeniedPath = new PathString("/Error/index");
            //                           });
            //// 添加 Cookie 服务
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //.AddCookie(options =>
            //{
            //    options.LoginPath = "/Account/LogIn";
            //    options.LogoutPath = "/Account/LogOff";
            //});
            //services.AddMemoryCache();//添加缓存
            services.AddScoped<IIdentityManager,IdentityManager>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //            app.UseStaticFiles(new StaticFileOptions()
            //            {
            //                FileProvider = new PhysicalFileProvider(
            //Path.Combine(Directory.GetCurrentDirectory(), @"Content")),
            //                RequestPath = new PathString("/Content")
            //            });

            //// 使用Cook的中间件
            //app.UseAuthentication();
//            app.UseStaticFiles(new StaticFileOptions()
//            {
//                FileProvider = new PhysicalFileProvider(
//Path.Combine(Directory.GetCurrentDirectory(), @"Content")),
//                RequestPath = new PathString("/Content")
//            });
            using(var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<LiveDbContext>()
                    .Database.Migrate();
            }
//            app.UseStaticFiles(new StaticFileOptions()
//            {
//                FileProvider = new PhysicalFileProvider(
//Path.Combine(Directory.GetCurrentDirectory(), @"Content")),
//                RequestPath = new PathString("/Content")
//            });
        }
    }
}
