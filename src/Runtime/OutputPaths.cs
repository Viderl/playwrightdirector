namespace playwrightbook.Runtime
{
    internal class OutputPaths
    {
        public static readonly string LOG_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
        public static readonly string SCREENSHOT_TIME_FORMAT = "yyyyMMddHHmmssfff";


        public string TimestampDir { get; }
        public string ScreenshotDir { get; }
        public string DownloadDir { get; }
        public string LogFile { get; }

        public OutputPaths(string? outputBase)
        {
            var stamp = DateTime.Now.ToString("yyyyMMddHHmm");
            var rootBase = string.IsNullOrEmpty(outputBase)
                ? Path.Combine(Environment.CurrentDirectory, "output")
                : (Path.IsPathRooted(outputBase)
                    ? outputBase
                    : Path.Combine(Environment.CurrentDirectory, outputBase));

            TimestampDir = Path.Combine(rootBase, stamp);
            ScreenshotDir = Path.Combine(TimestampDir, "screenshots");
            DownloadDir = Path.Combine(TimestampDir, "download");
            LogFile = Path.Combine(TimestampDir, "log.csv");
        }

        public void EnsureDirectoriesExist()
        {
            Directory.CreateDirectory(ScreenshotDir);
            Directory.CreateDirectory(DownloadDir);
            var logDir = Path.GetDirectoryName(LogFile);
            if (!string.IsNullOrEmpty(logDir))
                Directory.CreateDirectory(logDir);
        }

        /// <summary>
        /// 產生不重複的路徑（向後相容）。重複時將序號插入副檔名前。
        /// </summary>
        public string UniquePath(string originalFileName, bool isScreenshot)
        {
            var baseDir = isScreenshot ? ScreenshotDir : DownloadDir;
            var candidate = Path.Combine(baseDir, originalFileName);
            if (!File.Exists(candidate)) return candidate;

            var nameNoExt = Path.GetFileNameWithoutExtension(originalFileName);
            var ext = Path.GetExtension(originalFileName);
            for (int seq = 1; ; seq++)
            {
                candidate = Path.Combine(baseDir, $"{nameNoExt}_{seq}{ext}");
                if (!File.Exists(candidate)) return candidate;
            }
        }
    }
}
