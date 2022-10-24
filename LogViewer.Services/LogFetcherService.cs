using LogViewer.Constants;
using LogViewer.Extensions;
using LogViewer.Models;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LogViewer.Services
{
    public class LogFetcherService
    {
        public async Task<List<FrontendContent>> GetContentsWithFormatAsync(LogDirectoryConfiguration configuration)
        {
            if(configuration.RegexFormats.Length != 4)
            {
                //something went wrong, the count must be four
                return new List<FrontendContent>();
            }

            if(!Directory.Exists(configuration.DirectoryPath))
            {
                //Directory does not exist
                return new List<FrontendContent>();
            }

            var files = Directory.GetFiles(configuration.DirectoryPath);

            if(configuration.FilePaths.Any())
                files = files
                    .Where(file => configuration.FilePaths
                        .Contains(file))
                    .ToArray();

            var frontendContent = new List<FrontendContent>();

            foreach(var file in files)
            {
                var currentFile = await file.ToLogFileAsync();

                var regex = new Regex(RegexConstants.Date);
                var matches = regex.Matches(currentFile.Content)
                    .Where(x => x.Success)
                    .ToArray();

                for (int i = 0; i < matches.Length; i++)
                {
                    var match = matches[i];

                    if (match is null)
                        continue;

                    frontendContent.Add(new FrontendContent
                    {
                        Format = RegexFormat.Date,
                        LogName = currentFile.FullFileName,
                        Content = match.Value,
                        Index = i
                    });

                    foreach (var regexFormat in configuration.RegexFormats)
                    {
                        if (regexFormat is RegexFormat.Date)
                            continue;

                        string? subtext = currentFile.Content[match.Index..];

                        var regexAsString = regexFormat switch
                        {
                            RegexFormat.LogLevel => RegexConstants.LogLevel,
                            RegexFormat.ClassName => RegexConstants.ClassName,
                            RegexFormat.ThreadId => RegexConstants.ThreadId,
                            RegexFormat.Date => throw new NotImplementedException(),
                            _ => throw new NotImplementedException()
                        };

                        var subStringRegex = new Regex(regexAsString);
                        var subStringMatch = subStringRegex.Match(subtext);

                        frontendContent.Add(new FrontendContent
                        {
                            Format = regexFormat,
                            LogName = currentFile.FullFileName,
                            Content = subStringMatch.Value,
                            Index = i
                        });
                    }
                }
            }

            return frontendContent;
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
