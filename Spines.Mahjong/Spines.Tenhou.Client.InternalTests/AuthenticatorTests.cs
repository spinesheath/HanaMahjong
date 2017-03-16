// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spines.Tenhou.Client.InternalTests
{
  [TestClass]
  public class AuthenticatorTests
  {
    [TestMethod]
    public void TestTransform()
    {
      var t1 = Authenticator.Transform("20141229-cc32e3fd");
      Assert.AreEqual("20141229-3a2cc69f", t1, "AuthenticationString 1 was incorrectly transformed.");

      var t2 = Authenticator.Transform("20121027-1c7e80ca");
      Assert.AreEqual("20121027-3a20da9b", t2, "AuthenticationString 2 was incorrectly transformed.");
    }
  }
}