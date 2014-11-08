using Serilog;
using System;

namespace Phoenix.AutoDvdBackup.DiskInfoServices
{
    public class DiskInfoManager
    {
        private readonly IDiskInfoService[] _services;
        private readonly ILogger _logger;

        public DiskInfoManager(IDiskInfoService[] services, ILogger logger)
        {
            _services = services;
            _logger = logger;
        }

        public DiskInfoData GetData(string diskIdentifier)
        {
            DiskInfoData data = null;

            foreach(var service in _services)
            {
                try
                {
                    if (service.CanHandle(diskIdentifier))
                    {
                        _logger.Information("Getting disk info using {0}", service.GetType().Name);

                        var result = service.GetData(diskIdentifier);

                        if (result != null)
                        {
                            data = result;
                            break;
                        }

                        _logger.Information("No disk info found, trying next service");
                    }
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, "Service {0} returned error: {1}", service.GetType().Name, ex.Message);
                }
            }

            if (data == null)
            {
                _logger.Warning("No disk info found using configured services");
            }

            return data;
        }
    }
}
