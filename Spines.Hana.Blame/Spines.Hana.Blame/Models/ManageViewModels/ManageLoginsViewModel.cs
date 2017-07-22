// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Spines.Hana.Blame.Models.ManageViewModels
{
  public class ManageLoginsViewModel
  {
    public IList<UserLoginInfo> CurrentLogins { get; set; }

    public IList<AuthenticationDescription> OtherLogins { get; set; }
  }
}