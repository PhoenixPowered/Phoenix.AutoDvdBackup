using Serilog;
using System.Linq;

namespace Phoenix.AutoDvdBackup.DiskInfoServices
{
    public abstract class DiskInfoBase
    {
        protected readonly ILogger Logger;

        protected DiskInfoBase(ILogger logger)
        {
            Logger = logger;
        }

        protected string CleanTitle(string title)
        {
            Logger.Information("Title to Be Cleaned: {0}", title);

            if(string.IsNullOrEmpty(title))
            {
                return null;
            }

            var firstIndex = title.IndexOf('[');
            title = title.Substring(0, firstIndex);

            var cleanedTitle = string.Join(string.Empty, title.Where(x => char.IsLetterOrDigit(x) || char.IsWhiteSpace(x) || x == '_'));

            Logger.Information("Cleaned Title: {0}", cleanedTitle);

            return cleanedTitle;
        }
    }
}
