using Serilog;
using System;

namespace Phoenix.AutoDvdBackup.NotificationServices
{
    public class NotificationManager
    {
        private const string ErrorMessageFormat = "Processing for {0} failed with error {1}.";
        private const string SuccessMessageFormat = "Processing for {0} was successful.";

        private readonly INotificationService[] _services;
        private readonly ILogger _logger;

        public NotificationManager(INotificationService[] services, ILogger logger)
        {
            _services = services;
            _logger = logger;
        }

        public void SendErrorMessage(string movieTitle, string errorMessage)
        {
            SendMessage("Ripping Failure", string.Format(ErrorMessageFormat, movieTitle, errorMessage));
        }

        public void SendSuccessMessage(string movieTitle)
        {
            SendMessage("Ripping Success", string.Format(SuccessMessageFormat, movieTitle));
        }

        private void SendMessage(string subject, string message)
        {
            foreach(var service in _services)
            {
                try
                {
                    _logger.Information("Sending notification ({0}) via {1}", message, service.GetType().Name);
                    service.SendMessage(subject, message);
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, "Service {0} returned error: {1}", service.GetType().Name, ex.Message);
                }
            }
        }
    }
}
