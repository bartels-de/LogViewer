namespace LogViewer.Models
{
    public readonly record struct LogFile
    {
        public required string Content { get; init; }
        public required string FullFileName { get; init; }
    }
}
