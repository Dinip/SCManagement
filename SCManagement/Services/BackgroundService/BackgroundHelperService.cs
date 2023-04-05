using Microsoft.AspNetCore.Identity.UI.Services;
using SCManagement.Services.PaymentService;

namespace SCManagement.Services.BackgroundService
{
    public class BackgroundHelperService : IBackgroundHelperService
    {
        private readonly IEmailSender _emailSender;
        private readonly SharedResourceService _sharedResource;
        private readonly BackgroundWorkerService _backgroundWorker;

        public BackgroundHelperService(IEmailSender emailSender, SharedResourceService sharedResource, BackgroundWorkerService backgroundWorker)
        {
            _emailSender = emailSender;
            _sharedResource = sharedResource;
            _backgroundWorker = backgroundWorker;
        }

        public void SendEmail(string email, string lang, string EmailName, Dictionary<string, string> values)
        {
            _backgroundWorker.Enqueue(async (_) =>
            {
                if (email.Contains("scmanagement")) return;

                var emailBody = _sharedResource.Get($"Email_{EmailName}", lang);

                foreach (var entry in values)
                {
                    emailBody = emailBody.Replace(entry.Key, entry.Value);
                }

                await _emailSender.SendEmailAsync(email, _sharedResource.Get($"Subject_{EmailName}", lang), emailBody);
            });
        }
    }
}
