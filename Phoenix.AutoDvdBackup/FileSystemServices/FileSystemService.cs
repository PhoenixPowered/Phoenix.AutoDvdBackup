using Serilog;
using System;
using System.Configuration;
using System.IO;

namespace Phoenix.AutoDvdBackup.FileSystemServices
{
    public class FileSystemService : IFileSystemService
    {
        private readonly string _tempOutputLocation;
        private readonly string _finalOutputLocation;
        private readonly ILogger _logger;

        public FileSystemService(ILogger logger)
        {
            _logger = logger;

            _tempOutputLocation = ConfigurationManager.AppSettings["tempDirectory"];
            _finalOutputLocation = ConfigurationManager.AppSettings["finalDirectory"];
        }

        public string CreateOutputLocation(string directoryName)
        {
            var outputLocation = _tempOutputLocation + directoryName;

            _logger.Information("Creating output location: {0}", outputLocation);

            if (Directory.Exists(outputLocation))
            {
                _logger.Information("Found existing directory, removing it.");
                Directory.Delete(outputLocation, true);
            }

            Directory.CreateDirectory(outputLocation);

            return outputLocation;
        }

        public void MoveOutputToFinalLocation(string fileName, string outputLocation)
        {
            var files = Directory.GetFiles(outputLocation, "*.mkv");

            if (files.Length == 1)
            {
                var finalPath = _finalOutputLocation + fileName + ".mkv";

                _logger.Information("Found {0} and moving to {1}", files[0], finalPath);

                File.Move(files[0], finalPath);
            }

            if (files.Length > 1)
            {
                throw new InvalidOperationException("Rip produced more videos than configured.");
            }

            if (files.Length < 1)
            {
                throw new InvalidOperationException("Rip failed to produce video. Check rip log.");
            }
        }

        public void CleanUpOutputLocation(string outputLocation)
        {
            if (Directory.Exists(outputLocation))
            {
                _logger.Information("Cleaning up temp directory and removing {0}", outputLocation);
                Directory.Delete(outputLocation, true);
            }
        }
    }
}
