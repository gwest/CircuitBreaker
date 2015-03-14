namespace CircuitBreakerTests
{
    using CircuitBreaker;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MultipleMemoryCacheRollingCountTests
    {
        private IRollingCount rollingCount1;
        private IRollingCount rollingCount2;

        [TestInitialize]
        public void SetupTests()
        {
            this.rollingCount1 = new MemoryCacheRollingCount("test1");
            this.rollingCount2 = new MemoryCacheRollingCount("test2");
        }

        [TestMethod]
        public void CountReturns0AfterInstantiation()
        {
            Assert.AreEqual(0, this.rollingCount1.Count);
            Assert.AreEqual(0, this.rollingCount2.Count);
        }

        [TestMethod]
        public void IncrementAdds1ToOneRollingCountButNotTheOther()
        {
            this.rollingCount1.Increment();

            Assert.AreEqual(1, this.rollingCount1.Count);
            Assert.AreEqual(0, this.rollingCount2.Count);
        }

        [TestMethod]
        public void ResetClearsOneRollingCountButNotTheOther()
        {
            this.rollingCount1.Increment();
            this.rollingCount2.Increment();

            this.rollingCount1.Reset();

            Assert.AreEqual(0, this.rollingCount1.Count);
            Assert.AreEqual(1, this.rollingCount2.Count);
        }
    }
}