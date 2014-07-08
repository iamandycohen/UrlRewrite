using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite.Caching
{
    public abstract class CacheProvider<TCache> : ICacheProvider
    {

        public int CacheDuration
        {
            get;
            set;
        }

        private readonly int defaultCacheDurationInSeconds = 1800;

        protected TCache _cache;

        public CacheProvider()
        {
            CacheDuration = defaultCacheDurationInSeconds;
            _cache = InitCache();
        }
        public CacheProvider(int durationInSeconds)
        {
            CacheDuration = durationInSeconds;
            _cache = InitCache();
        }

        protected abstract TCache InitCache();

        public abstract bool Get<T>(string key, out T value);

        public abstract void Set<T>(string key, T value);

        public abstract void Set<T>(string key, T value, int duration);

        public abstract void Clear(string key);

        public abstract IEnumerable<KeyValuePair<string, object>> GetAll();

    }
}
