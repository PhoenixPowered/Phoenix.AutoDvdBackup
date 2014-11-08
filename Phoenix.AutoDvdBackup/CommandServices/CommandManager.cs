
using System.Linq;

namespace Phoenix.AutoDvdBackup.CommandServices
{
    public class CommandManager
    {
        private readonly ICommandService[] _services;

        public CommandManager(ICommandService[] services)
        {
            _services = services;
        }

        public bool Process(string[] commands)
        {
            bool commandsProcessed = false;

            foreach (var command in commands)
            {
                var service = _services.FirstOrDefault(x => x.CanProcess(command));

                if (service == null)
                {
                    continue;
                }

                service.Process(command, commands);
                commandsProcessed = true;
            }

            return commandsProcessed;
        }
    }
}
