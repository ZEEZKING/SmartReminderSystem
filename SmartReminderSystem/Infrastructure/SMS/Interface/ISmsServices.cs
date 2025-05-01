namespace SmartReminderSystem.Infrastructure.SMS.Interface
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string to, string message);
    }
}
