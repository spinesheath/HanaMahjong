using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spines.Utility.Test
{
    [TestClass]
    public class EnumerableExtensionTest
    {
        private readonly int[] _testArray000 = { 0, 0, 0 };
        private readonly int[] _testArray012 = { 0, 1, 2 };

        [TestMethod]
        public void TestAtLeastExact()
        {
            var b = _testArray000.AtLeast(_testArray000.Length);
            Assert.IsTrue(b);
        }

        [TestMethod]
        public void TestAtLeastTooFew()
        {
            var b = _testArray000.AtLeast(_testArray000.Length + 1);
            Assert.IsFalse(b);
        }

        [TestMethod]
        public void TestAtLeastLess()
        {
            var b = _testArray000.AtLeast(_testArray000.Length - 1);
            Assert.IsTrue(b);
        }

        [TestMethod]
        public void TestAtLeastNegative()
        {
            var b = _testArray000.AtLeast(-1);
            Assert.IsTrue(b);
        }

        [TestMethod]
        public void TestAtMostExact()
        {
            var b = _testArray000.AtMost(_testArray000.Length);
            Assert.IsTrue(b);
        }

        [TestMethod]
        public void TestAtMostTooMany()
        {
            var b = _testArray000.AtMost(_testArray000.Length - 1);
            Assert.IsFalse(b);
        }

        [TestMethod]
        public void TestAtMostMore()
        {
            var b = _testArray000.AtMost(_testArray000.Length + 1);
            Assert.IsTrue(b);
        }

        [TestMethod]
        public void TestAtMostNegative()
        {
            var b = _testArray000.AtMost(-1);
            Assert.IsFalse(b);
        }

        [TestMethod]
        public void TestHasDuplicatesFalse()
        {
            var b = _testArray012.HasDuplicates();
            Assert.IsFalse(b);
        }

        [TestMethod]
        public void TestHasDuplicatesTrue()
        {
            var b = _testArray000.HasDuplicates();
            Assert.IsTrue(b);
        }
    }
}
