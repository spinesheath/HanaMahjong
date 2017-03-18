// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace Spines.Hana.Clay.Controls
{
  public class BusyIcon : Control
  {
    static BusyIcon()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIcon), new FrameworkPropertyMetadata(typeof(BusyIcon)));
    }
  }
}