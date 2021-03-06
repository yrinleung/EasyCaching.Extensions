﻿namespace EasyCaching.Extensions.Demo.Controllers
{
    using EasyCaching.Extensions.Demo.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IAspectCoreService _aService;
        private readonly ICastleService _cService;
        private readonly IWebApiClientService _webApiClientService;

        public ValuesController(IAspectCoreService aService = null, ICastleService cService = null, IWebApiClientService webApiClientService=null)
        {
            this._aService = aService;
            this._cService = cService;
            this._webApiClientService = webApiClientService;
        }

        [HttpGet]
        [Route("aspectcore")]
        public string Aspectcore(int type = 1)
        {
            if (type == 1)
            {
                return _aService.GetCurrentUtcTime();
            }
            else if (type == 2)
            {
                _aService.DeleteSomething(1);
                return "ok";
            }
            else if (type == 3)
            {
                return _aService.PutSomething("123");
            }
            else if (type == 4)
            {
                var res = _aService.GetDemo(111);
                return $"{res.Id}-{res.Name}-{res.CreateTime}";
            }
            else
            {
                return "wait";
            }
        }

        [HttpGet]
        [Route("aspectcoreasync")]
        public async Task<string> AspectcoreAsync(int type = 1)
        {
            if (type == 1)
            {
                return await _aService.GetUtcTimeAsync();
            }
            else if (type == 2)
            {
                var res = await _aService.GetDemoAsync(999);
                return $"{res.Id}-{res.Name}-{res.CreateTime}";
            }
            else
            {
                return await Task.FromResult("wait");
            }
        }

        [HttpGet]
        [Route("castle")]
        public string Castle(int type = 1)
        {
            if (type == 1)
            {
                return _cService.GetCurrentUtcTime();
            }
            else if (type == 2)
            {
                _cService.DeleteSomething(1);
                return "ok";
            }
            else if (type == 3)
            {
                return _cService.PutSomething("123");
            }
            else if (type == 4)
            {
                var res = _cService.GetDemo(111);
                return $"{res.Id}-{res.Name}-{res.CreateTime}";
            }
            else
            {
                return "wait";
            }
        }

        [HttpGet]
        [Route("castleasync")]
        public async Task<string> CastleAsync(int type = 1)
        {
            if (type == 1)
            {
                return await _cService.GetUtcTimeAsync();
            }
            else if (type == 2)
            {
                var res = await _cService.GetDemoAsync(999);
                return $"{res.Id}-{res.Name}-{res.CreateTime}";
            }
            else
            {
                return await Task.FromResult<string>("wait");
            }
        }


        [HttpGet]
        [Route("WebApiClient")]
        public async Task<string> WebApiClient()
        {
            return await _webApiClientService.GetHtml();
        }
    }
}
