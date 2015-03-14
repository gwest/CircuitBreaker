namespace CircuitBreakerTests
{
    using CircuitBreaker;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Osherove.ThreadTester;

    [TestClass]
    public class ThreadedCircuitBreakerStatsTests
    {
        private ICircuitBreakerStats stats;

        [TestInitialize]
        public void SetupTests()
        {
            this.stats = new CircuitBreakerStats("statsThreadedTest");
        }

        [TestMethod]
        public void IncrementIn3ThreadsAddsCorrectlyToTheStats()
        {
            var tt = new ThreadTester();

            tt.AddThreadAction(() => this.stats.IncrementSuccess());
            tt.AddThreadAction(() => this.stats.IncrementSuccess());
            tt.AddThreadAction(() => this.stats.IncrementFailure());
            tt.StartAllThreads(1000);

            Assert.AreEqual(2, this.stats.CountSuccess);
            Assert.AreEqual(1, this.stats.CountFailure);
        }
    }
}