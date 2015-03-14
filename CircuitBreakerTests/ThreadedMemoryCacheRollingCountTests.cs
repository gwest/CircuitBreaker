namespace CircuitBreakerTests
{
    using System;
    using System.Threading;

    using CircuitBreaker;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Osherove.ThreadTester;

    [TestClass]
    public class ThreadedMemoryCacheRollingCountTests
    {
        private IRollingCount rollingCount;

        [TestInitialize]
        public void SetupTests()
        {
            this.rollingCount = new MemoryCacheRollingCount("RollingCountThreadedTest");
        }

        [TestMethod]
        public void IncrementIn2ThreadsAdds2ToTheRollingCount()
        {
            var tt = new ThreadTester();

            tt.AddThreadAction(() => this.rollingCount.Increment());
            tt.AddThreadAction(() => this.rollingCount.Increment());
            tt.StartAllThreads(1000);

            Assert.AreEqual(2, this.rollingCount.Count);
        }

        [TestMethod]
        public void IncrementIn50ThreadsAdds50ToTheRollingCount()
        {
            var tt = new ThreadTester();

            for (var i = 0; i < 50; i++)
            {
                tt.AddThreadAction(() => this.rollingCount.Increment());
            }

            tt.StartAllThreads(1000);

            Assert.AreEqual(50, this.rollingCount.Count);
        }

        [TestMethod]
        public void Increment10TimesIn50ThreadsAdds500ToTheRollingCount()
        {
            var tt = new ThreadTester();

            for (var i = 0; i < 50; i++)
            {
                tt.AddThreadAction(this.Increment10Times);
            }

            tt.StartAllThreads(10000);

            Assert.AreEqual(500, this.rollingCount.Count);
        }

        private void Increment10Times()
        {
            for (var j = 0; j < 10; j++)
            {
                this.rollingCount.Increment();
                Thread.Sleep(new Random(j + 1).Next(100, 300));
            }
        }
    }
}