using Serilog;
using System.Linq;

namespace Phoenix.AutoDvdBackup.DiskInfoServices
{
    public class DiskInfoData
    {
        private readonly ILogger _logger;

        public DiskInfoData(string rawTitle, string year, ILogger logger)
        {
            _logger = logger;

            Title = CleanTitle(rawTitle);
            Year = year;
            FileName = string.Format("{0} ({1})", CleanTitle(rawTitle), year);
        }

        public DiskInfoData(string rawTitle, ILogger logger)
        {
            _logger = logger;

            Title = CleanTitle(rawTitle);
            FileName = CleanTitle(rawTitle);
        }

        public string Title { get; set; }
        public string Year { get; set; }
        public string FileName { get; set; }

        protected string CleanTitle(string title)
        {
            _logger.Information("Title to Be Cleaned: {0}", title);

            if (string.IsNullOrEmpty(title))
            {
                return null;
            }

            var firstIndex = title.IndexOf('[');
            if (firstIndex > 0)
            {
                title = title.Substring(0, firstIndex);
            }

            var cleanedTitle = string.Join(string.Empty, title.Where(x => char.IsLetterOrDigit(x) || char.IsWhiteSpace(x) || x == '_')).Trim();

            _logger.Information("Cleaned Title: {0}", cleanedTitle);

            return cleanedTitle;
        }

        public override string ToString()
        {
            return string.Format("{{ Title = \"{0}\"; Year = \"{1}\"; FileName = \"{2}\";}}", Title, Year, FileName);
        }
    }
}
