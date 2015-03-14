namespace CircuitBreaker
{
    public interface IRollingCount
    {
        int Count { get; }

        void Increment();

        void Reset();
    }
}