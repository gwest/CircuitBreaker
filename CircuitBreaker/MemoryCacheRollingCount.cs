namespace CircuitBreaker
{
    using System;
    using System.Linq;
    using System.Runtime.Caching;

    public class MemoryCacheRollingCount : IRollingCount
    {
        private MemoryCache cache;

        private CacheItemPolicy policy;

        public MemoryCacheRollingCount(string name)
            : this(name, new TimeSpan(0, 1, 0))
        {
        }

        public MemoryCacheRollingCount(string name, TimeSpan timeSpan)
        {
            this.CreateCache(name, timeSpan);
        }

        public int Count { get { return cache.Count(); } }

        public void Increment()
        {
            this.cache.Add(Guid.NewGuid().ToString(), true, this.policy);
        }

        public void Reset()
        {
            var name = this.cache.Name;
            
            this.cache.Dispose();

            this.CreateCache(name, policy.SlidingExpiration);
        }

        private void CreateCache(string name, TimeSpan timeSpan)
        {

            this.policy = new CacheItemPolicy { SlidingExpiration = timeSpan };
            
            this.cache = new MemoryCache(name);
        }
    }
}
