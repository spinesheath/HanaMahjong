// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace Spines.Hana.Snitch
{
  /// <summary>
  /// Makes sure the application only runs a single instance.
  /// </summary>
  internal class SingleInstanceManager : WindowsFormsApplicationBase
  {
    public SingleInstanceManager()
    {
      IsSingleInstance = true;
    }

    protected override bool OnStartup(StartupEventArgs eventArgs)
    {
      var context = new SnitchContext();
      Application.ApplicationExit += context.OnExit;
      Application.Run(context);
      return false;
    }
  }
}