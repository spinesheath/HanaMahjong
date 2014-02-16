using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spines.Utility.Test
{
    [TestClass]
    public class NaturalEnumTest
    {
        [TestMethod]
        public void TestDefinedValues()
        {
            var d = NaturalEnum012.DefinedValues;
            Assert.AreEqual(3, d.Count);
            Assert.IsTrue(d.Contains(NaturalEnum012.Value0));
            Assert.IsTrue(d.Contains(NaturalEnum012.Value1));
            Assert.IsTrue(d.Contains(NaturalEnum012.Value2));
        }

        [TestMethod]
        [ExpectedException(typeof(InitializationException), "Duplicate values in natural enumeration.")]
        public void TestDefinedDuplicate()
        {
            var d = NaturalEnum001.DefinedValues;
            Assert.IsNotNull(d);
        }

        [TestMethod]
        [ExpectedException(typeof(InitializationException), "Not consecutive values in natural enumeration.")]
        public void TestDefinedNotConsecutive()
        {
            var d = NaturalEnum023.DefinedValues;
            Assert.IsNotNull(d);
        }
    }
}