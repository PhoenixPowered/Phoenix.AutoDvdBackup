
namespace Phoenix.AutoDvdBackup.CommandServices
{
    public interface ICommandService
    {
        bool CanProcess(string command);
        void Process(string command, string[] allCommands);
    }
}
