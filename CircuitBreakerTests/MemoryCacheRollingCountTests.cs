namespace CircuitBreakerTests
{
    using System;
    using System.Threading;

    using CircuitBreaker;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MemoryCacheRollingCountTests
    {
        private IRollingCount rollingCount;

        [TestInitialize]
        public void SetupTests()
        {
            this.rollingCount = new MemoryCacheRollingCount("test");
        }

        [TestMethod]
        public void CountReturns0AfterInstantiation()
        {
            var result = this.rollingCount.Count;

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void IncrementAdds1ToTheRollingCount()
        {
            var original = this.rollingCount.Count;
            this.rollingCount.Increment();
            var result = this.rollingCount.Count;

            Assert.AreEqual(original + 1, result);
        }

        [TestMethod]
        public void ExpirationWorksCorrectly()
        {
            this.rollingCount = new MemoryCacheRollingCount("test", new TimeSpan(0, 0, 5));

            var original = this.rollingCount.Count;

            this.rollingCount.Increment();

            var result = this.rollingCount.Count;

            Assert.AreEqual(original + 1, result);

            for (var i = 0; i < 10; i++)
            {
                result = this.rollingCount.Count;
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
            this.rollingCount = new MemoryCacheRollingCount("test", new TimeSpan(0, 1, 0));

            this.rollingCount.Increment();

            this.rollingCount.Reset();

            Assert.AreEqual(0, this.rollingCount.Count);
        }

        [TestMethod]
        public void Increment1KTimesTakesLessThan1Second()
        {
            this.IncrementPerformanceTest(1000, new TimeSpan(0, 0, 1).Ticks);
        }

        [TestMethod]
        public void Increment10KTimesTakesLessThan1Second()
        {
            this.IncrementPerformanceTest(10000, new TimeSpan(0, 0, 1).Ticks);
        }

        [TestMethod]
        public void Increment100KTimesTakesLessThan1Second()
        {
            this.IncrementPerformanceTest(100000, new TimeSpan(0, 0, 1).Ticks);
        }

        private void IncrementPerformanceTest(int incrementAmount, long limit)
        {
            var start = DateTime.UtcNow.Ticks;

            for (var i = 0; i < incrementAmount; i++)
            {
                this.rollingCount.Increment();
            }

            var end = DateTime.UtcNow.Ticks;
            var timeTaken = end - start;

            var result = this.rollingCount.Count;

            Assert.AreEqual(incrementAmount, result);
            Assert.IsTrue(timeTaken < limit);
        }
    }
}