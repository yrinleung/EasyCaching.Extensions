using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;

namespace EasyCaching.Extensions.Demo.HybridCache1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IHybridCachingProvider _provider;

        public ValuesController(IHybridCachingProvider provider)
        {
            this._provider = provider;
        }

        // GET api/values/get?type=1
        [HttpGet]
        [Route("get")]
        public string Get(string str)
        {
            var method = str.ToLower();
            switch (method)
            {
                case "get":
                    {
                        var res = _provider.Get<string>("demo");
                        return $"cached value : {res}";
                    }
                case "getset":
                    {
                        var res = _provider.Get("demo", () => "1-456", TimeSpan.FromHours(1));
                        return $"cached value : {res}";
                    }
                case "set":
                    _provider.Set("demo", "1-123", TimeSpan.FromHours(1));
                    return "seted";
                case "remove":
                    _provider.Remove("demo");
                    return "removed";
               
                default:
                    return "default";
            }
        }


        // GET api/values/getasync?type=1
        [HttpGet]
        [Route("getasync")]
        public async Task<string> GetAsync(string str)
        {
            var method = str.ToLower();
            switch (method)
            {
                case "get":
                    var res = await _provider.GetAsync("demo", async () => await Task.FromResult("1-456"), TimeSpan.FromHours(1));
                    return $"cached value : {res}";
                case "set":
                    await _provider.SetAsync("demo", "1-123", TimeSpan.FromHours(1));
                    return "seted";
                case "remove":
                    await _provider.RemoveAsync("demo");
                    return "removed";
                default:
                    return "default";
            }
        }

    }
}