using Microsoft.AspNetCore.Identity.UI.Services;

namespace SCManagement.Services.BackgroundService
{
    public class BackgroundHelperService : IBackgroundHelperService
    {
        private readonly IEmailSender _emailSender;
        private readonly SharedResourceService _sharedResource;
        private readonly BackgroundWorkerService _backgroundWorker;

        /// <summary>
        /// Background helper service constructor
        /// </summary>
        /// <param name="emailSender"></param>
        /// <param name="sharedResource"></param>
        /// <param name="backgroundWorker"></param>
        public BackgroundHelperService(IEmailSender emailSender, SharedResourceService sharedResource, BackgroundWorkerService backgroundWorker)
        {
            _emailSender = emailSender;
            _sharedResource = sharedResource;
            _backgroundWorker = backgroundWorker;
        }


        /// <summary>
        /// Enqueues the email to be sent in the background
        /// </summary>
        /// <param name="email"></param>
        /// <param name="lang"></param>
        /// <param name="EmailName"></param>
        /// <param name="values"></param>
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
