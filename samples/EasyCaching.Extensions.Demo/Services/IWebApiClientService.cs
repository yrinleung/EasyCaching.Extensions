namespace EasyCaching.Extensions.Demo.Services
{
    using System.Threading.Tasks;
    using EasyCaching.Core.Interceptor;
    using EasyCaching.Core.Internal;
    using WebApiClient;
    using WebApiClient.Attributes;

    public interface IWebApiClientService : IHttpApi
    {
        [EasyCachingAble(Expiration = 10)]
        [HttpGet("http://www.baidu.com/")]
        ITask<string> GetHtml();
    }

}
