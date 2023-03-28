namespace SCManagement.Services.NotificationService
{
    public interface IEmailNotificationService
    {
        public void SendEmail(string email, string lang, string EmailName, Dictionary<string, string> values);
    }
}
