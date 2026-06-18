using Microsoft.Extensions.Logging;

namespace playwrightbook.Runtime
{
    internal class ConsoleFileLogger : ILogger
    {
        private readonly StreamWriter _writer;
        private bool _disposed;

        public ConsoleFileLogger(string logFilePath)
        {
            var dir = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            _writer = new StreamWriter(logFilePath, append: true, System.Text.Encoding.UTF8)
            {
                AutoFlush = true
            };
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        /// <summary>
        /// 結構化日誌輸出: timestamp | step | action | level | img | message
        /// </summary>
        public void LogStructured(string step, string action, string level, string img, string message)
        {
            var timestamp = DateTime.Now.ToString(OutputPaths.LOG_TIME_FORMAT);
            var line = $"{timestamp} | {step} | {action} | {level} | {img} | {message}";

            _writer.WriteLine(line);

            if (level == "error")
            {
                var prev = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(line);
                Console.ForegroundColor = prev;
            }
            else
            {
                Console.WriteLine(line);
            }
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            var timestamp = DateTime.Now.ToString(OutputPaths.LOG_TIME_FORMAT);
            var levelStr = logLevel.ToString();
            var line = $"{timestamp} | {levelStr} | {message}";

            _writer.WriteLine(line);

            if (logLevel >= LogLevel.Error)
            {
                var prev = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(line);
                Console.ForegroundColor = prev;
            }
            else
            {
                Console.WriteLine(line);
            }

            if (exception is not null)
            {
                _writer.WriteLine(exception.ToString());
                if (logLevel >= LogLevel.Error)
                    Console.Error.WriteLine(exception.ToString());
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _writer.Flush();
            _writer.Dispose();
        }
    }

    internal class LoggerAdapter
    {
        private readonly ILogger _logger;
        private readonly ConsoleFileLogger? _consoleFileLogger;

        public LoggerAdapter(ILogger logger)
        {
            _logger = logger;
            _consoleFileLogger = logger as ConsoleFileLogger;
        }

        public void Info(string step, string action, string img, string message)
        {
            _consoleFileLogger?.LogStructured(step, action, "info", img, message);
        }

        public void Error(string step, string action, string img, string message)
        {
            _consoleFileLogger?.LogStructured(step, action, "error", img, message);
        }
    }
}
