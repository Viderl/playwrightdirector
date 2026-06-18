using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using playwrightbook.Cli;
using playwrightbook.Runtime;

namespace playwrightbook
{
    internal class Program
    {
        public const string VERSION = "0.0.3";

        static async Task<int> Main(string[] args)
        {
            // ── CLI 解析 ──────────────────────────────────────────────
            var opts = CliOptions.Parse(args);

            if (opts.ShowVersion)
            {
                CliOptions.PrintVersion();
                return 0;
            }

            if (opts.ShowHelp)
            {
                CliOptions.PrintHelp();
                return 0;
            }

            if (opts.HasError)
            {
                Console.Error.WriteLine($"參數錯誤：{opts.ErrorMessage}");
                CliOptions.PrintHelp();
                return 10;
            }

            // ── Playbook 路徑解析 ─────────────────────────────────────
            string? playbookFile = ResolvePlaybookPath(opts.PlaybookPath);
            if (playbookFile is null)
            {
                Console.Error.WriteLine("找不到 playbook 檔案。");
                CliOptions.PrintHelp();
                return 10;
            }

            // ── 輸出路徑初始化 ────────────────────────────────────────
            var outputPaths = new OutputPaths(opts.OutputPath);
            outputPaths.EnsureDirectoriesExist();

            // ── 設定依賴注入 ──────────────────────────────────────────
            var services = new ServiceCollection();
            var consoleFileLogger = new ConsoleFileLogger(outputPaths.LogFile);
            services.AddSingleton<ILogger>(consoleFileLogger);
            services.AddSingleton<LoggerAdapter>();
            services.AddSingleton(outputPaths);

            var serviceProvider = services.BuildServiceProvider();

            // ── 執行 ──────────────────────────────────────────────────
            try
            {
                var logger = serviceProvider.GetRequiredService<LoggerAdapter>();
                return await new PlaywrightRunner(outputPaths, logger).RunAsync(playbookFile);
            }
            finally
            {
                consoleFileLogger.Dispose();
                serviceProvider.Dispose();
            }
        }

        private static string? ResolvePlaybookPath(string? specifiedPath)
        {
            if (specifiedPath is not null)
            {
                // 先當絕對路徑，再當相對路徑
                if (File.Exists(specifiedPath)) return specifiedPath;
                var rel = Path.Combine(Environment.CurrentDirectory, specifiedPath);
                if (File.Exists(rel)) return rel;
                return null;
            }

            // 未指定時，尋找預設的 PlaybookTemplate.json
            var defaultPath = Path.Combine(Environment.CurrentDirectory, "PlaybookTemplate.json");
            return File.Exists(defaultPath) ? defaultPath : null;
        }
    }
}
