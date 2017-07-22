// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Spines.Hana.Blame.Models.ManageViewModels
{
  public class ConfigureTwoFactorViewModel
  {
    public string SelectedProvider { get; set; }

    public ICollection<SelectListItem> Providers { get; set; }
  }
}