// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Spines.Hana.Blame.Services
{
  public class AuthMessageSender : IEmailSender, ISmsSender
  {
    public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
    {
      _options = optionsAccessor.Value;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
      // Plug in your email service here to send an email.
      return Execute(_options.SendGridKey, subject, message, email);
    }

    private Task Execute(string apiKey, string subject, string message, string email)
    {
      var client = new SendGridClient(apiKey);
      var msg = new SendGridMessage
      {
        From = new EmailAddress(_options.EmailFrom, _options.EmailSenderName),
        Subject = subject,
        PlainTextContent = message,
        HtmlContent = message
      };
      msg.AddTo(new EmailAddress(email));
      return client.SendEmailAsync(msg);
    }

    public Task SendSmsAsync(string number, string message)
    {
      throw new NotImplementedException();
    }

    private readonly AuthMessageSenderOptions _options;
  }
}