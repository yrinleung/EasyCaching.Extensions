# EasyCaching.Extensions

[EasyCaching](https://github.com/dotnetcore/EasyCaching)项目的第三方扩展：[Autofac](https://github.com/autofac/Autofac)、[WebApiClient](https://github.com/dotnetcore/WebApiClient)

### 1 WebApiClient扩展
#### 1.1 Nuget
```
PM> Install-Package YrinLeung.EasyCaching.Interceptor.WebApiClient
```
支持 netstandard2.0

#### 1.2 使用方法

> Startup相关配置

```c#

public IServiceProvider ConfigureServices(IServiceCollection services)
{
	//将WebClient接口注入
    services.AddWebApiClientUseHttpClientFactory<IWebApiClientService>();
	...
}

```


### 2 AspectCore的Autofac扩展
#### 2.1 Nuget
```
PM> Install-Package YrinLeung.EasyCaching.Interceptor.AspectCore.Autofac
```
支持 netstandard2.0

#### 2.2 使用方法

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


### 3 Castle的Autofac扩展
#### 3.1 Nuget
```
PM> Install-Package YrinLeung.EasyCaching.Interceptor.Castle.Autofac
```
支持 netstandard2.0

#### 3.2 使用方法

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


