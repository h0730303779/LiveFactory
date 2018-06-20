using JFJT.Authorize;
using JFJT.Framework;
using LiveFactory.Application;
using LiveFactory.Application.Command;
using LiveFactory.Core;
using LiveFactory.Core.Models.Config;
using LiveFactory.EntityFramework;
using LiveFactory.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace LiveFactory.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)  //IConfiguration configuration,
        {
            var build = new ConfigurationBuilder()
                         .SetBasePath(env.ContentRootPath)
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                         .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                         .AddEnvironmentVariables();
            Configuration = build.Build(); ;
            //Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //services.AddJwtAuthorization(Configuration);

            var types = new[] { typeof(ApplicationModule) };
            services.AddCoreAuthorization(option=> { });

            //services.AddMvc(option => {
            //    option.AddAppFilters();
            //});

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            services.AddSignalR();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));


            var iservice = services.AddFramework<LiveFactoryDbContext>(opt =>
              {
                  opt.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
              });

            return iservice;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
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
            app.UseFramework(option =>
            {
                option.AddNLog(env, loggerFactory);
            });  //启用框架基类服务
            app.UseAuthentication();

            app.UseSignalR(route =>
            {
                route.MapHub<SignalrHubs>("/signalrHubs");
            });
            app.UseWebSockets();
            //using (var service = serviceScopeFactory.CreateScope())
            //{
            //    var command = service.ServiceProvider.GetService<ICommandService>();
            //    command.OnRecevice();
            //}
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<LiveFactoryDbContext>()
                    .Database.Migrate();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
