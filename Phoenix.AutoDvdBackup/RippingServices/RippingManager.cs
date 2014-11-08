using Serilog;
using System;
using System.Linq;

namespace Phoenix.AutoDvdBackup.RippingServices
{
    public class RippingManager
    {
        private readonly IRippingService[] _services;
        private readonly ILogger _logger;

        public RippingManager(IRippingService[] services, ILogger logger)
        {
            _services = services;
            _logger = logger;
        }

        public void Rip(string diskIdentifier, string outputLocation)
        {
            var service = _services.FirstOrDefault(x => x.CanRip(diskIdentifier));

            if(service == null)
            {
                throw new ArgumentException(string.Format("A ripping service can not be located for the provided diskIdentifier. ({0})", diskIdentifier), "diskIdentifier");
            }

            _logger.Information("Attempting to rip disk in {0} to {1} via {2}", diskIdentifier, outputLocation, service.GetType().Name);
            service.Rip(diskIdentifier, outputLocation);
        }
    }
}
