using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ArraySegment<string> FilePaths { get; set; } = ArraySegment<string>.Empty;
    }
}
