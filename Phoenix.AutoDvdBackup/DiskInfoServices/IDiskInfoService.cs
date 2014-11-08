
namespace Phoenix.AutoDvdBackup.DiskInfoServices
{
    public interface IDiskInfoService
    {
        bool CanHandle(string diskIdentifier);
        DiskInfoData GetData(string diskIdentifier);
    }
}
