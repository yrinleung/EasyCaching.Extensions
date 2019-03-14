using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;

namespace EasyCaching.Interceptor.WebApiClient
{
    public class ApiCacheTask<TResult> : ITask<TResult>
    {
        private readonly TResult Result;
        public ApiCacheTask(TResult result)
        {
            Result = result;
        }

        /// <summary>
        /// 执行InvokeAsync
        /// 并返回其TaskAwaiter对象
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter<TResult> GetAwaiter()
        {
            return this.InvokeAsync().GetAwaiter();
        }

        /// <summary>
        /// 配置用于等待的等待者
        /// </summary>
        /// <param name="continueOnCapturedContext">试图继续回夺取的原始上下文，则为 true；否则为 false</param>
        /// <returns></returns>
        public ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
        {
            return this.InvokeAsync().ConfigureAwait(continueOnCapturedContext);
        }

        public Task<TResult> InvokeAsync()
        {
            return Task<TResult>.FromResult(Result);
        }

        Task ITask.InvokeAsync()
        {
            return Task<TResult>.FromResult(Result);
        }

        public static ApiCacheTask<TResult> FromResult(TResult result)
        {
            return new ApiCacheTask<TResult>(result);
        }
    }
}
