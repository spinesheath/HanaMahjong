/*
 *  Copyright (C) 2015  Johannes Heckl
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
