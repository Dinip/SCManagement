using FluentEmail.Core;
using FluentEmail.MailKitSmtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;


namespace Auth.Services {

    public class EmailSenderMailcatcher : IEmailSender {
        private readonly ILogger _logger;

        public EmailSenderMailcatcher(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSenderMailcatcher> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            await Execute(subject, message, toEmail);
        }

        public async Task Execute(string subject, string message, string toEmail)
        {
            var sender = new MailKitSender(new SmtpClientOptions { Server = "localhost", Port = 1025 });

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
