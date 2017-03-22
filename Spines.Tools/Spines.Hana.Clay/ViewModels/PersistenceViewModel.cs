// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;

namespace Spines.Hana.Clay.ViewModels
{
  internal class PersistenceViewModel
  {
    public ICommand Save { get; }

    public ICommand Open { get; }

    public ICommand SaveAs { get; }

    public ICommand New { get; }
  }
}