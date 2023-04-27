﻿using FluentEmail.Core;
using FluentEmail.MailKitSmtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;


namespace SCManagement.Services.EmailService
{
    public class EmailSenderMailtrap : IEmailSender
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Mailtrap email sender constructor (dev email)
        /// </summary>
        /// <param name="optionsAccessor"></param>
        /// <param name="logger"></param>
        public EmailSenderMailtrap(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSenderMailtrap> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

        /// <summary>
        /// Sends an email to a given email address with a given subject 
        /// and message content using mailtrap (for testing)
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
            string user = key.Split(":")[0];
            string password = key.Split(":")[1];
            var sender = new MailKitSender(new SmtpClientOptions { Server = "smtp.mailtrap.io", Port = 2525, RequiresAuthentication = true, User = user, Password = password });

            var email = Email
            .From("noreply@scmanagement.me", "SCManagement")
            .To(toEmail)
            .Subject(subject)
            .Body(message, true);

            email.Sender = sender;

            var response = await email.SendAsync();

            _logger.LogInformation(response.Successful
                                   ? $"Email to {toEmail} queued successfully!"
                                   : $"Failure Email to {toEmail}");
        }
    }
}
