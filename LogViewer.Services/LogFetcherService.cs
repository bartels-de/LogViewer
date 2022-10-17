using LogViewer.Models;
using System.Runtime.CompilerServices;

namespace LogViewer.Services
{
    internal class LogFetcherService
    {
        /// <summary>
        /// Tries to fetch logs for the given <see cref="LogDirectoryConfiguration"/>
        /// </summary>
        /// <param name="configuration"><see cref="LogDirectoryConfiguration"/></param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns></returns>
        internal async IAsyncEnumerable<LogFile> GetLogsAsync(
            LogDirectoryConfiguration configuration, 
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (ConfigurationIsNotValid(configuration) || DirectoryDoesNotExists(configuration))
            {
                //TODO: Add logging or throw an exception
                yield break;
            }

            //We only want to aim for .log files
            var files = Directory.GetFiles(configuration.DirectoryPath)
                .Where(file => HasValidEnding(file)); //valid ending means that the file has to end with .log

            if (FilePathIsGiven(configuration))
            {
                files = files.Where(file => FileExistsInGivenFilePaths(configuration, file));
            }

            foreach (var file in files)
            {
                var contentOfTheFile = await File.ReadAllTextAsync(file, cancellationToken);

                yield return new LogFile
                {
                    FullFileName = file,
                    Content = contentOfTheFile
                };
            }
        }

        private static bool HasValidEnding(string file)
        {
            return file.EndsWith(".log");
        }

        private static bool FileExistsInGivenFilePaths(LogDirectoryConfiguration configuration, string file)
        {
            return configuration.FilePaths.Contains(file);
        }

        private static bool FilePathIsGiven(LogDirectoryConfiguration configuration)
        {
            return FilePathsAreValid(configuration);
        }

        private static bool ConfigurationIsNotValid(LogDirectoryConfiguration configuration)
        {
            return string.IsNullOrWhiteSpace(configuration.DirectoryPath) && !FilePathsAreValid(configuration);
        }

        private static bool DirectoryDoesNotExists(LogDirectoryConfiguration configuration)
        {
            return !Directory.Exists(configuration.DirectoryPath);
        }
        private static bool FilePathsAreValid(LogDirectoryConfiguration configuration)
        {
            return configuration.FilePaths.Any();
        }

    }
}
