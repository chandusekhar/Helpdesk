using Microsoft.AspNetCore.Identity.UI.Services;
using NETCore.MailKit.Core;

namespace Helpdesk.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly IEmailService _emailService;

        public EmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                await _emailService.SendAsync(email, subject, htmlMessage, true);
            }
            catch
            {
                // hide the error for now. we'll add logging in the future.
            }
        }
    }
}
