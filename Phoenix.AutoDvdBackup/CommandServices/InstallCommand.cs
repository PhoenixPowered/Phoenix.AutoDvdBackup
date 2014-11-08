using System;
using Serilog;

namespace Phoenix.AutoDvdBackup.CommandServices
{
    public class InstallCommand : CommandBase
    {
        private readonly ILogger _logger;

        public InstallCommand(ILogger logger)
        {
            _logger = logger;
        }

        public override string Command
        {
            get { return "install"; }
        }

        public override void Process(string command, string[] allCommands)
        {
            UacHelper.RequestApplicationElevationIfRequired(_logger, allCommands);

            _logger.Information("Installing application");
            Installer.Install();

            _logger.Information("Install completed successfully.  Hit enter to exit");
            Console.ReadLine();
        }
    }
}
