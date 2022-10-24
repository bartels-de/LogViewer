using LogViewer.Constants;

using System.Text.RegularExpressions;

namespace LogViewer.Models
{
    public class LogDirectoryConfiguration
    {
        /// <summary>
        /// List of protocols to be retrieved <br/>
        /// 
        /// Could be empty if <see cref="FilePath"/> has been filled
        /// </summary>
        public string DirectoryPath { get; set; } = string.Empty;

        /// <summary>
        /// Log file to be fetched <br/>
        /// 
        /// Could be empty if <see cref="DirectoryPath"/> has been filled
        /// </summary>
        public string[] FilePaths { get; set; } = Array.Empty<string>();


        public RegexFormat[] RegexFormats { get; set; } = Array.Empty<RegexFormat>();

    }
}
