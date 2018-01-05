// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Spines.Hana.Blame.Data
{
  internal struct RoleNames
  {
    public const string Admin = "Admin";
    public const string CommonUser = "CommonUser";

    public static IEnumerable<string> All()
    {
      return typeof(RoleNames).GetFields(BindingFlags.Public | BindingFlags.Static).Select(f => f.GetValue(null)).OfType<string>();
    }
  }
}