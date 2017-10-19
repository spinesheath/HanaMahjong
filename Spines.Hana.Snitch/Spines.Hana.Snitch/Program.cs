// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace Spines.Hana.Snitch
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      var context = new SnitchContext();
      Application.ApplicationExit += context.OnExit;
      Application.Run(context);
    }
  }
}