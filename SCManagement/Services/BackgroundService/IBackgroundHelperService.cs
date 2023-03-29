namespace SCManagement.Services.BackgroundService
{
    public interface IBackgroundHelperService
    {
        public void SendEmail(string email, string lang, string EmailName, Dictionary<string, string> values);
    }
}
