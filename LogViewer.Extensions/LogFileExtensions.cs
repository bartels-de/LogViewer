using LogViewer.Models;

namespace LogViewer.Extensions
{
    public static class LogFileExtensions
    {
        public static async Task<LogFile> ToLogFileAsync(this string filePath)
        {
            var file = new LogFile
            {
                Content = await File.ReadAllTextAsync(filePath),
                FullFileName = Path.GetFileName(filePath)
            };

            return file;
        }
    }
}
