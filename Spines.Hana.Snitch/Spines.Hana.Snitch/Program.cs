// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Hana.Snitch
{
  internal static class Program
  {
    [STAThread]
    private static void Main(string[] args)
    {
      new SingleInstanceManager().Run(args);
    }
  }
}