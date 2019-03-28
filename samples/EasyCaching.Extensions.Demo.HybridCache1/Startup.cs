using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Bus.RabbitMQ;
using EasyCaching.Core;
using EasyCaching.Core.Configurations;
using EasyCaching.HybridCache;
using EasyCaching.InMemory;
using EasyCaching.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyCaching.Extensions.Demo.HybridCache1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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
                option.WithCapBus(x =>
                {
                    x.TopicName = "test-topic";
                });

            });
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

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            // Important step for using Memcached Cache or SQLite Cache
            app.UseEasyCaching();

            app.UseMvc();
        }
    }
}
