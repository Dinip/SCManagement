using Microsoft.AspNetCore.Identity.UI.Services;
using SCManagement.Services.BackgroundService;

namespace SCManagement.Services.NotificationService
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly SharedResourceService _sharedResource;
        private readonly BackgroundWorkerService _backgroundWorker;

        public EmailNotificationService(IEmailSender emailSender, SharedResourceService sharedResource, BackgroundWorkerService backgroundWorker)
        {
            _emailSender = emailSender;
            _sharedResource = sharedResource;
            _backgroundWorker = backgroundWorker;
        }

        public void SendEmail(string email, string lang, string EmailName, Dictionary<string, string> values)
        {
            _backgroundWorker.Enqueue(async () =>
            {
                if (email.Contains("scmanagement")) return;

                string emailBody = _sharedResource.Get($"Email_{EmailName}", lang);

                foreach (KeyValuePair<string, string> entry in values)
                {
                    emailBody = emailBody.Replace(entry.Key, entry.Value);
                }

                await _emailSender.SendEmailAsync(email, _sharedResource.Get($"Subject_{EmailName}", lang), emailBody);
            });
        }
    }
}
