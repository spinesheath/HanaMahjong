// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spines.Tenhou.Client.InternalTests
{
  [TestClass]
  public class UserNameTests
  {
    [TestMethod]
    public void TestUserName()
    {
      var n0 = new UserName("%E3%82%82%E3%81%A8%E3%81%A1%E3%82%83%E3%82%93");
      var n1 = new UserName("%E3%81%B5%E3%81%B2%E3%81%B2");
      var n2 = new UserName("%E4%B8%8A%E6%B5%B7%E4%B8%80%E4%B9%9D%E5%9B%9B%E4%B8%89");
      var n3 = new UserName("%73%70%69%6E%65%73");
      Assert.AreEqual("もとちゃん", n0.Name, "n0");
      Assert.AreEqual("ふひひ", n1.Name, "n1");
      Assert.AreEqual("上海一九四三", n2.Name, "n2");
      Assert.AreEqual("spines", n3.Name, "n3");
    }
  }
}