using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Constants
{
    internal static class RegexConstants
    {
        public const string Date = "\\d{4}-\\d{2}-\\d{2}\\s\\d{2}:\\d{2}:\\d{2}.\\d{4}";
        public const string ThreadId = "[\\d]";
        public const string LogLevel = "(TRACE|DEBUG|INFO|NOTICE|WARN|WARNING|ERROR|SEVERE|FATAL)";
        public const string ClassName = "\\[[^\\]]*\\D]";
    }
}
