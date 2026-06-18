namespace playwrightbook.Runtime
{
    /// <summary>
    /// Tracks Playwright error count and classifies exceptions into exit codes.
    /// </summary>
    internal class ErrorPolicy
    {
        private int _errorCount;

        /// <summary>
        /// Classifies the exception.
        /// Returns 30 for System.IO errors (stop immediately).
        /// Returns 40 when Playwright error count exceeds 3.
        /// Returns null to continue to the next action.
        /// </summary>
        public int? Classify(Exception ex)
        {
            if (ex is IOException)
                return 30;

            _errorCount++;
            return _errorCount > 3 ? 40 : null;
        }
    }
}
