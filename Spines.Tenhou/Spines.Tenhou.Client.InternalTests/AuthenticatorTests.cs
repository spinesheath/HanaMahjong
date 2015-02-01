﻿/*
 *  Copyright (C) 2014  Johannes Heckl
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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