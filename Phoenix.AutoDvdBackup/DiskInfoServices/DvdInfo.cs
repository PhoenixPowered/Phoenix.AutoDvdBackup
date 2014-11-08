using System.IO;
using Serilog;

namespace Phoenix.AutoDvdBackup.DiskInfoServices
{
    public class DvdInfo : DiskInfoBase, IDiskInfoService
    {
        public DvdInfo(ILogger logger)
            : base(logger)
        { }

        public virtual bool CanHandle(string diskIdentifier)
        {
            return File.Exists(diskIdentifier + @"VIDEO_TS\VIDEO_TS.BUP");
        }

        public virtual DiskInfoData GetData(string diskIdentifier)
        {
            var driveInfo = new DriveInfo(diskIdentifier);
            var label = driveInfo.VolumeLabel;
            return !string.IsNullOrEmpty(label) ? new DiskInfoData(driveInfo.VolumeLabel, Logger) : null;
        }
    }
}
