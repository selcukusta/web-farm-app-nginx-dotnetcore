using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace app.Caching
{
    public class RedisCacheProvider
    {
        public string Host { get; private set; }
        public int DefaultDb { get; private set; }
        public TimeSpan DefaultCacheDuration => TimeSpan.FromSeconds(15);

        public RedisCacheProvider(string host, int defaultDb)
        {
            Host = host;
            DefaultDb = defaultDb;
        }

        public ConnectionMultiplexer GetConnection()
        {
            return ConnectionMultiplexer.Connect(Host);
        }

        public IDatabase GetDatabase()
        {
            return GetConnection().GetDatabase(DefaultDb);
        }

        public IDatabase Instance
        {
            get
            {
                return GetDatabase();
            }
        }

        public T Get<T>(string key)
        {
            if (Instance.KeyExists(key))
            {
                var redisValue = Instance.StringGet(key);
                if (redisValue.HasValue)
                {
                    return JsonConvert.DeserializeObject<T>(redisValue.ToString());
                }
            }

            return default(T);
        }

        public void Set<T>(string key, T value)
        {
            if (value == null)
            {
                return;
            }
            var jsonValue = JsonConvert.SerializeObject(value);
            Instance.StringSet(key, jsonValue);
        }

        public void Set<T>(string key, T value, TimeSpan cacheTime)
        {
            if (value == null)
            {
                return;
            }

            var jsonValue = JsonConvert.SerializeObject(value);
            Instance.StringSet(key, jsonValue, cacheTime);
        }

        public void Clear(string key)
        {
            if (Instance.KeyExists(key))
            {
                Instance.KeyDelete(key);
            }
        }
    }
}
