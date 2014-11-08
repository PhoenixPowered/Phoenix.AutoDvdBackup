using Ninject;
using Ninject.Activation;
using Phoenix.AutoDvdBackup.CommandServices;
using Phoenix.AutoDvdBackup.DiskInfoServices;
using Phoenix.AutoDvdBackup.FileSystemServices;
using Phoenix.AutoDvdBackup.NotificationServices;
using Phoenix.AutoDvdBackup.RippingServices;
using Serilog;
using Serilog.Events;
using System;
using System.Configuration;

namespace Phoenix.AutoDvdBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = ConfigureContainer();
            var logger = kernel.Get<ILogger>();

            if (args.Length == 0 || !args[0].Equals("elevating"))
            {
                logger.Information("==============================================");
                logger.Information("Application execution started");
            }

            try
            {
                var app = kernel.Get<IApplication>();

                app.Run(args);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An exception occurred while executing");
            }

            logger.Information("Application execution ended");
            logger.Information("==============================================");
            logger.Information(" ");
        }

        private static IKernel ConfigureContainer()
        {
            var kernel = new StandardKernel();

            kernel.Bind<ICommandService>().To<InstallCommand>();
            kernel.Bind<ICommandService>().To<UninstallCommand>();

            kernel.Bind<IDiskInfoService>().To<DvdXmlInfo>();
            kernel.Bind<IDiskInfoService>().To<DvdInfo>();

            kernel.Bind<IFileSystemService>().To<FileSystemService>();

            kernel.Bind<INotificationService>().To<PushAlot>();

            kernel.Bind<IRippingService>().To<MakeMkvRippingService>();

            kernel.Bind<CommandManager>().ToSelf();
            kernel.Bind<DiskInfoManager>().ToSelf();
            kernel.Bind<NotificationManager>().ToSelf();
            kernel.Bind<RippingManager>().ToSelf();

            kernel.Bind<IApplication>().To<Application>();

            kernel.Bind<ILogger>().ToMethod(ConfigureLogger).InSingletonScope();

            return kernel;
        }

        private static ILogger ConfigureLogger(IContext context)
        {
            var logFileLocation = ConfigurationManager.AppSettings["logFileLocation"];
            var logFileFormat = logFileLocation + "rip-{Date}.txt";

            return new LoggerConfiguration()
                .WriteTo.RollingFile(logFileFormat, LogEventLevel.Information, fileSizeLimitBytes: 1000000, retainedFileCountLimit:3)
                .WriteTo.ColoredConsole(LogEventLevel.Information)
                .CreateLogger();
        }
    }
}
