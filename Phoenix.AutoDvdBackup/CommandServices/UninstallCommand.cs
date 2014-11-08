using Serilog;
using System;

namespace Phoenix.AutoDvdBackup.CommandServices
{
    public class UninstallCommand : CommandBase
    {
        private readonly ILogger _logger;

        public UninstallCommand(ILogger logger)
        {
            _logger = logger;
        }

        public override string Command
        {
            get { return "uninstall"; }
        }

        public override void Process(string command, string[] allCommands)
        {
            UacHelper.RequestApplicationElevationIfRequired(_logger, allCommands);

            _logger.Information("Uninstalling application");
            Installer.Uninstall();

            _logger.Information("Uninstall completed successfully.  Hit enter to exit");
            Console.ReadLine();
        }
    }
}
