using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.MemoryStorage;
using EmailServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using JobBakupDatabaseNetCore.Invocable;

namespace JobBakupDatabaseNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {     
            //AddHangfire https://www.hangfire.io/
            //in this case use save data in memory
            services.AddHangfire(config =>
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseDefaultTypeSerializer()
            .UseMemoryStorage());

            services.AddHangfireServer();

            services.AddControllersWithViews();
            services.AddSingleton<IJobBackup, JobBackup>(x => new JobBackup(x.GetService<IConfiguration>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJonManager,
            IServiceProvider serviceProvider
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //Use dashboard
            app.UseHangfireDashboard();
            //job: At 10:30 on Friday
            recurringJonManager.AddOrUpdate(
               "Run every minute",
               () => serviceProvider.GetService<IJobBackup>().Send(),
               "30 10 * * fri", TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time")
                );
        }
    }
}
