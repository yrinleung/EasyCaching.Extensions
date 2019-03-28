using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EasyCaching.Core;
using EasyCaching.Extensions.Demo.Services;
using EasyCaching.InMemory;
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
                options.UseInMemory();

            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            var builder = new ContainerBuilder();
            builder.Populate(services);


            //使用AspectCore
            builder.AddAspectCoreInterceptor();

            //使用Castle
            //builder.AddCastleInterceptor();

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
