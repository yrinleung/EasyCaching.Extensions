using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EasyCaching.Core;
using EasyCaching.Extensions.Demo.Services;
using EasyCaching.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EasyCaching.Extensions.Demo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //AspectCore
            services.AddScoped<IAspectCoreService, AspectCoreService>();

            //Castle
            services.AddScoped<ICastleService, CastleService>();

            //将WebClient接口注入
            services.AddWebApiClientUseHttpClientFactory<IWebApiClientService>();
            //or
            // services.AddWebApiClientUseHttpClientFactory<IWebApiClientService>((httpApiConfig) =>
            // {
            // 	httpApiConfig.HttpHost = new Uri("http://www.baidu.com");
            // });
            //or
            // services.AddWebApiClientUseHttpClientFactory<IWebApiClientService>((httpApiConfig, p) =>
            // {
            // 	httpApiConfig.HttpHost = new Uri("http://www.baidu.com");
            // });

            services.AddEasyCaching(options =>
            {
                //options.UseRedis(config =>
                //{
                //    config.DBConfig.Endpoints.Add(new Core.Configurations.ServerEndPoint("127.0.0.1", 6379));
                //    config.DBConfig.Database = 5;
                //}, "myredis");
                options.UseInMemory();

            });

            services.AddEasyCachingCache(config =>
            {
                config.CachingProviderName = "myredis";
                config.DefaultSlidingExpiration = TimeSpan.FromMinutes(20);
            });

        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            //使用AspectCore
            //builder.AddAspectCoreInterceptor(x => x.CacheProviderName = EasyCachingConstValue.DefaultInMemoryName);

            //使用Castle
            builder.AddCastleInterceptor(x=>x.CacheProviderName= EasyCachingConstValue.DefaultInMemoryName);

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
