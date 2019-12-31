using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EasyCaching.Core;
using EasyCaching.Extensions.Demo.Services;
using EasyCaching.InMemory;
using EasyCaching.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EasyCaching.Extensions.Demo
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
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
                options.UseRedis(config =>
                {
                    config.DBConfig.Endpoints.Add(new Core.Configurations.ServerEndPoint("127.0.0.1", 6379));
                    config.DBConfig.Database = 5;
                }, "myredis");
                //options.UseInMemory();

            });

            services.AddEasyCachingCache(config =>
            {
                config.CachingProviderName = "myredis";
                config.DefaultSlidingExpiration = TimeSpan.FromMinutes(20);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            var builder = new ContainerBuilder();
            builder.Populate(services);


            //使用AspectCore
            builder.AddAspectCoreInterceptor(x => x.CacheProviderName = EasyCachingConstValue.DefaultInMemoryName);

            //使用Castle
            //builder.AddCastleInterceptor(x=>x.CacheProviderName= EasyCachingConstValue.DefaultInMemoryName);

            return new AutofacServiceProvider(builder.Build());
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
