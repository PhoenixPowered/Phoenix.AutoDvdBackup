using Newtonsoft.Json;
using System.Configuration;
using System.Net;

namespace Phoenix.AutoDvdBackup.NotificationServices
{
    public class PushAlot : INotificationService
    {
        private readonly string _apiUrl;
        private readonly string _authToken;

        public PushAlot()
        {
            _apiUrl = ConfigurationManager.AppSettings["pushalot.url"];
            _authToken = ConfigurationManager.AppSettings["pushalot.token"];
        }

        public void SendMessage(string subject, string message)
        {
            var messageObject = new
            {
                AuthorizationToken = _authToken,
                Body = message,
                Title = subject
            };

            var messageJson = JsonConvert.SerializeObject(messageObject);

            using(var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.UploadString(_apiUrl, messageJson);
            }
        }
    }
}
