// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;

namespace Spines.Hana.Blame.Models.AccountViewModels
{
  public class ExternalLoginConfirmationViewModel
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string UserName { get; set; }
  }
}