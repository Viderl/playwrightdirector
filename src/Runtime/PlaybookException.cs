namespace playwrightbook.Runtime
{
    internal class PlaybookException : Exception
    {
        public int ExitCode { get; }

        public PlaybookException(string message, int exitCode) : base(message)
        {
            ExitCode = exitCode;
        }
    }
}
