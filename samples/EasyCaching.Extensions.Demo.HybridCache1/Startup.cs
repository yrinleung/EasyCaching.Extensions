using EasyCaching.Core.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyCaching.Extensions.Demo.HybridCache1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //use CAP ，根据CAP官方文档配置即可
            services.AddCap(x =>
            {
                x.UseInMemoryStorage();
                x.UseRabbitMQ(configure =>
                {
                    configure.HostName = "127.0.0.1";
                    configure.UserName = "admin";
                    configure.Password = "admin";
                });
                x.UseDashboard();
            });

            //new configuration
            services.AddEasyCaching(option =>
            {
                //use memory cache
                option.UseInMemory("default");
                
                //use redis cache
                option.UseRedis(config =>
                {
                    config.DBConfig.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6379));
                    config.DBConfig.Database = 5;
                }, "redis1")
                ;

                option.UseHybrid(config =>
                {
                    config.TopicName = "test-topic";
                    config.EnableLogging = false;

                    // specify the local cache provider name after v0.5.4
                    config.LocalCacheProviderName = "default";
                    // specify the distributed cache provider name after v0.5.4
                    config.DistributedCacheProviderName = "redis1";
                });
                // use Cap bus
                option.WithCapBus(x=>
                {
                    x.TopicName = "test-topic";
                });

            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
