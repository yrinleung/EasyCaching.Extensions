namespace EasyCaching.Extensions.Demo.Controllers
{
    using EasyCaching.Extensions.Demo.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    public class DistributedCacheController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedCacheController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var byteArray = await _distributedCache.GetAsync("test");
            if (byteArray == null)
            {
                return "not key";
            }
            return System.Text.Encoding.Default.GetString(byteArray);
        }


        [HttpGet("set")]
        public async Task<string> Set()
        {
            await _distributedCache.SetStringAsync("test", "测试");
            return "ok";
        }

        [HttpGet("remove")]
        public async Task<string> Remove()
        {
            await _distributedCache.RemoveAsync("test");
            return "ok";
        }


        [HttpGet("refresh")]
        public async Task<string> Refresh()
        {
            await _distributedCache.RefreshAsync("test");
            return "ok";
        }
    }
}
