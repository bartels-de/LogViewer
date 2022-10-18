using LogViewer.Constants;
using LogViewer.Models;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LogViewer.Services
{
    internal class LogFetcherService
    {
        internal async Task GetContentWithFormat(
            LogDirectoryConfiguration configuration, 
            IAsyncEnumerator<LogFile> filesEnumerator)
        {
            if(configuration.RegexFormats.Count != 4)
            {
                //something went wrong, the count must be four
                return;
            }

            var frontendContent = new List<FrontendContent>();

            while (await filesEnumerator.MoveNextAsync())
            {
                var currentFile = filesEnumerator.Current;

                foreach (var regexFormat in configuration.RegexFormats)
                {
                    var regexString = regexFormat switch
                    {
                        RegexFormat.Date => RegexConstants.Date,
                        RegexFormat.LogLevel => RegexConstants.LogLevel,
                        RegexFormat.ClassName => RegexConstants.ClassName,
                        RegexFormat.ThreadId => RegexConstants.ThreadId,
                        _ => throw new NotImplementedException(),
                    };

                    var regex = new Regex(regexString);
                    var firstSplitArray = regex.Split(currentFile.Content);

                    frontendContent.Add(new FrontendContent
                    {
                        Content = firstSplitArray[0],
                        Format = regexFormat
                    });
                }
            }

        }


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
