// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spines.Hana.Clay.ViewModels
{
  internal class UkeIreResult
  {
    public UkeIreResult(IEnumerable<UkeIreViewModel> results)
    {
      UkeIre = new ObservableCollection<UkeIreViewModel>(results);
    }

    public ICollection<UkeIreViewModel> UkeIre { get; }
  }
}