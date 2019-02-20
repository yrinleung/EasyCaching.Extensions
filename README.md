# EasyCaching.Extensions
> 支持 netstandard2.0


[EasyCaching](https://github.com/dotnetcore/EasyCaching)项目的第三方扩展：
- EasyCaching.Interceptor.AspectCore的[Autofac](https://github.com/autofac/Autofac)扩展
- EasyCaching.Interceptor.Castle的[Autofac](https://github.com/autofac/Autofac)扩展


### 1 EasyCaching.Interceptor.AspectCore扩展

#### 1.1 使用方法

> Startup相关配置

```c#

public IServiceProvider ConfigureServices(IServiceCollection services)
{
	//Autofac配置
	var builder = new ContainerBuilder();
	builder.Populate(services);
	
	//将AspectCore加入Autofac
	builder.AddAspectCoreInterceptor();
	
    return new AutofacServiceProvider(builder.Build());
}

```


### 1 EasyCaching.Interceptor.Castle扩展

#### 1.1 使用方法

> Startup相关配置

```c#

public IServiceProvider ConfigureServices(IServiceCollection services)
{
	//Autofac配置
	var builder = new ContainerBuilder();
	builder.Populate(services);
	
	//将Castle加入Autofac
	builder.AddCastleInterceptor();
	
    return new AutofacServiceProvider(builder.Build());
}

```


