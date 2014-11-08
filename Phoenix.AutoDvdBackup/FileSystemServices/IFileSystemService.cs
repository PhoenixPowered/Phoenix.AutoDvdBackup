
namespace Phoenix.AutoDvdBackup.FileSystemServices
{
    public interface IFileSystemService
    {
        string CreateOutputLocation(string directoryName);
        void MoveOutputToFinalLocation(string fileName, string outputLocation);
        void CleanUpOutputLocation(string outputLocation);
    }
}
