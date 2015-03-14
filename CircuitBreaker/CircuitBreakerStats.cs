namespace CircuitBreaker
{
    using System;

    public class CircuitBreakerStats : ICircuitBreakerStats
    {
        private const string SuccessName = "Success";
        private const string FailureName = "Failure";

        private readonly IRollingCount rollingSuccessCount;
        private readonly IRollingCount rollingFailureCount;

        public CircuitBreakerStats(string name)
        {
            this.rollingSuccessCount = new MemoryCacheRollingCount(name + SuccessName);
            this.rollingFailureCount = new MemoryCacheRollingCount(name + FailureName);
        }

        public CircuitBreakerStats(string name, TimeSpan ttl)
        {
            this.rollingSuccessCount = new MemoryCacheRollingCount(name + SuccessName, ttl);
            this.rollingFailureCount = new MemoryCacheRollingCount(name + FailureName, ttl);
        }

        public int CountSuccess
        {
            get
            {
                return this.rollingSuccessCount.Count;
            } 
        }

        public int CountFailure
        {
            get
            {
                return this.rollingFailureCount.Count;
            }
        }

        public double PercFailure {
            get
            {
                var successCount = this.rollingSuccessCount.Count;
                var failureCount = this.rollingFailureCount.Count;

                if (failureCount == 0)
                {
                    return 0;
                }

                if (successCount == 0)
                {
                    return 100;
                }

                return (100.0 / successCount) * this.rollingFailureCount.Count;
            }
        }

        public void IncrementSuccess()
        {
            this.rollingSuccessCount.Increment();
        }

        public void IncrementFailure()
        {
            this.rollingFailureCount.Increment();
        }

        public void Reset()
        {
            this.rollingSuccessCount.Reset();
            this.rollingFailureCount.Reset();
        }
    }
}