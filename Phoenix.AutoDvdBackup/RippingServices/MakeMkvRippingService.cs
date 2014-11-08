using Serilog;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace Phoenix.AutoDvdBackup.RippingServices
{
    public class MakeMkvRippingService : IRippingService
    {
        private const string MakeMkvCmdLine = "--robot --messages \"{0}\\MKVrip.txt\" --decrypt --minlength={1} mkv disc:{2} 0 \"{0}\"";

        private readonly string _makeMkvLocation;
        private readonly string _minTitleLength;
        private readonly ILogger _logger;

        public MakeMkvRippingService(ILogger logger)
        {
            _minTitleLength = ConfigurationManager.AppSettings["makemkv.minTitleLength"];
            _makeMkvLocation = ConfigurationManager.AppSettings["makemkv.location"];
            _logger = logger;
        }

        public bool CanRip(string diskIdentifier)
        {
            return File.Exists(diskIdentifier + @"VIDEO_TS\VIDEO_TS.BUP");
        }

        public void Rip(string diskIdentifier, string outputLocation)
        {
            var driveId = GetDriveId(diskIdentifier, _makeMkvLocation);
            RipDisk(outputLocation, driveId, _minTitleLength, _makeMkvLocation);
        }

        private void RipDisk(string ripLocation, string diskIdentifier, string minLength, string makeMkvLocation)
        {
            string parameters = string.Format(MakeMkvCmdLine, ripLocation, minLength, diskIdentifier);
            _logger.Information("Make Mkv Parameters: {0}", parameters);

            var psi = new ProcessStartInfo {FileName = makeMkvLocation, Arguments = parameters, UseShellExecute = false};

            var process = new Process {StartInfo = psi};

            _logger.Information("Starting Rip");

            process.Start();
            process.WaitForExit();

            _logger.Information("Ending Rip");
        }

        private static string GetDriveId(string diskIdentifier, string makeMkvLocation)
        {
            var driveInfo = new DriveInfo(diskIdentifier);

            var psi = new ProcessStartInfo
            {
                FileName = makeMkvLocation,
                Arguments = "-r --cache=1 info disc:9999",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = new Process {StartInfo = psi};
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string driveId = null;

            foreach (var line in lines)
            {
                var lineParts = line.Split(',');
                if (lineParts.Length > 6 && driveInfo.VolumeLabel == lineParts[5].Replace("\"", string.Empty))
                {
                    driveId = lineParts[0].Substring(lineParts[0].IndexOf(":") + 1);
                    break;
                }
            }

            return driveId;
        }
    }
}
