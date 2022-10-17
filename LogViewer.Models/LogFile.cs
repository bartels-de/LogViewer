using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Models
{
    public class LogFile
    {
        public required string Content { get; set; }
        public required string FullFileName { get; set; }
    }
}
