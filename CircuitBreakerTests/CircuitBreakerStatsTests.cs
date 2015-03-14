namespace CircuitBreakerTests
{
    using System;
    using System.Threading;

    using CircuitBreaker;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CircuitBreakerStatsTests
    {
        private ICircuitBreakerStats stats;

        [TestInitialize]
        public void SetupTests()
        {
            this.stats = new CircuitBreakerStats("test");
        }

        [TestMethod]
        public void CountSuccessReturns0AfterInstantiation()
        {
            var result = this.stats.CountSuccess;

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CountFailureReturns0AfterInstantiation()
        {
            var result = this.stats.CountFailure;

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void IncrementSuccessAdds1ToTheSuccessCache()
        {
            this.stats.IncrementSuccess();
            var result = this.stats.CountSuccess;

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void IncrementFailureAdds1ToTheFailureCache()
        {
            this.stats.IncrementFailure();
            var result = this.stats.CountFailure;

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void IncrementSuccessDoesNotAdd1ToTheFailureCache()
        {
            this.stats.IncrementSuccess();
            var result = this.stats.CountFailure;

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void IncrementFailureAdds1ToTheSuccessCache()
        {
            this.stats.IncrementFailure();
            var result = this.stats.CountSuccess;

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void PercFailureReturnsCorrectly()
        {
            for (var i = 0; i < 20; i++)
            {
                this.stats.IncrementSuccess();

                if (i % 10 == 0)
                {
                    this.stats.IncrementFailure();
                }
            }
            
            var result = this.stats.PercFailure;

            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void PercFailureReturnsCorrectlyWhenSuccessIs0()
        {
            this.stats.IncrementFailure();
            
            var result = this.stats.PercFailure;

            Assert.AreEqual(100, result);
        }

        [TestMethod]
        public void PercFailureReturnsCorrectlyWhenFailureIs0()
        {
            this.stats.IncrementSuccess();

            var result = this.stats.PercFailure;

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void PercFailureReturnsCorrectlyWhenSuccessAndFailureIs0()
        {
            var result = this.stats.PercFailure;

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void SuccessCacheExpirationWorksCorrectly()
        {
            this.stats = new CircuitBreakerStats("test", new TimeSpan(0, 0, 5));

            var original = this.stats.CountSuccess;

            this.stats.IncrementSuccess();

            var result = this.stats.CountSuccess;

            Assert.AreEqual(original + 1, result);

            for (var i = 0; i < 10; i++)
            {
                result = this.stats.CountSuccess;
                Console.WriteLine(i + " seconds: " + result);

                if (result == original)
                {
                    break;
                }

                Thread.Sleep(1000 * i);
            }

            Assert.AreEqual(original, result);
        }

        [TestMethod]
        public void FailureCacheExpirationWorksCorrectly()
        {
            this.stats = new CircuitBreakerStats("test", new TimeSpan(0, 0, 5));

            var original = this.stats.CountFailure;

            this.stats.IncrementFailure();

            var result = this.stats.CountFailure;

            Assert.AreEqual(original + 1, result);

            for (var i = 0; i < 10; i++)
            {
                result = this.stats.CountFailure;
                Console.WriteLine(i + " seconds: " + result);

                if (result == original)
                {
                    break;
                }

                Thread.Sleep(1000 * i);
            }

            Assert.AreEqual(original, result);
        }

        [TestMethod]
        public void ResetWorksCorrectly()
        {
            this.stats.IncrementSuccess();
            this.stats.IncrementFailure();

            this.stats.Reset();

            Assert.AreEqual(0, this.stats.CountSuccess);
            Assert.AreEqual(0, this.stats.CountFailure);
        }

    }
}