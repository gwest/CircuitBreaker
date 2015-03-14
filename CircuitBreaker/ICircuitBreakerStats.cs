namespace CircuitBreaker
{
    public interface ICircuitBreakerStats
    {
        int CountSuccess { get; }

        int CountFailure { get; }

        double PercFailure { get; }

        void IncrementSuccess();

        void IncrementFailure();

        void Reset();
    }
}