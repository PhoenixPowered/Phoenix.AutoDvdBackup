using System;

namespace Phoenix.AutoDvdBackup.CommandServices
{
    public abstract class CommandBase : ICommandService
    {
        public abstract string Command { get; }

        public virtual bool CanProcess(string command)
        {
            return Command.Equals(command, StringComparison.CurrentCultureIgnoreCase);
        }

        public abstract void Process(string command, string[] allCommands);
    }
}
