// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;

namespace Spines.Hana.Blame.Models.ManageViewModels
{
  public class AddPhoneNumberViewModel
  {
    [Required]
    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }
  }
}