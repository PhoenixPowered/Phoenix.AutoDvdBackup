using Phoenix.AutoDvdBackup.CommandServices;
using Phoenix.AutoDvdBackup.DiskInfoServices;
using Phoenix.AutoDvdBackup.FileSystemServices;
using Phoenix.AutoDvdBackup.NotificationServices;
using Phoenix.AutoDvdBackup.RippingServices;
using Serilog;
using System;
using System.Threading;

namespace Phoenix.AutoDvdBackup
{
    public class Application : IApplication
    {
        private readonly CommandManager _commandManager;
        private readonly DiskInfoManager _diskInfoManager;
        private readonly IFileSystemService _fileSystemService;
        private readonly NotificationManager _notificationManager;
        private readonly RippingManager _rippingManager;
        private readonly ILogger _logger;

        public Application(
            CommandManager commandManager, 
            DiskInfoManager diskInfoManager, 
            IFileSystemService fileSystemService, 
            RippingManager rippingManager,
            NotificationManager notificationManager, 
            ILogger logger)
        {
            _commandManager = commandManager;
            _diskInfoManager = diskInfoManager;
            _fileSystemService = fileSystemService;
            _rippingManager = rippingManager;
            _notificationManager = notificationManager;
            _logger = logger;
        }

        public void Run(string[] args)
        {
            if(args.Length == 0)
            {
                throw new InvalidOperationException("No command or drive identifier was supplied.");
            }

            if(_commandManager.Process(args))
            {
                // if we were able to process a command directly, we are done.
                // otherwise we are attempting to rip a disk.
                _logger.Information("Processed commands successfully. Commands: {0}", args);

                return;
            }

            // get the drive letter that was passed  in.
            string diskIdentifier = args[0];

            _logger.Information("Starting rip process for {0}", diskIdentifier);

            DiskInfoData diskInfo = null;

            try
            {
                Thread.Sleep(5000);
                diskInfo = _diskInfoManager.GetData(diskIdentifier);

                var outputLocation = _fileSystemService.CreateOutputLocation(diskInfo.FileName);

                _rippingManager.Rip(diskIdentifier, outputLocation);

                _logger.Information("Taking a break... We'll continue in just a few seconds...");
                Thread.Sleep(5000);

                _fileSystemService.MoveOutputToFinalLocation(diskInfo.FileName, outputLocation);

                _logger.Information("Taking another break... We'll continue in just a few seconds...");
                Thread.Sleep(5000);

                _fileSystemService.CleanUpOutputLocation(outputLocation);

                _logger.Information("Ejecting disk");
                DriveTools.OpenDrive();
            }
            catch(Exception ex)
            {
                var movieTitle = (diskInfo == null) ? "Unknown" : diskInfo.Title;
                _notificationManager.SendErrorMessage(movieTitle, ex.Message);

                throw;
            }

            _notificationManager.SendSuccessMessage(diskInfo.Title);
        }
    }
}
