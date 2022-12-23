using FluentEmail.Core;
using FluentEmail.Mailgun;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;


namespace Auth.Services {

    public class EmailSenderMailgun : IEmailSender {
        private readonly ILogger _logger;

        public EmailSenderMailgun(IOptions<AuthMessageSenderOptions> optionsAccessor,
                           ILogger<EmailSenderMailgun> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(Options.AuthKey))
            {
                throw new Exception("Null AuthKey");
            }
            await Execute(Options.AuthKey, subject, message, toEmail);
        }

        public async Task Execute(string key, string subject, string message, string toEmail)
        {
            var sender = new MailgunSender(
              "scmanagement.me",
              key,
              MailGunRegion.EU
            );

            var email = Email
            .From("noreply@scmanagement.me", "SCManagement")
            .To(toEmail)
            .Subject(subject)
            .Body(message, true);

            email.Sender = sender;

            var response = await email.SendAsync();

            Console.WriteLine(response);

            _logger.LogInformation(response.Successful
                                   ? $"Email to {toEmail} queued successfully!"
                                   : $"Failure Email to {toEmail}");
        }
    }


}
