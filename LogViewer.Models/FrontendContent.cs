using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Models
{
    public readonly record struct FrontendContent
    {
        public RegexFormat Format { get; init; }
        public string Content { get; init; }
        public string LogName { get; init; }
        public int Index { get; init; }
    }
}
