// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Hana.Blame.Services
{
  public class AuthMessageSenderOptions
  {
    public string SendGridUser { get; set; }
    public string SendGridKey { get; set; }
    public string EmailFrom { get; set; }
    public string EmailSenderName { get; set; }
  }
}