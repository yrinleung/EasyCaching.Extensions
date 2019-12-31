using EasyCaching.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.EasyCaching
{
    public class EasyCachingDistributedCache : IDistributedCache
    {
        private readonly IEasyCachingProvider _provider;
        private readonly EasyCachingOptions _options;
        private readonly ConcurrentDictionary<string, TimeSpan> _expirations;


        public EasyCachingDistributedCache(IEasyCachingProviderFactory factory, IOptions<EasyCachingOptions> options)
        {
            _options = options.Value;
            this._provider = factory.GetCachingProvider(_options.CachingProviderName);
            _expirations = new ConcurrentDictionary<string, TimeSpan>();
        }

        public byte[] Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return this.GetAndRefreshAsync(key).GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return await this.GetAndRefreshAsync(key);
        }

        public void Refresh(string key)
        {
            this.GetAndRefreshAsync(key).GetAwaiter().GetResult();
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            await this.GetAndRefreshAsync(key);
        }

        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            this._provider.Remove(key);
            TimeSpan expiration;
            _expirations.TryRemove(key, out expiration);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await this._provider.RemoveAsync(key);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            this.SetAsync(key, value, options).GetAwaiter().GetResult();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            TimeSpan expiration = GetExpiration(options);

            if (!_expirations.ContainsKey(key))
            {
                _expirations.TryAdd(key, expiration);
            }

            await this.SetAsync(key, value, expiration);
        }

        private static DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset creationTime, DistributedCacheEntryOptions options)
        {
            if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(DistributedCacheEntryOptions.AbsoluteExpiration),
                    options.AbsoluteExpiration.Value,
                    "The absolute expiration value must be in the future.");
            }
            var absoluteExpiration = options.AbsoluteExpiration;
            if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                absoluteExpiration = creationTime + options.AbsoluteExpirationRelativeToNow;
            }

            return absoluteExpiration;
        }

        private static long? GetExpirationInSeconds(DateTimeOffset creationTime, DateTimeOffset? absoluteExpiration, DistributedCacheEntryOptions options)
        {
            if (absoluteExpiration.HasValue && options.SlidingExpiration.HasValue)
            {
                return (long)Math.Min(
                    (absoluteExpiration.Value - creationTime).TotalSeconds,
                    options.SlidingExpiration.Value.TotalSeconds);
            }
            else if (absoluteExpiration.HasValue)
            {
                return (long)(absoluteExpiration.Value - creationTime).TotalSeconds;
            }
            else if (options.SlidingExpiration.HasValue)
            {
                return (long)options.SlidingExpiration.Value.TotalSeconds;
            }
            return null;
        }

        private async Task<byte[]> GetAndRefreshAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheValue = this._provider.Get<byte[]>(key);
            if (cacheValue.IsNull || !cacheValue.HasValue)
            {
                return null;
            }
            TimeSpan expiration;
            if (!_expirations.TryGetValue(key, out expiration))
            {
                expiration = GetExpiration(new DistributedCacheEntryOptions());
            }

            await this.SetAsync(key, cacheValue.Value, expiration);

            return cacheValue.Value;
        }

        private TimeSpan GetExpiration(DistributedCacheEntryOptions options)
        {
            var creationTime = DateTimeOffset.UtcNow;
            GetOptions(ref options);
            var absoluteExpiration = GetAbsoluteExpiration(creationTime, options);

            var second = GetExpirationInSeconds(creationTime, absoluteExpiration, options);

            return TimeSpan.FromSeconds(second.Value);
        }


        private async Task SetAsync(string key, byte[] value, TimeSpan expiration)
        {
            await this._provider.SetAsync(key, value, expiration);
        }

        private void GetOptions(ref DistributedCacheEntryOptions options)
        {
            if (!options.AbsoluteExpiration.HasValue
                && !options.AbsoluteExpirationRelativeToNow.HasValue
                && !options.SlidingExpiration.HasValue)
            {
                options = new DistributedCacheEntryOptions()
                {
                    SlidingExpiration = _options.DefaultSlidingExpiration
                };
            }
        }
    }
}
