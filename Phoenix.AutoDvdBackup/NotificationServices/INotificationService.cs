
namespace Phoenix.AutoDvdBackup.NotificationServices
{
    public interface INotificationService
    {
        void SendMessage(string subject, string message);
    }
}
